// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using AccessibilityInsights.Enums;
using AccessibilityInsights.SharedUx.Telemetry;
using System;

namespace AccessibilityInsights.Misc
{
    internal static class PageTracker
    {
        /// <summary>
        /// Track page - updates context properties with given parameters
        /// </summary>
        /// <param name="page"></param>
        /// <param name="view"></param>
        /// <param name="framework">name of UI framework</param>
        public static void TrackPage(AppPage page, string view, string framework = null)
        {
            if (!Logger.IsEnabled)
                return;

            if (view != null)
            {
                Logger.AddOrUpdateContextProperty(TelemetryProperty.View, view);
            }
            if (framework != null)
            {
                Logger.AddOrUpdateContextProperty(TelemetryProperty.UIFramework, framework);
            }
            Logger.AddOrUpdateContextProperty(TelemetryProperty.ModeName, page.ToString());
            Logger.AddOrUpdateContextProperty(TelemetryProperty.ModeSessionId, Guid.NewGuid().ToString());
        }
    }
}
