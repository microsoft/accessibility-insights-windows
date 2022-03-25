// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace AccessibilityInsights.Extensions.AzureDevOps.Enums
{
    /// <summary>
    /// Supported identifiers for AzureDevOps fields
    /// </summary>
    public enum AzureDevOpsField
    {
        Title,
        ReproSteps,
        Tags,
        AreaPath,
        IterationPath,
    }

    /// <summary>
    /// Simple class to provide the API-required version of these IDs
    /// </summary>
    public static class AzureDevOpsFieldExtension
    {
        /// <summary>
        /// Converts an AzureDevOpsField value to its API-required string
        /// </summary>
        /// <param name="value">The AzureDevOpsField</param>
        /// <returns>The API-required string</returns>
        public static string ToApiString(this AzureDevOpsField value)
        {
            switch (value)
            {
                // These fields are all documented at https://docs.microsoft.com/en-us/rest/api/azure/devops/wit/Revisions/List?view=azure-devops-rest-5.0
                case AzureDevOpsField.Title: return "System.Title";
                case AzureDevOpsField.ReproSteps: return "Microsoft.VSTS.TCM.ReproSteps";
                case AzureDevOpsField.Tags: return "System.Tags";
                case AzureDevOpsField.AreaPath: return "System.AreaPath";
                case AzureDevOpsField.IterationPath: return "System.IterationPath";
            }
            return string.Empty;
        }
    }
}
