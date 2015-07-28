// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Collections.Generic;
using System.Runtime.Serialization;

namespace WcfService
{
    internal class MahojongTypes
    {
        [DataContract(Name = "ResultOf{0}", Namespace = "http://www.contoso.com/wcfnamespace")]
        public class ResultObject<TEntity>
        {
            private string _errorMessage;

            public ResultObject()
            {
                this.ErrorMessage = "OK";
                this.HttpStatusCode = System.Net.HttpStatusCode.OK;
                this.ErrorCode = 0;
                this.Result = default(TEntity);
            }

            public static ResultObject<T> CopyResultErrorsStatus<T, D>(ResultObject<D> anotherResult)
            {
                return new ResultObject<T> { ErrorCode = anotherResult.ErrorCode, ErrorMessage = anotherResult.ErrorMessage, HttpStatusCode = anotherResult.HttpStatusCode };
            }

            public static ResultObject<T> CreateDefault<T>()
            {
                return new ResultObject<T> { Result = default(T), ErrorCode = 0, ErrorMessage = MahojongTypes.ErrorMessage.Get(MahojongTypes.ErrorCode.Ok) };
            }

            public void Exception(System.Exception ex)
            {
                this.ErrorCode = -1;
                this.ErrorMessage = (ex == null) ? "unexpected" : ex.Message;
            }

            [DataMember]
            public int ErrorCode { get; set; }

            [DataMember]
            public string ErrorMessage
            {
                get
                {
                    return _errorMessage;
                }
                set
                {
                    _errorMessage = value;
                }
            }

            [DataMember]
            public System.Net.HttpStatusCode HttpStatusCode { get; set; }

            [DataMember(Name = "Result")]
            public TEntity Result { get; set; }
        }

        public static class ErrorMessage
        {
            private static Dictionary<ErrorCode, string> s_localizedErrorCodes;

            public static string Get(ErrorCode errorCode)
            {
                if (s_localizedErrorCodes != null)
                {
                    return (s_localizedErrorCodes.ContainsKey(errorCode) ? s_localizedErrorCodes[errorCode] : s_localizedErrorCodes[ErrorCode.UnknownException]);
                }
                return "Unexpected exception";
            }

            public static string GetErrorDescription(ErrorCode errorCode)
            {
                switch (errorCode)
                {
                    case ErrorCode.Ok:
                        return "Success";

                    case ErrorCode.DcXboxTokeNull:
                    case ErrorCode.DcDailyFileNotAvailable:
                    case ErrorCode.DcDailyFileBroken:
                        return "XboxErrorText";

                    case ErrorCode.DcMonthlyFileNotAvailable:
                    case ErrorCode.DcMonthlyFileBroken:
                        return "DCDownloadingDataErrorText";

                    case ErrorCode.DcCanNotWriteMonthlyUserProgress:
                    case ErrorCode.DcCanNotWriteDailyUserProgress:
                        return "XboxErrorSavingText";

                    case ErrorCode.NotOwner:
                        return "Current user is not owner of theme and can't change it";

                    case ErrorCode.ThemeNotFound:
                        return "Theme not found and can't be updated";

                    case ErrorCode.AsyncOperationFault:
                        return "AsyncOperationFault";

                    case ErrorCode.DataNotFound:
                        return "Data not found";

                    case ErrorCode.CantShare:
                        return "Theme can't be shared due to internal error";

                    case ErrorCode.GamePlayIsNotValid:
                        return "Game play is not valid";

                    case ErrorCode.UserNotAuthenticated:
                        return "User not authenticated";

                    case ErrorCode.UnknownException:
                        return "Exception cant be handled correctly";

                    case ErrorCode.NullData:
                        return "Null Data was passed to the service";

                    case ErrorCode.SameData:
                        return "Same data was requested";

                    case ErrorCode.OnlineDataReceived:
                        return "Online Data received successfully";

                    case ErrorCode.OfflineDataReceived:
                        return "Offline Data received successfully";

                    case ErrorCode.OfflineOnlineDataReceived:
                        return "Online and Offline Data received successfully";

                    case ErrorCode.LatencyOverhead:
                        return "Request latency overhead";
                }
                return "Unexpected exception";
            }

            public static void Init(Dictionary<ErrorCode, string> localizedErrorCodes)
            {
                ErrorMessage.s_localizedErrorCodes = localizedErrorCodes;
            }
        }

        public enum ErrorCode
        {
            AsyncOperationFault = 0x67,
            CantShare = 0x69,
            DataNotFound = 0x68,
            DcCanNotWriteDailyUserProgress = 7,
            DcCanNotWriteMonthlyUserProgress = 6,
            DcDailyFileBroken = 5,
            DcDailyFileNotAvailable = 4,
            DcFileBroken = 8,
            DcMonthlyFileBroken = 3,
            DcMonthlyFileNotAvailable = 2,
            DcUserMonthlyFileIsNotAvaliable = 9,
            DcXboxTokeNull = 1,
            DeserializeError = 12,
            GamePlayIsNotValid = 0xc9,
            LatencyOverhead = 0x198,
            NotOwner = 0x65,
            NullData = 0x195,
            OfflineDataReceived = 0x321,
            OfflineOnlineDataReceived = 0x322,
            Ok = 0,
            OnlineDataReceived = 800,
            PremiumErrorNoInternetConnection = 0x1f7,
            SameData = 0x130,
            SponsorThemeIncorrectFormat = 15,
            ThemeNotFound = 0x66,
            UnknownException = 0x194,
            UserNotAuthenticated = 0x191
        }

        [DataContract(Name = "UserGamePlay")]
        public class UserGamePlay : IUserGamePlayEntity
        {
            public UserGamePlay()
            {
            }

            public UserGamePlay(IUserGamePlayEntity userGamePlay)
            {
                if (userGamePlay != null)
                {
                    this.Key = userGamePlay.Key;
                    this.Value = userGamePlay.Value;
                    this.GameKey = userGamePlay.GameKey;
                    this.TimeStamp = userGamePlay.TimeStamp;
                    this.UserId = userGamePlay.UserId;
                }
            }

            private static UserGamePlay Convert(IUserGamePlayEntity userGamePlay)
            {
                if (userGamePlay == null)
                {
                    return null;
                }
                return new UserGamePlay { Key = userGamePlay.Key, Value = userGamePlay.Value, GameKey = userGamePlay.GameKey, TimeStamp = userGamePlay.TimeStamp, UserId = userGamePlay.UserId };
            }

            public static UserGamePlay ToUserGamePlay(IUserGamePlayEntity userGamePlayTableEntity)
            {
                return Convert(userGamePlayTableEntity);
            }

            [DataMember(Name = "GameKey")]
            public string GameKey { get; set; }

            [DataMember(Name = "Key")]
            public string Key { get; set; }

            [DataMember(Name = "TimeStamp")]
            public string TimeStamp { get; set; }

            [DataMember(Name = "UserId")]
            public string UserId { get; set; }

            [DataMember(Name = "Value")]
            public string Value { get; set; }
        }

        public interface IUserGamePlayEntity
        {
            string GameKey { get; set; }

            string Key { get; set; }

            string TimeStamp { get; set; }

            string UserId { get; set; }

            string Value { get; set; }
        }
    }
}
