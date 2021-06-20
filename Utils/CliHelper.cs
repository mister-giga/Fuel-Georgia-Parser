using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fuel_Georgia_Parser.Utils
{
    class CliHelper
    {
        public static void Git(string arguments, Action<string> onOutput = null) => Run("/usr/bin/git", arguments, onOutput);
        public static void Run(string fileName, string arguments, Action<string> onOutput = null)
        {
            Console.WriteLine();
            Console.WriteLine($"{fileName} {arguments}");
            var process = Process.Start(new ProcessStartInfo
            {
                CreateNoWindow = true,
                FileName = fileName,
                RedirectStandardOutput = true,
                UseShellExecute = false,
                Arguments = arguments
            });


            string currLine;
            while ((currLine = process.StandardOutput.ReadLine()) != null)
            {
                onOutput?.Invoke(currLine);
            }
            process.WaitForExit();
        }
    }
}
