// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using static Axe.Windows.Rules.PropertyConditions.Relationships;
using static Axe.Windows.Rules.PropertyConditions.StringProperties;

namespace Axe.Windows.Rules.PropertyConditions
{
    static class UWP
    {
        public static Condition TopLevelElement = StringProperties.Framework.Is(Core.Enums.Framework.XAML) & NotParent(StringProperties.Framework.Is(Core.Enums.Framework.XAML));
        public static Condition TitleBar = CreateTitleBarCondition();
        public static Condition MenuBar = CreateMenuBarCondition();

        private static Condition CreateTitleBarCondition()
        {
            var automationID = AutomationID.Is("TitleBar") | AutomationID.Is("TitleBarLeftButtons");
            var className = ClassName.Is("ApplicationFrameTitleBarWindow");
            var framework = StringProperties.Framework.Is(Core.Enums.Framework.Win32);
            return automationID & className & framework;
        }

        private static Condition CreateMenuBarCondition()
        {
            var automationID = AutomationID.Is("SystemMenuBar");
            var parentFramework = Relationships.Parent(StringProperties.Framework.Is(Core.Enums.Framework.Win32));
            return automationID & parentFramework;
        }
    } // class
} // namespace
