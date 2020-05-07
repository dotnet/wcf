// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using Microsoft.Xml;
using System.ServiceModel;
using Microsoft.Xml.Serialization;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.ServiceModel.Description;

namespace System.ServiceModel.Dispatcher
{
    internal class XmlSerializerObjectSerializer : XmlObjectSerializer
    {
        private XmlSerializer _serializer;
        private Type _rootType;
        private string _rootName;
        private string _rootNamespace;
        private bool _isSerializerSetExplicit = false;

        internal XmlSerializerObjectSerializer(Type type)
        {
            Initialize(type, null /*rootName*/, null /*rootNamespace*/, null /*xmlSerializer*/);
        }

        internal XmlSerializerObjectSerializer(Type type, XmlQualifiedName qualifiedName, XmlSerializer xmlSerializer)
        {
            if (qualifiedName == null)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("qualifiedName");
            }
            Initialize(type, qualifiedName.Name, qualifiedName.Namespace, xmlSerializer);
        }

        private void Initialize(Type type, string rootName, string rootNamespace, XmlSerializer xmlSerializer)
        {
            if (type == null)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("type");
            }
            _rootType = type;
            _rootName = rootName;
            _rootNamespace = rootNamespace == null ? string.Empty : rootNamespace;
            _serializer = xmlSerializer;

            if (_serializer == null)
            {
                if (_rootName == null)
                    _serializer = new XmlSerializer(type);
                else
                {
                    XmlRootAttribute xmlRoot = new XmlRootAttribute();
                    xmlRoot.ElementName = _rootName;
                    xmlRoot.Namespace = _rootNamespace;
                    _serializer = new XmlSerializer(type, xmlRoot);
                }
            }
            else
                _isSerializerSetExplicit = true;

            //try to get rootName and rootNamespace from type since root name not set explicitly
            if (_rootName == null)
            {
                XmlTypeMapping mapping = new XmlReflectionImporter(null, null).ImportTypeMapping(_rootType);
                _rootName = mapping.ElementName;
                _rootNamespace = mapping.Namespace;
            }
        }

        public override void WriteObject(XmlDictionaryWriter writer, object graph)
        {
            if (_isSerializerSetExplicit)
                _serializer.Serialize(writer, new object[] { graph });
            else
                _serializer.Serialize(writer, graph);
        }

        public override void WriteStartObject(XmlDictionaryWriter writer, object graph)
        {
            throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(NotImplemented.ByDesign);
        }

        public override void WriteObjectContent(XmlDictionaryWriter writer, object graph)
        {
            throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(NotImplemented.ByDesign);
        }

        public override void WriteEndObject(XmlDictionaryWriter writer)
        {
            throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(NotImplemented.ByDesign);
        }

        public override object ReadObject(XmlDictionaryReader reader, bool verifyObjectName)
        {
            if (_isSerializerSetExplicit)
            {
                object[] deserializedObjects = (object[])_serializer.Deserialize(reader);
                if (deserializedObjects != null && deserializedObjects.Length > 0)
                    return deserializedObjects[0];
                else
                    return null;
            }
            else
                return _serializer.Deserialize(reader);
        }

        public override bool IsStartObject(XmlDictionaryReader reader)
        {
            if (reader == null)
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentNullException("reader"));

            reader.MoveToElement();

            if (_rootName != null)
            {
                return reader.IsStartElement(_rootName, _rootNamespace);
            }
            else
            {
                return reader.IsStartElement();
            }
        }
    }
}

