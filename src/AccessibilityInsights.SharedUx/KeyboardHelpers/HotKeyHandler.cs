// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Interop;

namespace AccessibilityInsights.SharedUx.KeyboardHelpers
{
    /// <summary>
    /// Hotkey handler
    /// singleton
    /// this is not thread safe.
    /// </summary>
    public class HotKeyHandler
    {
        readonly IntPtr hWnd;
        private readonly HwndSource source;
        int idCount;
        readonly List<HotKey> HotKeyList = new List<HotKey>();

        private HotKeyHandler(IntPtr hWnd)
        {
            this.hWnd = hWnd;
            source = HwndSource.FromHwnd(hWnd);
        }

        ~HotKeyHandler()
        {
            ClearHotkeys();
        }

        public void RegisterHotKey(HotKey hk)
        {
            if (hk == null)
                throw new ArgumentNullException(nameof(hk));

            if (Find(hk) == null)
            {
                // Add the hook if needed
                if (!HotKeyList.Any())
                {
                    source.AddHook(HandleHotKeys);
                }

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
        private HotKey Find(HotKey hk)
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
        public bool Exist(HotKey hk)
        {
            return Find(hk) != null;
        }

        /// <summary>
        /// Clear the Hotkeys
        /// </summary>
        public void ClearHotkeys()
        {
            foreach (var k in this.HotKeyList)
            {
                k.Unregister(this.hWnd);
            }

            this.HotKeyList.Clear();
            source.RemoveHook(HandleHotKeys);
        }

        /// <summary>
        /// Find matched hot key by id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        private HotKey Find(int id)
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

        #region static code
        static HotKeyHandler singleton;

        public static HotKeyHandler GetHotkeyHandler(IntPtr hWnd)
        {
            if (singleton == null)
            {
                singleton = new HotKeyHandler(hWnd);
            }

            return singleton;
        }
        #endregion
    }
}
