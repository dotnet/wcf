// Copyright (c) Microsoft Corporation.
// Licensed under the MIT license.
using System;
using System.IO;
using System.Net;
using System.Runtime.InteropServices;
using System.Threading;
using Microsoft.Tools.ServiceModel.Svcutil;

namespace SvcutilTest
{
    static class DockerHelper
    {
        // This should match the name in the startTestServices.cmd script.
        private const string containerName = "svcutil-test-container";
        private const string startServiceScript = "startTestServices.cmd";
        private const string stopServiceScript = "stopTestServices.cmd";
        private const string hostIpEnvironmentVariable = "HOSTIP";
        private const string serviceIdEnvironmentVariable = "SERVICEID";
        private const string portOnHost = ":8080";

        public static string GetServiceContainerUrl()
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            {
                // If we're running on Linux check the environment variable for the host's hostname. This should be set by the host when the container is started.
                return Environment.GetEnvironmentVariable(hostIpEnvironmentVariable) + portOnHost;
            }
            else
            {
                // For Windows get the container's IP address.
                TryGetContainerIp(out string ip);
                return ip;
            }
        }

        public static string GetServiceContainerId()
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            {
                // Linux gets the service id from an environment variable passed into the docker container.
                return Environment.GetEnvironmentVariable(serviceIdEnvironmentVariable);
            }

            var ret = ProcessRunner.TryRunAsync("docker", $"ps -aqf \"name={containerName}\"", Directory.GetCurrentDirectory(), null, CancellationToken.None).Result;
            return ret.OutputText;
        }

        public static bool IsContainerRunning()
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            {
                // Linux means we're running the tests in a container. We have to assume the host has already started the service container.
                return true;
            }
            else
            {
                // Find whether the container is running by checking if you can get its IP address.
                return TryGetContainerIp(out string ip);
            }
        }

        private static bool TryGetContainerIp(out string ip)
        {
            var ret = ProcessRunner.TryRunAsync("docker", "inspect --format=\"{{.NetworkSettings.Networks.nat.IPAddress}}\" " + containerName, Directory.GetCurrentDirectory(), null, CancellationToken.None).Result;
            bool result = IPAddress.TryParse(ret.OutputText.Trim(), out IPAddress address);

            ip = address?.ToString();

            return result;
        }

        public static void StartContainer(string vsTestDirectory)
        {
            var ret = ProcessRunner.TryRunAsync("cmd.exe", "/C " + startServiceScript, vsTestDirectory, null, CancellationToken.None).Result;
        }

        public static void StopContainer(string vsTestDirectory)
        {
            var ret = ProcessRunner.TryRunAsync("cmd.exe", "/C " + stopServiceScript, vsTestDirectory, null, CancellationToken.None).Result;
        }
    }
}
