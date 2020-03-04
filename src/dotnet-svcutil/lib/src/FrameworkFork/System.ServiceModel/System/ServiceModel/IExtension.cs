// Copyright (c) Microsoft Corporation.
// Licensed under the MIT license.

using System;

namespace System.ServiceModel
{
    public interface IExtension<T> where T : IExtensibleObject<T>
    {
        void Attach(T owner);
        void Detach(T owner);
    }
}
