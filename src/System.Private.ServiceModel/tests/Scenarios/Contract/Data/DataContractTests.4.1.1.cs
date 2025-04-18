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

public partial class DataContractTests
{
    [WcfFact]
    [OuterLoop]
    public static void DataContractResolverTest()
    {
        IDataContractResolverService client = null;
        ChannelFactory<IDataContractResolverService> factory = null;
        try
        {
            factory = new ChannelFactory<IDataContractResolverService>(new BasicHttpBinding(), new EndpointAddress(Endpoints.DataContractResolver_Address));
            ContractDescription cd = factory.Endpoint.Contract;

            foreach (var operation in factory.Endpoint.Contract.Operations)
            {
                DataContractSerializerOperationBehavior behavior =
                         operation.OperationBehaviors.FirstOrDefault(
                             x => x.GetType() == typeof(DataContractSerializerOperationBehavior)) as DataContractSerializerOperationBehavior;
                behavior.DataContractResolver = new ManagerDataContractResolver();
            }

            client = factory.CreateChannel();
            client.AddEmployee(new Manager() { Age = "10", Name = "jone", OfficeId = 1 });
            var results = client.GetAllEmployees();
            Assert.Single(results);
            var manager = results.First();
            Assert.True(manager.GetType() == (typeof(Manager)), string.Format("Expected type: {0}, Actual type: {1}", typeof(Manager), manager.GetType()));
            Assert.True(((Manager)manager).Name == "jone", string.Format("Expected Name: {0}, Actual Name: {1}", "jone", ((Manager)manager).Name));
            Assert.True(((Manager)manager).Age == "10", string.Format("Expected Age: {0}, Actual Age: {1}", "10", ((Manager)manager).Age));
            Assert.True(((Manager)manager).OfficeId == 1, string.Format("Expected Id: {0}, Actual Id: {1}", 1, ((Manager)manager).OfficeId));
            factory.Close();
            ((ICommunicationObject)client).Close();
        }
        finally
        {
            // *** ENSURE CLEANUP *** \\
            ScenarioTestHelpers.CloseCommunicationObjects((ICommunicationObject)client, factory);
        }
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
