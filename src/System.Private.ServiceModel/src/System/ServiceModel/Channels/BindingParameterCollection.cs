// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.


using System.Collections.Generic;

namespace System.ServiceModel.Channels
{
    // Some binding elements can sometimes consume extra information when building factories.
    // BindingParameterCollection is a collection of objects with this extra information.
    // See comments in SecurityBindingElement and TransactionFlowBindingElement for examples
    // of binding elements that go looking for certain data in this collection.
    public class BindingParameterCollection : KeyedByTypeCollection<object>
    {
        public BindingParameterCollection() { }

        internal BindingParameterCollection(params object[] parameters)
        {
            if (parameters == null)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull(nameof(parameters));
            }

            for (int i = 0; i < parameters.Length; i++)
            {
                base.Add(parameters[i]);
            }
        }

        internal BindingParameterCollection(BindingParameterCollection parameters)
        {
            if (parameters == null)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull(nameof(parameters));
            }

            for (int i = 0; i < parameters.Count; i++)
            {
                base.Add(parameters[i]);
            }
        }
    }
}
