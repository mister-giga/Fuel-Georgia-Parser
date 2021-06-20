using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using System.Xml;
using Fuel_Georgia_Parser.Models;

namespace Fuel_Georgia_Parser.Services
{
    internal class LukoilDataParser : CompanyDataParserBase
    {
        public override string CompanyKey => "lukoil";

        public override string CompanyName => "ლუკოილი";

        public override string CompanyColor => "#DE1A22";

        public override async Task<Fuel[]> GetActiveFuelsAsync()
        {
            var url = "http://www.lukoil.ge/?m=328";
            var web = new HtmlWeb();
            var doc = await web.LoadFromWebAsync(url);

            var fuels = doc.DocumentNode.Descendants("table").First(x => x.HasClass("pricetable")).Descendants("tr")
                .Select(x =>
                {
                    var tds = x.Descendants("td").ToArray();
                    return (
                        name: tds[0].InnerText.Trim(),
                        price: decimal.Parse(tds[1].InnerText.Trim())
                    );
                }).Where(x => x.price > 0).ToArray();


            return fuels.Select(x => new Fuel
            {
                Key = ConvertFuelNameToKey(x.name),
                Name = x.name,
                Price = x.price
            }).ToArray();
        }

        public override async Task<Location[]> GetLocationsAsync()
        {
            var xml = await new HttpClient().GetStringAsync("http://www.lukoil.ge/gmap/googlemap/xml.php");

            var settings = new XmlReaderSettings();
            settings.IgnoreWhitespace = true;

            var stream = new MemoryStream();
            var writer = new StreamWriter(stream);
            writer.Write(xml);
            writer.Flush();
            stream.Position = 0;

            var locations = new List<Location>();

            using (var reader = XmlReader.Create(stream, settings))
            {
                while (reader.Read())
                    if (reader.Name == "marker")
                        locations.Add(new Location
                        {
                            Lat = double.Parse(reader.GetAttribute("lat")),
                            Lng = double.Parse(reader.GetAttribute("lng")),
                            Address =
                                $"{reader.GetAttribute("address")}, {reader.GetAttribute("district")}, {reader.GetAttribute("region")}"
                        });
            }

            return locations.ToArray();
        }
    }
}