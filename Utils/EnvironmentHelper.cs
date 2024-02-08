using System;

namespace Fuel_Georgia_Parser.Utils
{
    internal class EnvironmentHelper
    {
        public static string GetEnvVariable(string variable, string def = null, bool required = false)
        {
            var value = Environment.GetEnvironmentVariable(variable);

            if (required && value == null)
                throw new ArgumentException($"{variable} is not provided", variable);

            if (value == null)
                return def;
            return value;
        }

        public static bool GetEnvVariable(string variable, bool def, bool required = false)
        {
            return GetEnvVariable(variable, def.ToString(), required)
                .Equals("True", StringComparison.OrdinalIgnoreCase);
        }

        public static string GetRepoName(out string userName)
        {
            var repo = GetEnvVariable("GITHUB_REPOSITORY", required: true).Split('/');
            userName = repo[0];
            return repo[1];
        }
    }
}