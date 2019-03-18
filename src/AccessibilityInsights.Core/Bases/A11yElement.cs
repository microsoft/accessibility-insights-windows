// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using AccessibilityInsights.Core.Enums;
using AccessibilityInsights.Core.Misc;
using AccessibilityInsights.Core.Results;
using AccessibilityInsights.Core.Types;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;

using static System.FormattableString;

namespace AccessibilityInsights.Core.Bases
{
    /// <summary>
    /// Define IUIElement for abstract UIElement for all platform
    /// This is the base class for elements implemented in AccessibilityInsights
    /// </summary>
    public class A11yElement : IA11yElement, IDisposable
    {
        private string _ProcessName = null;

        [JsonIgnore]
        public string Name
        {
            get
            {
                return GetPropertySafely(PropertyType.UIA_NamePropertyId)?.TextValue;
            }
        }

        [JsonIgnore]
        public int ControlTypeId
        {
            get
            {
                var p = GetPropertySafely(PropertyType.UIA_ControlTypePropertyId);
                return p != null ? (int)p.Value : 0;
            }
        }

        [JsonIgnore]
        public string LocalizedControlType
        {
            get
            {
                return GetPropertySafely(PropertyType.UIA_LocalizedControlTypePropertyId)?.Value;
            }
        }

        [JsonIgnore]
        public string RuntimeId
        {
            get
            {
                return GetPropertySafely(PropertyType.UIA_RuntimeIdPropertyId)?.TextValue;
            }
        }

        [JsonIgnore]
        public string AcceleratorKey
        {
            get
            {
                return GetPropertySafely(PropertyType.UIA_AcceleratorKeyPropertyId)?.TextValue;
            }
        }

        [JsonIgnore]
        public string AccessKey
        {
            get
            {
                return GetPropertySafely(PropertyType.UIA_AccessKeyPropertyId)?.TextValue;
            }
        }

        [JsonIgnore]
        public string Culture
        {
            get
            {
                return GetPropertySafely(PropertyType.UIA_CulturePropertyId)?.TextValue;
            }
        }

        [JsonIgnore]
        public string AutomationId
        {
            get
            {
                return GetPropertySafely(PropertyType.UIA_AutomationIdPropertyId)?.TextValue;
            }
        }

        [JsonIgnore]
        public int ProcessId
        {
            get
            {
                var p = GetPropertySafely(PropertyType.UIA_ProcessIdPropertyId);
                return p != null ? (int)p.Value : 0;
            }
        }

        [JsonIgnore]
        public Rectangle BoundingRectangle
        {
            get
            {
                return GetPropertySafely(PropertyType.UIA_BoundingRectanglePropertyId).ToRectangle();
            }
        }

        [JsonIgnore]
        public string ClassName
        {
            get
            {
                var property = GetPropertySafely(PropertyType.UIA_ClassNamePropertyId);

                return property?.TextValue;
            }
        }

        [JsonIgnore]
        public string HelpText
        {
            get
            {
                return GetPropertySafely(PropertyType.UIA_HelpTextPropertyId)?.TextValue;
            }
        }

        [JsonIgnore]
        public bool IsEnabled
        {
            get
            {
                var p = GetPropertySafely(PropertyType.UIA_IsEnabledPropertyId);

                return p != null ? p.Value : false;
            }
        }

        [JsonIgnore]
        public bool IsKeyboardFocusable
        {
            get
            {
                var p = GetPropertySafely(PropertyType.UIA_IsKeyboardFocusablePropertyId);

                return p != null ? p.Value : false;
            }
        }

        [JsonIgnore]
        public bool IsContentElement
        {
            get
            {
                var p = GetPropertySafely(PropertyType.UIA_IsContentElementPropertyId);

                if (p != null && p.Value != null)
                {
                    return p.Value;
                }

                return false;
            }
        }

        [JsonIgnore]
        public bool IsControlElement
        {
            get
            {
                var p = GetPropertySafely(PropertyType.UIA_IsControlElementPropertyId);

                if (p != null && p.Value != null)
                {
                    return p.Value;
                }

                return false;
            }
        }

        [JsonIgnore]
        public bool IsOffScreen
        {
            get
            {
                var p = GetPropertySafely(PropertyType.UIA_IsOffscreenPropertyId);

                return p != null ? p.Value : false;
            }
        }

        [JsonIgnore]
        public string ItemStatus
        {
            get
            {
                var p = GetPropertySafely(PropertyType.UIA_ItemStatusPropertyId);

                if (p != null && p.Value != null)
                    return p.TextValue;

                return null;
            }
        }

        public OrientationType Orientation
        {
            get
            {
                var p = GetPropertySafely(PropertyType.UIA_OrientationPropertyId);
                return p != null ? (OrientationType)p.Value : 0;
            }
        }

        [JsonIgnore]
        public int LandmarkType
        {
            get
            {
                var p = GetPropertySafely(PropertyType.UIA_LandmarkTypePropertyId);

                return p != null ? (int)p.Value : 0;
            }
        }

        [JsonIgnore]
        public string LocalizedLandmarkType
        {
            get
            {
                var p = GetPropertySafely(PropertyType.UIA_LocalizedLandmarkTypePropertyId);

                return p?.TextValue;
            }
        }

        [JsonIgnore]
        public int HeadingLevel
        {
            get
            {
                var p = GetPropertySafely(PropertyType.UIA_HeadingLevelPropertyId);

                return p != null ? (int)p.Value : HeadingLevelType.HeadingLevelNone;
            }
        }

        [JsonIgnore]
        public string Framework
        {
            get
            {
                var p = GetPropertySafely(PropertyType.UIA_FrameworkIdPropertyId);

                return p?.TextValue;
            }
        }

        public bool TryGetPropertyValue<T>(int propertyId, out T value)
        {
            // assignment required
            value = default(T);

            var property = this.GetPropertySafely(propertyId);
            if (property == null) return false;

            dynamic temp = ConvertValueIfNecessary(propertyId, property.Value);

            // Want to throw new Exceptions.InvalidPropertyTypeRequestedException($"Expected temp, which is type {temp.GetType().Name}, to be type {typeof(T).Name}")
            // Except that a function beginning with "Try..." should (IMHO) theoretically not throw exceptions
            if (!(temp is T))
                return false;

            value = temp ;

            return true;
        }

        private static dynamic ConvertValueIfNecessary(int id, Newtonsoft.Json.Linq.JArray jArray)
        {
            // Tried to set this up inside A11yProperty
            // However, since that class can be initialized by Newtonsoft.Json,
            // Trying to set the value in the constructor does no good.
            // because the value is set later. And yet the time when the property id may be set is indeterminate.

            switch (id)
            {
                case PropertyType.UIA_BoundingRectanglePropertyId:
                    return GetValueAsArray<double>(jArray);
            } // switch

            return GetValueAsArray<int>(jArray);
        }
#pragma warning disable CA1801 // Parameter id of method ConvertValueIfNecessary is never used.Remove the parameter or use it in the method body.

        private static dynamic ConvertValueIfNecessary(int id, dynamic value)
        {
            return value;
        }
#pragma warning restore CA1801 // Parameter id of method ConvertValueIfNecessary is never used.Remove the parameter or use it in the method body.

        private static T[] GetValueAsArray<T>(Newtonsoft.Json.Linq.JArray jArray)
        {
            var enumerable = from v in jArray.Values<T>()
                             select v;

            return enumerable.ToArray();
        }

        public IA11yPattern GetPattern(int patternId)
        {
            var pattern = this.Patterns?.FirstOrDefault(p => p.Id == patternId);
            if (pattern == null) return null;

            return pattern;
        }

        public T GetPlatformPropertyValue<T>(int propertyId)
        {
            var property = this.PlatformProperties?.ById(propertyId);
            if (property == null) return default(T);
            if (!(property.Value is T)) throw new Exception(Invariant($"Expected property.Value, which is type {property.Value.GetType().Name}, to be type {typeof(T).Name}"));

            return property.Value;
        }

        [JsonIgnore]
        public int PositionInSet
        {
            get
            {
                var p = GetPropertySafely(PropertyType.UIA_PositionInSetPropertyId);

                return p != null ? (int)p.Value : 0;
            }
        }

        [JsonIgnore]
        public int SizeOfSet
        {
            get
            {
                var p = GetPropertySafely(PropertyType.UIA_SizeOfSetPropertyId);

                return p != null ? (int)p.Value : 0;
            }
        }

        [JsonIgnore]
        public bool TransformPatternCanResize
        {
            get
            {
                var p = GetPropertySafely(PropertyType.UIA_TransformPattern_CanResizePropertyId);

                return p != null ? p.Value : false;
            }
        }

        [JsonIgnore]
        public string ProcessName
        {
            get
            {
                if (_ProcessName != null)
                    return _ProcessName;

                var name = Utility.GetProcessName(this.ProcessId);

                _ProcessName = name ?? "";

                return _ProcessName;
            }
        }

        public IA11yElement GetFirstChild()
        {
            if (this.Children == null) return null;
            if (!this.Children.Any()) return null;

            return Children[0];
        }

        public A11yElement FindDescendent(Func<A11yElement, Boolean> condition)
        {
            Queue<A11yElement> children = new Queue<A11yElement>(this.Children);
            while (children.Count > 0)
            {
                var currentChild = children.Dequeue();
                if (condition(currentChild))
                {
                    return currentChild;
                }

                if (currentChild.Children?.Any() == true)
                {
                    foreach (A11yElement c in currentChild.Children)
                    {
                        children.Enqueue(c);
                    }
                }
            }

            return null;
        }

        #region Test related
        [JsonIgnore]
        public ScanStatus TestStatus
        {
            get
            {
                if (this.ScanResults != null)
                {
                    return this.ScanResults.Status;
                }

                return ScanStatus.NoResult;
            }
        }
#endregion

        /// <summary>
        /// Header of this element
        /// Pattern of text is below.
        /// Localized Control Type "Name"
        /// </summary>
        public string Glimpse { get; set; }

        /// <summary>
        /// Unique Id of the element
        /// this value is set based on each platform
        /// it should be populated by tree walker. 
        /// </summary>
        public int UniqueId { get; set; }

#pragma warning disable CA2227 // Collection properties should be read only
        /// <summary>
        /// Properties. it is populated automatically at construction
        /// </summary>
        public Dictionary<int, A11yProperty> Properties { get; set; }

        /// <summary>
        /// Platform specific attributes
        /// </summary>
        public Dictionary<int, A11yProperty> PlatformProperties { get; set; }

        public List<A11yPattern> Patterns { get; set; }

        /// <summary>
        /// Child Elements
        /// </summary>
        public List<A11yElement> Children { get; set; }
#pragma warning restore CA2227 // Collection properties should be read only

        /*
         * From https://docs.microsoft.com/en-us/dotnet/csharp/language-reference/compiler-messages/cs0106
         * The public keyword is not allowed on an explicit interface declaration. In this case, remove the
         * public keyword from the explicit interface declaration.
         */
        IEnumerable<IA11yElement> IA11yElement.Children
        {
            get
            {
                if (this.Children == null) yield break;

                foreach (var child in this.Children)
                    yield return child;
            }
        }

        public TreeViewMode TreeWalkerMode { get; set; }

        /// <summary>
        /// Parent Element
        /// </summary>
        [JsonIgnore]
        public A11yElement Parent { get; set; }

        [JsonIgnore]
        IA11yElement IA11yElement.Parent
        {
            get
            {
                return this.Parent;
            }
        }

        /// <summary>
        /// Platform Object
        /// on Windows, it would be IUIAutomation
        /// </summary>
        [JsonIgnore]
        public dynamic PlatformObject { get; set; }

        /// <summary>
        /// Indicate whether this is the ancestor of the selected element
        /// </summary>
        public bool IsAncestorOfSelected { get; set; } = false;

        /// <summary>
        /// ScanResults
        /// it is available only after some tests were run via TestFactory
        /// </summary>
        public ScanResults ScanResults { get; set; }

        /// <summary>
        /// Returns the display text for the issue.
        /// </summary>
        public string IssueDisplayText { get; set; }

        public ScanResults GetScannerResultInstance()
        {
            var srs = new ScanResults();
            this.ScanResults = srs;

            return srs;
        }

        /// <summary>
        /// Get Property Safely.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        protected A11yProperty GetPropertySafely(int id)
        {
            if (this.Properties == null) return null;

            return this.Properties.TryGetValue(id, out A11yProperty property)
                ? property : null;
        }

        /// <summary>
        /// Save element in Json format
        /// </summary>
        /// <param name="path"></param>
        public void SaveInJson(string path)
        {
            var json = JsonConvert.SerializeObject(this, Formatting.Indented);

            File.WriteAllText(path, json, Encoding.UTF8);
        }

        /// <summary>
        /// Load JSON stored A11yElement from stream
        /// </summary>
        /// <param name="stream"></param>
        /// <returns></returns>
        public static A11yElement FromStream(Stream stream)
        {
            using (StreamReader reader = new StreamReader(stream))
            {
                string json = reader.ReadToEnd();

                return FromText(json);
            }
        }

        /// <summary>
        /// Load from Json text and update parent
        /// </summary>
        /// <param name="json"></param>
        /// <returns></returns>
        private static A11yElement FromText(string json)
        {
            var e =  JsonConvert.DeserializeObject<A11yElement>(json);

            UpdateParent(null, e);

            return e;
        }

        /// <summary>
        /// Update Parent since snapshot doesn't contain parent reference. 
        /// </summary>
        /// <param name="p"></param>
        /// <param name="e"></param>
        private static void UpdateParent(A11yElement p, A11yElement e)
        {
            if(e != null)
            {
                e.Parent = p;
            }

            if(e.Children != null && e.Children.Count != 0)
            {
                foreach(var c in e.Children)
                {
                    UpdateParent(e, c);
                }
            }
        }

        #region IDisposable Support
        private bool _disposedValue = false; // To detect redundant calls
        protected bool DisposedValue { get => _disposedValue; set => _disposedValue = value; }

        protected virtual void Dispose(bool disposing)
        {
            if (!DisposedValue)
            {
                if (disposing)
                {
                    if (Patterns != null)
                    {
                        foreach (var ptn in Patterns)
                        {
                            ptn.Dispose();
                        }
                        this.Patterns.Clear();
                        this.Patterns = null;
                    }

                    if (Properties != null)
                    {
                        foreach (var pp in this.Properties.Values)
                        {
                            pp.Dispose();
                        }
                        this.Properties.Clear();
                        this.Properties = null;
                    }

                    this.PlatformProperties?.Clear();
                    this.PlatformProperties = null;
                    this.Children?.Clear();
                    this.Children = null;
                    this.Parent = null;
                    this.ScanResults = null;
                    this.PlatformObject = null;
                    this.Glimpse = null;
                }

                DisposedValue = true;
            }
        }

        // This code added to correctly implement the disposable pattern.
        public void Dispose()
        {
            // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
            Dispose(true);
            GC.SuppressFinalize(this);
        }
#endregion
    }
}
