// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Diagnostics.Tracing;

namespace Infrastructure.Common
{
    /// <summary>Simple event listener</summary>
    /// This class is based on https://github.com/dotnet/corefx/blob/master/src/Common/tests/System/Diagnostics/Tracing/TestEventListener.cs
    public sealed class TestEventListener : EventListener
    {
        private Dictionary<string, bool> _targetSourceName = new Dictionary<string, bool>();
        private readonly EventLevel _level;

        private Action<EventWrittenEventArgs> _eventWritten;
        private List<EventSource> _tmpEventSourceList = new List<EventSource>();

        public TestEventListener(List<string> targetSourceNames, EventLevel level)
        {
            // Store the arguments
            foreach (var targetSourceName in targetSourceNames)
            {
                _targetSourceName.Add(targetSourceName, true);
            }
            _level = level;

            LoadSourceList();
        }

        public new Action<EventWrittenEventArgs> EventWritten
        {
            get { return _eventWritten; }
            set { _eventWritten = value; }
        }

        private void LoadSourceList()
        {
            // The base constructor, which is called before this constructor,
            // will invoke the virtual OnEventSourceCreated method for each
            // existing EventSource, which means OnEventSourceCreated will be
            // called before _targetSourceName have been set.  As such,
            // we store a temporary list that just exists from the moment this instance
            // is created (instance field initializers run before the base constructor)
            // and until we finish construction... in that window, OnEventSourceCreated
            // will store the sources into the list rather than try to enable them directly,
            // and then here we can enumerate that list, then clear it out.
            List<EventSource> sources;
            lock (_tmpEventSourceList)
            {
                sources = _tmpEventSourceList;
                _tmpEventSourceList = null;
            }
            foreach (EventSource source in sources)
            {
                EnableSourceIfMatch(source);
            }
        }

        protected override void OnEventSourceCreated(EventSource eventSource)
        {
            List<EventSource> tmp = _tmpEventSourceList;
            if (tmp != null)
            {
                lock (tmp)
                {
                    if (_tmpEventSourceList != null)
                    {
                        _tmpEventSourceList.Add(eventSource);
                        return;
                    }
                }
            }

            EnableSourceIfMatch(eventSource);
        }

        private void EnableSourceIfMatch(EventSource source)
        {
            if (_targetSourceName.ContainsKey(source.Name))
            {
                EnableEvents(source, _level);
            }
        }
        protected override void OnEventWritten(EventWrittenEventArgs eventData)
        {
            _eventWritten?.Invoke(eventData);
        }
    }
}
