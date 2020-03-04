// Copyright (c) Microsoft Corporation.
// Licensed under the MIT license.

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace System.ServiceModel
{
    public interface IExtensionCollection<T> : ICollection<IExtension<T>>
    where T : IExtensibleObject<T>
    {
        E Find<E>();
        Collection<E> FindAll<E>();
    }
}
