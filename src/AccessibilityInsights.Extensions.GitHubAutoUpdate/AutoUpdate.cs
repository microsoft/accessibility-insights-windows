// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using AccessibilityInsights.Extensions.Helpers;
using AccessibilityInsights.Extensions.Interfaces.Upgrades;
using AccessibilityInsights.SetupLibrary;
using System;
using System.ComponentModel.Composition;
using System.Diagnostics;
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
    /// code defaults to the "default" channel, but this can be overridden by the caller.
    ///
    /// A note about timing: Due to the way that extensions get loaded, our constructor may get
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
        private readonly IChannelInfoProvider _channelInfoProvider;
        private readonly Func<string> _installedVersionProvider;
        private readonly Task<AutoUpdateOption> _initTask;
        private Version _installedVersion;
        private Version _currentChannelVersion;
        private Version _minimumChannelVersion;
        private Uri _releaseNotesUri;
        private Uri _installerUri;
        private int _msiSizeInBytes;
        private string _msiSha512;
        private readonly Stopwatch _initializationStopwatch = new Stopwatch();
        private readonly Stopwatch _updateStopwatch = new Stopwatch();
        private readonly ReleaseChannel _strongReleaseChannel;
        private Uri _manifestRequestUri;
        private Uri _manifestResponseUri;
        private int _manifestSizeInBytes;
        private static readonly IExceptionReporter ExceptionReporter = new ExceptionReporter();

        /// <summary>
        /// Implements <see cref="IAutoUpdate.ReleaseChannel"/>
        /// </summary>
        public string ReleaseChannel { get; }

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
        /// Implements <see cref="IAutoUpdate.ManifestRequestUri"/>
        /// </summary>
        public Uri ManifestRequestUri
        {
            get
            {
                WaitForInitializationToComplete();
                return _manifestRequestUri;
            }
        }

        /// <summary>
        /// Implements <see cref="IAutoUpdate.ManifestResponseUri"/>
        /// </summary>
        public Uri ManifestResponseUri
        {
            get
            {
                WaitForInitializationToComplete();
                return _manifestResponseUri;
            }
        }

        /// <summary>
        /// Implements <see cref="IAutoUpdate.ManifestSizeInBytes"/>
        /// </summary>
        public int ManifestSizeInBytes
        {
            get
            {
                WaitForInitializationToComplete();
                return _manifestSizeInBytes;
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

                VersionSwitcherWrapper.InstallUpgrade(_installerUri, _msiSizeInBytes, _msiSha512);
                return UpdateResult.Success;
            }
#pragma warning disable CA1031 // Do not catch general exception types
            catch (Exception e)
            {
                e.ReportException();
            }
#pragma warning restore CA1031 // Do not catch general exception types
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
        /// Production constructor - MUST be a default constructor or extensions will break
        /// </summary>
#pragma warning disable RS0034 // Exported parts should have [ImportingConstructor]
        public AutoUpdate() :
            this(ConfiguredReleaseChannelProvider, () => MsiUtilities.GetInstalledProductVersion(ExceptionReporter),
                new ProductionChannelInfoProvider(new GitHubWrapper(ExceptionReporter), ExceptionReporter))
        {
        }
#pragma warning restore RS0034 // Exported parts should have [ImportingConstructor]

        /// <summary>
        /// Unit testable constructor - allows dependency injection for testing
        /// </summary>
        /// <param name="releaseChannelProvider">Provides the client's current release channel</param>
        /// <param name="installedVersionProvider">Method that provides the installed version string</param>
        /// <param name="channelInfoProvider">Method that provides a (potentially invalid) ChannelInfo</param>
#pragma warning disable RS0034 // Exported parts should have [ImportingConstructor]
        internal AutoUpdate(Func<ReleaseChannel> releaseChannelProvider, Func<string> installedVersionProvider, IChannelInfoProvider channelInfoProvider)
        {
            _strongReleaseChannel = releaseChannelProvider();
            _installedVersionProvider = installedVersionProvider;
            _channelInfoProvider = channelInfoProvider;
            ReleaseChannel = _strongReleaseChannel.ToString();
            _initTask = Task.Run(() => InitializeWithTimer());
        }
#pragma warning restore RS0034 // Exported parts should have [ImportingConstructor]

        private static ReleaseChannel ConfiguredReleaseChannelProvider()
        {
            if (Enum.TryParse<ReleaseChannel>(ReleaseChannelProvider.ReleaseChannel, out ReleaseChannel releaseChannel))
            {
                return releaseChannel;
            }

            // Default value
            return SetupLibrary.ReleaseChannel.Production;
        }

        private AutoUpdateOption InitializeWithTimer()
        {
            _initializationStopwatch.Restart();
            AutoUpdateOption updateOption = Initialize();
            _initializationStopwatch.Stop();
            return updateOption;
        }

        /// <summary>
        /// Do not call this function directly.
        /// Instead, call InitializeWithTimer.
        /// </summary>
        /// <remarks>This function MUST NOT leak any exceptions</remarks>
        private AutoUpdateOption Initialize()
        {
            // Do NOT use anything that calls WaitForInitializationToComplete in this
            // method, or you may create a deadlock condition
            try
            {
                if (Version.TryParse(_installedVersionProvider(), out _installedVersion))
                {
                    if (_channelInfoProvider.TryGetChannelInfo(_strongReleaseChannel, out EnrichedChannelInfo enrichedChannelInfo) &&
                        enrichedChannelInfo.IsValid)
                    {
                        _manifestRequestUri = enrichedChannelInfo.Metadata.RequestUri;
                        _manifestResponseUri = enrichedChannelInfo.Metadata.ResponseUri;
                        _manifestSizeInBytes = enrichedChannelInfo.Metadata.DataByteCount;
                        _msiSizeInBytes = enrichedChannelInfo.MsiSizeInBytes;
                        _msiSha512 = enrichedChannelInfo.MsiSha512;
                        _currentChannelVersion = enrichedChannelInfo.CurrentVersion;
                        _minimumChannelVersion = enrichedChannelInfo.MinimumVersion;
                        _releaseNotesUri = new Uri(enrichedChannelInfo.ReleaseNotesAsset, UriKind.Absolute);
                        _installerUri = new Uri(enrichedChannelInfo.InstallAsset, UriKind.Absolute);

                        if (_installedVersion < _minimumChannelVersion)
                        {
                            return AutoUpdateOption.RequiredUpgrade;
                        }
                        if (_installedVersion < _currentChannelVersion)
                        {
                            return AutoUpdateOption.OptionalUpgrade;
                        }
                        if (_installedVersion > _currentChannelVersion)
                        {
                            return AutoUpdateOption.NewerThanCurrent;
                        }
                        return AutoUpdateOption.Current;
                    }
                }
            }
#pragma warning disable CA1031 // Do not catch general exception types
            catch (Exception e)
            {
                e.ReportException();
            }
#pragma warning restore CA1031 // Do not catch general exception types

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
