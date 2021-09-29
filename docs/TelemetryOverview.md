## Telemetry Overview

Accessibility Insights for Windows uses telemetry to better understand what features are most helpful to users, as well as to help identify potential issues that users are experiencing.

### Reporting a telemetry action
Telemetry actions typically indicate a user-initiated action (starting the app, clicking a button, etc.). They are also used to report the result of a user action (for examplle, a user attempt to open a file failed because the file was in an unrecognized format). At this time, telemetry actions are *not* supported in extensions. The telemetry classes are defined in the `AccessibilityInsights.Desktop.Telemetry` namespace. The `Logger` class provides 2 variants of the static `Logger.PublishTelemetryEvent` method:

#### Single-property action
This version reports an action with a single associated property:
```
Logger.PublishTelemetryEvent(
    TelemetryAction.Mainwindow_Timer_Started,       // Name of action 
    TelemetryProperty.Seconds,                      // Name of property
    count.ToString(CultureInfo.InvariantCulture));  // Value of property (must be a locale-agnostic string)
```

#### Multi-property action
This version reports an action with multiple associated properties:
```
Logger.PublishTelemetryEvent(TelemetryAction.Bug_Save, new Dictionary<TelemetryProperty, string>
    {
        { TelemetryProperty.RuleId, ruleId },  // values are locale-agnostic strings
        { TelemetryProperty.Url, url},
    });
```

The `TelemetryAction` and `TelemetryProperty` enumerations can be extended to add new actions or properties.

#### Context Properties
There are some properties that are desired on every reported telemetry action. The `Logger.AddOrUpdateContextProperty` method exists for this purpose. When a context property is set, it will be automatically added as a property to each subsequent call to `Logger.PublishTelemetryEvent`. This sample adds a context property:
```
Logger.AddOrUpdateContextProperty(TelemetryProperty.Version, GetAppVersion());  // value is a locale-agnostic string
```

### Reporting Exceptions
A dedicated mechanism exists to report an `Exception` caught by the code. It is implemented as an extension method to make the code simpler to read. In non-extension code, this method is defined in the `AccessibilityInsights.Desktop.Telemetry` namespace. For extension code, this method is defined in the `AccessibilityInsights.Extensions.Helpers` namespace. Here is a typical usage where an Exception is caught, reported, then eaten:
```
try
{
    // Code that might throw
}
catch (Exception e)
{
    e.ReportException();
}
```

### Control of Telemery
Telemetry (including reported exceptions) can be disabled either by the computer's administrator or the current user.

#### Administrative override
An administrator can disable telemetry for all users of a system by adding a DWORD value the registry:

```
Key: HKEY_LOCAL_MACHINE\SOFTWARE\Policies\Accessibility Insights for Windows  
Name: DisableTelemetry
Value (DWORD): 1
```
Here is the same data expressed as a Windows REG file:
```
Windows Registry Editor Version 5.00

[HKEY_LOCAL_MACHINE\SOFTWARE\Policies\Accessibility Insights for Windows]
"DisableTelemetry"=dword:00000001
```

Setting this override will disable telemetry for _all_ users of the computer. Since telemetry is disabled systemwide, users are never asked to enable or disable telemetry. The overridden status is reflected in the settings page within the application.

This override exists to support organizations who may be sensitive to any data being collected by outside systems. It is the responsibility of the organization to set this flag through external means.

#### User Control
If no [administrative override](#administrative-override) is set, then telemetry is controlled by each user. When the application starts the first time, the user is asked to enable or disable telemetry. The user can change this setting at any time via the settings page. If the [administrative override](#administrative-override) is later enabled, it will overide the telemetry selection for new application sessions for all users.

### More details
More details are available in the [Telemetry Details Documentation](./TelemetryDetails.md)