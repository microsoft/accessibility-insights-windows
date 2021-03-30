// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System.Windows;
using System.Windows.Automation.Peers;
using System.Windows.Controls;
using System.Windows.Media;

namespace AccessibilityInsights.CommonUxComponents.Controls
{
    /// <summary>
    /// Display two overlapping fabric icons
    /// </summary>
    public partial class DualFabricIconControl : UserControl
    {

        /// <summary>
        /// Name of icon in front to be drawn
        /// </summary>
        #region GlyphNameFront (Dependency Property)

        public static readonly DependencyProperty GlyphNameFrontProperty =
            DependencyProperty.Register("GlyphNameFront", typeof(FabricIcon?), typeof(DualFabricIconControl), new PropertyMetadata(null, OnGlyphNameFrontChanged));

        public FabricIcon? GlyphNameFront
        {
            get { return (FabricIcon)GetValue(GlyphNameFrontProperty); }

            set { SetValue(GlyphNameFrontProperty, value); }
        }

        public static void OnGlyphNameFrontChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            DualFabricIconControl sender = o as DualFabricIconControl;

            if (sender != null)
            {
                sender.fabicnFront.GlyphName = sender.GlyphNameFront;
            }
        }

        #endregion

        /// <summary>
        /// Size of icon in front
        /// </summary>
        #region IconSizeFront (Dependency Property)

        public static readonly DependencyProperty IconSizeFrontProperty =
            DependencyProperty.Register("IconSizeFront", typeof(double), typeof(DualFabricIconControl), new PropertyMetadata(1.0, OnIconSizeFrontChanged));

        public double IconSizeFront
        {
            get { return (double)GetValue(IconSizeFrontProperty); }

            set { SetValue(IconSizeFrontProperty, value); }
        }

        public static void OnIconSizeFrontChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            DualFabricIconControl sender = o as DualFabricIconControl;

            if (sender != null)
            {
                sender.fabicnFront.FontSize = sender.IconSizeFront;
            }
        }

        #endregion

        /// <summary>
        /// Color of icon in front
        /// </summary>
        #region Foreground (Dependency Property)

        new public static readonly DependencyProperty ForegroundProperty =
            DependencyProperty.Register("Foreground", typeof(Brush), typeof(DualFabricIconControl), new PropertyMetadata(null, OnForegroundChanged));

        new public Brush Foreground
        {
            get { return (Brush)GetValue(ForegroundProperty); }

            set { SetValue(ForegroundProperty, value); }
        }

        public static void OnForegroundChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            DualFabricIconControl sender = o as DualFabricIconControl;

            if (sender != null)
            {
                sender.fabicnFront.Foreground = sender.Foreground;
            }
        }

        #endregion

        /// <summary>
        /// Name of icon in back to be drawn
        /// </summary>
        #region GlyphNameBack (Dependency Property)

        public static readonly DependencyProperty GlyphNameBackProperty =
            DependencyProperty.Register("GlyphNameBack", typeof(FabricIcon?), typeof(DualFabricIconControl), new PropertyMetadata(null, OnGlyphNameBackChanged));

        public FabricIcon? GlyphNameBack
        {
            get { return (FabricIcon)GetValue(GlyphNameBackProperty); }

            set { SetValue(GlyphNameBackProperty, value); }
        }

        public static void OnGlyphNameBackChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            DualFabricIconControl sender = o as DualFabricIconControl;

            if (sender != null)
            {
                sender.fabicnBack.GlyphName = sender.GlyphNameBack;
            }
        }

        #endregion

        /// <summary>
        /// Size of icon in back
        /// </summary>
        #region IconSizeBack (Dependency Property)

        public static readonly DependencyProperty IconSizeBackProperty =
            DependencyProperty.Register("IconSizeBack", typeof(double), typeof(DualFabricIconControl), new PropertyMetadata(1.0, OnIconSizeBackChanged));

        public double IconSizeBack
        {
            get { return (double)GetValue(IconSizeBackProperty); }

            set { SetValue(IconSizeBackProperty, value); }
        }

        public static void OnIconSizeBackChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            DualFabricIconControl sender = o as DualFabricIconControl;

            if (sender != null)
            {
                sender.fabicnBack.FontSize = sender.IconSizeBack;
            }
        }

        #endregion

        /// <summary>
        /// Color of icon in back
        /// </summary>
        #region Background (Dependency Property)

        new public static readonly DependencyProperty BackgroundProperty =
            DependencyProperty.Register("Background", typeof(Brush), typeof(DualFabricIconControl), new PropertyMetadata(null, OnBackgroundChanged));

        new public Brush Background
        {
            get { return (Brush)GetValue(BackgroundProperty); }

            set { SetValue(BackgroundProperty, value); }
        }

        public static void OnBackgroundChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)

        {
            DualFabricIconControl sender = o as DualFabricIconControl;

            if (sender != null)
            {
                sender.fabicnBack.Foreground = sender.Background;
            }
        }

        #endregion

        #region ShowInControlView (Dependency Property)

        public static readonly DependencyProperty ShowInControlViewProperty =
            DependencyProperty.Register("ShowInControlView", typeof(bool), typeof(DualFabricIconControl), new PropertyMetadata(true));

        public bool ShowInControlView
        {
            get { return (bool)GetValue(ShowInControlViewProperty); }

            set { SetValue(ShowInControlViewProperty, value); }
        }

        #endregion

        /// <summary>
        /// Constructor with default size
        /// </summary>
        public DualFabricIconControl()
        {
            InitializeComponent();
            this.fabicnFront.GlyphSize = GlyphContext.Custom;
            this.fabicnBack.GlyphSize = GlyphContext.Custom;
        }

        protected override AutomationPeer OnCreateAutomationPeer() => CommonAutomationPeerCreator.CreateIconAutomationPeer(this, ShowInControlView);
    }
}
