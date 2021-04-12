## April 12, 2021 Insider Release ([v1.1.1562.01](https://github.com/Microsoft/accessibility-insights-windows/releases/tag/v1.1.1562.01))

Welcome to the April 12, 2021 Insider release of Accessibility Insights for Windows.

Installation Link: https://github.com/microsoft/accessibility-insights-windows/releases/download/v1.1.1562.01/AccessibilityInsights.msi

Documentation Link: https://accessibilityinsights.io/docs/en/windows/overview

### Highlights

- [Rule Updates](#rule-updates)
- [Bug Fixes](#bug-fixes)

#### Rule Updates

- Add the `FrameworkDoesNotSupportUIAutomation` rule to report failures on frameworks that are known to not support UI AUtomation [#554](https://github.com/microsoft/axe-windows/issues/554)
- Remove the `ControlShouldNotSupportTablePattern` rule as it wasn't generally useful [#566](https://github.com/microsoft/axe-windows/issues/566)
- Remove false positives in the `BoundingRectangleCompletelyObscuresContainer` rule [#540](https://github.com/microsoft/axe-windows/issues/540)
- Remove false positives in the `NameIsNotNull` rule [#556](https://github.com/microsoft/axe-windows/issues/556)

#### Bug Fixes

- Remove user configuration files when uninstalling the application [#1007](https://github.com/microsoft/accessibility-insights-windows/issues/1007)
- Reduce memory overhead when using the eyedropper control in the Color Contrast Analyzer [#1033](https://github.com/microsoft/accessibility-insights-windows/pull/1033)
- Improve discoverability of the help link in the TabStops instructions [#1052](https://github.com/microsoft/accessibility-insights-windows/pull/1052)
- Set the automation names on headers in the properties grid [#1072](https://github.com/microsoft/accessibility-insights-windows/pull/1072)
- Correct a color contrast issue in the Text Range find text dialog [#1086](https://github.com/microsoft/accessibility-insights-windows/pull/1086)
