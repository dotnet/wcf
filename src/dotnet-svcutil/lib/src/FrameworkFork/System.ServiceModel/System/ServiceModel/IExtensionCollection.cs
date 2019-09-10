// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

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
