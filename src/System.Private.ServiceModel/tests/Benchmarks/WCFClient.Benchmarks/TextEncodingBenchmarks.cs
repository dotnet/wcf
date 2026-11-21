// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System;
using System.ServiceModel.Channels;
using System.Text;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Exporters.Json;

namespace WCFClient.Benchmarks
{
    [MemoryDiagnoser]
    [JsonExporterAttribute.Full]
    public class TextEncodingBenchmarks
    {
        private const int MaximumMessageSize = 2 * 1024 * 1024;
        private MessageEncoder _encoder;
        private BufferManager _bufferManager;
        private string _body;
        private byte[] _encodedMessage;

        [Params(1024, 65536, 1048576)]
        public int BodyCharacters { get; set; }

        [Params(false, true)]
        public bool Pooling { get; set; }

        [GlobalSetup]
        public void Setup()
        {
            var encoding = new TextMessageEncodingBindingElement(MessageVersion.Soap11, Encoding.UTF8);
            encoding.ReaderQuotas.MaxStringContentLength = MaximumMessageSize;
            _encoder = encoding.CreateMessageEncoderFactory().Encoder;
            _bufferManager = BufferManager.CreateBufferManager(Pooling ? 4 * MaximumMessageSize : 0, MaximumMessageSize);
            _body = new string('x', BodyCharacters);

            using Message message = CreateMessage();
            ArraySegment<byte> buffer = _encoder.WriteMessage(message, MaximumMessageSize, _bufferManager);
            try
            {
                _encodedMessage = buffer.AsSpan().ToArray();
            }
            finally
            {
                _bufferManager.ReturnBuffer(buffer.Array);
            }

            Validate();
            Program.WriteProvenance();
        }

        [Benchmark]
        public int Encode()
        {
            using Message message = CreateMessage();
            ArraySegment<byte> buffer = _encoder.WriteMessage(message, MaximumMessageSize, _bufferManager);
            try
            {
                return buffer.Count;
            }
            finally
            {
                _bufferManager.ReturnBuffer(buffer.Array);
            }
        }

        [Benchmark]
        public string Decode()
        {
            byte[] buffer = _bufferManager.TakeBuffer(_encodedMessage.Length);
            Buffer.BlockCopy(_encodedMessage, 0, buffer, 0, _encodedMessage.Length);
            // The decoder owns the rented array until the message is closed.
            using Message message = _encoder.ReadMessage(new ArraySegment<byte>(buffer, 0, _encodedMessage.Length),
                _bufferManager, _encoder.ContentType);
            return message.GetBody<string>();
        }

        [GlobalCleanup]
        public void Cleanup() => _bufferManager?.Clear();

        internal void Validate()
        {
            if (Encode() != _encodedMessage.Length || Decode() != _body)
            {
                throw new InvalidOperationException("The benchmark failed its SOAP encoding round-trip check.");
            }
        }

        private Message CreateMessage() => Message.CreateMessage(MessageVersion.Soap11, (string)null, _body);
    }
}
