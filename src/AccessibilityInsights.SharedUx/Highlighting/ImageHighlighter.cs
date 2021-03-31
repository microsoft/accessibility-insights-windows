// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using AccessibilityInsights.SharedUx.Utilities;
using Axe.Windows.Core.Bases;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;

namespace AccessibilityInsights.SharedUx.Highlighting
{
    /// <summary>
    /// Highlighter with screenshot
    /// </summary>
    public class ImageHighlighter : WindowHighlighterBase, IDisposable
    {
        /// <summary>
        /// Border for Error Text
        /// </summary>
        Border bdClick;

        /// <summary>
        /// Previous window bounds
        /// </summary>
        Rect WinRect = Rect.Empty;

        /// <summary>
        /// Background image
        /// </summary>
        ImageBrush ib;

        /// <summary>
        /// Sets highlighter button staste
        /// </summary>
        readonly Action<bool> setHighlightBtnState;

        /// <summary>
        /// Has window.Closed() been called
        /// </summary>
        bool closedCalled;

        /// <summary>
        /// Constructor
        /// </summary>
        public ImageHighlighter(Action<bool> setHLBtn)
        {
            setHighlightBtnState = setHLBtn;
            WinRect = Rect.Empty;
            closedCalled = false;
            Items = new Dictionary<A11yElement, GroupHighlighterItem>();
        }

        /// <summary>
        /// Set base element and other info
        /// </summary>
        /// <param name="el"></param>
        /// <param name="gap"></param>
        /// <param name="brush"></param>
        public void SetElement(A11yElement el, int gap = 2, SolidColorBrush brush=null)
        {
            this.BaseElement = el ?? throw new ArgumentNullException(nameof(el));
            if (brush == null)
            {
                this.Brush = Application.Current.Resources["HLbrushRed"] as SolidColorBrush;
            }
            else
            {
                this.Brush = brush;
            }
            GapWidth = gap;

            Dimensions = GetBitMapWindowDimension(el);
        }

        /// <summary>
        /// Set button click handler
        /// if it is set to nall, button would not be shown on highlighter.
        /// </summary>
        /// <param name="h"></param>
        public void SetButtonClickHandler(MouseButtonEventHandler h)
        {
            TBCallback = h;
        }

        /// <summary>
        /// Get the proper dimension of selected element.
        /// </summary>
        /// <param name="el"></param>
        /// <returns></returns>
        private Rectangle GetBitMapWindowDimension(A11yElement el)
        {
            var dim = el.BoundingRectangle;

            // check whether el is from saved file.
            // if so make sure that dimension is correct to be visible.
            if(el.PlatformObject == null)
            {
                var dpi = HelperMethods.GetDPI((int)Application.Current.MainWindow.Top, (int)Application.Current.MainWindow.Left);
                this.WinRect = new Rect()
                {
                    Y = (int)Application.Current.MainWindow.Top,
                    X = (int)(Application.Current.MainWindow.Left - dim.Width / dpi),
                    Width = dim.Width / dpi,
                    Height = dim.Height / dpi
                };
            }

            return dim;
        }

        /// <summary>
        /// Set highlighter background imaage
        /// </summary>
        /// <param name="bmp"></param>
        public void SetBackground(Bitmap bmp)
        {
            if (bmp != null)
            {
                var bmps = bmp.ConvertToSource();
                ib = new ImageBrush(bmps)
                {
                    Stretch = Stretch.Uniform
                };
            }
        }

        /// <summary>
        /// Save old window bounds on close
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void onClosing(object sender, EventArgs e)
        {
            this.WinRect = HighlightWindow.RestoreBounds;
        }

        /// <summary>
        /// Toggle button on close
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void onClosed(object sender, EventArgs e)
        {
            if (!closedCalled)
            {
                setHighlightBtnState?.Invoke(false);
                IsVisible = false; // it is not visible any more.
            }
            closedCalled = false;
            this.HighlightWindow = null;
        }

        /// <summary>
        /// Warn user if they try to click on screenshot
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void onClick(object sender, EventArgs e)
        {
            var visKeyFrames = new ObjectKeyFrameCollection
            {
                new DiscreteObjectKeyFrame { KeyTime = TimeSpan.FromSeconds(0), Value = Visibility.Visible },
                new DiscreteObjectKeyFrame { KeyTime = TimeSpan.FromSeconds(2.5), Value = Visibility.Collapsed }
            };
            var opKeyFrames = new DoubleKeyFrameCollection
            {
                new DiscreteDoubleKeyFrame { KeyTime = TimeSpan.FromSeconds(0), Value = 1},
                new LinearDoubleKeyFrame { KeyTime = TimeSpan.FromSeconds(2), Value = 1},
                new LinearDoubleKeyFrame { KeyTime = TimeSpan.FromSeconds(2.5), Value = 0},
            };
            bdClick.BeginAnimation(TextBlock.VisibilityProperty, new ObjectAnimationUsingKeyFrames() { KeyFrames = visKeyFrames });
            bdClick.BeginAnimation(TextBlock.OpacityProperty, new DoubleAnimationUsingKeyFrames() { KeyFrames = opKeyFrames });
        }

        /// <summary>
        /// Create highlighter window
        /// </summary>
        public void InitializeWindow()
        {
            canvas = new Canvas();
            var vb = new Viewbox()
            {
                Child = canvas
            };

            this.HighlightWindow = new Window()
            {
                Content = vb,
                Title = Properties.Resources.ScreenshotWindowTitle,
                Icon = Application.Current.MainWindow.Icon,
                MinHeight = 80
            };

            bdClick = new Border()
            {
                Child = new TextBlock()
                {
                    FontSize = 28,
                    Text = Properties.Resources.ScreenshotWindowTitle,
                    Background = new SolidColorBrush(Colors.White),
                    HorizontalAlignment = HorizontalAlignment.Center,
                    VerticalAlignment = VerticalAlignment.Center,
                },
                Width = 620,
                Height = 70,
                Visibility = Visibility.Collapsed,
                BorderBrush = System.Windows.Media.Brushes.Red,
                BorderThickness = new Thickness(5),
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center,
            };

            this.HighlightWindow.Closing += onClosing;
            this.HighlightWindow.Closed += onClosed;
            this.HighlightWindow.MouseDown += onClick;
            this.HighlightWindow.MouseEnter += onClick;

            canvas.Width = Dimensions.Width;
            canvas.Height = Dimensions.Height;
            canvas.Children.Add(bdClick);
            bdClick.SetValue(Canvas.LeftProperty, (canvas.Width - bdClick.Width) / 2);
            bdClick.SetValue(Canvas.TopProperty, (canvas.Height - bdClick.Height) / 2);
            canvas.Background = ib;

            if (WinRect.IsEmpty)
            {
                var xyDpi = HelperMethods.GetWPFWindowPositioningDPI();
                this.HighlightWindow.Top = Dimensions.Top / xyDpi;
                this.HighlightWindow.Left = Dimensions.Left / xyDpi;

                var dpi = Dimensions.GetDPI();
                this.HighlightWindow.Width = Dimensions.Width / dpi;
                this.HighlightWindow.Height = Dimensions.Height / dpi;
            }
            else
            {
                this.HighlightWindow.Top = WinRect.Top;
                this.HighlightWindow.Left = WinRect.Left;
                this.HighlightWindow.Width = WinRect.Width;
                this.HighlightWindow.Height = WinRect.Height;
            }

            HighlightWindow.Show();
        }

        /// <summary>
        /// Create and show highlighter window
        /// </summary>
        public override void Show()
        {
            if (this.HighlightWindow == null && this.BaseElement != null && this.ib != null)
            {
                setHighlightBtnState?.Invoke(true);
                IsVisible = true;
                closedCalled = false;
                InitializeWindow();
            }
        }

        /// <summary>
        /// Clear selected elements
        /// </summary>
        public void ClearElements()
        {
            foreach (var elem in this.Items.Values.AsParallel())
            {
                canvas?.Children.Remove(elem.BrdrError);
                canvas?.Children.Remove(elem.TbError);
                elem.Clear();
            }

            this.Items.Clear();
        }

        /// <summary>
        /// Hide window
        /// </summary>
        public override void Hide()
        {
            this.ClearElements();
            if (canvas != null)
            {
                canvas.Children?.Remove(bdClick);
                canvas.Background = null;
            }

            IsVisible = false;
            this.canvas = null;

            this.closedCalled = true;
            this.HighlightWindow?.Close();
            this.HighlightWindow = null;
        }

        /// <summary>
        /// Clear all data. This will close the highlighter.
        /// </summary>
        public override void Clear()
        {
            this.WinRect = Rect.Empty;
            this.Hide();
            if (ib != null)
            {
                ib.ImageSource = null;
                ib = null;
            }
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
