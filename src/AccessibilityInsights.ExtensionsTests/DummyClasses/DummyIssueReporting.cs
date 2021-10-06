// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using AccessibilityInsights.Extensions.Interfaces.IssueReporting;
using System;
using System.ComponentModel.Composition;
using System.Threading.Tasks;

namespace AccessibilityInsights.ExtensionsTests.DummyClasses
{
    /// <summary>
    ///  Dummy IssueReporting implementation for container. No need to
    ///  implement the methods other than the boilerplate.
    ///  This is abstract so we can easily use it in 2 concrete classes
    /// </summary>
    public abstract class DummyIssueReportingBase : IIssueReporting
    {
        public string ServiceName => throw new NotImplementedException();

        public Guid StableIdentifier => throw new NotImplementedException();

        public bool IsConfigured => throw new NotImplementedException();

        public bool CanAttachFiles => throw new NotImplementedException();

        public ReporterFabricIcon Logo => throw new NotImplementedException();

        public string LogoText => throw new NotImplementedException();

        public Task<IIssueResultWithPostAction> FileIssueAsync(IssueInformation issueInfo)
        {
            throw new NotImplementedException();
        }

        public Task RestoreConfigurationAsync(string serializedConfig)
        {
            throw new NotImplementedException();
        }

        public IssueConfigurationControl RetrieveConfigurationControl(Action UpdateSaveButton)
        {
            throw new NotImplementedException();
        }

        public void SetConfigurationPath(string configurationPath)
        {
            throw new NotImplementedException();
        }

        public bool TryGetCurrentSerializedSettings(out string settings)
        {
            throw new NotImplementedException();
        }
    }

    [Export(typeof(IIssueReporting))]
    public class DummyIssueReporting1 : DummyIssueReportingBase
    {
    }

    [Export(typeof(IIssueReporting))]
    public class DummyIssueReporting2 : DummyIssueReportingBase
    {
    }
}
