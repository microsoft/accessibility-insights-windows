// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using AccessibilityInsights.SharedUx.Utilities;
using AccessibilityInsights.Win32;
using System;
using System.Drawing;
using System.Runtime.InteropServices;
using static System.FormattableString;

namespace AccessibilityInsights.SharedUx.Highlighting
{
    /// <summary>
    /// Class LineBorder
    /// it will draw a single border of highlighter
    /// Ported from C++ code of Hilight.cpp in Windows AT team code for Inspect.
    /// </summary>
    internal class LineBorder : IDisposable
    {
        private static readonly ReferenceHolder<IntPtr, LineBorder> Holder = new ReferenceHolder<IntPtr, LineBorder>();
        private static readonly WndProc MyWndProc = new WndProc(StaticWndProc);
        readonly IntPtr hWnd;

        public string WindowClassName { get; private set; }

        readonly IntPtr hInstance;
        readonly int Id;

        public int Color { get; set; }
        public Rectangle Rect { get; set; }
        public int Gap { get; set; }
        public int Width { get; set; }

        public bool ExcludeRect { get; set; }
        public RECT ExcludedRect { get; set; }
        public bool IsVisible { get; internal set; }

        IntPtr m_hrgn;

        public LineBorder(string cnb, int id)
        {
            this.WindowClassName = Invariant($"{cnb}-{id}");
            this.hInstance = NativeMethods.GetModuleHandle(null);
            this.Id = id;
            RegisterWindowClass();

            this.hWnd = CreateWindow();
            Holder.Add(this.hWnd, this);
        }

        private static IntPtr StaticWndProc(IntPtr hWnd, uint uMsg, IntPtr wParam, IntPtr lParam)
        {
            if (Holder.TryGet(hWnd, out LineBorder border))
            {
                return border.WndProc(hWnd, uMsg, wParam, lParam);
            }
            return NativeMethods.DefWindowProc(hWnd, uMsg, wParam, lParam);
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
                        IntPtr hDC = wParam;

                        NativeMethods.GetClientRect(hWnd, out RECT rc);

                        IntPtr hBrush = NativeMethods.CreateSolidBrush(this.Color);
#pragma warning disable CA1806 // Do not ignore method results
                        NativeMethods.FillRect(hDC, ref rc, hBrush);
#pragma warning restore CA1806 // Do not ignore method results
                        NativeMethods.DeleteObject(hBrush);

                        IntPtr hPen = NativeMethods.GetStockObject(StockObjects.BLACK_PEN);
                        hPen = NativeMethods.SelectObject(hDC, hPen);

                        // always draw left and right edges...
                        NativeMethods.MoveToEx(hDC, rc.left, rc.top, IntPtr.Zero);
                        NativeMethods.LineTo(hDC, rc.left, rc.bottom);
                        NativeMethods.MoveToEx(hDC, rc.right - 1, rc.top, IntPtr.Zero);
                        NativeMethods.LineTo(hDC, rc.right - 1, rc.bottom);

                        // if top, draw full top...
                        if (Id == 1)
                        {
                            // Full line across top...
                            NativeMethods.MoveToEx(hDC, rc.left, rc.top, IntPtr.Zero);
                            NativeMethods.LineTo(hDC, rc.right, rc.top);

                            // Small line across bottom...
                            NativeMethods.MoveToEx(hDC, rc.left + this.Width + 1, rc.bottom - 1, IntPtr.Zero);
                            NativeMethods.LineTo(hDC, rc.right - this.Width - 1, rc.bottom - 1);
                        }
                        else if (Id == 3)
                        {
                            // Full line across bottom...
                            NativeMethods.MoveToEx(hDC, rc.left, rc.bottom - 1, IntPtr.Zero);
                            NativeMethods.LineTo(hDC, rc.right, rc.bottom - 1);

                            // Small line across top...
                            NativeMethods.MoveToEx(hDC, rc.left + this.Width + 1, rc.top, IntPtr.Zero);
                            NativeMethods.LineTo(hDC, rc.right - this.Width - 1, rc.top);
                        }

                        NativeMethods.SelectObject(hDC, hPen);
                        return (IntPtr)1;
                    }
            }
            return NativeMethods.DefWindowProc(hWnd, uMsg, wParam, lParam);

        }

        /// <summary>
        /// Set the Visibility state.
        /// </summary>
        /// <param name="bVisible"></param>
        public void SetVisible(bool bVisible)
        {
            this.IsVisible = bVisible;
            Update(false, true);
        }

        public void Update(bool fColorChanged, bool fLocSizeChanged)
        {
            if (fColorChanged)
            {
                NativeMethods.InvalidateRect(this.hWnd, IntPtr.Zero, true);
            }

            if (fLocSizeChanged)
            {
                // Show/hide is necessary to get a clean transition without visual artifacts being
                // left behind.
                //
                // The SetWindowPos is before the SetwindowRgn call to eliminate a slight
                // flicker that occurs if they are the other way around.
                NativeMethods.ShowWindow(this.hWnd, ShowWindowCommands.Hide);
                RECT rcReal = CalcRect(this.Rect);

                NativeMethods.SetWindowPos(hWnd, (IntPtr)Win32Constants.HWND_TOPMOST, rcReal.left, rcReal.top, rcReal.right - rcReal.left, rcReal.bottom - rcReal.top,
                    Win32Constants.SWP_NOACTIVATE);

#pragma warning disable CA1806 // Do not ignore method results
                if (this.ExcludeRect)
                {
                    // Map exclude rectangle to client coords...
                    RECT rcExclude = this.ExcludedRect;
                    NativeMethods.MapWindowPoints(IntPtr.Zero, hWnd, ref rcExclude, 2);

                    IntPtr hrgnTemp = m_hrgn;

                    // Subtract the exclude region from this window's region...
                    IntPtr rgnExclude = NativeMethods.CreateRectRgn(rcExclude.left, rcExclude.top, rcExclude.right, rcExclude.bottom);
                    IntPtr rgnThis = NativeMethods.CreateRectRgn(0, 0, rcReal.right - rcReal.left, rcReal.bottom - rcReal.top);
                    IntPtr rgnResult = NativeMethods.CreateRectRgn(0, 0, 0, 0);
                    NativeMethods.CombineRgn(rgnResult, rgnThis, rgnExclude, (int)CombineRgnStyles.RGN_DIFF);
                    NativeMethods.DeleteObject(rgnThis);
                    NativeMethods.DeleteObject(rgnExclude);

                    m_hrgn = rgnResult;

                    NativeMethods.SetWindowRgn(hWnd, m_hrgn, false);
                    if (hrgnTemp != IntPtr.Zero)
                        NativeMethods.DeleteObject(hrgnTemp);
                }
                else if (m_hrgn != IntPtr.Zero)
                {
                    // Exclude mode was turned off - clear old region...
                    NativeMethods.SetWindowRgn(hWnd, IntPtr.Zero, false);
                    NativeMethods.DeleteObject(m_hrgn);
                    m_hrgn = IntPtr.Zero;
                }
#pragma warning restore CA1806 // Do not ignore method results

                NativeMethods.ShowWindow(hWnd, this.IsVisible ? ShowWindowCommands.ShowNA : ShowWindowCommands.Hide);
            }
        }

        RECT CalcRect(Rectangle rcIn)
        {
            RECT prcOut = new RECT();

            switch (Id)
            {
                case 0: // left
                    prcOut.left = rcIn.Left - this.Gap - this.Width - 2;
                    prcOut.right = rcIn.Left - this.Gap;
                    prcOut.top = rcIn.Top - this.Gap;
                    prcOut.bottom = rcIn.Bottom + this.Gap;
                    break;
                case 1: // top (overhang)
                    prcOut.left = rcIn.Left - this.Gap - this.Width - 2;
                    prcOut.right = rcIn.Right + this.Gap + this.Width + 2;
                    prcOut.top = rcIn.Top - this.Gap - this.Width - 2;
                    prcOut.bottom = rcIn.Top - this.Gap;
                    break;
                case 2: // right
                    prcOut.left = rcIn.Right + this.Gap;
                    prcOut.right = rcIn.Right + this.Gap + this.Width + 2;
                    prcOut.top = rcIn.Top - this.Gap;
                    prcOut.bottom = rcIn.Bottom + this.Gap;
                    break;
                case 3: // bottom (underhang)
                    prcOut.left = rcIn.Left - this.Gap - this.Width - 2;
                    prcOut.right = rcIn.Right + this.Gap + this.Width + 2;
                    prcOut.top = rcIn.Bottom + this.Gap;
                    prcOut.bottom = rcIn.Bottom + this.Gap + this.Width + 2;
                    break;
            }

            return prcOut;
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
                NativeMethods.DestroyWindow(this.hWnd);
                UnRegisterWindowClass();

                disposedValue = true;
            }
        }

        ~LineBorder()
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
