## April 19 2022 Insider Release ([v1.1.1934.01](https://github.com/Microsoft/accessibility-insights-windows/releases/tag/v1.1.1934.01))

Welcome to the April 19 2022 Insider release of Accessibility Insights for Windows.

Installation Link: https://github.com/microsoft/accessibility-insights-windows/releases/download/v1.1.1934.01/AccessibilityInsights.msi

Documentation Link: https://accessibilityinsights.io/docs/en/windows/overview

### Highlights

- [New Features](#new-features)
- [Rule Updates](#rule-updates)
- [Bug Fixes](#bug-fixes)

#### New Features
- Allow Single Sign On (SSO) in ADO issue filing [#1312](https://github.com/microsoft/accessibility-insights-windows/pull/1312), [#1316](https://github.com/microsoft/accessibility-insights-windows/pull/1316)

#### Rule Updates
- Name-based rules are no longer applied to `Text` elements that are children of a WPF `CheckBox` element [#648](https://github.com/microsoft/axe-windows/issues/648)
- Improve the messaging of the `SplitButtonInvokeAndTogglePatterns` rule to more accurately reflect what the rule checks [#653](https://github.com/microsoft/axe-windows/issues/653)
- Fix false positive triggered by `ControlShouldSupportExpandCollapsePattern` on a Win32 `SplitButton` control [#659](https://github.com/microsoft/axe-windows/issues/659)

#### Bug Fixes

- Reduce the memory footprint when using color contrast eyedroppers [#951](https://github.com/microsoft/accessibility-insights-windows/issues/951)
- Fix text contrast in dark mode [#845](https://github.com/microsoft/accessibility-insights-windows/issues/845)
- Fix scaling issue in welcome screen in large zoom mode [#1249](https://github.com/microsoft/accessibility-insights-windows/issues/1249)
- Fix text contrast in the `TextPattern` explorer in dark mode [#1262](https://github.com/microsoft/accessibility-insights-windows/issues/1262)
- Fix text contrast in the Attribute and Property pickers in dark mode [#1266](https://github.com/microsoft/accessibility-insights-windows/issues/1266)
- Fix sporadic text highlighter button failure (and potential crash) [#1290](https://github.com/microsoft/accessibility-insights-windows/issues/1290)
- Fix potential telemetry privacy issue if a file on the disk is malformed [#1297](https://github.com/microsoft/accessibility-insights-windows/pull/1297)
