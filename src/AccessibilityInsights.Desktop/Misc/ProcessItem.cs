// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;
using System.Diagnostics;

using static System.FormattableString;

namespace Axe.Windows.Desktop.Misc
{
    public class ProcessItem
    {
        public IntPtr HWnd { get; private set; }
        public int ProcessID { get; private set; }
        public string MainWindowTitle { get; private set; }

        public ProcessItem(Process p)
        {
            this.HWnd = p.MainWindowHandle;
            this.ProcessID = p.Id;
            this.MainWindowTitle = p.MainWindowTitle;
        }

        public override string ToString()
        {
            return Invariant($"{ProcessID}:{MainWindowTitle}");
        }
    }
}
