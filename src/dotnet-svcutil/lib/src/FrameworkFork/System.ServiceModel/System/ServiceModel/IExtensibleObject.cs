// Copyright (c) Microsoft Corporation.
// Licensed under the MIT license.

using System;
using System.Collections.Generic;

namespace System.ServiceModel
{
    public interface IExtensibleObject<T>
    where T : IExtensibleObject<T>
    {
        IExtensionCollection<T> Extensions { get; }
    }
}
