## July 24, 2019 Insider Release ([v1.1.0933.01](https://github.com/Microsoft/accessibility-insights-windows/releases/tag/v1.1.0933.01))

Welcome to the July 24, 2019 Insider release of Accessibility Insights for Windows.

### Highlights

  - [UIAccess changes](#rule-updates)
  - [Bug Fixes](#bug-fixes)
  - [Rule Updates](#rule-updates)

#### UIAccess Changes

UIAccess is an advancecd security feature that is required in some very specific scenarios, but it was enabled by default.
This led to some user confusion from users who do not need UIAccess to be enabled. To address this, we have disabled
UIAccess by default. It can be enabled by running the UIAccess.cmd script that installs with Accessibility Insights for Windows.

#### Bug Fixes
  - Settings button should not be a toggle [#364]
  - Tab stop should highlight the first element right away [#151]
  - Prevent crash that can occur after scope changes [#381]
  - Make the UIAccess status accessible (#388)
  - Fix a case where the update dialog was incorrectly being displayed [#394]
  - Fix a repaint issue that occurs when JAWS is running [#379]

#### Rule Updates
We have updated the following accessibility rule based on community feedback.
  - EditSupportsOnlyValuePattern now returns an error instead of a warning
