// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System.Management.Automation;

namespace Axe.Windows.Automation.PowerShell
{
    /// <summary>
    /// PowerShell Cmdlet to uninitialize the AxeWindows scan engine
    /// </summary>
    [Cmdlet(VerbsLifecycle.Stop, "AxeWindows")]
    public class StopCmdLet : Cmdlet
    {
        /// <summary>
        /// Uninitialize the scan engine
        /// </summary>
        protected override void ProcessRecord()
        {
            using (new AppDomainAdjuster())
            {
                WriteObject(StopCommand.Execute());
            }
        }
    }
}
