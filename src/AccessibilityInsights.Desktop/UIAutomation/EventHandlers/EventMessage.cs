// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using Axe.Windows.Core.Bases;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using UIAutomationClient;

namespace Axe.Windows.Desktop.UIAutomation.EventHandlers
{
    /// <summary>
    /// Event Message Class for passing event information to UI or other handler
    /// </summary>
    public class EventMessage:IA11yEventMessage, IDisposable
    {
        public int EventId { get; set; }

        /// <summary>
        /// Time stamp with millisecond accuracy
        /// </summary>
        public string TimeStamp { get; set; }

#pragma warning disable CA2227 // Collection properties should be read only
        public List<KeyValuePair<string,dynamic>> Properties { get; set; }
#pragma warning restore CA2227 // Collection properties should be read only

        public A11yElement Element { get; set; }


        /// <summary>
        /// Event Message class
        /// to pass information between listener and handler of event
        /// </summary>
        /// <param name="id"></param>
        /// <param name="sender"></param>
        /// <param name="comment"></param>
        /// <param name="isError"></param>
        private EventMessage(int id, IUIAutomationElement sender)
        {
            TimeStamp = DateTime.Now.ToString("HH:mm:ss.fff", CultureInfo.InvariantCulture);
            this.EventId = id;
            this.Element = sender != null ? new DesktopElement(sender) : null;
        }

        /// <summary>
        /// Constructor for serialization
        /// </summary>
        public EventMessage() { }

        private string BuildMessage(string comment)
        {
            StringBuilder sb = new StringBuilder();

            if (this.Element != null)
            {
                sb.Append($"{this.Element.Glimpse}:{comment}");
            }
            else
            {
                sb.Append(comment);
            }

            return sb.ToString();
        }

        #region static members
        /// <summary>
        /// Get instance of EventMessage
        /// when sender is null, it is generally error case. 
        /// 
        /// if it returns null, it means that the sender is hosted in the process of running Recorder code. 
        /// </summary>
        /// <param name="id"></param>
        /// <param name="sender"></param>
        /// <param name="comment"></param>
        /// <param name="isError"></param>
        /// <returns></returns>
        public static EventMessage GetInstance(int id, IUIAutomationElement sender)
        {
            if (sender == null || !DesktopElement.IsFromCurrentProcess(sender))
            {
                    return new EventMessage(id, sender);
            }

            return null;
        }
        #endregion

        #region IDisposable Support
        private bool disposedValue = false; // To detect redundant calls

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    this.Element?.Dispose();
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
