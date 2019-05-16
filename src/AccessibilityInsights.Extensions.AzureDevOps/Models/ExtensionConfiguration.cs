// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using AccessibilityInsights.Extensions.Helpers;
using Newtonsoft.Json;
using System;

namespace AccessibilityInsights.Extensions.AzureDevOps.Models
{
    public class ExtensionConfiguration
    {
        public int ZoomLevel { get; set; } = 100;

        public ConnectionInfo SavedConnection { get; set; }

        public ConnectionCache CachedConnections { get; set; }

        public string GetSerializedConfig() => JsonConvert.SerializeObject(this);

        public void LoadFromSerializedString(string serializedConfig)
        {
            try
            {
                var config = JsonConvert.DeserializeObject<ExtensionConfiguration>(serializedConfig);

                ZoomLevel = config.ZoomLevel;
                SavedConnection = config.SavedConnection;
                CachedConnections = config.CachedConnections;
            }
#pragma warning disable CA1031 // Do not catch general exception types
            catch (Exception e)
            {
                e.ReportException();
                // don't use serialized data if it can't be parsed
            }
#pragma warning restore CA1031 // Do not catch general exception types
        }
    }
}
