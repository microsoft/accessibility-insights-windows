## August 28, 2020 Production Release ([v1.1.1334.02](https://github.com/Microsoft/accessibility-insights-windows/releases/tag/v1.1.1334.02))

Welcome to the August 28, 2020 Production release of Accessibility Insights for Windows.

Installation Link: https://github.com/microsoft/accessibility-insights-windows/releases/download/v1.1.1334.02/AccessibilityInsights.msi

Documentation Link: https://accessibilityinsights.io/docs/en/windows/overview

### Highlights

- [Rule Updates](#rule-updates)
- [Bug Fixes](#bug-fixes)

#### Rule Updates

- Fixed an issue to properly exclude System menu bars if the text is non-English in BoundingRectangleNotNull [#402](https://github.com/microsoft/axe-windows/issues/402)
- Fixed a false positive with WPF DataGrid's [#417](https://github.com/microsoft/axe-windows/issues/417)

#### Bug Fixes

- Fixed an issue to persist zoom level in Azure Boards [#809](https://github.com/microsoft/accessibility-insights-windows/issues/809)
- Added minimum width for the UIA tree pane so the width cannot be reduced to zero [#814](https://github.com/microsoft/accessibility-insights-windows/issues/814)
- Fixed an issue that was causing an error with the Text Pattern Explorer [#840](https://github.com/microsoft/accessibility-insights-windows/issues/840)
- Fixed an issue that was causing attempts to upgrade or switch channels to fail [#891](https://github.com/microsoft/accessibility-insights-windows/issues/891)