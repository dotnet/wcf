// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

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
