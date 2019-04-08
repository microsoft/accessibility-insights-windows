// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
namespace Axe.Windows.Core.Fingerprint

{
    /// <summary>
    /// Exposes the session IIssueStore implementation. This will eventually need to support
    /// the ability to be stored on disk, but that will be added later
    /// </summary>
    public static class SessionIssueStore
    {
        private static object LockObject = new object();
        private static IIssueStore Store;

        /// <summary>
        /// Get the current IIssueStore for this session
        /// </summary>
        /// <returns>The current IIssueStore object</returns>
        public static IIssueStore GetInstance()
        {
            // Note: this would just be a Lazy, except for the future need to reset the contents if reading a saved baseline
            IIssueStore store;
            if ((store = Store) == null)
            {
                lock (LockObject)
                {
                    if ((store = Store) == null)
                    {
                        store = Store = new InMemoryIssueStore();
                    }
                }
            }

            return store;
        }
    }
}
