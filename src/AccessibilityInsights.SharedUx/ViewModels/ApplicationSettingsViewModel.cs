// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using AccessibilityInsights.SharedUx.Enums;
using AccessibilityInsights.SharedUx.Settings;
using System;

namespace AccessibilityInsights.SharedUx.ViewModels
{
    /// <summary>
    /// ViewModel for ApplicationSettingsControl
    /// Wraps boolean app settings values for XAML use and provides save/load methods
    /// </summary>
    public class ApplicationSettingsViewModel : ViewModelBase
    {
        internal Action UpdateSaveButton;

        bool _selectionByFocus;
        public bool SelectionByFocus
        {
            get
            {
                return _selectionByFocus;
            }
            set
            {
                _selectionByFocus = value;
                OnPropertyChanged(nameof(this.SelectionByFocus));
                UpdateSaveButton();
            }
        }

        bool _selectionByMouse;
        public bool SelectionByMouse
        {
            get
            {
                return _selectionByMouse;
            }
            set
            {
                _selectionByMouse = value;
                OnPropertyChanged(nameof(this.SelectionByMouse));
                UpdateSaveButton();
            }
        }

        bool _alwaysOnTop;
        public bool AlwaysOnTop
        {
            get
            {
                return _alwaysOnTop;
            }
            set
            {
                _alwaysOnTop = value;
                OnPropertyChanged(nameof(this.AlwaysOnTop));
                UpdateSaveButton();
            }
        }

        bool _enableTelemetry;
        public bool EnableTelemetry
        {
            get
            {
                return _enableTelemetry;
            }
            set
            {
                _enableTelemetry = value;
                OnPropertyChanged(nameof(this.EnableTelemetry));
                UpdateSaveButton();
            }
        }

        bool _showTelemetryDialog;
        public bool ShowTelemetryDialog
        {
            get
            {
                return _showTelemetryDialog;
            }
            set
            {
                _showTelemetryDialog = value;
                OnPropertyChanged(nameof(this.ShowTelemetryDialog));
                UpdateSaveButton();
            }
        }

        int _fontSize;
        public int FontSize
        {
            get
            {
                return _fontSize;
            }
            set
            {
                _fontSize = value;
                OnPropertyChanged(nameof(this.FontSize));
                UpdateSaveButton();
            }
        }

        int _highlighterMode;
        public int HighlighterMode
        {
            get
            {
                return _highlighterMode;
            }
            set
            {
                _highlighterMode = value;
                OnPropertyChanged(nameof(this.HighlighterMode));
                UpdateSaveButton();
            }
        }

        int _soundFeedbackMode;
        public int SoundFeedbackMode
        {
            get
            {
                return _soundFeedbackMode;
            }
            set
            {
                _soundFeedbackMode = value;
                OnPropertyChanged(nameof(this.SoundFeedbackMode));
                UpdateSaveButton();
            }
        }

        /// <summary>
        /// Constructor
        /// </summary>
        public ApplicationSettingsViewModel() { }

        public void UpdateFromConfig(ConfigurationModel config)
        {
            if (config == null)
                throw new ArgumentNullException(nameof(config));

            this.SelectionByFocus = config.SelectionByFocus;
            this.SelectionByMouse = config.SelectionByMouse;
            this.AlwaysOnTop = config.AlwaysOnTop;
            this.EnableTelemetry = config.EnableTelemetry;
            this.ShowTelemetryDialog = config.ShowTelemetryDialog;
            this.SoundFeedbackMode = (int) config.SoundFeedback;
            this.HighlighterMode = (int)config.HighlighterMode;
            this.FontSize = (int)config.FontSize;
        }

        public void SaveToConfig(ConfigurationModel config)
        {
            if (config == null)
                throw new ArgumentNullException(nameof(config));

            config.SelectionByFocus = this.SelectionByFocus;
            config.SelectionByMouse = this.SelectionByMouse;
            config.AlwaysOnTop = this.AlwaysOnTop;
            config.EnableTelemetry = this.EnableTelemetry;
            config.ShowTelemetryDialog = this.ShowTelemetryDialog;
            config.SoundFeedback = (SoundFeedbackMode) this.SoundFeedbackMode;
            config.HighlighterMode = (HighlighterMode)this.HighlighterMode;
            config.FontSize = (FontSize)this.FontSize;
        }
    }
}
