// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using AccessibilityInsights.SharedUx.Telemetry;
using AccessibilityInsights.Win32;
using Axe.Windows.Actions;
using Axe.Windows.Core.Bases;
using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using UIAutomationClient;

namespace AccessibilityInsights.SharedUx.Highlighting
{
    /// <summary>
    /// Since OverlayHighlighter is used for Tabstop which doesn't record data into Any context.
    /// it is using element to set highlighting information.
    /// </summary>
    public class ClearOverlayDriver:IDisposable
    {
        /// <summary>
        /// OverlayHighlighter to use
        /// </summary>
        private OverlayHighlighter Highlighter;

        /// <summary>
        /// Constructor
        /// </summary>
        private ClearOverlayDriver()
        {
            Highlighter = new OverlayHighlighter();
        }

        /// <summary>
        /// Show
        /// </summary>
        public void Show()
        {
            Highlighter.Show();
        }

        /// <summary>
        /// Show Toast on top of the hiligher window
        /// </summary>
        public void ShowToast(UserControl toast)
        {
            this.Highlighter.ShowToast(toast);
        }

        /// <summary>
        /// Bring the main window of POI element to front
        /// </summary>
        public static void BringMainWindowOfPOIElementToFront()
        {
            try
            {
                var procId = SelectAction.GetDefaultInstance().POIElementContext.Element.ProcessId;
                Process proc = Process.GetProcessById(procId);
                proc.Refresh();
                IntPtr handle = proc.MainWindowHandle;
                NativeMethods.FocusWindow(handle);
                ((IUIAutomationElement)SelectAction.GetDefaultInstance().POIElementContext.Element.PlatformObject).SetFocus();
            }
            catch (Exception e)
            {
                if (e is ArgumentException ||
                    e is COMException ||
                    e is NullReferenceException ||
                    e is InvalidOperationException)
                {
                    e.ReportException();
                }
                else
                {
                    throw;
                }
            }
        }

        /// <summary>
        /// Draw rectangle around the selected element in the application
        /// </summary>
        public void MarkSelectedElement()
        {
            A11yElement ele = SelectAction.GetDefaultInstance().POIElementContext.Element;
            this.Highlighter.MarkSelectedElement(ele);
        }

        /// <summary>
        /// Draw rectangle around the selected element in the pattern window in the application
        /// </summary>
        public void MarkElement(A11yElement ele)
        {
            this.Highlighter.MarkElement(ele);
        }

        /// <summary>
        /// Hide
        /// </summary>
        public void Hide()
        {
            Highlighter?.Hide();
        }

        /// <summary>
        /// Add Element in highlighter
        /// Overlay highlighter needs element than Id. please see comment above at class definition.
        /// </summary>
        /// <param name="e">element</param>
        /// <param name="num">Id Number of highlighter</param>
        public void AddElement(A11yElement e, string num)
        {
            Highlighter.AddElementRoundBorder(e,num);
        }

        /// <summary>
        /// Clear highligted elements in bitmap.
        /// Overlay highlighter needs element than Id. please see comment above at class definition.
        /// </summary>
        public void Clear()
        {
            Highlighter.Clear();
        }

        /// <summary>
        /// Set Element
        /// Overlay highlighter needs element than Id. please see comment above at class definition.
        /// </summary>
        /// <param name="e"></param>
        /// <param name="brush"></param>
        /// <param name="onMouseDown"></param>
        /// <param name="gap"></param>
        public void SetElement(A11yElement e, SolidColorBrush brush, MouseButtonEventHandler onMouseDown = null, int gap = 2)
        {
            Highlighter.SetElement(e, brush, onMouseDown, gap);
        }

        /// <summary>
        /// Remove Element
        /// Overlay highlighter needs element than Id. please see comment above at class definition.
        /// </summary>
        /// <param name="e"></param>
        public void RemoveElement(A11yElement e)
        {
            Highlighter.RemoveElement(e);
        }

        /// <summary>
        /// Update Element
        /// Overlay highlighter needs element than Id. please see comment above at class definition.
        /// </summary>
        /// <param name="el"></param>
        public void UpdateElement(A11yElement el)
        {
            Highlighter.UpdateElement(el);
        }

        /// <summary>
        /// Get the element count
        /// </summary>
        /// <returns></returns>
        public int ElementCount => Highlighter.Items.Count;

        #region static members
        private static ClearOverlayDriver defaultHighlightOverlayAction;

#pragma warning disable CA1024 // This should not be a property
        /// <summary>
        /// Get default HighlightAction
        /// </summary>
        /// <returns></returns>
        public static ClearOverlayDriver GetDefaultInstance()
        {
            if (defaultHighlightOverlayAction == null)
            {
                defaultHighlightOverlayAction = new ClearOverlayDriver();
            }

            return defaultHighlightOverlayAction;
        }
#pragma warning disable CA1024 // This should not be a property

        /// <summary>
        /// Clear default Highlighter instance
        /// </summary>
        public static void ClearDefaultInstance()
        {
            defaultHighlightOverlayAction?.Dispose();
            defaultHighlightOverlayAction = null;
        }
        #endregion

        #region IDisposable Support
        private bool disposedValue; // To detect redundant calls

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    if (Highlighter != null)
                    {
                        Highlighter.Dispose();
                        Highlighter = null;
                    }
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
