## June 21 2022 Insider Release ([v1.1.1997.01](https://github.com/Microsoft/accessibility-insights-windows/releases/tag/v1.1.1997.01))

Welcome to the June 21 2022 Insider release of Accessibility Insights for Windows.

Installation Link: https://github.com/microsoft/accessibility-insights-windows/releases/download/v1.1.1997.01/AccessibilityInsights.msi

Documentation Link: https://accessibilityinsights.io/docs/en/windows/overview

### Highlights

- [New Features](#new-features)
- [Rule Updates](#rule-updates)
- [Bug Fixes](#bug-fixes)

#### New Features
- Display known potential framework issues separately to help you focus on problems that are directly in your control
- Provide information about known framework issues, including potential workarounds or fixes

#### Rule Updates
- Remove the following three rules (see [Axe.Windows #698](https://github.com/microsoft/axe-windows/pull/698)):
  - `BoundingRectangleOnUWPMenuBar`
  - `BoundingRectangleOnUWPMenuItem`
  - `BoundingRectangleOnWPFTextParent`
- Add the `BoundingRectangleNotNullListViewXAML` rule to document possible workarounds to a framework limitation ([Axe.Windows #702](https://github.com/microsoft/axe-windows/pull/702))
- Add the `BoundingRectangleNotNullTextBlockXAML` rule to document possible workarounds to a framework limitation ([Axe.Windows #702](https://github.com/microsoft/axe-windows/pull/702))
- Add the `ClickablePointOnScreenWPF` rule to document possible workarounds to a framework limitation ([Axe.Windows #704](https://github.com/microsoft/axe-windows/pull/704))
- Add the `ControlShouldSupportTextPatternEditWinform` rule to document possible workarounds to a framework limitation ([Axe.Windows #705](https://github.com/microsoft/axe-windows/pull/705))
- Add the `IsControlElementTrueRequiredButtonWPF` rule to document possible workarounds to a framework limitation ([Axe.Windows #706](https://github.com/microsoft/axe-windows/pull/706))
- Add the `IsControlElementTrueRequiredTextInEditXAML` rule to document possible workarounds to a framework limitation ([Axe.Windows #706](https://github.com/microsoft/axe-windows/pull/706))
- Update the `FrameworkDoesNotSupportUIAutomation` rule to document possible workarounds to a framework limitation ([Axe.Windows #697](https://github.com/microsoft/axe-windows/pull/697))
- Update the `ControlShouldSupportSetInfoWPF` rule to document possible workarounds to a framework limitation ([Axe.Windows #697](https://github.com/microsoft/axe-windows/pull/697))
- Deprecate support for Edge Legacy ([Axe.Windows #687](https://github.com/microsoft/axe-windows/pull/687))
  - Add the `EdgeBrowserHasBeenDeprecated` rule to document possible workarounds to Edge Legacy's [end of support](https://docs.microsoft.com/en-us/lifecycle/announcements/edge-legacy-eos-details)
  - Remove the `ControlShouldSupportTablePatternInEdge` rule (only applicable in Edge Legacy)
  - Remove the `LandmarkOneMain` rule (only applicable in Edge Legacy)

#### Bug Fixes

- Improve accessibility by bolding selected items in context menus ([#1321](https://github.com/microsoft/accessibility-insights-windows/pull/1321))
- Improve accessibility by updating some control names ([#1322](https://github.com/microsoft/accessibility-insights-windows/issues/1322))
