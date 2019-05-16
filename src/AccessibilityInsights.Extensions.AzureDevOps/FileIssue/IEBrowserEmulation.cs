// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using AccessibilityInsights.Extensions.Helpers;
using Microsoft.Win32;
using System;
using System.Diagnostics;

namespace AccessibilityInsights.Extensions.AzureDevOps.FileIssue
{
    /// <summary>
    /// This class sets several registry values in order to ensure that the hosted WebBrowser control renders
    /// documents in standards mode. Without these changes the hosted WebBrowser is unable to properly
    /// render some pages when filing issues
    /// https://docs.microsoft.com/en-us/previous-versions/windows/internet-explorer/ie-developer/general-info/ee330730%28v%3dvs.85%29#browser-emulation
    /// http://msdn.microsoft.com/en-us/library/ee330720(v=vs.85).aspx
    /// </summary>
    public static class IEBrowserEmulation
    {
        public static void SetFeatureControls()
        {
            try
            {
                SetFeatureControlKey("FEATURE_BROWSER_EMULATION", GetBrowserEmulationValue());
                SetFeatureControlKey("FEATURE_AJAX_CONNECTIONEVENTS", 1);
                SetFeatureControlKey("FEATURE_GPU_RENDERING", 1);
            }
#pragma warning disable CA1031 // Do not catch general exception types
            catch (Exception ex)
            {
                ex.ReportException();
            }
#pragma warning restore CA1031 // Do not catch general exception types
        }

        private static void SetFeatureControlKey(string feature, uint value)
        {
            var currentApp = System.IO.Path.GetFileName(Process.GetCurrentProcess().MainModule.FileName);
            using (var key = Registry.CurrentUser.CreateSubKey(String.Concat(@"Software\Microsoft\Internet Explorer\Main\FeatureControl\", feature), RegistryKeyPermissionCheck.ReadWriteSubTree))
            {
                key.SetValue(currentApp, value, RegistryValueKind.DWord);
            }
        }

        /// <summary>
        /// Get the browser emulation mode most appropriate for the installed version of IE,
        /// or the default mode for applications hosting the WebBrowser control
        /// </summary>
        /// <returns></returns>
        private static uint GetBrowserEmulationValue()
        {
            int browserVer = 7;
            using (var Key = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Microsoft\Internet Explorer", RegistryKeyPermissionCheck.ReadSubTree, System.Security.AccessControl.RegistryRights.QueryValues))
            {
                var version = Key.GetValue("svcVersion") ?? Key.GetValue("Version");
                if (version == null || int.TryParse(version.ToString().Split('.')[0], out browserVer) == false)
                {
                    return 7000;
                }
            }

            switch (browserVer)
            {
                case 7: return 7000; // Webpages containing standards-based !DOCTYPE directives are displayed in IE7 Standards mode. Default value for applications hosting the WebBrowser Control.
                case 8: return 8000; // Webpages containing standards-based !DOCTYPE directives are displayed in IE8 mode. Default value for Internet Explorer 8
                case 9: return 9000; // Internet Explorer 9. Webpages containing standards-based !DOCTYPE directives are displayed in IE9 mode. Default value for Internet Explorer 9.
                case 11: return 11001; // Internet Explorer 11. Webpages containing standards-based !DOCTYPE directives are displayed in IE11 mode. Default value for Internet Explorer 11.
                case 10:
                default:
                    return 10000; // Internet Explorer 10. Webpages containing standards-based !DOCTYPE directives are displayed in IE10 mode. Default value for Internet Explorer 10.
            }
        }
    }
}
