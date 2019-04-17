// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using Axe.Windows.Actions.Attributes;
using Axe.Windows.Actions.Enums;
using Axe.Windows.Desktop.Utility;
using System;
using System.Drawing;

namespace Axe.Windows.Actions
{
    /// <summary>
    /// Class SelectionAction
    /// this class is to select unelement via focus or keyboard
    /// </summary>
    [InteractionLevel(UxInteractionLevel.NoUxInteraction)]
    public static class ScreenShotAction
    {
        /// <summary>
        /// Take a screenshot of the given element's parent window, if it has one
        ///     returns null if the bounding rectangle is 0-sized
        /// </summary>
        /// <param name="ecId">Element Context Id</param>
        public static void CaptureScreenShot(Guid ecId)
        {
            try
            {
                var ec = DataManager.GetDefaultInstance().GetElementContext(ecId);
                var el = ec.Element;

                var win = el.GetParentWindow();
                el = win ?? el;
                var rect = el.BoundingRectangle;
                if (rect.IsEmpty)
                {
                    return; // no capture. 
                }
                Bitmap bmp = new Bitmap(rect.Width, rect.Height);
                Graphics g = Graphics.FromImage(bmp);

                g.CopyFromScreen(rect.X, rect.Y, 0, 0, rect.Size);
                ec.DataContext.Screenshot = bmp;
                ec.DataContext.ScreenshotElementId = el.UniqueId;
            }
            catch(TypeInitializationException)
            {
                // silently ignore. since it happens only on WCOS.
                // in this case, the results file will be loaded with yellow box.
            }
        }
    }
}
