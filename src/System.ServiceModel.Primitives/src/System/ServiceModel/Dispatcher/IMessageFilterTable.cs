﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.ServiceModel.Channels;
using System.Collections.Generic;

namespace System.ServiceModel.Dispatcher
{
    public interface IMessageFilterTable<TFilterData> : IDictionary<MessageFilter, TFilterData>
    {
        // return a single match
        bool GetMatchingValue(Message message, out TFilterData value);
        bool GetMatchingValue(MessageBuffer messageBuffer, out TFilterData value);

        // return multiple matches
        bool GetMatchingValues(Message message, ICollection<TFilterData> results);
        bool GetMatchingValues(MessageBuffer messageBuffer, ICollection<TFilterData> results);

        // If you need both the filter and the data, use these functions then look up
        // the data using the filter tables IDictionary methods.
        bool GetMatchingFilter(Message message, out MessageFilter filter);
        bool GetMatchingFilter(MessageBuffer messageBuffer, out MessageFilter filter);
        bool GetMatchingFilters(Message message, ICollection<MessageFilter> results);
        bool GetMatchingFilters(MessageBuffer messageBuffer, ICollection<MessageFilter> results);
    }
}
