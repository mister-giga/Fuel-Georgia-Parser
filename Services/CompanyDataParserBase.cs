using Fuel_Georgia_Parser.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fuel_Georgia_Parser.Services
{
    abstract class CompanyDataParserBase
    {
        public abstract string CompanyKey { get; }
        public abstract string CompanyName { get; }
        public abstract Task<Location[]> GetLocationsAsync();
        public abstract Task<Fuel[]> GetActiveFuelsAsync();
        public abstract Task<Dictionary<string, PricePoint>> GetFuelsPricePointsAsync();
    }
}
