// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.Runtime.InteropServices;
using System.Security;
using System.Security.AccessControl;
using System.Security.Principal;

namespace System.ServiceModel.Activation
{
    unsafe static class Utility
    {
        const string WindowsServiceAccountFormat = "NT Service\\{0}";

        internal static Uri FormatListenerEndpoint(string serviceName, string listenerEndPoint)
        {
            UriBuilder builder = new UriBuilder(Uri.UriSchemeNetPipe, serviceName);
            builder.Path = string.Format(CultureInfo.InvariantCulture, "/{0}/", listenerEndPoint);
            return builder.Uri;
        }

        static SafeCloseHandle OpenCurrentProcessForWrite()
        {
            int processId = Process.GetCurrentProcess().Id;
#pragma warning disable CS1634 // Expected 'disable' or 'restore' after #pragma warning
#pragma warning suppress 56523 // elliotw, Win32Exception ctor calls Marshal.GetLastWin32Error()
            SafeCloseHandle process = ListenerUnsafeNativeMethods.OpenProcess(ListenerUnsafeNativeMethods.PROCESS_QUERY_INFORMATION | ListenerUnsafeNativeMethods.WRITE_DAC | ListenerUnsafeNativeMethods.READ_CONTROL, false, processId);
#pragma warning restore CS1634 // Expected 'disable' or 'restore' after #pragma warning
            if (process.IsInvalid)
            {
                Exception exception = new Win32Exception();
                process.SetHandleAsInvalid();
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(exception);
            }
            return process;
        }

        static SafeCloseHandle OpenProcessForQuery(int pid)
        {
#pragma warning disable CS1634 // Expected 'disable' or 'restore' after #pragma warning
#pragma warning suppress 56523 // elliotw, Win32Exception ctor calls Marshal.GetLastWin32Error()
            SafeCloseHandle process = ListenerUnsafeNativeMethods.OpenProcess(ListenerUnsafeNativeMethods.PROCESS_QUERY_INFORMATION, false, pid);
#pragma warning restore CS1634 // Expected 'disable' or 'restore' after #pragma warning
            if (process.IsInvalid)
            {
                Exception exception = new Win32Exception();
                process.SetHandleAsInvalid();
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(exception);
            }
            return process;
        }

        static SafeCloseHandle GetProcessToken(SafeCloseHandle process, int requiredAccess)
        {
            SafeCloseHandle processToken;
            bool success = ListenerUnsafeNativeMethods.OpenProcessToken(process, requiredAccess, out processToken);
            int error = Marshal.GetLastWin32Error();
            if (!success)
            {
                //System.ServiceModel.Diagnostics.Utility.CloseInvalidOutSafeHandle(processToken);
                //throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new Win32Exception(error));
                throw new Exception("Win32Exception(error)");
            }

            return processToken;
        }

        static int GetTokenInformationLength(SafeCloseHandle token, ListenerUnsafeNativeMethods.TOKEN_INFORMATION_CLASS tic)
        {
            int lengthNeeded;
            bool success = ListenerUnsafeNativeMethods.GetTokenInformation(token, tic, null, 0, out lengthNeeded);
            if (!success)
            {
                int error = Marshal.GetLastWin32Error();
                if (error != ListenerUnsafeNativeMethods.ERROR_INSUFFICIENT_BUFFER)
                {
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new Win32Exception(error));
                }
            }

            return lengthNeeded;
        }

        static void GetTokenInformation(SafeCloseHandle token, ListenerUnsafeNativeMethods.TOKEN_INFORMATION_CLASS tic, byte[] tokenInformation)
        {
            int lengthNeeded;
            if (!ListenerUnsafeNativeMethods.GetTokenInformation(token, tic, tokenInformation, tokenInformation.Length, out lengthNeeded))
            {
                int error = Marshal.GetLastWin32Error();
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new Win32Exception(error));
            }
        }

        static SafeServiceHandle OpenSCManager()
        {
#pragma warning disable CS1634 // Expected 'disable' or 'restore' after #pragma warning
#pragma warning suppress 56523 // elliotw, Win32Exception ctor calls Marshal.GetLastWin32Error()
            SafeServiceHandle scManager = ListenerUnsafeNativeMethods.OpenSCManager(null, null, ListenerUnsafeNativeMethods.SC_MANAGER_CONNECT);
#pragma warning restore CS1634 // Expected 'disable' or 'restore' after #pragma warning
            if (scManager.IsInvalid)
            {
                Exception exception = new Win32Exception();
                scManager.SetHandleAsInvalid();
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(exception);
            }
            return scManager;
        }

        static SafeServiceHandle OpenService(SafeServiceHandle scManager, string serviceName, int purpose)
        {
#pragma warning disable CS1634 // Expected 'disable' or 'restore' after #pragma warning
#pragma warning suppress 56523 // elliotw, Win32Exception ctor calls Marshal.GetLastWin32Error()
            SafeServiceHandle service = ListenerUnsafeNativeMethods.OpenService(scManager, serviceName, purpose);
#pragma warning restore CS1634 // Expected 'disable' or 'restore' after #pragma warning
            if (service.IsInvalid)
            {
                Exception exception = new Win32Exception();
                service.SetHandleAsInvalid();
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(exception);
            }
            return service;
        }

        internal static void AddRightGrantedToAccounts(List<SecurityIdentifier> accounts, int right, bool onProcess)
        {
            SafeCloseHandle process = OpenCurrentProcessForWrite();
            try
            {
                if (onProcess)
                {
                    EditKernelObjectSecurity(process, accounts, null, right, true);
                }
                else
                {
                    SafeCloseHandle token = GetProcessToken(process, ListenerUnsafeNativeMethods.TOKEN_QUERY | ListenerUnsafeNativeMethods.WRITE_DAC | ListenerUnsafeNativeMethods.READ_CONTROL);
                    try
                    {
                        EditKernelObjectSecurity(token, accounts, null, right, true);
                    }
                    finally
                    {
                        token.Close();
                    }
                }
            }
            finally
            {
                process.Close();
            }
        }

        internal static void AddRightGrantedToAccount(SecurityIdentifier account, int right)
        {
            SafeCloseHandle process = OpenCurrentProcessForWrite();
            try
            {
                EditKernelObjectSecurity(process, null, account, right, true);
            }
            finally
            {
                process.Close();
            }
        }

        internal static void RemoveRightGrantedToAccount(SecurityIdentifier account, int right)
        {
            SafeCloseHandle process = OpenCurrentProcessForWrite();
            try
            {
                EditKernelObjectSecurity(process, null, account, right, false);
            }
            finally
            {
                process.Close();
            }
        }

        // Do not use this method unless you understand the consequnces of lack of synchronization
        static void EditKernelObjectSecurity(SafeCloseHandle kernelObject, List<SecurityIdentifier> accounts, SecurityIdentifier account, int right, bool add)
        {
            // take the SECURITY_DESCRIPTOR from the kernelObject
            int lpnLengthNeeded;
            bool success = ListenerUnsafeNativeMethods.GetKernelObjectSecurity(kernelObject, ListenerUnsafeNativeMethods.DACL_SECURITY_INFORMATION, null, 0, out lpnLengthNeeded);
            if (!success)
            {
                int errorCode = Marshal.GetLastWin32Error();
                if (errorCode != ListenerUnsafeNativeMethods.ERROR_INSUFFICIENT_BUFFER)
                {
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new Win32Exception(errorCode));
                }
            }
            byte[] pSecurityDescriptor = new byte[lpnLengthNeeded];
#pragma warning disable CS1634 // Expected 'disable' or 'restore' after #pragma warning
#pragma warning suppress 56523 // elliotw, Win32Exception ctor calls Marshal.GetLastWin32Error()
            success = ListenerUnsafeNativeMethods.GetKernelObjectSecurity(kernelObject, ListenerUnsafeNativeMethods.DACL_SECURITY_INFORMATION, pSecurityDescriptor, pSecurityDescriptor.Length, out lpnLengthNeeded);
#pragma warning restore CS1634 // Expected 'disable' or 'restore' after #pragma warning
            if (!success)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new Win32Exception());
            }
            CommonSecurityDescriptor securityDescriptor = new CommonSecurityDescriptor(false, false, pSecurityDescriptor, 0);
            DiscretionaryAcl dacl = securityDescriptor.DiscretionaryAcl;
            // add ACEs to the SECURITY_DESCRIPTOR of the kernelObject
            if (account != null)
            {
                EditDacl(dacl, account, right, add);
            }
            else if (accounts != null)
            {
                foreach (SecurityIdentifier accountInList in accounts)
                {
                    EditDacl(dacl, accountInList, right, add);
                }
            }
            lpnLengthNeeded = securityDescriptor.BinaryLength;
            pSecurityDescriptor = new byte[lpnLengthNeeded];
            securityDescriptor.GetBinaryForm(pSecurityDescriptor, 0);
            // set the SECURITY_DESCRIPTOR on the kernelObject
#pragma warning disable CS1634 // Expected 'disable' or 'restore' after #pragma warning
#pragma warning suppress 56523 // elliotw, Win32Exception ctor calls Marshal.GetLastWin32Error()
            success = ListenerUnsafeNativeMethods.SetKernelObjectSecurity(kernelObject, ListenerUnsafeNativeMethods.DACL_SECURITY_INFORMATION, pSecurityDescriptor);
#pragma warning restore CS1634 // Expected 'disable' or 'restore' after #pragma warning
            if (!success)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new Win32Exception());
            }
        }

        static void EditDacl(DiscretionaryAcl dacl, SecurityIdentifier account, int right, bool add)
        {
            if (add)
            {
                dacl.AddAccess(AccessControlType.Allow, account, right, InheritanceFlags.None, PropagationFlags.None);
            }
            else
            {
                dacl.RemoveAccess(AccessControlType.Allow, account, right, InheritanceFlags.None, PropagationFlags.None);
            }
        }

        [SecuritySafeCritical]
        internal static SecurityIdentifier GetLogonSidForPid(int pid)
        {
            SafeCloseHandle process = OpenProcessForQuery(pid);
            try
            {
                SafeCloseHandle token = GetProcessToken(process, ListenerUnsafeNativeMethods.TOKEN_QUERY);
                try
                {
                    int length = GetTokenInformationLength(token, ListenerUnsafeNativeMethods.TOKEN_INFORMATION_CLASS.TokenGroups);
                    byte[] tokenInformation = new byte[length];
                    fixed (byte* pTokenInformation = tokenInformation)
                    {
                        GetTokenInformation(token, ListenerUnsafeNativeMethods.TOKEN_INFORMATION_CLASS.TokenGroups, tokenInformation);

                        ListenerUnsafeNativeMethods.TOKEN_GROUPS* ptg = (ListenerUnsafeNativeMethods.TOKEN_GROUPS*)pTokenInformation;
                        ListenerUnsafeNativeMethods.SID_AND_ATTRIBUTES* sids = (ListenerUnsafeNativeMethods.SID_AND_ATTRIBUTES*)(&(ptg->_groups));
                        for (int i = 0; i < ptg->_groupCount; i++)
                        {
                            if ((sids[i]._attributes & ListenerUnsafeNativeMethods.SidAttribute.SE_GROUP_LOGON_ID) == ListenerUnsafeNativeMethods.SidAttribute.SE_GROUP_LOGON_ID)
                            {
                                return new SecurityIdentifier(sids[i]._sid);
                            }
                        }
                    }
                    return new SecurityIdentifier(WellKnownSidType.LocalSystemSid, null);
                }
                finally
                {
                    token.Close();
                }
            }
            finally
            {
                process.Close();
            }
        }

        [SecuritySafeCritical]
        static ListenerUnsafeNativeMethods.SERVICE_STATUS_PROCESS GetStatusForService(string serviceName)
        {
            SafeServiceHandle scManager = OpenSCManager();
            try
            {
                SafeServiceHandle service = OpenService(scManager, serviceName, ListenerUnsafeNativeMethods.SERVICE_QUERY_STATUS);
                try
                {
                    int lpnLengthNeeded;
                    bool success = ListenerUnsafeNativeMethods.QueryServiceStatusEx(service, ListenerUnsafeNativeMethods.SC_STATUS_PROCESS_INFO, null, 0, out lpnLengthNeeded);
                    if (!success)
                    {
                        int errorCode = Marshal.GetLastWin32Error();
                        if (errorCode != ListenerUnsafeNativeMethods.ERROR_INSUFFICIENT_BUFFER)
                        {
                            throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new Win32Exception(errorCode));
                        }
                    }
                    byte[] serviceStatusProcess = new byte[lpnLengthNeeded];
#pragma warning disable CS1634 // Expected 'disable' or 'restore' after #pragma warning
#pragma warning suppress 56523 // elliotw, Win32Exception ctor calls Marshal.GetLastWin32Error()
                    success = ListenerUnsafeNativeMethods.QueryServiceStatusEx(service, ListenerUnsafeNativeMethods.SC_STATUS_PROCESS_INFO, serviceStatusProcess, serviceStatusProcess.Length, out lpnLengthNeeded);
#pragma warning restore CS1634 // Expected 'disable' or 'restore' after #pragma warning
                    if (!success)
                    {
                        throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new Win32Exception());
                    }
                    fixed (byte* pServiceStatusProcess = serviceStatusProcess)
                    {
                        return (ListenerUnsafeNativeMethods.SERVICE_STATUS_PROCESS)Marshal.PtrToStructure((IntPtr)pServiceStatusProcess, typeof(ListenerUnsafeNativeMethods.SERVICE_STATUS_PROCESS));
                    }
                }
                finally
                {
                    service.Close();
                }
            }
            finally
            {
                scManager.Close();
            }
        }
    }
}
