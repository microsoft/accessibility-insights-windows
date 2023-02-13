// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using AccessibilityInsights.SetupLibrary;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.IO;

namespace AccessibilityInsights.SetupLibraryUnitTests
{
    [TestClass]
    public class ChannelInfoUtilitiesUnitTests
    {
        private static Stream PopulateStream(string contents)
        {
            Stream stream = new MemoryStream();
            StreamWriter writer = new StreamWriter(stream);
            writer.Write(contents);
            writer.Flush();
            return stream;
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        [Timeout(2000)]
        public void GetChannelFromStream_StreamIsNull_ThrowsArgumentNullException()
        {
            ChannelInfoUtilities.GetChannelFromStream(null);
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidDataException))]
        [Timeout(2000)]
        public void GetChannelFromStream_StreamIsEmpty_ThrowsInvalidDataException()
        {
            using (Stream stream = PopulateStream(string.Empty))
            {
                ChannelInfoUtilities.GetChannelFromStream(stream);
            }
        }
    }
}
