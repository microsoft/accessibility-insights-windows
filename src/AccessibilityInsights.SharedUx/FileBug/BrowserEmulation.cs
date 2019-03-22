// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using Microsoft.Win32;
using System;
using System.Diagnostics;

namespace AccessibilityInsights.SharedUx.FileBug
{
    /// <summary>
    /// This class is adapted from https://stackoverflow.com/questions/18333459/c-sharp-webbrowser-ajax-call
    /// and sets several registry values in order to ensure that the hosted WebBrowser control renders
    /// documents in standards mode. Without these changes the hosted WebBrowser is unable to properly
    /// render some pages when filing issues
    /// https://docs.microsoft.com/en-us/previous-versions/windows/internet-explorer/ie-developer/general-info/ee330730%28v%3dvs.85%29#browser-emulation
    /// </summary>
    public static class BrowserEmulation
    {
        public static void SetBrowserFeatureControl()
        {
            // http://msdn.microsoft.com/en-us/library/ee330720(v=vs.85).aspx

            // FeatureControl settings are per-process
            var fileName = System.IO.Path.GetFileName(Process.GetCurrentProcess().MainModule.FileName);

            try
            {
                SetBrowserFeatureControlKey("FEATURE_BROWSER_EMULATION", fileName, GetBrowserEmulationMode());
                SetBrowserFeatureControlKey("FEATURE_AJAX_CONNECTIONEVENTS", fileName, 1);
                SetBrowserFeatureControlKey("FEATURE_GPU_RENDERING", fileName, 1);
            }
            catch (Exception)
            {
                // silently ignore
            }
        }

        private static void SetBrowserFeatureControlKey(string feature, string appName, uint value)
        {
            using (var key = Registry.CurrentUser.CreateSubKey(String.Concat(@"Software\Microsoft\Internet Explorer\Main\FeatureControl\", feature), RegistryKeyPermissionCheck.ReadWriteSubTree))
            {
                key.SetValue(appName, value, RegistryValueKind.DWord);
            }
        }

        /// <summary>
        /// Get the browser emulation mode most appropriate for the installed version of IE,
        /// or the default mode for applications hosting the WebBrowser control
        /// </summary>
        /// <returns></returns>
        private static uint GetBrowserEmulationMode()
        {
            int browserVersion = 7;
            using (var Key = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Microsoft\Internet Explorer", RegistryKeyPermissionCheck.ReadSubTree, System.Security.AccessControl.RegistryRights.QueryValues))
            {
                var version = Key.GetValue("svcVersion") ?? Key.GetValue("Version");
                if (version == null || int.TryParse(version.ToString().Split('.')[0], out browserVersion) == false)
                {
                    return 7000;
                }
            }

            switch (browserVersion)
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
