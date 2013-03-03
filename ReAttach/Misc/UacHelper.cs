using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Security.Principal;
using EnvDTE80;
using Microsoft.Win32;

namespace ReAttach.Misc
{
	public static class UacHelper
	{
		private const string UacRegistryKey = "Software\\Microsoft\\Windows\\CurrentVersion\\Policies\\System";
		private const string UacRegistryValue = "EnableLUA";

		private const uint StandardRightsRead = 0x00020000;
		private const uint TokenQuery = 0x0008;
		private const uint TokenRead = (StandardRightsRead | TokenQuery);

		[DllImport("advapi32.dll", SetLastError = true)]
		[return: MarshalAs(UnmanagedType.Bool)]
		static extern bool OpenProcessToken(IntPtr processHandle, UInt32 desiredAccess, out IntPtr tokenHandle);

		[DllImport("advapi32.dll", SetLastError = true)]
		public static extern bool GetTokenInformation(IntPtr tokenHandle, TokenInformationClass tokenInformationClass, 
			IntPtr tokenInformation, uint tokenInformationLength, out uint returnLength);

		public enum TokenInformationClass
		{
			TokenUser = 1,
			TokenGroups,
			TokenPrivileges,
			TokenOwner,
			TokenPrimaryGroup,
			TokenDefaultDacl,
			TokenSource,
			TokenType,
			TokenImpersonationLevel,
			TokenStatistics,
			TokenRestrictedSids,
			TokenSessionId,
			TokenGroupsAndPrivileges,
			TokenSessionReference,
			TokenSandBoxInert,
			TokenAuditPolicy,
			TokenOrigin,
			TokenElevationType,
			TokenLinkedToken,
			TokenElevation,
			TokenHasRestrictions,
			TokenAccessInformation,
			TokenVirtualizationAllowed,
			TokenVirtualizationEnabled,
			TokenIntegrityLevel,
			TokenUIAccess,
			TokenMandatoryPolicy,
			TokenLogonSid,
			MaxTokenInfoClass
		}

		public enum TokenElevationType
		{
			TokenElevationTypeDefault = 1,
			TokenElevationTypeFull,
			TokenElevationTypeLimited
		}

		public static bool IsUacEnabled
		{
			get
			{
				RegistryKey uacKey = Registry.LocalMachine.OpenSubKey(UacRegistryKey, false);
				bool result = uacKey.GetValue(UacRegistryValue).Equals(1);
				return result;
			}
		}

		public static bool IsProcessElevated(Process2 process2)
		{
			if (IsUacEnabled)
			{
				var process = Process.GetProcessById(process2.ProcessID);
				IntPtr tokenHandle;
				if (!OpenProcessToken(process.Handle, TokenRead, out tokenHandle))
					throw new ApplicationException("Could not get process token.  Win32 Error Code: " + Marshal.GetLastWin32Error());

				var elevationResult = TokenElevationType.TokenElevationTypeDefault;
				var elevationResultSize = Marshal.SizeOf((int)elevationResult);
				var elevationTypePtr = Marshal.AllocHGlobal(elevationResultSize);
				uint returnedSize = 0;
				var success = GetTokenInformation(tokenHandle, TokenInformationClass.TokenElevationType, elevationTypePtr, (uint)elevationResultSize, out returnedSize);
				if (success)
				{
					elevationResult = (TokenElevationType)Marshal.ReadInt32(elevationTypePtr);
					var isProcessAdmin = elevationResult == TokenElevationType.TokenElevationTypeFull;
					return isProcessAdmin;
				}
				throw new ApplicationException("Unable to determine the current elevation.");
			}
			else
			{
				var identity = WindowsIdentity.GetCurrent();
				var principal = new WindowsPrincipal(identity);
				var result = principal.IsInRole(WindowsBuiltInRole.Administrator);
				return result;
			}
		}
	}
}
