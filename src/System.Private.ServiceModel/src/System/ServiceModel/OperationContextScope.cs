// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.



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
            PushContext(new OperationContext(channel));
        }

        public OperationContextScope(OperationContext context)
        {
            PushContext(context);
        }

        public void Dispose()
        {
            if (!_disposed)
            {
                _disposed = true;
                PopContext();
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
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new InvalidOperationException(SR.SFxInterleavedContextScopes0));
            }

            if (OperationContext.Current != _currentContext)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new InvalidOperationException(SR.SFxContextModifiedInsideScope0));
            }

            OperationContextScope.s_currentScope = _originalScope;
            OperationContext.Current = _originalContext;

            if (_currentContext != null)
            {
                _currentContext.SetClientReply(null, false);
            }
        }
    }
}

