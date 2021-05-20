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

        public override async Task<Fuel[]> GetActiveFuelsAsync()
        {
            var url = "https://www.rompetrol.ge";
            var web = new HtmlWeb();
            var doc = await web.LoadFromWebAsync(url);

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

        public override Task<Dictionary<string, PricePoint>> GetFuelsPricePointsAsync()
        {
            throw new NotImplementedException();
        }

        public override Task<Location[]> GetLocationsAsync()
        {
            throw new NotImplementedException();
        }
    }
}
