
using Fuel_Georgia_Parser.Utils;
using Env = Fuel_Georgia_Parser.Utils.EnvironmentHelper;
using System;
using System.IO;

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

File.WriteAllText("test.txt", "Test text");

repo.CommitAndPush("Test commit");

