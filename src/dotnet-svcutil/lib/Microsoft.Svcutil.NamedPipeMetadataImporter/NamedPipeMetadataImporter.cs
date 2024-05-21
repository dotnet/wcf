// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.Threading.Tasks;
using System.Xml;

namespace Microsoft.Tools.ServiceModel.Svcutil
{
    public class NamedPipeMetadataImporter
    {
        const string UriSchemeNetPipe = "net.pipe";
        const string NamedPipeBindingName = "MetadataExchangeNamedPipeBinding";
        const string BindingNamespace = "http://schemas.microsoft.com/ws/2005/02/mex/bindings";

#if NET8_0
        [System.Runtime.Versioning.SupportedOSPlatform("windows")]
#endif
        public XmlReader GetReader(Uri uri)
        {
            return GetMetadatadataAsync(uri).GetAwaiter().GetResult();
        }

#if NET8_0
        [System.Runtime.Versioning.SupportedOSPlatform("windows")]
#endif
        public async Task<XmlReader> GetMetadatadataAsync(Uri uri)
        {
            if (uri.Scheme != UriSchemeNetPipe)
            {
                return null;
            }

            ChannelFactory<IMetadataExchange> factory = new ChannelFactory<IMetadataExchange>(CreateNamedPipeBinding(), new EndpointAddress(uri.AbsoluteUri));
            IMetadataExchange proxy = factory.CreateChannel();
            MessageVersion messageVersion = factory.Endpoint.Binding.MessageVersion;
            var tcs = new TaskCompletionSource<XmlReader>();
            try
            {
                var _message = Message.CreateMessage(messageVersion, WSTransfer.GetAction);
                IAsyncResult result = proxy.BeginGet(_message, new AsyncCallback(ar =>
                {
                    try
                    {
                        RequestCallback(ar, tcs);
                    }
                    catch (Exception ex)
                    {
                        tcs.SetException(ex);
                    }
                }), proxy);

                while (!result.IsCompleted)
                {
                    await Task.Delay(100);
                }
            }
            catch (Exception ex)
            {
                ((IClientChannel)proxy).Close();
                tcs.SetException(ex);
            }

            finally
            {
                ((IClientChannel)proxy).Abort();
            }

            return await tcs.Task;
        }

        public void RequestCallback(IAsyncResult result, TaskCompletionSource<XmlReader> tcs)
        {
            if (result.CompletedSynchronously)
                return;

            if (result.AsyncState is IMetadataExchange metadataClient)
            {
                try
                {
                    Message response = metadataClient.EndGet(result);
                    if (!response.IsFault)
                    {
                        XmlReader xmlReader = response.GetReaderAtBodyContents();
                        tcs.SetResult(xmlReader);
                    }
                    else
                    {
                        // Handle fault response
                        tcs.SetException(new Exception("Fault response received."));
                    }
                }
                catch (Exception ex)
                {
                    tcs.SetException(ex);
                }
            }
        }

#if NET8_0
        [System.Runtime.Versioning.SupportedOSPlatform("windows")]
#endif
        public CustomBinding CreateNamedPipeBinding()
        {
            CustomBinding binding = new CustomBinding(NamedPipeBindingName, BindingNamespace);
            NamedPipeTransportBindingElement pipeTransport = new NamedPipeTransportBindingElement();
            binding.Elements.Add(pipeTransport);
            return binding;
        }
    }
}
