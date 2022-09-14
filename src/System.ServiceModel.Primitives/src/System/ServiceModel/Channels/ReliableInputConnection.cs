// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Runtime;
using System.Threading.Tasks;

namespace System.ServiceModel.Channels
{
    internal sealed class ReliableInputConnection
    {
        private bool _isLastKnown = false;
        private ReliableMessagingVersion _reliableMessagingVersion;
        private InterruptibleWaitObject _shutdownWaitObject = new InterruptibleWaitObject(false);
        private bool _terminated = false;
        private InterruptibleWaitObject _terminateWaitObject = new InterruptibleWaitObject(false, false);

        public ReliableInputConnection()
        {
        }

        public bool AllAdded
        {
            get
            {
                return (Ranges.Count == 1
                    && Ranges[0].Lower == 1
                    && Ranges[0].Upper == Last)
                    || _isLastKnown;
            }
        }

        public bool IsLastKnown
        {
            get
            {
                return Last != 0 || _isLastKnown;
            }
        }

        public bool IsSequenceClosed { get; private set; } = false;

        public long Last { get; private set; } = 0;

        public SequenceRangeCollection Ranges { get; private set; } = SequenceRangeCollection.Empty;

        public ReliableMessagingVersion ReliableMessagingVersion
        {
            set
            {
                _reliableMessagingVersion = value;
            }
        }

        public void Abort(ChannelBase channel)
        {
            _shutdownWaitObject.Abort(channel);
            _terminateWaitObject.Abort(channel);
        }

        public bool CanMerge(long sequenceNumber)
        {
            return CanMerge(sequenceNumber, Ranges);
        }

        // Returns true if merging the number will not increase the number of ranges past MaxSequenceRanges.
        public static bool CanMerge(long sequenceNumber, SequenceRangeCollection ranges)
        {
            if (ranges.Count < ReliableMessagingConstants.MaxSequenceRanges)
            {
                return true;
            }

            ranges = ranges.MergeWith(sequenceNumber);
            return ranges.Count <= ReliableMessagingConstants.MaxSequenceRanges;
        }

        public void Fault(ChannelBase channel)
        {
            _shutdownWaitObject.Fault(channel);
            _terminateWaitObject.Fault(channel);
        }

        public bool IsValid(long sequenceNumber, bool isLast)
        {
            if (_reliableMessagingVersion == ReliableMessagingVersion.WSReliableMessagingFebruary2005)
            {
                if (isLast)
                {
                    if (Last == 0)
                    {
                        if (Ranges.Count > 0)
                        {
                            return sequenceNumber > Ranges[Ranges.Count - 1].Upper;
                        }
                        else
                        {
                            return true;
                        }
                    }
                    else
                    {
                        return sequenceNumber == Last;
                    }
                }
                else if (Last > 0)
                {
                    return sequenceNumber < Last;
                }
            }
            else
            {
                if (_isLastKnown)
                {
                    return Ranges.Contains(sequenceNumber);
                }
            }

            return true;
        }

        public void Merge(long sequenceNumber, bool isLast)
        {
            Ranges = Ranges.MergeWith(sequenceNumber);

            if (isLast)
                Last = sequenceNumber;

            if (AllAdded)
                _shutdownWaitObject.Set();
        }

        public bool SetCloseSequenceLast(long last)
        {
            WsrmUtilities.AssertWsrm11(_reliableMessagingVersion);
            bool validLast;

            if ((last < 1) || (Ranges.Count == 0))
            {
                validLast = true;
            }
            else
            {
                validLast = last >= Ranges[Ranges.Count - 1].Upper;
            }

            if (validLast)
            {
                IsSequenceClosed = true;
                SetLast(last);
            }

            return validLast;
        }

        private void SetLast(long last)
        {
            if (_isLastKnown)
            {
                throw Fx.AssertAndThrow("Last can only be set once.");
            }

            Last = last;
            _isLastKnown = true;
            _shutdownWaitObject.Set();
        }

        // Two error cases:
        // (1) The sequence contains holes.
        // (2) TerminateSequence.LastMsgNumber < last received message number.
        // In both cases the channel should be faulted. In case (2) the channel should send a fault.
        public bool SetTerminateSequenceLast(long last, out bool isLastLargeEnough)
        {
            WsrmUtilities.AssertWsrm11(_reliableMessagingVersion);
            isLastLargeEnough = true;

            // unspecified last
            if (last < 1)
            {
                return false;
            }

            int rangeCount = Ranges.Count;
            long lastReceived = (rangeCount > 0) ? Ranges[rangeCount - 1].Upper : 0;

            // last is too small to be valid
            if (last < lastReceived)
            {
                isLastLargeEnough = false;
                return false;
            }

            // there is a hole in the sequence
            if ((rangeCount > 1) || (last > lastReceived))
            {
                return false;
            }

            SetLast(last);
            return true;
        }

        public bool Terminate()
        {
            if ((_reliableMessagingVersion == ReliableMessagingVersion.WSReliableMessagingFebruary2005)
                || IsSequenceClosed)
            {
                if (!_terminated && AllAdded)
                {
                    _terminateWaitObject.Set();
                    _terminated = true;
                }

                return _terminated;
            }

            return _isLastKnown;
        }

        public async Task CloseAsync(TimeSpan timeout)
        {
            TimeoutHelper timeoutHelper = new TimeoutHelper(timeout);
            await _shutdownWaitObject.WaitAsync(timeoutHelper.RemainingTime());
            await _terminateWaitObject.WaitAsync(timeoutHelper.RemainingTime());
        }
    }
}
