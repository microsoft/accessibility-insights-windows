## February 28, 2020 Production Release ([v1.1.1152.01](https://github.com/Microsoft/accessibility-insights-windows/releases/tag/v1.1.1152.01))

Welcome to the February 28, 2020 Production release of Accessibility Insights for Windows.

Installation Link: https://github.com/microsoft/accessibility-insights-windows/releases/download/v1.1.1152.01/AccessibilityInsights.msi

Documentation Link: https://accessibilityinsights.io/docs/en/windows/overview

### Highlights

- [Rule Updates](#rule-updates)
- [Bug Fixes](#bug-fixes)
- [Other Updates](#other-updates)

#### Rule Updates

- Added two new rules to ensure that an element's clickable point and IsOffscreen property agree [#162)(https://github.com/microsoft/axe-windows/issues/162)
- Fixed two false positives [#239](https://github.com/microsoft/axe-windows/issues/239), [#240](https://github.com/microsoft/axe-windows/issues/240)

#### Bug Fixes

- Fixed issue to simplify automation name in Azure Boards connection dialog [#707](https://github.com/microsoft/accessibility-insights-windows/issues/707)
- Fixed issue to change programmatic order of save button on Settings page [#700](https://github.com/microsoft/accessibility-insights-windows/issues/700)
- Fixed issue to use system color for link-style buttons in high contrast modes [#693](https://github.com/microsoft/accessibility-insights-windows/pull/693)
- Fixed issue to change AutomationName and ToolTip for tabstops (i) icon link [#692](https://github.com/microsoft/accessibility-insights-windows/pull/692)
- Fixed UIA HeadingLevel values being reported incorrectly [#263](https://github.com/microsoft/axe-windows/issues/263)
- Fixed issue to prevent UIA StyleId property values being always reported as "Unknown" [#265](https://github.com/microsoft/axe-windows/issues/265)

#### Other Updates

- Updated shortcut name for clarity [#706](https://github.com/microsoft/accessibility-insights-windows/pull/706)