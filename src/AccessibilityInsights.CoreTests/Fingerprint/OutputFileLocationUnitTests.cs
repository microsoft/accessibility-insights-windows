// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using AccessibilityInsights.Core.Bases;
using AccessibilityInsights.Core.Fingerprint;
using Microsoft.VisualStudio.TestTools.UnitTesting;
#if FAKES_SUPPORTED
using System.IO.Fakes;
using AccessibilityInsights.Core.Bases.Fakes;
using AccessibilityInsights.Core.Fingerprint.Fakes;
using Microsoft.QualityTools.Testing.Fakes;
#endif

namespace AccessibilityInsights.CoreTests.Fingerprint
{
    [TestClass]
    public class OutputFileLocationUnitTests
    {
        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        [Timeout (2000)]
        public void Ctor_FileIsTrivial_ThrowsArgumentException()
        {
            try
            {
                new OutputFileLocation(string.Empty, new A11yElement());
            }
            catch (ArgumentException e)
            {
                Assert.AreEqual("file", e.ParamName);
                Assert.IsNull(e.InnerException);
                throw;
            }
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        [Timeout (2000)]
        public void Ctor_ElementIsNull_ThrowsArgumentNullException()
        {
            try
            {
                new OutputFileLocation(@"c:\foo", null);
            }
            catch (ArgumentNullException e)
            {
                Assert.AreEqual("element", e.ParamName);
                Assert.IsNull(e.InnerException);
                throw;
            }
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        [Timeout (2000)]
        public void Ctor_GetFullPathThrowsException_ThrowsArgumentException()
        {
            try
            {
                new OutputFileLocation("C::::::::::", new A11yElement());
            }
            catch (ArgumentException e)
            {
                Assert.AreEqual("file", e.ParamName);
                Assert.IsNotNull(e.InnerException);
                throw;
            }
        }

#if FAKES_SUPPORTED
        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        [Timeout(2000)]
        public void Ctor_FileDoesNotExist_ThrowsArgumentException()
        {
            const string invalidFile = @"c:\myfile.xyz";

            using (ShimsContext.Create())
            {
                ShimFile.ExistsString = (f) =>
                {
                    return f == invalidFile ? false : ShimsContext.ExecuteWithoutShims(() => File.Exists(f));
                };
                try
                {
                    new OutputFileLocation(invalidFile, new A11yElement());
                }
                catch (ArgumentException e)
                {
                    Assert.AreEqual("file", e.ParamName);
                    Assert.IsNull(e.InnerException);
                    throw;
                }
            }
        }

        [TestMethod]
        [Timeout(2000)]
        public void Ctor_SimpleCase_PropertiesAreSetCorrectly()
        {
            const string fileName = @"c:\xyz\abc";

            using (ShimsContext.Create())
            {
                ShimFile.ExistsString = (f) =>
                {
                    return f == fileName ? true : ShimsContext.ExecuteWithoutShims(() => File.Exists(f));
                };

                A11yElement element = new ShimA11yElement
                {
                    UniqueIdGet = () => 24680,
                };

                ILocation location = new OutputFileLocation(fileName, element);

                Assert.AreEqual(fileName, location.Source);
                Assert.AreEqual("24680", location.Id);
            }
        }

        [TestMethod]
        [Timeout(2000)]
        public void UserDisplayInfo_SimpleCase_OutputIsContainsLocationProperties()
        {
            const string fileName = "c:\\abc\\xyz";

            using (ShimsContext.Create())
            {
                ShimFile.ExistsString = (f) =>
                {
                    return f == fileName ? true : ShimsContext.ExecuteWithoutShims(() => File.Exists(f));
                };

                A11yElement element = new ShimA11yElement
                {
                    UniqueIdGet = () => 13579,
                };

                ILocation location = new OutputFileLocation(fileName, element);
                string displayInfo = location.UserDisplayInfo;
                Assert.IsTrue(displayInfo.Contains(location.Source));
                Assert.IsTrue(displayInfo.Contains(location.Id));
            }
        }

        [TestMethod]
        [Timeout (2000)]
        public void Equals_OtherIsNull_ReturnsFalse()
        {
            const string fileName = @"c:\xyz\abc";

            using (ShimsContext.Create())
            {
                ShimFile.ExistsString = (f) =>
                {
                    return f == fileName ? true : ShimsContext.ExecuteWithoutShims(() => File.Exists(f));
                };

                A11yElement element = new ShimA11yElement
                {
                    UniqueIdGet = () => 24680,
                };

                ILocation location = new OutputFileLocation(fileName, element);

                Assert.IsFalse(location.Equals(null));
            }
        }

        [TestMethod]
        [Timeout(2000)]
        public void Equals_OtherIsDifferentClass_ReturnsFalse()
        {
            const string fileName = @"c:\xyz\abc";

            using (ShimsContext.Create())
            {
                ShimFile.ExistsString = (f) =>
                {
                    return f == fileName ? true : ShimsContext.ExecuteWithoutShims(() => File.Exists(f));
                };

                A11yElement element = new ShimA11yElement
                {
                    UniqueIdGet = () => 24680,
                };

                ILocation location = new OutputFileLocation(fileName, element);

                Assert.IsFalse(location.Equals(new StubILocation()));
            }
        }

        [TestMethod]
        [Timeout (2000)]
        public void Equals_OtherHasDifferentFile_ReturnsFalse()
        {
            const string fileName1 = @"c:\xyz\abc";
            const string fileName2 = @"c:\abc\xyz";
            const int id1 = 24680;
            const int id2 = 24680;

            using (ShimsContext.Create())
            {
                ShimFile.ExistsString = (f) =>
                {
                    return (f == fileName1 || f == fileName2) ? true : ShimsContext.ExecuteWithoutShims(() => File.Exists(f));
                };

                A11yElement element1 = new ShimA11yElement
                {
                    UniqueIdGet = () => id1,
                };
                A11yElement element2 = new ShimA11yElement
                {
                    UniqueIdGet = () => id2,
                };

                ILocation location1 = new OutputFileLocation(fileName1, element1);
                ILocation location2 = new OutputFileLocation(fileName2, element2);

                Assert.IsFalse(location1.Equals(location2));
                Assert.IsFalse(location2.Equals(location1));
            }
        }

        [TestMethod]
        [Timeout(2000)]
        public void Equals_OtherHasDifferentId_ReturnsFalse()
        {
            const string fileName1 = @"c:\xyz\abc";
            const string fileName2 = @"c:\xyz\abc";
            const int id1 = 24680;
            const int id2 = 13579;

            using (ShimsContext.Create())
            {
                ShimFile.ExistsString = (f) =>
                {
                    return (f == fileName1 || f == fileName2) ? true : ShimsContext.ExecuteWithoutShims(() => File.Exists(f));
                };

                A11yElement element1 = new ShimA11yElement
                {
                    UniqueIdGet = () => id1,
                };
                A11yElement element2 = new ShimA11yElement
                {
                    UniqueIdGet = () => id2,
                };

                ILocation location1 = new OutputFileLocation(fileName1, element1);
                ILocation location2 = new OutputFileLocation(fileName2, element2);

                Assert.IsFalse(location1.Equals(location2));
                Assert.IsFalse(location2.Equals(location1));
            }
        }

        [TestMethod]
        [Timeout(2000)]
        public void Equals_OtherHasSameFileAndId_ReturnsTrue()
        {
            const string fileName1 = @"c:\xyz\abc";
            const string fileName2 = @"c:\xyz\abc";
            const int id1 = 24680;
            const int id2 = 24680;

            using (ShimsContext.Create())
            {
                ShimFile.ExistsString = (f) =>
                {
                    return (f == fileName1 || f == fileName2) ? true : ShimsContext.ExecuteWithoutShims(() => File.Exists(f));
                };

                A11yElement element1 = new ShimA11yElement
                {
                    UniqueIdGet = () => id1,
                };
                A11yElement element2 = new ShimA11yElement
                {
                    UniqueIdGet = () => id2,
                };

                ILocation location1 = new OutputFileLocation(fileName1, element1);
                ILocation location2 = new OutputFileLocation(fileName2, element2);

                Assert.IsTrue(location1.Equals(location2));
                Assert.IsTrue(location2.Equals(location1));
            }
        }

        [TestMethod]
        [Timeout (2000)]
        public void Open_FileDoesNotExist_ReturnsFalse()
        {
            const string fileName = @"c:\xyz\abc";
            const int id = 24680;
            bool fileExists = true;

            using (ShimsContext.Create())
            {
                ShimFile.ExistsString = (f) =>
                {
                    return (f == fileName) ? fileExists : ShimsContext.ExecuteWithoutShims(() => File.Exists(f));
                };

                A11yElement element = new ShimA11yElement
                {
                    UniqueIdGet = () => id,
                };

                ILocation location = new OutputFileLocation(fileName, element);

                fileExists = false;

                Assert.IsFalse(location.Open());
            }
        }

        [TestMethod]
        [Timeout(2000)]
        public void Open_ProcessStartThrowsEnumeratedException_ReturnsFalse()
        {
            const string fileName = @"c:\xyz\abc";
            const int id = 24680;

            using (ShimsContext.Create())
            {
                ShimFile.ExistsString = (f) =>
                {
                    return (f == fileName) ? true : ShimsContext.ExecuteWithoutShims(() => File.Exists(f));
                };

                A11yElement element = new ShimA11yElement
                {
                    UniqueIdGet = () => id,
                };

                List<Exception> enumeratedExceptions = new List<Exception>
                {
                    new InvalidOperationException(),
                    new ArgumentException(),
                    new ObjectDisposedException("blah", new Exception()),
                    new FileNotFoundException(),
                    new Win32Exception(),
                };

                int exceptionsTested = 0;
                foreach (Exception e in enumeratedExceptions)
                {
                    ILocation location = new OutputFileLocation(fileName, element, (startInfo) => { throw e; });
                    Assert.IsFalse(location.Open(), "Simulated Exception: " + e.ToString());
                    exceptionsTested++;
                }

                Assert.AreEqual(enumeratedExceptions.Count, exceptionsTested);
            }
        }

        [TestMethod]
        [ExpectedException(typeof(OutOfMemoryException))]
        [Timeout(2000)]
        public void Open_ProcessStartThrowsNonEnumeratedException_RethrowsException()
        {
            const string fileName = @"c:\xyz\abc";
            const int id = 24680;

            using (ShimsContext.Create())
            {
                ShimFile.ExistsString = (f) =>
                {
                    return (f == fileName) ? true : ShimsContext.ExecuteWithoutShims(() => File.Exists(f));
                };

                A11yElement element = new ShimA11yElement
                {
                    UniqueIdGet = () => id,
                };

                ILocation location = new OutputFileLocation(fileName, element, (startInfo) => { throw new OutOfMemoryException(); });

                location.Open();
            }
        }

        [TestMethod]
        [Timeout(2000)]
        public void Open_ProcessStartReturnsNull_ReturnsFalse()
        {
            const string fileName = @"c:\xyz\abc";
            const int id = 24680;

            using (ShimsContext.Create())
            {
                ShimFile.ExistsString = (f) =>
                {
                    return (f == fileName) ? true : ShimsContext.ExecuteWithoutShims(() => File.Exists(f));
                };

                A11yElement element = new ShimA11yElement
                {
                    UniqueIdGet = () => id,
                };

                ProcessStartInfo actualStartInfo = null;
                ILocation location = new OutputFileLocation(fileName, element, (startInfo) =>
                {
                    actualStartInfo = startInfo;
                    return null;
                });

                Assert.IsNull(actualStartInfo);
                Assert.IsFalse(location.Open());
                Assert.IsNotNull(actualStartInfo);
                Assert.AreEqual(fileName, actualStartInfo.FileName);
                Assert.AreEqual("open", actualStartInfo.Verb);
            }
        }

        [TestMethod]
        [Timeout(2000)]
        public void Open_ProcessStartReturnsNonNull_ReturnsTrue()
        {
            const string fileName = @"c:\xyz\abc";
            const int id = 24680;

            using (ShimsContext.Create())
            {
                ShimFile.ExistsString = (f) =>
                {
                    return (f == fileName) ? true : ShimsContext.ExecuteWithoutShims(() => File.Exists(f));
                };

                A11yElement element = new ShimA11yElement
                {
                    UniqueIdGet = () => id,
                };

                ProcessStartInfo actualStartInfo = null;
                ILocation location = new OutputFileLocation(fileName, element, (startInfo) =>
                {
                    actualStartInfo = startInfo;
                    return new Process();
                });

                Assert.IsNull(actualStartInfo);
                Assert.IsTrue(location.Open());
                Assert.IsNotNull(actualStartInfo);
                Assert.AreEqual(fileName, actualStartInfo.FileName);
                Assert.AreEqual("open", actualStartInfo.Verb);
            }
        }
#endif
    }
}
