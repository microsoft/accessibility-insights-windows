// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using AccessibilityInsights.CommonUxComponents.Utilities;
using AccessibilityInsights.SharedUx.Utilities;
using AccessibilityInsights.Win32;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Text;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Media;

namespace AccessibilityInsights.SharedUx.Highlighting
{
    public class Win32SnapshotButton : IDisposable
    {
        private readonly static ReferenceHolder<IntPtr, Win32SnapshotButton> Holder = new ReferenceHolder<IntPtr, Win32SnapshotButton>();
        private readonly static WndProc MyWndProc = new WndProc(StaticWndProc);

        private IntPtr hWnd;

        const int BorderMargin = 1;
        const int DefaultWidth = 20;
        const int DefaultHeight = 20;
        const String BeakerCode = "\uF3A6";
        private int Width = DefaultWidth;
        private int Height = DefaultHeight;
        private System.Drawing.Color FontBrush;
        private System.Drawing.Color BackgroundBrush;
        private System.Drawing.Color HoveredBackgroundBrush;

        private bool IsHovered;
        private bool isVisible;
        public bool IsVisible
        {
            get
            {
                return this.isVisible;
            }

            set
            {
                this.isVisible = value;
                SetVisible(value);
            }
        }

        public Rectangle HiLighterRect { get; set; }
        public string WindowClassName { get; private set; }
        IntPtr hInstance;
        public Action Clicked { get; set; }

        private Dictionary<double, Bitmap> beakerIconCollection;
        private Dictionary<double, Bitmap> hoveredBeakerIconCollection;

        public Win32SnapshotButton(string cnb)
        {
            this.WindowClassName = cnb;
            this.hInstance = NativeMethods.GetModuleHandle(null);
            this.IsHovered = false;

            var brush = Application.Current.Resources["SnapshotBtnBGBrush"] as SolidColorBrush;
            this.BackgroundBrush = System.Drawing.Color.FromArgb(brush.Color.A, brush.Color.R, brush.Color.G, brush.Color.B);
            brush = Application.Current.Resources["SnapshotBtnHoverBrush"] as SolidColorBrush;
            this.HoveredBackgroundBrush = System.Drawing.Color.FromArgb(brush.Color.A, brush.Color.R, brush.Color.G, brush.Color.B);
            brush = Application.Current.Resources["SnapshotBtnFGBrush"] as SolidColorBrush;
            this.FontBrush = System.Drawing.Color.FromArgb(brush.Color.A, brush.Color.R, brush.Color.G, brush.Color.B);

            beakerIconCollection = new Dictionary<double, Bitmap>();
            hoveredBeakerIconCollection = new Dictionary<double, Bitmap>();

            RegisterWindowClass();
            this.hWnd = CreateWindow(this.HiLighterRect);
            this.IsVisible = true;
            Holder.Add(this.hWnd, this);
        }

        private static IntPtr StaticWndProc(IntPtr hWnd, uint uMsg, IntPtr wParam, IntPtr lParam)
        {
            if (Holder.TryGet(hWnd, out Win32SnapshotButton button))
            {
                return button.WndProc(hWnd, uMsg, wParam, lParam);
            }
            return NativeMethods.DefWindowProc(hWnd, uMsg, wParam, lParam);
        }

        private static PrivateFontCollection LoadFontResource()
        {
            PrivateFontCollection fonts = new PrivateFontCollection();
            Assembly assembly = Assembly.GetExecutingAssembly();

            using (Stream fontStream = SharedResources.FabricIconFontResource.Stream)
            {
                IntPtr data = Marshal.AllocCoTaskMem((int)fontStream.Length);
                try
                {
                    byte[] fontdata = new byte[fontStream.Length];
                    fontStream.Read(fontdata, 0, (int)fontStream.Length);
                    Marshal.Copy(fontdata, 0, data, (int)fontStream.Length);
                    fonts.AddMemoryFont(data, (int)fontStream.Length);
                }
#pragma warning disable CA1031 // Do not catch general exception types
                catch (Exception ex)
                {
                    // TODO : Report this?
                    System.Diagnostics.Trace.WriteLine("Font stream exception: " + ex);
                }
#pragma warning restore CA1031 // Do not catch general exception types
                finally
                {
                    Marshal.FreeCoTaskMem(data);
                }
            }
            return fonts;
        }

        private Bitmap LoadBeakerImage(double DPI)
        {
            System.Drawing.FontFamily family;
            // Create font
            using (PrivateFontCollection fonts = LoadFontResource())
            {
                // Get font family
                family = (System.Drawing.FontFamily)fonts.Families.GetValue(0);
            }
            // Create bitmap
            Bitmap bitmap = new Bitmap(Convert.ToInt32(DefaultWidth * DPI), Convert.ToInt32(DefaultHeight * DPI));
            // Create graphics from image
            using (Graphics g = Graphics.FromImage(bitmap))
            using (StringFormat sf = new StringFormat())
            {
                sf.LineAlignment = StringAlignment.Center;
                sf.Alignment = StringAlignment.Center;

                System.Drawing.Color bkgColor = this.IsHovered ? this.HoveredBackgroundBrush : this.BackgroundBrush;

                // Draw string
                int fontSize = Convert.ToInt32(9.0f * DPI);
                g.Clear(bkgColor);
                using (Font font = new Font(family, fontSize, System.Drawing.FontStyle.Regular))
                using (SolidBrush brush = new SolidBrush(this.FontBrush))
                {
                    g.DrawString(
                        BeakerCode,
                        font,
                        brush,
                        new Rectangle(0, 0, this.Width, this.Height + BorderMargin * 3),
                        sf
                        );
                }
            }

            if (!this.IsHovered)
            {
                this.beakerIconCollection[DPI] = bitmap;
            }
            else
            {
                this.hoveredBeakerIconCollection[DPI] = bitmap;
            }

            return bitmap;
        }

        IntPtr CreateWindow(Rectangle rect)
        {
            return NativeMethods.CreateWindowEx(WindowStylesEx.WS_EX_TOOLWINDOW,
                this.WindowClassName,
                "",
                WindowStyles.WS_POPUP,
                rect.Left, rect.Top, rect.Width, rect.Height,
                IntPtr.Zero,
                IntPtr.Zero,
                this.hInstance,
                IntPtr.Zero);
        }

        short RegisterWindowClass()
        {
            // First register the host window class.
            WNDCLASSEX wcex = new WNDCLASSEX();
            wcex.cbSize = (uint)Marshal.SizeOf(wcex);
            wcex.style = (uint)ClassStyles.SaveBits | (uint)ClassStyles.VerticalRedraw | (uint)ClassStyles.HorizontalRedraw;
            wcex.cbWndExtra = 0;
            wcex.hInstance = this.hInstance;
            wcex.lpfnWndProc = MyWndProc;
            wcex.hIcon = IntPtr.Zero;
            wcex.hCursor = IntPtr.Zero;
            wcex.hbrBackground = IntPtr.Zero;
            wcex.lpszMenuName = null;
            wcex.lpszClassName = this.WindowClassName;

            wcex.style = 0;
            wcex.cbClsExtra = 0;
            wcex.cbWndExtra = 0;

            wcex.hIconSm = IntPtr.Zero;

            return NativeMethods.RegisterClassEx(ref wcex);
        }

        private double UpdateBeakerSize()
        {
            double currentDPI = this.HiLighterRect.IsEmpty ? 1.0 : this.HiLighterRect.GetDPI();

            this.Width = Convert.ToInt32((Convert.ToDouble(DefaultWidth) * currentDPI));
            this.Height = Convert.ToInt32((Convert.ToDouble(DefaultHeight) * currentDPI));

            return currentDPI;
        }

        public void SetLocation(Rectangle rc)
        {
            this.HiLighterRect = rc;
            NativeMethods.SetWindowPos(hWnd,
                (IntPtr)Win32Constants.HWND_TOPMOST,
                rc.Right - Width - BorderMargin,
                rc.Top - BorderMargin,
                Width,
                Height,
                Win32Constants.SWP_NOACTIVATE);
        }

        void UnRegisterWindowClass()
        {
            NativeMethods.UnregisterClass(this.WindowClassName, this.hInstance);
        }

        private Rectangle GetBeakerBoundingRectangle(Rectangle rect)
        {
            Rectangle beakerRect = new Rectangle(rect.Right - this.Width - BorderMargin, rect.Top, this.Width, this.Height);
            return beakerRect;
        }

        private void DrawBeaker()
        {
            double dpi = UpdateBeakerSize();

            Bitmap iconToDraw = null;

            var iconCollection = this.IsHovered ? hoveredBeakerIconCollection : beakerIconCollection;

            if (!iconCollection.TryGetValue(dpi, out iconToDraw))
            {
                iconToDraw = LoadBeakerImage(dpi);
            }

            IntPtr wndHDC = NativeMethods.GetDC(this.hWnd);
            IntPtr hdcMem = NativeMethods.CreateCompatibleDC(wndHDC);
            IntPtr hbmOld = NativeMethods.SelectObject(hdcMem, iconToDraw.GetHbitmap());
            NativeMethods.BitBlt(wndHDC, 0, 0, Width, Height, hdcMem, 0, 0, TernaryRasterOperations.SRCCOPY);
            NativeMethods.SelectObject(hdcMem, hbmOld);
            NativeMethods.DeleteDC(hdcMem);
        }

        private void Update()
        {
            NativeMethods.DestroyWindow(this.hWnd);
            UnRegisterWindowClass();
            UpdateBeakerSize();
            RegisterWindowClass();
            this.hWnd = this.CreateWindow(GetBeakerBoundingRectangle(this.HiLighterRect));
            this.SetVisible(this.IsVisible);
        }

        public IntPtr WndProc(IntPtr hWnd, uint uMsg, IntPtr wParam, IntPtr lParam)
        {
            switch (uMsg)
            {
                // Using ERASEBKGND instead of PAINT gives us more responsive update
                case Win32Constants.WM_ERASEBKGND:
                    this.IsHovered = false;
                    DrawBeaker();
                    return (IntPtr)1;
                case Win32Constants.WM_DPICHANGED:
                    Update();
                    this.IsHovered = false;
                    DrawBeaker();
                    return (IntPtr)1;
                case Win32Constants.WM_MOUSEMOVE:
                    // add track mouse event
                    TRACKMOUSEEVENT track = new TRACKMOUSEEVENT();
                    track.hWnd = this.hWnd;
                    track.cbSize = Marshal.SizeOf(typeof(TRACKMOUSEEVENT));
                    track.dwFlags = Win32Constants.TME_LEAVE;
                    NativeMethods.TrackMouseEvent(ref track);
                    this.IsHovered = true;
                    DrawBeaker();
                    return (IntPtr)1;
                case Win32Constants.WM_MOUSELEAVE:
                    this.IsHovered = false;
                    DrawBeaker();
                    return (IntPtr)1;
                case Win32Constants.WM_LBUTTONUP:
                    Clicked?.Invoke();
                    return (IntPtr)1;
            }
            return NativeMethods.DefWindowProc(hWnd, uMsg, wParam, lParam);
        }

        public void SetVisible(bool bVisible)
        {
            NativeMethods.ShowWindow(this.hWnd, bVisible ? ShowWindowCommands.ShowNA : ShowWindowCommands.Hide);
        }

        #region IDisposable Support
        private bool disposedValue; // To detect redundant calls
        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                Holder.Remove(hWnd);
                NativeMethods.DestroyWindow(this.hWnd);
                UnRegisterWindowClass();

                disposedValue = true;
            }
        }

        ~Win32SnapshotButton()
        {
            Dispose(false);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        #endregion
    }
}
