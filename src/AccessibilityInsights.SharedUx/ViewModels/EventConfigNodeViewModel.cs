// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using AccessibilityInsights.SharedUx.Enums;
using AccessibilityInsights.SharedUx.Properties;
using AccessibilityInsights.SharedUx.Settings;
using Axe.Windows.Core.Bases;
using Axe.Windows.Core.Types;
using Axe.Windows.Desktop.Types;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Windows;

namespace AccessibilityInsights.SharedUx.ViewModels
{
    /// <summary>
    /// EventConfigNodeViewModel class
    /// ViewModel for events configuration tab tree
    /// </summary>
    public class EventConfigNodeViewModel : ViewModelBase, INotifyPropertyChanged
    {
        /// <summary>
        /// Child ViewModels - external code can read but not modify
        /// </summary>
        public IReadOnlyCollection<EventConfigNodeViewModel> Children => this._children;

        private readonly ObservableCollection<EventConfigNodeViewModel> _children;

        /// <summary>
        /// Depth for treeviewitem
        /// </summary>
        public int Depth { get; set; }

        /// <summary>
        /// Status of IsThreeState property
        /// </summary>
        public bool IsThreeState { get; }

        /// <summary>
        /// Is node checked (null if in "mixed" ThreeState value)
        /// </summary>
        private bool? _ischecked = false;
        public bool? IsChecked
        {
            get
            {
                return this.IsThreeState ? this._ischecked : (this._ischecked.HasValue && this._ischecked.Value);
            }

            set
            {
                SetCheckedInternal(value);
            }
        }

        private void SetCheckedInternal(bool? value, bool respondingToChildChange = false)
        {
            if (this.IsThreeState)
            {
                if (respondingToChildChange)
                {
                    value = ComputeCheckedStateBasedOnChildren();
                }
                else if (!value.HasValue)
                {
                    value = false; // Skip user-driven intermediate state (false follows "mixed" in the cycle)
                }
            }

            if (value != this._ischecked)
            {
                this._ischecked = value;
                if (value.HasValue)
                {
                    switch (this.Type)
                    {
                        case EventConfigNodeType.Event:
                            SetChecked(ConfigurationManager.GetDefaultInstance().EventConfig, this.Id, RecordEntityType.Event, value.Value);
                            break;
                        case EventConfigNodeType.Property:
                            SetChecked(ConfigurationManager.GetDefaultInstance().EventConfig, this.Id, RecordEntityType.Property, value.Value, this.Header);
                            break;
                        case EventConfigNodeType.Group:
                            this._children?.ToList().ForEach(c => c.IsChecked = value.Value);
                            break;
                    }
                }
                this._parent?.ChildCheckStateHasChanged(value);
                OnPropertyChanged(nameof(this.IsChecked));
            }
        }
        private bool? ComputeCheckedStateBasedOnChildren()
        {
            int checkboxChildCount = 0;
            int checkedChildCount = 0;
            int uncheckedChildCount = 0;
            int indeterminateChildCount = 0;

            foreach (var child in this._children)
            {
                // Skip buttons
                if (!string.IsNullOrWhiteSpace(child.ButtonText))
                    continue;

                checkboxChildCount++;

                bool? childIsChecked = child?.IsChecked;

                if (!childIsChecked.HasValue)
                {
                    indeterminateChildCount++;
                }
                else if (childIsChecked.Value)
                {
                    checkedChildCount++;
                }
                else
                {
                    uncheckedChildCount++;
                }
            }

            if (checkedChildCount == checkboxChildCount)
            {
                return checkboxChildCount > 0; // Return false only if we have no checkbox children
            }
            if (uncheckedChildCount == checkboxChildCount)
            {
                return false;
            }
            return null;
        }

        /// <summary>
        /// Is node expanded
        /// </summary>
        private bool _isexpanded;
        public bool IsExpanded
        {
            get
            {
                return this._isexpanded;
            }

            set
            {
                this._isexpanded = value;
                OnPropertyChanged(nameof(IsExpanded));
            }
        }

        /// <summary>
        /// Text for node
        /// </summary>
        public string Header { get; private set; }

        string _buttontext;
        /// <summary>
        /// Text on button
        /// </summary>
        public string ButtonText
        {
            get
            {
                return _buttontext;
            }
            set
            {
                _buttontext = value;
                OnPropertyChanged(nameof(ButtonText));
            }
        }

        /// <summary>
        /// Is button visibile
        /// </summary>
        public Visibility ButtonVisibility { get; private set; }

        Visibility _buttonvisiblity;

        /// <summary>
        /// Is text visible
        /// </summary>
        public Visibility TextVisibility
        {
            get
            {
                return _buttonvisiblity;
            }
            set
            {
                _buttonvisiblity = value;
                OnPropertyChanged(nameof(TextVisibility));
            }
        }

        /// <summary>
        /// Insert a single child at the specified location
        /// </summary>
        /// <param name="index">The index in the list to insert the child</param>
        /// <param name="child">The child to insert</param>
        public void InsertChildAtIndex(int index, EventConfigNodeViewModel child)
        {
            if (child == null)
                throw new ArgumentNullException(nameof(child));

            this._children.Insert(index, child);
            child._parent = this;
            this.SetCheckedInternal(null, respondingToChildChange: true);
        }

        /// <summary>
        /// Add a single child to node
        /// </summary>
        /// <param name="child"></param>
        public void AddChild(EventConfigNodeViewModel child, bool isChecked=false)
        {
            if (child == null)
                throw new ArgumentNullException(nameof(child));

            this._children.Add(child);
            child.IsChecked = isChecked;
            if (child.Id == EventType.UIA_AutomationFocusChangedEventId)
            {
                child.IsChecked = ConfigurationManager.GetDefaultInstance().EventConfig.IsListeningFocusChangedEvent;
            }
            child.Depth = this.Depth + 1;
            child._parent = this;
            this.SetCheckedInternal(null, respondingToChildChange: true);
        }

        /// <summary>
        /// Create and add children from a collection of ids
        /// </summary>
        public void AddChildren(IEnumerable<int> ids, EventConfigNodeType type)
        {
            this.AddChildren(ids, type, false);
        }

        /// <summary>
        /// Create and add children from a collection of ids
        /// </summary>
        public void AddChildren(IEnumerable<int> ids, EventConfigNodeType type, bool isChecked)
        {
            if (ids == null)
                throw new ArgumentNullException(nameof(ids));

            foreach (int id in ids)
            {
                this.AddChild(new EventConfigNodeViewModel(id, type), isChecked);
            }
        }

        /// <summary>
        /// Create and add children from a collection of properties
        /// </summary>
        public void AddChildren(IEnumerable<A11yProperty> properties)
        {
            this.AddChildren(properties, false);
        }

        /// <summary>
        /// Create and add children from a collection of properties
        /// </summary>
        public void AddChildren(IEnumerable<A11yProperty> properties, bool isChecked)
        {
            if (properties == null)
                throw new ArgumentNullException(nameof(properties));

            foreach (A11yProperty property in properties)
            {
                this.AddChild(new EventConfigNodeViewModel(property.Id, property.Name, EventConfigNodeType.Property), isChecked);
            }
        }

        /// <summary>
        /// Node's event or property ID
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Is node event, property, or neither
        /// </summary>
        public EventConfigNodeType Type { get; set; }

        /// <summary>
        /// Sort node's children alphabetically
        /// </summary>
        public void SortChildren()
        {
            var l = _children.OrderBy(e => e.Header).ToList();
            this._children.Clear();
            foreach(var c in l)
            {
                this._children.Add(c);
            }
        }

        bool _iseditenabled = true;
        /// <summary>
        /// Can node be checked
        /// </summary>
        public bool IsEditEnabled
        {
            get
            {
                return _iseditenabled;
            }

            set
            {
                _iseditenabled = value;
                this._children?.AsParallel().ForAll(c => c.IsEditEnabled = value);
                OnPropertyChanged(nameof(IsEditEnabled));
            }
        }

        /// <summary>
        /// Remove a child
        /// </summary>
        /// <param name="child"></param>
        public void RemoveChild(EventConfigNodeViewModel child)
        {
            if (child == null)
                throw new ArgumentNullException(nameof(child));

            child._parent = null;
            this._children.Remove(child);
            this.SetCheckedInternal(null, respondingToChildChange: true);
            child.IsChecked = false;
        }

        /// <summary>
        /// Constructor for property leaf node with given name
        /// </summary>
        /// <param name="id"></param>
        /// <param name="type"></param>
        public EventConfigNodeViewModel(int id, string name, EventConfigNodeType type)
        {
            this.Id = id;
            this.Header = name;
            this.Type = type;
            this.ButtonVisibility = Visibility.Collapsed;
            this.TextVisibility = Visibility.Visible;
        }

        /// <summary>
        /// Constructor for event or property leaf node from id
        /// </summary>
        /// <param name="id"></param>
        /// <param name="type"></param>
        public EventConfigNodeViewModel(int id, EventConfigNodeType type)
        {
            this.Id = id;
            if (type == EventConfigNodeType.Event)
            {
                this.Header = EventType.GetInstance().GetNameById(id);
            }
            else
            {
                this.Header = PropertyType.GetInstance().GetNameById(id);
            }
            this.Type = type;
            this.ButtonVisibility = Visibility.Collapsed;
            this.TextVisibility = Visibility.Visible;
        }

        /// <summary>
        /// Constructor for a node with children
        /// </summary>
        /// <param name="name">Name to display (checkbox only)</param>
        /// <param name="vis">Initial Visibility State</param>
        /// <param name="txt">Text to display (buttons only)</param>
        /// <param name="isThreeState">true to enable ThreeState behavior (chechboxes only)</param>
        public EventConfigNodeViewModel(string name, Visibility vis = Visibility.Collapsed, string txt = "", bool isThreeState = false)
        {
            this.Type = EventConfigNodeType.Group;
            this.Header = name;
            this._children = new ObservableCollection<EventConfigNodeViewModel>();
            this.Depth = 0;
            this.ButtonText = txt;
            this.ButtonVisibility = vis;
            this.TextVisibility = Visibility.Visible;
            this.IsThreeState = isThreeState;
            if (isThreeState)
            {
                this.IsChecked = null;
            }
        }

        /// <summary>
        /// Expand tree
        /// if expandchildren is true, make children expanded too
        /// it will expand all descendants
        /// </summary>
        /// <param name="expandchildren"></param>
        internal void Expand(bool expandchildren = false)
        {
            this.IsExpanded = true;

            if(expandchildren && this._children.Count != 0)
            {
                foreach(var c in this._children)
                {
                    c.Expand(true);
                }
            }
        }

        /// <summary>
        /// Clear tree
        /// </summary>
        internal void Clear()
        {
            if (this._children != null)
            {
                foreach (var c in this._children)
                {
                    c.Clear();
                }

                this._children.Clear();
            }
        }

        /// <summary>
        /// Narrator string
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            if (this.TextVisibility == Visibility.Visible)
            {
                var check = (this.IsChecked.HasValue && this.IsChecked.Value)
                    ? Resources.EventConfigNodeViewModel_ToString_checked
                    : Resources.EventConfigNodeViewModel_ToString_unchecked;
                return this.Header + check;
            }
            else
            {
                return string.Format(CultureInfo.InvariantCulture,
                    Resources.EventConfigNodeViewModel_NodeFormat, this.ButtonText);
            }
        }

        /// <summary>
        /// Set the checked state based on id and type
        /// </summary>
        /// <param name="id"></param>
        /// <param name="type"></param>
        /// <param name="val"></param>
        private static void SetChecked(RecorderSetting setting, int id, RecordEntityType type, bool val, string name = null)
        {
            int change = val ? 1 : -1;
            if (type == RecordEntityType.Event)
            {
                if (id == EventType.UIA_AutomationFocusChangedEventId)
                {
                    setting.IsListeningFocusChangedEvent = val;
                }
                else
                {
                    setting.Events.Where(e => e.Id == id).First().CheckedCount += change;
                }
            }
            else
            {
                if (setting.Properties.Where(e => e.Id == id).Any())
                {
                    setting.Properties.Where(e => e.Id == id).First().CheckedCount += change;
                }
                else
                {
                    setting.Properties.Add(new RecordEntitySetting()
                    {
                        Type = RecordEntityType.Property,
                        Id = id,
                        Name = name,
                        IsCustom = true,
                        IsRecorded = false,
                        CheckedCount = 1
                    });
                }
            }
        }

        private EventConfigNodeViewModel _parent;

        private void ChildCheckStateHasChanged(bool? childIsChecked)
        {
            // Respond to children only if their state doesn't match the current state
            if (this.IsThreeState && childIsChecked != IsChecked)
            {
                SetCheckedInternal(null, respondingToChildChange: true);
            }
        }
    }
}
