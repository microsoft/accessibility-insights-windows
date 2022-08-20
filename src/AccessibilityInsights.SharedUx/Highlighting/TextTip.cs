// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using AccessibilityInsights.SharedUx.Utilities;
using AccessibilityInsights.Win32;
using System;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace AccessibilityInsights.SharedUx.Highlighting
{
    /// <summary>
    /// Show a text tip next to highlighter
    /// it will display text
    /// </summary>
    internal class TextTip : IDisposable
    {
        private static readonly ReferenceHolder<IntPtr, TextTip> Holder = new ReferenceHolder<IntPtr, TextTip>();
        private static readonly WndProc MyWndProc = new WndProc(StaticWndProc);

        const int Max_Text_Length = 60;
        const int TEXTBORDER = 2;
        const int TEXTTABMIN = 14;   // min space between left and right columns in rectangle
        const int TEXTGAP = 8;       // Gap between object and rectangle
        const int Default_Font_Height = 25;

        readonly IntPtr hWnd;

        public string WindowClassName { get; private set; }
        readonly IntPtr hInstance;

        POINT size;
        int m_TabStop;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="cn"></param>
        public TextTip(string cn)
        {
            this.hFont = NativeMethods.CreateFont(Default_Font_Height, 0, 0, 0, FontWeight.FW_LIGHT, false, false, false, FontCharSet.DEFAULT_CHARSET, FontPrecision.OUT_OUTLINE_PRECIS,
                 FontClipPrecision.CLIP_DEFAULT_PRECIS, FontQuality.CLEARTYPE_QUALITY, FontPitchAndFamily.DEFAULT_PITCH, "Calibri");
            this.WindowClassName = cn;
            this.hInstance = NativeMethods.GetModuleHandle(null);
            RegisterWindowClass();

            this.hWnd = CreateWindow();
            Holder.Add(this.hWnd, this);
        }

        private static IntPtr StaticWndProc(IntPtr hWnd, uint uMsg, IntPtr wParam, IntPtr lParam)
        {
            if (Holder.TryGet(hWnd, out TextTip text))
            {
                return text.WndProc(hWnd, uMsg, wParam, lParam);
            }
            return NativeMethods.DefWindowProc(hWnd, uMsg, wParam, lParam);
        }

        IntPtr hFont;
        public IntPtr Font
        {
            set
            {
                this.hFont = value;
                CalcSize();
                AdjustLoc();
            }
        }

        Rectangle location;
        public Rectangle Location
        {
            get
            {
                return this.location;
            }

            set
            {
                this.location = value;
                if (this.IsVisible)
                {
                    AdjustLoc();
                }
            }
        }

        bool isVisible;
        public bool IsVisible
        {
            get
            {
                return this.isVisible;
            }
            set
            {
                this.isVisible = value;
                if (this.isVisible)
                {
                    CalcSize();
                    AdjustLoc();
                }
                NativeMethods.ShowWindow(this.hWnd, this.isVisible ? ShowWindowCommands.ShowNA : ShowWindowCommands.Hide);
            }
        }

        string text;
        public string Text
        {
            get
            {
                return this.text;
            }

            set
            {
                this.text = value;
                if (string.IsNullOrEmpty(value))
                {
                    this.IsVisible = false;
                }
                else
                {
                    if (value.Length > Max_Text_Length)
                    {
                        this.text = value.Substring(0, Max_Text_Length) + "...";
                    }

#pragma warning disable CA2245 // Do not assign a property to itself.
                    // This looks odd, but it forces an invocation of IsVisual.Set
                    this.IsVisible = this.IsVisible;
#pragma warning restore CA2245 // Do not assign a property to itself.

                    CalcSize();
                }
            }
        }

        int textColor = NativeMethods.RGB(0, 0, 0);
        public int TextColor
        {
            get
            {
                return this.textColor;
            }

            set
            {
                this.textColor = value;
                Update();
            }
        }

        /// <summary>
        /// Update on screen.
        /// </summary>
        public void Update()
        {
            if (this.IsVisible)
            {
                NativeMethods.InvalidateRect(this.hWnd, IntPtr.Zero, true);
            }
        }

        int backgroundColor = NativeMethods.RGB(255, 255, 192);
        public int BackgroundColor
        {
            get
            {
                return this.backgroundColor;
            }

            set
            {
                this.backgroundColor = value;
                Update();
            }
        }

        private void AdjustLoc()
        {
            // Work out location...
            // Want to hover above the location rectangle...
            //IntPtr hWnd = Win32Apis.GetDesktopWindow();
            //RECT rcAvail;
            //Win32Apis.GetWindowRect(hWnd, out rcAvail);
            var ss = (from s in Screen.AllScreens
                      where s.Bounds.Left <= location.Left && s.Bounds.Right >= location.Left
                           && s.Bounds.Top <= location.Top && s.Bounds.Bottom >= location.Top
                      select s).FirstOrDefault();

            int left;
            int top;
            if (ss != null)
            {
                var rcAvail = ss.Bounds;
                // Try placing at left, if there's space...
                if (location.Left + TEXTGAP + size.x > rcAvail.Right)
                {
                    left = rcAvail.Right - TEXTGAP - size.x;
                }
                else
                {
                    left = location.Left;
                }

                // Try placing at top, if there's space...
                if (location.Bottom + TEXTGAP + size.y >= rcAvail.Bottom)
                {
                    top = location.Top - TEXTGAP - size.y;
                }
                // ... otherwise try at bottom...
                else
                {
                    top = location.Bottom + TEXTGAP;
                }
            }
            else
            {
                left = location.Left;
                top = location.Bottom + TEXTGAP;
            }

            NativeMethods.SetWindowPos(this.hWnd, new IntPtr(Win32Constants.HWND_TOPMOST), left, top, size.x, size.y, Win32Constants.SWP_NOACTIVATE);
        }

        private void CalcSize()
        {
            if (this.Text == null)
            {
                size.x = 0;
                size.y = 0;
                return;
            }

            // Have to parse the text into lines, and
            // work out width, height of each line.
            // Eventually, use the max width, and cumulative height.
            // Since Tabs stops aren't handled if we're using CALCRECT, we have
            // to roll our own... (ick)
            IntPtr hDC = NativeMethods.GetDC(IntPtr.Zero);
            IntPtr hFont = NativeMethods.SelectObject(hDC, this.hFont);

            int TotalHeight = 0;
            int MaxWidth1 = 0; // before the tab...
            int MaxWidth2 = 0; // after the tab...

            int idxCur = 0;
            while (idxCur < this.Text.Length)
            {
                // Find the end of the line...
                int idxEnd = idxCur;
                var ch = this.Text[idxEnd];

                while (ch != '\r' && ch != '\n' && ch != '\t')
                {
                    idxEnd++;
                    if (idxEnd < this.Text.Length)
                    {
                        ch = this.Text[idxEnd];
                    }
                    else
                    {
                        break;
                    }
                }

                // (int) cast for 64-bit compat. (We don't need 64-bit ptr diff here...)
                int Len = (int)(idxEnd - idxCur);

                // Now pass the line to DrawText to get the height/length...
                RECT rc = new RECT() { left = 0, top = 0, right = 2, bottom = 32768 };

                int Height = NativeMethods.DrawText(hDC, this.Text.Substring(idxCur, Len), Len, ref rc,
                        Win32Constants.DT_CALCRECT | Win32Constants.DT_LEFT | Win32Constants.DT_SINGLELINE | Win32Constants.DT_NOPREFIX);
                if (rc.right > MaxWidth1)
                    MaxWidth1 = rc.right;

                if (ch == '\t')
                {
                    // work out second part of the tab-stop'd line...
                    idxEnd++;
                    idxCur = idxEnd;

                    while (ch != '\r' && ch != '\n')
                    {
                        idxEnd++;
                        if (idxEnd < this.Text.Length)
                        {
                            ch = this.Text[idxEnd];
                        }
                        else
                        {
                            break;
                        }
                    }

                    Len = (int)(idxEnd - idxCur);

                    // Limit the rect to 1/4 of screen, and use DT_END_ELLIPSIS...
                    rc = new RECT() { left = 0, top = 0, right = 2, bottom = 32767 };
                    rc.right = NativeMethods.GetSystemMetrics(SystemMetric.SM_CXSCREEN) / 4;

                    int Height2 = NativeMethods.DrawText(hDC, this.Text.Substring(idxCur, Len), Len, ref rc,
                        Win32Constants.DT_CALCRECT | Win32Constants.DT_LEFT | Win32Constants.DT_SINGLELINE | Win32Constants.DT_NOPREFIX | Win32Constants.DT_END_ELLIPSIS);
                    if (MaxWidth2 < rc.right)
                        MaxWidth2 = rc.right;
                    if (Height < Height2)
                        Height = Height2;
                }

                TotalHeight += Height;

                // Skip over the \r (or \n) - and allow for a trailing \n (or \r) if on exists.
                if (ch == '\r')
                {
                    idxEnd++;
                    ch = this.Text[idxEnd];
                    if (ch == '\n')
                    {
                        idxEnd++;
                    }
                }
                else if (ch == '\n')
                {
                    idxEnd++;
                    ch = this.Text[idxEnd];
                    if (ch == '\r')
                    {
                        idxEnd++;
                    }
                }

                // Use this as the new starting pos...
                idxCur = idxEnd;
            }

            size.x = MaxWidth1 + TEXTBORDER * 2 + 2;
            if (MaxWidth2 != 0)
            {
                size.x += TEXTTABMIN + MaxWidth2;
                m_TabStop = MaxWidth1;
            }
            else
            {
                m_TabStop = 0;
            }
            size.y = TotalHeight + TEXTBORDER * 2 + 2;

            NativeMethods.SelectObject(hDC, hFont);
            NativeMethods.ReleaseDC(IntPtr.Zero, hDC);
        }

        /// <summary>
        /// WinProc
        /// </summary>
        /// <param name="hWnd"></param>
        /// <param name="msg"></param>
        /// <param name="wParam"></param>
        /// <param name="lParam"></param>
        /// <returns></returns>
        public IntPtr WndProc(IntPtr hWnd, uint uMsg, IntPtr wParam, IntPtr lParam)
        {
            switch (uMsg)
            {
                case Win32Constants.WM_NCHITTEST:
                    return (IntPtr)Win32Constants.HTTRANSPARENT;

                // Using ERASEBKGND instead of PAINT gives us more responsive update
                case Win32Constants.WM_ERASEBKGND:
                    {
                        if (this.Text == null || string.IsNullOrEmpty(this.Text))
                            return (IntPtr)1;

                        IntPtr hDC = wParam;
                        IntPtr hFont = NativeMethods.SelectObject(hDC, this.hFont);

                        NativeMethods.GetClientRect(hWnd, out RECT rcClient);

                        IntPtr hBrush = NativeMethods.CreateSolidBrush(this.BackgroundColor);
#pragma warning disable CA1806 // Do not ignore method results
                        NativeMethods.FillRect(hDC, ref rcClient, hBrush);
                        hBrush = NativeMethods.SelectObject(hDC, hBrush);
                        NativeMethods.SetBkColor(hDC, this.BackgroundColor);
                        NativeMethods.SetTextColor(hDC, this.TextColor);
#pragma warning restore CA1806 // Do not ignore method results

                        int CurYPos = 1 + TEXTBORDER;

                        int idxCur = 0;
                        while (idxCur < this.Text.Length)
                        {
                            // Find the end of the line...
                            int idxEnd = idxCur;
                            var ch = this.Text[idxEnd];

                            while (ch != '\r' && ch != '\n' && ch != '\t')
                            {
                                idxEnd++;
                                if (idxEnd < this.Text.Length)
                                {
                                    ch = this.Text[idxEnd];
                                }
                                else
                                {
                                    break;
                                }
                            }

                            int Len = (int)(idxEnd - idxCur);

                            // Now pass the line to DrawText to get the height/length...
                            RECT rc = new RECT() { left = 1 + TEXTBORDER, top = CurYPos, right = rcClient.right, bottom = rcClient.bottom };
                            // TODO - set tab stops correctly (default is 8 - probably too small...
                            int Height = NativeMethods.DrawText(hDC, this.Text.Substring(idxCur, Len), Len, ref rc,
                                    Win32Constants.DT_NOCLIP | Win32Constants.DT_LEFT | Win32Constants.DT_SINGLELINE | Win32Constants.DT_NOPREFIX);

                            if (ch == '\t')
                            {
                                // work out second part of the tab-stop'd line...
                                idxEnd++;
                                idxCur = idxEnd;

                                while (ch != '\r' && ch != '\n')
                                {
                                    idxEnd++;
                                    if (idxEnd < this.Text.Length)
                                    {
                                        ch = this.Text[idxEnd];
                                    }
                                    else
                                    {
                                        break;
                                    }
                                }

                                rc.left += m_TabStop + TEXTTABMIN;
                                Len = (int)(idxEnd - idxCur);
                                int Height2 = NativeMethods.DrawText(hDC, this.Text.Substring(idxCur, Len), Len, ref rc,
                                        Win32Constants.DT_NOCLIP | Win32Constants.DT_LEFT | Win32Constants.DT_SINGLELINE | Win32Constants.DT_NOPREFIX | Win32Constants.DT_END_ELLIPSIS);
                                if (Height < Height2)
                                    Height = Height2;
                            }

                            CurYPos += Height;

                            // Skip over the \r (or \n) - and allow for a trailing \n (or \r) if on exists.
                            if (ch == '\r')
                            {
                                idxEnd++;
                                ch = this.Text[idxEnd];
                                if (ch == '\n')
                                {
                                    idxEnd++;
                                }
                            }
                            else if (ch == '\n')
                            {
                                idxEnd++;
                                ch = this.Text[idxEnd];
                                if (ch == '\r')
                                {
                                    idxEnd++;
                                }
                            }

                            // Use this as the new starting pos...
                            idxCur = idxEnd;
                        }

                        hBrush = NativeMethods.SelectObject(hDC, hBrush);
                        NativeMethods.DeleteObject(hBrush);

                        IntPtr hPen = NativeMethods.GetStockObject(StockObjects.BLACK_PEN);
                        hPen = NativeMethods.SelectObject(hDC, hPen);

                        // Draw rectangle...
                        NativeMethods.MoveToEx(hDC, 0, 0, IntPtr.Zero);
                        NativeMethods.LineTo(hDC, 0, rcClient.bottom - 1);
                        NativeMethods.LineTo(hDC, rcClient.right - 1, rcClient.bottom - 1);
                        NativeMethods.LineTo(hDC, rcClient.right - 1, 0);
                        NativeMethods.LineTo(hDC, 0, 0);

                        NativeMethods.SelectObject(hDC, hPen);
                        NativeMethods.SelectObject(hDC, hFont);

                        return (IntPtr)1;
                    }
            }
            return NativeMethods.DefWindowProc(hWnd, uMsg, wParam, lParam);
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

        IntPtr CreateWindow()
        {
            return NativeMethods.CreateWindowEx(WindowStylesEx.WS_EX_TOOLWINDOW,
                this.WindowClassName, "",
                WindowStyles.WS_POPUP,
                1, 1, 1, 1,
                IntPtr.Zero,
                IntPtr.Zero,
                this.hInstance, IntPtr.Zero);
        }

        void UnRegisterWindowClass()
        {
            NativeMethods.UnregisterClass(this.WindowClassName, this.hInstance);
        }

        #region IDisposable Support
        private bool disposedValue; // To detect redundant calls

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                Holder.Remove(hWnd);
                NativeMethods.DeleteObject(hFont);
                NativeMethods.DestroyWindow(this.hWnd);
                UnRegisterWindowClass();

                disposedValue = true;
            }
        }

        ~TextTip()
        {
            // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
            Dispose(false);
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
