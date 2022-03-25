// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
namespace AccessibilityInsights.SharedUx.ViewModels
{
    /// <summary>
    /// Keep the status of ToolBarButton
    /// </summary>
    public class TwoStateButtonViewModel : ViewModelBase
    {
        private ButtonState state = ButtonState.On;
        public ButtonState State
        {
            get
            {
                return state;
            }

            set
            {
                this.state = value;
                OnPropertyChanged(nameof(State));
            }
        }

        public TwoStateButtonViewModel(ButtonState state)
        {
            this.State = state;
        }

        public void FlipButtonState()
        {
            if (this.State == ButtonState.On)
            {
                this.State = ButtonState.Off;
            }
            else
            {
                this.State = ButtonState.On;
            }
        }
    }

    public enum ButtonState
    {
        On,
        Off
    }
}
