using Fuel_Georgia_Parser.Models;
using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace Fuel_Georgia_Parser.Services
{
    class LukoilDataParser : CompanyDataParserBase
    {
        public override string CompanyKey => "lukoil";

        public override string CompanyName => "ლუკოილი";

        public override string CompanyColor => "#DE1A22";

        public override async Task<Fuel[]> GetActiveFuelsAsync()
        {
            var url = "http://www.lukoil.ge/prices-history";
            var web = new HtmlWeb();
            var doc = await web.LoadFromWebAsync(url);

            var table = doc.DocumentNode.Descendants("table").First();
            var tbody = table.Descendants("tbody").First();
            var tr = tbody.Descendants("tr").Last();
            var tds = tr.Descendants("td").Skip(1).SkipLast(1).ToArray();
            var prices = tds.Select(x => decimal.Parse(x.InnerText.Trim())).ToArray();

            var thead = table.Descendants("thead").First();
            var tr_name = thead.Descendants("tr").First();
            var ths = tr_name.Descendants("th").Skip(1).SkipLast(1).ToArray();
            var names = ths.Select(x => x.InnerText.Trim()).ToArray();

            var fuels = names.Zip(prices).Select(x => (name: x.First, price: x.Second)).Where(x => x.price > 0).ToArray();


            return fuels.Select(x => new Fuel
            {
                Key = ConvertFuelNameToKey(GetFixedName(x.name)),
                Name = GetFixedName(x.name),
                Price = x.price
            }).ToArray();

            static string GetFixedName(string name) => name switch
            {
                "Super Ecto" => "ევრო სუპერი",
                "Premium Avangard" => "პრემიუმ ავანგარდი",
                "Euro Regular" => "ევრო რეგულარი",
                "Euro Diesel" => "ევრო დიზელი",
                _ => throw new Exception($"Unknown name: {name}")
            };
        }

        public override async Task<Location[]> GetLocationsAsync()
        {
            var xml = await new HttpClient().GetStringAsync("http://www.lukoil.ge/gmap/googlemap/xml.php");

            XmlReaderSettings settings = new XmlReaderSettings();
            settings.IgnoreWhitespace = true;

            var stream = new MemoryStream();
            var writer = new StreamWriter(stream);
            writer.Write(xml);
            writer.Flush();
            stream.Position = 0;

            List<Location> locations = new List<Location>();

            using(XmlReader reader = XmlReader.Create(stream, settings))
            {
                while(reader.Read())
                {
                    if(reader.Name == "marker")
                    {
                        locations.Add(new Location 
                        {
                            Lat = double.Parse(reader.GetAttribute("lat")),
                            Lng = double.Parse(reader.GetAttribute("lng")),
                            Address = $"{reader.GetAttribute("address")}, {reader.GetAttribute("district")}, {reader.GetAttribute("region")}"
                        });
                    }
                }
            }

            return locations.ToArray();
        }
    }
}
