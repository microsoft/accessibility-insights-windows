## FAQ
#### Q. What is Accessibility Insights for Windows?
Accessibility Insights for Windows is the project for Accessibility tools on the Windows platform. This is one of the tools from a suite of tools that help diagnose accessibility issues. 

#### Q. Why should I contribute to Accessibility Insights for Windows?
By contributing you will help ensure that people with disabilities have full access to applications. Make the world a better place!

#### Q. How do I get started?
Visit the [Readme](../README.md) and the [Overview](Overview.md) page.

Once you are ready to make a contribution visit the [Contributions](../Contributing.md) page.

#### Q. Where can I download the application?
You can download and install the application from https://accessibilityinsights.io.

#### Q. How can I add or change an automated accessibility test?
Accessibility Insights uses rules from [axe-windows](https://github.com/microsoft/axe-windows). Please refer to the docs folder there about contributing additional rules.

#### Q. How do I go about adding unit tests?
Please follow the guidelines outline in the [Adding Unit Tests](AddUnitTests.md) page.

#### Q. Where can I find the standards for unit tests?
Standards have been documented on [Unit Test Bar and Standards](UnitTestBarAndStandards.md).

#### Q. What about UI tests?
Refer to the [UI Tests](../src/UITests/README.md) readme.

#### Q. How do I go about making internal interfaces available for testing? 
Please visit [Accessing Internals](AccessingInternals.md).

#### Q. How do I go about debugging an extension during development?
[Debugging Extensions](DebuggingExtensions.md) provides more detail.

#### Q. Is there anything that I need to support High Contrast?
Excellent question. Please visit the [High Contrast Support](HighContrastSupport.md) page for more information.

#### Q. Where can I find out more about a specific rule?
Accessibility Insights uses rules from [axe-windows](https://github.com/microsoft/axe-windows). Please refer to the docs folder there for an explanation of rules.

#### Q. What scenarios must I test before creating a PR? 
All PRs need to be tested against the scenarios that are documented at [Test Scenarios](Scenarios.md). 

If your PR includes UI changes please follow the additional instructions for UI changes in the [Test Scenarios](Scenarios.md) page.

#### Q. What happens after I submit a PR?
All PRs kick off a build and the build status is visible in the PR checks section. Failures need to be addressed before the PR can be merged. 

#### Q.How do I go about adding telemetry for my changes?
Please visit the [Telemetry Overview](TelemetryOverview.md) page on how to do so.

#### Q.How do I learn about the telemetry that is collected?
Please visit the [Telemetry Details](TelemetryDetails.md) page for more information.

#### Q.How do I access the telemetry dashboard?
This information is only available internally. If you have specific questions about telemetry, please create an issue providing the details of what you are hoping to learn and why. We will consider the request at our regularly scheduled triage meeting.

#### Q. How do I add a project to the solution?
Visit [Adding a new project](NewProject.md) for instructions.

#### Q. On my PR the "license/cla" check does not complete. What should I do?
Make sure that you have signed our [CLA agreement](../Contributing.md). If you haven't, please sign the agreement, then close and reopen the PR. This will trigger a new build with the CLA agreement in place.

#### Q. How do I get in contact?
Please file a [Github Issue](https://github.com/Microsoft/accessibility-insights-windows/issues/new/choose). We actively monitor PRs and issues.

#### Q. I encountered a bug. Where do I report it?
We use Github Issues for bug tracking.

Visit the [issues](https://github.com/Microsoft/accessibility-insights-windows/issues?q=is%3Aissue+is%3Aopen%2Cclosed) page to see if an issue for it already exists.

If it doesn't, feel free to [file an issue](https://github.com/Microsoft/accessibility-insights-windows/issues/new/choose).
