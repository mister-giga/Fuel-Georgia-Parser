using Fuel_Georgia_Parser.Utils;
using Env = Fuel_Georgia_Parser.Utils.EnvironmentHelper;
using System;
using System.IO;
using Fuel_Georgia_Parser.Services;
using System.Threading.Tasks;
using System.Linq;
using Fuel_Georgia_Parser.Models;
using System.Collections.Generic;

string repoName = Env.GetRepoName(out var userName);
string token = Env.GetEnvVariable("GH_TOKEN", required: true);
string branch = Env.GetEnvVariable("BRANCH", required: true);

RepoHelper repo = new()
{
    Branch = branch,
    UserName = userName,
    LineOutput = Console.WriteLine,
    RepoName = repoName,
    Token = token
};

repo.Clone();

Directory.SetCurrentDirectory(repoName);

var parsers = new CompanyDataParserBase[] {
    new WissolDataParser(),
    new LukoilDataParser(),
    new RompetrolDataParser()
};

var companiesLocalData = LocalDataRepository.GetCompanies();

var companiesFreshData = await Task.WhenAll(parsers.Select(p => GetFreshCompanyDataAsync(companiesLocalData.FirstOrDefault(x => x.Key == p.CompanyKey), p)));


LocalDataRepository.SetCompanies(companiesFreshData.Select(x => x.company).ToArray());


repo.CommitAndPush("Test commit");

#if DEBUG //cleanup
if(Directory.Exists("./"))
    Directory.Delete("./", true);
#endif


async Task<(Company company, string[] priceChangedFuelKeys)> GetFreshCompanyDataAsync(Company old, CompanyDataParserBase parser)
{
    Console.WriteLine($"Get fresh compnay data started for {parser.CompanyKey}");
    HashSet<string> stablePriceFuelKeys = new HashSet<string>();
    var activeFuels = await parser.GetActiveFuelsAsync();

    if(old?.Fuels?.Any() == true)
    {
        foreach(var activeFuel in activeFuels)
        {
            var oldFuel = old.Fuels.FirstOrDefault(f => f.Key == activeFuel.Key);
            if(oldFuel != null)
            {
                var priceFiff = activeFuel.Price - oldFuel.Price;

                if(priceFiff != 0)
                    activeFuel.Change = priceFiff;
                else
                {
                    activeFuel.Change = oldFuel.Change;
                    stablePriceFuelKeys.Add(activeFuel.Key);
                }
            }
        }
    }

    var newData = new Company
    {
        Key = parser.CompanyKey,
        Name = parser.CompanyName,
        Fuels = activeFuels,
    };

    Console.WriteLine($"Get fresh compnay data ended for {parser.CompanyKey}");
    return (company: newData, priceChangedFuelKeys: activeFuels.Select(x => x.Key).ToHashSet().Except(stablePriceFuelKeys).ToArray());
}