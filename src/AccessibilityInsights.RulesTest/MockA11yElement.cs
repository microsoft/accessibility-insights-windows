// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System.Collections.Generic;
using System.Drawing;
using Axe.Windows.Core.Bases;
using Axe.Windows.Core.Enums;
using Axe.Windows.Core.Types;

namespace Axe.Windows.RulesTest
{
    class MockA11yElement : A11yElement
    {
        public MockA11yElement() : base()
        {
            Properties = new Dictionary<int, A11yProperty>();
            Patterns = new List<A11yPattern>();
            Children = new List<A11yElement>();
            PlatformProperties = new Dictionary<int, A11yProperty>();
        }

        private void SetProperty(int id, dynamic value)
        {
            if (this.Properties.ContainsKey(id))
            {
                this.Properties[id].Value = value;
                return;
            }

            var property = new A11yProperty {  Id = id, Value = value };
            this.Properties[id] = property;
        }

        public new string Name
        {
            get
            {
                return base.Name;
            }

            set
            {
                SetProperty(PropertyType.UIA_NamePropertyId, value);
            }
        }

        public new string LocalizedControlType
        {
            get
            {
                return base.LocalizedControlType;
            }

            set
            {
                SetProperty(PropertyType.UIA_LocalizedControlTypePropertyId, value);
            }
        }

        public new int ControlTypeId
        {
            get
            {
                return base.ControlTypeId;
            }

            set
            {
                SetProperty(PropertyType.UIA_ControlTypePropertyId, value);
            }
        }

        public new Rectangle BoundingRectangle
        {
            get
            {
                return base.BoundingRectangle;
            }

            set
            {
                double[] _value = new double[4]
                {
                    value.Left,
                    value.Top,
                    value.Width,
                    value.Height
                };

                SetProperty(PropertyType.UIA_BoundingRectanglePropertyId, _value);
            }
        }

        public new string AutomationId
        {
            get
            {
                return base.AutomationId;
            }

            set
            {
                SetProperty(PropertyType.UIA_AutomationIdPropertyId, value);
            }
        }

        public new string ClassName
        {
            get
            {
                return base.ClassName;
            }

            set
            {
                SetProperty(PropertyType.UIA_ClassNamePropertyId, value);
            }
        }

        public new string Framework
        {
            get
            {
                return base.Framework;
            }

            set
            {
                SetProperty(PropertyType.UIA_FrameworkIdPropertyId, value);
            }
        }

        public new bool IsContentElement
        {
            get
            {
                return base.IsContentElement;
            }

            set
            {
                SetProperty(PropertyType.UIA_IsContentElementPropertyId, value);
            }
        }

        public new bool IsControlElement
        {
            get
            {
                return base.IsControlElement;
            }

            set
            {
                SetProperty(PropertyType.UIA_IsControlElementPropertyId, value);
            }
        }

        public new bool IsEnabled
        {
            get
            {
                return base.IsEnabled;
            }

            set
            {
                SetProperty(PropertyType.UIA_IsEnabledPropertyId, value);
            }
        }

        public new bool IsKeyboardFocusable
        {
            get
            {
                return base.IsKeyboardFocusable;
            }

            set
            {
                SetProperty(PropertyType.UIA_IsKeyboardFocusablePropertyId, value);
            }
        }

        public new bool IsOffScreen
        {
            get
            {
                return base.IsOffScreen;
            }

            set
            {
                SetProperty(PropertyType.UIA_IsOffscreenPropertyId, value);
            }
        }

        public new string ItemStatus
        {
            get
            {
                return base.ItemStatus;
            }

            set
            {
                SetProperty(PropertyType.UIA_ItemStatusPropertyId, value);
            }
        }

        public new OrientationType Orientation
        {
            get
            {
                return base.Orientation;
            }

            set
            {
                SetProperty(PropertyType.UIA_OrientationPropertyId, (int) value);
            }
        }

        public new int LandmarkType
        {
            get
            {
                return base.LandmarkType;
            }

            set
            {
                SetProperty(PropertyType.UIA_LandmarkTypePropertyId, value);
            }
        }

        public new string LocalizedLandmarkType
        {
            get
            {
                return base.LocalizedLandmarkType;
            }

            set
            {
                SetProperty(PropertyType.UIA_LocalizedLandmarkTypePropertyId, value);
            }
        }

        public new int HeadingLevel
        {
            get
            {
                return base.HeadingLevel;
            }

            set
            {
                SetProperty(PropertyType.UIA_HeadingLevelPropertyId, value);
            }
        }

        public new string HelpText
        {
            get
            {
                return base.HelpText;
            }

            set
            {
                SetProperty(PropertyType.UIA_HelpTextPropertyId, value);
            }
        }

        public new int PositionInSet
        {
            get
            {
                return base.PositionInSet;
            }

            set
            {
                SetProperty(PropertyType.UIA_PositionInSetPropertyId, value);
            }
        }

        public new int SizeOfSet
        {
            get
            {
                return base.SizeOfSet;
            }

            set
            {
                SetProperty(PropertyType.UIA_SizeOfSetPropertyId, value);
            }
        }
    } // class
} // namespace
