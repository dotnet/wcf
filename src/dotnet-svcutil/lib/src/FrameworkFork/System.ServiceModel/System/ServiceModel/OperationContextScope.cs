// Copyright (c) Microsoft Corporation.
// Licensed under the MIT license.

using System;
using System.ServiceModel.Channels;
using System.Threading;

namespace System.ServiceModel
{
    public sealed class OperationContextScope : IDisposable
    {
        [ThreadStatic]
        private static OperationContextScope s_currentScope;

        private OperationContext _currentContext;
        private bool _disposed;
        private readonly OperationContext _originalContext = OperationContext.Current;
        private readonly OperationContextScope _originalScope = OperationContextScope.s_currentScope;

        public OperationContextScope(IContextChannel channel)
        {
            this.PushContext(new OperationContext(channel));
        }

        public OperationContextScope(OperationContext context)
        {
            this.PushContext(context);
        }

        public void Dispose()
        {
            if (!_disposed)
            {
                _disposed = true;
                this.PopContext();
            }
        }

        private void PushContext(OperationContext context)
        {
            _currentContext = context;
            OperationContextScope.s_currentScope = this;
            OperationContext.Current = _currentContext;
        }

        private void PopContext()
        {
            if (OperationContextScope.s_currentScope != this)
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new InvalidOperationException(SRServiceModel.SFxInterleavedContextScopes0));

            if (OperationContext.Current != _currentContext)
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new InvalidOperationException(SRServiceModel.SFxContextModifiedInsideScope0));

            OperationContextScope.s_currentScope = _originalScope;
            OperationContext.Current = _originalContext;

            if (_currentContext != null)
                _currentContext.SetClientReply(null, false);
        }
    }
}

