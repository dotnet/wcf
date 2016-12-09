// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.


using System;
using System.ServiceModel;
using Infrastructure.Common;
using Xunit;
using System.Text;
using System.ServiceModel.Description;
using System.Runtime.Serialization;
using System.Xml;
using System.Linq;

public static partial class DataContractTests
{
    [WcfFact]
    [OuterLoop]
    public static void DataContractResolverTest()
    {
        StringBuilder errorBuilder = new StringBuilder();
        try
        {
            ChannelFactory<IWcfService> factory = new ChannelFactory<IWcfService>(new BasicHttpBinding(), new EndpointAddress(Endpoints.DataContractResolver_Address));
            ContractDescription cd = factory.Endpoint.Contract;

            foreach (var operation in factory.Endpoint.Contract.Operations)
            {
                DataContractSerializerOperationBehavior behavior =
                         operation.OperationBehaviors.FirstOrDefault(
                             x => x.GetType() == typeof(DataContractSerializerOperationBehavior)) as DataContractSerializerOperationBehavior;
                behavior.DataContractResolver = new ManagerDataContractResolver();
            }

            IWcfService client = factory.CreateChannel();
            client.AddEmployee(new Manager() { Age = "10", Name = "jone", OfficeId = 1 });
            var results = client.GetAllEmployees();
            Assert.Equal(1, results.Count());
            var manager = results.First();
            Assert.Equal(typeof(Manager), manager.GetType());
            Assert.Equal("jone", ((Manager)manager).Name);
            Assert.Equal("10", ((Manager)manager).Age);
            Assert.Equal(1, ((Manager)manager).OfficeId);
            factory.Close();
        }
        catch(Exception ex)
        {
            errorBuilder.AppendLine(String.Format("Unexpected exception was caught: {0}", ex.ToString()));
        }

        Assert.True(errorBuilder.Length == 0, errorBuilder.ToString());
    }
}

public class ManagerDataContractResolver : DataContractResolver
{
    private string Namespace
    {
        get { return typeof(Manager).Namespace ?? "global"; }
    }

    private string Name
    {
        get { return typeof(Manager).Name; }
    }


    public override Type ResolveName(string typeName, string typeNamespace, Type declaredType, DataContractResolver knownTypeResolver)
    {
        if (typeName == this.Name && typeNamespace == this.Namespace)
        {
            return typeof(Manager);
        }
        else
        {
            return knownTypeResolver.ResolveName(typeName, typeNamespace, declaredType, null);
        }
    }

    public override bool TryResolveType(Type type, Type declaredType, DataContractResolver knownTypeResolver, out XmlDictionaryString typeName, out XmlDictionaryString typeNamespace)
    {
        if (type == typeof(Manager))
        {
            XmlDictionary dic = new XmlDictionary();
            typeName = dic.Add(this.Name);
            typeNamespace = dic.Add(this.Namespace);
            return true;
        }
        else
        {
            return knownTypeResolver.TryResolveType(type, declaredType, null, out typeName, out typeNamespace);
        }
    }
}
