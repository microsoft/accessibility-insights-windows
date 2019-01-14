// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;
using System.Collections.Generic;
using System.Drawing;
using AccessibilityInsights.Core.Enums;

namespace AccessibilityInsights.Core.Bases
{
    /// <summary>
    /// Light weight abstraction of A11y elements
    /// used for validating rules
    /// </summary>
    public interface IA11yElement : IDisposable
    {
        string Name { get; }
        int ControlTypeId { get; }
        string LocalizedControlType { get; }
        string AutomationId { get; }
        Rectangle BoundingRectangle { get; }
        string ClassName { get; }
        string HelpText{ get; }
        bool IsEnabled { get; }
        bool IsKeyboardFocusable { get; }
        bool IsContentElement { get; }
        bool IsControlElement { get; }
        bool IsOffScreen { get; }
        string ItemStatus { get; }
        OrientationType Orientation { get; }
        int LandmarkType { get; }
        string LocalizedLandmarkType { get; }
        int HeadingLevel { get; }
        string RuntimeId { get; }
        int ProcessId { get; }
        string Framework { get; }
        bool TransformPatternCanResize { get; }
        string ProcessName { get;  }

        IA11yElement Parent { get; }
        IEnumerable<IA11yElement> Children { get; }
        IA11yElement GetFirstChild();

        bool TryGetPropertyValue<T>(int propertyId, out T value);
        IA11yPattern GetPattern(int patternId);
        T GetPlatformPropertyValue<T>(int propertyId);
    } // class
} // namespace
