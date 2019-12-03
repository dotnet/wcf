// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Description;
using System.Text;
using System.Threading.Tasks;
using Infrastructure.Common;
using Xunit;

public static class OperationBehaviorTest
{
    [WcfFact]
    public static void IOperationBehavior_Methods_AreCalled()
    {
        DuplexClientBase<ICustomOperationBehaviorDuplexService> duplexService = null;
        ICustomOperationBehaviorDuplexService proxy = null;

        NetTcpBinding binding = new NetTcpBinding();
        binding.Security.Mode = SecurityMode.None;

        WcfDuplexServiceCallback callbackService = new WcfDuplexServiceCallback();
        InstanceContext context = new InstanceContext(callbackService);

        duplexService = new MyDuplexClientBase<ICustomOperationBehaviorDuplexService>(context, binding, new EndpointAddress(FakeAddress.TcpAddress));
        proxy = duplexService.ChannelFactory.CreateChannel();

        // Wait to validate until the process has been given a reasonable time to complete.
        Task[] taskCollection = { MyOperationBehavior.validateMethodTcs.Task, MyOperationBehavior.addBindingParametersMethodTcs.Task, MyOperationBehavior.applyClientBehaviorMethodTcs.Task };
        bool waitAll = Task.WaitAll(taskCollection, 250);

        Assert.True(MyOperationBehavior.errorBuilder.Length == 0, "Test case FAILED with errors: " + MyOperationBehavior.errorBuilder.ToString());
        Assert.True(waitAll, "None of the IOperationBehavior methods were called.");

        ((ICommunicationObject)proxy).Close();
        ((ICommunicationObject)duplexService).Close();
    }

    [WcfFact]
    // Validate that we can use XmlSerializerOperationBehavior to modify or add XmlSerializerFormatAttribute on interface operations.
    public static void XmlSerializerOperationBehavior_BasicUsage()
    {
        XmlSerializerOperationBehavior serializerBehavior;
        BasicHttpBinding binding = new BasicHttpBinding();
        string baseAddress = "http://localhost:1066/SomeService";
        ChannelFactory<IXmlTestingType> factory = new ChannelFactory<IXmlTestingType>(binding, new EndpointAddress(baseAddress));
        ContractDescription cd = factory.Endpoint.Contract;
        OperationDescriptionCollection collection = cd.Operations;

        foreach (OperationDescription description in collection)
        {
            // Find the serializer behavior for those operations that have the attribute set via the interface.
            serializerBehavior = description.Behaviors.Find<XmlSerializerOperationBehavior>();
            if (serializerBehavior == null)
            {
                // This operation was not set with XmlSerializerFormatAttribute
                // Here we add the attribute programatically using defaults.
                if (String.Equals(description.Name, nameof(IXmlTestingType.XmlSerializerFormatAttribute_NotSet_One)))
                {
                    // Default OperationFormatStyle is "Document"
                    serializerBehavior = new XmlSerializerOperationBehavior(description);
                    description.Behaviors.Add(serializerBehavior);
                }
                // There is one additional operation not set with XmlSerializerFormatAttribute
                // Here we add the attribute programatically and further set the OperationFormatStyle to 'Rpc'
                else
                {
                    XmlSerializerFormatAttribute serializerAttribute = new XmlSerializerFormatAttribute();
                    serializerAttribute.Style = OperationFormatStyle.Rpc;

                    serializerBehavior = new XmlSerializerOperationBehavior(description, serializerAttribute);
                    description.Behaviors.Add(serializerBehavior);
                }

            }

            if (String.Equals(description.Name, nameof(IXmlTestingType.XmlSerializerFormatAttribute_Set_StyleSetTo_Rpc)) || (String.Equals(description.Name, nameof(IXmlTestingType.XmlSerializerFormatAttribute_NotSet_Two))))
            {
                Assert.Equal("Rpc", serializerBehavior.XmlSerializerFormatAttribute.Style.ToString());
            }
            else
            {
                Assert.Equal("Document", serializerBehavior.XmlSerializerFormatAttribute.Style.ToString());
            }
        }
    }

    [WcfFact]
    public static void DataContractSerializationSurrogateTest()
    {
        OperationDescription od = null;
        DataContractSerializerOperationBehavior behavior = new DataContractSerializerOperationBehavior(od);

        behavior.SerializationSurrogateProvider = new MySerializationSurrogateProvider();

        DataContractSerializer dcs = (DataContractSerializer)behavior.CreateSerializer(typeof(SurrogateTestType), nameof(SurrogateTestType), "ns", new List<Type>());

        var members = new NonSerializableType[2];
        members[0] = new NonSerializableType("name1", 1);
        members[1] = new NonSerializableType("name2", 2);

        using (MemoryStream ms = new MemoryStream())
        {
            SurrogateTestType obj = new SurrogateTestType { Members = members };

            dcs.WriteObject(ms, obj);
            ms.Position = 0;
            var deserialized = (SurrogateTestType)dcs.ReadObject(ms);

            Assert.True(((MySerializationSurrogateProvider)behavior.SerializationSurrogateProvider).mySurrogateProviderIsUsed);

            for (int i = 0; i < 2; i++)
            {
                Assert.Equal(obj.Members[i].Name, deserialized.Members[i].Name);
                Assert.StrictEqual(obj.Members[i].Index, deserialized.Members[i].Index);
            }
        }

    }

    [ServiceContract]
    public interface IXmlTestingType
    {
        [OperationContract]
        [XmlSerializerFormat(Style = OperationFormatStyle.Rpc)]
        void XmlSerializerFormatAttribute_Set_StyleSetTo_Rpc();

        [OperationContract]
        void XmlSerializerFormatAttribute_NotSet_One();
        [OperationContract]
        void XmlSerializerFormatAttribute_NotSet_Two();

        [OperationContract]
        [XmlSerializerFormat]
        void XmlSerializerFormatAttribute_Set_StyleSetTo_Default();

        [OperationContract]
        [XmlSerializerFormat(Style = OperationFormatStyle.Document)]
        void XmlSerializerFormatAttribute_Set_StyleSetTo_Document();
    }

    public class MySerializationSurrogateProvider : ISerializationSurrogateProvider
    {
        public bool mySurrogateProviderIsUsed = false;

        public object GetDeserializedObject(object obj, Type targetType)
        {
            mySurrogateProviderIsUsed = true;
            if (obj is NonSerializableTypeSurrogate)
            {
                NonSerializableTypeSurrogate surrogate = (NonSerializableTypeSurrogate)obj;
                return new NonSerializableType(surrogate.Name, surrogate.Index);
            }

            return obj;
        }

        public object GetObjectToSerialize(object obj, Type targetType)
        {
            mySurrogateProviderIsUsed = true;
            if (obj is NonSerializableType)
            {
                NonSerializableType i = (NonSerializableType)obj;
                NonSerializableTypeSurrogate surrogate = new NonSerializableTypeSurrogate
                {
                    Name = i.Name,
                    Index = i.Index,
                };

                return surrogate;
            }

            return obj;
        }

        public Type GetSurrogateType(Type type)
        {
            mySurrogateProviderIsUsed = true;
            if (type == typeof(NonSerializableType))
            {
                return typeof(NonSerializableTypeSurrogate);
            }

            return type;
        }
    }
}
