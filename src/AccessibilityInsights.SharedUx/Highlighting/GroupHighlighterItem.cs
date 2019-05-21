// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System.Windows;
using System.Windows.Controls;

namespace AccessibilityInsights.SharedUx.Highlighting
{
    /// <summary>
    /// Item containing elements for group highlighter
    /// </summary>
    public class GroupHighlighterItem
    {
        /// <summary>
        /// Rectangle in corner
        /// </summary>
        public TextBlock TbError { get; set; }

        /// <summary>
        /// Highlighter outline
        /// </summary>
        public Border BrdrError { get; set; }

        /// <summary>
        /// Number of times this element has been highlighted
        /// </summary>
        int count = 1;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="brdr"></param>
        /// <param name="tb"></param>
        public GroupHighlighterItem(Border brdr, TextBlock tb)
        {
            TbError = tb;
            BrdrError = brdr;
        }

        /// <summary>
        /// Increment count
        /// </summary>
        public void AddRef()
        {
            count++;
        }

        /// <summary>
        /// Decrement count; hide visuals if 0
        /// </summary>
        /// <returns></returns>
        public bool Release()
        {
            count--;
            if (count == 0)
            {
                if (TbError != null)
                {
                    TbError.Visibility = Visibility.Hidden;
                }
                BrdrError.Visibility = Visibility.Hidden;
                return true;
            }
            return false;
        }

        /// <summary>
        /// Hide visuals
        /// </summary>
        public void Hide()
        {
            if (TbError != null)
            {
                TbError.Visibility = Visibility.Hidden;
            }
            BrdrError.Visibility = Visibility.Hidden;
        }

        /// <summary>
        /// Show visuals
        /// </summary>
        public void Show()
        {
            if (TbError != null)
            {
                TbError.Visibility = Visibility.Visible;
            }
            BrdrError.Visibility = Visibility.Visible;
        }

        /// <summary>
        /// Clear elements
        /// </summary>
        public void Clear()
        {
            BrdrError.BorderBrush = null;
            BrdrError = null;
            if (TbError != null)
            {
                TbError.Background = null;
                TbError.Foreground = null;
                TbError = null;
            }
        }
    }
}
