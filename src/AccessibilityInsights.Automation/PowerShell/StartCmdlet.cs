// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System.Collections.Generic;
using System.Management.Automation;

namespace AccessibilityInsights.Automation.PowerShell
{
    /// <summary>
    /// PowerShell Cmdlet to initialize the AccessibilityInsights scan engine
    /// </summary>
    [Cmdlet(VerbsLifecycle.Start, "AccessibilityInsights")]
    public class StartCmdlet : Cmdlet
    {
        /// <summary>
        /// The process ID of the application to scan
        /// </summary>
        [Parameter(Mandatory = false)]
        public string TargetProcessId { get; set; }

        /// <summary>
        /// The output path for all scan results (will be created if it doesn't already exist)
        /// </summary>
        [Parameter(Mandatory = false)]
        public string OutputPath { get; set; }

        /// <summary>
        /// The optional JSON configuration file to specify parameters
        /// </summary>
        [Parameter(Mandatory = false)]
        public string ConfigFile { get; set; }

        /// <summary>
        /// Initialize the scan engine
        /// </summary>
        protected override void ProcessRecord()
        {
            Dictionary<string, string> parameters = new Dictionary<string, string>();

            if (!string.IsNullOrEmpty(this.TargetProcessId))
                parameters[CommandConstStrings.TargetProcessId] = this.TargetProcessId;

            if (!string.IsNullOrEmpty(this.OutputPath))
                parameters[CommandConstStrings.OutputPath] = this.OutputPath;

            string configFile = string.IsNullOrEmpty(this.ConfigFile) ? string.Empty : this.ConfigFile;

            using (new AppDomainAdjuster())
            {
                WriteObject(StartCommand.Execute(parameters, configFile, isPowerShell:true));
            }
        }
    }
}
