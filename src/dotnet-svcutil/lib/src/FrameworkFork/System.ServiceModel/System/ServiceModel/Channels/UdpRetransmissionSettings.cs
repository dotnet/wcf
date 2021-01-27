// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.ServiceModel.Channels
{
    using System;
    using System.ComponentModel;
    using System.Globalization;
    using System.Runtime;

    public sealed class UdpRetransmissionSettings
    {
        private int _maxUnicastRetransmitCount;
        private int _maxMulticastRetransmitCount;
        private TimeSpan _delayLowerBound;
        private TimeSpan _delayUpperBound;
        private TimeSpan _maxDelayPerRetransmission;
        private int _delayLowerBoundMilliseconds;
        private int _delayUpperBoundMilliseconds;
        private int _maxDelayMilliseconds;

        //this constructor disables retransmission
        public UdpRetransmissionSettings()
            : this(0, 0)
        {
        }

        public UdpRetransmissionSettings(int maxUnicastRetransmitCount, int maxMulticastRetransmitCount)
            : this(maxUnicastRetransmitCount, maxMulticastRetransmitCount,
            UdpConstants.Defaults.DelayLowerBoundTimeSpan, UdpConstants.Defaults.DelayUpperBoundTimeSpan, UdpConstants.Defaults.MaxDelayPerRetransmissionTimeSpan)
        {
        }

        public UdpRetransmissionSettings(int maxUnicastRetransmitCount, int maxMulticastRetransmitCount, TimeSpan delayLowerBound, TimeSpan delayUpperBound, TimeSpan maxDelayPerRetransmission)
        {
            if (maxUnicastRetransmitCount < 0)
            {
                throw FxTrace.Exception.ArgumentOutOfRange("maxUnicastRetransmitCount", maxUnicastRetransmitCount,
                    string.Format(SRServiceModel.ArgumentOutOfMinRange, 0));
            }

            if (maxMulticastRetransmitCount < 0)
            {
                throw FxTrace.Exception.ArgumentOutOfRange("maxMulticastRetransmitCount", maxMulticastRetransmitCount,
                    string.Format(SRServiceModel.ArgumentOutOfMinRange, 0));
            }


            if (delayLowerBound < TimeSpan.Zero)
            {
                throw FxTrace.Exception.ArgumentOutOfRange("delayLowerBound", delayLowerBound, string.Format(SRServiceModel.ArgumentOutOfMinRange, TimeSpan.Zero));
            }

            if (delayUpperBound < TimeSpan.Zero)
            {
                throw FxTrace.Exception.ArgumentOutOfRange("delayUpperBound", delayUpperBound, string.Format(SRServiceModel.ArgumentOutOfMinRange, TimeSpan.Zero));
            }

            if (maxDelayPerRetransmission < TimeSpan.Zero)
            {
                throw FxTrace.Exception.ArgumentOutOfRange("maxDelayPerRetransmission", maxDelayPerRetransmission,
                     string.Format(SRServiceModel.ArgumentOutOfMinRange, TimeSpan.Zero));
            }

            _maxUnicastRetransmitCount = maxUnicastRetransmitCount;
            _maxMulticastRetransmitCount = maxMulticastRetransmitCount;
            _delayLowerBound = delayLowerBound;
            _delayUpperBound = delayUpperBound;
            _maxDelayPerRetransmission = maxDelayPerRetransmission;

            _delayLowerBoundMilliseconds = TimeoutHelper.ToMilliseconds(_delayLowerBound);
            _delayUpperBoundMilliseconds = TimeoutHelper.ToMilliseconds(_delayUpperBound);
            _maxDelayMilliseconds = TimeoutHelper.ToMilliseconds(_maxDelayPerRetransmission);

            ValidateSettings();
        }

        private UdpRetransmissionSettings(UdpRetransmissionSettings other)
            : this(other._maxUnicastRetransmitCount, other._maxMulticastRetransmitCount, other._delayLowerBound, other._delayUpperBound, other._maxDelayPerRetransmission)
        {
        }

        [DefaultValue(UdpConstants.Defaults.MaxUnicastRetransmitCount)]
        public int MaxUnicastRetransmitCount
        {
            get
            {
                return _maxUnicastRetransmitCount;
            }
            set
            {
                const int min = 0;
                if (value < min)
                {
                    throw FxTrace.Exception.ArgumentOutOfRange("value", value, string.Format(SRServiceModel.ArgumentOutOfMinRange, min));
                }
                _maxUnicastRetransmitCount = value;
            }
        }

        [DefaultValue(UdpConstants.Defaults.MaxMulticastRetransmitCount)]
        public int MaxMulticastRetransmitCount
        {
            get
            {
                return _maxMulticastRetransmitCount;
            }
            set
            {
                const int min = 0;
                if (value < min)
                {
                    throw FxTrace.Exception.ArgumentOutOfRange("value", value, string.Format(SRServiceModel.ArgumentOutOfMinRange, min));
                }
                _maxMulticastRetransmitCount = value;
            }
        }

        public TimeSpan DelayLowerBound
        {
            get
            {
                return _delayLowerBound;
            }
            set
            {
                if (value < TimeSpan.Zero)
                {
                    throw FxTrace.Exception.ArgumentOutOfRange("value", value, string.Format(SRServiceModel.ArgumentOutOfMinRange, TimeSpan.Zero));
                }

                _delayLowerBound = value;
                _delayLowerBoundMilliseconds = TimeoutHelper.ToMilliseconds(_delayLowerBound);
            }
        }

        public TimeSpan DelayUpperBound
        {
            get
            {
                return _delayUpperBound;
            }
            set
            {
                if (value < TimeSpan.Zero)
                {
                    throw FxTrace.Exception.ArgumentOutOfRange("value", value, string.Format(SRServiceModel.ArgumentOutOfMinRange, TimeSpan.Zero));
                }

                _delayUpperBound = value;
                _delayUpperBoundMilliseconds = TimeoutHelper.ToMilliseconds(_delayUpperBound);
            }
        }

        public TimeSpan MaxDelayPerRetransmission
        {
            get
            {
                return _maxDelayPerRetransmission;
            }
            set
            {
                if (value < TimeSpan.Zero)
                {
                    throw FxTrace.Exception.ArgumentOutOfRange("value", value, string.Format(SRServiceModel.ArgumentOutOfMinRange, TimeSpan.Zero));
                }

                _maxDelayPerRetransmission = value;
                _maxDelayMilliseconds = TimeoutHelper.ToMilliseconds(_maxDelayPerRetransmission);
            }
        }

        public bool ShouldSerializeDelayLowerBound()
        {
            return !TimeSpansEqual(_delayLowerBound, UdpConstants.Defaults.DelayLowerBoundTimeSpan);
        }

        public bool ShouldSerializeDelayUpperBound()
        {
            return !TimeSpansEqual(_delayUpperBound, UdpConstants.Defaults.DelayUpperBoundTimeSpan);
        }

        public bool ShouldSerializeMaxDelayPerRetransmission()
        {
            return !TimeSpansEqual(_maxDelayPerRetransmission, UdpConstants.Defaults.MaxDelayPerRetransmissionTimeSpan);
        }


        //called at send time to avoid repeated rounding and casting
        internal int GetDelayLowerBound()
        {
            return _delayLowerBoundMilliseconds;
        }

        //called at send time to avoid repeated rounding and casting
        internal int GetDelayUpperBound()
        {
            return _delayUpperBoundMilliseconds;
        }

        //called at send time to avoid repeated rounding and casting
        internal int GetMaxDelayPerRetransmission()
        {
            return _maxDelayMilliseconds;
        }

        internal bool Enabled
        {
            get
            {
                return _maxUnicastRetransmitCount > 0 || _maxMulticastRetransmitCount > 0;
            }
        }

        internal void ValidateSettings()
        {
            if (_delayLowerBound > _delayUpperBound)
            {
                throw FxTrace.Exception.ArgumentOutOfRange("DelayLowerBound", _delayLowerBound, "TODO: "); // SR.Property1LessThanOrEqualToProperty2("DelayLowerBound", this.delayLowerBound, "DelayUpperBound", this.delayUpperBound));
            }


            if (_delayUpperBound > _maxDelayPerRetransmission)
            {
                throw FxTrace.Exception.ArgumentOutOfRange("DelayUpperBound", _delayUpperBound, "TODO: "); //  SR.Property1LessThanOrEqualToProperty2("DelayUpperBound", this.delayUpperBound, "MaxDelayPerRetransmission", this.maxDelayPerRetransmission));
            }
        }

        internal UdpRetransmissionSettings Clone()
        {
            return new UdpRetransmissionSettings(this);
        }

        internal bool IsMatch(UdpRetransmissionSettings udpRetransmissionSettings)
        {
            if (this.DelayLowerBound != udpRetransmissionSettings.DelayLowerBound)
            {
                return false;
            }

            if (this.DelayUpperBound != udpRetransmissionSettings.DelayUpperBound)
            {
                return false;
            }

            if (this.MaxDelayPerRetransmission != udpRetransmissionSettings.MaxDelayPerRetransmission)
            {
                return false;
            }

            if (this.MaxMulticastRetransmitCount != udpRetransmissionSettings.MaxMulticastRetransmitCount)
            {
                return false;
            }

            if (this.MaxUnicastRetransmitCount != udpRetransmissionSettings.MaxUnicastRetransmitCount)
            {
                return false;
            }

            return true;
        }

        private bool TimeSpansEqual(TimeSpan ts1, TimeSpan ts2)
        {
            long diff = Math.Abs(ts1.Ticks - ts2.Ticks);
            long max = Math.Max(Math.Abs(ts1.Ticks), Math.Abs(ts2.Ticks));
            return diff < TimeSpan.FromMilliseconds(1).Ticks || (max > 0 && diff / (double)max < 1e-3);
        }
    }
}
