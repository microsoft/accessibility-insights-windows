// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;
using System.Windows;
using System.Windows.Media.Animation;

namespace AccessibilityInsights.SharedUx.Animations
{
    /// <summary>
    /// Animation class for GridLength type
    /// Based on https://social.msdn.microsoft.com/Forums/vstudio/en-US/da47a4b8-4d39-4d6e-a570-7dbe51a842e4/gridlengthanimation
    /// </summary>
    public class GridLengthAnimation : AnimationTimeline
    {
        /// <summary>
        /// Constructor
        /// </summary>
        public GridLengthAnimation() { }

        /// <summary>
        /// Initial value
        /// </summary>
        public GridLength From
        {
            get
            {
                return (GridLength)GetValue(FromProperty);
            }
            set
            {
                SetValue(FromProperty, value);
            }
        }

        public static readonly DependencyProperty FromProperty =
          DependencyProperty.Register("From", typeof(GridLength), typeof(GridLengthAnimation));

        /// <summary>
        /// Destination value
        /// </summary>
        public GridLength To
        {
            get
            {
                return (GridLength)GetValue(ToProperty);
            }
            set
            {
                SetValue(ToProperty, value);
            }
        }

        public static readonly DependencyProperty ToProperty =
            DependencyProperty.Register("To", typeof(GridLength), typeof(GridLengthAnimation));

        public override Type TargetPropertyType
        {
            get
            {
                return typeof(GridLength);
            }
        }

        protected override Freezable CreateInstanceCore()
        {
            return new GridLengthAnimation();
        }

        /// <summary>
        /// Calculates gridlength based on animation clock progress
        /// </summary>
        public override object GetCurrentValue(object defaultOriginValue, object defaultDestinationValue, AnimationClock animationClock)
        {
            if (animationClock == null)
                throw new ArgumentNullException(nameof(animationClock));

            if (From.Value > To.Value)
            {
                return new GridLength((1 - animationClock.CurrentProgress.Value) * (From.Value - To.Value) + To.Value, To.IsStar ? GridUnitType.Star : GridUnitType.Pixel);
            }
            else
            {
                return new GridLength((animationClock.CurrentProgress.Value) * (To.Value - From.Value) + From.Value, To.IsStar ? GridUnitType.Star : GridUnitType.Pixel);
            }
        }
    }
}
