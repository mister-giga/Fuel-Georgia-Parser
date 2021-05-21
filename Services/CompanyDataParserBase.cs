using Fuel_Georgia_Parser.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Fuel_Georgia_Parser.Services
{
    abstract class CompanyDataParserBase
    {
        public abstract string CompanyKey { get; }
        public abstract string CompanyName { get; }
        public abstract string CompanyColor { get; }
        public abstract Task<Location[]> GetLocationsAsync();
        public abstract Task<Fuel[]> GetActiveFuelsAsync();

        readonly static char[] validChars;
        readonly static Dictionary<char, char> charMappings;
        static CompanyDataParserBase()
        {
            validChars = "abcdefghijklmnopqrstuvwxyz_0123456789".ToCharArray();
            charMappings = new Dictionary<char, char>
            {
                { 'ა', 'a' },
                { 'ბ', 'b' },
                { 'გ', 'g' },
                { 'დ', 'd' },
                { 'ე', 'e' },
                { 'ვ', 'v' },
                { 'ზ', 'z' },
                { 'თ', 't' },
                { 'ი', 'i' },
                { 'კ', 'k' },
                { 'ლ', 'l' },
                { 'მ', 'm' },
                { 'ნ', 'n' },
                { 'ო', 'o' },
                { 'პ', 'p' },
                { 'ჟ', 'j' },
                { 'რ', 'r' },
                { 'ს', 's' },
                { 'ტ', 't' },
                { 'უ', 'u' },
                { 'ფ', 'f' },
                { 'ქ', 'q' },
                { 'ღ', 'r' },
                { 'ყ', 'y' },
                { 'შ', 's' },
                { 'ჩ', 'c' },
                { 'ც', 'c' },
                { 'ძ', 'z' },
                { 'წ', 'w' },
                { 'ჭ', 'w' },
                { 'ხ', 'x' },
                { 'ჯ', 'j' },
                { 'ჰ', 'h' },
                { ' ', '_' }
            };
        }
        protected static string ConvertFuelNameToKey(string fuelName)
        {
            return new string(MapAndFilter(fuelName).ToArray());

            static IEnumerable<char> MapAndFilter(IEnumerable<char> chars)
            {
                foreach(var c in chars)
                {
                    if(charMappings.ContainsKey(c))
                    {
                        yield return charMappings[c];
                        continue;
                    }
                    var lowered = char.ToLower(c);
                    if(validChars.Contains(lowered))
                    {
                        yield return lowered;
                        continue;
                    }
                }
            }
        }

        protected static string StripHTML(string input)
        {
            return Regex.Replace(input, "<.*?>", String.Empty);
        }
    }
}
