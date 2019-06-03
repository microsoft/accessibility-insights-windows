## June 4th, 2019 Insider Release ([v1.1.0883.01](https://github.com/Microsoft/accessibility-insights-windows/releases/tag/v1.1.0883.01))

Welcome to the June 4th, 2019 release of Accessibility Insights for Windows.

Installation Link: [https://github.com/Microsoft/accessibility-insights-windows/releases/download/v1.1.0883.01/AccessibilityInsights.msi](https://github.com/Microsoft/accessibility-insights-windows/releases/download/v1.1.0883.01/AccessibilityInsights.msi)

Documentation Link: [https://accessibilityinsights.io/docs/en/windows/overview](https://accessibilityinsights.io/docs/en/windows/overview)

### Highlights

  - [Axe.Windows](#axe.windows)
  - [Telemetry Improvements](#telemetry-improvements)
  - [New Rules](#new-rules)
  
#### Axe.Windows

The core scanning code from Accessibility Insights for Windows has been moved into a separate GitHub repo (https://github.com/microsoft/axe-windows). Accessibility Insights now gets its core scanning code from that location. This code will be handled separately to make this functionality more broadly available to the accessibility community.

#### Telemetry Improvements

We identified a few points in the code where we had no telemetry about failures that users might be experiencing. We added these data points to help us make the product better. No additional customer data is exposed.

#### New Rules

No new rules have been added to this release.

### Bug Fixes

- Prevent Accessibility Insights from hiding the UAC dialog during upgrade or channel switch [#336](https://github.com/Microsoft/accessibility-insights-windows/issues/336)
- Improve the color contrast (including high contrast) in the buttons of the upgrade dialog [#342](https://github.com/Microsoft/accessibility-insights-windows/issues/342)
- Prevent the upgrade dialog from floating above the browser when viewing release notes [#351](https://github.com/Microsoft/accessibility-insights-windows/issues/351)
- Make the highlighter more intuitive when opening an events file [#365](https://github.com/Microsoft/accessibility-insights-windows/issues/365)
- Fix a couple of issues when entering custom hot keys [#378](https://github.com/Microsoft/accessibility-insights-windows/pull/378)
