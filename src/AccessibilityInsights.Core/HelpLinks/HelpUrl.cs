// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using Axe.Windows.Core.Enums;

namespace Axe.Windows.Core.HelpLinks
{
    /// <summary>
    /// HelpUrl class
    /// </summary>
    public class HelpUrl
    {
#pragma warning disable CA1056 // Uri properties should not be strings
        public string Url { get; set; }
#pragma warning restore CA1056 // Uri properties should not be strings

        public UrlType Type { get; set; }
    }
}
