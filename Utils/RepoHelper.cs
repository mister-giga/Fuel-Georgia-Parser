using System;
using Cli = Fuel_Georgia_Parser.Utils.CliHelper;

namespace Fuel_Georgia_Parser.Utils
{
    internal class RepoHelper
    {
        public string Branch { get; init; }
        public string UserName { get; init; }
        public string RepoName { get; init; }
        public string Token { get; init; }
        public string CommiterUserName { get; init; } = "Bot";

        public Action<string> LineOutput { get; init; }

        public void Clone()
        {
            Cli.Git($"clone --branch {Branch} https://github.com/{UserName}/{RepoName}.git", LineOutput);
        }

        public void CommitAndPush(string message)
        {
            Cli.Git($"config user.name \"{CommiterUserName}\"", LineOutput);
            Cli.Git($"config user.email @{CommiterUserName}", LineOutput);

            Cli.Git("add .", LineOutput);
            Cli.Git($"commit -m\"{message}\"", LineOutput);
            Cli.Git($"push https://{Token}@github.com/{UserName}/{RepoName}.git", LineOutput);
        }
    }
}