## January 30 2023 Production Release ([v1.1.2213.01](https://github.com/Microsoft/accessibility-insights-windows/releases/tag/v1.1.2213.01))

Welcome to the January 30 2023 Production release of Accessibility Insights for Windows.

Installation Link: https://github.com/microsoft/accessibility-insights-windows/releases/download/v1.1.2213.01/AccessibilityInsights.msi

Documentation Link: https://accessibilityinsights.io/docs/en/windows/overview

### Highlights

- [New Features](#new-features)
- [Rule Updates](#rule-updates)
- [Bug Fixes](#bug-fixes)

#### New Features

- We have added new features to improve the user experience for users who are blind:
  - Sound options are now "Always", "Never" or "Automatic" (plays sounds if an active screen reader is detected) [#1495](https://github.com/microsoft/accessibility-insights-windows/pull/1495)
  - New sounds now report start and stop of event recording [#1440](https://github.com/microsoft/accessibility-insights-windows/issues/1440)
  - The sound for scanning is now unique to minimize confusion with system sounds [#1444](https://github.com/microsoft/accessibility-insights-windows/issues/1444)
  - Keyboard shortcuts for in-app navigation are now easier to discover [#1509](https://github.com/microsoft/accessibility-insights-windows/pull/1509), [#1512](https://github.com/microsoft/accessibility-insights-windows/issues/1512)
- As requested in [#1454](https://github.com/microsoft/accessibility-insights-windows/issues/1454), we now display MSAA role names as strings, in addition to the numeric value, to describe `LegacyIAccessiblePattern.Role` values 

#### Rule Updates

- We have updated from [Axe.Windows 1.1.7](https://github.com/microsoft/axe-windows/releases/tag/v1.1.7) to [Axe.Windows 2.1.0](https://github.com/microsoft/axe-windows/releases/tag/v2.1.0)
  - We have modified the `BoundingRectangleSizeReasonable` rule to not fail Telerik Sparkline elements with an area of less than 25 pixels [Axe.Windows #780](https://github.com/microsoft/axe-windows/issues/780)
  - We have improved the _How to fix_ text for several rules that previous just repeated the problem description [Axe.Windows #790](https://github.com/microsoft/axe-windows/pull/790), [Axe.Windows #791](https://github.com/microsoft/axe-windows/pull/791)

#### Bug Fixes

- We have provided a user-transparent way to request support of unsupport pattern actions [#1420](https://github.com/microsoft/accessibility-insights-windows/issues/1420)
- We now target .NET Framework 4.8 to enable new accessibility features [#1465](https://github.com/microsoft/accessibility-insights-windows/issues/1465)
- We now leverage UIA properties (toggle state, items in a list, etc.) to better support both Braille and user-customized screen readers [#1323](https://github.com/microsoft/accessibility-insights-windows/issues/1323), [#1514](https://github.com/microsoft/accessibility-insights-windows/issues/1514)
- We have improved the screen reader experience while navigating the event recording grid [#1438](https://github.com/microsoft/accessibility-insights-windows/issues/1438)
- We have fixed a visual glitch while navigating via keyboard [#1150](https://github.com/microsoft/accessibility-insights-windows/issues/1150)
- We now enforce SSL security checks to better protect against spoof attacks when checking for new versions [#1516](https://github.com/microsoft/accessibility-insights-windows/pull/1516)
- We have added support for signed upgrade manifests to better protect against tampering attacks [#1493](https://github.com/microsoft/accessibility-insights-windows/pull/1493)

