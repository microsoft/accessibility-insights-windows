// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
namespace AccessibilityInsights.Extensions.AzureDevOps.Models
{
    /// <summary>
    /// View model for a byte array, currently used for avatar image
    /// </summary>
    public class ByteArrayViewModel : ViewModelBase
    {
        private byte[] array;
#pragma warning disable CA1819 // Properties should not return arrays
        public byte[] ByteData
#pragma warning restore CA1819 // Properties should not return arrays
        {
            get
            {
                return array;
            }
            set
            {
                array = value;
                OnPropertyChanged(nameof(this.ByteData));
            }
        }
    }
}
