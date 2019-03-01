// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using AccessibilityInsights.Extensions.Helpers;
using AccessibilityInsights.Extensions.Interfaces.Upgrades;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AccessibilityInsights.Extensions.GitHubAutoUpdate
{
    /// <summary>
    /// GitHub-based implementation of IAutoUpdate. Basic strategy is as follows:
    /// 1) Fetch a JSON file from a specified web location (typically in the GitHub repo)
    /// 2) Extract the version information from the JSON file, then compare that to the
    ///    version of the currently installed app
    /// 3) Cache the information needed to get the installer and release notes, then return
    ///    the upgrade status to the caller
    /// 4) If requested, use the cached information to download the installer, which will
    ///    be stored locally and validate before is it launched.
    ///
    /// To allow multiple Release Cadences, the JSON file can report back more than one
    /// set of information (each CadenceInfo object holds the data for one cadence). The
    /// code defaults to the "stable" cadence, but this can be overridden by the caller.
    ///
    /// A note about timing: Due to the way that extensions get loaded, our ctor may get
    /// called some time before the app requests the upgrade status. Since we need to make
    /// a web call to get the data, we take advantage of that extra time to fetch the data
    /// from the web. We do that in a separate Task to prevent blocking the app. That means,
    /// however, that when we app calls to request the data, we can't answer until after the
    /// web call has completed. That's why you see WaitForInitializationToComplete in many
    /// of the property getters.
    /// </summary>
    [Export(typeof(IAutoUpdate))]
    public class AutoUpdate : IAutoUpdate
    {
        // The release cadence we use unless overridden by setting the ReleaseCadence property
        private const string DefaultReleaseCadence = "default";

        private readonly Func<string> _installedVersionProvider;
        private readonly IGitHubWrapper _gitHub;
        private readonly Task<AutoUpdateOption> _initTask;
        private Version _installedVersion;
        private Version _latestVersion;
        private Version _minimumVersion;
        private Uri _releaseNotesUri;
        private Uri _installerUri;
        private readonly Stopwatch _initializationStopwatch = new Stopwatch();
        private readonly Stopwatch _installerDownloadStopwatch = new Stopwatch();
        private readonly Stopwatch _installerVerificationStopwatch = new Stopwatch();

        /// <summary>
        /// Implements <see cref="IAutoUpdate.ReleaseCadence"/>
        /// </summary>
        public string ReleaseCadence { get; set; } = DefaultReleaseCadence;

        /// <summary>
        /// Implements <see cref="IAutoUpdate.InstalledVersion"/>
        /// </summary>
        public Version InstalledVersion
        {
            get
            {
                WaitForInitializationToComplete();
                return _installedVersion;
            }
        }

        /// <summary>
        /// Reports the latest version being considered
        /// </summary>
        public Version LatestVersion
        {
            get
            {
                WaitForInitializationToComplete();
                return _latestVersion;
            }
        }

        /// <summary>
        /// Reports the minimum required version
        /// </summary>
        public Version MinimumVersion
        {
            get
            {
                WaitForInitializationToComplete();
                return _minimumVersion;
            }
        }

        /// <summary>
        /// The Uri to the release notes
        /// </summary>
        public Uri ReleaseNotesUri
        {
            get
            {
                WaitForInitializationToComplete();
                return _releaseNotesUri;
            }
        }

        /// <summary>
        /// Implements <see cref="IAutoUpdate.UpdateAsync"/>
        /// </summary>
        public Task<UpdateResult> UpdateAsync()
        {
            return Task.Run(() => Update());
        }

        /// <summary>
        /// Synchronously update (gets wrapped into a tag)
        /// </summary>
        /// <returns>The result of the upgrade operation</returns>
        private UpdateResult Update()
        {
            string tempFile = Path.ChangeExtension(Path.GetTempFileName(), "msi");

            // Reset here; in case anything goes wrong in the Interim, the value will reflect that.
            _installerVerificationStopwatch.Reset();

            try
            {
                WaitForInitializationToComplete();

                if (!TryDownloadInstaller(tempFile))
                    return UpdateResult.DownloadFailed;

                // The verification wraps the beginning of the installation to preserve
                // the integrity of the file by holding an open handle.
                _installerVerificationStopwatch.Start();
                using (var trustVerifier = new TrustVerifier(tempFile))
                {
                    if (!trustVerifier.IsVerified)
                        return UpdateResult.VerificationFailed;

                    _installerVerificationStopwatch.Stop();

                    UpdateMethods.BeginMSIInstall(tempFile);

                    return UpdateResult.Success;
                }
            }
            catch (Exception e)
            {
                e.ReportException();
                if (File.Exists(tempFile))
                    File.Delete(tempFile);
            }
            finally
            {
                _installerVerificationStopwatch.Stop();
            }

            return UpdateResult.Unknown;
        }

        /// <summary>
        /// Implements <see cref="IAutoUpdate.UpdateOptionAsync"/>
        /// </summary>
        public Task<AutoUpdateOption> UpdateOptionAsync => _initTask;

        /// <summary>
        /// Implements <see cref="IAutoUpdate.GetInitializationTime"/>
        /// </summary>
        public TimeSpan? GetInitializationTime()
        {
            return _initializationStopwatch.Elapsed;
        }

        /// <summary>
        /// Implements <see cref="IAutoUpdate.GetInstallerDownloadTime"/>
        /// </summary>
        public TimeSpan? GetInstallerDownloadTime()
        {
            return _installerDownloadStopwatch.Elapsed;
        }

        /// <summary>
        /// Implements <see cref="IAutoUpdate.GetInstallerVerificationTime"/>
        /// </summary>
        public TimeSpan? GetInstallerVerificationTime()
        {
            return _installerVerificationStopwatch.Elapsed;
        }

        /// <summary>
        /// Production ctor
        /// </summary>
        public AutoUpdate() : this(new GitHubWrapper(), UpdateMethods.GetInstalledProductVersion)
        {
        }

        /// <summary>
        /// Unit test ctor - allows dependency injection for testing
        /// </summary>
        /// <param name="wrapper">Provides GitHub support</param>
        /// <param name="installedVersionProvider">Where to get the installed version</param>
        internal AutoUpdate(IGitHubWrapper wrapper, Func<string> installedVersionProvider)
        {
            _gitHub = wrapper;
            _installedVersionProvider = installedVersionProvider;
            _initTask = Task.Run(() => InitializeWithTimer());
        }

        private static bool TryGetCadencesFromStream(Stream stream, out Dictionary<string, CadenceInfo> cadences)
        {
            cadences = new Dictionary<string, CadenceInfo>();
            stream.Position = 0;
            StreamReader reader = new StreamReader(stream, Encoding.UTF8);
            string configInfo = reader.ReadToEnd();
            Dictionary<string, CadenceInfo> rawResults = JsonConvert.DeserializeObject<Dictionary<string, CadenceInfo>>(configInfo);

            foreach (KeyValuePair<string, CadenceInfo> pair in rawResults)
            {
                if (pair.Value.IsValid)
                {
                    cadences.Add(pair.Key, pair.Value);
                }
            }

            return cadences.Any();
        }

        private bool TryParseConfigInfo(Stream stream, string cadence)
        {
            if (_gitHub.TryGetConfigInfo(stream))
            {
                if (cadence != null)
                {
                    try
                    {
                        if (TryGetCadencesFromStream(stream, out Dictionary<string, CadenceInfo> cadences))
                        {
                            if (cadences.TryGetValue(cadence, out CadenceInfo cadenceInfo))
                            {
                                _latestVersion = cadenceInfo.CurrentVersion;
                                _minimumVersion = cadenceInfo.MinimumVersion;
                                _releaseNotesUri = new Uri(cadenceInfo.ReleaseNotesAsset, UriKind.Absolute);
                                _installerUri = new Uri(cadenceInfo.InstallAsset, UriKind.Absolute);
                                return true;
                            }
                        }
                    }
                    catch (Exception e)
                    {
                        e.ReportException();
                        Trace.WriteLine("AccessibilityInsights upgrade - exception when converting the config data: "
                            + e.ToString());
                    }
                }
            }

            // Default values
            _latestVersion = null;
            _minimumVersion = null;
            _releaseNotesUri = null;
            _installerUri = null;
            return false;
        }

        private AutoUpdateOption InitializeWithTimer()
        {
            _initializationStopwatch.Reset();

            try
            {
                _initializationStopwatch.Start(); // stopped in the finally block
                return Initialize();
            }
            catch (Exception e)
            {
                e.ReportException();
                Trace.WriteLine($"Unable to get update info from meta file at {e.Message}");
            }
            finally
            {
                _initializationStopwatch.Stop();
            }

            return AutoUpdateOption.Unknown;  // Our fallback value if we can't prove a better option
        }

        /// <summary>
        /// Do not call this function directly.
        /// Instead, call InitializeWithTimer.
        /// </summary>
        /// <returns></returns>
        private AutoUpdateOption Initialize()
        {
            // Do NOT use anything that calls WaitForInitializationToComplete in this
            // method, or you may create a deadlock condition
            if (Version.TryParse(_installedVersionProvider(), out _installedVersion))
            {
                using (Stream configStream = new MemoryStream())
                {
                    if (TryParseConfigInfo(configStream, ReleaseCadence))
                    {
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
                } // using
            }

            return AutoUpdateOption.Unknown;
        }

        private void WaitForInitializationToComplete()
        {
            _initTask.Wait();
        }

        private bool TryDownloadInstaller(string targetFilePath)
        {
            _installerDownloadStopwatch.Reset();

            using (Stream stream = new FileStream(targetFilePath, FileMode.CreateNew))
            {
                _installerDownloadStopwatch.Start();

                try
                {
                    return TryGetTargetAsset(_installerUri.ToString(), stream);
                }
                catch (Exception e)
                {
                    e.ReportException();
                    Debug.WriteLine(e.ToString());
                }
                finally
                {
                    _installerDownloadStopwatch.Stop();
                }
            } // using

            return false;
        }

        private bool TryGetTargetAsset(string assetName, Stream stream)
        {
            if (assetName == null)
                return false;

            if (_gitHub.TryGetSpecificAsset(new Uri(assetName), stream))
            {
                stream.Flush();
                stream.Seek(0, SeekOrigin.Begin);

                StreamReader reader = new StreamReader(stream);
                return true;
            }

            return false;
        }
    }
}
