// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using AccessibilityInsights.Extensions.Helpers;
using AccessibilityInsights.Extensions.Interfaces.Upgrades;
using System;
using System.ComponentModel.Composition;
using System.IO;
using System.Threading.Tasks;

namespace AccessibilityInsights.Extensions.AutoUpdate
{
    [Export(typeof(IAutoUpdate))]
    public class UpgradeHelper : IAutoUpdate
    {
        const string ConfigFile = "AutoUpdate.settings";
        const int MaxSecondsForDownload = 120;

        private readonly Task<AutoUpdateOption> _initTask;
        private MetaMSIInfo _metaMSISettings;
        private Version _installedVersion;
        private Version _latestVersion;
        private Version _minimumVersion;
        private string _releaseLocation = string.Empty;
        private string _installerLocation = string.Empty;

        public string ReleaseChannel { get; set; } = "default";

        public Version InstalledVersion
        {
            get
            {
                WaitForInitializationToComplete();
                return _installedVersion;
            }
        }

        public Version LatestVersion
        {
            get
            {
                WaitForInitializationToComplete();
                return _latestVersion;
            }
        }

        public Version MinimumVersion
        {
            get
            {
                WaitForInitializationToComplete();
                return _minimumVersion;
            }
        }

        public Uri ReleaseNotesUri
        {
            get
            {
                WaitForInitializationToComplete();
                return Uri.TryCreate(_metaMSISettings?.ReleaseNotes, UriKind.Absolute, out Uri uri) ?
                    uri : null;
            }
        }

        public Task<AutoUpdateOption> UpdateOptionAsync => _initTask;

        public TimeSpan? GetInitializationTime() { return null; }
        public TimeSpan? GetUpdateTime() { return null; }

        private AutoUpdateOption WaitForInitializationToComplete()
        {
            return _initTask.Result;
        }

        public UpgradeHelper()
        {
            _initTask = Task.Run<AutoUpdateOption>(() =>Initialize());
        }

        private AutoUpdateOption Initialize()
        {
            // Do NOT use anything that calls WaitForInitializationToComplete in this
            // method, or you may create a deadlock condition
            try
            {
                OverridableConfig config = new OverridableConfig(ConfigFile);
                _releaseLocation = UpdateMethods.GetReleaseLocation(config);
                _metaMSISettings = UpdateMethods.ExtractMSIInfo(_releaseLocation);

                if (_metaMSISettings == null)
                {
                    // silently ignore
                    System.Diagnostics.Trace.WriteLine($"Unable to get update info from meta file at {_releaseLocation}");
                    return AutoUpdateOption.Unknown;
                }

                _installerLocation = _metaMSISettings.GetMSIPathSafely(_releaseLocation);
#pragma warning disable CA1806 // Do not ignore method results
                Version.TryParse(_metaMSISettings.Version, out _latestVersion);
                Version.TryParse(_metaMSISettings.MinimumVersion, out _minimumVersion);
                Version.TryParse(UpdateMethods.GetInstalledProductVersion(), out _installedVersion);
#pragma warning restore CA1806 // Do not ignore method results

                if (_installedVersion != null && _latestVersion != null && _minimumVersion != null)
                {
                    if (_latestVersion < _minimumVersion)
                    {
                        return AutoUpdateOption.Unknown;
                    }
                    if (_installedVersion < _minimumVersion)
                    {
                        return AutoUpdateOption.RequiredUpgrade;
                    }
                    else if (_installedVersion < _latestVersion)
                    {
                        return AutoUpdateOption.OptionalUpgrade;
                    }
                    return AutoUpdateOption.Current;
                }
            }
            catch (Exception e)
            {
                e.ReportException();
            }
            return AutoUpdateOption.Unknown;  // Our fallback value if we can't prove a better option
        }

        public Task<UpdateResult> UpdateAsync()
        {
            return Task.Run<UpdateResult>(() =>
            {
                try
                {
                    WaitForInitializationToComplete();
                    var ext = Path.GetExtension(_installerLocation);
                    var tempFile = Path.ChangeExtension(Path.GetTempFileName(), ext);

                    if (!CopyFile(_installerLocation, tempFile))
                        return UpdateResult.DownloadFailed;

                    using (var trustVerifier = new TrustVerifier(tempFile))
                    {
                        if (!trustVerifier.IsVerified)
                        {
                            return UpdateResult.VerificationFailed;
                        }
                        UpdateMethods.BeginMSIInstall(tempFile);
                    }

                    return UpdateResult.Success;
                }
                catch (Exception e)
                {
                    e.ReportException();
                    return UpdateResult.Unknown;
                }
            });
        }

        private static bool CopyFile(string source, string target)
        {
            var task = Task.Run(() => File.Copy(source, target, true));
            return task.Wait(TimeSpan.FromSeconds(MaxSecondsForDownload));
        }
    }
}
