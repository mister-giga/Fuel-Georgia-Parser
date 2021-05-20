using Fuel_Georgia_Parser.Models;
using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fuel_Georgia_Parser.Services
{
    class LukoilDataParser : CompanyDataParserBase
    {
        public override string CompanyKey => "lukoil";

        public override string CompanyName => "ლუკოილი";

        public override async Task<Fuel[]> GetActiveFuelsAsync()
        {
            var url = "http://www.lukoil.ge/?m=328";
            var web = new HtmlWeb();
            var doc = await web.LoadFromWebAsync(url);

            var fuels = doc.DocumentNode.Descendants("table").First(x => x.HasClass("pricetable")).Descendants("tr").Select(x =>
            {
                var tds = x.Descendants("td").ToArray();
                return (
                    name: tds[0].InnerText.Trim(),
                    price: decimal.Parse(tds[1].InnerText.Trim())
                );
            }).Where(x=>x.price>0).ToArray();


            return fuels.Select(x => new Fuel
            {
                Key = ConvertFuelNameToKey(x.name),
                Name = x.name,
                Price = x.price
            }).ToArray();
        }

        public override Task<Location[]> GetLocationsAsync()
        {
            throw new NotImplementedException();
        }
    }
}
