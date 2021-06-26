using System;
using System.IO;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Unicode;
using Fuel_Georgia_Parser.Models;

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

    internal abstract class DataAccess<T> where T : class, new()
    {
        public T[] Data
        {
            get
            {
                if (File.Exists(GetPath()))
                {
                    var json = File.ReadAllText(GetPath());
                    return JsonSerializer.Deserialize<T[]>(json, DataAccessOptions.JsonSerializerOptions);
                }

                return GetDefault();
            }
            set => File.WriteAllText(EnsureDirectory(GetPath()),
                JsonSerializer.Serialize(value, DataAccessOptions.JsonSerializerOptions));
        }

        protected abstract string GetPath();

        public virtual T[] GetDefault()
        {
            return Array.Empty<T>();
        }

        private static string EnsureDirectory(string path)
        {
            Directory.CreateDirectory(Path.GetDirectoryName(path));
            return path;
        }
    }

    internal class CompaniesDataAccess : DataAccess<Company>
    {
        protected override string GetPath()
        {
            return Path.Combine(DataAccessOptions.RootPath, "companies.json");
        }
    }

    internal class LocationsDataAccess : DataAccess<Location>
    {
        private readonly string companyKey;

        public LocationsDataAccess(string companyKey)
        {
            this.companyKey = companyKey;
        }

        protected override string GetPath()
        {
            return Path.Combine(DataAccessOptions.RootPath, companyKey, "locations.json");
        }
    }

    internal class FuelPriceChangesDataAccess : DataAccess<PricePoint>
    {
        private readonly string companyKey;
        private readonly string fuelKey;

        public FuelPriceChangesDataAccess(string companyKey, string fuelKey)
        {
            this.companyKey = companyKey;
            this.fuelKey = fuelKey;
        }

        protected override string GetPath()
        {
            return Path.Combine(DataAccessOptions.RootPath, companyKey, "priceChanges", $"{fuelKey}.json");
        }
    }
}