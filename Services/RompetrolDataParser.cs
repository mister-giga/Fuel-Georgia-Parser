using Fuel_Georgia_Parser.Models;
using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Fuel_Georgia_Parser.Services
{
    class RompetrolDataParser : CompanyDataParserBase
    {
        public override string CompanyKey => "rompetrol";

        public override string CompanyName => "რომპეტროლი";

        public override string CompanyColor => "#FF9914";

        public override async Task<Fuel[]> GetActiveFuelsAsync()
        {
            var url = "https://www.rompetrol.ge";
            var httpClient = new HttpClient();
            httpClient.DefaultRequestHeaders.TryAddWithoutValidation("User-Agent", "Mozilla/5.0 (Macintosh; Intel Mac OS X 10_15_7) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/121.0.0.0 Safari/537.36");
            var html = await httpClient.GetStringAsync(url);
            var doc = new HtmlDocument();
            doc.LoadHtml(html);

            var fuels = doc.DocumentNode.Descendants("table").First().Descendants("tr").Skip(1).Select(x =>
            {
                var tds = x.Descendants("td").ToArray();
                return (
                    name: tds[0].InnerText.Trim(),
                    price: decimal.Parse(tds[1].InnerText.Trim())
                );
            }).ToArray();


            return fuels.Select(x => new Fuel 
            {
                Key = ConvertFuelNameToKey(x.name),
                Name = x.name,
                Price = x.price
            }).ToArray();
        }

        public override async Task<Location[]> GetLocationsAsync()
        {
            var json = await new HttpClient().GetStringAsync("https://www.rompetrol.ge/routeplanner/stations?language_id=1");
            var data = System.Text.Json.JsonSerializer.Deserialize<List<LocationModel>>(json);
            return data.Select(x => new Location
            {
                Lat = x.lat,
                Lng = x.lng,
                Address = $"{x.address}, {x.city}"
            }).ToArray();
        }

        class LocationModel
        {
            public double lat { get; set; }
            public double lng { get; set; }
            public string city { get; set; }
            public string address { get; set; }
        }
    }
}
