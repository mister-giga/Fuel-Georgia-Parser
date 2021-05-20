using Fuel_Georgia_Parser.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fuel_Georgia_Parser.Services
{
    class WissolDataParser : CompanyDataParserBase
    {
        public override string CompanyKey => "wissol";

        public override string CompanyName => "ვისოლი";

        public override Task<Fuel[]> GetActiveFuelsAsync()
        {
            return Task.FromResult(new Fuel[]
            {
                new Fuel
                {
                    Key = "eko_premiumi",
                    Change = -0.01m,
                    Name = "ეკო პრემიუმი",
                    Price = 3.11m
                },
                new Fuel
                {
                    Key = "eko_superi",
                    Change = 0.02m,
                    Name = "ეკო სუპერი",
                    Price = 3.01m
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
