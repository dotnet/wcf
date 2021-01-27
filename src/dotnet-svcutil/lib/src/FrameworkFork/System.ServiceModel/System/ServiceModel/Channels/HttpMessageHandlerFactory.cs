// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using System.Globalization;
using System.Net.Http;
using System.Reflection;
using System.Runtime;
using System.Runtime.CompilerServices;

namespace System.ServiceModel.Channels
{
    /// <summary>
    /// Default HTTP message handler factory used by <see cref="HttpChannelListener"/> upon creation of an <see cref="HttpMessageHandler"/> 
    /// for instantiating a set of HTTP message handler types using their default constructors.
    /// For more complex initialization scenarios, derive from <see cref="HttpMessageHandlerFactory"/>
    /// and override the <see cref="HttpMessageHandlerFactory.OnCreate"/> method.
    /// </summary>
    public class HttpMessageHandlerFactory
    {
        private static readonly Type s_delegatingHandlerType = typeof(DelegatingHandler);

        private Type[] _httpMessageHandlers;
        private ConstructorInfo[] _handlerCtors;
        private Func<IEnumerable<DelegatingHandler>> _handlerFunc;

        /// <summary>
        /// Initializes a new instance of the <see cref="HttpMessageHandlerFactory"/> class given
        /// a set of HTTP message handler types to instantiate using their default constructors.
        /// </summary>
        /// <param name="handlers">An ordered list of HTTP message handler types to be invoked as part of an 
        /// <see cref="HttpMessageHandler"/> instance.
        /// HTTP message handler types must derive from <see cref="DelegatingHandler"/> and have a public constructor
        /// taking exactly one argument of type <see cref="HttpMessageHandler"/>. The handlers are invoked in a 
        /// bottom-up fashion in the incoming path and top-down in the outgoing path. That is, the last entry is called first 
        /// for an incoming request message but invoked last for an outgoing response message.</param>
        [MethodImpl(MethodImplOptions.NoInlining)]
        public HttpMessageHandlerFactory(params Type[] handlers)
        {
            if (handlers == null)
            {
                throw FxTrace.Exception.ArgumentNull("handlers");
            }

            if (handlers.Length == 0)
            {
                throw FxTrace.Exception.Argument("handlers", SRServiceModel.InputTypeListEmptyError);
            }

            _handlerCtors = new ConstructorInfo[handlers.Length];
            for (int cnt = 0; cnt < handlers.Length; cnt++)
            {
                Type handler = handlers[cnt];
                if (handler == null)
                {
                    throw FxTrace.Exception.Argument(
                        string.Format(CultureInfo.InvariantCulture, "handlers[<<{0}>>]", cnt),
                        string.Format(SRServiceModel.HttpMessageHandlerTypeNotSupported, "null", s_delegatingHandlerType.Name));
                }

                if (!s_delegatingHandlerType.IsAssignableFrom(handler) || handler.IsAbstract())
                {
                    throw FxTrace.Exception.Argument(
                        string.Format(CultureInfo.InvariantCulture, "handlers[<<{0}>>]", cnt),
                        string.Format(SRServiceModel.HttpMessageHandlerTypeNotSupported, handler.Name, s_delegatingHandlerType.Name));
                }

                ConstructorInfo ctorInfo = handler.GetConstructor(Array.Empty<Type>());
                if (ctorInfo == null)
                {
                    throw FxTrace.Exception.Argument(
                        string.Format(CultureInfo.InvariantCulture, "handlers[<<{0}>>]", cnt),
                        string.Format(SRServiceModel.HttpMessageHandlerTypeNotSupported, handler.Name, s_delegatingHandlerType.Name));
                }

                _handlerCtors[cnt] = ctorInfo;
            }

            _httpMessageHandlers = handlers;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="HttpMessageHandlerFactory"/> class given
        /// a function to create a set of <see cref="DelegatingHandler"/> instances.
        /// </summary>
        /// <param name="handlers">A function to generate an ordered list of <see cref="DelegatingHandler"/> instances 
        /// to be invoked as part of an <see cref="HttpMessageHandler"/> instance.
        /// The handlers are invoked in a bottom-up fashion in the incoming path and top-down in the outgoing path. That is, 
        /// the last entry is called first for an incoming request message but invoked last for an outgoing response message.</param>
        [MethodImpl(MethodImplOptions.NoInlining)]
        public HttpMessageHandlerFactory(Func<IEnumerable<DelegatingHandler>> handlers)
        {
            if (handlers == null)
            {
                throw FxTrace.Exception.ArgumentNull("handlers");
            }

            _handlerFunc = handlers;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="HttpMessageHandlerFactory"/> class.
        /// </summary>
        [MethodImpl(MethodImplOptions.NoInlining)]
        protected HttpMessageHandlerFactory()
        {
        }

        /// <summary>
        /// Creates an instance of an <see cref="HttpMessageHandler"/> using the HTTP message handlers
        /// provided in the constructor.
        /// </summary>
        /// <param name="innerChannel">The inner channel represents the destination of the HTTP message channel.</param>
        /// <returns>The HTTP message channel.</returns>
        [MethodImpl(MethodImplOptions.NoInlining)]
        public HttpMessageHandler Create(HttpMessageHandler innerChannel)
        {
            if (innerChannel == null)
            {
                throw FxTrace.Exception.ArgumentNull("innerChannel");
            }

            return this.OnCreate(innerChannel);
        }


        /// <summary>
        /// Creates an instance of an <see cref="HttpMessageHandler"/> using the HTTP message handlers
        /// provided in the constructor.
        /// </summary>
        /// <param name="innerChannel">The inner channel represents the destination of the HTTP message channel.</param>
        /// <returns>The HTTP message channel.</returns>
        protected virtual HttpMessageHandler OnCreate(HttpMessageHandler innerChannel)
        {
            if (innerChannel == null)
            {
                throw FxTrace.Exception.ArgumentNull("innerChannel");
            }

            // Get handlers either by constructing types or by calling Func
            IEnumerable<DelegatingHandler> handlerInstances = null;
            try
            {
                if (_handlerFunc != null)
                {
                    handlerInstances = _handlerFunc.Invoke();
                    if (handlerInstances != null)
                    {
                        foreach (DelegatingHandler handler in handlerInstances)
                        {
                            if (handler == null)
                            {
                                throw FxTrace.Exception.Argument("handlers", string.Format(SRServiceModel.DelegatingHandlerArrayFromFuncContainsNullItem, s_delegatingHandlerType.Name, GetFuncDetails(_handlerFunc)));
                            }
                        }
                    }
                }
                else if (_handlerCtors != null)
                {
                    DelegatingHandler[] instances = new DelegatingHandler[_handlerCtors.Length];
                    for (int cnt = 0; cnt < _handlerCtors.Length; cnt++)
                    {
                        instances[cnt] = (DelegatingHandler)_handlerCtors[cnt].Invoke(Array.Empty<Type>());
                    }

                    handlerInstances = instances;
                }
            }
            catch (TargetInvocationException targetInvocationException)
            {
                throw FxTrace.Exception.AsError(targetInvocationException);
            }

            // Wire handlers up
            HttpMessageHandler pipeline = innerChannel;
            if (handlerInstances != null)
            {
                foreach (DelegatingHandler handler in handlerInstances)
                {
                    if (handler.InnerHandler != null)
                    {
                        throw FxTrace.Exception.Argument("handlers", string.Format(SRServiceModel.DelegatingHandlerArrayHasNonNullInnerHandler, s_delegatingHandlerType.Name, "InnerHandler", handler.GetType().Name));
                    }

                    handler.InnerHandler = pipeline;
                    pipeline = handler;
                }
            }

            return pipeline;
        }

        private static string GetFuncDetails(Func<IEnumerable<DelegatingHandler>> func)
        {
            Fx.Assert(func != null, "Func should not be null.");
            throw ExceptionHelper.PlatformNotSupported();
        }
    }
}
