// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Threading.Tasks;

namespace ReportingService
{
    [ServiceContract]
    public interface IStressDataCollector
    {
        [OperationContract]
        RunId RunStarted(RunStartupData startupData);

        [OperationContract(IsOneWay = true)]
        Task RunHeartBeatAsync(RunId runId, int threadId, long iteration, DateTime time);

        [OperationContract(IsOneWay = true)]
        Task LogMessageAsync(RunId runId, string message);

        [OperationContract]
        Task RunFinishedAsync(RunId runId, bool success, string message);
    }

    public enum ProgramToRun { Stress, Perf }
    public enum TestToRun { HelloWorld, HelloWorldAPM, Streaming, Duplex, DuplexStreaming }
    public enum TestBinding { Http, Https, NetTcp, NetHttpBinding }
    public enum StreamingScenarios
    {
        StreamNone = 0,
        StreamOut = 1,
        StreamIn = 2,
        StreamEcho = 4,
        StreamAll = StreamOut | StreamIn | StreamEcho
    };

    [DataContract]
    public class RunStartupData
    {
        [DataMember]
        public string MachineName { get; set; }
        [DataMember]
        public ProgramToRun Program2Run { get; set; }
        [DataMember]
        public int StressLevel { get; set; }
        [DataMember]
        public TimeSpan StressRunDuration { get; set; }
        [DataMember]
        public string HostName { get; set; }
        [DataMember]
        public string AppName { get; set; }
        [DataMember]
        public TestBinding Binding { get; set; }
        [DataMember]
        public bool UseAsync { get; set; }
        [DataMember]
        public TestToRun TestToRun { get; set; }
        [DataMember]
        public StreamingScenarios StreamingScenario { get; set; }
        [DataMember]
        public bool PoolFactoriesForPerfStartup { get; set; }
        [DataMember]
        public bool UseSeparateTaskForEachChannel { get; set; }
        [DataMember]
        public int PerfMaxStartupTasks { get; set; }
        [DataMember]
        public int PerfMaxThroughputTasks { get; set; }
        [DataMember]
        public int PerfThroughputTaskStep { get; set; }
        [DataMember]
        public int PerfStartupIterations { get; set; }
        [DataMember]
        public TimeSpan PerfMeasurementDuration { get; set; }
        [DataMember]
        public int SHPortNum { get; set; }
        [DataMember]
        public int SHPorts { get; set; }

        // Security
        [DataMember]
        public SecurityMode BindingSecurityMode { get; set; }
        [DataMember]
        public HttpClientCredentialType HttpClientCredentialType { get; set; }
        [DataMember]
        public TcpClientCredentialType TcpClientCredentialType { get; set; }
        [DataMember]
        public string ServerDnsEndpointIdentity { get; set; }
        [DataMember]
        public string ClientCertThumbprint { get; set; }

        // Results
        [DataMember]
        public string ResultsLog { get; set; }
        [DataMember]
        public string RunName { get; set; }

        [DataMember]
        public long Iterations { get; set; }
    }

    [DataContract]
    public class RunId
    {
        [DataMember]
        public string RunIdPartitionKey { get; set; }
        [DataMember]
        public string RunIdRowKey { get; set; }
    }
}
