// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
namespace Axe.Windows.Desktop.Types
{
    /// <summary>
    /// Defines values that specify the scope of elements within the automation tree
    /// that should be listened to for events
    /// </summary>
    public enum ListenScope
    {
        None,
        Element,
        Descendants,
        Subtree,
    }
}
