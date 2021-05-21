using Fuel_Georgia_Parser.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Unicode;
using System.Threading.Tasks;

namespace Fuel_Georgia_Parser.Services
{
    public static class DataAccessOptions
    {
        public static readonly JsonSerializerOptions JsonSerializerOptions;
        public static string RootPath;

        static DataAccessOptions()
        {
            JsonSerializerOptions = new JsonSerializerOptions
            {
                WriteIndented = true,
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                Encoder = JavaScriptEncoder.Create(UnicodeRanges.All)
            };
        }
    }
    abstract class DataAccess<T> where T : class, new()
    {
        protected abstract string GetPath();
        public virtual T[] GetDefault() => Array.Empty<T>();
        public T[] Data
        {
            get
            {
                if(File.Exists(GetPath()))
                {
                    var json = File.ReadAllText(GetPath());
                    return JsonSerializer.Deserialize<T[]>(json, DataAccessOptions.JsonSerializerOptions);
                }
                else
                {
                    return GetDefault();
                }
            }
            set => File.WriteAllText(EnsureDirectory(GetPath()), JsonSerializer.Serialize(value, DataAccessOptions.JsonSerializerOptions));
        }

        static string EnsureDirectory(string path)
        {
            Directory.CreateDirectory(Path.GetDirectoryName(path));
            return path;
        }
    }

    class CompaniesDataAccess : DataAccess<Company>
    {
        protected override string GetPath() => Path.Combine(DataAccessOptions.RootPath, "companies.json");
    }

    class LocationsDataAccess : DataAccess<Location>
    {
        private readonly string companyKey;

        public LocationsDataAccess(string companyKey)
        {
            this.companyKey = companyKey;
        }
        protected override string GetPath() => Path.Combine(DataAccessOptions.RootPath, companyKey, "locations.json");
    }

    class FuelPriceChangesDataAccess : DataAccess<PricePoint>
    {
        private readonly string companyKey;
        private readonly string fuelKey;

        public FuelPriceChangesDataAccess(string companyKey, string fuelKey)
        {
            this.companyKey = companyKey;
            this.fuelKey = fuelKey;
        }
        protected override string GetPath() => Path.Combine(DataAccessOptions.RootPath, companyKey, "priceChanges", $"{fuelKey}.json");
    }
}
