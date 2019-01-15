// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System.Collections.Generic;
using System.Management.Automation;

namespace AccessibilityInsights.Automation.PowerShell
{
    /// <summary>
    /// PowerShell Cmdlet to run a single scan (aka snapshot)
    /// </summary>
    [Cmdlet(VerbsLifecycle.Invoke, "Snapshot")]
    public class InvokeSnapshotCmdlet : Cmdlet
    {
        /// <summary>
        /// The process ID of the application to scan
        /// </summary>
        [Parameter(Mandatory=false)]
        public string TargetProcessId { get; set; }

        /// <summary>
        /// The Output file where the scan results will be saved
        /// </summary>
        [Parameter(Mandatory = false)]
        public string OutputFile { get; set; }

        /// <summary>
        /// This is where the scan is triggered
        /// </summary>
        protected override void ProcessRecord()
        {
            Dictionary<string, string> parameters = new Dictionary<string, string>();

            if (!string.IsNullOrEmpty(this.TargetProcessId))
                parameters[CommandConstStrings.TargetProcessId] = this.TargetProcessId;

            if (!string.IsNullOrEmpty(this.OutputFile))
                parameters[CommandConstStrings.OutputFile] = this.OutputFile;

            using (new AppDomainAdjuster())
            {
                WriteObject(SnapshotCommand.Execute(parameters));
            }
        }
    }
}
