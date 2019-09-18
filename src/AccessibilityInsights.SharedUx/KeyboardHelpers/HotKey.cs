// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using AccessibilityInsights.Win32;
using System;
using System.Linq;
using System.Windows.Forms;

namespace AccessibilityInsights.SharedUx.KeyboardHelpers
{
    /// <summary>
    /// Class for hot key
    /// </summary>
    public class HotKey
    {
        /// <summary>
        /// this value will be set by Hotkey Handler
        /// </summary>
        public int Id { get; internal set; }

        public Keys Key { get; internal set; }
        public HotkeyModifier Modifier { get; internal set; }

        public Action Action { get; private set; }

        public uint ErrorCode { get; private set; }

        /// <summary>
        /// Register hot key
        /// </summary>
        /// <param name="hWnd"></param>
        public bool Register(IntPtr hWnd)
        {
            this.ErrorCode = 0;

            if (NativeMethods.RegisterHotKey(hWnd, Id, (int)this.Modifier, Key.GetHashCode()) == false)
            {
                ErrorCode = NativeMethods.GetLastError();
                return false;
            }

            return true;
        }

        /// <summary>
        /// Unregister HotKey
        /// </summary>
        /// <param name="hWnd"></param>
        public bool Unregister(IntPtr hWnd)
        {
            this.ErrorCode = 0;

            if (NativeMethods.UnregisterHotKey(hWnd, Id) == false)
            {
                ErrorCode = NativeMethods.GetLastError();
                return false;
            }

            return true;
        }

        /// <summary>
        /// Get an instance of HotKey based on given string
        /// </summary>
        /// <param name="txt"></param>
        /// <returns></returns>
        public static HotKey GetInstance(string txt)
        {
            if (txt == null)
                throw new ArgumentNullException(nameof(txt));

            var atoms = from a in txt.Split(new char[] { '+' }, StringSplitOptions.RemoveEmptyEntries)
                        select a.Trim();
            var hk = new HotKey();

            try
            {
                if (atoms.Count() == 2)
                {
                    hk.Modifier = GetModifier(atoms.ElementAt(0));

                    hk.Key = GetKey(atoms.ElementAt(1));
                }
                else
                {
                    throw new ArgumentException($"Hotkey format is not Modifier + Key: {txt}");
                }
            }
#pragma warning disable CA1031 // Do not catch general exception types
            catch
            {
                return null;
            }
#pragma warning restore CA1031 // Do not catch general exception types

            return hk;
        }

        private static Keys GetKey(string a)
        {
            KeysConverter kc = new KeysConverter();
            return (Keys)kc.ConvertFromInvariantString(a.ToUpperInvariant());
        }

        /// <summary>
        /// Get Modifier value from given text
        /// </summary>
        /// <param name="mods">modifiers. comma separated</param>
        /// <returns></returns>
        private static HotkeyModifier GetModifier(string mods)
        {
            var fms = from m in mods.Split(',')
                      select $"MOD_{m.Trim().ToUpperInvariant()}";

            HotkeyModifier result = HotkeyModifier.MOD_NoModifier;

            foreach (var fm in fms)
            {
                if (Enum.TryParse<HotkeyModifier>(fm, out HotkeyModifier tmp) == false)
                {
                    result = HotkeyModifier.MOD_NoModifier;
                    break;
                }
                else
                {
                    if (result == HotkeyModifier.MOD_NoModifier)
                    {
                        result = tmp;
                    }
                    else
                    {
                        result |= tmp;
                    }

                }
            }

            return result;
        }

        public void SetConditionalAction(Action action, Func<bool> condition)
        {
            Action = () =>
            {
                if (condition == null || condition())
                {
                    action();
                }
            };
        }
    }
}
