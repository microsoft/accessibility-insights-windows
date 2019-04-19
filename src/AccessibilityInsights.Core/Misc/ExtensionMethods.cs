// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using Axe.Windows.Core.Attributes;
using Axe.Windows.Core.Bases;
using Axe.Windows.Core.Enums;
using Axe.Windows.Core.Results;
using Axe.Windows.Core.Types;
using Axe.Windows.Telemetry;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Text;

using static System.FormattableString;

namespace Axe.Windows.Core.Misc
{
    /// <summary>
    /// Extension method class
    /// </summary>
    public static class ExtensionMethods
    {
        /// <summary>
        /// DesktopElement runtime Id. 
        /// </summary>
        const string DesktopElementRuntimeId = "[2A,10010]";
        /// <summary>
        /// WCOS runtime ID
        /// </summary>
        const string WCOSElementRuntimeId = "[0,0]";

        /// <summary>
        /// Get A11yElementData object out of A11yElement
        /// </summary>
        /// <param name="e"></param>
        /// <returns></returns>
        public static A11yElementData GetA11yElementData(this A11yElement e)
        {
            return new A11yElementData() { Patterns = e.Patterns, Properties = e.Properties };
        }

        /// <summary>
        /// Check whether element is control or Content
        /// </summary>
        /// <param name="e"></param>
        /// <returns></returns>
        public static bool IsControlOrContent(this A11yElement e)
        {
            return e.IsContentElement || e.IsControlElement;
        }

        /// <summary>
        /// Obtains the ClassName of the element
        /// </summary>
        /// <param name="e">A11yElement</param>
        /// <returns>UIA_ClassNamePropertyId</returns>
        public static string GetClassName(this A11yElement e)
        {
            if (e.Properties != null && e.Properties.ContainsKey(PropertyType.UIA_ClassNamePropertyId))
            {
                return e.Properties[PropertyType.UIA_ClassNamePropertyId].TextValue;
            }
            else
                return null;
        }

        /// <summary>
        /// Check whether Pattern is actionable or not based on PatternMethod
        /// </summary>
        /// <param name="ptn"></param>
        /// <returns></returns>
        public static bool IsUIActionablePatternByPatternMethodType(this A11yPattern ptn)
        {
            return (from m in ptn.GetType().GetMethods()
                    let a = m.GetCustomAttribute(typeof(PatternMethodAttribute))
                    where a != null && (bool)a.GetType().GetProperty("IsUIAction").GetValue(a) == true
                    select a).Count() != 0;
        }

        /// <summary>
        /// convert int array to string in Hex value. 
        /// </summary>
        /// <param name="array"></param>
        /// <returns></returns>
        public static string ConvertIntArrayToString(Int32[] array)
        {
            StringBuilder sb = new StringBuilder();

            sb.Append("[");

            if (array != null && array.Length > 0)
            {
                sb.Append(Invariant($"{array.GetValue(0):X}"));
                for (int i = 1; i < array.Length; i++)
                {
                    sb.Append(Invariant($",{array.GetValue(i):X}"));
                }
            }

            sb.Append("]");

            return sb.ToString();
        }

        /// <summary>
        /// To handle deserialized value from json file. 
        /// Need to figure out a way to remove this duplicated code.
        /// </summary>
        /// <param name="array"></param>
        /// <returns></returns>
        public static string ConvertIntArrayToString(Newtonsoft.Json.Linq.JArray array)
        {
            StringBuilder sb = new StringBuilder();

            sb.Append("[");

            if (array != null && array.Count > 0)
            {
                sb.Append(Invariant($"{array[0]:X}"));
                for (int i = 1; i < array.Count; i++)
                {
                    sb.Append(Invariant($",{array[i]:X}"));
                }
            }

            sb.Append("]");

            return sb.ToString();
        }

        public static string ConvertDoubleArrayToString(Double[] array)
        {
            if (array != null && array.Length != 0)
            {
                StringBuilder sb = new StringBuilder();

                sb.Append("[");

                if (array.Length > 0)
                {
                    sb.Append(Invariant($"{array.GetValue(0)}"));
                    for (int i = 1; i < array.Length; i++)
                    {
                        sb.Append(Invariant($",{array.GetValue(i)}"));
                    }
                }

                sb.Append("]");

                return sb.ToString();
            }
            return null;
        }

        public static string ConvertDoubleArrayToString(this Array array)
        {
            return ConvertDoubleArrayToString((Double[])array);
        }

        public static string ConvertInt32ArrayToString(Int32[] array)
        {
            if (array != null && array.Length != 0)
            {
                StringBuilder sb = new StringBuilder();

                sb.Append("[");

                if (array.Length > 0)
                {
                    sb.Append(Invariant($"{array.GetValue(0)}"));
                    for (int i = 1; i < array.Length; i++)
                    {
                        sb.Append(Invariant($",{array.GetValue(i)}"));
                    }
                }

                sb.Append("]");

                return sb.ToString();
            }

            return null;
        }

        public static string ConvertInt32ArrayToString(this Array array)
        {
            return ConvertInt32ArrayToString((Int32[])array);
        }

        /// <summary>
        /// Check whether UIElement is same or not. 
        /// </summary>
        /// <param name="element1"></param>
        /// <param name="element2"></param>
        /// <returns></returns>
        public static bool IsSameUIElement(this A11yElement element1, A11yElement element2)
        {
            if (element1 != null && element2 != null)
            {
                return element1.IsSameUIElement(element2.RuntimeId, element2.BoundingRectangle, element2.ControlTypeId, element2.Name);
            }

            return false;
        }

        /// <summary>
        /// Check whether UIElement is same or not 
        /// based on runtime Id, name, controltype, and BoundingRectangle
        /// </summary>
        /// <param name="element1"></param>
        /// <param name="runtimeId"></param>
        /// <param name="rect"></param>
        /// <param name="controltype">Control type Id</param>
        /// <param name="name">Name of control</param>
        /// <returns></returns>
        public static bool IsSameUIElement(this A11yElement element1, string runtimeId, Rectangle? rect, int controltype, string name)
        {
            if (element1 != null)
            {
                if (string.IsNullOrEmpty(element1.RuntimeId) == false
                    || string.IsNullOrEmpty(runtimeId) == false)
                {
                    return element1.RuntimeId == runtimeId;
                }
                else if(element1.Name == name)
                {
                    if (element1.ControlTypeId == controltype)
                    {
                        // if control type is same check, bounding rectangle for sure. 
                        if (!element1.BoundingRectangle.IsEmpty
                            && rect != null
                            && !rect.Value.IsEmpty)
                        {
                            var r = element1.BoundingRectangle;
                            var l = rect.Value;
                            return l.Left == r.Left && l.Top == r.Top && l.Right == l.Right && l.Bottom == r.Bottom;
                        }
                        else
                        {
                            // if both has no bounding rectangle, consider them as same element. 
                            return element1.BoundingRectangle.IsEmpty
                                && (rect == null
                                || rect.Value.IsEmpty);
                        }
                    }
                }
            }

            return false;
        }

        /// <summary>
        /// Get the POI element with test results from this element and descendents
        /// POI element Unique ID should be 0.
        /// </summary>
        /// <param name="element">The root of the element tree</param>
        /// <returns>The POI if it exists</returns>
        /// <exception cref="ArgumentException"> is thrown if no POI can be located in the element tree</exception>
        public static A11yElement FindPOIElementFromLoadedData(this A11yElement element)
        {
            if (element.TryRecursivelyFindPOI(out A11yElement poi))
            {
                return poi;
            }

            throw new ArgumentException("Unable to locate the target element in the file.");
        }

        private static bool TryRecursivelyFindPOI(this A11yElement element, out A11yElement poi)
        {
            if (element != null)
            {
                if (element.UniqueId == 0)
                {
                    poi = element;
                    return true;
                }

                if (element.Children != null)
                {
                    foreach (A11yElement child in element.Children)
                    {
                        if (child.TryRecursivelyFindPOI(out poi))
                        {
                            return true;
                        }
                    }
                }
            }

            poi = null;
            return false;
        }

        /// <summary>
        /// Returns a list of A11yElements along the path from the origin to this element
        /// </summary>
        /// <param name="element">Element to find the path to</param>
        /// <param name="descending">Indicates whether the path goes from top to bottom</param>
        /// <returns></returns>
        public static List<A11yElement> GetPathFromOriginAncestor(this A11yElement element, bool descending = true)
        {
            List<A11yElement> res = new List<A11yElement>() { element };
            var e = element;
            while (e != null && e.Parent != null)
            {
                e = e.Parent;
                res.Add(e);
            }
            if (descending)
            {
                res.Reverse();
            }
            return res;
        }

        /// <summary>
        /// Gets the Top level Ancestor from Ancestry.
        ///     If "controlType" is passed in, then the return type is the
        ///     first ancestor of the specified control type. Usually used to find app window
        /// </summary>
        /// <param name="element"></param>
        /// <returns></returns>
        public static A11yElement GetOriginAncestor(this A11yElement element, int? controlType = null)
        {
            var e = element;
            while (e != null && e.Parent != null && (controlType == null || controlType != e.ControlTypeId))
            {
                e = e.Parent;
            }

            return e;
        }

        public static string GetSafeSenderStringValue(GetStringValue getString)
        {
            string txt = null;
            try
            {
                txt = getString();
            }
            catch (Exception e)
            {
                e.ReportException();
                txt = "";
            }

            return txt;
        }

        /// <summary>
        /// Convert Element Value to formated string
        /// </summary>
        /// <param name="p"></param>
        /// <returns></returns>
        public static string ConvertIntArrayToString(this A11yProperty p)
        {
            return p.Value != null ? ConvertIntArrayToString(p.Value) : null;
        }

        /// <summary>
        /// Gets the counts of scan result statuses from given parameter
        /// </summary>
        /// <param name="scanStatuses"></param>
        /// <returns>counts where result-array[scan-status] is the count of ScanStatus 
        ///          elements with scan-status
        /// </returns>
        public static int[] GetStatusCounts(this IEnumerable<ScanStatus> scanStatuses)
        {
            int numStatusTypes = Enum.GetNames(typeof(ScanStatus)).Length;
            int[] results = new int[numStatusTypes];
            foreach (var status in scanStatuses)
            {
                results[(int) status]++;
            }
            return results;
        }

        /// <summary>
        /// Get the aggregated test status.
        /// </summary>
        /// <param name="tss"></param>
        /// <returns></returns>
        public static ScanStatus GetAggregatedScanStatus(this IEnumerable<ScanStatus> tss)
        {
            if (tss.Count() != 0)
            {
                if(HasTestResults(tss, ScanStatus.ScanNotSupported))
                {
                    return ScanStatus.ScanNotSupported;
                }
                else if (HasTestResults(tss, ScanStatus.Fail))
                {
                    return ScanStatus.Fail;
                }
                else if (HasTestResults(tss, ScanStatus.Uncertain))
                {
                    return ScanStatus.Uncertain;
                }
                else if (HasTestResults(tss, ScanStatus.NoResult))
                {
                    // it means that there was no rule executed or 
                    if (System.Diagnostics.Debugger.IsAttached)
                    {
                         System.Diagnostics.Debugger.Break();
                    }
                }

                return ScanStatus.Pass;
            }

            return ScanStatus.NoResult;
        }

        private static bool HasTestResults(IEnumerable<ScanStatus> tss, ScanStatus ets)
        {
            return tss.Contains(ets);
        }

        /// <summary>
        /// Check whether a indicated pattern ID exists in pattern list
        /// </summary>
        /// <param name="ps"></param>
        /// <returns></returns>
        public static bool HasPatternBy(this List<A11yPattern> ps, int id)
        {
            if (ps == null || ps.Count == 0) return false;

            return (from p in ps
                    where p.Id == id
                    select p).Count() != 0;
        }

        /// <summary>
        /// Check whether a indicated control type ID exists in child element list.
        /// </summary>
        /// <param name="cs"></param>
        /// <returns></returns>
        public static bool HasChildBy(this List<A11yElement> cs, int id)
        {
            return (from c in cs
                    let cid = c.Properties.ById(PropertyType.UIA_ControlTypePropertyId).Value
                    where c.ControlTypeId == id
                    select c).Count() != 0;
        }

        /// <summary>
        /// find matching Pattern by Id
        /// </summary>
        /// <param name="ps"></param>
        /// <returns></returns>
        public static A11yPattern ById(this List<A11yPattern> ps, int id)
        {
            if (ps == null || ps.Count == 0) return null;

            return (from p in ps
                    where p.Id == id
                    select p).FirstOrDefault();
        }


        /// <summary>
        /// find matching Property by Id
        /// </summary>
        /// <param name="ps"></param>
        /// <returns></returns>
        public static A11yProperty ById(this Dictionary<int, A11yProperty> ps, int id)
        {
            if (ps != null && ps.ContainsKey(id))
            {
                return ps[id];
            }

            return null;
        }

        public static IA11yPattern ById(this IEnumerable<IA11yPattern> patterns, int id)
        {
            if (patterns == null) return null;

            return patterns.FirstOrDefault(p => p.Id == id);
        }

        /// <summary>
        /// find matching PatternProperty by Name
        /// </summary>
        /// <param name="ps"></param>
        /// <returns></returns>
        public static A11yPatternProperty ByName(this List<A11yPatternProperty> ps, string name)
        {
            return (from p in ps
                    where p.Name == name
                    select p).FirstOrDefault();
        }

        /// <summary>
        /// Count the number of element with the expected control type
        /// </summary>
        /// <param name="es"></param>
        /// <returns></returns>
        public static int CountMatchedByControlType(this List<A11yElement> es, int id)
        {
            return (from e in es
                    where e.ControlTypeId == id
                    select e).Count();
        }

        /// <summary>
        /// Get all children with the specified control type Id
        /// </summary>
        /// <param name="es"></param>
        /// <returns></returns>
        public static List<A11yElement> ByControlType(this List<A11yElement> es, int id)
        {
            return (from e in es
                    where e.ControlTypeId == id
                    select e).ToList();
        }

        /// <summary>
        /// Count the number of element which are not matched in the given list
        /// </summary>
        /// <param name="es"></param>
        /// <returns></returns>
        public static int CountUnMatchedByControlTypes(this List<A11yElement> es, int[] ids)
        {
            return (from e in es
                    where ids.Contains(e.ControlTypeId) == false
                    select e).Count();
        }

        /// <summary>
        /// Convert Element Property value to Rectangle
        /// </summary>
        /// <param name="ep"></param>
        /// <returns></returns>
        public static Rectangle ToRectangle(this A11yProperty property)
        {
            if (property == null) return Rectangle.Empty;

            switch (property.Value)
            {
                case double[] rectangle:
                    {
                        if (rectangle.Length != 4)
                            return Rectangle.Empty;

                        return new Rectangle(
                x: (int)rectangle[0],
                y: (int)rectangle[1],
                width: rectangle[2] > 0 ? (int)rectangle[2] : 0,
                height: rectangle[3] > 0 ? (int)rectangle[3] : 0);
                    }
                case Newtonsoft.Json.Linq.JArray jArray:
                    {
                        if (jArray.Count != 4)
                            return Rectangle.Empty;

                        foreach (var token in jArray)
                        {
                            if (token.Type != Newtonsoft.Json.Linq.JTokenType.Float)
                                return Rectangle.Empty;
                        }

                        int width = jArray[2].ToObject<int>();
                        int height = jArray[3].ToObject<int>();

                        return new Rectangle(
                            x: jArray[0].ToObject<int>(),
                            y: jArray[1].ToObject<int>(),
                            width: width > 0 ? width : 0,
                            height: height > 0 ? height : 0);
                    }
            } // switch

            return Rectangle.Empty;
        }

        public static T[] ToArray<T>(this Newtonsoft.Json.Linq.JArray j)
        {
            if (j == null) return null;

            var retVal = new T[j.Count];

            return retVal;
        }

        public static string ToLeftTopRightBottomString(this Rectangle r)
        {
            return Invariant($"[l={r.Left},t={r.Top},r={r.Right},b={r.Bottom}]");
        }

        /// <summary>
        /// Check whether the UI element is offscreen or not based on IsOffScreen property. 
        /// </summary>
        /// <param name="e"></param>
        /// <returns></returns>
        public static bool IsOffScreen(this A11yElement e)
        {
            var p = e.Properties.ById(PropertyType.UIA_IsOffscreenPropertyId);
            return p != null && p.Value == true;
        }

        /// <summary>
        /// Check whether this element is Desktop Element or not. 
        /// it is based on Runtime ID
        /// </summary>
        /// <param name="e"></param>
        /// <returns></returns>
        public static bool IsRootElement(this IA11yElement e)
        {
            // on Desktop, RuntimeId is available with specific ID. 
            // however on WCOS, the desktop equivalent process is PID 0 and runtime ID is 0,0
            return e != null && (e.RuntimeId == DesktopElementRuntimeId || (e.ProcessId == 0 && e.RuntimeId == WCOSElementRuntimeId));
        }

        /// <summary>
        /// Check whether string is matched with DesktopElment runtime Id. 
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static bool IsDesktopElementRuntimeId(string str)
        {
            return DesktopElementRuntimeId == str;
        }

        /// <summary>
        /// Get the Framework value.
        /// </summary>
        /// <param name="e"></param>
        /// <returns></returns>
        public static string GetUIFramework(this IA11yElement e)
        {
            if (e == null) return null;

            return (string.IsNullOrEmpty(e.Framework))
                ? GetUIFramework(e.Parent)
                : e.Framework;
        }

        /// <summary>
        /// Returns the right tree structure rule for the given viewmode
        /// Throws InvalidEnumArgumentException if no rule id property exists
        /// </summary>
        /// <param name="viewMode">tree walker mode (content/control/raw)</param>
        /// <returns>associated rule for typical tree structure</returns>
        public static RuleId GetTreeStructureRule(TreeViewMode viewMode)
        {
            switch (viewMode)
            {
                case TreeViewMode.Content:
                    return RuleId.TypicalTreeStructureContent;
                case TreeViewMode.Control:
                    return RuleId.TypicalTreeStructureControl;
                case TreeViewMode.Raw:
                    return RuleId.TypicalTreeStructureRaw;
                default:
                    throw new InvalidEnumArgumentException(Invariant($"No rule id exists for the given view mode {viewMode}"));
            }
        }

        public static T GetPropertyOrDefault<T>(this A11yElement e, int propertyId)
        {
            if (e == null) return default(T);

            return e.TryGetPropertyValue(propertyId, out T value)
                ? value : default(T);
        }
    }
}
