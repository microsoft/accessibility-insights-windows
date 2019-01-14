// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;

namespace AccessibilityInsights.Core.Fingerprint
{
    /// <summary>
    /// Abstraction for the information needed to open a specific issue location.
    /// </summary>
    public interface ILocation : IEquatable<ILocation>
    {
        /// <summary>
        /// The description to show in the UI
        /// </summary>
        string UserDisplayInfo { get; }

        /// <summary>
        /// The source of the location -- format and content will vary with the type of location
        /// </summary>
        string Source { get; }

        /// <summary>
        /// The Id within the source -- format and content will vary with the type of location
        /// </summary>
        string Id { get; }

        /// <summary>
        /// Open the location
        /// </summary>
        /// <returns>true iff the location was successfully opened</returns>
        bool Open();
    }
}
