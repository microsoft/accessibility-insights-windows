## July 22 2021 Insider Release ([v1.1.1663.01](https://github.com/Microsoft/accessibility-insights-windows/releases/tag/v1.1.1663.01))

Welcome to the July 22 2021 Insider release of Accessibility Insights for Windows.

Installation Link: https://github.com/microsoft/accessibility-insights-windows/releases/download/v1.1.1663.01/AccessibilityInsights.msi

Documentation Link: https://accessibilityinsights.io/docs/en/windows/overview

### Highlights

- [New Features](#new-features)
- [Rule Updates](#rule-updates)
- [Bug Fixes](#bug-fixes)

#### New Features

- Added support for inspecting and saving [Custom UIAutomation properties](https://docs.microsoft.com/en-us/windows/win32/winauto/uiauto-propertiesoverview). Details on how to configure this feature can be found [in the online documentation](https://accessibilityinsights.io/docs/en/windows/reference/faq/#does-accessibility-insights-for-windows-support-custom-ui-automation-properties).

#### Rule Updates

- `TextBlock` contents inside `Button` elements are no longer flagged in the NameExcludesPrivateUnicodeCharacters rule for WPF. ([#572](https://github.com/microsoft/axe-windows/issues/572))
- The implementation of `TogglePattern` on radio button elements is no longer flagged as an error due to the removal of the `ControlShouldNotSupportTogglePattern` rule. ([#590](https://github.com/microsoft/axe-windows/issues/590))

#### Bug Fixes

- To improve usability, we now use "milliseconds" instead of "ms" when describing the focus tracking delay. ([#1121](https://github.com/microsoft/accessibility-insights-windows/issues/1121))
- Fixed a bug where tooltip tracking in some combo boxes was incorrect. ([#1123](https://github.com/microsoft/accessibility-insights-windows/pull/1123))
- Fixed a color contrast issue in the properties search bar. ([#1149](https://github.com/microsoft/accessibility-insights-windows/pull/1149))
