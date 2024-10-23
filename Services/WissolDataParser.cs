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
            var json = await new HttpClient().GetStringAsync("https://api.wissol.ge/FuelPrice");
            var fuels = System.Text.Json.JsonSerializer.Deserialize<List<FuelModel>>(json);

            return fuels.Select(x => new Fuel 
            {
                Name = x.fuel_name,
                Key = ConvertFuelNameToKey(x.fuel_name),
                Price = decimal.Parse(x.fuel_price)
            }).ToArray();
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
            [System.Text.Json.Serialization.JsonPropertyName("fuelType")]
            public string fuel_name { get; set; }

            [System.Text.Json.Serialization.JsonPropertyName("price")]
            public decimal fuel_price_decimal {get;set;}
            public string fuel_price => fuel_price_decimal.ToString(System.Globalization.CultureInfo.InvariantCulture);
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
