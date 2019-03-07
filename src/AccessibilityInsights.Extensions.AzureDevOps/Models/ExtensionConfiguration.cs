// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Newtonsoft.Json;

namespace AccessibilityInsights.Extensions.AzureDevOps.Models
{
    public class ExtensionConfiguration
    {
        public int ZoomLevel { get; set; }

        public ConnectionInfo SavedConnection { get; set; }

        public ConnectionCache CachedConnections { get; set; }

        public string SerializedConfig => JsonConvert.SerializeObject(this);

        public void LoadFromSerializedString(string serializedConfig)
        {
            var config = JsonConvert.DeserializeObject<ExtensionConfiguration>(serializedConfig);

            ZoomLevel = config.ZoomLevel;
            SavedConnection = config.SavedConnection;
            CachedConnections = config.CachedConnections;
        }
    }
}
