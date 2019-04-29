## April 18th, 2019 Insider Release ([v1.1.0837.02](https://github.com/Microsoft/accessibility-insights-windows/releases/tag/v1.1.0837.02))

Welcome to the April 18th, 2019 release of Accessibility Insights for Windows.

Installation Link: [https://github.com/Microsoft/accessibility-insights-windows/releases/download/v1.1.0837.02/AccessibilityInsights.msi](https://github.com/Microsoft/accessibility-insights-windows/releases/download/v1.1.0837.02/AccessibilityInsights.msi)

Documentation Link: [https://accessibilityinsights.io/docs/en/windows/overview](https://accessibilityinsights.io/docs/en/windows/overview)

### Highlights

  - [GitHub Issue Filing](#github-issue-filing)
  - [New Rules](#new-rules)
  - [Multiple Release Channels](#multiple-release-channels)
  
#### GitHub Issue Filing

GitHub users can now file issues directly from inside Accessibility Insights for Windows. Bug filing from Azure DevOps is still supported, and you can freely switch between the two.

#### New Rules

We've added the following accessibility rule based on community feedback:
- Generate a warning if items in a listbox have duplicated names [#133](https://github.com/Microsoft/accessibility-insights-windows/issues/133)

#### Multiple Release Channels

You can now choose how frequently your client updates:
- The **Production** channel updates every 2-4 weeks.
- The **Insider** channel updates every 1-2 weeks.
- The **Canary** channel updates multiple times per week.

You'll be in the **Production** channel by default, but you can change your channel at any time via the **Settings** page

### Bug Fixes

- Improved Automatic color contrast detection in High Contrast White theme [#144](https://github.com/Microsoft/accessibility-insights-windows/issues/144)
- Properly display event details when loading an event details file [#161](https://github.com/Microsoft/accessibility-insights-windows/issues/161)
- Fixed the appearance of the restore button when the app was maximized [#199](https://github.com/Microsoft/accessibility-insights-windows/issues/199)
- Clarified the color descriptions in the Color Contrast Analyzer [#210](https://github.com/Microsoft/accessibility-insights-windows/issues/210)
- ListItems within a Spinner control or a ControlViewSpinner control are no longer treated as errors [#268](https://github.com/Microsoft/accessibility-insights-windows/issues/268), [#272](https://github.com/Microsoft/accessibility-insights-windows/issues/272)
- Modified the description shown when a SplitButton control has a MenuItem child [#269](https://github.com/Microsoft/accessibility-insights-windows/issues/269)
- Added a splash screen to the startup sequence [#202](https://github.com/Microsoft/accessibility-insights-windows/issues/202)
- Fix a typo in the message to configure for Issue filing [#306](https://github.com/Microsoft/accessibility-insights-windows/issues/306)
- Include telemetry context when reporting Exceptions [PR #312](https://github.com/Microsoft/accessibility-insights-windows/pull/312)
- Filter out extended characters when filing issues in AzureDevOps [PR #314](https://github.com/Microsoft/accessibility-insights-windows/pull/314)
- Fix an error that was causing the app to hang on some machines [#317](https://github.com/Microsoft/accessibility-insights-windows/issues/317)
