// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.


using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Runtime;
using System.ServiceModel.Channels;
using System.Threading;

namespace System.ServiceModel
{
    public class MessageHeader<T>
    {
        private bool _relay;

        public MessageHeader()
        {
        }

        public MessageHeader(T content)
            : this(content, false, "", false)
        {
        }

        public MessageHeader(T content, bool mustUnderstand, string actor, bool relay)
        {
            Content = content;
            MustUnderstand = mustUnderstand;
            Actor = actor;
            _relay = relay;
        }

        public string Actor { get; set; }

        public T Content { get; set; }

        public bool MustUnderstand { get; set; }

        public bool Relay
        {
            get { return _relay; }
            set { _relay = value; }
        }

        internal Type GetGenericArgument()
        {
            return typeof(T);
        }

        public MessageHeader GetUntypedHeader(string name, string ns)
        {
            return MessageHeader.CreateHeader(name, ns, Content, MustUnderstand, Actor, _relay);
        }
    }

    // problem: creating / getting content / settings content on a MessageHeader<T> given the type at runtime
    // require reflection.
    // solution: This class creates a cache of adapters that provide an untyped wrapper over a particular
    // MessageHeader<T> instantiation.
    // better solution: implement something like "IUntypedTypedHeader" that has a "object Content" property,
    // privately implement this on TypedHeader, and then just use that iface to operation on the header (actually
    // you'd still have the creation problem...).  the issue with that is you now have a new public interface
    internal abstract class TypedHeaderManager
    {
        private static Dictionary<Type, TypedHeaderManager> s_cache = new Dictionary<Type, TypedHeaderManager>();
        private static ReaderWriterLockSlim s_cacheLock = new ReaderWriterLockSlim();
        private static Type s_GenericAdapterType = typeof(GenericAdapter<>);

        internal static object Create(Type t, object content, bool mustUnderstand, bool relay, string actor)
        {
            return GetTypedHeaderManager(t).Create(content, mustUnderstand, relay, actor);
        }

        internal static object GetContent(Type t, object typedHeaderInstance, out bool mustUnderstand, out bool relay, out string actor)
        {
            return GetTypedHeaderManager(t).GetContent(typedHeaderInstance, out mustUnderstand, out relay, out actor);
        }

        internal static Type GetMessageHeaderType(Type contentType)
        {
            return GetTypedHeaderManager(contentType).GetMessageHeaderType();
        }
        internal static Type GetHeaderType(Type headerParameterType)
        {
            if (headerParameterType.IsGenericType() && headerParameterType.GetGenericTypeDefinition() == typeof(MessageHeader<>))
            {
                return headerParameterType.GetGenericArguments()[0];
            }

            return headerParameterType;
        }

        [SuppressMessage(FxCop.Category.Usage, "CA2301:EmbeddableTypesInContainersRule", MessageId = "cache", Justification = "No need to support type equivalence here.")]
        private static TypedHeaderManager GetTypedHeaderManager(Type t)
        {
            TypedHeaderManager result = null;

            bool readerLockHeld = false;
            bool writerLockHeld = false;
            try
            {
                try { }
                finally
                {
                    s_cacheLock.TryEnterUpgradeableReadLock(Timeout.Infinite);
                    readerLockHeld = true;
                }
                if (!s_cache.TryGetValue(t, out result))
                {
                    s_cacheLock.TryEnterWriteLock(Timeout.Infinite);
                    writerLockHeld = true;
                    if (!s_cache.TryGetValue(t, out result))
                    {
                        result = (TypedHeaderManager)Activator.CreateInstance(s_GenericAdapterType.MakeGenericType(t));
                        s_cache.Add(t, result);
                    }
                }
            }
            finally
            {
                if (writerLockHeld)
                {
                    s_cacheLock.ExitWriteLock();
                }
                if (readerLockHeld)
                {
                    s_cacheLock.ExitUpgradeableReadLock();
                }
            }

            return result;
        }

        protected abstract object Create(object content, bool mustUnderstand, bool relay, string actor);
        protected abstract object GetContent(object typedHeaderInstance, out bool mustUnderstand, out bool relay, out string actor);
        protected abstract Type GetMessageHeaderType();

        private class GenericAdapter<T> : TypedHeaderManager
        {
            protected override object Create(object content, bool mustUnderstand, bool relay, string actor)
            {
                MessageHeader<T> header = new MessageHeader<T>();
                header.Content = (T)content;
                header.MustUnderstand = mustUnderstand;
                header.Relay = relay;
                header.Actor = actor;
                return header;
            }

            protected override object GetContent(object typedHeaderInstance, out bool mustUnderstand, out bool relay, out string actor)
            {
                mustUnderstand = false;
                relay = false;
                actor = null;
                if (typedHeaderInstance == null)
                {
                    return null;
                }

                MessageHeader<T> header = typedHeaderInstance as MessageHeader<T>;
                if (header == null)
                {
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentException("typedHeaderInstance"));
                }

                mustUnderstand = header.MustUnderstand;
                relay = header.Relay;
                actor = header.Actor;
                return header.Content;
            }

            protected override Type GetMessageHeaderType()
            {
                return typeof(MessageHeader<T>);
            }
        }
    }
}
