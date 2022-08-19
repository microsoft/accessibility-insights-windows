## UI Tests
### Overview
UI tests reside in the `UITests` project. [WinAppDriver](https://github.com/Microsoft/WinAppDriver) is used for navigation and [Axe.Windows](https://github.com/microsoft/axe-windows) is used for accessibility scanning. UI tests are automatically run as a part of the PR build.

### Running UI Tests
* To run tests locally, [WinAppDriver](https://github.com/Microsoft/WinAppDriver) must already be running. Once WinAppDriver is open, simply run the UI tests from Visual Studio the [same way unit tests are run](https://docs.microsoft.com/en-us/previous-versions/ms182470(v=vs.140)). 
* UI tests are automatically run as a part of Accessibility Insights for Windows's PR build. Results can be viewed in the [Tests tab](https://docs.microsoft.com/en-us/azure/devops/pipelines/test/review-continuous-test-results-after-build?view=azure-devops#tests-tab) of a specific build. The following attachments are associated with UI tests:
  * Screen capture. You can download a screen capture video of any UI test.
  * Events log. In case a crash occurs, we attach the Windows event log events that were captured during a UI test if there were any such events.
  * A11y test file. If an accessibility issue is found during a UI test, we attach the corresponding .a11ytest file. You can open this with Accessibility Insights for Windows.

### Writing UI Tests
* Test classes inherit from `AIWinSession`.
* Each test class covers an AI-Win usage scenario. For example, the `LoadTestFile` test covers the scenario of loading a test file and inspecting individual test results. Each test class has at least the following methods:
  * A `TestMethod` named `NameOfTestClassTests` that performs the desired validation.
  * A `TestInitialize` method which calls `Setup()`. This will launch Accessibility Insights for Windows.
  * A `TestCleanup` method which calls `TearDown()`. This will exit Accessibility Insights for Windows.
* Each test uses appropriate `Assert` methods to validate its conditions.
* `driver.ScanAIWin` is called to assess the accessibility of the UI being tested.
* Navigation related functionality (such as entering the settings page or returning to live mode) is separated into the appropriate mode-specific classes in the `UILibrary` folder.
  * If you add functionality related to navigating Accessibility Insights for Windows or that you expect to reuse in other UI tests, consider adding that functionality to the appropriate class in `UILibrary` rather than to the specific test.
* UI Automation [AutomationIDs](https://docs.microsoft.com/en-us/dotnet/framework/ui-automation/use-the-automationid-property) should be used when navigating to specific controls in Accessibility Insights for Windows. When you need to navigate to a specific control which does not already have a specific AutomationID set, refer to the following steps:
  1. Add a property to `AccessibilityInsights.SharedUx.Properties.AutomationIDs` with this format:
        ```cs
        public static string <NameOfParentControl><NameOfControl><ControlType>){ get; } = nameof (<NameOfParentControl><NameOfControl><ControlType>);
        ```

        For example, the entry for the Expand All button in  `AutomatedChecksControl` looks like:
        ```cs
        public static string AutomatedChecksExpandAllButton { get; } = nameof(AutomatedChecksExpandAllButton);
        ```
    2. Reference that property when setting the `AutomationId` property of your control in xaml:
        ```cs
        AutomationProperties.AutomationId="{x:Static Properties:AutomationIDs.AutomatedChecksExpandAllButton}"
        ```
    3. Use that property when navigating Accessibility Insights for Windows in the UI test:
        ```cs
        var element = driver.FindElementByAccessibilityId(AutomationIDs.AutomatedChecksExpandAllButton)
        ```
