

## September [INSERT DATE], 2019 Insider Release ([v1.1.0990.01](https://github.com/Microsoft/accessibility-insights-windows/releases/tag/v1.1.0990.01))

Welcome to the September [INSERT DATE], 2019 Insider release of Accessibility Insights for Windows.

Installation Link: https://github.com/microsoft/accessibility-insights-windows/releases/download/v1.1.0990.01/AccessibilityInsights.msi

Documentation Link: https://accessibilityinsights.io/docs/en/windows/overview

### Highlights

- [Color Contrast Changes](#color-contrast-changes)
- [Bug Fixes](#bug-fixes)
- [Rule Updates](#rule-updates)

#### Color Contrast Changes

We have added functionality to the Color Contrast Analyzer feature in Accessibility Insights for Windows to help users determine whether the contrast ratio of their foreground and background colors passes the new [WCAG 2.1 AA Non-text Contrast Success Criterion](https://www.w3.org/WAI/WCAG21/Understanding/non-text-contrast.html) for graphical objects and UI components (at least 3:1)

#### Bug Fixes

- We have fixed several issues to make the app more usable for screen reader users.
- We have updated to Axe.Windows 0.3.4 [#529](https://github.com/microsoft/accessibility-insights-windows/pull/529)
- We have fixed the issue that caused "Prerelease Build" to display incorrectly in the version bar [#450](https://github.com/microsoft/accessibility-insights-windows/pull/450)
- We have fixed the "File issue" button column width so it scales appropriately with font size [#551](https://github.com/microsoft/accessibility-insights-windows/pull/551)

#### Rule Updates

We have added/updated the following accessibility rules based on community feedback:

- Added the `NameExcludesPrivateUnicodeCharacters`, `HelpTextExcludesPrivateUnicodeCharacters`, and `LocalizedControlTypeExcludesPrivateUnicodeCharacters` rules to detect characters in the private Unicode range with UIA Name, HelpText, and LocalizedControlType properties; and any other properties where a speech synthesizer or Braille display might be used to convey content to a user [#129](https://github.com/microsoft/axe-windows/issues/129).

- Updated the `ControlShouldSupportTextPattern` rule to add the following exemptions [#140](https://github.com/microsoft/axe-windows/issues/140):

  - Edit fields which are not focusable in the DirectUI framework from the ControlShouldSupportTextPattern rule because they do not appear to be truly editable.
  - Edit fields built in with the Win32 framework. Win32 edit fields don’t seem to present a barrier to ATs, probably because they can be accessed using Windows messages.

- Updated the `BoundingRectangleNotNull` rule to add the following exemptions [#139](https://github.com/microsoft/axe-windows/issues/139), [#142](https://github.com/microsoft/axe-windows/issues/142):

  - Non-focusable buttons which are the children of sliders
  - Group elements inside Edge (Original, not based on Chromium).
