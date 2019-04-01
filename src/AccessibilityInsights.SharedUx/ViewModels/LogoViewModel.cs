// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
namespace AccessibilityInsights.SharedUx.ViewModels
{
    /// <summary>
    /// View model for the issue reporter logo
    /// </summary>
    public class LogoViewModel : ViewModelBase
    {
        private string _fabricIconLogoName;
        public string FabricIconLogoName
        {
            get
            {
                return _fabricIconLogoName;
            }
            set
            {
                _fabricIconLogoName = value;
                OnPropertyChanged(nameof(this.FabricIconLogoName));
            }
        }
    }
}
