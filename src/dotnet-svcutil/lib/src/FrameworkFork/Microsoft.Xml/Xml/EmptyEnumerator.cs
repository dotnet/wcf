// Copyright (c) Microsoft Corporation.
// Licensed under the MIT license.

using System;
using System.Collections;

namespace Microsoft.Xml
{
    using System;


    internal sealed class EmptyEnumerator : IEnumerator
    {
        bool IEnumerator.MoveNext()
        {
            return false;
        }

        void IEnumerator.Reset()
        {
        }

        object IEnumerator.Current
        {
            get
            {
                throw new InvalidOperationException(ResXml.GetString(ResXml.Xml_InvalidOperation));
            }
        }
    }
}
