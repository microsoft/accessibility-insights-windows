// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;
using System.Windows;
using System.Windows.Resources;

namespace AccessibilityInsights.CommonUxComponents.Utilities
{
    /// <summary>
    /// Provides access to resources contained in CommonUxComponents. Both CommonUxComponents as well as
    /// outside assemblies should go through this class to programmatically access these resources.
    /// </summary>
    public static class SharedResources
    {
        private static readonly Uri FabricIconFontUri = new Uri(@"pack://application:,,,/AccessibilityInsights.CommonUxComponents;component/Resources/FabMDL2.ttf");

        public static StreamResourceInfo FabricIconFontResource => Application.GetResourceStream(FabricIconFontUri);
    }
}
