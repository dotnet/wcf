// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using System.Globalization;
using System.Net.Http;
using System.Runtime;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

namespace System.ServiceModel.Channels
{
    internal static class WebSocketHelper
    {
        internal const int OperationNotStarted = 0;
        internal const int OperationFinished = 1;

        internal const string SecWebSocketKey = "Sec-WebSocket-Key";
        internal const string SecWebSocketVersion = "Sec-WebSocket-Version";
        internal const string SecWebSocketProtocol = "Sec-WebSocket-Protocol";
        internal const string SecWebSocketAccept = "Sec-WebSocket-Accept";
        internal const string MaxPendingConnectionsString = "MaxPendingConnections";
        internal const string WebSocketTransportSettingsString = "WebSocketTransportSettings";

        internal const string CloseOperation = "CloseOperation";
        internal const string SendOperation = "SendOperation";
        internal const string ReceiveOperation = "ReceiveOperation";

        internal static readonly char[] ProtocolSeparators = new char[] { ',' };

        private const string SchemeWs = "ws";
        private const string SchemeWss = "wss";

        private static readonly HashSet<char> s_InvalidSeparatorSet = new HashSet<char>(new char[] { '(', ')', '<', '>', '@', ',', ';', ':', '\\', '"', '/', '[', ']', '?', '=', '{', '}', ' ' });

        internal static int GetReceiveBufferSize(long maxReceivedMessageSize)
        {
            int effectiveMaxReceiveBufferSize = maxReceivedMessageSize <= WebSocketDefaults.BufferSize ? (int)maxReceivedMessageSize : WebSocketDefaults.BufferSize;
            return Math.Max(WebSocketDefaults.MinReceiveBufferSize, effectiveMaxReceiveBufferSize);
        }

        internal static bool UseWebSocketTransport(WebSocketTransportUsage transportUsage, bool isContractDuplex)
        {
            return transportUsage == WebSocketTransportUsage.Always
                || (transportUsage == WebSocketTransportUsage.WhenDuplex && isContractDuplex);
        }

        internal static bool IsWebSocketUri(Uri uri)
        {
            return uri != null &&
                (WebSocketHelper.SchemeWs.Equals(uri.Scheme, StringComparison.OrdinalIgnoreCase) ||
                 WebSocketHelper.SchemeWss.Equals(uri.Scheme, StringComparison.OrdinalIgnoreCase));
        }

        internal static Uri NormalizeHttpSchemeWithWsScheme(Uri uri)
        {
            Fx.Assert(uri != null, "RemoteAddress.Uri should not be null.");
            if (IsWebSocketUri(uri))
            {
                return uri;
            }

            UriBuilder builder = new UriBuilder(uri);

            switch (uri.Scheme.ToLowerInvariant())
            {
                case UriEx.UriSchemeHttp:
                    builder.Scheme = SchemeWs;
                    break;
                case UriEx.UriSchemeHttps:
                    builder.Scheme = SchemeWss;
                    break;
                default:
                    break;
            }

            return builder.Uri;
        }

        internal static bool IsSubProtocolInvalid(string protocol, out string invalidChar)
        {
            Fx.Assert(protocol != null, "protocol should not be null");
            char[] chars = protocol.ToCharArray();
            for (int i = 0; i < chars.Length; i++)
            {
                char ch = chars[i];
                if (ch < 0x21 || ch > 0x7e)
                {
                    invalidChar = string.Format(CultureInfo.InvariantCulture, "[{0}]", (int)ch);
                    return true;
                }

                if (s_InvalidSeparatorSet.Contains(ch))
                {
                    invalidChar = ch.ToString();
                    return true;
                }
            }

            invalidChar = null;
            return false;
        }

        internal static WebSocketTransportSettings GetRuntimeWebSocketSettings(WebSocketTransportSettings settings)
        {
            WebSocketTransportSettings runtimeSettings = settings.Clone();
            return runtimeSettings;
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage(FxCop.Category.ReliabilityBasic, FxCop.Rule.WrapExceptionsRule,
                    Justification = "The exceptions thrown here are already wrapped.")]
        internal static void ThrowCorrectException(Exception ex)
        {
            throw ConvertAndTraceException(ex);
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage(FxCop.Category.ReliabilityBasic, FxCop.Rule.WrapExceptionsRule,
                    Justification = "The exceptions thrown here are already wrapped.")]
        internal static void ThrowCorrectException(Exception ex, TimeSpan timeout, string operation)
        {
            throw ConvertAndTraceException(ex, timeout, operation);
        }

        internal static Exception ConvertAndTraceException(Exception ex)
        {
            return ConvertAndTraceException(
                    ex,
                    TimeSpan.MinValue, // this is a dummy since operation type is null, so the timespan value won't be used
                    null);
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage(FxCop.Category.ReliabilityBasic, "Reliability103:ThrowWrappedExceptionsRule",
                    Justification = "The exceptions wrapped here will be thrown out later.")]
        internal static Exception ConvertAndTraceException(Exception ex, TimeSpan timeout, string operation)
        {
            ObjectDisposedException objectDisposedException = ex as ObjectDisposedException;
            if (objectDisposedException != null)
            {
                CommunicationObjectAbortedException communicationObjectAbortedException = new CommunicationObjectAbortedException(ex.Message, ex);
                FxTrace.Exception.AsWarning(communicationObjectAbortedException);
                return communicationObjectAbortedException;
            }

            AggregateException aggregationException = ex as AggregateException;
            if (aggregationException != null)
            {
                Exception exception = FxTrace.Exception.AsError<OperationCanceledException>(aggregationException);
                OperationCanceledException operationCanceledException = exception as OperationCanceledException;
                if (operationCanceledException != null)
                {
                    TimeoutException timeoutException = GetTimeoutException(exception, timeout, operation);
                    FxTrace.Exception.AsWarning(timeoutException);
                    return timeoutException;
                }
                else
                {
                    Exception communicationException = ConvertAggregateExceptionToCommunicationException(aggregationException);
                    if (communicationException is CommunicationObjectAbortedException)
                    {
                        FxTrace.Exception.AsWarning(communicationException);
                        return communicationException;
                    }
                    else
                    {
                        return FxTrace.Exception.AsError(communicationException);
                    }
                }
            }

            return FxTrace.Exception.AsError(ex);
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage(FxCop.Category.ReliabilityBasic, "Reliability103",
                            Justification = "The exceptions will be wrapped by the callers.")]
        internal static Exception ConvertAggregateExceptionToCommunicationException(AggregateException ex)
        {
            Exception exception = FxTrace.Exception.AsError<Exception>(ex);

            ObjectDisposedException objectDisposedException = exception as ObjectDisposedException;
            if (objectDisposedException != null)
            {
                return new CommunicationObjectAbortedException(exception.Message, exception);
            }

            return new CommunicationException(exception.Message, exception);
        }

        internal static void ThrowExceptionOnTaskFailure(Task task, TimeSpan timeout, string operation)
        {
            if (task.IsFaulted)
            {
                throw FxTrace.Exception.AsError<CommunicationException>(task.Exception);
            }
            if (task.IsCanceled)
            {
                throw FxTrace.Exception.AsError(GetTimeoutException(null, timeout, operation));
            }
        }

        // TODO: Move to correct place alphabetically, it's here temporariliy to make editting easier
        internal static Exception CreateExceptionOnTaskFailure(Task task, TimeSpan timeout, string operation)
        {
            if (task.IsFaulted)
            {
                return FxTrace.Exception.AsError<CommunicationException>(task.Exception);
            }
            if (task.IsCanceled)
            {
                throw FxTrace.Exception.AsError(GetTimeoutException(null, timeout, operation));
            }
            return null;
        }

        internal static TimeoutException GetTimeoutException(Exception innerException, TimeSpan timeout, string operation)
        {
            string errorMsg = string.Empty;
            if (operation != null)
            {
                switch (operation)
                {
                    case WebSocketHelper.CloseOperation:
                        errorMsg = string.Format(SRServiceModel.CloseTimedOut, timeout);
                        break;
                    case WebSocketHelper.SendOperation:
                        errorMsg = string.Format(SRServiceModel.WebSocketSendTimedOut, timeout);
                        break;
                    case WebSocketHelper.ReceiveOperation:
                        errorMsg = string.Format(SRServiceModel.WebSocketReceiveTimedOut, timeout);
                        break;
                    default:
                        errorMsg = string.Format(SRServiceModel.WebSocketOperationTimedOut, operation, timeout);
                        break;
                }
            }

            return innerException == null ? new TimeoutException(errorMsg) : new TimeoutException(errorMsg, innerException);
        }
    }
}
