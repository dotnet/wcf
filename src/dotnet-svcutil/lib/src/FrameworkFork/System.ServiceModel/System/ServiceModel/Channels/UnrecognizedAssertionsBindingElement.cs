// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.ServiceModel.Channels
{
    using System.Collections;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Runtime;
    using System.ServiceModel.Description;
    using Microsoft.Xml;

    internal class UnrecognizedAssertionsBindingElement : BindingElement
    {
        private XmlQualifiedName _wsdlBinding;
        private ICollection<XmlElement> _bindingAsserions;
        private IDictionary<OperationDescription, ICollection<XmlElement>> _operationAssertions;
        private IDictionary<MessageDescription, ICollection<XmlElement>> _messageAssertions;

        internal protected UnrecognizedAssertionsBindingElement(XmlQualifiedName wsdlBinding, ICollection<XmlElement> bindingAsserions)
        {
            Fx.Assert(wsdlBinding != null, "");
            _wsdlBinding = wsdlBinding;
            _bindingAsserions = bindingAsserions;
        }

        internal XmlQualifiedName WsdlBinding
        {
            get { return _wsdlBinding; }
        }

        internal ICollection<XmlElement> BindingAsserions
        {
            get
            {
                if (_bindingAsserions == null)
                    _bindingAsserions = new Collection<XmlElement>();
                return _bindingAsserions;
            }
        }

        internal IDictionary<OperationDescription, ICollection<XmlElement>> OperationAssertions
        {
            get
            {
                if (_operationAssertions == null)
                    _operationAssertions = new Dictionary<OperationDescription, ICollection<XmlElement>>();
                return _operationAssertions;
            }
        }

        internal IDictionary<MessageDescription, ICollection<XmlElement>> MessageAssertions
        {
            get
            {
                if (_messageAssertions == null)
                    _messageAssertions = new Dictionary<MessageDescription, ICollection<XmlElement>>();
                return _messageAssertions;
            }
        }

        internal void Add(OperationDescription operation, ICollection<XmlElement> assertions)
        {
            ICollection<XmlElement> existent;
            if (!OperationAssertions.TryGetValue(operation, out existent))
            {
                OperationAssertions.Add(operation, assertions);
            }
            else
            {
                foreach (XmlElement assertion in assertions)
                    existent.Add(assertion);
            }
        }

        internal void Add(MessageDescription message, ICollection<XmlElement> assertions)
        {
            ICollection<XmlElement> existent;
            if (!MessageAssertions.TryGetValue(message, out existent))
            {
                MessageAssertions.Add(message, assertions);
            }
            else
            {
                foreach (XmlElement assertion in assertions)
                    existent.Add(assertion);
            }
        }

        protected UnrecognizedAssertionsBindingElement(UnrecognizedAssertionsBindingElement elementToBeCloned)
            : base(elementToBeCloned)
        {
            _wsdlBinding = elementToBeCloned._wsdlBinding;
            _bindingAsserions = elementToBeCloned._bindingAsserions;
            _operationAssertions = elementToBeCloned._operationAssertions;
            _messageAssertions = elementToBeCloned._messageAssertions;
        }

        public override T GetProperty<T>(BindingContext context)
        {
            if (context == null)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("context");
            }
            return context.GetInnerProperty<T>();
        }

        public override BindingElement Clone()
        {
            //throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new InvalidOperationException(SR.Format(SR.UnsupportedBindingElementClone, typeof(UnrecognizedAssertionsBindingElement).Name)));
            // do not allow Cloning, return an empty BindingElement
            return new UnrecognizedAssertionsBindingElement(new XmlQualifiedName(_wsdlBinding.Name, _wsdlBinding.Namespace), null);
        }
    }
}
