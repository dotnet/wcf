// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

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
