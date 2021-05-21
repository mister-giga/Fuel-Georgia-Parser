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
string token = Env.GetEnvVariable("INPUT_GH_TOKEN", required: true);
string branch = Env.GetEnvVariable("INPUT_BRANCH", required: true);
bool updateLocations = Convert.ToBoolean(Env.GetEnvVariable("INPUT_UPDATE_LOCATIONS", def: "false", required: false));
bool updatePrices = Convert.ToBoolean(Env.GetEnvVariable("INPUT_UPDATE_PRICES", def: "true", required: false));
DataAccessOptions.RootPath = Env.GetEnvVariable("INPUT_DIR", "data");

FaultColletor.Instance.Init(token, userName, repoName);

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
Directory.CreateDirectory(DataAccessOptions.RootPath);

var parsers = new CompanyDataParserBase[] {
    new WissolDataParser(),
    new LukoilDataParser(),
    new RompetrolDataParser()
};

if(updatePrices)
{
    Console.WriteLine("Start update prices");
    var companiesDataAccess = new CompaniesDataAccess();

    var companiesLocalData = companiesDataAccess.Data;

    var companiesFreshData = await Task.WhenAll(parsers.Select(p => GetFreshCompanyDataAsync(companiesLocalData.FirstOrDefault(x => x.Key == p.CompanyKey), p)));


    companiesDataAccess.Data = companiesFreshData.Select(x => x.company).ToArray();


    foreach(var companyFreshData in companiesFreshData)
    {
        foreach(var priceChangeFuelKey in companyFreshData.priceChangedFuelKeys)
        {
            var fuelPriceChangesDataAccess = new FuelPriceChangesDataAccess(companyFreshData.company.Key, priceChangeFuelKey);
            var currentFuelStatus = companyFreshData.company.Fuels.First(x => x.Key == priceChangeFuelKey);
            fuelPriceChangesDataAccess.Data = fuelPriceChangesDataAccess.Data.Append(new PricePoint
            {
                Price = currentFuelStatus.Price,
                Change = currentFuelStatus.Change,
                Date = DateTime.UtcNow
            }).ToArray();
        }
    }

    Console.WriteLine("Complete update prices");
}

if(updateLocations)
{
    Console.WriteLine("Start update locations");

    await Task.WhenAll(parsers.Select(x => UpdateLocationAsync(x)));

    Console.WriteLine("Complete update locations");
}



repo.CommitAndPush("Test commit");

#if DEBUG //cleanup
if(Directory.Exists("./"))
    Directory.Delete("./", true);
#endif


await FaultColletor.Instance.UploadAsync();


async Task<(Company company, string[] priceChangedFuelKeys)> GetFreshCompanyDataAsync(Company old, CompanyDataParserBase parser)
{
    Console.WriteLine($"Get fresh compnay data started for {parser.CompanyKey}");
    HashSet<string> stablePriceFuelKeys = new();
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

async Task UpdateLocationAsync(CompanyDataParserBase parser)
{
    try
    {
        var freshLocations = await parser.GetLocationsAsync();

        if(freshLocations?.Any() == true)
        {
            var locationDataAccess = new LocationsDataAccess(parser.CompanyKey);
            locationDataAccess.Data = freshLocations;
        }
    }
    catch(Exception ex)
    {
        FaultColletor.Instance.Register($"{parser.GetType().FullName}.GetLocationsAsync() - {parser.CompanyName}", ex, "locations", parser.CompanyKey);
    }
}