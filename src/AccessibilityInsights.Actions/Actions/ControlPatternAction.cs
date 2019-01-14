// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using AccessibilityInsights.Actions.Attributes;
using AccessibilityInsights.Actions.Enums;
using AccessibilityInsights.Core.Bases;
using System.Linq;
using System.Reflection;

namespace AccessibilityInsights.Actions
{
    /// <summary>
    /// ControlAction class
    /// this class will allow you to run control pattern via Action. 
    /// it is a prep work for moving to UWP or JScript based Ux. 
    /// </summary>
    [InteractionLevel(UxInteractionLevel.NoUxInteraction)]
    public static class ControlPatternAction
    {
        /// <summary>
        /// Run Action method of a Pattern
        /// </summary>
        /// <param name="ecId">ElementContext Id</param>
        /// <param name="eId">Element Id</param>
        /// <param name="ptId">A11yPattern Id</param>
        /// <param name="mname">method name</param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public static dynamic RunAction(int eId, int ptId, string mname, object[] parameters)
        {
            var ecId = SelectAction.GetDefaultInstance().GetSelectedElementContextId();
            A11yPattern ptn = DataManager.GetDefaultInstance().GetA11yPattern(ecId.Value, eId, ptId);

            MethodInfo mi = ptn.Methods.Where(m => m.Name == mname).First();

            return mi.Invoke(ptn, parameters);
        }
    }
}
