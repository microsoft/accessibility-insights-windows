// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Text;

namespace Axe.Windows.Win32
{
    /// <summary>
    /// Class NativeMethods for Win32 Apis pInvoke and Win32 specific code.
    /// Some of these definitions originated from https://pinvoke.net/
    /// </summary>
    internal static partial class NativeMethods
    {
        /// <returns>If function succeeds, it returns 0(S_OK). Otherwise, it returns an error code.</returns>
        [DllImport("ole32.dll", CharSet = CharSet.Auto, SetLastError = true, CallingConvention = CallingConvention.StdCall)]
        internal static extern int CoInitializeEx( [In, Optional] IntPtr pvReserved, [In]  COINIT dwCoInit);

        [DllImport("ole32.dll", CharSet = CharSet.Auto, SetLastError = true, CallingConvention = CallingConvention.StdCall)]
        internal static extern int CoInitialize( [In, Optional] IntPtr pvReserved);

        [DllImport("ole32.dll", CharSet = CharSet.Auto, SetLastError = true, CallingConvention = CallingConvention.StdCall)]
        internal static extern int CoUninitialize();

        [DllImport("ole32.dll", CharSet = CharSet.Unicode, ExactSpelling = true, PreserveSig = false)]
        internal static extern int CoCreateInstanceEx( [In, MarshalAs(UnmanagedType.LPStruct)] Guid rclsid,
                                    [MarshalAs(UnmanagedType.IUnknown)] object pUnkOuter,
                                    CLSCTX dwClsCtx,
                                    IntPtr pServerInfo,
                                    uint cmq,
                                    [In, Out] MULTI_QI[] pResults);

        [DllImport("ole32.dll")]
        internal static extern int CoCreateInstance([In]ref Guid rclsid, IntPtr pUnkOuter,
                                   Int32 dwClsContext,
                                   [In]ref Guid riid,
                                   [MarshalAs(UnmanagedType.Interface)] out object ppv);

        [DllImport("gdi32.dll", CharSet = CharSet.Auto, SetLastError = true, ExactSpelling = true)]
        internal static extern uint GetDeviceCaps(IntPtr hDC, int nIndex);

        [DllImport("Gdi32.dll", CharSet = CharSet.Auto, SetLastError = true, ExactSpelling = true)]
        internal static extern IntPtr CreateCompatibleDC(IntPtr hDC);

        [DllImport("user32.dll", SetLastError = true)]
        internal static extern int SetWindowLong(IntPtr hWnd, int nIndex, uint dwNewLong);

        //https://msdn.microsoft.com/en-us/library/windows/desktop/dd145062(v=vs.85).aspx
        [DllImport("User32.dll")]
        internal static extern IntPtr MonitorFromPoint([In]Point pt, [In]uint dwFlags);

        [DllImport("user32.dll")]
        internal static extern bool SystemParametersInfo(int iAction, int iParam, out bool bActive, int iUpdate);

        //https://msdn.microsoft.com/en-us/library/windows/desktop/dn280510(v=vs.85).aspx
        [DllImport("Shcore.dll")]
        internal static extern IntPtr GetDpiForMonitor([In]IntPtr hmonitor, [In]DpiType dpiType, [Out]out uint dpiX, [Out]out uint dpiY);

        [DllImport("user32.dll", SetLastError = true)]
        internal static extern bool TrackMouseEvent(ref TRACKMOUSEEVENT lpEventTrack);

        /// <summary>
        /// Open Process
        /// </summary>
        /// <param name="processAccess"></param>
        /// <param name="bInheritHandle"></param>
        /// <param name="processId"></param>
        /// <returns></returns>
        [DllImport("kernel32.dll", SetLastError = true)]
        internal static extern IntPtr OpenProcess(ProcessAccessFlags processAccess, bool bInheritHandle, int processId );

        /// <summary>
        /// Clean up Variant
        /// </summary>
        /// <param name="pvarg"></param>
        /// <returns></returns>
        [DllImport("OleAut32.dll")]
        internal static extern uint VariantClear(ref dynamic pvarg);

        /// <summary>
        /// EnumProcessModules
        /// </summary>
        /// <param name="hProcess"></param>
        /// <param name="lphModule"></param>
        /// <param name="cb"></param>
        /// <param name="lpcbNeeded"></param>
        /// <returns></returns>
        [DllImport("psapi.dll", SetLastError = true)]
        internal static extern bool EnumProcessModules(IntPtr hProcess, out IntPtr lphModule, int cb, [MarshalAs(UnmanagedType.U4)] out uint lpcbNeeded);

        /// <summary>
        /// Get Module Base Name
        /// </summary>
        /// <param name="hProcess"></param>
        /// <param name="hModule"></param>
        /// <param name="lpBaseName"></param>
        /// <param name="nSize"></param>
        /// <returns></returns>
        [DllImport("psapi.dll", CharSet = CharSet.Unicode)]
        internal static extern uint GetModuleBaseName(IntPtr hProcess, IntPtr hModule, StringBuilder lpBaseName, uint nSize);

        /// <summary>
        /// Close handle
        /// </summary>
        /// <param name="hHandle"></param>
        /// <returns></returns>
        [DllImport("kernel32.dll", SetLastError = true)]
        internal static extern bool CloseHandle(IntPtr hHandle);

        /// <summary>
        /// Open process Token
        /// </summary>
        /// <param name="ProcessHandle"></param>
        /// <param name="DesiredAccess"></param>
        /// <param name="TokenHandle"></param>
        /// <returns></returns>
        [DllImport("advapi32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        internal static extern bool OpenProcessToken(IntPtr ProcessHandle, UInt32 DesiredAccess, out IntPtr TokenHandle);

        [DllImport("advapi32.dll", SetLastError = true)]
        internal static extern bool GetTokenInformation(IntPtr TokenHandle, TOKEN_INFORMATION_CLASS TokenInformationClass, out uint TokenInformation, uint TokenInformationLength, out uint ReturnLength);

        [DllImport("user32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        internal static extern bool GetCursorPos(out POINT point);

        [DllImport("advapi32.dll", SetLastError = true)]
        internal static extern int RegCloseKey(UIntPtr hKey);

        [DllImport("advapi32.dll", CharSet = CharSet.Unicode)]
        internal static extern int RegOpenKeyEx(
          UIntPtr hKey,
          string subKey,
          int ulOptions,
          int samDesired,
          out UIntPtr hkResult);

        // This signature will not get an entire REG_BINARY value. It will stop at the first null byte.
        [DllImport("advapi32.dll", CharSet = CharSet.Unicode, EntryPoint = "RegQueryValueExW", SetLastError = true)]
        internal static extern int RegQueryValueEx(
            UIntPtr hKey,
            string lpValueName,
            IntPtr lpReserved,
            out uint lpType,
            System.Text.StringBuilder lpData,
            ref uint lpcbData);

        [DllImport("user32.dll")]
        internal static extern IntPtr GetForegroundWindow();

        [DllImport("user32.dll")]
        internal static extern bool SetForegroundWindow(IntPtr hWnd);

        [DllImport("gdi32.dll", EntryPoint = "DeleteDC")]
        internal static extern bool DeleteDC([In] IntPtr hdc);

        [DllImport("gdi32.dll", EntryPoint = "BitBlt", SetLastError = true)]
        internal static extern bool BitBlt([In] IntPtr hdc, int nXDest, int nYDest, int nWidth, int nHeight, [In] IntPtr hdcSrc, int nXSrc, int nYSrc, TernaryRasterOperations dwRop);

        [DllImport("user32.dll")]
        internal static extern bool CloseWindow(IntPtr hWnd);

        [DllImport("gdi32.dll", CharSet = CharSet.Unicode)]
        internal static extern IntPtr CreateFont(int nHeight, int nWidth, int nEscapement,
         int nOrientation, FontWeight fnWeight, bool fdwItalic, bool fdwUnderline, 
         bool fdwStrikeOut, FontCharSet fdwCharSet, FontPrecision fdwOutputPrecision,
         FontClipPrecision fdwClipPrecision, FontQuality fdwQuality, FontPitchAndFamily fdwPitchAndFamily, string lpszFace);

        [DllImport("user32.dll", SetLastError = false)]
        internal static extern IntPtr GetDesktopWindow();

        [DllImport("user32.dll")]
        internal static extern int GetSystemMetrics(SystemMetric smIndex);

        [DllImport("user32.dll")]
        internal static extern IntPtr GetDC(IntPtr hWnd);

        [DllImport("gdi32.dll")]
        internal static extern uint SetBkColor(IntPtr hdc, int crColor);

        [DllImport("gdi32.dll")]
        internal static extern uint SetTextColor(IntPtr hdc, int crColor);

        [DllImport("user32.dll", CharSet = CharSet.Unicode)]
        internal static extern int DrawText(IntPtr hDC, string lpString, int nCount, ref RECT lpRect, uint uFormat);

        [DllImport("kernel32.dll")]
        internal static extern uint GetLastError();

        // FindWindow() is used by the LinkProcessor and SampleEventHandler to finds a browser window.

        [DllImport("kernel32.dll", CharSet = CharSet.Unicode)]
        internal static extern IntPtr GetModuleHandle(string strModuleName);

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.U2)]
        internal static extern short RegisterClassEx([In] ref WNDCLASSEX lpwcx);

        [DllImport("user32.dll", CharSet = CharSet.Unicode)]
        internal static extern IntPtr CreateWindowEx(
           WindowStylesEx dwExStyle,
           string lpClassName,
           string lpWindowName,
           WindowStyles dwStyle,
           int x,
           int y,
           int nWidth,
           int nHeight,
           IntPtr hWndParent,
           IntPtr hMenu,
           IntPtr hInstance,
           IntPtr lpParam);

        [DllImport("user32.dll")]
        internal static extern bool DestroyWindow(IntPtr hWnd);

        [DllImport("user32.dll")]
        internal static extern bool ShowWindow(IntPtr hWnd, ShowWindowCommands nCmdShow);

        [DllImport("user32.dll")]
        internal static extern bool UpdateWindow(IntPtr hWnd);

        [return: MarshalAs(UnmanagedType.Bool)]
        [DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        internal static extern bool PostMessage(IntPtr hWnd, uint Msg, IntPtr wParam, IntPtr lParam);

        [DllImport("user32.dll")]
        internal static extern void PostQuitMessage(int nExitCode);

        [DllImport("user32.dll")]
        internal static extern bool GetMessage(ref WndMsg msg, IntPtr hWnd, uint min, uint max);

        [DllImport("user32.dll")]
        internal static extern bool TranslateMessage([In] ref WndMsg lpMsg);

        [DllImport("user32.dll")]
        internal static extern IntPtr DispatchMessage([In] ref WndMsg lpmsg);

        [DllImport("user32.dll")]
        internal static extern IntPtr DefWindowProc(IntPtr hWnd, uint uMsg, IntPtr wParam, IntPtr lParam);

        [DllImport("user32.dll")]
        internal static extern System.UInt32 GetWindowLong(IntPtr hWnd, int nIndex);

        [DllImport("user32.dll")]
        internal static extern bool SetWindowPos(IntPtr hWnd, IntPtr hWndInsertAfter, int x, int y, int cx, int cy, uint uFlags);

        [DllImport("user32.dll", SetLastError = true)]
        internal static extern bool BringWindowToTop(IntPtr hWnd);

        [DllImport("user32.dll")]
        internal static extern bool InvalidateRect(IntPtr hWnd, IntPtr lpRect, bool bErase);

        [DllImport("user32.dll")]
        internal static extern IntPtr SetActiveWindow(IntPtr hWnd);
        [DllImport("user32.dll")]
        internal static extern bool AttachThreadInput(uint idAttach, uint idAttachTo, bool fAttach);
        [DllImport("user32.dll")]
        internal static extern uint GetWindowThreadProcessId(IntPtr hWnd, IntPtr ProcessId);

        // DLL libraries used to manage hotkeys
        [DllImport("user32.dll")]
        internal static extern bool RegisterHotKey(IntPtr hWnd, int id, int fsModifiers, int vlc);
        [DllImport("user32.dll")]
        internal static extern bool UnregisterHotKey(IntPtr hWnd, int id);

        [DllImport("user32.dll")]
        internal static extern IntPtr GetWindowDC(IntPtr hWnd);

        [DllImport("gdi32.dll", EntryPoint = "SelectObject")]
        internal static extern IntPtr SelectObject(IntPtr hdc, IntPtr hgdiobj);

        [DllImport("gdi32.dll")]
        internal static extern IntPtr GetStockObject(StockObjects fnObject);

        [DllImport("gdi32.dll")]
        internal static extern bool Rectangle(IntPtr hdc, int nLeftRect, int nTopRect, int nRightRect, int nBottomRect);

        [DllImport("user32.dll")]
        internal static extern bool ReleaseDC(IntPtr hWnd, IntPtr hDC);

        [DllImport("gdi32.dll", EntryPoint = "DeleteObject")]
        [return: MarshalAs(UnmanagedType.Bool)]
        internal static extern bool DeleteObject(IntPtr hObject);

        [DllImport("user32.dll")]
        internal static extern bool GetClientRect(IntPtr hWnd, out RECT lpRect);

        [DllImport("gdi32.dll")]
        internal static extern IntPtr CreateSolidBrush(int crColor);

        [DllImport("user32.dll")]
        internal static extern int FillRect(IntPtr hDC, [In] ref RECT lprc, IntPtr hbr);

        [DllImport("gdi32.dll")]
        internal static extern bool MoveToEx(IntPtr hdc, int X, int Y, IntPtr lpPoint);

        [DllImport("gdi32.dll")]
        internal static extern bool LineTo(IntPtr hdc, int nXEnd, int nYEnd);

        [DllImport("user32.dll", CharSet = CharSet.Unicode)]
        internal static extern bool UnregisterClass(string lpClassName, IntPtr hInstance);

        [DllImport("user32", ExactSpelling = true, SetLastError = true)]
        internal static extern int MapWindowPoints(IntPtr hWndFrom, IntPtr hWndTo, [In, Out] ref RECT rect, [MarshalAs(UnmanagedType.U4)] int cPoints);

        [DllImport("gdi32.dll")]
        internal static extern IntPtr CreateRectRgn(int nLeftRect, int nTopRect, int nRightRect, int nBottomRect);

        [DllImport("gdi32.dll")]
        internal static extern int CombineRgn(IntPtr hrgnDest, IntPtr hrgnSrc1, IntPtr hrgnSrc2, int fnCombineMode);

        [DllImport("user32.dll")]
        internal static extern int SetWindowRgn(IntPtr hWnd, IntPtr hRgn, bool bRedraw);

        [DllImport("User32.dll")]
        internal static extern bool SetCursorPos(int X, int Y);

        [DllImport("shcore.dll")]
        internal static extern int SetProcessDpiAwareness(int flag);

        [DllImport("wintrust.dll", ExactSpelling = true, SetLastError = false, CharSet = CharSet.Unicode)]
        public static extern WinVerifyTrustResult WinVerifyTrust(
        [In] IntPtr hwnd,
        [In] [MarshalAs(UnmanagedType.LPStruct)] Guid pgActionID,
        [In] WinTrustData pWVTData);

        public static readonly Guid WINTRUST_ACTION_GENERIC_VERIFY_V2 = new Guid("{00AAC56B-CD44-11d0-8CC2-00C04FC295EE}");
    }
}
