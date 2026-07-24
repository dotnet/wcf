// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.


using System.Collections.Generic;
using System.Globalization;
using System.Runtime.InteropServices;
using System.Runtime.Versioning;

namespace System.ServiceModel.Channels
{
    // Builder for an outgoing MSMQ message represented as the native
    // MQMSGPROPS structure that MQSendMessage consumes.
    //
    // Each call to a public setter appends one slot to three parallel
    // arrays (ids / property variants / status), all of which are
    // allocated in unmanaged memory via Marshal.AllocHGlobal. Pinned
    // buffers (body bytes, label string, etc.) are tracked separately
    // and freed on Dispose so the native call sees stable addresses for
    // the lifetime of the Send.
    //
    // Layout notes:
    //   * MQPROPVARIANT is treated as a 24-byte slot on every arch.
    //     The header is fixed (8 bytes: vt + 3 reserved words) and the
    //     union spans the remaining 16 bytes, which is the upper bound
    //     used by MSMQ's property surface (DECIMAL / CAUB / BLOB).
    //   * Vector members write { cElems @ 8, pElems @ VectorElementsOffset }.
    //     VectorElementsOffset is 12 on x86, 16 on x64, accounting for
    //     pointer-width alignment of the second field in CAUB.
    //   * Wide strings are stored UNICODE (CharSet=Unicode on the
    //     P/Invoke side), allocated via Marshal.StringToHGlobalUni,
    //     and accompanied by a separate LEN propid (in characters).
    [SupportedOSPlatform("windows")]
    internal sealed class NativeMsmqMessage : IDisposable
    {
        private const int PropVariantSize = 24;
        private static int VectorElementsOffset => IntPtr.Size == 8 ? 16 : 12;

        // Per-slot state — populated as the caller adds properties.
        private readonly List<uint> _ids = new List<uint>(16);
        // Each entry has its 24 bytes pre-written into a temp byte[]
        // and then BLOB-copied into the unmanaged propVars buffer at
        // Freeze time. This keeps the layout code in one place.
        private readonly List<byte[]> _slotBytes = new List<byte[]>(16);
        private readonly List<GCHandle> _pinnedHandles = new List<GCHandle>();
        private readonly List<IntPtr> _hglobals = new List<IntPtr>();

        // Allocated only on Freeze.
        private IntPtr _propIdsBuffer;
        private IntPtr _propVarsBuffer;
        private IntPtr _statusBuffer;
        private IntPtr _propsStructBuffer;
        private bool _frozen;
        private bool _disposed;

        // ----- Typed setters -----

        public void SetByte(uint propId, byte value)
        {
            var slot = new byte[PropVariantSize];
            WriteUShort(slot, 0, UnsafeNativeMethods.VT_UI1);
            slot[8] = value;
            Append(propId, slot);
        }

        public void SetUInt32(uint propId, uint value)
        {
            var slot = new byte[PropVariantSize];
            WriteUShort(slot, 0, UnsafeNativeMethods.VT_UI4);
            WriteUInt32(slot, 8, value);
            Append(propId, slot);
        }

        // Adds a length-prefixed unicode string. lengthPropId, if
        // non-zero, also appends a VT_UI4 with the string's *character*
        // count (not byte count). The string is stored in unmanaged
        // memory and freed in Dispose().
        public void SetWideString(uint propId, string value, uint lengthPropId)
        {
            if (value == null)
            {
                return;
            }

            IntPtr stringPtr = Marshal.StringToHGlobalUni(value);
            _hglobals.Add(stringPtr);

            var slot = new byte[PropVariantSize];
            WriteUShort(slot, 0, UnsafeNativeMethods.VT_LPWSTR);
            WriteIntPtr(slot, 8, stringPtr);
            Append(propId, slot);

            if (lengthPropId != 0)
            {
                // Character count INCLUDING terminating null, per MSMQ docs.
                SetUInt32(lengthPropId, (uint)(value.Length + 1));
            }
        }

        // Adds a byte-vector. lengthPropId, if non-zero, also appends a
        // VT_UI4 with the byte count.
        public void SetByteVector(uint propId, byte[] value, uint lengthPropId)
        {
            if (value == null)
            {
                return;
            }

            // Pin so MSMQ sees a stable address. Released in Dispose.
            GCHandle pin = GCHandle.Alloc(value, GCHandleType.Pinned);
            _pinnedHandles.Add(pin);

            var slot = new byte[PropVariantSize];
            WriteUShort(slot, 0, (ushort)(UnsafeNativeMethods.VT_VECTOR | UnsafeNativeMethods.VT_UI1));
            WriteUInt32(slot, 8, (uint)value.Length);
            WriteIntPtr(slot, VectorElementsOffset, pin.AddrOfPinnedObject());
            Append(propId, slot);

            if (lengthPropId != 0)
            {
                SetUInt32(lengthPropId, (uint)value.Length);
            }
        }

        // Body is special: PROPID_M_BODY holds the byte vector and
        // PROPID_M_BODY_SIZE holds the *actual* byte count (which can
        // be smaller than the buffer when MSMQ writes a received body
        // back into a pre-sized buffer; on the send path these are
        // equal).
        public void SetBody(byte[] body, int offset, int count)
        {
            if (body == null)
            {
                throw new ArgumentNullException(nameof(body));
            }
            byte[] slice;
            if (offset == 0 && count == body.Length)
            {
                slice = body;
            }
            else
            {
                slice = new byte[count];
                Buffer.BlockCopy(body, offset, slice, 0, count);
            }

            GCHandle pin = GCHandle.Alloc(slice, GCHandleType.Pinned);
            _pinnedHandles.Add(pin);

            var bodySlot = new byte[PropVariantSize];
            WriteUShort(bodySlot, 0, (ushort)(UnsafeNativeMethods.VT_VECTOR | UnsafeNativeMethods.VT_UI1));
            WriteUInt32(bodySlot, 8, (uint)slice.Length);
            WriteIntPtr(bodySlot, VectorElementsOffset, pin.AddrOfPinnedObject());
            Append(UnsafeNativeMethods.PROPID_M_BODY, bodySlot);

            SetUInt32(UnsafeNativeMethods.PROPID_M_BODY_SIZE, (uint)slice.Length);
        }

        public void SetTimeToBeReceived(TimeSpan value)
        {
            uint seconds = ToMsmqSeconds(value);
            SetUInt32(UnsafeNativeMethods.PROPID_M_TIME_TO_BE_RECEIVED, seconds);
        }

        public void SetTimeToReachQueue(TimeSpan value)
        {
            uint seconds = ToMsmqSeconds(value);
            SetUInt32(UnsafeNativeMethods.PROPID_M_TIME_TO_REACH_QUEUE, seconds);
        }

        // MSMQ timer fields are uint seconds. TimeSpan.MaxValue maps to
        // the "infinite" sentinel; anything else is clamped to the
        // [0, uint.MaxValue-1] range so it round-trips cleanly.
        private static uint ToMsmqSeconds(TimeSpan value)
        {
            if (value == TimeSpan.MaxValue)
            {
                return uint.MaxValue;
            }
            double s = value.TotalSeconds;
            if (s <= 0)
            {
                return 0;
            }
            if (s >= uint.MaxValue)
            {
                return uint.MaxValue - 1;
            }
            return (uint)s;
        }

        // ----- Finalization -----

        // Lays out the unmanaged MQMSGPROPS the native API expects.
        // Returns a pointer that remains valid until Dispose.
        public IntPtr Freeze()
        {
            if (_frozen)
            {
                return _propsStructBuffer;
            }
            int n = _ids.Count;
            _propIdsBuffer = Marshal.AllocHGlobal(sizeof(uint) * n);
            _propVarsBuffer = Marshal.AllocHGlobal(PropVariantSize * n);
            _statusBuffer = Marshal.AllocHGlobal(sizeof(int) * n);

            for (int i = 0; i < n; i++)
            {
                Marshal.WriteInt32(_propIdsBuffer, i * sizeof(uint), unchecked((int)_ids[i]));
                Marshal.Copy(_slotBytes[i], 0, _propVarsBuffer + i * PropVariantSize, PropVariantSize);
                Marshal.WriteInt32(_statusBuffer, i * sizeof(int), 0);
            }

            // Layout of MQMSGPROPS:
            //   DWORD cProp;             (4 bytes)
            //   <4 bytes padding on x64>
            //   MSGPROPID* aPropID;      (pointer)
            //   MQPROPVARIANT* aPropVar; (pointer)
            //   HRESULT* aStatus;        (pointer)
            // Total: 8 + IntPtr.Size * 3 on x64, 4 + IntPtr.Size * 3 on x86.
            int headerSize = IntPtr.Size == 8 ? 8 : 4;
            int structSize = headerSize + IntPtr.Size * 3;
            _propsStructBuffer = Marshal.AllocHGlobal(structSize);
            Marshal.WriteInt32(_propsStructBuffer, 0, n);
            // Zero the padding word on x64 so the upper 32 bits of cProp
            // can't be read as garbage on a future ABI revision.
            if (IntPtr.Size == 8)
            {
                Marshal.WriteInt32(_propsStructBuffer, 4, 0);
            }
            Marshal.WriteIntPtr(_propsStructBuffer, headerSize + IntPtr.Size * 0, _propIdsBuffer);
            Marshal.WriteIntPtr(_propsStructBuffer, headerSize + IntPtr.Size * 1, _propVarsBuffer);
            Marshal.WriteIntPtr(_propsStructBuffer, headerSize + IntPtr.Size * 2, _statusBuffer);

            _frozen = true;
            return _propsStructBuffer;
        }

        public void Dispose()
        {
            if (_disposed)
            {
                return;
            }
            _disposed = true;

            if (_propsStructBuffer != IntPtr.Zero)
            {
                Marshal.FreeHGlobal(_propsStructBuffer);
                _propsStructBuffer = IntPtr.Zero;
            }
            if (_statusBuffer != IntPtr.Zero)
            {
                Marshal.FreeHGlobal(_statusBuffer);
                _statusBuffer = IntPtr.Zero;
            }
            if (_propVarsBuffer != IntPtr.Zero)
            {
                Marshal.FreeHGlobal(_propVarsBuffer);
                _propVarsBuffer = IntPtr.Zero;
            }
            if (_propIdsBuffer != IntPtr.Zero)
            {
                Marshal.FreeHGlobal(_propIdsBuffer);
                _propIdsBuffer = IntPtr.Zero;
            }
            foreach (IntPtr p in _hglobals)
            {
                if (p != IntPtr.Zero)
                {
                    Marshal.FreeHGlobal(p);
                }
            }
            _hglobals.Clear();
            foreach (GCHandle h in _pinnedHandles)
            {
                if (h.IsAllocated)
                {
                    h.Free();
                }
            }
            _pinnedHandles.Clear();
        }

        // ----- Internals -----

        private void Append(uint id, byte[] slot)
        {
            if (_frozen)
            {
                throw new InvalidOperationException(SR.MsmqMessageAlreadyFrozen);
            }
            _ids.Add(id);
            _slotBytes.Add(slot);
        }

        private static void WriteUShort(byte[] buffer, int offset, ushort value)
        {
            buffer[offset] = (byte)(value & 0xFF);
            buffer[offset + 1] = (byte)((value >> 8) & 0xFF);
        }

        private static void WriteUInt32(byte[] buffer, int offset, uint value)
        {
            buffer[offset] = (byte)(value & 0xFF);
            buffer[offset + 1] = (byte)((value >> 8) & 0xFF);
            buffer[offset + 2] = (byte)((value >> 16) & 0xFF);
            buffer[offset + 3] = (byte)((value >> 24) & 0xFF);
        }

        private static void WriteIntPtr(byte[] buffer, int offset, IntPtr value)
        {
            if (IntPtr.Size == 8)
            {
                ulong v = unchecked((ulong)value.ToInt64());
                for (int i = 0; i < 8; i++)
                {
                    buffer[offset + i] = (byte)(v & 0xFF);
                    v >>= 8;
                }
            }
            else
            {
                uint v = unchecked((uint)value.ToInt32());
                for (int i = 0; i < 4; i++)
                {
                    buffer[offset + i] = (byte)(v & 0xFF);
                    v >>= 8;
                }
            }
        }

        // Exposes the calculated layout constants so unit tests can
        // verify the marshaling code without invoking MSMQ.
        internal static int PropVariantSizeForTests => PropVariantSize;
        internal static int VectorElementsOffsetForTests => VectorElementsOffset;

        // Test helper: dump the freshly-built propvar slot bytes for
        // a single property as a 24-byte array. Used by the
        // NativeMsmqMessage marshaling unit tests.
        internal byte[] GetSlotForTests(int index) => _slotBytes[index];
        internal uint GetPropIdForTests(int index) => _ids[index];
        internal int SlotCountForTests => _ids.Count;

        // Parses an MSMQ message-id string ("GUID\NUMBER") into the
        // 20-byte binary form MSMQ expects for PROPID_M_CORRELATIONID:
        // 16 bytes of GUID followed by a 4-byte little-endian counter.
        // Throws FormatException on a string that doesn't match the
        // expected shape — the caller is responsible for wrapping or
        // ignoring.
        internal static byte[] ParseMessageId(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                throw new FormatException(SR.MsmqInvalidMessageId);
            }
            int sep = id.LastIndexOf('\\');
            if (sep < 0 || sep == 0 || sep == id.Length - 1)
            {
                throw new FormatException(SR.MsmqInvalidMessageId);
            }
            string guidPart = id.Substring(0, sep);
            string idPart = id.Substring(sep + 1);
            if (!Guid.TryParseExact(guidPart, "D", out Guid g))
            {
                throw new FormatException(SR.MsmqInvalidMessageId);
            }
            if (!uint.TryParse(idPart, NumberStyles.Integer, CultureInfo.InvariantCulture, out uint counter))
            {
                throw new FormatException(SR.MsmqInvalidMessageId);
            }
            byte[] bytes = new byte[20];
            g.ToByteArray().CopyTo(bytes, 0);
            BitConverter.GetBytes(counter).CopyTo(bytes, 16);
            return bytes;
        }
    }
}
