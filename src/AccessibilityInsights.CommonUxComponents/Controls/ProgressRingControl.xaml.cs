// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.IO;
using System.Media;
using System.Reflection;
using System.Timers;
using System.Windows;
using System.Windows.Automation.Peers;
using System.Windows.Controls;
using System.Windows.Media.Animation;

namespace AccessibilityInsights.CommonUxComponents.Controls
{
#pragma warning disable CA1001 // Types that own disposable fields should be disposable
    /// <summary>
    /// Loading indicator modelled on UWP ProgressRing
    /// </summary>
    public partial class ProgressRingControl : UserControl
#pragma warning restore CA1001 // Types that own disposable fields should be disposable
    {
        /// <summary>
        /// DependencyProperty IsActive
        /// </summary>
        public static readonly DependencyProperty IsActiveProperty =
            DependencyProperty.Register(nameof(IsActive), typeof(bool), typeof(ProgressRingControl),
                new PropertyMetadata(new PropertyChangedCallback(IsActivePropertyChanged)));

        /// <summary>
        /// DependencyProperty WithSound
        /// </summary>
        public static readonly DependencyProperty WithSoundProperty =
            DependencyProperty.Register(nameof(WithSound), typeof(bool), typeof(ProgressRingControl),
                new PropertyMetadata(new PropertyChangedCallback(WithSoundPropertyChanged)));

        /// <summary>
        /// IsActive Property Change event handler
        /// </summary>
        /// <param name="d"></param>
        /// <param name="e"></param>
        private static void IsActivePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var control = d as ProgressRingControl;
            if (control != null)
            {
                control.IsActive = (bool)e.NewValue;
            }
        }

        /// <summary>
        /// WithSound Property Change event handler
        /// </summary>
        /// <param name="d"></param>
        /// <param name="e"></param>
        private static void WithSoundPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var control = d as ProgressRingControl;
            if (control != null)
            {
                control.WithSound = (bool)e.NewValue;
            }
        }

        /// <summary>
        /// Private variables used
        /// </summary>
        private readonly SoundPlayer player;
        private Timer timer;
        private bool isPlayingSound;
        private bool withSound;
        private SelectedSound _selectedSound;

        public SelectedSound SelectedSound
        {
            get => _selectedSound;
            set
            {
                if (value == _selectedSound) return;

                // Enforce that we only allow the property to be changed once
                if (_selectedSound != SelectedSound.NoSound)
                {
                    throw new ArgumentException("SelectedSound can only be changed once", nameof(value));
                }

                Assembly assembly = Assembly.GetExecutingAssembly();
                Stream stream = assembly.GetManifestResourceStream(assembly.GetName().Name + GetResourceForSound(value));
                player.Stream = stream;

                _selectedSound = value;
            }
        }

        private static string GetResourceForSound(SelectedSound selectedSound)
        {
            switch (selectedSound)
            {
                case SelectedSound.Scanner:
                    return ".Resources.Sound.scanner_sound.wav";
                default:
                    // Please refer to comments in https://github.com/microsoft/accessibility-insights-windows/pull/1523 if this is thrown
                    throw new ArgumentException($"No sound loaded for {selectedSound}", nameof(selectedSound));
            }
        }

        /// <summary>
        /// Check whether to play sound feedback while scanning
        /// </summary>
        public bool ShouldPlayScannerSound() => WithSound;

        /// <summary>
        /// Overriding LocalizedControlType
        /// </summary>
        /// <returns></returns>
        protected override AutomationPeer OnCreateAutomationPeer()
        {
            return new CustomControlOverridingAutomationPeer(this, "progress ring");
        }

        /// <summary>
        /// Start to play the scanner sound in specified milliseconds
        /// </summary>
        private void PlayScannerSound(int delayInMilliSecond)
        {
            // set up the timer for the sound
            timer = new Timer(delayInMilliSecond);
            timer.Elapsed += (Object source, System.Timers.ElapsedEventArgs e) =>
            {
                player.PlayLooping();
            };
            timer.AutoReset = false;
            timer.Start();
            isPlayingSound = true;
        }

        private void StopPlayingScannerSound()
        {
            player.Stop();
            timer.Stop();
            isPlayingSound = false;
        }

        public bool WithSound
        {
            get
            {
                // Because we don't expect to be notified when this binding's source has updated,
                // we need to initiate the update here
                GetBindingExpression(WithSoundProperty)?.UpdateTarget();

                return withSound;
            }
            set
            {
                withSound = value;
            }
        }

        /// <summary>
        /// Stops and starts animation, with or without sound depending on
        /// the configuration as well as if there is an AT user
        /// </summary>
        public bool IsActive
        {
            get
            {
                return sb1.GetCurrentState() == ClockState.Active;
            }
            set
            {
                if (value)
                {
                    Visibility = Visibility.Visible;
                    sb1.Begin();
                    sb2.Begin();
                    sb3.Begin();
                    sb4.Begin();
                    sb5.Begin();
                    if (ShouldPlayScannerSound())
                    {
                        PlayScannerSound(3000);
                    }
                }
                else
                {
                    sb1.Stop();
                    sb2.Stop();
                    sb3.Stop();
                    sb4.Stop();
                    sb5.Stop();
                    Application.Current.Dispatcher.Invoke(() => Visibility = Visibility.Collapsed);
                    if (isPlayingSound)
                    {
                        StopPlayingScannerSound();
                    }
                }
            }
        }

        public void Activate()
        {
            IsActive = true;
        }

        public void Deactivate()
        {
            IsActive = false;
        }

        /// <summary>
        /// Gets and sets width/height of ring
        /// </summary>
        public int Size
        {
            set
            {
                double x, y;
                x = 0.5;
                y = value / ell1.Height / -2;
                var pt = new Point(x, y);
                ell1.RenderTransformOrigin = pt;
                ell2.RenderTransformOrigin = pt;
                ell3.RenderTransformOrigin = pt;
                ell4.RenderTransformOrigin = pt;
                ell5.RenderTransformOrigin = pt;
            }
            get
            {
                return (int)(ell1.RenderTransformOrigin.Y * ell1.Height * -2);
            }
        }

        /// <summary>
        /// Constructor
        /// </summary>
        public ProgressRingControl()
        {
            InitializeComponent();
            player = new SoundPlayer();
        }
    }
}
