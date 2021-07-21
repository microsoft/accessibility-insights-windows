## July 21, 2021 Production Release ([v1.1.1662.02](https://github.com/Microsoft/accessibility-insights-windows/releases/tag/v1.1.1662.02))

Welcome to the July 21, 2021 Production release of Accessibility Insights for Windows.

Installation Link: https://github.com/microsoft/accessibility-insights-windows/releases/download/v1.1.1662.02/AccessibilityInsights.msi

Documentation Link: https://accessibilityinsights.io/docs/en/windows/overview

### Highlights

- [New Features](#new-features)
- [Rule Updates](#rule-updates)
- [Bug Fixes](#bug-fixes)

#### New Features

- Add support for inspecting and saving [Custom UIAutomation properties](https://docs.microsoft.com/en-us/windows/win32/winauto/uiauto-propertiesoverview). Details on how to configure this feature can be found [here](https://accessibilityinsights.io/docs/en/windows/reference/faq/)

#### Rule Updates

- Don't flag text errors on `TextBlock` content inside `Button` elements [#572](https://github.com/microsoft/axe-windows/issues/572)
- The implementation of `TogglePattern` on radio button elements in no longer flagged as an error [#590](https://github.com/microsoft/axe-windows/issues/590)

#### Bug Fixes

- Use "milliseconds" instead of "ms" when describing the focus tracking delay [#1121](https://github.com/microsoft/accessibility-insights-windows/issues/1121)
- Fix a bug where tooltip tracking in some comboboxes was incorrect [#1123](https://github.com/microsoft/accessibility-insights-windows/pull/1123)
- Fix a color contrast issue in the properties search bar [#1149](https://github.com/microsoft/accessibility-insights-windows/pull/1149)
