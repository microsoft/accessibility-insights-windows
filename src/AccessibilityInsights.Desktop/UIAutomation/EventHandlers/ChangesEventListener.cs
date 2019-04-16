// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using Axe.Windows.Desktop.Types;
using UIAutomationClient;
using TreeScope = UIAutomationClient.TreeScope;

namespace Axe.Windows.Desktop.UIAutomation.EventHandlers
{
    /// <summary>
    /// Changes event listener. 
    /// It is place holder. it is not hooked up yet since AccEvent doesn't support it yet. 
    /// </summary>
    public class ChangesEventListener : EventListenerBase, IUIAutomationChangesEventHandler
    {
        private static int[] ChangeTypes = new int[]
        {
            EventType.UIA_ToolTipOpenedEventId,
            EventType.UIA_ToolTipClosedEventId,
            EventType.UIA_StructureChangedEventId,
            EventType.UIA_MenuOpenedEventId,
            EventType.UIA_AutomationPropertyChangedEventId,
            EventType.UIA_AutomationFocusChangedEventId,
            EventType.UIA_AsyncContentLoadedEventId,
            EventType.UIA_MenuClosedEventId,
            EventType.UIA_LayoutInvalidatedEventId,
            EventType.UIA_Invoke_InvokedEventId,
            EventType.UIA_SelectionItem_ElementAddedToSelectionEventId,
            EventType.UIA_SelectionItem_ElementRemovedFromSelectionEventId,
            EventType.UIA_SelectionItem_ElementSelectedEventId,
            EventType.UIA_Selection_InvalidatedEventId,
            EventType.UIA_Text_TextSelectionChangedEventId,
            EventType.UIA_Text_TextChangedEventId,
            EventType.UIA_Window_WindowOpenedEventId,
            EventType.UIA_Window_WindowClosedEventId,
            EventType.UIA_MenuModeStartEventId,
            EventType.UIA_MenuModeEndEventId,
            EventType.UIA_InputReachedTargetEventId,
            EventType.UIA_InputReachedOtherElementEventId,
            EventType.UIA_InputDiscardedEventId,
            EventType.UIA_SystemAlertEventId,
            EventType.UIA_LiveRegionChangedEventId,
            EventType.UIA_HostedFragmentRootsInvalidatedEventId,
            EventType.UIA_Drag_DragStartEventId,
            EventType.UIA_Drag_DragCancelEventId,
            EventType.UIA_Drag_DragCompleteEventId,
            EventType.UIA_DropTarget_DragEnterEventId,
            EventType.UIA_DropTarget_DragLeaveEventId,
            EventType.UIA_DropTarget_DroppedEventId,
            EventType.UIA_TextEdit_TextChangedEventId,
            EventType.UIA_TextEdit_ConversionTargetChangedEventId,
            EventType.UIA_ChangesEventId,
            EventType.UIA_NotificationEventId,
            EventType.UIA_ActiveTextPositionChangedEventId,
        };

        /// <summary>
        /// Create an event handler and register it.
        /// </summary>
        public ChangesEventListener(CUIAutomation8 uia8, IUIAutomationElement element, TreeScope scope, HandleUIAutomationEventMessage peDelegate) : base(uia8, element, scope, EventType.UIA_NotificationEventId, peDelegate)
        {
            Init();
        }

        public override void Init()
        {
            IUIAutomation4 uia4 = this.IUIAutomation4;
            if (uia4 != null)
            {
                uia4.AddChangesEventHandler(this.Element, this.Scope, ref ChangeTypes[0], ChangeTypes.Length, null, this);
                this.IsHooked = true;
            }
        }

        public void HandleChangesEvent(IUIAutomationElement sender, ref UiaChangeInfo uiaChanges, int changesCount)
        {
            var m = EventMessage.GetInstance(this.EventId, sender);

            if (m != null)
            {
                this.ListenEventMessage(m);
            }
        }

        protected override void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    if (this.IsHooked)
                    {
                        IUIAutomation4 uia4 = this.IUIAutomation4;
                        if (uia4 != null)
                        {
                            uia4.RemoveChangesEventHandler(this.Element, this);
                        }
                    }
                }
            }
            base.Dispose(disposing);
        }
    }
}
