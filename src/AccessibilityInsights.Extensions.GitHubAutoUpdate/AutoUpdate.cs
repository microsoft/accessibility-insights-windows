// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using AccessibilityInsights.Extensions.Helpers;
using AccessibilityInsights.Extensions.Interfaces.Upgrades;
using AccessibilityInsights.SetupLibrary;
using System;
using System.ComponentModel.Composition;
using System.Diagnostics;
using System.IO;
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
    /// To allow multiple Release Channels, the JSON file can report back more than one
    /// set of information (each ChannelInfo object holds the data for one channel). The
    /// code defaults to the "deafult" channel, but this can be overridden by the caller.
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
        // The Channel we use unless overridden by setting the ReleaseChannel property
        private const string DefaultReleaseChannel = "default";

        // Delegate for test ctor -- can't use generics because of the out parameter
        internal delegate bool ChannelInfoProvider(IGitHubWrapper gitHubWrapper, string releaseChannel, out ChannelInfo channelInfo);

        private readonly ChannelInfoProvider _channelInfoProvider;
        private readonly Func<string> _installedVersionProvider;
        private readonly IGitHubWrapper _gitHubWrapper;
        private readonly Task<AutoUpdateOption> _initTask;
        private Version _installedVersion;
        private Version _currentChannelVersion;
        private Version _minimumChannelVersion;
        private Uri _releaseNotesUri;
        private Uri _installerUri;
        private readonly Stopwatch _initializationStopwatch = new Stopwatch();
        private readonly Stopwatch _updateStopwatch = new Stopwatch();

        private static readonly IExceptionReporter ExceptionReporter = new ExceptionReporter();

        /// <summary>
        /// Implements <see cref="IAutoUpdate.ReleaseChannel"/>
        /// </summary>
        public string ReleaseChannel { get; set; } = DefaultReleaseChannel;

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
        /// Reports the current channel version
        /// </summary>
        public Version CurrentChannelVersion
        {
            get
            {
                WaitForInitializationToComplete();
                return _currentChannelVersion;
            }
        }

        /// <summary>
        /// Reports the minimum required channel version
        /// </summary>
        public Version MinimumChannelVersion
        {
            get
            {
                WaitForInitializationToComplete();
                return _minimumChannelVersion;
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
        /// Synchronously update
        /// </summary>
        /// <returns>The result of the upgrade operation</returns>
        private UpdateResult Update()
        {
            // Reset here; in case anything goes wrong in the Interim, the value will reflect that.
            _updateStopwatch.Restart();

            try
            {
                WaitForInitializationToComplete();

                // Short-circuit updates that don't make sense
                if (UpdateOptionAsync.Result != AutoUpdateOption.OptionalUpgrade &&
                    UpdateOptionAsync.Result != AutoUpdateOption.RequiredUpgrade)
                {
                    return UpdateResult.NoUpdateAvailable;
                }

                if (VersionSwitcherWrapper.InstallUpgrade(_installerUri))
                {
                    return UpdateResult.Success;
                }
            }
            catch (Exception e)
            {
                e.ReportException();
            }
            finally
            {
                _updateStopwatch.Stop();
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
        /// Implements <see cref="IAutoUpdate.GetUpdateTime"/>
        /// </summary>
        public TimeSpan? GetUpdateTime()
        {
            return _updateStopwatch.Elapsed;
        }

        /// <summary>
        /// Production ctor
        /// </summary>
        public AutoUpdate(string releaseChannel = null) : this(releaseChannel, new GitHubWrapper(ExceptionReporter), MsiUtilities.GetInstalledProductVersion, TryGetChannelInfo)
        {
        }

        /// <summary>
        /// Unit test ctor - allows dependency injection for testing
        /// </summary>
        /// <param name="gitHubWrapper">Provides GitHub support</param>
        /// <param name="installedVersionProvider">Where to get the installed version</param>
        internal AutoUpdate(string releaseChannel, IGitHubWrapper gitHubWrapper, Func<string> installedVersionProvider, ChannelInfoProvider channelInfoProvider)
        {
            ReleaseChannel = releaseChannel ?? DefaultReleaseChannel;
            _gitHubWrapper = gitHubWrapper;
            _installedVersionProvider = installedVersionProvider;
            _channelInfoProvider = channelInfoProvider;
            _initTask = Task.Run(() => InitializeWithTimer());
        }

        /// <summary>
        /// Production code to retrieve a ChannelInfo from the web-based config file
        /// </summary>
        /// <param name="gitHub">Wrapper to access GitHub mechanisms</param>
        /// <param name="releaseChannel">The channel being requested</param>
        /// <param name="channelInfo">Receives the data if found</param>
        /// <returns>true if the data was located, otherwise false</returns>
        private static bool TryGetChannelInfo(IGitHubWrapper gitHub, string releaseChannel, out ChannelInfo channelInfo)
        {
            try
            {
                using (Stream stream = new MemoryStream())
                {
                    gitHub.LoadChannelInfoIntoStream(releaseChannel, stream);
                    return ChannelInfo.TryGetChannelFromStream(stream, releaseChannel, out channelInfo, ExceptionReporter);
                }
            }
            catch (Exception e)
            {
                ExceptionReporter.ReportException(e);
            }

            // Default values
            channelInfo = null;
            return false;
        }

        private AutoUpdateOption InitializeWithTimer()
        {
            _initializationStopwatch.Restart();

            try
            {
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
            try
            {
                if (Version.TryParse(_installedVersionProvider(), out _installedVersion))
                {
                    if (_channelInfoProvider(_gitHubWrapper, ReleaseChannel, out ChannelInfo channelInfo) && channelInfo.IsValid)
                    {
                        _currentChannelVersion = channelInfo.CurrentVersion;
                        _minimumChannelVersion = channelInfo.MinimumVersion;
                        _releaseNotesUri = new Uri(channelInfo.ReleaseNotesAsset, UriKind.Absolute);
                        _installerUri = new Uri(channelInfo.InstallAsset, UriKind.Absolute);

                        if (_installedVersion < _minimumChannelVersion)
                        {
                            return AutoUpdateOption.RequiredUpgrade;
                        }
                        else if (_installedVersion < _currentChannelVersion)
                        {
                            return AutoUpdateOption.OptionalUpgrade;
                        }
                        return AutoUpdateOption.Current;
                    }
                }
            }
            catch (Exception e)
            {
                e.ReportException();
            }

            // Default values
            _currentChannelVersion = null;
            _minimumChannelVersion = null;
            _releaseNotesUri = null;
            _installerUri = null;
            return AutoUpdateOption.Unknown;
        }

        private void WaitForInitializationToComplete()
        {
            _initTask.Wait();
        }
    }
}
