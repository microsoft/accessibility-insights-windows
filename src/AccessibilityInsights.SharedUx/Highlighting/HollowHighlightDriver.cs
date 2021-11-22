// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using AccessibilityInsights.SharedUx.Enums;
using Axe.Windows.Actions;
using Axe.Windows.Core.Bases;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;

namespace AccessibilityInsights.SharedUx.Highlighting
{
    /// <summary>
    /// Wraps and provides access to highlight capabilities that are 'hollow', e.g.
    /// hovering over the resulting rectangle still allows clicking through to the
    /// underlying element
    /// </summary>
    public class HollowHighlightDriver:IDisposable
    {
        readonly Highlighter Highlighter;

        /// <summary>
        /// information of current element
        /// </summary>
        Rectangle? BoundingRectangle;

        #region public members
        /// <summary>
        /// Hilight an element with selected hilighter
        /// </summary>
        /// <param name="ecId">ElementContext Id</param>
        /// <param name="eId"></param>
        public void SetElement(Guid ecId, int eId)
        {
            SetElementInternal(GetDataAction.GetA11yElementInDataContext(ecId, eId));
        }

        /// <summary>
        /// public interface for the highlighter mode
        /// </summary>
        public HighlighterMode HighlighterMode
        {
            get
            {
                return this.Highlighter.HighlighterMode;
            }

            set
            {
                this.Highlighter.HighlighterMode = value;
            }
        }

        bool isEnabled = true;
        /// <summary>
        /// Does the user want to see the highlighter
        /// </summary>
        public bool IsEnabled
        {
            get
            {
                return isEnabled;
            }

            set
            {
                this.Highlighter.IsVisible = isEnabled = value;
            }
        }

        /// <summary>
        /// SetElement with A11yElement
        /// this one should be used only for Event handling case
        /// </summary>
        /// <param name="el"></param>
        public void SetElement(A11yElement el)
        {
#pragma warning disable CA1062 // Validate arguments of public methods
            SetElementInternal(el);
#pragma warning restore CA1062 // Validate arguments of public methods
        }

        /// <summary>
        /// SetElementInternal
        /// </summary>
        /// <param name="element"></param>
        void SetElementInternal(A11yElement element)
        {
            bool visible = element != null && !element.BoundingRectangle.IsEmpty;
            if (visible)
            {
                if (this.BoundingRectangle == null || element.BoundingRectangle.Equals(this.BoundingRectangle) == false)
                {
                    this.Highlighter.SetLocation(element.BoundingRectangle);
                    this.BoundingRectangle = element.BoundingRectangle;
                }

                if (this.IsEnabled)
                {
                    var text = GetHighlightText(element);
                    this.Highlighter.SetText(text);
                }
            }

            this.Highlighter.IsVisible = this.IsEnabled && visible;
        }

        /// <summary>
        /// Set the text value
        /// </summary>
        /// <param name="text"></param>
        public void SetText(string text)
        {
            this.Highlighter.SetText(text);
        }

        /// <summary>
        /// Set call back for snapshot
        /// </summary>
        /// <param name="callback"></param>
        public void SetCallBackForSnapshot(Action callback)
        {
            this.Highlighter.SetCallBackForSnapshot(callback);
        }

        /// <summary>
        /// Remove exising highlighter information
        /// it will turn off highlighter
        /// </summary>
        public void Clear()
        {
            this.Highlighter.IsVisible = false;
            this.Highlighter.SetLocation(Rectangle.Empty);
            this.BoundingRectangle = null;
        }
        #endregion

        #region Private members
        /// <summary>
        /// private constructor
        /// </summary>
        /// <param name="hasSnapshotButton"></param>
        private HollowHighlightDriver(bool hasSnapshot)
        {
            this.Highlighter = new Highlighter(HighlighterColor.DefaultBrush, hasSnapshot);
        }

        /// <summary>
        /// Get highlighter text
        /// </summary>
        /// <param name="element"></param>
        /// <returns></returns>
        private static string GetHighlightText(A11yElement element)
        {
            StringBuilder sb = new StringBuilder();

            sb.AppendLine(element.Glimpse);
            sb.AppendLine(element.IsKeyboardFocusable ?
                Properties.Resources.Highlight_Focusable : Properties.Resources.Highlight_NotFocusable);

            return sb.ToString();
        }

        #endregion

        #region static methods
        static Dictionary<string, HollowHighlightDriver> sHighlightActions = new Dictionary<string, HollowHighlightDriver>();

        /// <summary>
        /// Get default HighlightAction
        /// </summary>
        /// <returns></returns>
        public static HollowHighlightDriver GetDefaultInstance()
        {
            return GetInstance(HighlighterType.Default);
        }

        /// <summary>
        /// Get Instance of indicated highlighter type
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static HollowHighlightDriver GetInstance(HighlighterType type)
        {
            return GetInstance(type.ToString());
        }

        /// <summary>
        /// Get named Highlighter
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public static HollowHighlightDriver GetInstance(string name)
        {
            HollowHighlightDriver ha = null;

            if(sHighlightActions.ContainsKey(name))
            {
                ha = sHighlightActions[name];
            }
            else
            {
                ha = new HollowHighlightDriver(name == HighlighterType.Default.ToString())
                {
                    HighlighterMode = HighlighterMode.Highlighter,
                };
                sHighlightActions.Add(name, ha);
            }

            return ha;
        }

        /// <summary>
        /// Clear all Highlighter instances
        /// </summary>
        public static void ClearAllHighlighters()
        {
            if(sHighlightActions != null && sHighlightActions.Count != 0)
            {
                foreach(var ha in sHighlightActions.Values)
                {
                    ha.Dispose();
                }

                sHighlightActions.Clear();
            }
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
                        this.Highlighter.IsVisible = false;
                        this.Highlighter.Dispose();
                        this.BoundingRectangle = null;
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
