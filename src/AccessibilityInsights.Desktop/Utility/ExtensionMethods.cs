// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using AccessibilityInsights.Core.Bases;
using AccessibilityInsights.Win32;
using AccessibilityInsights.Desktop.UIAutomation;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using UIAutomationClient;
using System.Windows.Forms;
using Newtonsoft.Json;
using System.IO;
using AccessibilityInsights.Core.Misc;
using AccessibilityInsights.Core.Types;
using System.Diagnostics;
using System.Windows.Media.Imaging;
using System.Windows.Interop;
using System.Windows;
using System.Net;

namespace AccessibilityInsights.Desktop.Utility
{
    public static class ExtensionMethods
    {

        /// <summary>
        /// Get Process name
        /// </summary>
        /// <param name="e"></param>
        /// <returns></returns>
        public static string GetProcessName(this A11yElement e)
        {
            try
            {
                var prc = Process.GetProcessById(e.ProcessId);

                return prc.ProcessName;
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// Get Process Main Module information
        /// </summary>
        /// <param name="e"></param>
        /// <returns></returns>
        public static ProcessModule GetProcessModule(this A11yElement e)
        {
            try
            {
                var prc = Process.GetProcessById(e.ProcessId);

                return prc.MainModule;
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// Change IUIAutomationElementArray To a list of DesktopElement
        /// </summary>
        /// <param name="array"></param>
        /// <returns></returns>
        public static List<DesktopElement> ToListOfDesktopElements( this IUIAutomationElementArray array)
        {
            List<DesktopElement> list = new List<DesktopElement>();

            for (int i = 0; i < array.Length; i++)
            {
                list.Add(new DesktopElement(array.GetElement(i)));
            }

            return list;
        }

        /// <summary>
        /// Capture the bitmap of the given element
        /// </summary>
        /// <param name="e"></param>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="width"></param>
        /// <param name="height"></param>
        /// <returns></returns>
        public static Bitmap CaptureBitmap(this A11yElement e)
        {
            var rect = e.BoundingRectangle;
            IntPtr wndHWND = NativeMethods.GetDesktopWindow();
            IntPtr wndHDC = NativeMethods.GetDC(wndHWND);

            Bitmap bmp;
            IntPtr dc1;
            IntPtr dc2;
            Graphics g;

            bmp = new Bitmap(rect.Width, rect.Height);
            g = System.Drawing.Graphics.FromImage(bmp);

            dc1 = g.GetHdc();
            dc2 = NativeMethods.GetWindowDC(wndHDC);

            NativeMethods.BitBlt(dc1, 0, 0, rect.Width, rect.Height, dc2, rect.Left, rect.Top, TernaryRasterOperations.SRCCOPY);

            g.ReleaseHdc(dc1);

            //  release window and capture resources
            NativeMethods.ReleaseDC(wndHWND, wndHDC); // release window context

            return bmp; 
        }

        /// <summary>
        /// Gets source for bitmap
        /// </summary>
        /// <param name="bitmap"></param>
        /// <returns></returns>
        public static BitmapSource ConvertToSource(this Bitmap bitmap)
        {
            var hbmp = bitmap.GetHbitmap();
            var result = Imaging.CreateBitmapSourceFromHBitmap(hbmp, IntPtr.Zero, Int32Rect.Empty, BitmapSizeOptions.FromEmptyOptions());
            NativeMethods.DeleteObject(hbmp);
            return result;
        }

        /// <summary>
        /// Check whether p is fully visible based on current screen size
        /// </summary>
        /// <param name="p"></param>
        /// <param name="c"></param>
        /// <param name="margin"></param>
        /// <returns></returns>
        public static bool IsVisibleLocation(this Rectangle p)
        {
            return (from s in Screen.AllScreens
                    where s.Bounds.Contains(p)
                    select s).Count() != 0;
        }

        /// <summary>
        /// Store config file as JSON in given location
        /// </summary>
        /// <param name="kc">Calling config object</param>
        /// <param name="path">Config file location and name</param>
        public static void SerializeInJSON(this ConfigurationBase kc, string path)
        {            
            var json = JsonConvert.SerializeObject(kc, Formatting.Indented);

            File.WriteAllText(path, json, Encoding.UTF8);
        }

        /// <summary>
        /// Gets window that is eventual parent of element. If there is no window parent, get
        /// highest element that isn't desktop and has a bounding rectangle
        /// </summary>
        /// <param name="el"></param>
        /// <returns></returns>
        public static A11yElement GetParentWindow(this A11yElement el)
        {
            A11yElement prev = null;
            A11yElement curr = el;
            while (curr != null && curr.Parent != null && curr.ControlTypeId != ControlType.UIA_WindowControlTypeId && !curr.IsRootElement())
            {
                prev = curr;
                curr = curr.Parent;
            }
            
            if (curr.ControlTypeId != ControlType.UIA_WindowControlTypeId && prev != null)
            {
                curr = prev;
                while (curr.UniqueId < -1 && curr.Children?[0].BoundingRectangle == null)
                {
                    if (curr.Children.Count > 0)
                    {
                        curr = curr.Children[0];
                    }
                    else
                    {
                        // if no elements have bounding rectangles, give up
                        break;
                    }
                }
            }
            return curr;
        }

        /// <summary>
        /// Get DPI
        /// </summary>
        /// <param name="rc"></param>
        /// <returns></returns>
        public static double GetDPI(this Rectangle rc)
        {
            return GetDPI(rc.Left, rc.Top);
        }

        /// <summary>
        /// Get DPI with left/top
        /// </summary>
        /// <param name="left"></param>
        /// <param name="top"></param>
        /// <returns></returns>
        public static double GetDPI(int left, int top)
        {
            NativeMethods.GetDpi(new System.Drawing.Point(left, top), DpiType.Effective, out uint dpiX, out uint dpiY);

            return GetDPIRate(dpiX);
        }

        /// <summary>
        /// Gets the DPI that WPF seems to use internally
        /// when positioning windows; scale by this DPI 
        /// when setting Window.Left / Window.Top before calling show()
        /// </summary>
        /// <returns></returns>
        public static double GetWPFWindowPositioningDPI()
        {
            return new Rectangle().GetDPI();
        }

        /// <summary>
        /// turn raw DPI value to DPI rate
        /// </summary>
        /// <param name="dpiX"></param>
        /// <returns></returns>
        private static double GetDPIRate(uint dpi)
        {
            return dpi / 96.0; // 96 is 100% scale. 
        }

        // If a web exception contains one of the following status values, it might be transient.
        private readonly static HashSet<WebExceptionStatus> TransientWebExceptions = new HashSet<WebExceptionStatus>()
        {
            WebExceptionStatus.ConnectionClosed,
            WebExceptionStatus.Timeout,
            WebExceptionStatus.RequestCanceled,
            WebExceptionStatus.NameResolutionFailure
        };

        /// <summary>
        /// Attempt to determine if we expect an exception to be transient.
        /// Will check children of an aggregate exception.
        /// Based on https://docs.microsoft.com/en-us/azure/architecture/patterns/retry
        /// </summary>
        /// <param name="ex">The exception to check</param>
        /// <returns></returns>
        public static bool IsTransient(this Exception ex)
        {
            switch (ex)
            {
                case null:
                    return false;
                case AggregateException agEx:
                    foreach (var inner in agEx.InnerExceptions)
                    {
                        if (!inner.IsTransient()) return false;
                    }
                    return true;
                case WebException webEx:
                    return TransientWebExceptions.Contains(webEx.Status);
                // This is what we saw happen to bug attachments in our telemetry
                case TimeoutException tEx:
                    return true;
                default:
                    return false;
            }
        }

        /// <summary>
        /// Convert a Url string to a Uri, with proper handling of null inputs
        /// </summary>
        /// <param name="stringValue">The url (potentially empty or null) to convert</param>
        /// <returns>The Uri if the url is neither empty nor null</returns>
        public static Uri ToUri(this string stringValue)
        {
            if (string.IsNullOrEmpty(stringValue))
                return null;

            return new Uri(stringValue);
        }
    }
}
