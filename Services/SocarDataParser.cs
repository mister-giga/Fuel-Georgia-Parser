using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Fuel_Georgia_Parser.Models;

namespace Fuel_Georgia_Parser.Services
{
    internal class SocarDataParser : CompanyDataParserBase
    {
        public override string CompanyKey => "socar";

        public override string CompanyName => "სოკარი";

        public override string CompanyColor => "#EC1529";

        public override async Task<Fuel[]> GetActiveFuelsAsync()
        {
            var url = "https://www.sgp.ge/ge/price";
            var web = new HtmlWeb();
            var doc = await web.LoadFromWebAsync(url);

            var table = doc.DocumentNode.Descendants("table").First();
            var names = table.Descendants("th").Skip(1).Select(x => x.InnerHtml.Trim()).ToArray();
            var values = table.Descendants("tr").First().Descendants("td").Skip(1)
                .Select(x => decimal.Parse(x.InnerText.Trim())).ToArray();

            return names.Zip(values).Select(x => new Fuel
            {
                Key = ConvertFuelNameToKey(x.First),
                Name = x.First,
                Price = x.Second
            }).Where(x => x.Price > 0).ToArray();
        }

        public override async Task<Location[]> GetLocationsAsync()
        {
            var json = await new HttpClient().GetStringAsync("https://sgp.ge/ge/map/getResult");
            var data = JsonSerializer.Deserialize<List<LocationModel>>(json);
            return data.Select(x => new Location
            {
                Address = StripHTML(x.text),
                Lat = double.Parse(x.lat),
                Lng = double.Parse(x.lon)
            }).ToArray();
        }

        private class LocationModel
        {
            [JsonPropertyName("lon")] public string lat { get; set; }

            [JsonPropertyName("lat")] public string lon { get; set; }

            public string text { get; set; }
        }
    }
}