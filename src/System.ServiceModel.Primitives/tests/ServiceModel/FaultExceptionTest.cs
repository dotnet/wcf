// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.


using System;
using System.Globalization;
using System.IO;
using System.Runtime.Serialization;
using System.ServiceModel;
using Infrastructure.Common;
using Xunit;

public static class FaultExceptionTest
{
    [WcfFact]
    public static void Ctor_TDetail_FaultReason()
    {
        var detail = new FaultDetail("Fault Message");
        var reason = new FaultReason("Fault reason");
        var exception = new FaultException<FaultDetail>(detail, reason);
        Assert.NotNull(exception);
        Assert.NotNull(exception.Detail);
        Assert.NotNull(exception.Reason);
        Assert.NotNull(exception.Code);
        Assert.Equal(detail, exception.Detail);
        Assert.Equal(reason, exception.Reason);

        FaultDetail nullDetail = null;
        FaultReason nullReason = null;
        var exception2 = new FaultException<FaultDetail>(nullDetail, nullReason);
        Assert.NotNull(exception2);
        Assert.NotNull(exception2.Code);
        Assert.NotNull(exception2.Reason);
        Assert.Null(exception2.Detail);
    }

    [WcfFact]
    public static void Serializable_TDetail()
    {
        // This isn't exactly what NetFx generates as the HResult is different due to CommunicationException deriving from SystemException on NetFx
        string netfxBsl = @"<FaultExceptionOfFaultDetailInChYzgb xmlns=""http://schemas.datacontract.org/2004/07/System.ServiceModel"" xmlns:i=""http://www.w3.org/2001/XMLSchema-instance"" xmlns:x=""http://www.w3.org/2001/XMLSchema""><ClassName i:type=""x:string"" xmlns="""">System.ServiceModel.FaultException`1[FaultDetail]</ClassName><Message i:type=""x:string"" xmlns="""">The creator of this fault did not specify a Reason.</Message><Data i:nil=""true"" xmlns=""""/><InnerException i:nil=""true"" xmlns=""""/><HelpURL i:nil=""true"" xmlns=""""/><StackTraceString i:nil=""true"" xmlns=""""/><RemoteStackTraceString i:nil=""true"" xmlns=""""/><RemoteStackIndex i:type=""x:int"" xmlns="""">0</RemoteStackIndex><ExceptionMethod i:nil=""true"" xmlns=""""/><HResult i:type=""x:int"" xmlns="""">-2146233088</HResult><Source i:nil=""true"" xmlns=""""/><WatsonBuckets i:nil=""true"" xmlns=""""/><code i:type=""a:ArrayOfFaultException.FaultCodeData"" xmlns="""" xmlns:a=""http://schemas.datacontract.org/2004/07/System.ServiceModel""><a:FaultException.FaultCodeData><a:name>Sender</a:name><a:ns/></a:FaultException.FaultCodeData></code><reason i:type=""a:ArrayOfFaultException.FaultReasonData"" xmlns="""" xmlns:a=""http://schemas.datacontract.org/2004/07/System.ServiceModel""><a:FaultException.FaultReasonData><a:text>The creator of this fault did not specify a Reason.</a:text><a:xmlLang>en-US</a:xmlLang></a:FaultException.FaultReasonData></reason><messageFault i:nil=""true"" xmlns=""""/><action i:nil=""true"" xmlns=""""/><detail i:type=""a:FaultDetail"" xmlns="""" xmlns:a=""http://www.contoso.com/wcfnamespace""><a:Message>Fault Message</a:Message></detail></FaultExceptionOfFaultDetailInChYzgb>";
        var dcs = new DataContractSerializer(typeof(FaultException<FaultDetail>));
        using (var ms = new MemoryStream())
        {
            using (var sr = new StreamReader(ms))
            {
                dcs.WriteObject(ms, new FaultException<FaultDetail>(new FaultDetail("Fault Message")));
                ms.Position = 0;
                Assert.Equal(netfxBsl, sr.ReadToEnd());
                ms.Position = 0;
                var faultException = (FaultException<FaultDetail>)dcs.ReadObject(ms);
                Assert.NotNull(faultException.Detail);
                Assert.Equal("Fault Message", faultException.Detail.Message);
            }
        }
    }

    [WcfFact]
    public static void Serializable_Default()
    {
        string netfxBsl = @"<FaultException xmlns=""http://schemas.datacontract.org/2004/07/System.ServiceModel"" xmlns:i=""http://www.w3.org/2001/XMLSchema-instance"" xmlns:x=""http://www.w3.org/2001/XMLSchema""><ClassName i:type=""x:string"" xmlns="""">System.ServiceModel.FaultException</ClassName><Message i:type=""x:string"" xmlns="""">The creator of this fault did not specify a Reason.</Message><Data i:nil=""true"" xmlns=""""/><InnerException i:nil=""true"" xmlns=""""/><HelpURL i:nil=""true"" xmlns=""""/><StackTraceString i:nil=""true"" xmlns=""""/><RemoteStackTraceString i:nil=""true"" xmlns=""""/><RemoteStackIndex i:type=""x:int"" xmlns="""">0</RemoteStackIndex><ExceptionMethod i:nil=""true"" xmlns=""""/><HResult i:type=""x:int"" xmlns="""">-2146233088</HResult><Source i:nil=""true"" xmlns=""""/><WatsonBuckets i:nil=""true"" xmlns=""""/><code i:type=""a:ArrayOfFaultException.FaultCodeData"" xmlns="""" xmlns:a=""http://schemas.datacontract.org/2004/07/System.ServiceModel""><a:FaultException.FaultCodeData><a:name>Sender</a:name><a:ns/></a:FaultException.FaultCodeData></code><reason i:type=""a:ArrayOfFaultException.FaultReasonData"" xmlns="""" xmlns:a=""http://schemas.datacontract.org/2004/07/System.ServiceModel""><a:FaultException.FaultReasonData><a:text>The creator of this fault did not specify a Reason.</a:text><a:xmlLang>en-US</a:xmlLang></a:FaultException.FaultReasonData></reason><messageFault i:nil=""true"" xmlns=""""/><action i:nil=""true"" xmlns=""""/></FaultException>";
        var dcs = new DataContractSerializer(typeof(FaultException));

        using (var ms = new MemoryStream())
        {
            using (var sr = new StreamReader(ms))
            {
                dcs.WriteObject(ms, new FaultException());
                ms.Position = 0;
                Assert.Equal(netfxBsl, sr.ReadToEnd());
                ms.Position = 0;
                var faultException = (FaultException)dcs.ReadObject(ms);
                Assert.Equal("The creator of this fault did not specify a Reason.", faultException.Message);
                Assert.Null(faultException.Action);
                Assert.NotNull(faultException.Code);
                Assert.Equal("Sender", faultException.Code.Name);
                Assert.Equal(string.Empty, faultException.Code.Namespace);
                Assert.Null(faultException.Code.SubCode);
                Assert.NotNull(faultException.Reason);
                var reasonTranslations = faultException.Reason.Translations;
                Assert.Single(reasonTranslations);
                var faultReasonText = reasonTranslations[0];
                Assert.Equal(CultureInfo.CurrentCulture.Name, faultReasonText.XmlLang);
                Assert.Equal("The creator of this fault did not specify a Reason.", faultReasonText.Text);
            }
        }
    }

    [WcfFact]
    public static void Ctor_StringReason()
    {
        string reason = "Fault reason";
        var exception = new FaultException(reason);
        Assert.NotNull(exception);
        Assert.NotNull(exception.Reason);
        Assert.NotNull(exception.Code);
        Assert.Equal(reason, exception.Reason.ToString());
    }
}
