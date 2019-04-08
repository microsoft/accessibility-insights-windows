// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using Axe.Windows.Actions.Attributes;
using Axe.Windows.Actions.Contexts;
using Axe.Windows.Actions.Enums;
using Axe.Windows.Core.Bases;
using Axe.Windows.Core.Misc;
using Axe.Windows.Desktop.UIAutomation;
using System;

namespace Axe.Windows.Actions
{
    /// <summary>
    /// GetDataAction class
    /// Get Data from DataManager
    /// </summary>
    [InteractionLevel(UxInteractionLevel.NoUxInteraction)]
    public static class GetDataAction
    {
        /// <summary>
        /// Get element Context from Action DataManager
        /// </summary>
        /// <param name="ecId"></param>
        /// <returns></returns>
        static public ElementContext GetElementContext(Guid ecId)
        {
            return DataManager.GetDefaultInstance().GetElementContext(ecId);
        }

        /// <summary>
        /// Checking whether there is EC with the indicated Id
        /// </summary>
        /// <param name="ecId"></param>
        /// <returns></returns>
        static public bool ExistElementContext(Guid ecId)
        {
            return DataManager.GetDefaultInstance().GetElementContext(ecId) != null;
        }

        /// <summary>
        /// Get the Id of Selected element of ElementContext(Id)
        /// </summary>
        /// <param name="ecId">Element context Id</param>
        /// <returns></returns>
        static public int GetSelectedElementId(Guid ecId)
        {
            return DataManager.GetDefaultInstance().GetElementContext(ecId).Element.UniqueId;
        }

        /// <summary>
        /// Get data context of ElementContext
        /// </summary>
        /// <param name="ecId"></param>
        /// <returns></returns>
        static public ElementDataContext GetElementDataContext(Guid ecId)
        {
            return DataManager.GetDefaultInstance().GetElementContext(ecId)?.DataContext;
        }

        /// <summary>
        /// Get Instance of A11yElement with live data
        /// for now, it updates data in input A11yElement and return it. 
        /// ToDo: it will be changed along with ActionDataManager code. 
        /// </summary>
        /// <param name="ecId">ElementContext Id</param>
        /// <param name="eId">Element Id</param>
        /// <returns></returns>
        public static A11yElement GetA11yElementWithLiveData(Guid ecId, int eId)
        {
            var e = DataManager.GetDefaultInstance().GetA11yElement(ecId, eId);

            e?.PopulateAllPropertiesWithLiveData();

            return e;
        }

        /// <summary>
        /// Get Instance of A11yElement with data in Data context
        /// </summary>
        /// <param name="ecId">ElementContext Id</param>
        /// <param name="eId">Element Id</param>
        /// <returns></returns>
        public static A11yElement GetA11yElementInDataContext(Guid ecId, int eId)
        {
            return DataManager.GetDefaultInstance().GetA11yElement(ecId, eId);
        }

        /// <summary>
        /// Get Process name and Ui Framework of Element Context
        /// </summary>
        /// <param name="ecId"></param>
        /// <returns></returns>
        public static Tuple<string,string> GetProcessAndUIFrameworkOfElementContext(Guid ecId)
        {
            var ec = DataManager.GetDefaultInstance().GetElementContext(ecId);

            return new Tuple<string, string>(ec.ProcessName, ec.Element.GetUIFramework());
        }

        /// <summary>
        /// Get the DataContext Mode of currently selected ElementContext
        /// </summary>
        /// <returns></returns>
        public static DataContextMode GetDataContextMode()
        {
            var ec = DataManager.GetDefaultInstance().GetElementContext(SelectAction.GetDefaultInstance().GetSelectedElementContextId().Value);

            return ec.DataContext != null ? ec.DataContext.Mode : DataContextMode.Live;
        }
    }
}
