// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System;
using System.IO;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.Text;
using System.Xml;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;

public class MtomBindingTestHelper
{
    [ServiceContract]
    public interface IMtomStreamingService
    {
        [OperationContract]
        long UploadStream(Stream stream);
    }

    public class MtomStreamingService : IMtomStreamingService
    {
        public long UploadStream(Stream stream)
        {
            var buffer = new byte[65536];
            long totalBytes = 0;
            int bytesRead;

            while ((bytesRead = stream.Read(buffer, 0, buffer.Length)) > 0)
            {
                totalBytes += bytesRead;
            }

            return totalBytes;
        }
    }

    public static WebApplication BuildWCFService()
    {
        var builder = WebApplication.CreateBuilder();
        builder.WebHost.ConfigureKestrel(serverOptions =>
        {
            serverOptions.AllowSynchronousIO = true;
            serverOptions.Limits.MaxRequestBodySize = 5_368_709_120;
        });

        var app = builder.Build();

        app.MapPost("/", async (HttpContext context) =>
        {
            context.Features.Get<IHttpMaxRequestBodySizeFeature>().MaxRequestBodySize = 5_368_709_120;
            var buffer = new byte[8192];
            while (await context.Request.Body.ReadAsync(buffer, 0, 8192) != 0) { }

            context.Response.Headers.ContentType = "multipart/related; type=\"application/xop+xml\";start=\"<http://tempuri.org/0>\";boundary=\"uuid:fca834ef-6b4a-43c0-a7d0-09064d2827e8+id=1\";start-info=\"text/xml\"";
            await context.Response.WriteAsync("--uuid:fca834ef-6b4a-43c0-a7d0-09064d2827e8+id=1\r\nContent-ID: <http://tempuri.org/0>\r\nContent-Transfer-Encoding: 8bit\r\nContent-Type: application/xop+xml;charset=utf-8;type=\"text/xml\"\r\n\r\n<s:Envelope xmlns:s=\"http://schemas.xmlsoap.org/soap/envelope/\"><s:Body><UploadStreamResponse xmlns=\"http://tempuri.org/\"></UploadStreamResponse></s:Body></s:Envelope>");
        });
        
        return app;
    }

    public static Binding CreateMtomClientBinding()
    {
        var binding = new CustomBinding();
        var mtomElement = new MtomMessageEncodingBindingElement(MessageVersion.Soap11, Encoding.UTF8);
        XmlDictionaryReaderQuotas.Max.CopyTo(mtomElement.ReaderQuotas);
        binding.Elements.Add(mtomElement);
        binding.Elements.Add(new HttpTransportBindingElement
        {
            TransferMode = TransferMode.Streamed,
            MaxBufferSize = 1024 * 64,
            MaxBufferPoolSize = 1,
            MaxReceivedMessageSize = 5_368_709_120
        });
        binding.SendTimeout = TimeSpan.FromMinutes(5);
        binding.ReceiveTimeout = TimeSpan.FromMinutes(5);
        return binding;
    }
}
