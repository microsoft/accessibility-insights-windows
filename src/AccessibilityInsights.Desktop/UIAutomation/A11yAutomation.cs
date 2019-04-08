// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using Axe.Windows.Core.Bases;
using Axe.Windows.Core.Enums;
using Axe.Windows.Core.Misc;
using Axe.Windows.Core.Types;
using Axe.Windows.Desktop.UIAutomation.TreeWalkers;
using UIAutomationClient;

namespace Axe.Windows.Desktop.UIAutomation
{
    /// <summary>
    /// Wrapper for CUIAutomation COM object
    /// </summary>
    public static class A11yAutomation
    {
        static CUIAutomation UIAutomation = new CUIAutomation();

        /// <summary>
        /// Get UIAutomation8 Object
        /// it is singleton model. basically when you call it, it returns precreated object as needed. 
        /// </summary>
        /// <returns></returns>
        public static CUIAutomation GetUIAutomationObject()
        {
            return UIAutomation;
        }

        /// <summary>
        /// Get DesktopElement based on Process Id. 
        /// </summary>
        /// <param name="pid"></param>
        /// <returns>return null if fail to get an element by process Id</returns>
        public static DesktopElement ElementFromProcessId(int pid)
        {
            IUIAutomationElement root = null;
            DesktopElement element = null;
            IUIAutomationCondition condition = null;
            try
            {
                // check whether process exist first. 
                // if not, it will throw an ArgumentException
                using (var proc = Process.GetProcessById(pid))
                {
                    root = UIAutomation.GetRootElement();
                    condition = UIAutomation.CreatePropertyCondition(PropertyType.UIA_ProcessIdPropertyId, pid);
                    var uia = root.FindFirst(TreeScope.TreeScope_Descendants, condition);
                    element = ElementFromUIAElement(uia);
                }
            }
            catch (Exception ex)
            {
                //silent and let it return null
                Debug.WriteLine(ex);
            }
            finally
            {
                if (root != null)
                {
                    Marshal.ReleaseComObject(root);
                }
                if (condition != null)
                {
                    Marshal.ReleaseComObject(condition);
                }
            }
            return element;
        }

        /// <summary>
        /// Get the top level DesktopElement from Windows handle
        /// </summary>
        /// <param name="hWnd"></param>
        /// <returns></returns>
        public static DesktopElement ElementFromHandle(IntPtr hWnd)
        {
            try
            {
                return ElementFromUIAElement(UIAutomation.ElementFromHandle(hWnd));
            }
            catch
            {
            }

            return null;
        }

        /// <summary>
        /// Get DesktopElement from UIAElement interface. 
        /// </summary>
        /// <param name="uia"></param>
        /// <returns></returns>
        private static DesktopElement ElementFromUIAElement(IUIAutomationElement uia)
        {
            if (uia != null)
            {
                if (!DesktopElement.IsFromCurrentProcess(uia))
                {
                    var el = new DesktopElement(uia, true, false);

                    el.PopulateMinimumPropertiesForSelection();

                    return el;
                }
                else
                {
                    Marshal.ReleaseComObject(uia);
                }
            }

            return null;
        }

        /// <summary>
        /// Get the top level IUIAutomationElement from Windows handle
        /// </summary>
        /// <param name="hWnd"></param>
        /// <returns></returns>
        public static IUIAutomationElement UIAElementFromHandle(IntPtr hWnd)
        {
            try
            {
                return UIAutomation.ElementFromHandle(hWnd);
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// Get the focused Element
        /// </summary>
        /// <returns></returns>
        public static DesktopElement GetFocusedElement()
        {
            try
            {
                var uia = UIAutomation.GetFocusedElement();

                if (!DesktopElement.IsFromCurrentProcess(uia))
                {
                    return new DesktopElement(uia, true);
                }
                else
                {
                    Marshal.ReleaseComObject(uia);
                }
            }
            catch
            {
            }

            return null;

        }

        /// <summary>
        /// Get an App element from uia
        /// it is trace back to the top most ancestor with same process Id. 
        /// </summary>
        /// <param name="e">A11yElement</param>
        /// <returns></returns>
        public static A11yElement GetAppElement(A11yElement e)
        {
            var walker = GetTreeWalker(TreeViewMode.Control);
            var tree = new DesktopElementAncestry(TreeViewMode.Control, e);
            Marshal.ReleaseComObject(walker);
            A11yElement app = tree.First;

            // make sure that we have an app first
            if (app != null)
            {
                // get first item under Desktop. if app is not Desktop, then it is ok to take it as is. 
                if (app.IsRootElement())
                {
                    app = app.Children.First();
                }

                tree.Items.Remove(app);

                tree.Items.ForEach(el => el.Dispose());

                // make sure that Unique ID is set to 0 since this element will be POI.
                app.UniqueId = 0;
            }

            return app;
        }

        /// <summary>
        /// Get IUIAutomationTreeWalker based on inidcated mode.
        /// </summary>
        /// <param name="mode">TreeViewMode to get walker</param>
        /// <returns></returns>
        public static IUIAutomationTreeWalker GetTreeWalker(TreeViewMode mode)
        {
            IUIAutomationTreeWalker walker = null;

            CUIAutomation h = A11yAutomation.GetUIAutomationObject();

            switch (mode)
            {
                case TreeViewMode.Content:
                    walker = h.ContentViewWalker;
                    break;
                case TreeViewMode.Control:
                    walker = h.ControlViewWalker;
                    break;
                case TreeViewMode.Raw:
                    walker = h.RawViewWalker;
                    break;
            }

            return walker;
        }

        /// <summary>
        /// Get DesktopElement from x, y position
        /// </summary>
        /// <param name="xPos"></param>
        /// <param name="yPos"></param>
        /// <returns></returns>
        public static DesktopElement ElementFromPoint(int xPos, int yPos)
        {
            try
            {
                var uia = UIAutomation.ElementFromPoint(new tagPOINT() { x = xPos, y = yPos });

                if (!DesktopElement.IsFromCurrentProcess(uia))
                {
                    var e = new DesktopElement(uia, true, false);
                    e.PopulateMinimumPropertiesForSelection();

                    return e;

                }
                else
                {
                    Marshal.ReleaseComObject(uia);
                }
            }
            catch
            {
            }

            return null;
        }

        /// <summary>
        /// Get Property programmatic Name from UIA service. 
        /// </summary>
        /// <param name="pid"></param>
        /// <returns></returns>
        public static string GetPropertyProgrammaticName(int pid)
        {
            return UIAutomation.GetPropertyProgrammaticName(pid);
        }

        /// <summary>
        /// Get DesktopElement from Desktop
        /// </summary>
        /// <returns></returns>
        public static DesktopElement ElementFromDesktop()
        {
            return new DesktopElement(UIAutomation.GetRootElement());
        }
    }
}
