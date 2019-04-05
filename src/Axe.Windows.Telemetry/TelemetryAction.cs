// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// 
namespace Axe.Windows.Telemetry
{
    /// <summary>
    /// Telemetry Actions, following the pattern of Scope_Verb_Result
    /// </summary>
    public enum TelemetryAction
    {
#pragma warning disable CA1707 // Identifiers should not contain underscores

        Scan_Statistics,
        SingleRule_Tested_Results,

#pragma warning restore CA1707 // Identifiers should not contain underscores
    }
}
