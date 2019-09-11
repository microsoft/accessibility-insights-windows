// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using Axe.Windows.Core.Bases;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Collections.ObjectModel;
using System.ComponentModel;
using Axe.Windows.Core.Types;
using Axe.Windows.Desktop.Types;
using AccessibilityInsights.SharedUx.Settings;
using AccessibilityInsights.SharedUx.Enums;
using AccessibilityInsights.SharedUx.Properties;
using System;

namespace AccessibilityInsights.SharedUx.ViewModels
{
    /// <summary>
    /// EventConfigNodeViewModel class
    /// ViewModel for events configuration tab tree
    /// </summary>
    public class EventConfigNodeViewModel : ViewModelBase, INotifyPropertyChanged
    {
        /// <summary>
        /// Child ViewModels
        /// </summary>
        public ObservableCollection<EventConfigNodeViewModel> Children { get; private set; }

        /// <summary>
        /// Depth for treeviewitem
        /// </summary>
        public int Depth { get; set; }

        /// <summary>
        /// Is node checked
        /// </summary>
        private bool _ischecked = false;
        public bool IsChecked
        {
            get
            {
                return this._ischecked;
            }

            set
            {
                if (_ischecked != value)
                {
                    switch (this.Type)
                    {
                        case EventConfigNodeType.Event:
                            SetChecked(ConfigurationManager.GetDefaultInstance().EventConfig, this.Id, RecordEntityType.Event, value);
                            break;
                        case EventConfigNodeType.Property:
                            SetChecked(ConfigurationManager.GetDefaultInstance().EventConfig, this.Id, RecordEntityType.Property, value, this.Header);
                            break;
                        case EventConfigNodeType.Group:
                            this.Children?.ToList().ForEach(c => c.IsChecked = value);
                            break;
                    }
                }
                this._ischecked = value;
                OnPropertyChanged(nameof(this.IsChecked));
            }
        }

        /// <summary>
        /// Is node expanded
        /// </summary>
        private bool _isexpanded = false;
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
        /// Add a single child to node
        /// </summary>
        /// <param name="child"></param>
        public void AddChild(EventConfigNodeViewModel child, bool isChecked=false)
        {
            if (child == null)
                throw new ArgumentNullException(nameof(child));

            this.Children.Add(child);
            child.IsChecked = isChecked;
            if (child.Id == EventType.UIA_AutomationFocusChangedEventId)
            {
                child.IsChecked = ConfigurationManager.GetDefaultInstance().EventConfig.IsListeningFocusChangedEvent;
            }
            child.Depth = this.Depth + 1;
        }

        /// <summary>
        /// Create and add children from a collection of ids
        /// </summary>
        /// <param name="ids"></param>
        /// <param name="type"></param>
        public void AddChildren(IEnumerable<int> ids, EventConfigNodeType type, bool isChecked=false)
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
        /// <param name="ids"></param>
        /// <param name="type"></param>
        public void AddChildren(IEnumerable<A11yProperty> properties, bool isChecked = false)
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
            var l = Children.OrderBy(e => e.Header).ToList();
            this.Children.Clear();
            foreach(var c in l)
            {
                this.Children.Add(c);
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
                this.Children?.AsParallel().ForAll(c => c.IsEditEnabled = value);
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

            child.IsChecked = false;
            this.Children.Remove(child);
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
        /// <param name="name"></param>
        /// <param name="vis"></param>
        /// <param name="txt"></param>
        public EventConfigNodeViewModel(string name, Visibility vis = Visibility.Collapsed, string txt = "")
        {
            this.Type = EventConfigNodeType.Group;
            this.Header = name;
            this.Children = new ObservableCollection<EventConfigNodeViewModel>();
            this.Depth = 0;
            this.ButtonText = txt;
            this.ButtonVisibility = vis;
            this.TextVisibility = Visibility.Visible;
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

            if(expandchildren && this.Children.Count != 0)
            {
                foreach(var c in this.Children)
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
            if (this.Children != null)
            {
                foreach (var c in this.Children)
                {
                    c.Clear();
                }

                this.Children.Clear();
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
                var check = this.IsChecked ? Resources.EventConfigNodeViewModel_ToString_checked : Resources.EventConfigNodeViewModel_ToString_unchecked;
                return this.Header + check;
            }
            else
            {
                return this.ButtonText + " node";
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
    }
}
