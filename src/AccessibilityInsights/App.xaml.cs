// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using AccessibilityInsights.SharedUx.Enums;
using AccessibilityInsights.SharedUx.Highlighting;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Threading;

namespace AccessibilityInsights
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        /// <summary>
        /// Color theme
        /// </summary>
        public enum Theme
        {
            Light,
            Dark,
            HighContrast
        }

        public static bool DisableHardwareRendering
        {
            get
            {
                // Value stored in high order word.
                // Stackoverflow: https://stackoverflow.com/questions/4951058/software-rendering-mode-wpf answer by Matt Varblow
                int renderingTier = (RenderCapability.Tier >> 16);
                return renderingTier == 0;
            }
        }

        /// <summary>
        /// Collection of Uris for theme resource dictionaries
        /// </summary>
        Dictionary<Theme, Uri> Brushes;

        /// <summary>
        /// Collection of Uris for font resource dictionaries
        /// </summary>
        Dictionary<FontSize, Uri> Fonts;

        void App_DispatcherUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
        {
            e.Handled = false;
        }

        public App()
        {
            Brushes = new Dictionary<Theme, Uri>()
            {
                { Theme.Light, new Uri(@"pack://application:,,,/AccessibilityInsights.SharedUx;component/Resources/Light/Brushes.xaml", UriKind.Absolute) },
                { Theme.Dark, new Uri(@"pack://application:,,,/AccessibilityInsights.SharedUx;component/Resources/Dark/Brushes.xaml", UriKind.Absolute) },
                { Theme.HighContrast, new Uri(@"pack://application:,,,/AccessibilityInsights.SharedUx;component/Resources/HighContrast/Brushes.xaml", UriKind.Absolute) }
            };

            Fonts = new Dictionary<FontSize, Uri>()
            {
                { FontSize.Small, new Uri(@"pack://application:,,,/AccessibilityInsights.SharedUx;component/Resources/Font/SmallFont.xaml", UriKind.Absolute) },
                { FontSize.Standard, new Uri(@"pack://application:,,,/AccessibilityInsights.SharedUx;component/Resources/Font/StandardFont.xaml", UriKind.Absolute) }
            };
        }

        /// <summary>
        /// reference to current theme resource
        /// </summary>
        private ResourceDictionary themeResourceDictionary;

        /// <summary>
        /// reference to current font resource
        /// </summary>
        private ResourceDictionary fontResourceDictionary;

        /// <summary>
        /// Set color theme
        /// </summary>
        public void SetColorTheme(Theme theme)
        {
            Resources.MergedDictionaries.Remove(this.themeResourceDictionary);
            this.themeResourceDictionary = new ResourceDictionary() { Source = Brushes[theme] };
            Resources.MergedDictionaries.Add(this.themeResourceDictionary);
            HollowHighlightDriver.ClearAllHighlighters();

            // give the window a border if in high contrast mode
            if (theme == Theme.HighContrast)
            {
                App.Current.MainWindow.BorderThickness = new Thickness(4);
            }
            else
            {
                App.Current.MainWindow.BorderThickness = new Thickness(0);
            }
        }

        public void SetFontSize(FontSize fontSize)
        {
            Resources.MergedDictionaries.Remove(this.fontResourceDictionary);
            this.fontResourceDictionary = new ResourceDictionary() { Source = Fonts[fontSize] };
            Resources.MergedDictionaries.Add(this.fontResourceDictionary);
        }

        protected override void OnStartup(StartupEventArgs e)
        {
            // StackOverflow: https://stackoverflow.com/questions/4951058/software-rendering-mode-wpf answer by Matt Varblow
            if (DisableHardwareRendering)
            {
                RenderOptions.ProcessRenderMode = RenderMode.SoftwareOnly;
            }

            // WPF hides tooltips after a few seconds, which is bad for accessibility.
            // Override the default to 1 day
            ToolTipService.ShowDurationProperty.OverrideMetadata(typeof(UIElement),
                new FrameworkPropertyMetadata((int)TimeSpan.FromDays(1).TotalMilliseconds));
        }
    }
}
