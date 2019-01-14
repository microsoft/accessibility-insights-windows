// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using AccessibilityInsights.Core.Types;
using System.Text;

namespace AccessibilityInsights.Desktop.Types
{
    public class EventType : TypeBase
    {
#pragma warning disable CA1707 // Identifiers should not contain underscores
        public const int UIA_EventRecorderNotificationEventId = 0; // just for internal use. it is to notify message other than actual event message

        /// <summary>
        /// Events from UIAutomationClient.H
        /// </summary>
        public const int UIA_ToolTipOpenedEventId = 20000;
        public const int UIA_ToolTipClosedEventId = 20001;
        public const int UIA_StructureChangedEventId = 20002;
        public const int UIA_MenuOpenedEventId = 20003;
        public const int UIA_AutomationPropertyChangedEventId = 20004;
        public const int UIA_AutomationFocusChangedEventId = 20005;
        public const int UIA_AsyncContentLoadedEventId = 20006;
        public const int UIA_MenuClosedEventId = 20007;
        public const int UIA_LayoutInvalidatedEventId = 20008;
        public const int UIA_Invoke_InvokedEventId = 20009;
        public const int UIA_SelectionItem_ElementAddedToSelectionEventId = 20010;
        public const int UIA_SelectionItem_ElementRemovedFromSelectionEventId = 20011;
        public const int UIA_SelectionItem_ElementSelectedEventId = 20012;
        public const int UIA_Selection_InvalidatedEventId = 20013;
        public const int UIA_Text_TextSelectionChangedEventId = 20014;
        public const int UIA_Text_TextChangedEventId = 20015;
        public const int UIA_Window_WindowOpenedEventId = 20016;
        public const int UIA_Window_WindowClosedEventId = 20017;
        public const int UIA_MenuModeStartEventId = 20018;
        public const int UIA_MenuModeEndEventId = 20019;
        public const int UIA_InputReachedTargetEventId = 20020;
        public const int UIA_InputReachedOtherElementEventId = 20021;
        public const int UIA_InputDiscardedEventId = 20022;
        public const int UIA_SystemAlertEventId = 20023;
        public const int UIA_LiveRegionChangedEventId = 20024;
        public const int UIA_HostedFragmentRootsInvalidatedEventId = 20025;
        public const int UIA_Drag_DragStartEventId = 20026;
        public const int UIA_Drag_DragCancelEventId = 20027;
        public const int UIA_Drag_DragCompleteEventId = 20028;
        public const int UIA_DropTarget_DragEnterEventId = 20029;
        public const int UIA_DropTarget_DragLeaveEventId = 20030;
        public const int UIA_DropTarget_DroppedEventId = 20031;
        public const int UIA_TextEdit_TextChangedEventId = 20032;
        public const int UIA_TextEdit_ConversionTargetChangedEventId = 20033;
        public const int UIA_ChangesEventId = 20034;
        public const int UIA_NotificationEventId = 20035; // this is available from Win10 RS3
        public const int UIA_ActiveTextPositionChangedEventId = 20036; // Available from Win10 RS5
#pragma warning restore CA1707 // Identifiers should not contain underscores

        private static EventType sInstance;

        /// <summary>
        /// static method to get an instance of this class
        /// singleton
        /// </summary>
        /// <returns></returns>
        public static EventType GetInstance()
        {
            if (sInstance == null)
            {
                sInstance = new EventType();
            }

            return sInstance;
        }

        /// <summary>
        /// private constructor since it would be singleton model
        /// </summary>
        private EventType() : base() { }

        /// <summary>
        /// change name into right format in dictionary and list.
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        protected override string GetNameInProperFormat(string name, int id)
        {
            StringBuilder sb = new StringBuilder(name);

            sb.Replace("UIA_", "");
            sb.Replace("EventId", "");

            return sb.ToString();
        }

        /// <summary>
        /// Exclude FocusChanged and PropertyChanged types from List
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        protected override bool IsPartOfKeyValuePairList(int id)
        {
            switch(id)
            {
                case UIA_AutomationFocusChangedEventId:
                case UIA_AutomationPropertyChangedEventId:
                case UIA_EventRecorderNotificationEventId:
                    return false;
                default:
                    return true;
            }
        }
    }
}
