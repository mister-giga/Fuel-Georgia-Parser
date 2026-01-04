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
            var response = System.Text.Json.JsonSerializer.Deserialize<SocarPricesResponse>(json);
            var fuels = response.GetCurrentPrices.Results.Select(x => new Fuel
            {
                Key = GetKey(x),
                Name = x.FuelNameGeo,
                Price = x.FuelUnitPrice
            }).Where(x => x.Price > 0).ToArray();
            
            return fuels;

            static string GetKey(FuelPrice price) => price.FuelCode switch
            {
                "LPG" => "txevadi_airi",
                "CNG" => "bunebrivi_airi",
                _ => ConvertFuelNameToKey(price.FuelNameGeo),
            };
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

        public class SocarPricesResponse
        {
            [JsonPropertyName("GetCurrentPrices")]
            public GetCurrentPrices GetCurrentPrices { get; set; }
        }
        
        public class GetCurrentPrices
        {
            [JsonPropertyName("Status")]
            public string Status { get; set; }
            [JsonPropertyName("Message")]
            public string Message { get; set; }
            [JsonPropertyName("Results")]
            public List<FuelPrice> Results { get; set; }
        }
        
        public class FuelPrice
        {
            [JsonPropertyName("ActionDate")]
            public DateTime ActionDate { get; set; }
            [JsonPropertyName("FuelNameGeo")]
            public string FuelNameGeo { get; set; }
            [JsonPropertyName("FuelNameEng")]
            public string FuelNameEng { get; set; }
            [JsonPropertyName("FuelUnitPrice")]
            public decimal FuelUnitPrice { get; set; }
            [JsonPropertyName("FuelCode")]
            public string FuelCode { get; set; }
        }
    }
}
