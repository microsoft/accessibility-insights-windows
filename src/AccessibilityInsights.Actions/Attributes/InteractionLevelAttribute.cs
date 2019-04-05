// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using Axe.Windows.Actions.Enums;
using System;

namespace Axe.Windows.Actions.Attributes
{
    /// <summary>
    /// Describes the amount of user interaction an Action offers
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    class InteractionLevelAttribute : Attribute
    {
        private UxInteractionLevel _interactionLevel;

        /// <summary>
        /// The ux interaction level of described class
        /// </summary>
        public UxInteractionLevel InteractionLevel
        {
            get { return _interactionLevel; }
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="interactionLevel"></param>
        public InteractionLevelAttribute(UxInteractionLevel interactionLevel) : base()
        {
            this._interactionLevel = interactionLevel;
        }
    }
}
