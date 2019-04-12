// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using Axe.Windows.Core.Enums;
using Axe.Windows.Core.Types;
using Axe.Windows.Rules.Resources;

namespace Axe.Windows.Rules.PropertyConditions
{
    static class IntProperties
    {
        public static EnumProperty<OrientationType> Orientation = new EnumProperty<OrientationType>(PropertyType.UIA_OrientationPropertyId);
        public static IntProperty HeadingLevel = new IntProperty(PropertyType.UIA_HeadingLevelPropertyId);
        public static IntProperty PositionInSet = new IntProperty(PropertyType.UIA_PositionInSetPropertyId);
        public static IntProperty SizeOfSet = new IntProperty(PropertyType.UIA_SizeOfSetPropertyId);
        public static IntProperty NativeWindowHandle = new IntProperty(PropertyType.UIA_NativeWindowHandlePropertyId, ConditionDescriptions.NativeWindowHandle);
    } // class
} // namespace
