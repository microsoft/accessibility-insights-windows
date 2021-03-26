// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using Axe.Windows.Actions;
using System;
using System.Windows.Input;

namespace AccessibilityInsights.SharedUx.Highlighting
{
    /// <summary>
    /// Wraps and provides access to highlight capabilities for
    /// elements over an image
    /// </summary>
    public class ImageOverlayDriver:IDisposable
    {
        /// <summary>
        /// imageHighlighter to use
        /// </summary>
        private ImageHighlighter Highlighter;

        /// <summary>
        /// Constructor
        /// </summary>
        private ImageOverlayDriver()
        {
            Highlighter = new ImageHighlighter(SetHighlightBtnState);
        }

        /// <summary>
        /// Show
        /// </summary>
        public void Show()
        {
            Highlighter.Show();
        }

        /// <summary>
        /// Hide
        /// </summary>
        public void Hide()
        {
            Highlighter.Hide();
        }

        /// <summary>
        /// Add Element in highlighter
        /// </summary>
        /// <param name="ecId">ElementContext Id</param>
        /// <param name="eId">Element Id</param>
        public void AddElement(Guid ecId, int eId)
        {
            var e = DataManager.GetDefaultInstance().GetA11yElement(ecId, eId);

            Highlighter.AddElement(e);
        }

        /// <summary>
        /// Set single element
        /// </summary>
        /// <param name="ecId"></param>
        /// <param name="e"></param>
        /// <param name="txt">text to show in top right corner</param>
        public void SetSingleElement(Guid ecId, int eId, string txt = null)
        {
            var e = GetDataAction.GetA11yElementInDataContext(ecId, eId);

            Highlighter.SetSingleElement(e, txt);
        }

        /// <summary>
        /// Clear highligted elements in bitmap.
        /// </summary>
        public void ClearElements()
        {
            Highlighter.ClearElements();
        }

        /// <summary>
        /// Set Background image with Element on highlighter
        /// it is to set bounding rectangle of background image based on screenshot element.
        /// </summary>
        /// <param name="ecId"></param>
        /// <param name="eId"></param>
        /// <param name="onClick"></param>
        public void SetImageElement(Guid ecId)
        {
            var e = DataManager.GetDefaultInstance().GetScreenshotElement(ecId);

            Highlighter.SetElement(e);
            Highlighter.SetBackground(DataManager.GetDefaultInstance().GetScreenshot(ecId));
        }

        /// <summary>
        /// set button click handler of Highlighter
        /// if it is null, button would not be shown in highlighter.
        /// </summary>
        /// <param name="onClick"></param>
        public void SetHighlighterButtonClickHandler(MouseButtonEventHandler onClick)
        {
            Highlighter.SetButtonClickHandler(onClick);
        }

        /// <summary>
        /// Remove Element
        /// </summary>
        /// <param name="ecId">ElementContext Id</param>
        /// <param name="eId">Element Id</param>
        public void RemoveElement(Guid ecId, int eId)
        {
            var e = DataManager.GetDefaultInstance().GetA11yElement(ecId, eId);
            Highlighter.RemoveElement(e);
        }

        /// <summary>
        /// Clear all elements in highlighter
        /// </summary>
        public void Clear()
        {
            Highlighter.Clear();
        }

        /// <summary>
        /// Get the visible state of highlighter
        /// </summary>
        /// <returns></returns>
        public bool IsVisible()
        {
            return Highlighter.IsVisible;
        }

        #region static members
        /// <summary>
        /// Action to set highlighter button state
        /// </summary>
        public static Action<bool> SetHighlightBtnState { get; set; }

        static ImageOverlayDriver defaultHighlightImageAction;

#pragma warning disable CA1024 // This should not be a property
        /// <summary>
        /// Get default HighlightAction
        /// </summary>
        /// <returns></returns>
        public static ImageOverlayDriver GetDefaultInstance()
        {
            if (defaultHighlightImageAction == null)
            {
                defaultHighlightImageAction = new ImageOverlayDriver();
            }

            return defaultHighlightImageAction;
        }
#pragma warning restore CA1024 // This should not be a property

        /// <summary>
        /// Clear default Highlighter instance
        /// </summary>
        public static void ClearDefaultInstance()
        {
            defaultHighlightImageAction?.Dispose();
            defaultHighlightImageAction = null;
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
                    if (this.Highlighter != null)
                    {
                        this.Highlighter.Dispose();
                        this.Highlighter = null;
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
