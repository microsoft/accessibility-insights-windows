// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
namespace Axe.Windows.Win32
{
    /// <summary>
    /// Win32-related constants. Some of these definitions originated from https://pinvoke.net/
    /// </summary>
    public static class Win32Constants
    {
        public const int MAX_PATH = 512;

        //Declare the mouse hook constant.
        //For other hook types, you can obtain these values from Winuser.h in the Microsoft SDK.
        public const int WH_MOUSE = 7;
        public const int WH_MOUSE_LL = 14;
        public const int WM_LBUTTONUP = 0x0202;

        public const string WC_MAGNIFIER = "Magnifier";

        public const int TME_LEAVE = 0x00000002;

        public const uint LWA_ALPHA = 0x00000002;

        public const int GWL_STYLE = -16;
        public const int GWL_EXSTYLE = -20;

        public const int SWP_NOZORDER = 0x0004;
        public const int SWP_NOREDRAW = 0x0008;
        public const int SWP_NOACTIVATE = 0x0010;

        public const int WM_CREATED = 0x0001;
        public const int WM_DESTROY = 0x0002;
        public const int WM_CLOSE = 0x0010;
        public const int WM_DPICHANGED = 0x02E0;
        public const int WM_MOUSELEAVE = 0x02A3;
        public const int WM_MOUSEMOVE = 0x0200;
        public const int WM_NCCREATE = 0x0081;
        public const int WM_NCHITTEST = 0x0084;

        public const int HTTRANSPARENT = -1;
        public const int WM_ERASEBKGND = 0x0014;
        public const int HWND_TOPMOST = -1;

        public const int DT_TOP = 0x00000000;
        public const int DT_LEFT = 0x00000000;
        public const int DT_CENTER = 0x00000001;
        public const int DT_RIGHT = 0x00000002;
        public const int DT_VCENTER = 0x00000004;
        public const int DT_BOTTOM = 0x00000008;
        public const int DT_WORDBREAK = 0x00000010;
        public const int DT_SINGLELINE = 0x00000020;
        public const int DT_EXPANDTABS = 0x00000040;
        public const int DT_TABSTOP = 0x00000080;
        public const int DT_NOCLIP = 0x00000100;
        public const int DT_EXTERNALLEADING = 0x00000200;
        public const int DT_CALCRECT = 0x00000400;
        public const int DT_NOPREFIX = 0x00000800;
        public const int DT_INTERNAL = 0x00001000;
        public const int DT_END_ELLIPSIS = 0x00008000;

        public const int FW_NORMAL = 400;
        public const int DEFAULT_CHARSET = 1;
        public const int OUT_OUTLINE_PRECIS = 8;

        public const int DPI_AWARENESS_CONTEXT_UNAWARE              = -1;
        public const int DPI_AWARENESS_CONTEXT_SYSTEM_AWARE         = -2;
        public const int DPI_AWARENESS_CONTEXT_PER_MONITOR_AWARE    = -3;
        public const int DPI_AWARENESS_CONTEXT_PER_MONITOR_AWARE_V2 = -4;

        public const uint STANDARD_RIGHTS_REQUIRED = 0x000F0000;
        public const uint STANDARD_RIGHTS_READ = 0x00020000;
        public const uint TOKEN_ASSIGN_PRIMARY = 0x0001;
        public const uint TOKEN_DUPLICATE = 0x0002;
        public const uint TOKEN_IMPERSONATE = 0x0004;
        public const uint TOKEN_QUERY = 0x0008;
        public const uint TOKEN_QUERY_SOURCE = 0x0010;
        public const uint TOKEN_ADJUST_PRIVILEGES = 0x0020;
        public const uint TOKEN_ADJUST_GROUPS = 0x0040;
        public const uint TOKEN_ADJUST_DEFAULT = 0x0080;
        public const uint TOKEN_ADJUST_SESSIONID = 0x0100;
        public const uint TOKEN_READ = (STANDARD_RIGHTS_READ | TOKEN_QUERY);
        public const uint TOKEN_ALL_ACCESS = (STANDARD_RIGHTS_REQUIRED | TOKEN_ASSIGN_PRIMARY |
        TOKEN_DUPLICATE | TOKEN_IMPERSONATE | TOKEN_QUERY | TOKEN_QUERY_SOURCE |
        TOKEN_ADJUST_PRIVILEGES | TOKEN_ADJUST_GROUPS | TOKEN_ADJUST_DEFAULT |
        TOKEN_ADJUST_SESSIONID);
        public const int S_OK = 0;

        /// <summary>
        /// Constant identify if windows is minimized
        /// </summary>
        public const uint WS_MINIMIZE = 0x20000000;
    }
}
