// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.


using System.Globalization;

namespace System.ServiceModel.Channels
{
    public abstract class FaultConverter
    {
        public static FaultConverter GetDefaultFaultConverter(MessageVersion version)
        {
            return new DefaultFaultConverter(version);
        }

        protected abstract bool OnTryCreateException(Message message, MessageFault fault, out Exception exception);
        protected abstract bool OnTryCreateFaultMessage(Exception exception, out Message message);

        public bool TryCreateException(Message message, MessageFault fault, out Exception exception)
        {
            if (message == null)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull(nameof(message));
            }
            if (fault == null)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull(nameof(fault));
            }

            bool created = OnTryCreateException(message, fault, out exception);

            if (created)
            {
                if (exception == null)
                {
                    string text = SRP.Format(SRP.FaultConverterDidNotCreateException, GetType().Name);
                    Exception error = new InvalidOperationException(text);
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(error);
                }
            }
            else
            {
                if (exception != null)
                {
                    string text = SRP.Format(SRP.FaultConverterCreatedException, GetType().Name);
                    Exception error = new InvalidOperationException(text, exception);
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(error);
                }
            }

            return created;
        }

        public bool TryCreateFaultMessage(Exception exception, out Message message)
        {
            bool created = OnTryCreateFaultMessage(exception, out message);

            if (created)
            {
                if (message == null)
                {
                    string text = SRP.Format(SRP.FaultConverterDidNotCreateFaultMessage, GetType().Name);
                    Exception error = new InvalidOperationException(text);
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(error);
                }
            }
            else
            {
                if (message != null)
                {
                    string text = SRP.Format(SRP.FaultConverterCreatedFaultMessage, GetType().Name);
                    Exception error = new InvalidOperationException(text);
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(error);
                }
            }

            return created;
        }

        internal class DefaultFaultConverter : FaultConverter
        {
            private MessageVersion _version;

            internal DefaultFaultConverter(MessageVersion version)
            {
                _version = version;
            }

            protected override bool OnTryCreateException(Message message, MessageFault fault, out Exception exception)
            {
                exception = null;

                // SOAP MustUnderstand
                if (string.Compare(fault.Code.Namespace, _version.Envelope.Namespace, StringComparison.Ordinal) == 0
                    && string.Compare(fault.Code.Name, MessageStrings.MustUnderstandFault, StringComparison.Ordinal) == 0)
                {
                    exception = new ProtocolException(fault.Reason.GetMatchingTranslation(CultureInfo.CurrentCulture).Text);
                    return true;
                }

                bool checkSender;
                bool checkReceiver;
                FaultCode code;

                if (_version.Envelope == EnvelopeVersion.Soap11)
                {
                    checkSender = true;
                    checkReceiver = true;
                    code = fault.Code;
                }
                else
                {
                    checkSender = fault.Code.IsSenderFault;
                    checkReceiver = fault.Code.IsReceiverFault;
                    code = fault.Code.SubCode;
                }

                if (code == null)
                {
                    return false;
                }

                if (code.Namespace == null)
                {
                    return false;
                }

                if (checkSender)
                {
                    // WS-Addressing
                    if (string.Compare(code.Namespace, _version.Addressing.Namespace, StringComparison.Ordinal) == 0)
                    {
                        if (string.Compare(code.Name, AddressingStrings.ActionNotSupported, StringComparison.Ordinal) == 0)
                        {
                            exception = new ActionNotSupportedException(fault.Reason.GetMatchingTranslation(CultureInfo.CurrentCulture).Text);
                            return true;
                        }
                        else if (string.Compare(code.Name, AddressingStrings.DestinationUnreachable, StringComparison.Ordinal) == 0)
                        {
                            exception = new EndpointNotFoundException(fault.Reason.GetMatchingTranslation(CultureInfo.CurrentCulture).Text);
                            return true;
                        }
                        else if (string.Compare(code.Name, Addressing10Strings.InvalidAddressingHeader, StringComparison.Ordinal) == 0)
                        {
                            if (code.SubCode != null && string.Compare(code.SubCode.Namespace, _version.Addressing.Namespace, StringComparison.Ordinal) == 0 &&
                                string.Compare(code.SubCode.Name, Addressing10Strings.InvalidCardinality, StringComparison.Ordinal) == 0)
                            {
                                exception = new MessageHeaderException(fault.Reason.GetMatchingTranslation(CultureInfo.CurrentCulture).Text, true);
                                return true;
                            }
                        }
                        else if (_version.Addressing == AddressingVersion.WSAddressing10)
                        {
                            if (string.Compare(code.Name, Addressing10Strings.MessageAddressingHeaderRequired, StringComparison.Ordinal) == 0)
                            {
                                exception = new MessageHeaderException(fault.Reason.GetMatchingTranslation(CultureInfo.CurrentCulture).Text);
                                return true;
                            }
                            else if (string.Compare(code.Name, Addressing10Strings.InvalidAddressingHeader, StringComparison.Ordinal) == 0)
                            {
                                exception = new ProtocolException(fault.Reason.GetMatchingTranslation(CultureInfo.CurrentCulture).Text);
                                return true;
                            }
                        }
                        else
                        {
                            if (string.Compare(code.Name, Addressing200408Strings.MessageInformationHeaderRequired, StringComparison.Ordinal) == 0)
                            {
                                exception = new ProtocolException(fault.Reason.GetMatchingTranslation(CultureInfo.CurrentCulture).Text);
                                return true;
                            }
                            else if (string.Compare(code.Name, Addressing200408Strings.InvalidMessageInformationHeader, StringComparison.Ordinal) == 0)
                            {
                                exception = new ProtocolException(fault.Reason.GetMatchingTranslation(CultureInfo.CurrentCulture).Text);
                                return true;
                            }
                        }
                    }
                }

                if (checkReceiver)
                {
                    // WS-Addressing
                    if (string.Compare(code.Namespace, _version.Addressing.Namespace, StringComparison.Ordinal) == 0)
                    {
                        if (string.Compare(code.Name, AddressingStrings.EndpointUnavailable, StringComparison.Ordinal) == 0)
                        {
                            exception = new ServerTooBusyException(fault.Reason.GetMatchingTranslation(CultureInfo.CurrentCulture).Text);
                            return true;
                        }
                    }
                }

                return false;
            }

            protected override bool OnTryCreateFaultMessage(Exception exception, out Message message)
            {
                // WSA
                if (_version.Addressing == AddressingVersion.WSAddressing10)
                {
                    if (exception is MessageHeaderException)
                    {
                        MessageHeaderException mhe = exception as MessageHeaderException;
                        if (mhe.HeaderNamespace == AddressingVersion.WSAddressing10.Namespace)
                        {
                            message = mhe.ProvideFault(_version);
                            return true;
                        }
                    }
                    else if (exception is ActionMismatchAddressingException)
                    {
                        ActionMismatchAddressingException amae = exception as ActionMismatchAddressingException;
                        message = amae.ProvideFault(_version);
                        return true;
                    }
                }
                if (_version.Addressing != AddressingVersion.None)
                {
                    if (exception is ActionNotSupportedException)
                    {
                        ActionNotSupportedException anse = exception as ActionNotSupportedException;
                        message = anse.ProvideFault(_version);
                        return true;
                    }
                }

                // SOAP
                if (exception is MustUnderstandSoapException)
                {
                    MustUnderstandSoapException muse = exception as MustUnderstandSoapException;
                    message = muse.ProvideFault(_version);
                    return true;
                }

                message = null;
                return false;
            }
        }
    }
}
