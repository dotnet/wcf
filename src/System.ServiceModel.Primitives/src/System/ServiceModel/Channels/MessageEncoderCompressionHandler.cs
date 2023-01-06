// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.


using System.IO;
using System.IO.Compression;

namespace System.ServiceModel.Channels
{
    internal static class MessageEncoderCompressionHandler
    {
        internal const string GZipContentEncoding = "gzip";
        internal const string DeflateContentEncoding = "deflate";
        private const int DecompressBlockSize = 1024;

        internal static void DecompressBuffer(ref ArraySegment<byte> buffer, BufferManager bufferManager, CompressionFormat compressionFormat, long maxReceivedMessageSize)
        {
            MemoryStream memoryStream = new MemoryStream(buffer.Array, buffer.Offset, buffer.Count);
            int maxDecompressedSize = (int)Math.Min(maxReceivedMessageSize, int.MaxValue);

            using (BufferManagerOutputStream bufferedOutStream = new BufferManagerOutputStream(SRP.MaxReceivedMessageSizeExceeded, 1024, maxDecompressedSize, bufferManager))
            {
                bufferedOutStream.Write(buffer.Array, 0, buffer.Offset);

                byte[] tempBuffer = bufferManager.TakeBuffer(DecompressBlockSize);
                try
                {
                    using (Stream ds = compressionFormat == CompressionFormat.GZip ?
                        (Stream)new GZipStream(memoryStream, CompressionMode.Decompress) :
                        (Stream)new DeflateStream(memoryStream, CompressionMode.Decompress))
                    {
                        while (true)
                        {
                            int bytesRead = ds.Read(tempBuffer, 0, DecompressBlockSize);
                            if (bytesRead > 0)
                            {
                                bufferedOutStream.Write(tempBuffer, 0, bytesRead);
                            }
                            else
                            {
                                break;
                            }
                        }
                    }
                }
                finally
                {
                    bufferManager.ReturnBuffer(tempBuffer);
                }

                int length = 0;
                byte[] decompressedBytes = bufferedOutStream.ToArray(out length);
                bufferManager.ReturnBuffer(buffer.Array);
                buffer = new ArraySegment<byte>(decompressedBytes, buffer.Offset, length - buffer.Offset);
            }
        }

        internal static void CompressBuffer(ref ArraySegment<byte> buffer, BufferManager bufferManager, CompressionFormat compressionFormat)
        {
            using (BufferManagerOutputStream bufferedOutStream = new BufferManagerOutputStream(SRP.MaxSentMessageSizeExceeded, 1024, int.MaxValue, bufferManager))
            {
                bufferedOutStream.Write(buffer.Array, 0, buffer.Offset);

                using (Stream ds = compressionFormat == CompressionFormat.GZip ?
                    (Stream)new GZipStream(bufferedOutStream, CompressionMode.Compress, true) :
                    (Stream)new DeflateStream(bufferedOutStream, CompressionMode.Compress, true))
                {
                    ds.Write(buffer.Array, buffer.Offset, buffer.Count);
                }

                int length = 0;
                byte[] compressedBytes = bufferedOutStream.ToArray(out length);
                bufferManager.ReturnBuffer(buffer.Array);
                buffer = new ArraySegment<byte>(compressedBytes, buffer.Offset, length - buffer.Offset);
            }
        }

        internal static Stream GetDecompressStream(Stream compressedStream, CompressionFormat compressionFormat)
        {
            return compressionFormat == CompressionFormat.GZip ?
                    (Stream)new GZipStream(compressedStream, CompressionMode.Decompress, false) :
                    (Stream)new DeflateStream(compressedStream, CompressionMode.Decompress, false);
        }

        internal static Stream GetCompressStream(Stream uncompressedStream, CompressionFormat compressionFormat)
        {
            return compressionFormat == CompressionFormat.GZip ?
                    (Stream)new GZipStream(uncompressedStream, CompressionMode.Compress, true) :
                    (Stream)new DeflateStream(uncompressedStream, CompressionMode.Compress, true);
        }
    }
}
