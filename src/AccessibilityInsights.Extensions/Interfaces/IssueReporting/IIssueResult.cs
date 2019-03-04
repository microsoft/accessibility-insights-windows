using System;

namespace AccessibilityInsights.Extensions.Interfaces.IssueReporting
{
    /// <summary>
    /// Interface for the details displayed on the new issue button after filing is complete
    /// </summary>
    public interface IIssueResult
    {
        string DisplayText { get; }

        Uri IssueLink { get; }
    }
}
