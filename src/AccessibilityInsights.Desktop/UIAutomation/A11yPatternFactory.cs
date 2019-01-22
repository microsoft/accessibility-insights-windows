// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using AccessibilityInsights.Core.Bases;
using AccessibilityInsights.Core.Types;
using AccessibilityInsights.Desktop.Telemetry;
using AccessibilityInsights.Desktop.UIAutomation.Patterns;
using System;
using UIAutomationClient;

namespace AccessibilityInsights.Desktop.UIAutomation
{
    public static class A11yPatternFactory
    {
        /// <summary>
        /// Get the Instance of specific pattern. 
        /// if there is no matching pattern, it is wrapped up as Unknown. 
        /// </summary>
        /// <param name="e"></param>
        /// <param name="id"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        public static A11yPattern GetPatternInstance(A11yElement e, IUIAutomationElement uia, int id, string name)
        {
            try
            {
                dynamic pt = null;

                if (PatternType.GetInstance().Exists(id))
                {
                    pt = uia.GetCachedPattern(id);
                }

                if (pt != null)
                {
                    switch (id)
                    {
                        case PatternType.UIA_AnnotationPatternId:
                            return new AnnotationPattern(e, pt);
                        case PatternType.UIA_CustomNavigationPatternId:
                            return new CustomNavigationPattern(e, pt);
                        case PatternType.UIA_DockPatternId:
                            return new DockPattern(e, pt);
                        case PatternType.UIA_DragPatternId:
                            return new DragPattern(e, pt);
                        case PatternType.UIA_DropTargetPatternId:
                            return new DropTargetPattern(e, pt);
                        case PatternType.UIA_ExpandCollapsePatternId:
                            return new ExpandCollapsePattern(e, pt);
                        case PatternType.UIA_GridItemPatternId:
                            return new GridItemPattern(e, pt);
                        case PatternType.UIA_GridPatternId:
                            return new GridPattern(e, pt);
                        case PatternType.UIA_InvokePatternId:
                            return new InvokePattern(e, pt);
                        case PatternType.UIA_ItemContainerPatternId:
                            return new ItemContainerPattern(e, pt);
                        case PatternType.UIA_LegacyIAccessiblePatternId:
                            return new LegacyIAccessiblePattern(e, pt);
                        case PatternType.UIA_MultipleViewPatternId:
                            return new MultipleViewPattern(e, pt);
                        case PatternType.UIA_ObjectModelPatternId:
                            return new ObjectModelPattern(e, pt);
                        case PatternType.UIA_RangeValuePatternId:
                            return new RangeValuePattern(e, pt);
                        case PatternType.UIA_ScrollItemPatternId:
                            return new ScrollItemPattern(e, pt);
                        case PatternType.UIA_ScrollPatternId:
                            return new ScrollPattern(e, pt);
                        case PatternType.UIA_SelectionItemPatternId:
                            return new SelectionItemPattern(e, pt);
                        case PatternType.UIA_SelectionPatternId:
                            return new SelectionPattern(e, pt);
                        case PatternType.UIA_SelectionPattern2Id:
                            return new SelectionPattern2(e, pt);
                        case PatternType.UIA_SpreadsheetPatternId:
                            return new SpreadsheetPattern(e, pt);
                        case PatternType.UIA_SpreadsheetItemPatternId:
                            return new SpreadsheetItemPattern(e, pt);
                        case PatternType.UIA_StylesPatternId:
                            return new StylesPattern(e, pt);
                        case PatternType.UIA_SynchronizedInputPatternId:
                            return new SynchronizedInputPattern(e, pt);
                        case PatternType.UIA_TableItemPatternId:
                            return new TableItemPattern(e, pt);
                        case PatternType.UIA_TablePatternId:
                            return new TablePattern(e, pt);
                        case PatternType.UIA_TextChildPatternId:
                            return new TextChildPattern(e, pt);
                        case PatternType.UIA_TextEditPatternId:
                            return new TextEditPattern(e, pt);
                        case PatternType.UIA_TextPatternId:
                            return new TextPattern(e, pt);
                        case PatternType.UIA_TextPattern2Id:
                            return new TextPattern2(e, pt);
                        case PatternType.UIA_TogglePatternId:
                            return new TogglePattern(e, pt);
                        case PatternType.UIA_TransformPatternId:
                            return new TransformPattern(e, pt);
                        case PatternType.UIA_TransformPattern2Id:
                            return new TransformPattern2(e, pt);
                        case PatternType.UIA_ValuePatternId:
                            return new ValuePattern(e, pt);
                        case PatternType.UIA_VirtualizedItemPatternId:
                            return new VirtualizedItemPattern(e, pt);
                        case PatternType.UIA_WindowPatternId:
                            return new WindowPattern(e, pt);
                    }
                }
                return new UnKnownPattern(e, id, name);
            }
            catch(Exception ex)
            {
                ex.ReportException();

                return null;
            }
        }
    }
}
