// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;
using System.Collections.Generic;
using System.Globalization;
using AccessibilityInsights.Automation;

namespace AccessibilityInsights.CI
{
    class Program
    {
        /// <summary>
        /// This entry point does not ship, but it makes for a quick and easy way to debug through the
        /// automation code. One caveat--we intentionally don't build symbols for this app, so while you
        /// can use it to to debug the automation code, breakpoints set in this class will be ignored.
        /// </summary>
        static void Main(string[] args)
        {
            Dictionary<string, string> parameters = new Dictionary<string, string>();
            string secondaryConfigFile = string.Empty;

            char[] delimiters = {'='};

            foreach (string arg in args)
            {
                string[] pieces = arg.Split(delimiters);
                if (pieces.Length == 2)
                {
                    string key = pieces[0].Trim();
                    string value = pieces[1].Trim();

                    if (!string.IsNullOrWhiteSpace(key) && !string.IsNullOrWhiteSpace(value))
                    {
                        // Special case for SecondaryConfigFile
                        if (key.Equals("SecondaryConfigFile", StringComparison.OrdinalIgnoreCase))
                            secondaryConfigFile = value;
                        else
                            parameters[key] = value;
                        continue;
                    }
                }

                Console.WriteLine("Ignoring malformed input: {0}", arg);
            };

            Console.WriteLine(StartCommand.Execute(parameters, secondaryConfigFile).ToString());

            int autoFileId = 0;

            while (true)
            {
                Console.Write("Enter process ID to capture (blank to exit): ");
                string input = Console.ReadLine();

                if (string.IsNullOrEmpty(input))
                    break;

                if (!int.TryParse(input, out int processId))
                {
                    Console.WriteLine("Not a valid int: " + input);
                    continue;
                }

                Dictionary<string, string> snapshotParameters = new Dictionary<string, string>
                {
                    { CommandConstStrings.TargetProcessId, input },
                    { CommandConstStrings.OutputFile, autoFileId++.ToString(CultureInfo.InvariantCulture) },
                };
                Console.WriteLine(SnapshotCommand.Execute(snapshotParameters).ToString());
            }
            Console.WriteLine(StopCommand.Execute().ToString());
        }
    }
}
