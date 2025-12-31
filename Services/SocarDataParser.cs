using Fuel_Georgia_Parser.Models;
using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Text.Json.Serialization;

namespace Fuel_Georgia_Parser.Services
{
    class SocarDataParser : CompanyDataParserBase
    {
        public override string CompanyKey => "socar";

        public override string CompanyName => "სოკარი";

        public override string CompanyColor => "#EC1529";

        public override async Task<Fuel[]> GetActiveFuelsAsync()
        {
            var url = "https://sgp.ge/sgp-backend/api/integration/info/current-prices";
            var json = await new HttpClient().GetStringAsync(url);
            var response = System.Text.Json.JsonSerializer.Deserialize<GetCurrentPricesResponse>(json);
            var fuels = response.Results.Select(x => new Fuel
            {
                Key = ConvertFuelNameToKey(x.FuelNameGeo),
                Name = x.FuelNameGeo,
                Price = x.FuelUnitPrice
            }).Where(x => x.Price > 0).ToArray();
            
            return fuels;
        }

        public override async Task<Location[]> GetLocationsAsync()
        {
            var json = await new HttpClient().GetStringAsync("https://sgp.ge/ge/map/getResult");
            var data = System.Text.Json.JsonSerializer.Deserialize<List<LocationModel>>(json);
            return data.Select(x => new Location
            {
                Address = StripHTML(x.text),
                Lat = double.Parse(x.lat),
                Lng = double.Parse(x.lon)
            }).ToArray();
        }

        class LocationModel
        {
            [JsonPropertyName("lon")]
            public string lat { get; set; }
            [JsonPropertyName("lat")]
            public string lon { get; set; }   
            public string text { get; set; }
        }
        
        public class GetCurrentPricesResponse
        {
            public string Status { get; set; }
        
            public string Message { get; set; }
        
            public List<FuelPrice> Results { get; set; }
        }
        
        public class FuelPrice
        {
            public DateTime ActionDate { get; set; }
        
            public string FuelNameGeo { get; set; }
        
            public string FuelNameEng { get; set; }
        
            public decimal FuelUnitPrice { get; set; }
        
            public string FuelCode { get; set; }
        }
    }
}
