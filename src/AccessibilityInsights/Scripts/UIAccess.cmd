@echo off
if /i '%1' == 'Enable' goto Confirm
if /i '%1' == 'Disable' goto Confirm

:Usage
echo.
echo UIAccess.cmd
echo.
echo This script will set the UIAccess status of Accessibility Insights for Windows.
echo UIAccess support is disabled by default. This script must be run from an
echo administrative command prompt.
echo.
echo Usage: UIAccess.cmd ^<action^>
echo.
echo Where ^<action^> is one of:
echo   ENABLE  (to enable UIAccess support)
echo   DISABLE (to disable UIAccess support)
goto :eof

:Confirm
echo.
echo CAUTION! You should only change the UIAccess state if you understand the
echo          potential security implications!
echo.
choice /m "Are you sure you want to %1 UIAccess support?"
if ErrorLevel 2 goto UserCanceled
goto UpdateManifest

:UserCanceled
echo.
echo Operation was canceled.
goto :eof

:UpdateManifest
rem DO NOT use copy here, as we also need to update the file's last write time.
rem An "Access is denied" failure will not set errorlevel without help. The
rem extra echo statements provide that help
type UIAccess_%1d.manifest > AccessibilityInsights.exe.manifest && echo. || echo.
if errorlevel 1 goto UpdateFailed

:UpdateSucceeded
echo Update complete. Changes will take effect when Accessibility Insights for
echo Windows is next started.
goto :eof

:UpdateFailed
echo Unable to complete the update. Please ensure that you are running this script
echo with administrative privileges, and that you are running it from the same
echo folder where Accessibility Insights for Windows has been installed.
