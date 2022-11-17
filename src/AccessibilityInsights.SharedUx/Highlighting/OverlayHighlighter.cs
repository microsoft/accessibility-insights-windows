// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using AccessibilityInsights.SharedUx.Utilities;
using Axe.Windows.Core.Bases;
using Axe.Windows.Desktop.Utility;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace AccessibilityInsights.SharedUx.Highlighting
{
    /// <summary>
    /// Highlighter with transparent background
    /// </summary>
    public class OverlayHighlighter : WindowHighlighterBase, IDisposable
    {
        /// <summary>
        /// Selected element rectangle
        /// </summary>
        private Border SelectedElementRect;

        /// <summary>
        /// Selected element rectangle in Table or Grid Pattern
        /// </summary>
        private Border PatternSelectedElementRect;

        /// <summary>
        /// Constructor
        /// </summary>
        public OverlayHighlighter()
        {
            canvas = new Canvas();

            Items = new Dictionary<A11yElement, GroupHighlighterItem>();
        }

        /// <summary>
        /// Set base element and other info
        /// </summary>
        /// <param name="el"></param>
        /// <param name="brush"></param>
        /// <param name="onMouseDown"></param>
        /// <param name="gap"></param>
        public void SetElement(A11yElement el, SolidColorBrush brush, MouseButtonEventHandler onMouseDown, int gap)
        {
            if (el == null)
                throw new ArgumentNullException(nameof(el));

            var win = el.GetParentWindow();
            TBCallback = onMouseDown;
            Dimensions = win == null ? el.BoundingRectangle : win.BoundingRectangle;
            Brush = brush;
            GapWidth = gap;
        }

        /// <summary>
        /// Create and show highlighter window
        /// </summary>
        public override void Show()
        {
            if (this.HighlightWindow != null)
            {
                this.Hide();
            }
            IsVisible = true;
            canvas = new Canvas();

            this.HighlightWindow = new Window()
            {
                WindowStyle = WindowStyle.None,
                AllowsTransparency = true,
                Content = canvas,
                Background = null,
                Topmost = true,
                ShowInTaskbar = false,
                ResizeMode = ResizeMode.NoResize,
                WindowStartupLocation = WindowStartupLocation.Manual
            };

            var xyDpi = HelperMethods.GetWPFWindowPositioningDPI();
            this.HighlightWindow.Top = Dimensions.Top / xyDpi;
            this.HighlightWindow.Left = Dimensions.Left / xyDpi;

            DPI = Dimensions.GetDPI();
            this.HighlightWindow.Width = Dimensions.Width / DPI;
            this.HighlightWindow.Height = Dimensions.Height / DPI;

            this.HighlightWindow.Show();
        }

        /// <summary>
        /// Draw rectangle around the selected element in the pattern window in the application
        /// </summary>
        public void MarkElement(A11yElement ele)
        {
            RemoveBorder(PatternSelectedElementRect);
            PatternSelectedElementRect = AddElement(ele, Brush);
        }

        /// <summary>
        /// Draw rectangle using the element bounding rectangle
        /// </summary>
        private Border AddElement(A11yElement ele, SolidColorBrush brush)
        {
            if (ele == null)
                throw new ArgumentNullException(nameof(ele));

            Rectangle rect = ele.BoundingRectangle;

            var l = (rect.Left - Dimensions.Left) / DPI;
            var t = (rect.Top - Dimensions.Top) / DPI;

            Border brd = new Border()
            {
                BorderBrush = brush,
                BorderThickness = new Thickness(5),
                Width = rect.Width / DPI,
                Height = rect.Height / DPI,
            };
            canvas.Children.Add(brd);
            brd.SetValue(Canvas.LeftProperty, l);
            brd.SetValue(Canvas.TopProperty, t);
            brd.SetValue(Canvas.ZIndexProperty, 0);

            return brd;
        }

        /// <summary>
        /// remove border from the canvas
        /// </summary>
        private void RemoveBorder(Border brd)
        {
            try
            {
                canvas?.Children.Remove(brd);
            }
            catch (InvalidOperationException)
            {
                // TODO : Report this?
            }
            catch (NullReferenceException)
            {
                // TODO : Report this?
            }
            catch (ArgumentNullException)
            {
                // TODO : Report this?
            }
        }

        /// <summary>
        /// Draw rectangle around the selected element in the application
        /// </summary>
        public void MarkSelectedElement(A11yElement ele)
        {
            if (ele == null)
                throw new ArgumentNullException(nameof(ele));

            RemoveBorder(SelectedElementRect);
            SelectedElementRect = AddElement(ele, Application.Current.Resources["LoadingBrush"] as SolidColorBrush);
        }

        /// <summary>
        /// Hide highlighter window
        /// </summary>
        public override void Hide()
        {
            IsVisible = false;

            foreach (var elem in this.Items.Values.AsParallel())
            {
                canvas?.Children.Remove(elem.BrdrError);
                canvas?.Children.Remove(elem.TbError);
                elem.Clear();
            }

            this.Items.Clear();
            this.canvas = null;
            this.HighlightWindow?.Close();
            this.HighlightWindow = null;
            try
            {
                RemoveBorder(SelectedElementRect);
                RemoveBorder(PatternSelectedElementRect);
            }
            catch (InvalidOperationException)
            {
                // TODO : Report this?
            }
            catch (NullReferenceException)
            {
                // TODO : Report this?
            }
            catch (ArgumentNullException)
            {
                // TODO : Report this?
            }
        }

        /// <summary>
        /// Show the toast
        /// </summary>
        public void ShowToast(UserControl toast)
        {
            if (toast == null)
                throw new ArgumentNullException(nameof(toast));

            try
            {
                var xyDpi = HelperMethods.GetDPI((int)this.HighlightWindow.Left + (3 * GapWidth), (int)this.HighlightWindow.Top + (3 * GapWidth));
                var l = (Dimensions.Width / xyDpi - toast.Width) - (GapWidth * 2);
                var t = (Dimensions.Height / xyDpi - toast.Height) - (GapWidth * 2);
                canvas.Children.Add(toast);
                toast.SetValue(Canvas.LeftProperty, l);
                toast.SetValue(Canvas.TopProperty, t);
            }
            catch (InvalidOperationException)
            {
                // TODO : Report this?
            }
        }

        /// <summary>
        /// Clear highlighter data. This will close the highlighter.
        /// </summary>
        public override void Clear()
        {
            this.Hide();

            canvas = null;
            this.Items.Clear();
            this.BaseElement = null;
        }

        #region IDisposable Support
        private bool disposedValue; // To detect redundant calls

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    this.Clear();
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
