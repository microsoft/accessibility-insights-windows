// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using Axe.Windows.Core.Attributes;
using Axe.Windows.Core.Misc;
using Axe.Windows.Core.Types;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Axe.Windows.Core.Bases
{
    /// <summary>
    /// Control pattern class
    /// </summary>
    public class A11yPattern : IA11yPattern, IDisposable
    {
        /// <summary>
        /// Property name
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// Property Id
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Pattern properties list
        /// CA2227 exemption since Properties should be serialized in results file.
        /// </summary>
#pragma warning disable CA2227 // Collection properties should be read only
        public List<A11yPatternProperty> Properties { get; set; }
#pragma warning restore CA2227 // Collection properties should be read only
        
        /// <summary>
        /// indicate whether it is actionable or not.
        /// </summary>
        public bool IsUIActionable { get; set; }

        /// <summary>
        /// Element which this pattern is from. 
        /// hide from serialization
        /// </summary>
        [JsonIgnore]
        public A11yElement Element { get; private set; }

        /// <summary>
        /// Pattern method list
        /// </summary>
        [JsonIgnore]
        public List<MethodInfo> Methods { get; private set; }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="e"></param>
        public A11yPattern(A11yElement e, int id): this(e,id, PatternType.GetInstance().GetNameById(id)) { }

        /// <summary>
        /// Base class constructor for unknown pattern type.
        /// </summary>
        /// <param name="e"></param>
        /// <param name="id"></param>
        /// <param name="name"></param>
        public A11yPattern(A11yElement e, int id,string name)
        {
            this.Element = e;
            this.Id = id;
            this.Name = name;
            this.IsUIActionable = this.IsUIActionablePatternByPatternMethodType();
            this.Properties = new List<A11yPatternProperty>();
            this.Methods = GetListOfPatternMethods();
        }

        /// <summary>
        /// Get the list of pattern methods.
        /// </summary>
        /// <returns>if there is no method with "PatternmethodAttribute", return null</returns>
        private List<MethodInfo> GetListOfPatternMethods()
        {
            return (from m in this.GetType().GetMethods()
                    let a = m.GetCustomAttribute(typeof(PatternMethodAttribute))
                    where a != null
                    select m).ToList();
        }

        /// <summary>
        /// Constructor for deserialization
        /// </summary>
        public A11yPattern() { }

        public T GetValue<T>(string name)
        {
            if (this.Properties == null) return default(T);

            var item = this.Properties.Find(p => p.Name == name);
            if (item == null) return default(T);

            if (item.Value is T)
                return item.Value;

            // allow integer types to be cast
            // Otherwise, types don't match.
            if (!IsNumber(item.Value)
                || !IsNumber<T>())
                return default(T);

            // the return value will be an int of some kind, but it may require the cast not to throw and exception
            return (T) item.Value;
        }

        private static bool IsNumber(dynamic d)
        {
            if (d == null) return false;

            return IsNumber(Type.GetTypeCode(d.GetType()));
        }

        static bool IsNumber<T>()
        {
            return IsNumber(Type.GetTypeCode(typeof(T)));
        }

        static bool IsNumber(TypeCode code)
        {
            switch (code)
            {
                case TypeCode.Decimal:
                case TypeCode.Double:
                case TypeCode.Int16:
                case TypeCode.Int32:
                case TypeCode.Int64:
                case TypeCode.Single:
                case TypeCode.UInt16:
                case TypeCode.UInt32:
                case TypeCode.UInt64:
                    return true;
            }

            return false;
        }

        public override string ToString()
        {
            return string.Format("{0}: {1}", this.Name, this.Properties.FirstOrDefault()?.Value);
        }

        #region IDisposable Support
        private bool disposedValue = false; // To detect redundant calls

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    this.Properties.ForEach(p => p.Dispose());
                    this.Properties?.Clear();
                    this.Properties = null;
                    this.Name = null;
                    this.Element = null;
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
