// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using AccessibilityInsights.SharedUx.Telemetry;
using AccessibilityInsights.SharedUx.Utilities;
using Axe.Windows.Actions.Contexts;
using System;
using System.Windows;
using System.Windows.Input;

namespace AccessibilityInsights.SharedUx.Dialogs
{
    /// <summary>
    /// Interaction logic for EyedropperWindow.xaml
    /// 
    /// The original screenshot is padded with a border of 
    /// length MAGNIFY_RADIUS pixels. The border
    /// allows the rectangle to magnify corners of the original 
    /// screenshot without extra work. 
    /// 
    /// The original screenshot is larger than the application appears 
    /// on HDPI monitors, so it is scaled down by the current DPI 
    /// to take up the same amount of space that the corresponding live 
    /// application would have 
    ///     The other option is to keep the eyedropper window smaller 
    ///     but render the image at "original" size which will appear
    ///     larger than the live application would, but mouse selection
    ///     may seem easier
    ///     
    /// </summary>
    public partial class EyedropperWindow : Window
    {
        public EyedropperWindow()
        {
            InitializeComponent();
        }

        /// <summary>
        /// When the user clicks the mouse button or presses enter to
        /// finalize their color choice, this action is invoked
        /// </summary>
        internal Action RecordingCompleted;

        /// <summary>
        /// Stores element, screenshot
        /// </summary>
        public ElementContext EC { get; private set; }

        /// <summary>
        /// When the mouse hovers over a new pixel, this action is invoked
        /// </summary>
        private Action<System.Drawing.Point> HoverPositionChanged;

        /// <summary>
        /// Image over which eyedropper operates (has added border)
        /// </summary>
        private System.Drawing.Bitmap background;

        /// <summary>
        /// Magnified squared image for pixel selection inside rectangle
        /// </summary>
        private System.Drawing.Bitmap scaledBackground;

        /// <summary>
        /// Initial width of eyedropper window will not exceed these values
        /// </summary>
        private const int INITIAL_WINDOW_WIDTH = 800;
        private const int INITIAL_WINDOW_HEIGHT = 600;
        private const int MIN_INITIAL_WINDOW_WIDTH = 200;
        private const int MIN_INITIAL_WINDOW_HEIGHT = 200;

        /// <summary>
        /// Pixel position over original screenshot that is at the center of the magnified region
        /// The top left corner of the original image (0, 0) would correspond to (MAGNIFY_RADIUS, MAGNIFY_RADIUS)
        /// </summary>
        private System.Drawing.Point magnifierPosition = new System.Drawing.Point();

        /// <summary>
        /// The screenshot itself is usually larger than it appears visually because of HDPI.
        /// We want the screenshot to appear on the screen as if the original application were being
        /// rendered, so we scale the screenshot by the current DPI when displaying it.
        /// </summary>
        private double backgroundImageScale;

        #region scaling constants
        /// <summary>
        /// The magnifier will scale a square of 
        /// MAGNIFY_RADIUS * 2 pixels length from the background image at a time
        /// </summary>
        public const int MagnifyRadius = 16;

        /// <summary>
        /// How much we scale each pixel in the original image
        /// </summary>
        public const double ScaleFactor = 4;

        /// <summary>
        /// Size of the actual magnifier rectangle rendered on the screen
        /// </summary>
        public const double MagnifierSize = MagnifyRadius * ScaleFactor * 2;

        /// <summary>
        /// How much to offset the 1-pixel color preview rectangle from its containing magnifier rectangle parent
        /// </summary>
        public const double PixelRectangleOffset = MagnifierSize / 2;
        #endregion

        /// <summary>
        /// Updates the background image, adds a border in order to enable
        /// pixel selection around the edges, and modifies the size of this window
        /// </summary>
        /// <param name="context">ElementContext containing screenshot</param>
        public void UpdateBackground(ElementContext context)
        {
            var screenshot = context?.DataContext?.Screenshot;
            this.EC = context;
            if (screenshot != null)
            {
                this.backgroundImageScale = HelperMethods.GetDPI(
                    (int)System.Windows.Application.Current.MainWindow.Top, 
                    (int)System.Windows.Application.Current.MainWindow.Left);
                
                // Increase size and add white border (this is so we can magnify the corners without having to worry about resizing the magnifier preview
                System.Drawing.Bitmap newBitmap = new System.Drawing.Bitmap((int) (screenshot.Width + MagnifyRadius * 2), (int) (screenshot.Height + MagnifyRadius * 2));
                using (System.Drawing.Graphics g = System.Drawing.Graphics.FromImage(newBitmap))
                {
                    g.Clear(System.Drawing.Color.White);
                    g.DrawImageUnscaled(screenshot, MagnifyRadius, MagnifyRadius);
                }

                // Size to scale to
                var ourImageWidth = (screenshot.Width + MagnifyRadius * 2) / this.backgroundImageScale;
                var ourImageHeight = (screenshot.Height + MagnifyRadius * 2) / this.backgroundImageScale;
                this.background = newBitmap;
                this.image.Source = background.ConvertToSource();
                this.image.Width = ourImageWidth;
                this.image.Height = ourImageHeight;
                this.grid.MaxWidth = ourImageWidth;
                this.grid.MaxHeight = ourImageHeight;
                this.Width = Math.Min(Math.Max(ourImageWidth, MIN_INITIAL_WINDOW_WIDTH), INITIAL_WINDOW_WIDTH);
                this.Height = Math.Min(Math.Max(ourImageHeight, MIN_INITIAL_WINDOW_HEIGHT), INITIAL_WINDOW_HEIGHT);

                ChangeMagnifierPositionBy(0, 0); // shows initial magnified state
            }
        }

        /// <summary>
        /// Keeps track of whether we are currently
        /// updating the stored color using the eyedropper
        /// </summary>
        private bool updating = false;
        private bool Updating {
            get
            {
                return updating;
            }
            set
            {
                this.magborder.Visibility = value ? Visibility.Visible : Visibility.Collapsed;
                this.Cursor = value ? System.Windows.Input.Cursors.None : System.Windows.Input.Cursors.Arrow;
                updating = value;
            }
        }

        /// <summary>
        /// Enable recording via hover and get focus on this control
        /// </summary>
        /// <param name="initialPosition">initial point (relative to original datacontext screenshot) to magnify</param>
        /// <param name="pixelLocationChanged">will be executed when the pixel location changes</param>
        public void StartRecording(System.Drawing.Point initialPosition, Action<System.Drawing.Point> pixelLocationChanged)
        {
            UpdateMagnifierLocation(initialPosition.X + MagnifyRadius, initialPosition.Y + MagnifyRadius);
            this.HoverPositionChanged = pixelLocationChanged;
            this.magimage.BringIntoView();
            Updating = true;
            this.Focus();
        }

        /// <summary>
        /// Set the magnifier to the given x/y position relative to expanded original backing image (with added border)
        /// </summary>
        private void UpdateMagnifierLocation(int x, int y)
        {
            try
            {
                magnifierPosition.X = x;
                magnifierPosition.Y = y;

                // bounds checks
                if (magnifierPosition.X < MagnifyRadius)
                {
                    magnifierPosition.X = (int) (MagnifyRadius);
                }
                else if (magnifierPosition.X >= this.EC.DataContext.Screenshot.Width + (int)(MagnifyRadius))
                {
                    magnifierPosition.X = this.EC.DataContext.Screenshot.Width + (int)(MagnifyRadius);
                }
                if (magnifierPosition.Y < MagnifyRadius)
                {
                    magnifierPosition.Y = (int) (MagnifyRadius);
                }
                else if (magnifierPosition.Y >= this.EC.DataContext.Screenshot.Height + (int)(MagnifyRadius))
                {
                    magnifierPosition.Y = this.EC.DataContext.Screenshot.Height + (int)(MagnifyRadius);
                }

                // update listener with point relative to original image (sanity bounds check)
                var pointOrigCoords = new System.Drawing.Point((int)(magnifierPosition.X - MagnifyRadius),
                                                               (int)(magnifierPosition.Y - MagnifyRadius));
                if (pointOrigCoords.X >= 0 && pointOrigCoords.X < this.EC.DataContext.Screenshot.Width &&
                    pointOrigCoords.Y >= 0 && pointOrigCoords.Y < this.EC.DataContext.Screenshot.Height)
                {
                    HoverPositionChanged?.Invoke(pointOrigCoords);
                }

                var magnifyRect = new System.Drawing.Rectangle(magnifierPosition.X - MagnifyRadius, magnifierPosition.Y - MagnifyRadius, 
                    MagnifyRadius * 2, MagnifyRadius * 2);

                // Update the magnified image and change margin so it is in the right place
                var backgroundRect = new System.Drawing.Rectangle(new System.Drawing.Point(0, 0), background.Size);
                if (backgroundRect.Contains(magnifyRect))
                {
                    scaledBackground = this.background.Clone(magnifyRect, this.background.PixelFormat);
                    magimage.Source = scaledBackground.ConvertToSource();
                    var scaledMagWidth = ScaleFactor * MagnifyRadius;
                    var scaledMagHeight = ScaleFactor * MagnifyRadius;
                    magborder.Margin = new Thickness((magnifierPosition.X / backgroundImageScale) - scaledMagWidth - 1,
                        (magnifierPosition.Y / backgroundImageScale) - scaledMagHeight - 1, 0, 0);
                }
            }
            catch (NullReferenceException e)
            {
                e.ReportException();
            }
            catch (ArgumentNullException e)
            {
                e.ReportException();
            }
        }

        /// <summary>
        /// Changes the magnifier position by the given delta
        /// Magnifier position is limited to original screenshot size + border width/height
        /// </summary>
        /// <param name="dx">x delta to add</param>
        /// <param name="dy">y delta to add</param>
        private void ChangeMagnifierPositionBy(int dx, int dy)
        {
            magnifierPosition.Offset(dx, dy);
            UpdateMagnifierLocation(magnifierPosition.X, magnifierPosition.Y);
        }

        /// <summary>
        /// arrow keys move magnifier position by 1 in given direction, enter/escape finalizes color choice
        /// </summary>
        private void Window_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            e.Handled = true;
            switch (e.Key)
            {
                case Key.Left:
                    ChangeMagnifierPositionBy(-1, 0);
                    break;
                case Key.Right:
                    ChangeMagnifierPositionBy(1, 0);
                    break;
                case Key.Up:
                    ChangeMagnifierPositionBy(0, -1);
                    break;
                case Key.Down:
                    ChangeMagnifierPositionBy(0, 1);
                    break;
                case Key.Enter:
                case Key.Escape:
                    FinalizeColorChoice();
                    break;
                default:
                    e.Handled = false;
                    break;
            }
        }

        /// <summary>
        /// Stop updating the color upon mouse click/enter and 
        /// invoke RecordingCompleted if we were recording
        /// </summary>
        private void FinalizeColorChoice()
        {
            if (Updating)
            {
                RecordingCompleted?.Invoke();
            }
            Updating = false;
        }

        /// <summary>
        /// Stop updating the color upon mouse click and 
        /// invoke RecordingCompleted if we were recording
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Grid_MouseDown(object sender, MouseButtonEventArgs e)
        {
            FinalizeColorChoice();
            e.Handled = true;
        }
        
        private System.Windows.Point lastMousePosition;
        /// <summary>
        /// Update magnification position as mouse moves
        /// </summary>
        private void Window_MouseMove(object sender, System.Windows.Input.MouseEventArgs e)
        {
            if (Updating)
            {
                // This event fires even if the underlying image moves (e.g. when using arrow keys), so we only update
                // the location if the mouse position has changed since last event
                var pos = e.GetPosition(this.image);
                if (pos != lastMousePosition)
                {
                    UpdateMagnifierLocation((int) Math.Round(pos.X * this.backgroundImageScale), (int) Math.Round(pos.Y * backgroundImageScale));
                }
                lastMousePosition = pos;
            }
        }

        /// <summary>
        /// Hide window
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            this.Visibility = Visibility.Collapsed;
        }
        
        /// <summary>
        /// Show or hide the first selected pixel at its current position
        /// </summary>
        public void ShowFirstColorLocation(bool show)
        {
            firstPixelRectPreview.Visibility = show ? Visibility.Visible : Visibility.Collapsed;
        }

        /// <summary>
        /// Show or hide the second selected pixel at its current position
        /// </summary>
        public void ShowSecondColorLocation(bool show)
        {
            secondPixelRectPreview.Visibility = show ? Visibility.Visible : Visibility.Collapsed;
        }

        /// <summary>
        /// Sets the location of the first chosen pixel in the screenshot
        /// </summary>
        /// <param name="p">pixel in original screenshot (without border)</param>
        public void SetFirstColorLocation(System.Drawing.Point p)
        {
            var newP1 = new System.Drawing.Point((int) ((MagnifyRadius + p.X) / this.backgroundImageScale),
                                                 (int) ((MagnifyRadius + p.Y) / this.backgroundImageScale));
            firstPixelRectPreview.Margin = new Thickness(newP1.X - firstPixelRectPreview.ActualWidth / 2 - 1,
                                                         newP1.Y - firstPixelRectPreview.ActualHeight / 2 - 1, 0, 0);
        }

        /// <summary>
        /// Sets the location of the second chosen pixel in the screenshot
        /// </summary>
        /// <param name="p">second pixel in original screenshot (without border)</param>
        public void SetSecondColorLocation(System.Drawing.Point p)
        {
            var newP1 = new System.Drawing.Point((int)((MagnifyRadius + p.X) / this.backgroundImageScale),
                                                 (int)((MagnifyRadius + p.Y) / this.backgroundImageScale));
            secondPixelRectPreview.Margin = new Thickness(newP1.X - secondPixelRectPreview.ActualWidth / 2 - 1,
                                                         newP1.Y - secondPixelRectPreview.ActualHeight / 2 - 1, 0, 0);
        }
    }
}
