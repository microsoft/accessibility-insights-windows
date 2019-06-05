// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;
using System.Threading;

namespace AccessibilityInsights.Win32
{
    /// <summary>
    /// NativeMethods partial class to hold all Win32 related helper methods.  
    /// </summary>
    internal static partial class NativeMethods
    {
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
