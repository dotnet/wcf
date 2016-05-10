// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.


using System;

using WcfTestBridgeCommon;

namespace WcfService.TestResources
{
    public class AuthenticationResource : IResource
    {
        public ResourceResponse Put(ResourceRequestContext context)
        {
            throw new NotImplementedException("Cannot PUT on this resource");
        }

        public ResourceResponse Get(ResourceRequestContext context)
        {
            ResourceResponse response = new ResourceResponse();
            AuthenticationResourceHelper.AddCredentialsToResponse(response);
            return response;
        }
    }
}
