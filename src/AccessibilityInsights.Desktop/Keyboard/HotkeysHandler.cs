// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Interop;

namespace AccessibilityInsights.Desktop.Keyboard
{
    /// <summary>
    /// Hotkey handler 
    /// singleton
    /// this is not thread safe
    /// </summary>
    public class HotkeysHandler : IDisposable
    {
        readonly IntPtr hWnd;
        private HwndSource source;
        int idCount = 0;
        readonly List<Hotkey> HotKeyList = new List<Hotkey>();

        private HotkeysHandler(IntPtr hWnd)
        {
            this.hWnd = hWnd; 
            source = HwndSource.FromHwnd(hWnd);

            source.AddHook(HandleHotKeys);
        }

        public void RegisterHotKey(Hotkey hk)
        {
            if (Find(hk) == null)
            {
                hk.Id = idCount;
                this.HotKeyList.Add(hk);
                idCount++;

                hk.Register(hWnd);
            }
            // in case with matched one, silently exit
        }

        /// <summary>
        /// Find matched key in the existing list
        /// </summary>
        /// <param name="hk"></param>
        /// <returns></returns>
        private Hotkey Find(Hotkey hk)
        {
            return (from k in this.HotKeyList
                    where k.Key == hk.Key && k.Modifier == hk.Modifier
                    select k).FirstOrDefault();
        }

        /// <summary>
        /// Check whether the given hotkey combination exists or not.
        /// </summary>
        /// <param name="hk"></param>
        /// <returns></returns>
        public bool Exist(Hotkey hk)
        {
            return Find(hk) != null;
        }

        /// <summary>
        /// Find matched hot key by id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        private Hotkey Find(int id)
        {
            return (from k in this.HotKeyList
                    where k.Id == id
                    select k).FirstOrDefault();
        }

        /// <summary>
        /// HotKey handler 
        /// </summary>
        /// <param name="hwnd"></param>
        /// <param name="msg"></param>
        /// <param name="wParam"></param>
        /// <param name="lParam"></param>
        /// <param name="handled"></param>
        /// <returns></returns>
        private IntPtr HandleHotKeys(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
        {
            const int WM_HOTKEY = 0x0312;
            switch (msg)
            {
                case WM_HOTKEY:
                    var id = wParam.ToInt32();
                    var hk = Find(id);
                    hk.Action();
                    break;
            }
            return IntPtr.Zero;
        }

        #region IDisposable Support
        private bool disposedValue = false; // To detect redundant calls

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    foreach(var k in this.HotKeyList)
                    {
                        k.Unregister(this.hWnd);
                    }

                    this.HotKeyList.Clear();
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

        #region static code
        static HotkeysHandler singleton = null;

        public static HotkeysHandler GetHotkeyHandler(IntPtr hWnd)
        {
            if (singleton == null)
            {
                singleton = new HotkeysHandler(hWnd);
            }

            return singleton;
        }
        #endregion
    }
}
