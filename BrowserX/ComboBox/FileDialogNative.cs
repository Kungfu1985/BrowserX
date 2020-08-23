// System.Windows.Forms.FileDialogNative
using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Windows.Forms;

internal static class FileDialogNative
{
	[ComImport]
	[Guid("d57c7288-d4ad-4768-be02-9d969532d960")]
	[CoClass(typeof(FileOpenDialogRCW))]
	internal interface NativeFileOpenDialog : IFileOpenDialog, IFileDialog
	{
	}

	[ComImport]
	[Guid("84bccd23-5fde-4cdb-aea4-af64b83d78ab")]
	[CoClass(typeof(FileSaveDialogRCW))]
	internal interface NativeFileSaveDialog : IFileSaveDialog, IFileDialog
	{
	}

	[ComImport]
	[ClassInterface(ClassInterfaceType.None)]
	[TypeLibType(TypeLibTypeFlags.FCanCreate)]
	[Guid("DC1C5A9C-E88A-4dde-A5A1-60F82A20AEF7")]
	internal class FileOpenDialogRCW
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern FileOpenDialogRCW();
	}

	[ComImport]
	[ClassInterface(ClassInterfaceType.None)]
	[TypeLibType(TypeLibTypeFlags.FCanCreate)]
	[Guid("C0B4E2F3-BA21-4773-8DBA-335EC946EB8B")]
	internal class FileSaveDialogRCW
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern FileSaveDialogRCW();
	}

	internal class IIDGuid
	{
		internal const string IModalWindow = "b4db1657-70d7-485e-8e3e-6fcb5a5c1802";

		internal const string IFileDialog = "42f85136-db7e-439c-85f1-e4075d135fc8";

		internal const string IFileOpenDialog = "d57c7288-d4ad-4768-be02-9d969532d960";

		internal const string IFileSaveDialog = "84bccd23-5fde-4cdb-aea4-af64b83d78ab";

		internal const string IFileDialogEvents = "973510DB-7D7F-452B-8975-74A85828D354";

		internal const string IShellItem = "43826D1E-E718-42EE-BC55-A1E261C37BFE";

		internal const string IShellItemArray = "B63EA76D-1F85-456F-A19C-48159EFA858B";

		private IIDGuid()
		{
		}
	}

	internal class CLSIDGuid
	{
		internal const string FileOpenDialog = "DC1C5A9C-E88A-4dde-A5A1-60F82A20AEF7";

		internal const string FileSaveDialog = "C0B4E2F3-BA21-4773-8DBA-335EC946EB8B";

		private CLSIDGuid()
		{
		}
	}

	[ComImport]
	[Guid("b4db1657-70d7-485e-8e3e-6fcb5a5c1802")]
	[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
	internal interface IModalWindow
	{
		[PreserveSig]
		int Show([In] IntPtr parent);
	}

	internal enum SIATTRIBFLAGS
	{
		SIATTRIBFLAGS_AND = 1,
		SIATTRIBFLAGS_OR,
		SIATTRIBFLAGS_APPCOMPAT
	}

	[ComImport]
	[Guid("B63EA76D-1F85-456F-A19C-48159EFA858B")]
	[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
	internal interface IShellItemArray
	{
		void BindToHandler([In] [MarshalAs(UnmanagedType.Interface)] IntPtr pbc, [In] ref Guid rbhid, [In] ref Guid riid, out IntPtr ppvOut);

		void GetPropertyStore([In] int Flags, [In] ref Guid riid, out IntPtr ppv);

		void GetPropertyDescriptionList([In] ref PROPERTYKEY keyType, [In] ref Guid riid, out IntPtr ppv);

		void GetAttributes([In] SIATTRIBFLAGS dwAttribFlags, [In] uint sfgaoMask, out uint psfgaoAttribs);

		void GetCount(out uint pdwNumItems);

		void GetItemAt([In] uint dwIndex, [MarshalAs(UnmanagedType.Interface)] out IShellItem ppsi);

		void EnumItems([MarshalAs(UnmanagedType.Interface)] out IntPtr ppenumShellItems);
	}

	[StructLayout(LayoutKind.Sequential, Pack = 4)]
	internal struct PROPERTYKEY
	{
		internal Guid fmtid;

		internal uint pid;
	}

	[ComImport]
	[Guid("42f85136-db7e-439c-85f1-e4075d135fc8")]
	[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
	internal interface IFileDialog
	{
		[PreserveSig]
		int Show([In] IntPtr parent);

		void SetFileTypes([In] uint cFileTypes, [In] [MarshalAs(UnmanagedType.LPArray)] COMDLG_FILTERSPEC[] rgFilterSpec);

		void SetFileTypeIndex([In] uint iFileType);

		void GetFileTypeIndex(out uint piFileType);

		void Advise([In] [MarshalAs(UnmanagedType.Interface)] IFileDialogEvents pfde, out uint pdwCookie);

		void Unadvise([In] uint dwCookie);

		void SetOptions([In] FOS fos);

		void GetOptions(out FOS pfos);

		void SetDefaultFolder([In] [MarshalAs(UnmanagedType.Interface)] IShellItem psi);

		void SetFolder([In] [MarshalAs(UnmanagedType.Interface)] IShellItem psi);

		void GetFolder([MarshalAs(UnmanagedType.Interface)] out IShellItem ppsi);

		void GetCurrentSelection([MarshalAs(UnmanagedType.Interface)] out IShellItem ppsi);

		void SetFileName([In] [MarshalAs(UnmanagedType.LPWStr)] string pszName);

		void GetFileName([MarshalAs(UnmanagedType.LPWStr)] out string pszName);

		void SetTitle([In] [MarshalAs(UnmanagedType.LPWStr)] string pszTitle);

		void SetOkButtonLabel([In] [MarshalAs(UnmanagedType.LPWStr)] string pszText);

		void SetFileNameLabel([In] [MarshalAs(UnmanagedType.LPWStr)] string pszLabel);

		void GetResult([MarshalAs(UnmanagedType.Interface)] out IShellItem ppsi);

		void AddPlace([In] [MarshalAs(UnmanagedType.Interface)] IShellItem psi, int alignment);

		void SetDefaultExtension([In] [MarshalAs(UnmanagedType.LPWStr)] string pszDefaultExtension);

		void Close([MarshalAs(UnmanagedType.Error)] int hr);

		void SetClientGuid([In] ref Guid guid);

		void ClearClientData();

		void SetFilter([MarshalAs(UnmanagedType.Interface)] IntPtr pFilter);
	}

	[ComImport]
	[Guid("d57c7288-d4ad-4768-be02-9d969532d960")]
	[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
	internal interface IFileOpenDialog : IFileDialog
	{
		[PreserveSig]
		new int Show([In] IntPtr parent);

		void SetFileTypes([In] uint cFileTypes, [In] ref COMDLG_FILTERSPEC rgFilterSpec);

		new void SetFileTypeIndex([In] uint iFileType);

		new void GetFileTypeIndex(out uint piFileType);

		new void Advise([In] [MarshalAs(UnmanagedType.Interface)] IFileDialogEvents pfde, out uint pdwCookie);

		new void Unadvise([In] uint dwCookie);

		new void SetOptions([In] FOS fos);

		new void GetOptions(out FOS pfos);

		new void SetDefaultFolder([In] [MarshalAs(UnmanagedType.Interface)] IShellItem psi);

		new void SetFolder([In] [MarshalAs(UnmanagedType.Interface)] IShellItem psi);

		new void GetFolder([MarshalAs(UnmanagedType.Interface)] out IShellItem ppsi);

		new void GetCurrentSelection([MarshalAs(UnmanagedType.Interface)] out IShellItem ppsi);

		new void SetFileName([In] [MarshalAs(UnmanagedType.LPWStr)] string pszName);

		new void GetFileName([MarshalAs(UnmanagedType.LPWStr)] out string pszName);

		new void SetTitle([In] [MarshalAs(UnmanagedType.LPWStr)] string pszTitle);

		new void SetOkButtonLabel([In] [MarshalAs(UnmanagedType.LPWStr)] string pszText);

		new void SetFileNameLabel([In] [MarshalAs(UnmanagedType.LPWStr)] string pszLabel);

		new void GetResult([MarshalAs(UnmanagedType.Interface)] out IShellItem ppsi);

		void AddPlace([In] [MarshalAs(UnmanagedType.Interface)] IShellItem psi, FileDialogCustomPlace fdcp);

		new void SetDefaultExtension([In] [MarshalAs(UnmanagedType.LPWStr)] string pszDefaultExtension);

		new void Close([MarshalAs(UnmanagedType.Error)] int hr);

		new void SetClientGuid([In] ref Guid guid);

		new void ClearClientData();

		new void SetFilter([MarshalAs(UnmanagedType.Interface)] IntPtr pFilter);

		void GetResults([MarshalAs(UnmanagedType.Interface)] out IShellItemArray ppenum);

		void GetSelectedItems([MarshalAs(UnmanagedType.Interface)] out IShellItemArray ppsai);
	}

	[ComImport]
	[Guid("84bccd23-5fde-4cdb-aea4-af64b83d78ab")]
	[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
	internal interface IFileSaveDialog : IFileDialog
	{
		[PreserveSig]
		new int Show([In] IntPtr parent);

		void SetFileTypes([In] uint cFileTypes, [In] ref COMDLG_FILTERSPEC rgFilterSpec);

		new void SetFileTypeIndex([In] uint iFileType);

		new void GetFileTypeIndex(out uint piFileType);

		new void Advise([In] [MarshalAs(UnmanagedType.Interface)] IFileDialogEvents pfde, out uint pdwCookie);

		new void Unadvise([In] uint dwCookie);

		new void SetOptions([In] FOS fos);

		new void GetOptions(out FOS pfos);

		new void SetDefaultFolder([In] [MarshalAs(UnmanagedType.Interface)] IShellItem psi);

		new void SetFolder([In] [MarshalAs(UnmanagedType.Interface)] IShellItem psi);

		new void GetFolder([MarshalAs(UnmanagedType.Interface)] out IShellItem ppsi);

		new void GetCurrentSelection([MarshalAs(UnmanagedType.Interface)] out IShellItem ppsi);

		new void SetFileName([In] [MarshalAs(UnmanagedType.LPWStr)] string pszName);

		new void GetFileName([MarshalAs(UnmanagedType.LPWStr)] out string pszName);

		new void SetTitle([In] [MarshalAs(UnmanagedType.LPWStr)] string pszTitle);

		new void SetOkButtonLabel([In] [MarshalAs(UnmanagedType.LPWStr)] string pszText);

		new void SetFileNameLabel([In] [MarshalAs(UnmanagedType.LPWStr)] string pszLabel);

		new void GetResult([MarshalAs(UnmanagedType.Interface)] out IShellItem ppsi);

		void AddPlace([In] [MarshalAs(UnmanagedType.Interface)] IShellItem psi, FileDialogCustomPlace fdcp);

		new void SetDefaultExtension([In] [MarshalAs(UnmanagedType.LPWStr)] string pszDefaultExtension);

		new void Close([MarshalAs(UnmanagedType.Error)] int hr);

		new void SetClientGuid([In] ref Guid guid);

		new void ClearClientData();

		new void SetFilter([MarshalAs(UnmanagedType.Interface)] IntPtr pFilter);

		void SetSaveAsItem([In] [MarshalAs(UnmanagedType.Interface)] IShellItem psi);

		void SetProperties([In] [MarshalAs(UnmanagedType.Interface)] IntPtr pStore);

		void SetCollectedProperties([In] [MarshalAs(UnmanagedType.Interface)] IntPtr pList, [In] int fAppendDefault);

		void GetProperties([MarshalAs(UnmanagedType.Interface)] out IntPtr ppStore);

		void ApplyProperties([In] [MarshalAs(UnmanagedType.Interface)] IShellItem psi, [In] [MarshalAs(UnmanagedType.Interface)] IntPtr pStore, [In] [ComAliasName("ShellObjects.wireHWND")] ref IntPtr hwnd, [In] [MarshalAs(UnmanagedType.Interface)] IntPtr pSink);
	}

	[ComImport]
	[Guid("973510DB-7D7F-452B-8975-74A85828D354")]
	[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
	internal interface IFileDialogEvents
	{
		[PreserveSig]
		int OnFileOk([In] [MarshalAs(UnmanagedType.Interface)] IFileDialog pfd);

		[PreserveSig]
		int OnFolderChanging([In] [MarshalAs(UnmanagedType.Interface)] IFileDialog pfd, [In] [MarshalAs(UnmanagedType.Interface)] IShellItem psiFolder);

		void OnFolderChange([In] [MarshalAs(UnmanagedType.Interface)] IFileDialog pfd);

		void OnSelectionChange([In] [MarshalAs(UnmanagedType.Interface)] IFileDialog pfd);

		void OnShareViolation([In] [MarshalAs(UnmanagedType.Interface)] IFileDialog pfd, [In] [MarshalAs(UnmanagedType.Interface)] IShellItem psi, out FDE_SHAREVIOLATION_RESPONSE pResponse);

		void OnTypeChange([In] [MarshalAs(UnmanagedType.Interface)] IFileDialog pfd);

		void OnOverwrite([In] [MarshalAs(UnmanagedType.Interface)] IFileDialog pfd, [In] [MarshalAs(UnmanagedType.Interface)] IShellItem psi, out FDE_OVERWRITE_RESPONSE pResponse);
	}

	[ComImport]
	[Guid("43826D1E-E718-42EE-BC55-A1E261C37BFE")]
	[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
	internal interface IShellItem
	{
		void BindToHandler([In] [MarshalAs(UnmanagedType.Interface)] IntPtr pbc, [In] ref Guid bhid, [In] ref Guid riid, out IntPtr ppv);

		void GetParent([MarshalAs(UnmanagedType.Interface)] out IShellItem ppsi);

		void GetDisplayName([In] SIGDN sigdnName, [MarshalAs(UnmanagedType.LPWStr)] out string ppszName);

		void GetAttributes([In] uint sfgaoMask, out uint psfgaoAttribs);

		void Compare([In] [MarshalAs(UnmanagedType.Interface)] IShellItem psi, [In] uint hint, out int piOrder);
	}

	internal enum SIGDN : uint
	{
		SIGDN_NORMALDISPLAY = 0u,
		SIGDN_PARENTRELATIVEPARSING = 2147581953u,
		SIGDN_DESKTOPABSOLUTEPARSING = 2147647488u,
		SIGDN_PARENTRELATIVEEDITING = 2147684353u,
		SIGDN_DESKTOPABSOLUTEEDITING = 2147794944u,
		SIGDN_FILESYSPATH = 2147844096u,
		SIGDN_URL = 2147909632u,
		SIGDN_PARENTRELATIVEFORADDRESSBAR = 2147991553u,
		SIGDN_PARENTRELATIVE = 2148007937u
	}

	[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto, Pack = 4)]
	internal struct COMDLG_FILTERSPEC
	{
		[MarshalAs(UnmanagedType.LPWStr)]
		internal string pszName;

		[MarshalAs(UnmanagedType.LPWStr)]
		internal string pszSpec;
	}

	[Flags]
	internal enum FOS : uint
	{
		FOS_OVERWRITEPROMPT = 0x2u,
		FOS_STRICTFILETYPES = 0x4u,
		FOS_NOCHANGEDIR = 0x8u,
		FOS_PICKFOLDERS = 0x20u,
		FOS_FORCEFILESYSTEM = 0x40u,
		FOS_ALLNONSTORAGEITEMS = 0x80u,
		FOS_NOVALIDATE = 0x100u,
		FOS_ALLOWMULTISELECT = 0x200u,
		FOS_PATHMUSTEXIST = 0x800u,
		FOS_FILEMUSTEXIST = 0x1000u,
		FOS_CREATEPROMPT = 0x2000u,
		FOS_SHAREAWARE = 0x4000u,
		FOS_NOREADONLYRETURN = 0x8000u,
		FOS_NOTESTFILECREATE = 0x10000u,
		FOS_HIDEMRUPLACES = 0x20000u,
		FOS_HIDEPINNEDPLACES = 0x40000u,
		FOS_NODEREFERENCELINKS = 0x100000u,
		FOS_DONTADDTORECENT = 0x2000000u,
		FOS_FORCESHOWHIDDEN = 0x10000000u,
		FOS_DEFAULTNOMINIMODE = 0x20000000u
	}

	internal enum FDE_SHAREVIOLATION_RESPONSE
	{
		FDESVR_DEFAULT,
		FDESVR_ACCEPT,
		FDESVR_REFUSE
	}

	internal enum FDE_OVERWRITE_RESPONSE
	{
		FDEOR_DEFAULT,
		FDEOR_ACCEPT,
		FDEOR_REFUSE
	}
}
