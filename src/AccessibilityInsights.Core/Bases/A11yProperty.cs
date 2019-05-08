// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using Axe.Windows.Core.Types;
using System;
using Axe.Windows.Core.Misc;
using Newtonsoft.Json;
using AccessibilityInsights.Win32;

namespace Axe.Windows.Core.Bases
{
    /// <summary>
    /// Wrapper class for UIAutomationElement Property
    /// </summary>
    public class A11yProperty:IDisposable
    {
        /// <summary>
        /// Property Id
        /// </summary>
        public int Id { get; set; }
        /// <summary>
        /// Property Name
        /// </summary>
        public string Name { get; set; }

#pragma warning disable CA1051 // Do not declare visible instance fields
        /// <summary>
        /// Property value
        /// because it is used in referenced variable later, it can't be property. 
        /// please keep it as field. 
        /// 
        /// CA1051 because of backward compat issue with loading existing results file, can't change it to field.
        /// </summary>
        public dynamic Value;
#pragma warning restore CA1051 // Do not declare visible instance fields

        [JsonIgnore]
        public string TextValue
        {
            get
            {
                return this.ToString();
            }
        }

        /// <summary>
        /// Constructor with normal case
        /// </summary>
        /// <param name="id"></param>
        /// <param name="element"></param>
        public A11yProperty (int id, dynamic value,string name = null): this(id,name)
        {
            this.Value = value;
        }

        /// <summary>
        /// private constructor 
        /// </summary>
        /// <param name="id"></param>
        /// <param name="name">if null, it is get name from PropertyTypes by id</param>
        private A11yProperty(int id, string name)
        {
            this.Id = id;
            this.Name = name ?? PropertyType.GetInstance().GetNameById(id);
        }

        /// <summary>
        /// Constructor for serialization
        /// </summary>
        public A11yProperty() { }

        public override string ToString()
        {
            string txt = null;

            if (this.Value != null)
            {
                switch (this.Id)
                {
                    case PropertyType.UIA_RuntimeIdPropertyId:
                        txt = this.ConvertIntArrayToString();
                        break;
                    case PropertyType.UIA_ControlTypePropertyId:
                        txt = this.Value != null ? ControlType.GetInstance().GetNameById(this.Value) : "";
                        break;
                    case PropertyType.UIA_BoundingRectanglePropertyId:
                        // if bounding rectangle is [0,0,0,0], treat it as non-exist. same behavior as Inspect
                        txt = GetBoundingRectangleText();
                        break;
                    case PropertyType.UIA_OrientationPropertyId:
                        switch ((int)this.Value)
                        {
                            case 0: //OrientationType_None
                                txt = "None(0)";
                                break;
                            case 1: //OrientationType_Horizontal
                                txt = "Horizontal(1)";
                                break;
                            case 2: // OrientationType_Vertical
                                txt = "Vertical(2)";
                                break;
                        }
                        break;
                    case PropertyType.UIA_PositionInSetPropertyId:
                    case PropertyType.UIA_LevelPropertyId:
                    case PropertyType.UIA_SizeOfSetPropertyId:
                        /// these properties are 1 based.
                        /// if value is smaller than 1, it should be ignored. 
                        if(this.Value != null && this.Value > 0)
                        {
                            txt = this.Value?.ToString();
                        }
                        break;
                    case PropertyType.UIA_HeadingLevelPropertyId:
                        txt = HeadingLevelType.GetInstance().GetNameById(this.Value);
                        break;
                    case PropertyType.UIA_LandmarkTypePropertyId:
                        txt = this.Value != 0 ? LandmarkType.GetInstance().GetNameById(this.Value) : null; // 0 is default value. 
                        break;
                    default:
                        if(this.Value is Int32[])
                        {
                            txt = ((Int32[])this.Value).ConvertInt32ArrayToString();
                        }
                        else if(this.Value is Double[])
                        {
                            txt = ((Double[])this.Value).ConvertDoubleArrayToString();
                        }
                        else
                        {
                            txt = this.Value?.ToString();
                        }
                        break;
                }
            }
            return txt;
        }

        /// <summary>
        /// Get proper bounding rectangle text based on teh date
        /// </summary>
        /// <returns></returns>
        private string GetBoundingRectangleText()
        {
            string text = null;
            var arr = this.Value;

            if ((double)arr[2] < 0 || (double)arr[3] < 0)
            {
                // the 3rd and 4th values in array are negative value, we need to show value in different format like l,t,w,h
                text = string.Format("[l={0},t={1},w={2},h={3}]", arr[0], arr[1], arr[2], arr[3]);
            }
            else
            {
                text = this.ToRectangle().IsEmpty == false ? this.ToRectangle().ToLeftTopRightBottomString() : null;
            }

            return text;
        }

        #region IDisposable Support
        private bool disposedValue = false; // To detect redundant calls

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    this.Name = null;
                    if (this.Value != null)
                    {
                        if(NativeMethods.VariantClear(ref this.Value) == Win32Constants.S_OK)
                        {
                            this.Value = null;
                        }
                    }
                }

                disposedValue = true;
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
