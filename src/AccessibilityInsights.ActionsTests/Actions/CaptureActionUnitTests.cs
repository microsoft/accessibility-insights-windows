// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;
using System.Collections.Generic;
using System.Linq;
using AccessibilityInsights.Actions;
using AccessibilityInsights.Actions.Contexts;
using AccessibilityInsights.Actions.Enums;
using AccessibilityInsights.Core.Bases;
using AccessibilityInsights.Core.Enums;
using AccessibilityInsights.Core.Misc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
#if FAKES_SUPPORTED
using AccessibilityInsights.Actions.Contexts.Fakes;
using AccessibilityInsights.Core.Bases.Fakes;
using AccessibilityInsights.Desktop.UIAutomation.TreeWalkers.Fakes;
using Microsoft.QualityTools.Testing.Fakes;
using AccessibilityInsights.Actions.Fakes;
#endif

namespace AccessibilityInsights.ActionsTests.Actions
{
    [TestClass]
    public class CaptureActionUnitTests
    {
#if FAKES_SUPPORTED
        [TestMethod]
        [Timeout(1000)]
        public void SetTestModeDataContext_OldContextIsNull_CreatesElementDataContext_ChainsToPopulateData_CorrectParameters()
        {
            using (ShimsContext.Create())
            {
                ElementDataContext dataContext = null;

                ElementContext elementContext = new ShimElementContext
                {
                    DataContextGet = () => dataContext,
                    DataContextSetElementDataContext = (dcNew) => { dataContext = dcNew; },
                };

                ShimDataManager.GetDefaultInstance = () => new ShimDataManager
                {
                    GetElementContextGuid = (_) => elementContext,
                };

                ElementDataContext actualContext = null;
                DataContextMode? actualMode = null;
                TreeViewMode? actualTreeViewMode = null;

                ShimCaptureAction.PopulateDataElementDataContextDataContextModeTreeViewMode = (dc, dcMode, tm) =>
                {
                    actualContext = dc;
                    actualMode = dcMode;
                    actualTreeViewMode = tm;
                };

                Assert.IsTrue(CaptureAction.SetTestModeDataContext(Guid.Empty, DataContextMode.Test, TreeViewMode.Content));

                Assert.IsNotNull(dataContext);
                Assert.AreEqual(0, dataContext.ElementCounter.Count);
                Assert.AreEqual(20000, dataContext.ElementCounter.UpperBound);
                Assert.AreSame(dataContext, actualContext);
                Assert.AreEqual(DataContextMode.Test, actualMode);
                Assert.AreEqual(TreeViewMode.Content, actualTreeViewMode);
            }
        }

        [TestMethod]
        [Timeout(1000)]
        public void PopulateData_TestMode_ChainsToTreeWalkerRefreshTreeData_CorrectParameters()
        {
            using (ShimsContext.Create())
            {
                const DataContextMode expectedDcMode = DataContextMode.Test;
                const TreeViewMode expectedTreeViewMode = TreeViewMode.Content;

                A11yElement expectedElement = new ShimA11yElement();
                List<A11yElement> expectedElements = null;
                ElementDataContext dataContext = new ElementDataContext(expectedElement, 1);

                BoundedCounter actualCounter = null;
                A11yElement actualElement = null;
                TreeViewMode? actualTreeMode = null;
                A11yElement rootElement = null;

                ShimTreeWalkerForTest.ConstructorA11yElementBoundedCounter = (ktw, e, c) =>
                {
                    actualCounter = c;
                    actualElement = e;

                    new ShimTreeWalkerForTest(ktw)
                    {
                        RefreshTreeDataTreeViewMode = (mode) =>
                        {
                            actualTreeMode = mode;
                            expectedElements = new List<A11yElement> { expectedElement };
                            rootElement = expectedElement;
                            Assert.IsTrue(dataContext.ElementCounter.TryIncrement());
                        },
                        ElementsGet = () => expectedElements,
                        TopMostElementGet = () => rootElement,
                    };
                };

                CaptureAction.PopulateData(dataContext, expectedDcMode, expectedTreeViewMode);

                Assert.AreSame(dataContext.ElementCounter, actualCounter);
                Assert.AreSame(expectedElement, actualElement);
                Assert.AreEqual(expectedDcMode, dataContext.Mode);
                Assert.AreEqual(expectedTreeViewMode, dataContext.TreeMode);
                Assert.AreSame(expectedElement, dataContext.Elements.Values.First());
                Assert.AreSame(expectedElement, dataContext.RootElment);
            }
        }

        [TestMethod]
        [Timeout(1000)]
        public void PopulateData_LoadMode_ChainsToAddElementAndChildrenIntoList_CorrectParameters()
        {
            using (ShimsContext.Create())
            {
                const DataContextMode expectedDcMode = DataContextMode.Load;
                const TreeViewMode expectedTreeViewMode = TreeViewMode.Control;
                A11yElement expectedElement = new ShimA11yElement();

                ElementDataContext dataContext = new ElementDataContext(expectedElement, 1);

                A11yElement actualElement = null;
                BoundedCounter actualCounter = null;
                Dictionary<int, A11yElement> actualDictionary = null;
                ShimCaptureAction.AddElementAndChildrenIntoListA11yElementDictionaryOfInt32A11yElementBoundedCounter = (e, d, c) =>
                {
                    actualElement = e;
                    actualDictionary = d;
                    actualCounter = c;
                };

                CaptureAction.PopulateData(dataContext, expectedDcMode, expectedTreeViewMode);

                Assert.AreEqual(expectedDcMode, dataContext.Mode);
                Assert.AreEqual(expectedTreeViewMode, dataContext.TreeMode);
                Assert.AreSame(expectedElement, actualElement);
                Assert.IsNotNull(actualDictionary);
                Assert.IsFalse(actualDictionary.Any());
                Assert.AreSame(dataContext.ElementCounter, actualCounter);
                Assert.AreSame(dataContext.Elements, actualDictionary);
            }
        }
#endif

        [TestMethod]
        [Timeout(1000)]
        public void AddElementAndChildrenIntoList_CounterIsFull_ReturnsWithoutAdding()
        {
            BoundedCounter counter = new BoundedCounter(1);
            Assert.IsFalse(counter.TryAdd(2));
            Assert.AreEqual(2, counter.Attempts);

            // We intentionally pass in nulls here, since it's an error if they're accessed in any way
            CaptureAction.AddElementAndChildrenIntoList(null, null, counter);

            Assert.AreEqual(3, counter.Attempts);
        }

#if FAKES_SUPPORTED
        [TestMethod]
        [Timeout(1000)]
        public void AddElementAndChildrenIntoList_GeneralCase_BuildsCorrectDictionary()
        {
            using (ShimsContext.Create())
            {
                const int elementCount = 7;

                BoundedCounter counter = new BoundedCounter(100);
                Dictionary<int, A11yElement> elementsOut = new Dictionary<int, A11yElement>();

                // Build our tree
                List<ShimA11yElement> elements = new List<ShimA11yElement>();
                for (int loop = 0; loop < elementCount; loop++)
                {
                    int uniqueId = loop;
                    elements.Add(new ShimA11yElement
                    {
                        UniqueIdGet = () => uniqueId,  // Don't use loop here, since it will get the final value, not the in-loop value
                    });
                }
                elements[0].ChildrenGet = () => new List<A11yElement> { elements[1], elements[2] };
                elements[2].ChildrenGet = () => new List<A11yElement> { elements[3] };
                elements[3].ChildrenGet = () => new List<A11yElement> { elements[4], elements[5], elements[6] };
                elements[6].ChildrenGet = () => new List<A11yElement>();

                CaptureAction.AddElementAndChildrenIntoList(elements[0], elementsOut, counter);

                Assert.AreEqual(elementCount, elementsOut.Count);
                for (int loop = 0; loop < elementCount; loop++)
                {
                    Assert.AreEqual(loop, elementsOut[loop].UniqueId);
                }
            }
        }
#endif
    }
}
