// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;

namespace AccessibilityInsights.Extensions.AzureDevOps
{
    static class ForcedDependencies
    {
        private static void ForceDependency(Type _)
        {
        }

        private static void ForceDependencies()
        {
            // The types here are simply the first ones that show up in Intellisense. The specific
            // types are irrelevant, as long as get the assembly. Namespaces follow the assembly
            // name except where otherwise noted
            ForceDependency(typeof(System.Diagnostics.EnhancedStackFrame));  // Ben.Demystifier
            ForceDependency(typeof(Microsoft.TeamFoundation.Build.WebApi.AccessDeniedException));
            ForceDependency(typeof(Microsoft.TeamFoundation.Dashboards.WebApi.CreateDashboardWithExistingIdException));
            ForceDependency(typeof(Microsoft.TeamFoundation.Test.WebApi.AutomatedTestRunSliceStatus));
            ForceDependency(typeof(Microsoft.TeamFoundation.Wiki.WebApi.Wiki));
            ForceDependency(typeof(Microsoft.TeamFoundation.WorkItemTracking.Process.WebApi.WitConstants));
            ForceDependency(typeof(Microsoft.VisualStudio.Services.TestManagement.TestPlanning.WebApi.BuildDefinitionReference));
            ForceDependency(typeof(Microsoft.VisualStudio.Services.TestResults.WebApi.Attachment));
        }
    }
}
