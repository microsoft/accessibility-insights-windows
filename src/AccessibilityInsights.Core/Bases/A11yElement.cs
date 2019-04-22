// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using Axe.Windows.Core.Enums;
using Axe.Windows.Core.Misc;
using Axe.Windows.Core.Results;
using Axe.Windows.Core.Types;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;

using static System.FormattableString;

namespace Axe.Windows.Core.Bases
{
    /// <summary>
    /// Define IUIElement for abstract UIElement for all platform
    /// This is the base class for elements implemented in Axe.Windows
    /// </summary>
    public class A11yElement : IA11yElement, IDisposable
    {
        private string _ProcessName = null;

        /// <summary>
        /// The label or primary text identifier for the element
        /// </summary>
        [JsonIgnore]
        public string Name
        {
            get
            {
                return GetPropertySafely(PropertyType.UIA_NamePropertyId)?.TextValue;
            }
        }

        /// <summary>
        /// The control type identifier for the element; see <see cref="Axe.Windows.Core.Types.ControlType"/>
        /// </summary>
        [JsonIgnore]
        public int ControlTypeId
        {
            get
            {
                var p = GetPropertySafely(PropertyType.UIA_ControlTypePropertyId);
                return p != null ? (int)p.Value : 0;
            }
        }

        /// <summary>
        /// a localized text representation of <see cref="ControlTypeId"/>
        /// </summary>
        [JsonIgnore]
        public string LocalizedControlType
        {
            get
            {
                return GetPropertySafely(PropertyType.UIA_LocalizedControlTypePropertyId)?.Value;
            }
        }

        /// <summary>
        /// a unique identifier for the element, valid only for the element's lifetime
        /// </summary>
        [JsonIgnore]
        public string RuntimeId
        {
            get
            {
                return GetPropertySafely(PropertyType.UIA_RuntimeIdPropertyId)?.TextValue;
            }
        }

        /// <summary>
        /// a text description of a keystroke which activates the element; valid within a dialog, pane, or app
        /// </summary>
        [JsonIgnore]
        public string AcceleratorKey
        {
            get
            {
                return GetPropertySafely(PropertyType.UIA_AcceleratorKeyPropertyId)?.TextValue;
            }
        }

        /// <summary>
        /// a text description of a keystroke which activates the element; usually a single key; only valid within a limited scope such as menus
        /// </summary>
        [JsonIgnore]
        public string AccessKey
        {
            get
            {
                return GetPropertySafely(PropertyType.UIA_AccessKeyPropertyId)?.TextValue;
            }
        }

        /// <summary>
        /// the culture/language of the text properties of the element
        /// </summary>
        [JsonIgnore]
        public string Culture
        {
            get
            {
                return GetPropertySafely(PropertyType.UIA_CulturePropertyId)?.TextValue;
            }
        }

        /// <summary>
        /// a permanent unique identifier for the specific element, or sometimes the type of element
        /// </summary>
        [JsonIgnore]
        public string AutomationId
        {
            get
            {
                return GetPropertySafely(PropertyType.UIA_AutomationIdPropertyId)?.TextValue;
            }
        }

        /// <summary>
        /// the process ID of the process which owns the element
        /// </summary>
        [JsonIgnore]
        public int ProcessId
        {
            get
            {
                var p = GetPropertySafely(PropertyType.UIA_ProcessIdPropertyId);
                return p != null ? (int)p.Value : 0;
            }
        }

        /// <summary>
        /// the screen rectangle encompassing the element; typically corresponds to the visible bounds of the element
        /// </summary>
        [JsonIgnore]
        public Rectangle BoundingRectangle
        {
            get
            {
                return GetPropertySafely(PropertyType.UIA_BoundingRectanglePropertyId).ToRectangle();
            }
        }

        /// <summary>
        /// a name identifying the underlying class type of the element; common for types such as windows
        /// </summary>
        [JsonIgnore]
        public string ClassName
        {
            get
            {
                var property = GetPropertySafely(PropertyType.UIA_ClassNamePropertyId);

                return property?.TextValue;
            }
        }

        /// <summary>
        /// information on how to interact with a control, useful contextual information, or other special instructions
        /// </summary>
        [JsonIgnore]
        public string HelpText
        {
            get
            {
                return GetPropertySafely(PropertyType.UIA_HelpTextPropertyId)?.TextValue;
            }
        }

        /// <summary>
        /// whether the user may interact with the element
        /// </summary>
        [JsonIgnore]
        public bool IsEnabled
        {
            get
            {
                var p = GetPropertySafely(PropertyType.UIA_IsEnabledPropertyId);

                return p != null ? p.Value : false;
            }
        }

        /// <summary>
        /// whether the element may receive keyboard focus
        /// </summary>
        [JsonIgnore]
        public bool IsKeyboardFocusable
        {
            get
            {
                var p = GetPropertySafely(PropertyType.UIA_IsKeyboardFocusablePropertyId);

                return p != null ? p.Value : false;
            }
        }

        /// <summary>
        /// whether the element is part of the UI Automation content view
        /// </summary>
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

        /// <summary>
        /// whether the element is part of the UI Automation control view
        /// </summary>
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

        /// <summary>
        /// whether the element is visible on the screen
        /// </summary>
        [JsonIgnore]
        public bool IsOffScreen
        {
            get
            {
                var p = GetPropertySafely(PropertyType.UIA_IsOffscreenPropertyId);

                return p != null ? p.Value : false;
            }
        }

        /// <summary>
        /// a text description of the element's status
        /// </summary>
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

        /// <summary>
        /// whether the element is oriented horizontally or vertically
        /// </summary>
        public OrientationType Orientation
        {
            get
            {
                var p = GetPropertySafely(PropertyType.UIA_OrientationPropertyId);
                return p != null ? (OrientationType)p.Value : 0;
            }
        }

        /// <summary>
        /// the type of landmark if it exists; otherwise, 0; related to aria-landmarks
        /// </summary>
        [JsonIgnore]
        public int LandmarkType
        {
            get
            {
                var p = GetPropertySafely(PropertyType.UIA_LandmarkTypePropertyId);

                return p != null ? (int)p.Value : 0;
            }
        }

        /// <summary>
        /// a localized text representation of <see cref="LandmarkType"/> if it exists; otherwise, null
        /// </summary>
        [JsonIgnore]
        public string LocalizedLandmarkType
        {
            get
            {
                var p = GetPropertySafely(PropertyType.UIA_LocalizedLandmarkTypePropertyId);

                return p?.TextValue;
            }
        }

        /// <summary>
        /// the heading level if it exists; otherwise, 0
        /// </summary>
        [JsonIgnore]
        public int HeadingLevel
        {
            get
            {
                var p = GetPropertySafely(PropertyType.UIA_HeadingLevelPropertyId);

                return p != null ? (int)p.Value : HeadingLevelType.HeadingLevelNone;
            }
        }

        /// <summary>
        /// the framework on which the application is based: WPF, UWP, etc
        /// </summary>
        [JsonIgnore]
        public string Framework
        {
            get
            {
                var p = GetPropertySafely(PropertyType.UIA_FrameworkIdPropertyId);

                return p?.TextValue;
            }
        }

        /// <summary>
        /// Safely gets a specified property's value. Throws no exceptions.
        /// </summary>
        /// <typeparam name="T">the expected type of the value parameter; The function will return false if the given type does not match the actual type of the property.</typeparam>
        /// <param name="propertyId">the ID of the property to retrieve; see <see cref="Axe.Windows.Core.Types.PropertyType"/></param>
        /// <param name="value">If the method returns true, this parameter contains the property value. If the function returns false, the value is undefined.</param>
        /// <returns>true if the property was retrieved successfully and types match; otherwise, false</returns>
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

        /// <summary>
        /// Retrieves an <see cref="IA11yPattern"/> representing the specified pattern.
        /// </summary>
        /// <param name="patternId">the ID of the pattern to retrieve; see <see cref="PatternType"/></param>
        /// <returns>an <see cref="IA11yPattern"/> object for the specified pattern if it exists; otherwise, null.</returns>
        public IA11yPattern GetPattern(int patternId)
        {
            var pattern = this.Patterns?.FirstOrDefault(p => p.Id == patternId);
            if (pattern == null) return null;

            return pattern;
        }

        /// <summary>
        /// Gets a specified property value for the current platform, e.g., Windows.
        /// </summary>
        /// <typeparam name="T">the expected type of the property value</typeparam>
        /// <param name="propertyId">the ID of the platform property to retrieve; see <see cref="PlatformPropertyType"/></param>
        /// <returns>the value of the specified property if it exists; otherwise, the default value for the given type</returns>
        public T GetPlatformPropertyValue<T>(int propertyId)
        {
            var property = this.PlatformProperties?.ById(propertyId);
            if (property == null) return default(T);
            if (!(property.Value is T)) throw new Exception(Invariant($"Expected property.Value, which is type {property.Value.GetType().Name}, to be type {typeof(T).Name}"));

            return property.Value;
        }

        /// <summary>
        /// the index of an item within a set
        /// </summary>
        [JsonIgnore]
        public int PositionInSet
        {
            get
            {
                var p = GetPropertySafely(PropertyType.UIA_PositionInSetPropertyId);

                return p != null ? (int)p.Value : 0;
            }
        }

        /// <summary>
        /// the total number of items in a set
        /// </summary>
        [JsonIgnore]
        public int SizeOfSet
        {
            get
            {
                var p = GetPropertySafely(PropertyType.UIA_SizeOfSetPropertyId);

                return p != null ? (int)p.Value : 0;
            }
        }

        /// <summary>
        /// whether the element is resizeable
        /// </summary>
        [JsonIgnore]
        public bool TransformPatternCanResize
        {
            get
            {
                var p = GetPropertySafely(PropertyType.UIA_TransformPattern_CanResizePropertyId);

                return p != null ? p.Value : false;
            }
        }

        /// <summary>
        /// the name of the process that owns the element
        /// </summary>
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

        /// <summary>
        /// Gets the element's first child element if it exists.
        /// </summary>
        /// <returns>an <see cref="IA11yElement"/> object for the child if it exists</returns>
        public IA11yElement GetFirstChild()
        {
            if (this.Children == null) return null;
            if (!this.Children.Any()) return null;

            return Children[0];
        }

        /// <summary>
        /// Finds the first decendent element matching the given condition.
        /// </summary>
        /// <param name="condition">a function that returns true if the given descendant element meets a set of criteria</param>
        /// <returns>an <see cref="IA11yElement"/> object representing the matching descendant if one exists; otherwise, false</returns>
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

        /// <summary>
        /// a collection of patterns supported by the element
        /// </summary>
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

        /// <summary>
        /// the <see cref="TreeViewMode"/> in which the element was found, or which should be used to walk to related elements
        /// </summary>
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
