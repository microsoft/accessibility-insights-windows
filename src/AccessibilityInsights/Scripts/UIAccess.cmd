@echo off
 if /i '%1' == 'Enable' goto Verify
 if /i '%1' == 'Disable' goto Verify
 
 :Usage
 echo.
 echo UIAccess.cmd
 echo.
 echo This script will set the UIAccess status of Accessibilty Insights for Windows.
 echo UIAccess support is disabled by default. This script must be run from an
 echo administrative command prompt.
 echo.
 echo Usage: UIAccess.cmd ^<action^>
 echo.
 echo Where ^<action^> is one of:
 echo   ENABLE  (to enable UIAccess support)
 echo   DISABLE (to disable UIAccess support)
 echo.
 goto :eof
 
 :Verify
 echo.
 echo CAUTION! You should only change the UIAccess state if you understand the
 echo          potential security implications!
 echo.
 choice /m "Are you sure you want to %1 UIAccess support?"
 if ErrorLevel 1 goto UpdateManifest
 
 echo.
 echo No change was made
 echo.
 goto :eof
 
 :UpdateManifest
 type UIAccess_%1d.manifest > AccessibilityInsights.exe.manifest
 echo.
 echo Update complete. The change will take effect the next time Accessibilty
 echo Insights for Windows is started.
 echo.
 