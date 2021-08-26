// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Xml;

namespace System.Web.Services.Description
{
    public sealed class ServiceDescriptionCollection : ServiceDescriptionBaseCollection {
        public ServiceDescriptionCollection() : base(null) { }

        public ServiceDescription this[int index] {
            get { return (ServiceDescription)List[index]; }
            set { List[index] = value; }
        }

        public ServiceDescription this[string ns] {
            get { return (ServiceDescription)Table[ns]; }
        }

        public int Add(ServiceDescription serviceDescription) {
            return List.Add(serviceDescription);
        }

        public void Insert(int index, ServiceDescription serviceDescription) {
            List.Insert(index, serviceDescription);
        }

        public int IndexOf(ServiceDescription serviceDescription) {
            return List.IndexOf(serviceDescription);
        }

        public bool Contains(ServiceDescription serviceDescription) {
            return List.Contains(serviceDescription);
        }

        public void Remove(ServiceDescription serviceDescription) {
            List.Remove(serviceDescription);
        }

        public void CopyTo(ServiceDescription[] array, int index) {
            List.CopyTo(array, index);
        }

        protected override string GetKey(object value) {
            string ns = ((ServiceDescription)value).TargetNamespace;
            if (ns == null)
            {
                return string.Empty;
            }

            return ns;
        }

        private Exception ItemNotFound(XmlQualifiedName name, string type) {
            return new Exception(SR.Format(SR.WebDescriptionMissingItem, type, name.Name, name.Namespace));
        }

        public Message GetMessage(XmlQualifiedName name) {
            ServiceDescription sd = GetServiceDescription(name);
            Message message = null;
            while (message == null && sd != null) {
                message = sd.Messages[name.Name];
                sd = sd.Next;
            }
            if (message == null)
            {
                throw ItemNotFound(name, "message");
            }

            return message;
        }

        public PortType GetPortType(XmlQualifiedName name) {
            ServiceDescription sd = GetServiceDescription(name);
            PortType portType = null;
            while (portType == null && sd != null) {
                portType = sd.PortTypes[name.Name];
                sd = sd.Next;
            }
            if (portType == null)
            {
                throw ItemNotFound(name, "message");
            }

            return portType;
        }

        public Service GetService(XmlQualifiedName name) {
            ServiceDescription sd = GetServiceDescription(name);
            Service service = null;
            while (service == null && sd != null) {
                service = sd.Services[name.Name];
                sd = sd.Next;
            }
            if (service == null)
            {
                throw ItemNotFound(name, "service");
            }

            return service;
        }

        public Binding GetBinding(XmlQualifiedName name) {
            ServiceDescription sd = GetServiceDescription(name);
            Binding binding = null;
            while (binding == null && sd != null) {
                binding = sd.Bindings[name.Name];
                sd = sd.Next;
            }
            if (binding == null)
            {
                throw ItemNotFound(name, nameof(binding));
            }

            return binding;
        }

        private ServiceDescription GetServiceDescription(XmlQualifiedName name) {
            ServiceDescription serviceDescription = this[name.Namespace];
            if (serviceDescription == null) {
                throw new ArgumentException(SR.Format(SR.WebDescriptionMissing, name.ToString(), name.Namespace), nameof(name));
            }

            return serviceDescription;
        }

        protected override void SetParent(object value, object parent) {
            ((ServiceDescription)value).SetParent((ServiceDescriptionCollection)parent);
        }

        protected override void OnInsertComplete(int index, object value) {
            string key = GetKey(value);
            if (key != null) {
                ServiceDescription item = (ServiceDescription)Table[key];
                ((ServiceDescription)value).Next = (ServiceDescription)Table[key];
                Table[key] = value;
            }
            SetParent(value, this);
        }
    }
}
