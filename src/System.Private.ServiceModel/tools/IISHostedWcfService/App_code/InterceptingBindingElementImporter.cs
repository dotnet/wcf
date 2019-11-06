// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.ServiceModel.Description;

namespace Microsoft.Samples.MessageInterceptor
{
    public abstract class InterceptingBindingElementImporter : IPolicyImportExtension
    {
        void IPolicyImportExtension.ImportPolicy(MetadataImporter importer, PolicyConversionContext context)
        {
            ChannelMessageInterceptor messageInterceptor = CreateMessageInterceptor();
            messageInterceptor.OnImportPolicy(importer, context);
        }

        protected abstract ChannelMessageInterceptor CreateMessageInterceptor();
    }
}
