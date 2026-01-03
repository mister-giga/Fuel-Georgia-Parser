using Fuel_Georgia_Parser.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Fuel_Georgia_Parser.Services
{
    class WissolDataParser : CompanyDataParserBase
    {
        public override string CompanyKey => "wissol";

        public override string CompanyName => "ვისოლი";

        public override string CompanyColor => "#00A651";

        public override async Task<Fuel[]> GetActiveFuelsAsync()
        {
            var json = await new HttpClient().GetStringAsync("https://api.wissol.ge/fuelpricehistory/?days=1");
            var response = System.Text.Json.JsonSerializer.Deserialize<FuelModel>(json);
            var fuels = response.Data.Select(o => new Fuel()
            {
                Name = o.Name,
                Key = ConvertFuelNameToKey(o.Name),
                Price = o.Series.Length > 0 ? o.Series[0].Value : 0
            }).Where(x => x.Price > 0).ToArray();

            return fuels;
        }

        public async override Task<Location[]> GetLocationsAsync()
        {
            var json = await new HttpClient().GetStringAsync("http://wissol.ge/adminarea/api/ajaxapi/map?lang=geo");
            var fuels = System.Text.Json.JsonSerializer.Deserialize<List<LocatonModel>>(json);

            return fuels.Select(x => new Location
            {
                Address = $"{x.address}, {x.city}",
                Lat = double.Parse(x.lat),
                Lng = double.Parse(x.lng)
            }).ToArray();
        }

        class FuelModel
        {
            [System.Text.Json.Serialization.JsonPropertyName("data")]
            public FuelData[] Data { get; set; }
        }

        class FuelData
        {
            [System.Text.Json.Serialization.JsonPropertyName("name")]
            public string Name { get; set; }
            [System.Text.Json.Serialization.JsonPropertyName("series")]
            public Series[] Series { get; set; }
        }
        
        class Series
        {
            [System.Text.Json.Serialization.JsonPropertyName("name")]
            public string Name { get; set; }
            [System.Text.Json.Serialization.JsonPropertyName("value")]
            public decimal Value { get; set; }
        }

        class LocatonModel
        {
            public string address { get; set; }
            public string lat { get; set; }
            public string lng { get; set; }
            public string city { get; set; }
        }
    }
}
