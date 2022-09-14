// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.


namespace System.ServiceModel.Channels
{
    internal class ChannelDemuxer
    {
        public readonly static TimeSpan UseDefaultReceiveTimeout = TimeSpan.MinValue;

        public ChannelDemuxer()
        {
            PeekTimeout = ChannelDemuxer.UseDefaultReceiveTimeout; //use the default receive timeout (original behavior)
            MaxPendingSessions = 10;
        }

        public TimeSpan PeekTimeout { get; set; }

        public int MaxPendingSessions { get; set; }
    }

    //
    // Binding element
    //

    internal class ChannelDemuxerBindingElement : BindingElement
    {
        private ChannelDemuxer _demuxer;
        private CachedBindingContextState _cachedContextState;
        private bool _cacheContextState;

        public ChannelDemuxerBindingElement(bool cacheContextState)
        {
            _cacheContextState = cacheContextState;
            if (cacheContextState)
            {
                _cachedContextState = new CachedBindingContextState();
            }
            _demuxer = new ChannelDemuxer();
        }

        public ChannelDemuxerBindingElement(ChannelDemuxerBindingElement element)
        {
            _demuxer = element._demuxer;
            _cacheContextState = element._cacheContextState;
            _cachedContextState = element._cachedContextState;
        }

        public TimeSpan PeekTimeout
        {
            get
            {
                return _demuxer.PeekTimeout;
            }
            set
            {
                if (value < TimeSpan.Zero && value != ChannelDemuxer.UseDefaultReceiveTimeout)
                {
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentOutOfRangeException(nameof(value)));
                }

                _demuxer.PeekTimeout = value;
            }
        }

        public int MaxPendingSessions
        {
            get
            {
                return _demuxer.MaxPendingSessions;
            }
            set
            {
                if (value < 1)
                {
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentOutOfRangeException(nameof(value), SRP.ValueMustBeGreaterThanZero));
                }

                _demuxer.MaxPendingSessions = value;
            }
        }

        private void SubstituteCachedBindingContextParametersIfNeeded(BindingContext context)
        {
            if (!_cacheContextState)
            {
                return;
            }
            if (!_cachedContextState.IsStateCached)
            {
                foreach (object parameter in context.BindingParameters)
                {
                    _cachedContextState.CachedBindingParameters.Add(parameter);
                }
                _cachedContextState.IsStateCached = true;
            }
            else
            {
                context.BindingParameters.Clear();
                foreach (object parameter in _cachedContextState.CachedBindingParameters)
                {
                    context.BindingParameters.Add(parameter);
                }
            }
        }

        public override IChannelFactory<TChannel> BuildChannelFactory<TChannel>(BindingContext context)
        {
            if (context == null)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull(nameof(context));
            }

            SubstituteCachedBindingContextParametersIfNeeded(context);
            return context.BuildInnerChannelFactory<TChannel>();
        }

        public override bool CanBuildChannelFactory<TChannel>(BindingContext context)
        {
            if (context == null)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull(nameof(context));
            }

            return context.CanBuildInnerChannelFactory<TChannel>();
        }

        public override BindingElement Clone()
        {
            return new ChannelDemuxerBindingElement(this);
        }

        public override T GetProperty<T>(BindingContext context)
        {
            // augment the context with cached binding parameters
            if (_cacheContextState && _cachedContextState.IsStateCached)
            {
                for (int i = 0; i < _cachedContextState.CachedBindingParameters.Count; ++i)
                {
                    if (!context.BindingParameters.Contains(_cachedContextState.CachedBindingParameters[i].GetType()))
                    {
                        context.BindingParameters.Add(_cachedContextState.CachedBindingParameters[i]);
                    }
                }
            }
            return context.GetInnerProperty<T>();
        }

        private class CachedBindingContextState
        {
            public bool IsStateCached;
            public BindingParameterCollection CachedBindingParameters;

            public CachedBindingContextState()
            {
                CachedBindingParameters = new BindingParameterCollection();
            }
        }
    }
}
