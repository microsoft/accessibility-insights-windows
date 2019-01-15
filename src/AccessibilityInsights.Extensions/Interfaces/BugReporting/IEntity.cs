// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;

namespace AccessibilityInsights.Extensions.Interfaces.BugReporting
{
    public interface IEntity
    {
        Guid Id { get; }
        string Name { get; }
    }
}
