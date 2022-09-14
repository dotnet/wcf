// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.IO;

namespace System.ServiceModel.Channels
{
    internal struct IntDecoderHelper
    {
        private int _value;
        private short _index;
        private const int LastIndex = 4;

        public int Value
        {
            get
            {
                if (!IsValueDecoded)
                {
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new InvalidOperationException(SRP.FramingValueNotAvailable));
                }

                return _value;
            }
        }

        public bool IsValueDecoded { get; private set; }

        public void Reset()
        {
            _index = 0;
            _value = 0;
            IsValueDecoded = false;
        }

        public int Decode(byte[] buffer, int offset, int size)
        {
            ValidateSize(size);
            if (IsValueDecoded)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new InvalidOperationException(SRP.FramingValueNotAvailable));
            }
            int bytesConsumed = 0;
            while (bytesConsumed < size)
            {
                int next = buffer[offset];
                _value |= (next & 0x7F) << (_index * 7);
                bytesConsumed++;
                if (_index == LastIndex && (next & 0xF8) != 0)
                {
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new InvalidDataException(SRP.FramingSizeTooLarge));
                }
                _index++;
                if ((next & 0x80) == 0)
                {
                    IsValueDecoded = true;
                    break;
                }
                offset++;
            }
            return bytesConsumed;
        }

        public static void ValidateSize(int size)
        {
            if (size <= 0)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentOutOfRangeException(nameof(size), size, SRP.ValueMustBePositive));
            }
        }
    }
}
