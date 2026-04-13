// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using System.ServiceModel.Security;
using System.Transactions;

namespace System.ServiceModel.Channels
{
    public sealed class TransactionMessageProperty
    {
        private ServiceModel.Transactions.TransactionInfo _flowedTransactionInfo;
        private Transaction _flowedTransaction;
        private const string PropertyName = "TransactionMessageProperty";

        private TransactionMessageProperty()
        {
        }

        public Transaction Transaction
        {
            get
            {
                if (_flowedTransaction == null && _flowedTransactionInfo != null)
                {
                    try
                    {
                        _flowedTransaction = _flowedTransactionInfo.UnmarshalTransaction();
                    }
                    catch (TransactionException e)
                    {
                        throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(e);
                    }
                }
                return _flowedTransaction;
            }
        }

        internal static TransactionMessageProperty TryGet(Message message)
        {
            if (message.Properties.ContainsKey(PropertyName))
                return message.Properties[PropertyName] as TransactionMessageProperty;
            else
                return null;
        }

        internal static Transaction TryGetTransaction(Message message)
        {
            if (!message.Properties.ContainsKey(PropertyName))
                return null;

            return ((TransactionMessageProperty)message.Properties[PropertyName]).Transaction;
        }

        private static TransactionMessageProperty GetPropertyAndThrowIfAlreadySet(Message message)
        {
            if (message.Properties.ContainsKey(PropertyName))
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(
                    new FaultException(SRP.SFxTryAddMultipleTransactionsOnMessage));
            }

            return new TransactionMessageProperty();
        }

        public static void Set(Transaction transaction, Message message)
        {
            TransactionMessageProperty property = GetPropertyAndThrowIfAlreadySet(message);
            property._flowedTransaction = transaction;
            message.Properties.Add(PropertyName, property);
        }

        internal static void Set(ServiceModel.Transactions.TransactionInfo transactionInfo, Message message)
        {
            TransactionMessageProperty property = GetPropertyAndThrowIfAlreadySet(message);
            property._flowedTransactionInfo = transactionInfo;
            message.Properties.Add(PropertyName, property);
        }
    }

    internal class TransactionFlowProperty
    {
        private Transaction _flowedTransaction;
        private List<RequestSecurityTokenResponse> _issuedTokens;
        private const string PropertyName = "TransactionFlowProperty";

        private TransactionFlowProperty()
        {
        }

        internal ICollection<RequestSecurityTokenResponse> IssuedTokens
        {
            get
            {
                if (_issuedTokens == null)
                {
                    _issuedTokens = new List<RequestSecurityTokenResponse>();
                }

                return _issuedTokens;
            }
        }

        internal Transaction Transaction
        {
            get { return _flowedTransaction; }
        }

        internal static TransactionFlowProperty Ensure(Message message)
        {
            if (message.Properties.ContainsKey(PropertyName))
                return (TransactionFlowProperty)message.Properties[PropertyName];

            TransactionFlowProperty property = new TransactionFlowProperty();
            message.Properties.Add(PropertyName, property);
            return property;
        }

        internal static TransactionFlowProperty TryGet(Message message)
        {
            if (message.Properties.ContainsKey(PropertyName))
                return message.Properties[PropertyName] as TransactionFlowProperty;
            else
                return null;
        }

        internal static ICollection<RequestSecurityTokenResponse> TryGetIssuedTokens(Message message)
        {
            TransactionFlowProperty property = TransactionFlowProperty.TryGet(message);
            if (property == null)
                return null;

            if (property._issuedTokens == null || property._issuedTokens.Count == 0)
                return null;

            return property._issuedTokens;
        }

        internal static Transaction TryGetTransaction(Message message)
        {
            if (!message.Properties.ContainsKey(PropertyName))
                return null;

            return ((TransactionFlowProperty)message.Properties[PropertyName]).Transaction;
        }

        private static TransactionFlowProperty GetPropertyAndThrowIfAlreadySet(Message message)
        {
            TransactionFlowProperty property = TryGet(message);

            if (property != null)
            {
                if (property._flowedTransaction != null)
                {
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(
                        new FaultException(SRP.SFxTryAddMultipleTransactionsOnMessage));
                }
            }
            else
            {
                property = new TransactionFlowProperty();
                message.Properties.Add(PropertyName, property);
            }

            return property;
        }

        internal static void Set(Transaction transaction, Message message)
        {
            TransactionFlowProperty property = GetPropertyAndThrowIfAlreadySet(message);
            property._flowedTransaction = transaction;
        }
    }
}
