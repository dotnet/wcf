// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.ServiceModel.Description
{
    using System.Collections;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using Microsoft.Xml;
    using WsdlNS = System.Web.Services.Description;
    using System.Globalization;

    //
    // PolicyReader is a complex nested class in the MetadataImporter
    //
    public abstract partial class MetadataImporter
    {
        internal MetadataImporterQuotas Quotas;

        private PolicyReader _policyNormalizer = null;

        internal delegate void PolicyWarningHandler(XmlElement contextAssertion, string warningMessage);

        // Consider, hsomu, make this public?
        internal event PolicyWarningHandler PolicyWarningOccured;

        internal IEnumerable<IEnumerable<XmlElement>> NormalizePolicy(IEnumerable<XmlElement> policyAssertions)
        {
            if (_policyNormalizer == null)
            {
                _policyNormalizer = new PolicyReader(this);
            }

            return _policyNormalizer.NormalizePolicy(policyAssertions);
        }

        //DevNote: The error handling goal for this class is to NEVER throw an exception.
        //  * Any Ignored Policy should generate a warning
        //  * All policy parsing errors should be logged as warnings in the WSDLImporter.Errors collection.
        private sealed class PolicyReader
        {
            private int _nodesRead = 0;

            private readonly MetadataImporter _metadataImporter;

            internal PolicyReader(MetadataImporter metadataImporter)
            {
                _metadataImporter = metadataImporter;
            }

            private static IEnumerable<XmlElement> s_empty = new PolicyHelper.EmptyEnumerable<XmlElement>();
            private static IEnumerable<IEnumerable<XmlElement>> s_emptyEmpty = new PolicyHelper.SingleEnumerable<IEnumerable<XmlElement>>(new PolicyHelper.EmptyEnumerable<XmlElement>());

            //
            // the core policy reading logic
            // each step returns a list of lists -- an "and of ors": 
            // each inner list is a policy alternative: it contains the set of assertions that comprise the alternative
            // the outer list represents the choice between alternatives
            //

            private IEnumerable<IEnumerable<XmlElement>> ReadNode(XmlNode node, XmlElement contextAssertion, YieldLimiter yieldLimiter)
            {
                if (_nodesRead >= _metadataImporter.Quotas.MaxPolicyNodes)
                {
                    if (_nodesRead == _metadataImporter.Quotas.MaxPolicyNodes)
                    {
                        // add wirning once
                        string warningMsg = string.Format(SRServiceModel.ExceededMaxPolicyComplexity, node.Name, PolicyHelper.GetFragmentIdentifier((XmlElement)node));
                        _metadataImporter.PolicyWarningOccured.Invoke(contextAssertion, warningMsg);
                        _nodesRead++;
                    }
                    return s_emptyEmpty;
                }
                _nodesRead++;
                IEnumerable<IEnumerable<XmlElement>> nodes = s_emptyEmpty;
                switch (PolicyHelper.GetNodeType(node))
                {
                    case PolicyHelper.NodeType.Policy:
                    case PolicyHelper.NodeType.All:
                        nodes = ReadNode_PolicyOrAll((XmlElement)node, contextAssertion, yieldLimiter);
                        break;
                    case PolicyHelper.NodeType.ExactlyOne:
                        nodes = ReadNode_ExactlyOne((XmlElement)node, contextAssertion, yieldLimiter);
                        break;
                    case PolicyHelper.NodeType.Assertion:
                        nodes = ReadNode_Assertion((XmlElement)node, yieldLimiter);
                        break;
                    case PolicyHelper.NodeType.PolicyReference:
                        nodes = ReadNode_PolicyReference((XmlElement)node, contextAssertion, yieldLimiter);
                        break;
                    case PolicyHelper.NodeType.UnrecognizedWSPolicy:
                        string warningMsg = string.Format(SRServiceModel.UnrecognizedPolicyElementInNamespace, node.Name, node.NamespaceURI);
                        _metadataImporter.PolicyWarningOccured.Invoke(contextAssertion, warningMsg);
                        break;
                        //consider hsomu, add more error handling here. default?
                }
                return nodes;
            }

            private IEnumerable<IEnumerable<XmlElement>> ReadNode_PolicyReference(XmlElement element, XmlElement contextAssertion, YieldLimiter yieldLimiter)
            {
                string idRef = element.GetAttribute(MetadataStrings.WSPolicy.Attributes.URI);
                if (idRef == null)
                {
                    string warningMsg = string.Format(SRServiceModel.PolicyReferenceMissingURI, MetadataStrings.WSPolicy.Attributes.URI);
                    _metadataImporter.PolicyWarningOccured.Invoke(contextAssertion, warningMsg);
                    return s_emptyEmpty;
                }
                else if (idRef == string.Empty)
                {
                    string warningMsg = SRServiceModel.PolicyReferenceInvalidId;
                    _metadataImporter.PolicyWarningOccured.Invoke(contextAssertion, warningMsg);
                    return s_emptyEmpty;
                }

                XmlElement policy = _metadataImporter.ResolvePolicyReference(idRef, contextAssertion);
                if (policy == null)
                {
                    string warningMsg = string.Format(SRServiceModel.UnableToFindPolicyWithId, idRef);
                    _metadataImporter.PolicyWarningOccured.Invoke(contextAssertion, warningMsg);
                    return s_emptyEmpty;
                }

                //
                // Since we looked up a reference, the context assertion changes.
                //
                return ReadNode_PolicyOrAll(policy, policy, yieldLimiter);
            }

            private IEnumerable<IEnumerable<XmlElement>> ReadNode_Assertion(XmlElement element, YieldLimiter yieldLimiter)
            {
                if (yieldLimiter.IncrementAndLogIfExceededLimit())
                    yield return s_empty;
                else
                    yield return new PolicyHelper.SingleEnumerable<XmlElement>(element);
            }

            private IEnumerable<IEnumerable<XmlElement>> ReadNode_ExactlyOne(XmlElement element, XmlElement contextAssertion, YieldLimiter yieldLimiter)
            {
                foreach (XmlNode child in element.ChildNodes)
                {
                    if (child.NodeType == XmlNodeType.Element)
                    {
                        foreach (IEnumerable<XmlElement> alternative in ReadNode(child, contextAssertion, yieldLimiter))
                        {
                            if (yieldLimiter.IncrementAndLogIfExceededLimit())
                            {
                                yield break;
                            }
                            else
                            {
                                yield return alternative;
                            }
                        }
                    }
                }
            }

            private IEnumerable<IEnumerable<XmlElement>> ReadNode_PolicyOrAll(XmlElement element, XmlElement contextAssertion, YieldLimiter yieldLimiter)
            {
                IEnumerable<IEnumerable<XmlElement>> target = s_emptyEmpty;

                foreach (XmlNode child in element.ChildNodes)
                {
                    if (child.NodeType == XmlNodeType.Element)
                    {
                        IEnumerable<IEnumerable<XmlElement>> childPolicy = ReadNode(child, contextAssertion, yieldLimiter);
                        target = PolicyHelper.CrossProduct<XmlElement>(target, childPolicy, yieldLimiter);
                    }
                }
                return target;
            }

            internal IEnumerable<IEnumerable<XmlElement>> NormalizePolicy(IEnumerable<XmlElement> policyAssertions)
            {
                IEnumerable<IEnumerable<XmlElement>> target = s_emptyEmpty;
                YieldLimiter yieldLimiter = new YieldLimiter(_metadataImporter.Quotas.MaxYields, _metadataImporter);
                foreach (XmlElement child in policyAssertions)
                {
                    IEnumerable<IEnumerable<XmlElement>> childPolicy = ReadNode(child, child, yieldLimiter);
                    target = PolicyHelper.CrossProduct<XmlElement>(target, childPolicy, yieldLimiter);
                }

                return target;
            }
        }

        internal class YieldLimiter
        {
            private int _maxYields;
            private int _yieldsHit;
            private readonly MetadataImporter _metadataImporter;

            internal YieldLimiter(int maxYields, MetadataImporter metadataImporter)
            {
                _metadataImporter = metadataImporter;
                _yieldsHit = 0;
                _maxYields = maxYields;
            }

            internal bool IncrementAndLogIfExceededLimit()
            {
                if (++_yieldsHit > _maxYields)
                {
                    string warningMsg = SRServiceModel.ExceededMaxPolicySize;
                    _metadataImporter.PolicyWarningOccured.Invoke(null, warningMsg);
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }

        internal static class PolicyHelper
        {
            internal static string GetFragmentIdentifier(XmlElement element)
            {
                string id = element.GetAttribute(MetadataStrings.Wsu.Attributes.Id, MetadataStrings.Wsu.NamespaceUri);

                if (id == null)
                {
                    id = element.GetAttribute(MetadataStrings.Xml.Attributes.Id, MetadataStrings.Xml.NamespaceUri);
                }

                if (string.IsNullOrEmpty(id))
                    return string.Empty;
                else
                    return string.Format(CultureInfo.InvariantCulture, "#{0}", id);
            }

            internal static bool IsPolicyURIs(XmlAttribute attribute)
            {
                return ((attribute.NamespaceURI == MetadataStrings.WSPolicy.NamespaceUri
                    || attribute.NamespaceURI == MetadataStrings.WSPolicy.NamespaceUri15)
                            && attribute.LocalName == MetadataStrings.WSPolicy.Attributes.PolicyURIs);
            }

            internal static NodeType GetNodeType(XmlNode node)
            {
                XmlElement currentElement = node as XmlElement;

                if (currentElement == null)
                    return PolicyHelper.NodeType.NonElement;

                if (currentElement.NamespaceURI != MetadataStrings.WSPolicy.NamespaceUri
                    && currentElement.NamespaceURI != MetadataStrings.WSPolicy.NamespaceUri15)
                    return NodeType.Assertion;
                else if (currentElement.LocalName == MetadataStrings.WSPolicy.Elements.Policy)
                    return NodeType.Policy;
                else if (currentElement.LocalName == MetadataStrings.WSPolicy.Elements.All)
                    return NodeType.All;
                else if (currentElement.LocalName == MetadataStrings.WSPolicy.Elements.ExactlyOne)
                    return NodeType.ExactlyOne;
                else if (currentElement.LocalName == MetadataStrings.WSPolicy.Elements.PolicyReference)
                    return NodeType.PolicyReference;
                else
                    return PolicyHelper.NodeType.UnrecognizedWSPolicy;
            }

            // 
            // some helpers for dealing with ands of ors
            //

            internal static IEnumerable<IEnumerable<T>> CrossProduct<T>(IEnumerable<IEnumerable<T>> xs, IEnumerable<IEnumerable<T>> ys, YieldLimiter yieldLimiter)
            {
                foreach (IEnumerable<T> x in AtLeastOne<T>(xs, yieldLimiter))
                {
                    foreach (IEnumerable<T> y in AtLeastOne<T>(ys, yieldLimiter))
                    {
                        if (yieldLimiter.IncrementAndLogIfExceededLimit())
                        {
                            yield break;
                        }
                        else
                        {
                            yield return Merge<T>(x, y, yieldLimiter);
                        }
                    }
                }
            }

            private static IEnumerable<IEnumerable<T>> AtLeastOne<T>(IEnumerable<IEnumerable<T>> xs, YieldLimiter yieldLimiter)
            {
                bool gotOne = false;
                foreach (IEnumerable<T> x in xs)
                {
                    gotOne = true;

                    if (yieldLimiter.IncrementAndLogIfExceededLimit())
                    {
                        yield break;
                    }
                    else
                    {
                        yield return x;
                    }
                }
                if (!gotOne)
                {
                    if (yieldLimiter.IncrementAndLogIfExceededLimit())
                    {
                        yield break;
                    }
                    else
                    {
                        yield return new EmptyEnumerable<T>();
                    }
                }
            }

            private static IEnumerable<T> Merge<T>(IEnumerable<T> e1, IEnumerable<T> e2, YieldLimiter yieldLimiter)
            {
                foreach (T t1 in e1)
                {
                    if (yieldLimiter.IncrementAndLogIfExceededLimit())
                    {
                        yield break;
                    }
                    else
                    {
                        yield return t1;
                    }
                }
                foreach (T t2 in e2)
                {
                    if (yieldLimiter.IncrementAndLogIfExceededLimit())
                    {
                        yield break;
                    }
                    else
                    {
                        yield return t2;
                    }
                }
            }

            //
            // some helper enumerators
            //

            internal class EmptyEnumerable<T> : IEnumerable<T>, IEnumerator<T>
            {
                IEnumerator IEnumerable.GetEnumerator()
                {
                    return this.GetEnumerator();
                }

                public IEnumerator<T> GetEnumerator()
                {
                    return this;
                }

                object IEnumerator.Current
                {
                    get { return this.Current; }
                }

                public T Current
                {
                    get
                    {
#pragma warning disable 56503 // jruiz, IEnumerator guidelines, Current throws exception before calling MoveNext
                        throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new InvalidOperationException(SRServiceModel.NoValue0));
                    }
                }

                public bool MoveNext()
                {
                    return false;
                }

                public void Dispose()
                {
                }

                void IEnumerator.Reset()
                {
                }
            }

            internal class SingleEnumerable<T> : IEnumerable<T>
            {
                private T _value;

                internal SingleEnumerable(T value)
                {
                    _value = value;
                }

                IEnumerator IEnumerable.GetEnumerator()
                {
                    return this.GetEnumerator();
                }

                public IEnumerator<T> GetEnumerator()
                {
                    yield return _value;
                }
            }

            //
            // the NodeType enum
            //
            internal enum NodeType
            {
                NonElement,
                Policy,
                All,
                ExactlyOne,
                Assertion,
                PolicyReference,
                UnrecognizedWSPolicy,
            }
        }
    }
}
