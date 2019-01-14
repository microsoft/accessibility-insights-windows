// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using Microsoft.VisualStudio.TestTools.UnitTesting;
using AccessibilityInsights.Core.Bases;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using Newtonsoft.Json;

namespace AccessibilityInsights.CoreTests.Bases
{
    /// <summary>
    /// Tests A11yElement class
    /// </summary>
    [TestClass()]
    public class A11yElementTests
    {

        [TestMethod()]
        [Timeout(2000)]
        public void FindDescendent_SimpleCondition_ReturnsChild()
        {
            A11yElement child = new A11yElement()
            {
                UniqueId = 0
            };
            A11yElement parent = new A11yElement()
            {
                Children = new List<A11yElement>() { child }
            };
            var result = parent.FindDescendent(ke => ke.UniqueId == 0);
            Assert.AreEqual(result, child);
        }

        [TestMethod()]
        [Timeout(2000)]
        public void FindDescendent_SimpleCondition_ReturnsNull()
        {
            A11yElement child = new A11yElement()
            {
                UniqueId = 5,
            };
            A11yElement parent = new A11yElement()
            {
                Children = new List<A11yElement>() { child },
            };
            var result = parent.FindDescendent(ke => ke.UniqueId == 0);
            Assert.IsNull(result);
        }

        [TestMethod()]
        [Timeout(2000)]
        public void FindDescendent_SimpleCondition_ReturnsGrandchild()
        {
            A11yElement grandChild = new A11yElement()
            {
                UniqueId = 0
            };
            A11yElement parent = new A11yElement()
            {
                Children = new List<A11yElement>() {
                    new A11yElement() {
                        UniqueId = -1,
                        Children = new List<A11yElement>() {
                            grandChild
                        }
                    }
                }
            };
            var result = parent.FindDescendent(ke => ke.UniqueId == 0);
            Assert.AreEqual(result, grandChild);
        }

        [TestMethod()]
        [Timeout(2000)]
        public void FindDescendent_SimpleConditionNonePass_ReturnsNull()
        {
            A11yElement child = new A11yElement()
            {
                UniqueId = -1,
                Children = new List<A11yElement>()
                {
                    new A11yElement()
                    {
                        UniqueId = -1
                    }
                }
            };
            A11yElement parent = new A11yElement()
            {
                Children = new List<A11yElement>() { child }
            };
            var result = parent.FindDescendent(ke => ke.UniqueId == 0);
            Assert.IsNull(result);
        }

        /// <summary>
        /// Test various getters which use GetPropertySafely
        /// </summary>
        [TestMethod()]
        public void GetPropertySafelyTest()
        {
            A11yElement ke = FromJson("Resources/A11yElementTest.hier");

            Assert.AreEqual("Text Editor", ke.Name);
            ///Assert.AreEqual(ControlTypes.UIA_EditControlTypeId, ke.ControlTypeId);
            Assert.AreEqual("edit", ke.LocalizedControlType);
            Assert.AreEqual("[7,436C,50B051]", ke.RuntimeId);
            Assert.AreEqual(17260, ke.ProcessId);

            Assert.AreEqual(new Rectangle()
            {
                X = 723,
                Y = 203,
                Width = 1380,
                Height = 1009
            }, ke.BoundingRectangle);

            Assert.IsTrue(ke.IsContentElement);
            Assert.IsTrue(ke.IsControlElement);
            Assert.IsTrue(ke.IsKeyboardFocusable);
        }

        /// <summary>
        /// Deserialize saved A11yElement
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static A11yElement FromJson(string path)
        {
            A11yElement element = null;
            if (File.Exists(path))
            {
                var json = File.ReadAllText(path);
                element = JsonConvert.DeserializeObject<A11yElement>(json);
                if (element == null)
                {
                    return null;
                }
                // Set parents
                Queue<A11yElement> elements = new Queue<A11yElement>();
                elements.Enqueue(element);
                while (elements.Count > 0)
                {
                    var next = elements.Dequeue();
                    if (next.Children != null)
                    {
                        next.Children.ForEach(c => {
                            c.Parent = next;
                            elements.Enqueue(c);
                        });
                    }
                }
            }
            
            return element;
        }
    }
}
