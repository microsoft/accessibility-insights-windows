// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;

namespace AccessibilityInsights.Extensions.AzureDevOps
{
    /// <summary>
    /// The C# compiler "helps" us by pruning transitive dependencies when it detects no code path that
    /// can reach them. However, that detection is imperfect, and we end up lacking some assemblies
    /// that we genuinely need. To counter this, we trick the compiler into detecting references
    /// to these assemblies that it fails to detect. It's very unfortunate that we need to do this,
    /// but it's the only solution that I could find after multiple attempts to find a better option.
    /// </summary>
    static class ForcedDependencies
    {
        private static void ForceDependency(Type _)
        {
        }

#pragma warning disable IDE0051 // Refer to class summary for why these are here
        private static void TestEnforcedDependencies()
        {
            // Types that we can detect via unit tests. Namespace generally matches the package.
            ForceDependency(typeof(Microsoft.TeamFoundation.Build.WebApi.AccessDeniedException));
            ForceDependency(typeof(Microsoft.TeamFoundation.Dashboards.WebApi.CreateDashboardWithExistingIdException));
            ForceDependency(typeof(Microsoft.TeamFoundation.Test.WebApi.AutomatedTestRunSliceStatus));
            ForceDependency(typeof(Microsoft.TeamFoundation.Wiki.WebApi.Wiki));
            ForceDependency(typeof(Microsoft.TeamFoundation.WorkItemTracking.Process.WebApi.WitConstants));
            ForceDependency(typeof(Microsoft.VisualStudio.Services.TestManagement.TestPlanning.WebApi.BuildDefinitionReference));
            ForceDependency(typeof(Microsoft.VisualStudio.Services.TestResults.WebApi.Attachment));
            ForceDependency(typeof(System.Diagnostics.EnhancedStackFrame));  // Ben.Demystifier
        }

        private static void ManuallyEnforcedDependencies()
        {
            // Types that we can't detect via unit tests. Namespace generally matches the package.
            ForceDependency(typeof(Microsoft.IdentityModel.Clients.ActiveDirectory.AdalClaimChallengeException));
            ForceDependency(typeof(Microsoft.IdentityModel.JsonWebTokens.JsonClaimValueTypes));
            ForceDependency(typeof(Microsoft.IdentityModel.Logging.IdentityModelEventSource));
            ForceDependency(typeof(Microsoft.IdentityModel.Tokens.AsymmetricSecurityKey));
            ForceDependency(typeof(System.IdentityModel.Tokens.Jwt.JsonClaimValueTypes));
            ForceDependency(typeof(System.Net.Http.Formatting.BaseJsonMediaTypeFormatter));
            ForceDependency(typeof(System.Reflection.Metadata.ArrayShape));
        }
#pragma warning restore IDE0051 // Refer to class summary for why these are here
    }
}
