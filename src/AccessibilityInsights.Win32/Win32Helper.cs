// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using Microsoft.Win32;
using System;
using System.Drawing;
using System.Threading;

namespace AccessibilityInsights.Win32
{
    /// <summary>
    /// NativeMethods partial class to hold all Win32 related helper methods.  
    /// </summary>
    internal static partial class NativeMethods
    {

        /// <summary>
        /// Windows 10 version number
        /// the value is based on the value in @"HKEY_LOCAL_MACHINE\Software\Microsoft\Windows NT\CurrentVersion", "CurrentVersion"
        /// </summary>
        static readonly Version Win10Version = new Version(6, 3);
        const string WindowsVersionRegKey = @"HKEY_LOCAL_MACHINE\Software\Microsoft\Windows NT\CurrentVersion";

        /// <summary>
        /// Check whether the current Windows is Windows 7 or not. 
        /// </summary>
        /// <returns></returns>
        internal static bool IsWindows7()
        {
            return Environment.OSVersion.Version.Major == 6 && Environment.OSVersion.Version.Minor == 1;
        }

        /// <summary>
        /// Get DPI value from pointer
        /// </summary>
        /// <param name="point"></param>
        /// <param name="dpiType"></param>
        /// <param name="dpiX"></param>
        /// <param name="dpiY"></param>
        internal static void GetDpi(Point point, DpiType dpiType, out uint dpiX, out uint dpiY)
        {
            var mon = NativeMethods.MonitorFromPoint(point, 2/*MONITOR_DEFAULTTONEAREST*/);
            if (IsWindows7())
            {
                Graphics g = Graphics.FromHwnd(IntPtr.Zero);
                IntPtr desktop = g.GetHdc();

                dpiX = NativeMethods.GetDeviceCaps(desktop, (int)DeviceCap.LOGPIXELSX);
                dpiY = NativeMethods.GetDeviceCaps(desktop, (int)DeviceCap.LOGPIXELSY);
            }
            else
            {
                NativeMethods.GetDpiForMonitor(mon, dpiType, out dpiX, out dpiY);
            }
        }

        /// <summary>
        /// Check whether App is running with the UIAccess privilege.
        /// </summary>
        /// <returns></returns>
        internal static bool IsRunningWithUIAccess()
        {
            IntPtr hToken;
            if (NativeMethods.OpenProcessToken(System.Diagnostics.Process.GetCurrentProcess().Handle, Win32Constants.TOKEN_QUERY, out hToken))
            {
                try
                {
                    uint cbData;
                    uint uIAccess;
                    if (NativeMethods.GetTokenInformation(hToken, TOKEN_INFORMATION_CLASS.TokenUIAccess, out uIAccess, sizeof(uint), out cbData))
                    {
                        if (uIAccess != 0)
                        {
                            return true;
                        }
                    }
                }
                finally
                {
                    NativeMethods.CloseHandle(hToken);
                }
            }

            return false;
        }

        /// <summary>
        /// Set focus on the windows given the windows handle
        /// </summary>
        /// <param name="focusOnWindowHandle"></param>
        internal static void FocusWindow(IntPtr focusOnWindowHandle)
        {
            uint style = NativeMethods.GetWindowLong(focusOnWindowHandle, -16);

            // Minimize and restore to be able to make it active.
            if ((style & Win32Constants.WS_MINIMIZE) == Win32Constants.WS_MINIMIZE)
            {
                NativeMethods.ShowWindow(focusOnWindowHandle, ShowWindowCommands.Restore);
            }

            uint currentlyFocusedWindowProcessId = NativeMethods.GetWindowThreadProcessId(NativeMethods.GetForegroundWindow(), IntPtr.Zero);
            uint appThread = (uint)Thread.CurrentThread.ManagedThreadId;

            if (currentlyFocusedWindowProcessId != appThread)
            {
                NativeMethods.AttachThreadInput(currentlyFocusedWindowProcessId, appThread, true);
                NativeMethods.BringWindowToTop(focusOnWindowHandle);
                NativeMethods.ShowWindow(focusOnWindowHandle, ShowWindowCommands.Show);
                NativeMethods.AttachThreadInput(currentlyFocusedWindowProcessId, appThread, false);
            }

            else
            {
                NativeMethods.BringWindowToTop(focusOnWindowHandle);
                NativeMethods.ShowWindow(focusOnWindowHandle, ShowWindowCommands.Show);
            }
            NativeMethods.SetActiveWindow(focusOnWindowHandle);
        }

        /// <summary>
        /// Get the current Windows version info from HKLM
        /// </summary>
        /// <returns></returns>
        private static string GetCurrentWindowsVersion()
        {
            return (string)Registry.GetValue(WindowsVersionRegKey, "CurrentVersion", "");
        }

        /// <summary>
        /// Get the current Windows build info from HKLM
        /// </summary>
        /// <returns></returns>
        private static string GetCurrentWindowsBuild()
        {
            return (string)Registry.GetValue(WindowsVersionRegKey, "CurrentBuild", "");
        }

        private static OsComparisonResult CompareWindowsVersionToWin10()
        {
            if (Version.TryParse(GetCurrentWindowsVersion(), out Version currentVersion))
            {
                if (currentVersion > Win10Version)
                    return OsComparisonResult.Newer;

                if (currentVersion == Win10Version)
                    return OsComparisonResult.Equal;
            }

            return OsComparisonResult.Older;
        }

        private static OsComparisonResult CompareToWindowsBuildNumber(uint minimalBuild)
        {
            if (uint.TryParse(GetCurrentWindowsBuild(), out uint currentBuild))
            {
                if (currentBuild > minimalBuild)
                    return OsComparisonResult.Newer;

                if (currentBuild == minimalBuild)
                    return OsComparisonResult.Equal;
            }

            return OsComparisonResult.Older;
        }

        /// <summary>
        /// Helper function to evaluate builds
        /// </summary>
        /// <param name="minimalBuild">The minimal build needed to pass</param>
        /// <returns>True iff the OS is at least Win10 at the specified build</returns>
        internal static bool IsAtLeastWin10WithSpecificBuild(uint minimalBuild)
        {
            OsComparisonResult win10ComparisonResult = CompareWindowsVersionToWin10();

            return win10ComparisonResult == OsComparisonResult.Newer ||
                (win10ComparisonResult == OsComparisonResult.Equal && CompareToWindowsBuildNumber(minimalBuild) != OsComparisonResult.Older);
        }

        /// <summary>
        /// Check whether current OS is Win10 RS3 or later
        /// </summary>
        /// <returns>True iff the OS is at least Win10 RS3</returns>
        internal static bool IsWindowsRS3OrLater()
        {
            return IsAtLeastWin10WithSpecificBuild(16228); // Build 16228 is confirmed in the RS3 range
        }

        /// <summary>
        /// Check whether current OS is Win10 RS5 or later
        /// </summary>
        /// <returns>True iff the OS is at least Win10 RS5</returns>
        internal static bool IsWindowsRS5OrLater()
        {
            return IsAtLeastWin10WithSpecificBuild(17713); // Build 17713 is confirmed in the RS5 range
        }

        /// <summary>
        /// Get RGB value
        /// </summary>
        /// <param name="r"></param>
        /// <param name="g"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        internal static int RGB(int r, int g, int b)
        {
            return r | g << 8 | b << 16;
        }
    }
}
