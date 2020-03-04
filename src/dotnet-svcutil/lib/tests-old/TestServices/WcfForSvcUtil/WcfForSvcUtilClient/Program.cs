using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Threading.Tasks;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Description;
using System.Net;
using System.Xml;
using System.Xml.Schema;

namespace WcfForSvcUtil
{
    class Program
    {
        static void Main(string[] args)
        {
            EndpointAddress address = new EndpointAddress("http://localhost/WcfForSvcUtil/Service1.svc");
            Binding binding = new BasicHttpBinding();
            ChannelFactory<IService1> contractFactory = new ChannelFactory<IService1>(binding, address);
            IService1 proxy = contractFactory.CreateChannel();
            Console.WriteLine(proxy.GetData(10));

            try
            {
                binding = new WSHttpBinding();
                ChannelFactory<IMetadataExchange> metaFactory = new ChannelFactory<IMetadataExchange>(binding, address);
                IMetadataExchange metaProxy = metaFactory.CreateChannel();
                Console.WriteLine(metaProxy.Get(Message.CreateMessage(binding.MessageVersion, "http://schemas.xmlsoap.org/ws/2004/09/transfer/Get")));
            }
            catch
            {
            }

            WebClient client = new WebClient();
            client.BaseAddress = address.ToString();
            byte[] data = client.DownloadData(address + "?singleWsdl");
            StreamReader reader = new StreamReader(new MemoryStream(data));
            string txt = reader.ReadToEnd();
            XmlDocument doc = new XmlDocument();
            doc.LoadXml(txt);
            XmlNodeList schemaSections = doc.GetElementsByTagName("xs:schema");

            XmlSchemaSet coll = new XmlSchemaSet();

            foreach (XmlNode section in schemaSections)
            {
                Stream xmlStream = new MemoryStream(Encoding.ASCII.GetBytes(section.OuterXml));
                var schema = XmlSchema.Read(xmlStream, null);
                coll.Add(schema);
            }

            Stream txtStream = new MemoryStream(Encoding.ASCII.GetBytes(txt));
            XmlReader xmlReader = XmlReader.Create(txtStream);



            //    XmlSchema schema = XmlSchema.Read(reader, null);

            MetadataExchangeClient mexClient = new MetadataExchangeClient(address);
            mexClient.ResolveMetadataReferences = false;
            MetadataSet metaSet = mexClient.GetMetadata();

            Console.WriteLine(metaSet.ToString());
        }
    }
}
