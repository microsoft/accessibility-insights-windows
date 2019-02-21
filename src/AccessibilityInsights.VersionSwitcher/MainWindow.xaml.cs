// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;
using System.Diagnostics;
using System.IO;
using System.Windows;

namespace AccessibilityInsights.VersionSwitcher
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        const string ProductName = "Accessibility Insights For Windows v1.1";

        public MainWindow()
        {
            InitializeComponent();
            Hide();

            if (TryGetMsiPath(out string msiPath))
            {
                InstallHelper.DeleteOldVersion(ProductName);
                InstallHelper.InstallNewVersion(msiPath);
            }

            Close();
        }

        private static bool TryGetMsiPath(out string msiPath)
        {
            // Temporary implementation for testing--still need to finalize actual command line
            string[] args = Environment.GetCommandLineArgs();
            if (args.Length > 1)
            {
                if (File.Exists(args[1]))
                {
                    msiPath = args[1];
                    return true;
                }
                else
                {
                    Trace.TraceError("Invalid Path: " + args[1]);
                }
            }
            else
            {
                Trace.TraceError("No msi path specified!");
            }

            msiPath = null;
            return false;
        }
    }
}
