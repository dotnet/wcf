// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Runtime.InteropServices;
using System.Diagnostics.CodeAnalysis;

namespace System.Runtime.Diagnostics
{
    [StructLayout(LayoutKind.Explicit, Size = 16)]
    internal struct EventDescriptor
    {
        [FieldOffset(0)]
        private ushort _id;
        [FieldOffset(2)]
        private byte _version;
        [FieldOffset(3)]
        private byte _channel;
        [FieldOffset(4)]
        private byte _level;
        [FieldOffset(5)]
        private byte _opcode;
        [FieldOffset(6)]
        private ushort _task;
        [FieldOffset(8)]
        private long _keywords;

        [SuppressMessage("Microsoft.Naming", "CA1702:CompoundWordsShouldBeCasedCorrectly",
            MessageId = "opcode", Justification = "This is to align with the definition in System.Core.dll")]
        public EventDescriptor(
                int id,
                byte version,
                byte channel,
                byte level,
                byte opcode,
                int task,
                long keywords
                )
        {
            if (id < 0)
            {
                throw Fx.Exception.ArgumentOutOfRange("id", id, InternalSR.ValueMustBeNonNegative);
            }

            if (id > ushort.MaxValue)
            {
                throw Fx.Exception.ArgumentOutOfRange("id", id, string.Empty);
            }

            _id = (ushort)id;
            _version = version;
            _channel = channel;
            _level = level;
            _opcode = opcode;
            _keywords = keywords;

            if (task < 0)
            {
                throw Fx.Exception.ArgumentOutOfRange("task", task, InternalSR.ValueMustBeNonNegative);
            }

            if (task > ushort.MaxValue)
            {
                throw Fx.Exception.ArgumentOutOfRange("task", task, string.Empty);
            }

            _task = (ushort)task;
        }

        public int EventId
        {
            get
            {
                return _id;
            }
        }

        public byte Version
        {
            get
            {
                return _version;
            }
        }

        public byte Channel
        {
            get
            {
                return _channel;
            }
        }
        public byte Level
        {
            get
            {
                return _level;
            }
        }

        [SuppressMessage("Microsoft.Naming", "CA1702:CompoundWordsShouldBeCasedCorrectly",
            MessageId = "Opcode", Justification = "This is to align with the definition in System.Core.dll")]
        public byte Opcode
        {
            get
            {
                return _opcode;
            }
        }

        public int Task
        {
            get
            {
                return _task;
            }
        }

        public long Keywords
        {
            get
            {
                return _keywords;
            }
        }

        public override bool Equals(object obj)
        {
            if (!(obj is EventDescriptor))
                return false;

            return Equals((EventDescriptor)obj);
        }

        public override int GetHashCode()
        {
            return _id ^ _version ^ _channel ^ _level ^ _opcode ^ _task ^ (int)_keywords;
        }

        public bool Equals(EventDescriptor other)
        {
            if ((_id != other._id) ||
                (_version != other._version) ||
                (_channel != other._channel) ||
                (_level != other._level) ||
                (_opcode != other._opcode) ||
                (_task != other._task) ||
                (_keywords != other._keywords))
            {
                return false;
            }
            return true;
        }

        public static bool operator ==(EventDescriptor event1, EventDescriptor event2)
        {
            return event1.Equals(event2);
        }

        public static bool operator !=(EventDescriptor event1, EventDescriptor event2)
        {
            return !event1.Equals(event2);
        }
    }
}
