// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.


using System;
using System.Runtime.CompilerServices;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Description;
using Infrastructure.Common;
using TestTypes;
using Xunit;

public static class ContractDescriptionTest
{
    [WcfFact]
    public static void Manually_Generated_Service_Type()
    {
        // -----------------------------------------------------------------------------------------------
        // IDescriptionTestsService:
        //    Contains 2 operations, synchronous versions only.
        // -----------------------------------------------------------------------------------------------
        string results = ContractDescriptionTestHelper.ValidateContractDescription<IDescriptionTestsService>(new ContractDescriptionData
        {
            Operations = new OperationDescriptionData[]
            {
                new OperationDescriptionData
                {
                    Name = "MessageRequestReply",
                    IsOneWay = false,
                    HasTask = false,
                    Messages = new MessageDescriptionData[]
                    {
                        new MessageDescriptionData
                        {
                            Action = "http://tempuri.org/IDescriptionTestsService/MessageRequestReply",
                            Direction = MessageDirection.Input
                        },
                        new MessageDescriptionData
                        {
                            Action = "http://tempuri.org/IDescriptionTestsService/MessageRequestReplyResponse",
                            Direction = MessageDirection.Output
                        }
                    }
                 },
                new OperationDescriptionData
                {
                    Name = "Echo",
                    IsOneWay = false,
                    HasTask = false,
                    Messages = new MessageDescriptionData[]
                    {
                        new MessageDescriptionData
                        {
                            Action = "http://tempuri.org/IDescriptionTestsService/Echo",
                            Direction = MessageDirection.Input
                        },
                        new MessageDescriptionData
                        {
                            Action = "http://tempuri.org/IDescriptionTestsService/EchoResponse",
                            Direction = MessageDirection.Output
                        }
                    }
                }
            }
        });

        // Assert.True because results contains informative error failure
        Assert.True(results == null, results);
    }

    [WcfFact]
    public static void SvcUtil_Generated_Service_Type()
    {
        // -----------------------------------------------------------------------------------------------
        // IDescriptionTestsServiceGenerated:
        //    Generated via SvcUtil and contains sync and Task versions of same 2 operations as above.
        // -----------------------------------------------------------------------------------------------
        string results = ContractDescriptionTestHelper.ValidateContractDescription<IDescriptionTestsServiceGenerated>(new ContractDescriptionData
        {
            Operations = new OperationDescriptionData[]
            {
                new OperationDescriptionData
                {
                    Name = "MessageRequestReply",
                    IsOneWay = false,
                    HasTask = true,
                    Messages = new MessageDescriptionData[]
                    {
                        new MessageDescriptionData
                        {
                            Action = "http://tempuri.org/IDescriptionTestsService/MessageRequestReply",
                            Direction = MessageDirection.Input
                        },
                        new MessageDescriptionData
                        {
                            Action = "http://tempuri.org/IDescriptionTestsService/MessageRequestReplyResponse",
                            Direction = MessageDirection.Output
                        }
                    }
                },
                new OperationDescriptionData
                {
                    Name = "Echo",
                    IsOneWay = false,
                    HasTask = true,
                    Messages = new MessageDescriptionData[]
                    {
                        new MessageDescriptionData
                        {
                            Action = "http://tempuri.org/IDescriptionTestsService/Echo",
                            Direction = MessageDirection.Input
                        },
                        new MessageDescriptionData
                        {
                            Action = "http://tempuri.org/IDescriptionTestsService/EchoResponse",
                            Direction = MessageDirection.Output
                        }
                    }
                }
            }
        });

        // Assert.True because results contains informative error failure
        Assert.True(results == null, results);
    }

    [WcfFact]
    public static void MessageContract_Service_Type()
    {
        // -----------------------------------------------------------------------------------------------
        // IFeedbackService:
        //    Service exposes a single async operation that uses MessageContract.
        //    This variant tests the a MessageContract can build "typed messages" for the ContractDescription.
        // -----------------------------------------------------------------------------------------------
        string results = ContractDescriptionTestHelper.ValidateContractDescription<IFeedbackService>(new ContractDescriptionData
        {
            Operations = new OperationDescriptionData[]
            {
                new OperationDescriptionData
                {
                    Name = "Feedback",
                    IsOneWay = false,
                    HasTask = true,
                    Messages = new MessageDescriptionData[]
                    {
                        new MessageDescriptionData
                        {
                            Action = "http://app.my.com/MyFeedback/Feedback",
                            Direction = MessageDirection.Input,
                            MessageType = typeof(FeedbackRequest)
                        },
                        new MessageDescriptionData
                        {
                            Action = "*",
                            Direction = MessageDirection.Output,
                            MessageType = typeof(FeedbackResponse)
                        }
                    }
               }
            }
        });

        // Assert.True because results contains informative error failure
        Assert.True(results == null, results);
    }

    [WcfFact]
    public static void Duplex_ContractDescription_Builds_From_ServiceContract()
    {
        // Arrange
        CustomBinding binding = new CustomBinding();
        binding.Elements.Add(new TextMessageEncodingBindingElement());
        binding.Elements.Add(new HttpTransportBindingElement());
        EndpointAddress address = new EndpointAddress(FakeAddress.HttpAddress);

        //Act
        ChannelFactory<IDuplexHello> factory = new ChannelFactory<IDuplexHello>(binding, address);
        ContractDescription contract = factory.Endpoint.Contract;

        // Assert
        Assert.NotNull(contract);
        Assert.Equal<Type>(typeof(IHelloCallbackContract), contract.CallbackContractType);

        // Duplex contracts capture operations from both the service and the callback type
        Assert.Equal(2, contract.Operations.Count);
        OperationDescription operation = contract.Operations.Find("Hello");
        Assert.True(operation != null, "Failed to find Hello operation in contract.");
        Assert.True(operation.IsOneWay, "Expected Hello operation to be IsOneWay.");

        operation = contract.Operations.Find("Reply");
        Assert.True(operation != null, "Failed to find Reply operation in contract.");
        Assert.True(operation.IsOneWay, "Expected Reply operation to be IsOneWay.");
    }

    [WcfFact]
    public static void ContractDescription_GetContract()
    {
        // Simple validation of the newly exposed "public ContractDescription GetContract(Type contractType);" method
        ContractDescription contractDescription = ContractDescription.GetContract(typeof(IDescriptionTestsService));
        Assert.Equal(typeof(IDescriptionTestsService).Name, contractDescription.ContractType.Name);
        Assert.Equal("http://tempuri.org/", contractDescription.Namespace);
    }

    [WcfFact]
    public static void OperationDescription_BeginEndSyncMethod_Property()
    {
        ContractDescription contractDescription = ContractDescription.GetContract(typeof(IDescriptionTestsServiceBeginEndGenerated));
        Assert.Equal(2, contractDescription.Operations.Count);
        foreach(OperationDescription operation in contractDescription.Operations)
        {
            Assert.NotNull(operation.BeginMethod);
            Assert.NotNull(operation.EndMethod);
            if(operation.Name.Equals("Echo"))
            {
                Assert.Equal(typeof(IDescriptionTestsServiceBeginEndGenerated).GetMethod(nameof(IDescriptionTestsServiceBeginEndGenerated.BeginEcho)), operation.BeginMethod);
                Assert.Equal(typeof(IDescriptionTestsServiceBeginEndGenerated).GetMethod(nameof(IDescriptionTestsServiceBeginEndGenerated.EndEcho)), operation.EndMethod);
            }
            else
            {
                Assert.Equal(typeof(IDescriptionTestsServiceBeginEndGenerated).GetMethod(nameof(IDescriptionTestsServiceBeginEndGenerated.BeginMessageRequestReply)), operation.BeginMethod);
                Assert.Equal(typeof(IDescriptionTestsServiceBeginEndGenerated).GetMethod(nameof(IDescriptionTestsServiceBeginEndGenerated.EndMessageRequestReply)), operation.EndMethod);
            }
        }

        contractDescription = ContractDescription.GetContract(typeof(IDescriptionTestsService));
        Assert.Equal(2, contractDescription.Operations.Count);
        foreach (OperationDescription operation in contractDescription.Operations)
        {
            Assert.NotNull(operation.SyncMethod);
            if (operation.Name.Equals("Echo"))
            {
                Assert.Equal(typeof(IDescriptionTestsService).GetMethod(nameof(IDescriptionTestsService.Echo)), operation.SyncMethod);
            }
            else
            {
                Assert.Equal(typeof(IDescriptionTestsService).GetMethod(nameof(IDescriptionTestsService.MessageRequestReply)), operation.SyncMethod);
            }
        }
    }
}
