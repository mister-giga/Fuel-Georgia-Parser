using Fuel_Georgia_Parser.Models;
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

        public override Task<Fuel[]> GetActiveFuelsAsync()
        {
            return Task.FromResult(new Fuel[]
            {
                new Fuel
                {
                    Key = "premiumi",
                    Change = 0.03m,
                    Name = "პრემიუმი",
                    Price = 3.01m
                },
                new Fuel
                {
                    Key = "superi",
                    Change = -0.01m,
                    Name = "სუპერი",
                    Price = 2.9m
                }
            });
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
