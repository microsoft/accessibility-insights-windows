// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using AccessibilityInsights.SharedUx.Enums;
using AccessibilityInsights.Win32;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows;
using System.Windows.Media;

namespace AccessibilityInsights.SharedUx.Highlighting
{
    /// <summary>
    /// Class Hilighter
    /// hilight bounding rectangle and show tooltip
    /// </summary>
    public class Highlighter : IDisposable
    {
        // internal width (excluding borders)
        const int DEFAULT_RECTWIDTH = 4;
        const int DEFAULT_RECTGAP = 2;

        readonly List<LineBorder> Border = new List<LineBorder>();
        TextTip Text;
        Win32SnapshotButton Win32Snapshot { get; set; }
        private readonly string WndClassNameBase;

        /// <summary>
        /// Hilighter constructor
        /// </summary>
        /// <param name="color">RGB in int value</param>
        public Highlighter(HighlighterColor color = HighlighterColor.DefaultBrush, bool hasSnapshot = false)
        {
            var brush = GetBrush(color);
            this.WndClassNameBase = Guid.NewGuid().ToString();
            for (int i = 0; i < 4; i++)
            {
                this.Border.Add(new LineBorder(this.WndClassNameBase, i)
                {
                    Color = NativeMethods.RGB(brush.Color.R, brush.Color.G, brush.Color.B),
                    Gap = DEFAULT_RECTGAP,
                    Width = DEFAULT_RECTWIDTH
                });
            }

            this.Text = new TextTip(this.WndClassNameBase);
            if (hasSnapshot)
            {
                this.Win32Snapshot = new Win32SnapshotButton("Beaker");
            }
        }

        bool isVisible;
        /// <summary>
        /// Indicate whether we show highlighter/beaker/tooltip or nothing at all
        /// </summary>
        public bool IsVisible
        {
            get
            {
                return this.isVisible;
            }

            set
            {
                if (value)
                {
                    LoadHighlighterMode(this.HighlighterMode);
                }
                else
                {
                    this.TurnOff();
                }
                this.isVisible = value;
            }
        }

        private bool isBorderVisible;
        private bool IsBorderVisible
        {
            get => this.isBorderVisible;

            set
            {
                if (this.isBorderVisible != value)
                {
                    this.isBorderVisible = value;
                    this.Border.ForEach(b => b.SetVisible(value));
                }
            }
        }

        private HighlighterMode highlighterMode;
        public HighlighterMode HighlighterMode
        {
            get
            {
                return this.highlighterMode;
            }

            set
            {
                this.highlighterMode = value;
                if (this.IsVisible)
                {
                    LoadHighlighterMode(value);
                }
            }
        }

        /// <summary>
        /// Configure the Highlighter mode
        /// </summary>
        public void LoadHighlighterMode(HighlighterMode HilighterMode)
        {
            UpdateAllChildren(false, true);
            switch (HilighterMode)
            {
                case HighlighterMode.HighlighterBeakerTooltip:
                    this.IsBorderVisible = true;
                    AssignSnapshotSafely(true);
                    this.Text.IsVisible = true;
                    break;
                case HighlighterMode.HighlighterBeaker:
                    this.IsBorderVisible = true;
                    AssignSnapshotSafely(true);
                    this.Text.IsVisible = false;
                    break;
                case HighlighterMode.HighlighterTooltip:
                    this.IsBorderVisible = true;
                    AssignSnapshotSafely(false);
                    this.Text.IsVisible = true;
                    break;
                case HighlighterMode.Highlighter:
                    this.IsBorderVisible = true;
                    AssignSnapshotSafely(false);
                    this.Text.IsVisible = false;
                    break;
                case HighlighterMode.Tooltip:
                    this.IsBorderVisible = false;
                    AssignSnapshotSafely(false);
                    this.Text.IsVisible = true;
                    break;
                default:
                    throw new ArgumentException($"Argument {this.HighlighterMode.ToString()} is invalid");
            }
        }

        /// <summary>
        /// Turn off highlighter/tooltip/beaker
        /// </summary>
        private void TurnOff()
        {
            this.IsBorderVisible = false;
            AssignSnapshotSafely(false);
            this.Text.IsVisible = false;
        }

        private void AssignSnapshotSafely(bool val)
        {
            if (this.Win32Snapshot != null)
            {
                this.Win32Snapshot.IsVisible = val;
            }
        }

        /// <summary>
        /// Update all children(Highlighter and Text)
        /// </summary>
        /// <param name="fColorChanged"></param>
        /// <param name="fLocSizeChanged"></param>
        void UpdateAllChildren(bool fColorChanged, bool fLocSizeChanged)
        {
            // make sure that we don't update the border location when mode is tooltip only.
            // otherwise, it will draw borders.
            if (this.HighlighterMode != HighlighterMode.Tooltip)
            {
                this.Border.ForEach(b => b.Update(fColorChanged, fLocSizeChanged));
            }
            this.Text?.Update();
        }

        public void SetLocation(Rectangle rc)
        {
            this.Border.ForEach(b => b.Rect = rc);
            this.Text.Location = rc;
            UpdateAllChildren(false, true); // FALSE -> Color not changed; TRUE -> location/size changed
            this.Win32Snapshot?.SetLocation(rc);
        }

        /// <summary>
        /// set call back for snapshot
        /// </summary>
        /// <param name="callback"></param>
        public void SetCallBackForSnapshot(Action callback)
        {
            if (this.Win32Snapshot != null)
            {
                this.Win32Snapshot.Clicked = callback;
            }
        }

        /// <summary>
        /// Loads a Highlighter Brush from the style dictionary
        /// </summary>
        /// <param name="colorKey"></param>
        /// <returns></returns>
        private static SolidColorBrush GetBrush(HighlighterColor color)
        {
            return Application.Current.Resources[$"HL{color}"] as SolidColorBrush;
        }

        /// <summary>
        /// Set the text value
        /// </summary>
        /// <param name="text"></param>
        public void SetText(string text)
        {
            if (text != this.Text.Text)
            {
                this.Text.Text = text;
            }
        }

        #region IDisposable Support
        private bool disposedValue; // To detect redundant calls

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    if(this.Border.Count != 0)
                    {
                        this.Border.ForEach(b => b.Dispose());
                        this.Border.Clear();
                        this.Text.Dispose();
                        this.Text = null;
                        this.Win32Snapshot?.Dispose();
                    }
                }

                disposedValue = true;
            }
        }

        // This code added to correctly implement the disposable pattern.
        public void Dispose()
        {
            // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        #endregion

    }
}
