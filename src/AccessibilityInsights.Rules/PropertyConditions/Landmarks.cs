// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using AccessibilityInsights.Core.Types;
using AccessibilityInsights.Rules.Resources;
using static AccessibilityInsights.Rules.PropertyConditions.StringProperties;

namespace AccessibilityInsights.Rules.PropertyConditions
{
    class Landmarks
    {
        public static Condition Custom = Condition.Create(e => e.LandmarkType == LandmarkType.UIA_CustomLandmarkTypeId)[ConditionDescriptions.CustomLandmark];
        public static Condition Application = Custom & LocalizedLandmarkType.IsNoCase("application");
        public static Condition Banner = Custom & LocalizedLandmarkType.IsNoCase("banner");
        public static Condition Complementary = Custom & LocalizedControlType.IsNoCase("complementary");
        public static Condition ContentInfo = Custom & LocalizedControlType.IsNoCase("contentinfo");
        public static Condition Form = Condition.Create(e => e.LandmarkType == LandmarkType.UIA_FormLandmarkTypeId)[ConditionDescriptions.FormLandmark];
        public static Condition Main = Condition.Create(e => e.LandmarkType == LandmarkType.UIA_MainLandmarkTypeId)[ConditionDescriptions.MainLandmark];
        public static Condition Navigation = Condition.Create(e => e.LandmarkType == LandmarkType.UIA_NavigationLandmarkTypeId)[ConditionDescriptions.NavigationLandmark];
        public static Condition Search = Condition.Create(e => e.LandmarkType == LandmarkType.UIA_SearchLandmarkTypeId)[ConditionDescriptions.SearchLandmark];
        public static Condition AnyUIA = (Custom | Form | Main | Navigation | Search)[ConditionDescriptions.Parentheses];
        public static Condition Any = (Application | Banner | Complementary | ContentInfo | Custom | Form | Main | Navigation | Search)[ConditionDescriptions.Parentheses];
    } // class
} // namespace
