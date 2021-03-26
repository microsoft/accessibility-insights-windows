// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using AccessibilityInsights.Enums;
using AccessibilityInsights.SharedUx.Telemetry;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace AccessibilityInsights
{
    /// <summary>
    /// MainWindow partial class for set title bar text based on current page and view
    /// </summary>
    public partial class MainWindow
    {
        /// <summary>
        /// List of text mappings for Title bar text
        /// it is based on CurrentPage and CurrentView.
        /// </summary>
        private List<Tuple<AppPage, dynamic, string>> TitleTextMap = new List<Tuple<AppPage, dynamic, string>>()
        {
            new Tuple<AppPage, dynamic, string>(AppPage.Start, null, Properties.Resources.TitleTextMapStart),
            new Tuple<AppPage, dynamic, string>(AppPage.Inspect,InspectView.Live,Properties.Resources.TitleTextMapInspectLive),
            new Tuple<AppPage, dynamic, string>(AppPage.Inspect,InspectView.CapturingData, Properties.Resources.TitleTextMapInspectScan),
            new Tuple<AppPage, dynamic, string>(AppPage.Test,TestView.NoSelection,Properties.Resources.TitleTextMapTestNoSelection),
            new Tuple<AppPage, dynamic, string>(AppPage.Test,TestView.AutomatedTestResults,Properties.Resources.TitleTextMapTestResults),
            new Tuple<AppPage, dynamic, string>(AppPage.Test,TestView.CapturingData,Properties.Resources.TitleTextMapTestScan),
            new Tuple<AppPage, dynamic, string>(AppPage.Test,TestView.TabStop,Properties.Resources.TitleTextMapTestTabStops),
            new Tuple<AppPage, dynamic, string>(AppPage.Test,TestView.ElementDetails, Properties.Resources.TitleTextMapTestElementDetails),
            new Tuple<AppPage, dynamic, string>(AppPage.Test,TestView.ElementHowToFix, Properties.Resources.TitleTextMapTestHowToFix),
            new Tuple<AppPage, dynamic, string>(AppPage.Events,EventsView.Config,Properties.Resources.TitleTextMapEvents),
            new Tuple<AppPage, dynamic, string>(AppPage.Events,EventsView.Load,Properties.Resources.TitleTextMapEventsLoad),
            new Tuple<AppPage, dynamic, string>(AppPage.Events,EventsView.Recording,Properties.Resources.TitleTextMapEventsRecord),
            new Tuple<AppPage, dynamic, string>(AppPage.Events,EventsView.Stopped,Properties.Resources.TitleTextMapEventsStopped),
            new Tuple<AppPage, dynamic, string>(AppPage.CCA,CCAView.Automatic,Properties.Resources.TitleTextMapCCA),
            new Tuple<AppPage, dynamic, string>(AppPage.CCA,CCAView.CapturingData,Properties.Resources.TitleTextMapCCA),
            new Tuple<AppPage, dynamic, string>(AppPage.CCA,CCAView.Manual,Properties.Resources.TitleTextMapCCA),
            new Tuple<AppPage, dynamic, string>(AppPage.CCA,null,Properties.Resources.TitleTextMapCCA),
            new Tuple<AppPage, dynamic, string>(AppPage.Exit,null,Properties.Resources.TitleTextMapExit),
        };

        /// <summary>
        /// return Current State text based on current page and current view
        /// </summary>
        /// <returns></returns>
        private string GetCurrentStateTextBasedOnCurrentPageAndCurrentView()
        {
            try
            {
                return (from m in TitleTextMap
                        where m.Item1 == CurrentPage && m.Item2 == CurrentView
                        select m).First().Item3;
            }
#pragma warning disable CA1031 // Do not catch general exception types
            catch (Exception e)
            {
                e.ReportException();
                return "";
            }
#pragma warning restore CA1031 // Do not catch general exception types
        }

        /// <summary>
        /// Get the file extension from given file name.
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        private static string GetNormalizedFileExtension(string fileName)
        {
            string ext = null;
            try
            {
                var fi = new FileInfo(fileName);

#pragma warning disable CA1308 // Normalize strings to uppercase
                ext = fi.Extension.ToLowerInvariant();
#pragma warning restore CA1308 // Normalize strings to uppercase
            }
#pragma warning disable CA1031 // Do not catch general exception types
            catch (Exception e)
            {
                e.ReportException();
            }
#pragma warning restore CA1031 // Do not catch general exception types

            return ext;
        }
    }
}
