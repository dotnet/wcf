// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

#if NET
using CoreWCF;
#else
using System;
using System.Collections.Generic;
using System.IO;
using System.ServiceModel;
using System.Threading;
using System.Threading.Tasks;
#endif

namespace WcfService
{
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.PerSession, ConcurrencyMode = ConcurrencyMode.Multiple)]
    public class WSDuplexService : IWSDuplexService
    {
        private static string s_contentToReplace = "ContentToReplace";
        private static string s_responseReplaceThisContent = "ResponseReplaceThisContent";
        private static string s_replacedContent = "ReplacedContent";
        private static string s_lastMessage = "LastMessage";

        private string _exceptionstring = string.Empty;
        private List<string> _log = new List<string>();
        private bool _continuePushingData;
        private Random _rand = new Random(DateTime.Now.Millisecond);
        private FlowControlledStream _localStream;

        public void UploadData(string data)
        {
            if (data.Contains(s_contentToReplace) || data.Contains(s_replacedContent) || data.Contains(s_responseReplaceThisContent))
            {
                _log.Add(string.Format("UploadData received {0}", data));
            }
            else
            {
                _log.Add(string.Format("UploadData received {0} length string.", data.Length));
            }
        }

        public string DownloadData()
        {
            string data = CreateInterestingString(_rand.Next(512, 4096));
            _log.Add(string.Format("DownloadData returning {0} length string", data.Length));
            return data;
        }

        public void UploadStream(Stream stream)
        {
            int readResult;
            int bytesRead = 0;
            byte[] buffer = new byte[1000];
            do
            {
                readResult = stream.Read(buffer, 0, buffer.Length);
                bytesRead += readResult;
            }
            while (readResult != 0);

            stream.Close();

            _log.Add(string.Format("UploadStream read {0} bytes from client's stream", bytesRead));
        }

        // Not using the localStream because this is the request-reply operation.
        public Stream DownloadStream()
        {
            _log.Add("DownloadStream");
            FlowControlledStream stream = new FlowControlledStream();
            stream.StreamDuration = TimeSpan.FromSeconds(1);
            stream.ReadThrottle = TimeSpan.FromMilliseconds(500);
            return stream;
        }

        public void StartPushingData()
        {
            _log.Add("StartPushingData");
            _continuePushingData = true;
            IPushCallback pushCallbackChannel = OperationContext.Current.GetCallbackChannel<IPushCallback>();
            Task.Factory.StartNew(PushData, pushCallbackChannel, CancellationToken.None, TaskCreationOptions.DenyChildAttach, TaskScheduler.Default);
        }

        public void StopPushingData()
        {
            _log.Add("StopPushingData");
            _continuePushingData = false;
        }

        public void StartPushingStream()
        {
            _log.Add("StartPushingStream");
            IPushCallback pushCallbackChannel = OperationContext.Current.GetCallbackChannel<IPushCallback>();
            Task.Factory.StartNew(PushStream, pushCallbackChannel, CancellationToken.None, TaskCreationOptions.DenyChildAttach, TaskScheduler.Default);
        }

        public void StartPushingStreamLongWait()
        {
            _log.Add("StartPushingStream");
            IPushCallback pushCallbackChannel = OperationContext.Current.GetCallbackChannel<IPushCallback>();
            Task.Factory.StartNew(PushStreamLongwait, pushCallbackChannel, CancellationToken.None, TaskCreationOptions.DenyChildAttach, TaskScheduler.Default);
        }

        public void StopPushingStream()
        {
            _log.Add("StopPushingStream");
            _localStream.StopStreaming = true;
        }

        private void PushData(object state)
        {
            IPushCallback pushCallbackChannel = state as IPushCallback;

            do
            {
                pushCallbackChannel.ReceiveData(CreateInterestingString(_rand.Next(4, 256)));
            }
            while (_continuePushingData);

            pushCallbackChannel.ReceiveData(s_lastMessage);
        }

        private void PushStream(object state)
        {
            IPushCallback pushCallbackChannel = state as IPushCallback;
            _localStream = new FlowControlledStream();
            _localStream.ReadThrottle = TimeSpan.FromMilliseconds(800);

            pushCallbackChannel.ReceiveStream(_localStream);
        }

        private void PushStreamLongwait(object state)
        {
            IPushCallback pushCallbackChannel = state as IPushCallback;
            _localStream = new FlowControlledStream();
            _localStream.ReadThrottle = TimeSpan.FromMilliseconds(3000);
            _localStream.StreamDuration = TimeSpan.FromSeconds(2);

            try
            {
                pushCallbackChannel.ReceiveStreamWithException(_localStream);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Server got the following exception: {0}", ex);
                _exceptionstring = ex.GetType().Name;
            }
        }

        /// <summary>
        /// This method was used to pass the exception message caught in the PushStreamLongwait method to the client
        /// </summary>
        /// <returns></returns>
        public string GetExceptionString()
        {
            return _exceptionstring;
        }

        public void GetLog()
        {
            IPushCallback pushCallbackChannel = OperationContext.Current.GetCallbackChannel<IPushCallback>();
            pushCallbackChannel.ReceiveLog(_log);
        }

        public string CreateInterestingString(int length)
        {
            char[] chars = new char[length];
            int index = 0;

            // Arrays of odd length will start with a single char.
            // The rest of the entries will be surrogate pairs.
            if (length % 2 == 1)
            {
                chars[index] = 'a';
                index++;
            }

            // Fill remaining entries with surrogate pairs
            char highSurrogate;
            char lowSurrogate;

            while (index < length)
            {
                highSurrogate = Convert.ToChar(_rand.Next(0xD800, 0xDC00));
                lowSurrogate = Convert.ToChar(_rand.Next(0xDC00, 0xE000));

                chars[index] = highSurrogate;
                chars[index + 1] = lowSurrogate;
                index += 2;
            }

            return new string(chars);
        }
    }
}
