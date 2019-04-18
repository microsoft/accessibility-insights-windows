// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using Axe.Windows.Rules.PropertyConditions;
using System.Collections.Generic;
using System.Reflection;

namespace Axe.Windows.Rules
{
    /// <summary>
    /// Remove this class from Rules assembly
    /// </summary>
    public class CCAControlTypesFilter
    {
        private static CCAControlTypesFilter DefaultInstance;
        private readonly HashSet<int> MainTypes = new HashSet<int>();

        /// <summary>
        /// GetDefaultInstance
        /// </summary>
        /// <returns></returns>
        public static CCAControlTypesFilter GetDefaultInstance()
        {
            if (DefaultInstance == null)
            {
                DefaultInstance = new CCAControlTypesFilter();
                DefaultInstance.Filter();
            }

            return DefaultInstance;
        }

        private void Filter()
        {
            var type = typeof(ControlType);
            var fields = type.GetFields(BindingFlags.GetField | BindingFlags.Public | BindingFlags.Static);

            foreach (FieldInfo field in fields)
            {
                    MainTypes.Add(((ControlTypeCondition)field.GetValue(field)).ControlType);
            }
        }

        /// <summary>
        /// Remove this class from Rules assembly
        /// </summary>
        /// <param name="typeId"></param>
        /// <returns></returns>
        public bool Contains(int typeId)
        {
            return MainTypes.Contains(typeId);
        }
}


}
