// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using AccessibilityInsights.Actions.Attributes;
using AccessibilityInsights.Actions.Enums;
using System;

namespace AccessibilityInsights.Actions
{
    /// <summary>
    /// SetDataAction class
    /// Set/Release Data in Data Manager
    /// </summary>
    [InteractionLevel(UxInteractionLevel.NoUxInteraction)]
    public static class SetDataAction
    {
        /// <summary>
        /// Release ElementContext
        /// </summary>
        /// <param name="ecId">ElementContext Id</param>
        public static void ReleaseElementContext(Guid ecId)
        {
            DataManager.GetDefaultInstance().RemoveDataContext(ecId);
        }

        /// <summary>
        /// Release all data in Action Data Manager
        /// </summary>
        public static void ReleaseAll()
        {
            DataManager.Clear();
        }

        /// <summary>
        /// Release DataContext of an ElementContext.
        /// you may keep main element if the purpose of ReleaseDataContext is for refreshing data. 
        /// </summary>
        /// <param name="ecId">ElementContext Id</param>
        public static void ReleaseDataContext(Guid ecId)
        {
            DataManager.GetDefaultInstance().RemoveDataContext(ecId);
        }
    }
}
