using Fuel_Georgia_Parser.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Fuel_Georgia_Parser.Services
{
    static class LocalDataRepository
    {
        private static readonly JsonSerializerOptions options;

        static LocalDataRepository()
        {
            options = new JsonSerializerOptions 
            {
                WriteIndented = true,
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            };
        }

        static string GetCompaniesFilePath() => Path.Combine("data", "companies.json");
        public static Company[] GetCompanies() => GetDataOrDefault(GetCompaniesFilePath(), new Company[0]);
        public static void SetCompanies(Company[] companies) => SetData(GetCompaniesFilePath(), companies);

        static T GetDataOrDefault<T>(string path, T def)
        {
            var res = def;

            if(File.Exists(path))
            {
                var json = File.ReadAllText(path);
                res = JsonSerializer.Deserialize<T>(json, options);
            }

            return res;
        }

        static void SetData<T>(string path, T data) => File.WriteAllText(path, JsonSerializer.Serialize(data, options));
    }
}
