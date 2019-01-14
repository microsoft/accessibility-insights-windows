// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading.Tasks;

namespace AccessibilityInsights.WebApiHost.Middlewares.Tests
{
    [TestClass()]
    public class NoRemoteConnectionMiddlewareTests
    {
        [TestMethod()]
        public void TestInvoke_RemoteConnection_NextInvokeNotCalled()
        {
            bool isNextCalled = false;
            var next = new Microsoft.Owin.Fakes.StubOwinMiddleware(null)
            {
                InvokeIOwinContext = c => new Task(() => isNextCalled = true)
            };

            var mw = new NoRemoteConnectionMiddleware(next);

            var ctx = new Microsoft.Owin.Fakes.StubIOwinContext();
            var request = new Microsoft.Owin.Fakes.StubIOwinRequest();
            request.RemoteIpAddressGet = () => "10.1.1.1";
            ctx.RequestGet = () => request;

            mw.Invoke(ctx).Wait();

            Assert.IsFalse(isNextCalled);
        }

        [TestMethod()]
        public void TestInvoke_LocalConnection_NextInvokCalled()
        {
            bool isNextCalled = false;
            var next = new Microsoft.Owin.Fakes.StubOwinMiddleware(null)
            {
#pragma warning disable CS1998 // Async method lacks 'await' operators and will run synchronously
                InvokeIOwinContext = async (c) => isNextCalled = true
#pragma warning restore CS1998 // Async method lacks 'await' operators and will run synchronously
            };

            var mw = new NoRemoteConnectionMiddleware(next);

            var ctx = new Microsoft.Owin.Fakes.StubIOwinContext();
            var request = new Microsoft.Owin.Fakes.StubIOwinRequest();
            request.RemoteIpAddressGet = () => "127.0.0.1";
            ctx.RequestGet = () => request;

            mw.Invoke(ctx).Wait();

            Assert.IsTrue(isNextCalled);
        }
    }
}
