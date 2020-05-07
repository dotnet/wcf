// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

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
