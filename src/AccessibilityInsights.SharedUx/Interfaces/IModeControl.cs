// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;
using System.Threading.Tasks;

namespace AccessibilityInsights.SharedUx.Interfaces
{
    /// <summary>
    /// Interface for main window mode controls
    /// </summary>
    public interface IModeControl
    {
        /// <summary>
        /// Hides control
        /// </summary>
        void HideControl();

        /// <summary>
        /// Shows control
        /// </summary>
        void ShowControl();

        /// <summary>
        /// Sets element context for control
        /// </summary>
        /// <param name="ecId">ElementContext Id</param>
        Task SetElement(Guid ecId);

        /// <summary>
        /// Clear all UI controls
        /// </summary>
        void Clear();

        /// <summary>
        /// Saves layout info to config
        /// </summary>
        void UpdateConfigWithSize();

        /// <summary>
        /// Updates layout based on config
        /// </summary>
        void AdjustMainWindowSize();

        /// <summary>
        /// Copy appropriate text to clipboard
        /// </summary>
        void CopyToClipboard();

        /// <summary>
        /// Allow Refresh Button
        /// </summary>
        bool IsRefreshEnabled { get; }

        /// <summary>
        /// Allow Save Button
        /// </summary>
        bool IsSaveEnabled { get; }

        /// <summary>
        /// Method to handle Refresh request
        /// </summary>
        void Refresh();

        /// <summary>
        /// Method to handle Save request
        /// </summary>
        void Save();

        /// <summary>
        /// Method to handle highlighter toggle request
        /// </summary>
        /// <returns></returns>
        bool ToggleHighlighter();

        /// <summary>
        /// Set focus to a default element
        /// </summary>
        void SetFocusOnDefaultControl();
    }
}
