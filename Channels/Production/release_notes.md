## September 27 2022 Production Release ([v1.1.2089.01](https://github.com/Microsoft/accessibility-insights-windows/releases/tag/v1.1.2089.01))

Welcome to the September 27 2022 Production release of Accessibility Insights for Windows.

Installation Link: https://github.com/microsoft/accessibility-insights-windows/releases/download/v1.1.2089.01/AccessibilityInsights.msi

Documentation Link: https://accessibilityinsights.io/docs/en/windows/overview

### Highlights

- [Rule Updates](#rule-updates)
- [Bug Fixes](#bug-fixes)

#### Rule Updates

- Update from [Axe.Windows 1.1.5](https://github.com/microsoft/axe-windows/releases/tag/v1.1.5) to [Axe.Windows 1.1.7](https://github.com/microsoft/axe-windows/releases/tag/v1.1.7)
  - Fix some spelling inconsistencies in rule descriptions and _How to fix_ instructions

#### Bug Fixes

- Remove our dependency on [Unofficial.Microsoft.mshtml](https://www.nuget.org/packages/Unofficial.Microsoft.mshtml), as it it not properly supported [#1377](https://github.com/microsoft/accessibility-insights-windows/issues/1377)
- Improve debuggability by including symbols directly into the assemblies and linking them to the source code using [SourceLink](https://www.nuget.org/packages/Microsoft.SourceLink.GitHub)
- Fix a bug where ADO bug filing generated a "Request query string is too long" error message [#1398](https://github.com/microsoft/accessibility-insights-windows/issues/1398)
- Update how automatic color detection is described and disable it by default (it was leading to some user confusion)
