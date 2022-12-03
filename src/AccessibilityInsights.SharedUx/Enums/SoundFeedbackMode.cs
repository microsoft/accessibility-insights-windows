// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace AccessibilityInsights.SharedUx.Enums
{
    /// <summary>
    /// Available user-selectable choices for the "provide feedback with sound" config setting.
    /// </summary>
    public enum SoundFeedbackMode
    {
        Auto, // Enable when the system screen reader flag is set, disable otherwise
        Always,
        Never
    }
}
