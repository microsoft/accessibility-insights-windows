// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using Microsoft.Deployment.WindowsInstaller;
using Microsoft.Win32;
using System;
using System.Diagnostics;
using System.Globalization;

namespace AccessibilityInsights.VersionSwitcher
{
    internal static class InstallHelper
    {
        internal static bool InstallNewVersion(string msiPath)
        {
            try
            {
                Trace.TraceInformation("Attempting to install from \"{0}\"", msiPath);
                Stopwatch stopwatch = Stopwatch.StartNew();
                Installer.SetInternalUI(InstallUIOptions.Silent);
                Installer.InstallProduct(msiPath, "");
                stopwatch.Stop();
                Trace.TraceInformation("Installed {0} in {1} milliseconds", msiPath, stopwatch.ElapsedMilliseconds);
                return true;
            }
            catch (ArgumentException e)
            {
                Trace.TraceError("Caught ArgumentException: " + e.ParamName + " " + e.Message);
            }
            catch (Exception e)
            {
                Trace.TraceError("Caught Exception: " + e);
            }
            return false;
        }

        internal static bool DeleteOldVersion(string productName)
        {
            try
            {
                Trace.TraceInformation("Attempting to find product: \"{0}\"", productName);
                Stopwatch stopwatch = Stopwatch.StartNew();
                string productId = FindInstalledProductKey(productName).ToString("B", CultureInfo.InvariantCulture);
                stopwatch.Stop();

                Trace.TraceInformation("Found productId: {0} in {1} milliseconds", productId, stopwatch.ElapsedMilliseconds);

                stopwatch.Restart();
                Installer.SetInternalUI(InstallUIOptions.Silent);
                Installer.ConfigureProduct(productId, 0, InstallState.Absent, "");
                stopwatch.Start();
                Trace.TraceInformation("Deleted productId: {0} in {1} milliseconds", productId, stopwatch.ElapsedMilliseconds);
                return true;
            }
            catch (ArgumentException e)
            {
                Trace.TraceError("Caught ArgumentException: " + e.ParamName + " " + e.Message);
            }
            catch (Exception e)
            {
                Trace.TraceError("Caught Exception: " + e);
            }
            return false;
        }

        private static Guid FindInstalledProductKey(string productName)
        {
            RegistryKey productsKey = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Uninstall");
            {
                foreach (string keyName in productsKey.GetSubKeyNames())
                {
                    RegistryKey subKey = productsKey.OpenSubKey(keyName);
                    string registryProductName = (string)subKey.GetValue("DisplayName", string.Empty);

                    if (registryProductName.Equals(productName, StringComparison.OrdinalIgnoreCase))
                    {
                        return new Guid(keyName);
                    }
                }
            }

            throw new ArgumentException("Unable to locate key for product: " + productName, nameof(productName));
        }
    }
}
