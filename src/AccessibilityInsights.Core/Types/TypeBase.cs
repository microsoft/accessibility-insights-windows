// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;
using System.Collections.Generic;

using static System.FormattableString;

namespace Axe.Windows.Core.Types
{
    /// <summary>
    /// Base class for all Types(ControlPattern, Event, Property...)
    /// </summary>
    public abstract class TypeBase
    {
        const string NamePattern = "UIA_";
        protected Dictionary<int, string> Dic { get; private set; }

        protected TypeBase() : this(NamePattern) { }

        protected TypeBase(string np)
        {
            this.Dic = new Dictionary<int, string>();

            PopulateDictionary(np);
        }

        /// <summary>
        /// Populate dictionary based on const members of inherited class
        /// </summary>
        private void PopulateDictionary(string namepattern)
        {
            var fields =  this.GetType().GetFields(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.FlattenHierarchy );

            foreach(var f in fields)
            {
                if (f.Name.StartsWith(namepattern, StringComparison.Ordinal))
                {
                    int id = (int)f.GetValue(f);
                    this.Dic.Add(id, GetNameInProperFormat(f.Name, id));
                }
            }

        }

        /// <summary>
        /// get name in right pattern
        /// </summary>
        /// <param name="name"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        protected virtual string GetNameInProperFormat(string name,int id)
        {
            return name;
        }

        /// <summary>
        /// check whether the contant should be part of List
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        protected virtual bool IsPartOfKeyValuePairList(int id)
        {
            return true;
        }

        /// <summary>
        /// Get the full list of known types in List of KeyValuePairs
        /// </summary>
        public List<KeyValuePair<int, string>> GetKeyValuePairList()
        {
            var list = new List<KeyValuePair<int, string>>();

            foreach (var k in this.Dic.Keys)
            {
                if(IsPartOfKeyValuePairList(k))
                {
                    list.Add(new KeyValuePair<int, string>(k, this.Dic[k]));
                }
            }

            return list;
        }

        /// <summary>
        /// Get Name of Type by Id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public string GetNameById(int id)
        {
            if (this.Dic.ContainsKey(id))
            {
                return this.Dic[id];
            }

            return Invariant($"Unknown({id})");
        }

        /// <summary>
        /// Get Name of Type by Id (Long)
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public string GetNameById(long id)
        {
            return GetNameById(Convert.ToInt32(id));
        }

        /// <summary>
        /// Check whether Id exist or not. 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public bool Exists(int id)
        {
            return this.Dic.ContainsKey(id);
        }

        /// <summary>
        /// the values of the types contained in the inheriting class
        /// </summary>
        public IEnumerable<int> Values => Dic.Keys;
    }
}
