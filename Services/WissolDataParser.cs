using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using Fuel_Georgia_Parser.Models;

namespace Fuel_Georgia_Parser.Services
{
    internal class WissolDataParser : CompanyDataParserBase
    {
        public override string CompanyKey => "wissol";

        public override string CompanyName => "ვისოლი";

        public override string CompanyColor => "#00A651";

        public override async Task<Fuel[]> GetActiveFuelsAsync()
        {
            var json = await new HttpClient().GetStringAsync(
                "http://wissol.ge/adminarea/api/ajaxapi/get_fuel_prices?lang=geo");
            var fuels = JsonSerializer.Deserialize<List<FuelModel>>(json);

            return fuels.Select(x => new Fuel
            {
                Name = x.fuel_name,
                Key = ConvertFuelNameToKey(x.fuel_name),
                Price = decimal.Parse(x.fuel_price)
            }).ToArray();
        }

        public override async Task<Location[]> GetLocationsAsync()
        {
            var json = await new HttpClient().GetStringAsync("http://wissol.ge/adminarea/api/ajaxapi/map?lang=geo");
            var fuels = JsonSerializer.Deserialize<List<LocatonModel>>(json);

            return fuels.Select(x => new Location
            {
                Address = $"{x.address}, {x.city}",
                Lat = double.Parse(x.lat),
                Lng = double.Parse(x.lng)
            }).ToArray();
        }

        private class FuelModel
        {
            public string fuel_name { get; set; }
            public string fuel_price { get; set; }
        }

        private class LocatonModel
        {
            public string address { get; set; }
            public string lat { get; set; }
            public string lng { get; set; }
            public string city { get; set; }
        }
    }
}