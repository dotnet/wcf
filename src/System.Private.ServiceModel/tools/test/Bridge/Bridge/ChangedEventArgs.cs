// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.


using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bridge
{
    // General purpose EventArgs used to communicate changes
    // to an object of a type T.
    public class ChangedEventArgs<T> : EventArgs
    {
        public ChangedEventArgs(T oldValue, T newValue)
        {
            OldValue = oldValue;
            NewValue = newValue;
        }
        public T OldValue { get; private set; }
        public T NewValue { get; private set; }
    }
}
