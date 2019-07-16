## UI Tests
* UI tests reside in the `UITests` project.
* [WinAppDriver](https://github.com/Microsoft/WinAppDriver) is used for navigation and [Axe.Windows](https://github.com/microsoft/axe-windows) is used for accessibility scanning.
* Test classes inherit from `AIWinSession`.
* Each test class covers an AI-Win usage scenario. For example, the `LoadTestFile` test covers the scenario of loading a test file and inspecting indiviudal test results. Each test class has at least the following methods:
  * A `TestMethod` named `NameOfTestClassTests` that performs the desired validation.
  * A `TestInitialize` method which calls `Setup()`. This will launch AI-Win.
  * A `TestCleanup` method which calls `TearDown()`. This will exit AI-Win.
* Each test uses appropriate `Assert` methods to validate its conditions.
* `driver.ScanAIWin` is called to assess the accessibility of the UI being tested.
* Navigation related functionality (such as entering the settings page or returning to live mode) is separated into the appropriate mode-specific classes in the `UILibrary` folder.
  * If you add functionality related to navigating AI-Win or that you expect to reuse in other UI tests, consider adding that functionality to the appropriate class in `UILibrary` rather than to the specific test.
* UI Automation [AutomationIDs](https://docs.microsoft.com/en-us/dotnet/framework/ui-automation/use-the-automationid-property) should be used when navigating to specific controls in AI-Win. When you need to navigate to a specific control which does not already have a specific AutomationID set, refer to the following steps:
  1. Add a property to `AccessibilityInsights.SharedUx.Properties.AutomationIDs` with this format:
        ```cs
        public static string <NameOfParentControl><NameOfControl><ControlType>){ get; } = nameof (<NameOfParentControl><NameOfControl><ControlType>);
        ```

        For example, the entry for the Expand All button in  `AutomatedChecksControl` looks like:
        ```cs
        public static string AutomatedChecksExpandAllButton{ get; } = nameof(AutomatedChecksExpandAllButton);
        ```
    2. Reference that property when setting the `AutomationId` property of your control in xaml:
        ```cs
        AutomationProperties.AutomationId="{x:Static Properties:AutomationIDs.AutomatedChecksExpandAllButton}"
        ```
    3. Use that property when navigating AI-Win in the UI test:
        ```cs
        var element = driver.FindElementByAccessibilityId(AutomationIDs.AutomatedChecksExpandAllButton)
        ```