// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.


using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel.Channels;
using System.Text;
using System.Threading.Tasks;

namespace System.ServiceModel.Tests.Common
{
    public class TestData
    {
        public static MemberDataSet<TimeSpan> ValidTimeOuts
        {
            get
            {
                return new MemberDataSet<TimeSpan>
                {
                    { TimeSpan.FromMilliseconds(0)},
                    { TimeSpan.FromMinutes(1)},
                    { TimeSpan.MaxValue},
                };
            }
        }

        public static MemberDataSet<TimeSpan> InvalidTimeOuts
        {
            get
            {
                return new MemberDataSet<TimeSpan>
                {
                    { TimeSpan.FromMinutes(-1)},
                    { TimeSpan.MinValue},
                };
            }
        }

        public static MemberDataSet<Encoding> ValidEncodings
        {
            get
            {
                return new MemberDataSet<Encoding>
                {
                    { Encoding.BigEndianUnicode },
                    { Encoding.Unicode },
                    { Encoding.UTF8 },
                };
            }
        }

        public static MemberDataSet<Encoding> InvalidEncodings
        {
            get
            {
                MemberDataSet<Encoding> data = new MemberDataSet<Encoding>();
                foreach (string encodingName in new string[] { "utf-7", "Windows-1252", "us-ascii", "iso-8859-1", "x-Chinese-CNS", "IBM273" })
                {
                    try
                    {
                        Encoding encoding = Encoding.GetEncoding(encodingName);
                        data.Add(encoding);
                    }
                    catch
                    {
                        // not all encodings are supported on all frameworks
                    }
                }
                return data;
            }
        }

        public static MemberDataSet<MessageVersion> ValidTextMessageEncoderMessageVersions
        {
            get
            {
                return new MemberDataSet<MessageVersion>
                {
                    { MessageVersion.Default },
                    { MessageVersion.Soap11 },
                    { MessageVersion.Soap12WSAddressing10 },
                    { MessageVersion.None },
                    { MessageVersion.CreateVersion(EnvelopeVersion.Soap12, AddressingVersion.None)},
                    { MessageVersion.CreateVersion(EnvelopeVersion.Soap11, AddressingVersion.WSAddressing10)}
                };
            }
        }

        public static MemberDataSet<MessageVersion> ValidBinaryMessageEncoderMessageVersions
        {
            get
            {
                return new MemberDataSet<MessageVersion>
                {
                    { MessageVersion.Default },
                    { MessageVersion.Soap12WSAddressing10 },
                };
            }
        }

        public static MemberDataSet<MessageVersion> InvalidBinaryMessageEncoderMessageVersions
        {
            get
            {
                return new MemberDataSet<MessageVersion>
                {
                    { MessageVersion.None },
                    { MessageVersion.Soap11 },
                };
            }
        }

        public static MemberDataSet<MessageVersion, EnvelopeVersion, AddressingVersion> MessageVersionsWithEnvelopeAndAddressingVersions
        {
            get
            {
                return new MemberDataSet<MessageVersion, EnvelopeVersion, AddressingVersion>
            {
                { MessageVersion.None, EnvelopeVersion.None, AddressingVersion.None },
                { MessageVersion.Soap11, EnvelopeVersion.Soap11, AddressingVersion.None },
                { MessageVersion.Soap12WSAddressing10, EnvelopeVersion.Soap12, AddressingVersion.WSAddressing10 }
            };
            }
        }
    }
}