// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using AccessibilityInsights.SharedUx.Utilities;
using AccessibilityInsights.SharedUx.ViewModels;
using AccessibilityInsights.Win32;
using System;
using System.Drawing;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;

namespace AccessibilityInsights.SharedUx.Dialogs
{
#pragma warning disable CA1001 // Types that own disposable fields should be disposable
    /// <summary>
    /// Interaction logic for GlobalEyedropperWindow.xaml
    /// This is a self contained eyedropper that will let the user pick a color anywhere on the screen
    /// It will update the provided VM's colors based on the provided bool values and the selected color
    /// </summary>
    public partial class GlobalEyedropperWindow : Window
#pragma warning restore CA1001 // Types that own disposable fields should be disposable
    {
        const int zoomLevel = 4;

        int radius;
        ColorContrastViewModel ccVM;
        bool selectingFirst;
        bool selectingSecond;
        Bitmap desktopScreenshot;
        Timer updatePosTimer;
        TransformGroup renderTransformGroup;
        Action<object, EventArgs> onClose;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="vm">VM of which to update with selected colors</param>
        /// <param name="first">Should FirstColor in the VM be updated</param>
        /// <param name="second">Should SecondColor in the VM be updated</param>
        /// <param name="closed">This will be called on eyedropper closing</param>
        public GlobalEyedropperWindow(ColorContrastViewModel vm, bool first, bool second, Action<object, EventArgs> closed)
        {
            InitializeComponent();
            onClose = closed;
            ccVM = vm;
            selectingFirst = first;
            selectingSecond = second;
            radius = Convert.ToInt32(Height / 2);

            InitializeRenderTransform();
            CaptureScreenshot();
            InitializeUpdateTimer();
            UpdatePos();
        }

        private void CaptureScreenshot()
        {
            desktopScreenshot = new Bitmap(SystemInformation.VirtualScreen.Width, SystemInformation.VirtualScreen.Height, System.Drawing.Imaging.PixelFormat.Format24bppRgb);

            using (Graphics g = Graphics.FromImage(desktopScreenshot))
            {
                g.CopyFromScreen(SystemInformation.VirtualScreen.Left, SystemInformation.VirtualScreen.Top, 0, 0, desktopScreenshot.Size);
            }
        }

        private void InitializeUpdateTimer()
        {
            updatePosTimer = new Timer()
            {
                Enabled = true,
                Interval = 5,
            };
            updatePosTimer.Tick += Timer_Tick;
        }

        private void InitializeRenderTransform()
        {
            renderTransformGroup = new TransformGroup();
            renderTransformGroup.Children.Add(new ScaleTransform(zoomLevel,  zoomLevel));
            eyedropperPreview.RenderTransform = renderTransformGroup;
        }

        private void UpdateColor(System.Drawing.Color col)
        {
            if (selectingFirst)
            {
                ccVM.FirstColor = col.ToMediaColor();
            }
            else if (selectingSecond)
            {
                ccVM.SecondColor = col.ToMediaColor();
            }
        }

        private static System.Drawing.Point ScreenCoordsToScreenshotPos(System.Drawing.Point pos)
        {
            return new System.Drawing.Point(pos.X - SystemInformation.VirtualScreen.Left, pos.Y - SystemInformation.VirtualScreen.Top);
        }

        private void UpdatePos()
        {
            var mousePos = Control.MousePosition;
            var dpi = VisualTreeHelper.GetDpi(this);

            var screenshotPosition = ScreenCoordsToScreenshotPos(mousePos);
            UpdateColor(desktopScreenshot.GetPixel(screenshotPosition.X, screenshotPosition.Y));

            Rectangle desktopRegion = new Rectangle(
                screenshotPosition.X - radius / zoomLevel,
                screenshotPosition.Y - radius / zoomLevel,
                radius * 2,
                radius * 2
            );
            desktopRegion.Intersect(new Rectangle(System.Drawing.Point.Empty, desktopScreenshot.Size));

            using (Bitmap b = desktopScreenshot.Clone(desktopRegion, desktopScreenshot.PixelFormat))
            {
                eyedropperPreview.Source = b.ConvertToSource();
            }

            this.Left = mousePos.X / dpi.DpiScaleX - radius;
            this.Top = mousePos.Y / dpi.DpiScaleY - radius;
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            UpdatePos();
        }

        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            this.Close();
        }

        private void Window_LostFocus(object sender, RoutedEventArgs e)
        {
            if (this.IsActive)
            {
                this.Close();
            }
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            updatePosTimer.Stop();
            updatePosTimer.Dispose();
            desktopScreenshot.Dispose();
            onClose.Invoke(null, new EventArgs());
        }

        private static void MoveCursor(int offsetX, int offsetY)
        {
            var pt = Control.MousePosition;
            NativeMethods.SetCursorPos(pt.X + offsetX, pt.Y + offsetY);
        }

        /// <summary>
        /// Control window and mouse cursor with keyboard
        /// </summary>
        private void Window_PreviewKeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            switch (e.Key)
            {
                case Key.Up:
                    MoveCursor(0, -StepSize(Height));
                    break;
                case Key.Down:
                    MoveCursor(0, StepSize(Height));
                    break;
                case Key.Left:
                    MoveCursor(-StepSize(Width), 0);
                    break;
                case Key.Right:
                    MoveCursor(StepSize(Width), 0);
                    break;
                case Key.Enter:
                case Key.Escape:
                    this.Close();
                    break;
            }

            e.Handled = true;
        }

        private static int StepSize(double stepWithCtrl)
        {
            if (Keyboard.IsKeyDown(Key.LeftCtrl) || Keyboard.IsKeyDown(Key.RightCtrl))
                return (int)(stepWithCtrl / zoomLevel);
            return 1;
        }
    }
}
