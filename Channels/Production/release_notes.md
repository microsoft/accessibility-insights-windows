## January 7, 2021 Production Release ([v1.1.1467.01](https://github.com/Microsoft/accessibility-insights-windows/releases/tag/v1.1.1467.01))

Welcome to the January 7, 2021 Production release of Accessibility Insights for Windows.

Installation Link: https://github.com/microsoft/accessibility-insights-windows/releases/download/v1.1.1467.01/AccessibilityInsights.msi

Documentation Link: https://accessibilityinsights.io/docs/en/windows/overview

### Highlights

- [Rule Updates](#rule-updates)
- [Bug Fixes](#bug-fixes)

#### Rule Updates

- Add how-to-fix info for 'custom' DataGridCell behavior in old .NET Framework versions [#441](https://github.com/microsoft/axe-windows/issues/441)
- Split the `ControlShouldSupportSetInfo` rule into 2 rules--one for WPF (`ControlShouldSupportSetInfoWPF`) and one for XAML (`ControlShouldSupportSetInfoXAML`) [#445](https://github.com/microsoft/axe-windows/pull/445)
- Split the `ControlShouldSupportTablePattern` rule into `ControlShouldSupportTablePattern` and `ControlShouldSupportTablePatternInEdge` [#455](https://github.com/microsoft/axe-windows/pull/455)
- Disable the `NameExcludesLocalizedControlType` rule on password edit controls in non-Chromium versions of Microsoft Edge [#471](https://github.com/microsoft/axe-windows/issues/471)
- Disable the `ControlShouldNotSupportTablePattern` rule on Win32 SysListView32 controls [#483](https://github.com/microsoft/axe-windows/issues/483)
- Disable the `NameIsInformative` rule on Win32 controls [#485](https://github.com/microsoft/axe-windows/issues/485)
- Correct the description for the `NameIsInformative` rule [#486](https://github.com/microsoft/axe-windows/pull/486)

#### Bug Fixes

- Honor the currently selected TreeViewMode when selecting elements via mouse tracking [#448](https://github.com/microsoft/axe-windows/pull/448)
- Be more explicit about Color Contrast scenarios where colors can't be determined [#976](https://github.com/microsoft/accessibility-insights-windows/pull/976)
- Don't clip descriptive text unnecessarily [#978](https://github.com/microsoft/accessibility-insights-windows/pull/978)
- Store the Event Listening config with the other config files [#1010](https://github.com/microsoft/accessibility-insights-windows/pull/1010)
- Fix a typo in the local event logging that occurs during upgrades [#1012](https://github.com/microsoft/accessibility-insights-windows/pull/1012)
- Fix several issues to improve product accessibility, especially in High Contrast modes
