// Copyright (c) Microsoft Corporation.
// Licensed under the MIT license.

using System.Threading.Tasks;

namespace Microsoft.Xml {
				using System;
				

    public abstract partial class XmlResolver {

        public virtual Task<Object> GetEntityAsync(Uri absoluteUri,
                                             string role,
                                             Type ofObjectToReturn) {

            throw new NotImplementedException();
        }
    }
}
