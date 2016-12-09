using System;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Activation;
using System.ServiceModel.Channels;
using System.ServiceModel.Description;
using System.Xml;

namespace WcfService
{
    public class DataContractResolverTestServiceHostFactory : ServiceHostFactory
    {
        protected override ServiceHost CreateServiceHost(Type serviceType, Uri[] baseAddresses)
        {
            DataContractResolverTestServiceHost serviceHost = new DataContractResolverTestServiceHost(serviceType, baseAddresses);
            return serviceHost;
        }
    }

    public class DataContractResolverTestServiceHost : TestServiceHostBase<IWcfService>
    {
        protected override string Address { get { return "DataContractResolver"; } }

        protected override Binding GetBinding()
        {
            return new BasicHttpBinding();
        }

        protected override void ApplyConfiguration()
        {
            base.ApplyConfiguration();
            this.AddServiceEndpoint(typeof(IWcfService), new BasicHttpBinding(), "");
            foreach (ServiceEndpoint endpoint in this.Description.Endpoints)
            {
                foreach (OperationDescription operation in endpoint.Contract.Operations)
                {
                    if (operation.Name == "GetAllEmployees" || operation.Name == "AddEmployee")
                    {
                        DataContractSerializerOperationBehavior behavior =
                            operation.OperationBehaviors.FirstOrDefault(
                                x => x.GetType() == typeof(DataContractSerializerOperationBehavior)) as DataContractSerializerOperationBehavior;
                        behavior.DataContractResolver = new ManagerDataContractResolver();
                    }
                }
            }
        }
        public DataContractResolverTestServiceHost(Type serviceType, params Uri[] baseAddresses)
            : base(serviceType, baseAddresses)
        {
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