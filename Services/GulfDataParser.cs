using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using Fuel_Georgia_Parser.Models;
using HtmlAgilityPack;

namespace Fuel_Georgia_Parser.Services
{
    internal class GulfDataParser : CompanyDataParserBase
    {
        public override string CompanyKey => "gulf";

        public override string CompanyName => "გალფი";

        public override string CompanyColor => "#F2682B";

        public override async Task<Fuel[]> GetActiveFuelsAsync()
        {
            var url = "https://gulf.ge/";
            var web = new HtmlWeb();
            var doc = await web.LoadFromWebAsync(url);

            var fuels = doc.DocumentNode.Descendants("div").Where(x => x.HasClass("price_entry")).Select(x =>
            {
                return (
                    name: x.Descendants("div").First(x => x.HasClass("product_name")).InnerText.Trim(),
                    price: decimal.Parse(x.Descendants("div").First(x => x.HasClass("product_price")).InnerText.Trim())
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
            var json = await new HttpClient().GetStringAsync("https://gulf.ge/map/get_objects");
            var data = JsonSerializer.Deserialize<List<LocationModel>>(json).Where(x => x.type == "1");
            return data.Select(x => new Location
            {
                Address = StripHTML(x.description),
                Lat = double.Parse(x.latitude),
                Lng = double.Parse(x.longitude)
            }).ToArray();
        }

        private class LocationModel
        {
            public string type { get; }
            public string description { get; }
            public string latitude { get; }
            public string longitude { get; }
        }
    }
}