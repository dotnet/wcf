//------------------------------------------------------------------------------
// <copyright file="EmptyEnumerator.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>
// <owner current="true" primary="true">helenak</owner>
//------------------------------------------------------------------------------

using System;
using System.Collections;

namespace Microsoft.Xml {
				using System;
				

    internal sealed class EmptyEnumerator : IEnumerator {

        bool IEnumerator.MoveNext() {
            return false;
        }

        void IEnumerator.Reset() {
        }

        object IEnumerator.Current {
            get {
                throw new InvalidOperationException( ResXml.GetString( ResXml.Xml_InvalidOperation ) );
            }
        }
    }
}
