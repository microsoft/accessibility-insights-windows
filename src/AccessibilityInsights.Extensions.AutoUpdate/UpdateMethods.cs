// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using Microsoft.Deployment.WindowsInstaller;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace AccessibilityInsights.Extensions.AutoUpdate
{
    /// <summary>
    /// API for checking whether there is a new update of AccessibilityInsights
    /// For the moment, updates are done only for release builds, not for daily builds.
    /// - Relies on unique/new product version for each new update
    /// - Relies on meta file on remote drop (described below)
    /// - Relies on drop file in local ConfigurationBase.sConfigFolder (see getPathToDropFile)
    /// </summary>
    internal static class UpdateMethods
    {
        private const string UpdateGuid = "{0D760959-F713-46C4-9A3D-4E73619EE3B5}";
        private const string ReleaseLocation = "ReleaseLocation";

        /// <summary>
        /// name of file expected in remote drop
        ///  JSON format with key defined below -> relative path to latest MSI file
        /// </summary>
        private const string MetaFileName = "meta.json";

        /// <summary>
        /// Starts a process to install the MSI file at the specified location
        ///     Method returns before installation is complete
        /// </summary>
        /// <param name="location"></param>
        /// <returns></returns>
        internal static void BeginMSIInstall(string location)
        {
            Process p = new Process();
            p.StartInfo.FileName = "cmd.exe";
            p.StartInfo.UseShellExecute = false;
            p.StartInfo.CreateNoWindow = true;
            p.StartInfo.Arguments = "/K timeout /t 5 & msiexec /i " + location + " /qb-";
            p.Start();
        }

        /// <summary>
        /// Returns the overridable release location
        /// </summary>
        /// <param name="config">The config that allows override of the default settings</param>
        /// <returns>The release location</returns>
        internal static string GetReleaseLocation(OverridableConfig config)
        {
            // The BuildConstants class is generated at build time, meaning that it's possible
            // for BuildConstants.DefaultReleaseLocation to be empty. If this occurs, we'll use
            // the assembly's location as the default release location
            string defaultReleaseLocation = (BuildConstants.DefaultReleaseLocation.Length > 0) ?
                BuildConstants.DefaultReleaseLocation : Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);

            return config.GetConfigSetting(ReleaseLocation, defaultReleaseLocation);
        }

        /// <summary>
        /// Returns the product version of the currently installed
        ///     Microsoft.AccessibilityInsights application
        /// Returns null if the version could not be found
        ///     - could occur if the application has not been installed at all
        ///     - could occur if Windows Installer's cached MSI file is corrupted or deleted
        /// </summary>
        /// <returns></returns>
        internal static string GetInstalledProductVersion()
        {
            string targetUpgradeCode = UpdateGuid.ToUpperInvariant();

            // Check whether application with target upgrade code is installed on this machine
            IEnumerable<ProductInstallation> installations = ProductInstallation.GetRelatedProducts(targetUpgradeCode);
            bool existingApp;
            try
            {
                existingApp = installations.Any<ProductInstallation>();
            }
            catch (System.ArgumentException)
            {
                // occurs when the upgrade code is formatted incorrectly
                // exception text: "Parameter is incorrect"
                return null;
            }
            if (!existingApp)
            {
                // occurs when the upgrade code does not match any existing application
                return null;
            }
            ProductInstallation existingInstall = installations.FirstOrDefault<ProductInstallation>(i => i.ProductVersion != null);
            if (existingInstall == null)
            {
                return null;
            }
            string msiFilePath = existingInstall.LocalPackage;
            if (msiFilePath != null)
            {
                return GetMSIProductVersion(msiFilePath);
            }

            // Should only get here if LocalPackage not set
            return null;
        }

        /// <summary>
        /// Returns the MetaMSIInfo structure created by
        ///     deserializing the meta file in the given directory
        /// Returns null if file doesn't exist or file is formatted incorrectly
        /// Returns null if the share is not reachable within two seconds
        /// Returns null if the pathToSignedDirectory is null
        /// </summary>
        /// <param name="pathToSignedDirectory"></param>
        /// <returns></returns>
        internal static MetaMSIInfo ExtractMSIInfo(string pathToSignedDirectory)
        {
            if (pathToSignedDirectory == null)
            {
                return null;
            }
            string pathToMetaFile = Path.Combine(pathToSignedDirectory, MetaFileName);
            var task = Task.Run(() =>
            {
                if (!File.Exists(pathToMetaFile))
                {
                    return null;
                }
                try
                {
                    return JsonConvert.DeserializeObject<MetaMSIInfo>(File.ReadAllText(pathToMetaFile));
                }
                catch (Exception)
                {
                    return null;
                }
            });
            var completedInTime = task.Wait(TimeSpan.FromSeconds(2));
            if (completedInTime)
            {
                return task.Result;
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// Returns the product version of the file at the given path
        /// </summary>
        /// <param name="pathToMSI"></param>
        /// <returns></returns>
        internal static string GetMSIProductVersion(string pathToMSI)
        {
            using (Database db = new Database(pathToMSI, DatabaseOpenMode.ReadOnly))
            {
                return db.ExecutePropertyQuery("ProductVersion");
            }
        }
    }
}
