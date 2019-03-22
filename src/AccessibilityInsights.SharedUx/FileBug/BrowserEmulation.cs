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
            dynamic fileName = System.IO.Path.GetFileName(Process.GetCurrentProcess().MainModule.FileName);

            // make the control is not running inside Visual Studio Designer
            if (String.Compare(fileName, "devenv.exe", true) == 0 || String.Compare(fileName, "XDesProc.exe", true) == 0)
            {
                return;
            }

            SetBrowserFeatureControlKey("FEATURE_BROWSER_EMULATION", fileName, GetBrowserEmulationMode());
            // Webpages containing standards-based !DOCTYPE directives are displayed in IE10 Standards mode.
            SetBrowserFeatureControlKey("FEATURE_AJAX_CONNECTIONEVENTS", fileName, 1);
            SetBrowserFeatureControlKey("FEATURE_GPU_RENDERING", fileName, 1);
        }

        private static void SetBrowserFeatureControlKey(string feature, string appName, uint value)
        {
            using (var key = Registry.CurrentUser.CreateSubKey(String.Concat("Software\\Microsoft\\Internet Explorer\\Main\\FeatureControl\\", feature), RegistryKeyPermissionCheck.ReadWriteSubTree))
            {
                key.SetValue(appName, (UInt32)value, RegistryValueKind.DWord);
            }
        }

        private static UInt32 GetBrowserEmulationMode()
        {
            int browserVersion = 7;
            using (var Key = Registry.LocalMachine.OpenSubKey("SOFTWARE\\Microsoft\\Internet Explorer", RegistryKeyPermissionCheck.ReadSubTree, System.Security.AccessControl.RegistryRights.QueryValues))
            {
                dynamic version = Key.GetValue("svcVersion");
                if (null == version)
                {
                    version = Key.GetValue("Version");
                    if (null == version)
                    {
                        throw new ApplicationException("Microsoft Internet Explorer is required!");
                    }
                }
                int.TryParse(version.ToString().Split('.')[0], out browserVersion);
            }

            UInt32 mode = 10000;
            // Internet Explorer 10. Webpages containing standards-based !DOCTYPE directives are displayed in IE10 Standards mode. Default value for Internet Explorer 10.
            switch (browserVersion)
            {
                case 7:
                    mode = 7000;
                    // Webpages containing standards-based !DOCTYPE directives are displayed in IE7 Standards mode. Default value for applications hosting the WebBrowser Control.
                    break;
                case 8:
                    mode = 8000;
                    // Webpages containing standards-based !DOCTYPE directives are displayed in IE8 mode. Default value for Internet Explorer 8
                    break;
                case 9:
                    mode = 9000;
                    // Internet Explorer 9. Webpages containing standards-based !DOCTYPE directives are displayed in IE9 mode. Default value for Internet Explorer 9.
                    break;
                case 10:
                    mode = 10000;
                    // Internet Explorer 10. Webpages containing standards-based !DOCTYPE directives are displayed in IE9 mode. Default value for Internet Explorer 9.
                    break;
                case 11:
                    mode = 11001;
                    // Internet Explorer 11. Webpages containing standards-based !DOCTYPE directives are displayed in IE9 mode. Default value for Internet Explorer 9.
                    break;
            }

            return mode;
        }
    }
}
