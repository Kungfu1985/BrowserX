// System.Windows.Forms.Application
using Microsoft.Win32;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Deployment.Application;
using System.Deployment.Internal.Isolation;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Runtime.ConstrainedExecution;
using System.Runtime.InteropServices;
using System.Security;
using System.Security.Permissions;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using System.Windows.Forms.Layout;
using System.Windows.Forms.VisualStyles;

public sealed class Application
{
	[EditorBrowsable(EditorBrowsableState.Advanced)]
	public delegate bool MessageLoopCallback();

	private class ClickOnceUtility
	{
		public enum HostType
		{
			Default,
			AppLaunch,
			CorFlag
		}

		private static System.Deployment.Internal.Isolation.StoreApplicationReference InstallReference => new System.Deployment.Internal.Isolation.StoreApplicationReference(System.Deployment.Internal.Isolation.IsolationInterop.GUID_SXS_INSTALL_REFERENCE_SCHEME_OPAQUESTRING, "{3f471841-eef2-47d6-89c0-d028f03a4ad5}", null);

		private ClickOnceUtility()
		{
		}

		public static HostType GetHostTypeFromMetaData(string appFullName)
		{
			HostType result = HostType.Default;
			try
			{
				IDefinitionAppId appId = System.Deployment.Internal.Isolation.IsolationInterop.AppIdAuthority.TextToDefinition(0u, appFullName);
				result = ((!GetPropertyBoolean(appId, "IsFullTrust")) ? HostType.AppLaunch : HostType.CorFlag);
				return result;
			}
			catch
			{
				return result;
			}
		}

		private static bool GetPropertyBoolean(System.Deployment.Internal.Isolation.IDefinitionAppId appId, string propName)
		{
			string propertyString = GetPropertyString(appId, propName);
			if (string.IsNullOrEmpty(propertyString))
			{
				return false;
			}
			try
			{
				return Convert.ToBoolean(propertyString, CultureInfo.InvariantCulture);
			}
			catch
			{
				return false;
			}
		}

		private static string GetPropertyString(System.Deployment.Internal.Isolation.IDefinitionAppId appId, string propName)
		{
			byte[] deploymentProperty = System.Deployment.Internal.Isolation.IsolationInterop.UserStore.GetDeploymentProperty(System.Deployment.Internal.Isolation.Store.GetPackagePropertyFlags.Nothing, appId, InstallReference, new Guid("2ad613da-6fdb-4671-af9e-18ab2e4df4d8"), propName);
			int num = deploymentProperty.Length;
			if (num == 0 || deploymentProperty.Length % 2 != 0 || deploymentProperty[num - 2] != 0 || deploymentProperty[num - 1] != 0)
			{
				return null;
			}
			return Encoding.Unicode.GetString(deploymentProperty, 0, num - 2);
		}
	}

	private class ComponentManager : UnsafeNativeMethods.IMsoComponentManager
	{
		private class ComponentHashtableEntry
		{
			public UnsafeNativeMethods.IMsoComponent component;

			public NativeMethods.MSOCRINFOSTRUCT componentInfo;
		}

		private Hashtable oleComponents;

		private int cookieCounter;

		private UnsafeNativeMethods.IMsoComponent activeComponent;

		private UnsafeNativeMethods.IMsoComponent trackingComponent;

		private int currentState;

		private Hashtable OleComponents
		{
			get
			{
				if (oleComponents == null)
				{
					oleComponents = new Hashtable();
					cookieCounter = 0;
				}
				return oleComponents;
			}
		}

		int UnsafeNativeMethods.IMsoComponentManager.QueryService(ref Guid guidService, ref Guid iid, out object ppvObj)
		{
			ppvObj = null;
			return -2147467262;
		}

		bool UnsafeNativeMethods.IMsoComponentManager.FDebugMessage(IntPtr hInst, int msg, IntPtr wparam, IntPtr lparam)
		{
			return true;
		}

		bool UnsafeNativeMethods.IMsoComponentManager.FRegisterComponent(UnsafeNativeMethods.IMsoComponent component, NativeMethods.MSOCRINFOSTRUCT pcrinfo, out IntPtr dwComponentID)
		{
			ComponentHashtableEntry componentHashtableEntry = new ComponentHashtableEntry();
			componentHashtableEntry.component = component;
			componentHashtableEntry.componentInfo = pcrinfo;
			OleComponents.Add(++cookieCounter, componentHashtableEntry);
			dwComponentID = (IntPtr)cookieCounter;
			return true;
		}

		bool UnsafeNativeMethods.IMsoComponentManager.FRevokeComponent(IntPtr dwComponentID)
		{
			int num = (int)(long)dwComponentID;
			ComponentHashtableEntry componentHashtableEntry = (ComponentHashtableEntry)OleComponents[num];
			if (componentHashtableEntry == null)
			{
				return false;
			}
			if (componentHashtableEntry.component == activeComponent)
			{
				activeComponent = null;
			}
			if (componentHashtableEntry.component == trackingComponent)
			{
				trackingComponent = null;
			}
			OleComponents.Remove(num);
			return true;
		}

		bool UnsafeNativeMethods.IMsoComponentManager.FUpdateComponentRegistration(IntPtr dwComponentID, NativeMethods.MSOCRINFOSTRUCT info)
		{
			int num = (int)(long)dwComponentID;
			ComponentHashtableEntry componentHashtableEntry = (ComponentHashtableEntry)OleComponents[num];
			if (componentHashtableEntry == null)
			{
				return false;
			}
			componentHashtableEntry.componentInfo = info;
			return true;
		}

		bool UnsafeNativeMethods.IMsoComponentManager.FOnComponentActivate(IntPtr dwComponentID)
		{
			int num = (int)(long)dwComponentID;
			ComponentHashtableEntry componentHashtableEntry = (ComponentHashtableEntry)OleComponents[num];
			if (componentHashtableEntry == null)
			{
				return false;
			}
			activeComponent = componentHashtableEntry.component;
			return true;
		}

		bool UnsafeNativeMethods.IMsoComponentManager.FSetTrackingComponent(IntPtr dwComponentID, bool fTrack)
		{
			int num = (int)(long)dwComponentID;
			ComponentHashtableEntry componentHashtableEntry = (ComponentHashtableEntry)OleComponents[num];
			if (componentHashtableEntry == null)
			{
				return false;
			}
			if ((componentHashtableEntry.component == trackingComponent) ^ fTrack)
			{
				return false;
			}
			if (fTrack)
			{
				trackingComponent = componentHashtableEntry.component;
			}
			else
			{
				trackingComponent = null;
			}
			return true;
		}

		void UnsafeNativeMethods.IMsoComponentManager.OnComponentEnterState(IntPtr dwComponentID, int uStateID, int uContext, int cpicmExclude, int rgpicmExclude, int dwReserved)
		{
			int num = (int)(long)dwComponentID;
			currentState |= uStateID;
			if (uContext == 0 || uContext == 1)
			{
				foreach (ComponentHashtableEntry value in OleComponents.Values)
				{
					value.component.OnEnterState(uStateID, fEnter: true);
				}
			}
		}

		bool UnsafeNativeMethods.IMsoComponentManager.FOnComponentExitState(IntPtr dwComponentID, int uStateID, int uContext, int cpicmExclude, int rgpicmExclude)
		{
			int num = (int)(long)dwComponentID;
			currentState &= ~uStateID;
			if (uContext == 0 || uContext == 1)
			{
				foreach (ComponentHashtableEntry value in OleComponents.Values)
				{
					value.component.OnEnterState(uStateID, fEnter: false);
				}
			}
			return false;
		}

		bool UnsafeNativeMethods.IMsoComponentManager.FInState(int uStateID, IntPtr pvoid)
		{
			return (currentState & uStateID) != 0;
		}

		bool UnsafeNativeMethods.IMsoComponentManager.FContinueIdle()
		{
			NativeMethods.MSG msg = default(NativeMethods.MSG);
			return !UnsafeNativeMethods.PeekMessage(ref msg, NativeMethods.NullHandleRef, 0, 0, 0);
		}

		bool UnsafeNativeMethods.IMsoComponentManager.FPushMessageLoop(IntPtr dwComponentID, int reason, int pvLoopData)
		{
			int num = (int)(long)dwComponentID;
			int num2 = currentState;
			bool flag = true;
			if (!OleComponents.ContainsKey(num))
			{
				return false;
			}
			UnsafeNativeMethods.IMsoComponent msoComponent = activeComponent;
			try
			{
				NativeMethods.MSG msg = default(NativeMethods.MSG);
				NativeMethods.MSG[] array = new NativeMethods.MSG[1]
				{
					msg
				};
				bool flag2 = false;
				ComponentHashtableEntry componentHashtableEntry = (ComponentHashtableEntry)OleComponents[num];
				if (componentHashtableEntry == null)
				{
					return false;
				}
				UnsafeNativeMethods.IMsoComponent msoComponent2 = activeComponent = componentHashtableEntry.component;
				while (flag)
				{
					UnsafeNativeMethods.IMsoComponent msoComponent3 = (trackingComponent != null) ? trackingComponent : ((activeComponent == null) ? msoComponent2 : activeComponent);
					if (UnsafeNativeMethods.PeekMessage(ref msg, NativeMethods.NullHandleRef, 0, 0, 0))
					{
						array[0] = msg;
						flag = msoComponent3.FContinueMessageLoop(reason, pvLoopData, array);
						if (flag)
						{
							if (msg.hwnd != IntPtr.Zero && SafeNativeMethods.IsWindowUnicode(new HandleRef(null, msg.hwnd)))
							{
								flag2 = true;
								UnsafeNativeMethods.GetMessageW(ref msg, NativeMethods.NullHandleRef, 0, 0);
							}
							else
							{
								flag2 = false;
								UnsafeNativeMethods.GetMessageA(ref msg, NativeMethods.NullHandleRef, 0, 0);
							}
							if (msg.message == 18)
							{
								ThreadContext.FromCurrent().DisposeThreadWindows();
								if (reason != -1)
								{
									UnsafeNativeMethods.PostQuitMessage((int)msg.wParam);
								}
								flag = false;
								break;
							}
							if (!msoComponent3.FPreTranslateMessage(ref msg))
							{
								UnsafeNativeMethods.TranslateMessage(ref msg);
								if (flag2)
								{
									UnsafeNativeMethods.DispatchMessageW(ref msg);
								}
								else
								{
									UnsafeNativeMethods.DispatchMessageA(ref msg);
								}
							}
						}
					}
					else
					{
						if (reason == 2 || reason == -2)
						{
							break;
						}
						bool flag3 = false;
						if (OleComponents != null)
						{
							IEnumerator enumerator = OleComponents.Values.GetEnumerator();
							while (enumerator.MoveNext())
							{
								ComponentHashtableEntry componentHashtableEntry2 = (ComponentHashtableEntry)enumerator.Current;
								flag3 |= componentHashtableEntry2.component.FDoIdle(-1);
							}
						}
						flag = msoComponent3.FContinueMessageLoop(reason, pvLoopData, null);
						if (flag)
						{
							if (flag3)
							{
								UnsafeNativeMethods.MsgWaitForMultipleObjectsEx(0, IntPtr.Zero, 100, 255, 4);
							}
							else if (!UnsafeNativeMethods.PeekMessage(ref msg, NativeMethods.NullHandleRef, 0, 0, 0))
							{
								UnsafeNativeMethods.WaitMessage();
							}
						}
					}
				}
			}
			finally
			{
				currentState = num2;
				activeComponent = msoComponent;
			}
			return !flag;
		}

		bool UnsafeNativeMethods.IMsoComponentManager.FCreateSubComponentManager(object punkOuter, object punkServProv, ref Guid riid, out IntPtr ppvObj)
		{
			ppvObj = IntPtr.Zero;
			return false;
		}

		bool UnsafeNativeMethods.IMsoComponentManager.FGetParentComponentManager(out UnsafeNativeMethods.IMsoComponentManager ppicm)
		{
			ppicm = null;
			return false;
		}

		bool UnsafeNativeMethods.IMsoComponentManager.FGetActiveComponent(int dwgac, UnsafeNativeMethods.IMsoComponent[] ppic, NativeMethods.MSOCRINFOSTRUCT info, int dwReserved)
		{
			UnsafeNativeMethods.IMsoComponent msoComponent = null;
			switch (dwgac)
			{
			case 0:
				msoComponent = activeComponent;
				break;
			case 1:
				msoComponent = trackingComponent;
				break;
			case 2:
				msoComponent = ((trackingComponent == null) ? activeComponent : trackingComponent);
				break;
			}
			if (ppic != null)
			{
				ppic[0] = msoComponent;
			}
			if (info != null && msoComponent != null)
			{
				foreach (ComponentHashtableEntry value in OleComponents.Values)
				{
					if (value.component == msoComponent)
					{
						info = value.componentInfo;
						break;
					}
				}
			}
			return msoComponent != null;
		}
	}

	internal sealed class ThreadContext : MarshalByRefObject, UnsafeNativeMethods.IMsoComponent
	{
		private const int STATE_OLEINITIALIZED = 1;

		private const int STATE_EXTERNALOLEINIT = 2;

		private const int STATE_INTHREADEXCEPTION = 4;

		private const int STATE_POSTEDQUIT = 8;

		private const int STATE_FILTERSNAPSHOTVALID = 16;

		private const int STATE_TRACKINGCOMPONENT = 32;

		private const int INVALID_ID = -1;

		private static Hashtable contextHash = new Hashtable();

		private static object tcInternalSyncObject = new object();

		private static int totalMessageLoopCount;

		private static int baseLoopReason;

		[ThreadStatic]
		private static ThreadContext currentThreadContext;

		internal ThreadExceptionEventHandler threadExceptionHandler;

		internal EventHandler idleHandler;

		internal EventHandler enterModalHandler;

		internal EventHandler leaveModalHandler;

		private ApplicationContext applicationContext;

		private List<ParkingWindow> parkingWindows = new List<ParkingWindow>();

		private Control marshalingControl;

		private CultureInfo culture;

		private ArrayList messageFilters;

		private IMessageFilter[] messageFilterSnapshot;

		private int inProcessFilters;

		private IntPtr handle;

		private int id;

		private int messageLoopCount;

		private int threadState;

		private int modalCount;

		private WeakReference activatingControlRef;

		private UnsafeNativeMethods.IMsoComponentManager componentManager;

		private bool externalComponentManager;

		private bool fetchingComponentManager;

		private int componentID = -1;

		private Form currentForm;

		private ThreadWindows threadWindows;

		private NativeMethods.MSG tempMsg;

		private int disposeCount;

		private bool ourModalLoop;

		private MessageLoopCallback messageLoopCallback;

		public ApplicationContext ApplicationContext => applicationContext;

		internal UnsafeNativeMethods.IMsoComponentManager ComponentManager
		{
			get
			{
				if (componentManager == null)
				{
					if (fetchingComponentManager)
					{
						return null;
					}
					fetchingComponentManager = true;
					try
					{
						UnsafeNativeMethods.IMsoComponentManager msoComponentManager = null;
						Application.OleRequired();
						IntPtr oldMsgFilter = (IntPtr)0;
						if (NativeMethods.Succeeded(UnsafeNativeMethods.CoRegisterMessageFilter(NativeMethods.NullHandleRef, ref oldMsgFilter)) && oldMsgFilter != (IntPtr)0)
						{
							IntPtr oldMsgFilter2 = (IntPtr)0;
							UnsafeNativeMethods.CoRegisterMessageFilter(new HandleRef(null, oldMsgFilter), ref oldMsgFilter2);
							object obj = Marshal.GetObjectForIUnknown(oldMsgFilter);
							Marshal.Release(oldMsgFilter);
							UnsafeNativeMethods.IOleServiceProvider oleServiceProvider = obj as UnsafeNativeMethods.IOleServiceProvider;
							if (oleServiceProvider != null)
							{
								try
								{
									IntPtr ppvObject = IntPtr.Zero;
									Guid guidService = new Guid("000C060B-0000-0000-C000-000000000046");
									Guid riid = new Guid("{000C0601-0000-0000-C000-000000000046}");
									int hr = oleServiceProvider.QueryService(ref guidService, ref riid, out ppvObject);
									if (NativeMethods.Succeeded(hr) && ppvObject != IntPtr.Zero)
									{
										IntPtr ppv;
										try
										{
											Guid iid = typeof(UnsafeNativeMethods.IMsoComponentManager).GUID;
											hr = Marshal.QueryInterface(ppvObject, ref iid, out ppv);
										}
										finally
										{
											Marshal.Release(ppvObject);
										}
										if (NativeMethods.Succeeded(hr) && ppv != IntPtr.Zero)
										{
											try
											{
												msoComponentManager = ComponentManagerBroker.GetComponentManager(ppv);
											}
											finally
											{
												Marshal.Release(ppv);
											}
										}
										if (msoComponentManager != null)
										{
											if (oldMsgFilter == ppvObject)
											{
												obj = null;
											}
											externalComponentManager = true;
											AppDomain.CurrentDomain.DomainUnload += OnDomainUnload;
											AppDomain.CurrentDomain.ProcessExit += OnDomainUnload;
										}
									}
								}
								catch
								{
								}
							}
							if (obj != null && Marshal.IsComObject(obj))
							{
								Marshal.ReleaseComObject(obj);
							}
						}
						if (msoComponentManager == null)
						{
							msoComponentManager = new ComponentManager();
							externalComponentManager = false;
						}
						if (msoComponentManager != null && componentID == -1)
						{
							NativeMethods.MSOCRINFOSTRUCT mSOCRINFOSTRUCT = new NativeMethods.MSOCRINFOSTRUCT();
							mSOCRINFOSTRUCT.cbSize = Marshal.SizeOf(typeof(NativeMethods.MSOCRINFOSTRUCT));
							mSOCRINFOSTRUCT.uIdleTimeInterval = 0;
							mSOCRINFOSTRUCT.grfcrf = 9;
							mSOCRINFOSTRUCT.grfcadvf = 1;
							IntPtr dwComponentID;
							bool flag = msoComponentManager.FRegisterComponent(this, mSOCRINFOSTRUCT, out dwComponentID);
							componentID = (int)(long)dwComponentID;
							if (flag && !(msoComponentManager is ComponentManager))
							{
								messageLoopCount++;
							}
							componentManager = msoComponentManager;
						}
					}
					finally
					{
						fetchingComponentManager = false;
					}
				}
				return componentManager;
			}
		}

		internal bool CustomThreadExceptionHandlerAttached => threadExceptionHandler != null;

		internal Control ActivatingControl
		{
			get
			{
				if (activatingControlRef != null && activatingControlRef.IsAlive)
				{
					return activatingControlRef.Target as Control;
				}
				return null;
			}
			set
			{
				if (value != null)
				{
					activatingControlRef = new WeakReference(value);
				}
				else
				{
					activatingControlRef = null;
				}
			}
		}

		internal Control MarshalingControl
		{
			get
			{
				lock (this)
				{
					if (marshalingControl == null)
					{
						marshalingControl = new MarshalingControl();
					}
					return marshalingControl;
				}
			}
		}

		public ThreadContext()
		{
			IntPtr handleTarget = IntPtr.Zero;
			UnsafeNativeMethods.DuplicateHandle(new HandleRef(null, SafeNativeMethods.GetCurrentProcess()), new HandleRef(null, SafeNativeMethods.GetCurrentThread()), new HandleRef(null, SafeNativeMethods.GetCurrentProcess()), ref handleTarget, 0, inheritHandle: false, 2);
			handle = handleTarget;
			id = SafeNativeMethods.GetCurrentThreadId();
			messageLoopCount = 0;
			currentThreadContext = this;
			contextHash[id] = this;
		}

		internal ParkingWindow GetParkingWindow(System.Windows.Forms.DpiAwarenessContext context)
		{
			lock (this)
			{
				ParkingWindow parkingWindow = GetParkingWindowForContext(context);
				if (parkingWindow == null)
				{
					IntSecurity.ManipulateWndProcAndHandles.Assert();
					try
					{
						using (System.Windows.Forms.DpiHelper.EnterDpiAwarenessScope(context))
						{
							parkingWindow = new ParkingWindow();
						}
						parkingWindows.Add(parkingWindow);
					}
					finally
					{
						CodeAccessPermission.RevertAssert();
					}
				}
				return parkingWindow;
			}
		}

		internal ParkingWindow GetParkingWindowForContext(System.Windows.Forms.DpiAwarenessContext context)
		{
			if (parkingWindows.Count == 0)
			{
				return null;
			}
			if (!System.Windows.Forms.DpiHelper.EnableDpiChangedHighDpiImprovements || CommonUnsafeNativeMethods.TryFindDpiAwarenessContextsEqual(context, System.Windows.Forms.DpiAwarenessContext.DPI_AWARENESS_CONTEXT_UNSPECIFIED))
			{
				return parkingWindows[0];
			}
			foreach (ParkingWindow parkingWindow in parkingWindows)
			{
				if (CommonUnsafeNativeMethods.TryFindDpiAwarenessContextsEqual(parkingWindow.DpiAwarenessContext, context))
				{
					return parkingWindow;
				}
			}
			return null;
		}

		internal void AddMessageFilter(IMessageFilter f)
		{
			if (messageFilters == null)
			{
				messageFilters = new ArrayList();
			}
			if (f != null)
			{
				SetState(16, value: false);
				if (messageFilters.Count > 0 && f is IMessageModifyAndFilter)
				{
					messageFilters.Insert(0, f);
				}
				else
				{
					messageFilters.Add(f);
				}
			}
		}

		internal void BeginModalMessageLoop(ApplicationContext context)
		{
			bool flag = ourModalLoop;
			ourModalLoop = true;
			try
			{
				ComponentManager?.OnComponentEnterState((IntPtr)componentID, 1, 0, 0, 0, 0);
			}
			finally
			{
				ourModalLoop = flag;
			}
			DisableWindowsForModalLoop(onlyWinForms: false, context);
			modalCount++;
			if (enterModalHandler != null && modalCount == 1)
			{
				enterModalHandler(Thread.CurrentThread, EventArgs.Empty);
			}
		}

		internal void DisableWindowsForModalLoop(bool onlyWinForms, ApplicationContext context)
		{
			ThreadWindows previousThreadWindows = threadWindows;
			threadWindows = new ThreadWindows(onlyWinForms);
			threadWindows.Enable(state: false);
			threadWindows.previousThreadWindows = previousThreadWindows;
			(context as ModalApplicationContext)?.DisableThreadWindows(disable: true, onlyWinForms);
		}

		internal void Dispose(bool postQuit)
		{
			lock (this)
			{
				try
				{
					if (disposeCount++ == 0)
					{
						if (messageLoopCount > 0 && postQuit)
						{
							PostQuit();
						}
						else
						{
							bool flag = SafeNativeMethods.GetCurrentThreadId() == id;
							try
							{
								if (flag)
								{
									if (componentManager != null)
									{
										RevokeComponent();
									}
									DisposeThreadWindows();
									try
									{
										RaiseThreadExit();
									}
									finally
									{
										if (GetState(1) && !GetState(2))
										{
											SetState(1, value: false);
											UnsafeNativeMethods.OleUninitialize();
										}
									}
								}
							}
							finally
							{
								if (handle != IntPtr.Zero)
								{
									UnsafeNativeMethods.CloseHandle(new HandleRef(this, handle));
									handle = IntPtr.Zero;
								}
								try
								{
									if (totalMessageLoopCount == 0)
									{
										RaiseExit();
									}
								}
								finally
								{
									lock (tcInternalSyncObject)
									{
										contextHash.Remove(id);
									}
									if (currentThreadContext == this)
									{
										currentThreadContext = null;
									}
								}
							}
						}
						GC.SuppressFinalize(this);
					}
				}
				finally
				{
					disposeCount--;
				}
			}
		}

		private void DisposeParkingWindow()
		{
			if (parkingWindows.Count == 0)
			{
				return;
			}
			int lpdwProcessId;
			int windowThreadProcessId = SafeNativeMethods.GetWindowThreadProcessId(new HandleRef(parkingWindows[0], parkingWindows[0].Handle), out lpdwProcessId);
			int currentThreadId = SafeNativeMethods.GetCurrentThreadId();
			for (int i = 0; i < parkingWindows.Count; i++)
			{
				if (windowThreadProcessId == currentThreadId)
				{
					parkingWindows[i].Destroy();
				}
				else
				{
					parkingWindows[i] = null;
				}
			}
			parkingWindows.Clear();
		}

		internal void DisposeThreadWindows()
		{
			try
			{
				if (applicationContext != null)
				{
					applicationContext.Dispose();
					applicationContext = null;
				}
				ThreadWindows threadWindows = new ThreadWindows(onlyWinForms: true);
				threadWindows.Dispose();
				DisposeParkingWindow();
			}
			catch
			{
			}
		}

		internal void EnableWindowsForModalLoop(bool onlyWinForms, ApplicationContext context)
		{
			if (threadWindows != null)
			{
				threadWindows.Enable(state: true);
				threadWindows = threadWindows.previousThreadWindows;
			}
			(context as ModalApplicationContext)?.DisableThreadWindows(disable: false, onlyWinForms);
		}

		internal void EndModalMessageLoop(ApplicationContext context)
		{
			EnableWindowsForModalLoop(onlyWinForms: false, context);
			bool flag = ourModalLoop;
			ourModalLoop = true;
			try
			{
				ComponentManager?.FOnComponentExitState((IntPtr)componentID, 1, 0, 0, 0);
			}
			finally
			{
				ourModalLoop = flag;
			}
			modalCount--;
			if (leaveModalHandler != null && modalCount == 0)
			{
				leaveModalHandler(Thread.CurrentThread, EventArgs.Empty);
			}
		}

		internal static void ExitApplication()
		{
			ExitCommon(disposing: true);
		}

		private static void ExitCommon(bool disposing)
		{
			lock (tcInternalSyncObject)
			{
				if (contextHash != null)
				{
					ThreadContext[] array = new ThreadContext[contextHash.Values.Count];
					contextHash.Values.CopyTo(array, 0);
					for (int i = 0; i < array.Length; i++)
					{
						if (array[i].ApplicationContext != null)
						{
							array[i].ApplicationContext.ExitThread();
						}
						else
						{
							array[i].Dispose(disposing);
						}
					}
				}
			}
		}

		internal static void ExitDomain()
		{
			ExitCommon(disposing: false);
		}

		~ThreadContext()
		{
			if (handle != IntPtr.Zero)
			{
				UnsafeNativeMethods.CloseHandle(new HandleRef(this, handle));
				handle = IntPtr.Zero;
			}
		}

		internal void FormActivated(bool activate)
		{
			if (activate)
			{
				UnsafeNativeMethods.IMsoComponentManager msoComponentManager = ComponentManager;
				if (msoComponentManager != null && !(msoComponentManager is ComponentManager))
				{
					msoComponentManager.FOnComponentActivate((IntPtr)componentID);
				}
			}
		}

		internal void TrackInput(bool track)
		{
			if (track != GetState(32))
			{
				UnsafeNativeMethods.IMsoComponentManager msoComponentManager = ComponentManager;
				if (msoComponentManager != null && !(msoComponentManager is ComponentManager))
				{
					msoComponentManager.FSetTrackingComponent((IntPtr)componentID, track);
					SetState(32, track);
				}
			}
		}

		internal static ThreadContext FromCurrent()
		{
			ThreadContext threadContext = currentThreadContext;
			if (threadContext == null)
			{
				threadContext = new ThreadContext();
			}
			return threadContext;
		}

		internal static ThreadContext FromId(int id)
		{
			ThreadContext threadContext = (ThreadContext)contextHash[id];
			if (threadContext == null && id == SafeNativeMethods.GetCurrentThreadId())
			{
				threadContext = new ThreadContext();
			}
			return threadContext;
		}

		internal bool GetAllowQuit()
		{
			if (totalMessageLoopCount > 0)
			{
				return baseLoopReason == -1;
			}
			return false;
		}

		internal IntPtr GetHandle()
		{
			return handle;
		}

		internal int GetId()
		{
			return id;
		}

		internal CultureInfo GetCulture()
		{
			if (culture == null || culture.LCID != SafeNativeMethods.GetThreadLocale())
			{
				culture = new CultureInfo(SafeNativeMethods.GetThreadLocale());
			}
			return culture;
		}

		internal bool GetMessageLoop()
		{
			return GetMessageLoop(mustBeActive: false);
		}

		internal bool GetMessageLoop(bool mustBeActive)
		{
			if (messageLoopCount > ((mustBeActive && externalComponentManager) ? 1 : 0))
			{
				return true;
			}
			if (ComponentManager != null && externalComponentManager)
			{
				if (!mustBeActive)
				{
					return true;
				}
				UnsafeNativeMethods.IMsoComponent[] array = new UnsafeNativeMethods.IMsoComponent[1];
				if (ComponentManager.FGetActiveComponent(0, array, null, 0) && array[0] == this)
				{
					return true;
				}
			}
			return messageLoopCallback?.Invoke() ?? false;
		}

		private bool GetState(int bit)
		{
			return (threadState & bit) != 0;
		}

		public override object InitializeLifetimeService()
		{
			return null;
		}

		internal bool IsValidComponentId()
		{
			return componentID != -1;
		}

		internal ApartmentState OleRequired()
		{
			Thread currentThread = Thread.CurrentThread;
			if (!GetState(1))
			{
				int num = UnsafeNativeMethods.OleInitialize();
				SetState(1, value: true);
				if (num == -2147417850)
				{
					SetState(2, value: true);
				}
			}
			if (GetState(2))
			{
				return ApartmentState.MTA;
			}
			return ApartmentState.STA;
		}

		private void OnAppThreadExit(object sender, EventArgs e)
		{
			Dispose(postQuit: true);
		}

		[PrePrepareMethod]
		private void OnDomainUnload(object sender, EventArgs e)
		{
			RevokeComponent();
			ExitDomain();
		}

		internal void OnThreadException(Exception t)
		{
			if (!GetState(4))
			{
				SetState(4, value: true);
				try
				{
					if (threadExceptionHandler != null)
					{
						threadExceptionHandler(Thread.CurrentThread, new ThreadExceptionEventArgs(t));
					}
					else if (SystemInformation.UserInteractive)
					{
						ThreadExceptionDialog threadExceptionDialog = new ThreadExceptionDialog(t);
						DialogResult dialogResult = DialogResult.OK;
						IntSecurity.ModifyFocus.Assert();
						try
						{
							dialogResult = threadExceptionDialog.ShowDialog();
						}
						finally
						{
							CodeAccessPermission.RevertAssert();
							threadExceptionDialog.Dispose();
						}
						switch (dialogResult)
						{
						case DialogResult.Abort:
							ExitInternal();
							new SecurityPermission(SecurityPermissionFlag.UnmanagedCode).Assert();
							Environment.Exit(0);
							break;
						case DialogResult.Yes:
						{
							WarningException ex = t as WarningException;
							if (ex != null)
							{
								Help.ShowHelp(null, ex.HelpUrl, ex.HelpTopic);
							}
							break;
						}
						}
					}
				}
				finally
				{
					SetState(4, value: false);
				}
			}
		}

		internal void PostQuit()
		{
			UnsafeNativeMethods.PostThreadMessage(id, 18, IntPtr.Zero, IntPtr.Zero);
			SetState(8, value: true);
		}

		internal void RegisterMessageLoop(MessageLoopCallback callback)
		{
			messageLoopCallback = callback;
		}

		internal void RemoveMessageFilter(IMessageFilter f)
		{
			if (messageFilters != null)
			{
				SetState(16, value: false);
				messageFilters.Remove(f);
			}
		}

		internal void RunMessageLoop(int reason, ApplicationContext context)
		{
			IntPtr userCookie = IntPtr.Zero;
			if (useVisualStyles)
			{
				userCookie = UnsafeNativeMethods.ThemingScope.Activate();
			}
			try
			{
				RunMessageLoopInner(reason, context);
			}
			finally
			{
				UnsafeNativeMethods.ThemingScope.Deactivate(userCookie);
			}
		}

		private void RunMessageLoopInner(int reason, ApplicationContext context)
		{
			if (reason == 4 && !SystemInformation.UserInteractive)
			{
				throw new InvalidOperationException(SR.GetString("CantShowModalOnNonInteractive"));
			}
			if (reason == -1)
			{
				SetState(8, value: false);
			}
			if (totalMessageLoopCount++ == 0)
			{
				baseLoopReason = reason;
			}
			messageLoopCount++;
			if (reason == -1)
			{
				if (messageLoopCount != 1)
				{
					throw new InvalidOperationException(SR.GetString("CantNestMessageLoops"));
				}
				applicationContext = context;
				applicationContext.ThreadExit += OnAppThreadExit;
				if (applicationContext.MainForm != null)
				{
					applicationContext.MainForm.Visible = true;
				}
				System.Windows.Forms.DpiHelper.InitializeDpiHelperForWinforms();
				System.AccessibilityImprovements.ValidateLevels();
			}
			Form form = currentForm;
			if (context != null)
			{
				currentForm = context.MainForm;
			}
			bool flag = false;
			bool flag2 = false;
			HandleRef hWnd = new HandleRef(null, IntPtr.Zero);
			if (reason == -2)
			{
				flag2 = true;
			}
			if (reason == 4 || reason == 5)
			{
				flag = true;
				bool flag3 = currentForm != null && currentForm.Enabled;
				BeginModalMessageLoop(context);
				hWnd = new HandleRef(null, UnsafeNativeMethods.GetWindowLong(new HandleRef(currentForm, currentForm.Handle), -8));
				if (hWnd.Handle != IntPtr.Zero)
				{
					if (!SafeNativeMethods.IsWindowEnabled(hWnd))
					{
						hWnd = new HandleRef(null, IntPtr.Zero);
					}
					else
					{
						SafeNativeMethods.EnableWindow(hWnd, enable: false);
					}
				}
				if (currentForm != null && currentForm.IsHandleCreated && SafeNativeMethods.IsWindowEnabled(new HandleRef(currentForm, currentForm.Handle)) != flag3)
				{
					SafeNativeMethods.EnableWindow(new HandleRef(currentForm, currentForm.Handle), flag3);
				}
			}
			try
			{
				if (messageLoopCount == 1)
				{
					WindowsFormsSynchronizationContext.InstallIfNeeded();
				}
				if (flag && currentForm != null)
				{
					currentForm.Visible = true;
				}
				if ((!flag && !flag2) || ComponentManager is ComponentManager)
				{
					bool flag4 = ComponentManager.FPushMessageLoop((IntPtr)componentID, reason, 0);
				}
				else if (reason == 2 || reason == -2)
				{
					bool flag4 = LocalModalMessageLoop(null);
				}
				else
				{
					bool flag4 = LocalModalMessageLoop(currentForm);
				}
			}
			finally
			{
				if (flag)
				{
					EndModalMessageLoop(context);
					if (hWnd.Handle != IntPtr.Zero)
					{
						SafeNativeMethods.EnableWindow(hWnd, enable: true);
					}
				}
				currentForm = form;
				totalMessageLoopCount--;
				messageLoopCount--;
				if (messageLoopCount == 0)
				{
					WindowsFormsSynchronizationContext.Uninstall(turnOffAutoInstall: false);
				}
				if (reason == -1)
				{
					Dispose(postQuit: true);
				}
				else if (messageLoopCount == 0 && componentManager != null)
				{
					RevokeComponent();
				}
			}
		}

		private bool LocalModalMessageLoop(Form form)
		{
			try
			{
				NativeMethods.MSG msg = default(NativeMethods.MSG);
				bool flag = false;
				bool flag2 = true;
				while (flag2)
				{
					if (UnsafeNativeMethods.PeekMessage(ref msg, NativeMethods.NullHandleRef, 0, 0, 0))
					{
						if (msg.hwnd != IntPtr.Zero && SafeNativeMethods.IsWindowUnicode(new HandleRef(null, msg.hwnd)))
						{
							flag = true;
							if (!UnsafeNativeMethods.GetMessageW(ref msg, NativeMethods.NullHandleRef, 0, 0))
							{
								continue;
							}
						}
						else
						{
							flag = false;
							if (!UnsafeNativeMethods.GetMessageA(ref msg, NativeMethods.NullHandleRef, 0, 0))
							{
								continue;
							}
						}
						if (!PreTranslateMessage(ref msg))
						{
							UnsafeNativeMethods.TranslateMessage(ref msg);
							if (flag)
							{
								UnsafeNativeMethods.DispatchMessageW(ref msg);
							}
							else
							{
								UnsafeNativeMethods.DispatchMessageA(ref msg);
							}
						}
						if (form != null)
						{
							flag2 = !form.CheckCloseDialog(closingOnly: false);
						}
					}
					else
					{
						if (form == null)
						{
							break;
						}
						if (!UnsafeNativeMethods.PeekMessage(ref msg, NativeMethods.NullHandleRef, 0, 0, 0))
						{
							UnsafeNativeMethods.WaitMessage();
						}
					}
				}
				return flag2;
			}
			catch
			{
				return false;
			}
		}

		internal bool ProcessFilters(ref NativeMethods.MSG msg, out bool modified)
		{
			bool result = false;
			modified = false;
			if (messageFilters != null && !GetState(16) && (LocalAppContextSwitches.DontSupportReentrantFilterMessage || inProcessFilters == 0))
			{
				if (messageFilters.Count > 0)
				{
					messageFilterSnapshot = new IMessageFilter[messageFilters.Count];
					messageFilters.CopyTo(messageFilterSnapshot);
				}
				else
				{
					messageFilterSnapshot = null;
				}
				SetState(16, value: true);
			}
			inProcessFilters++;
			try
			{
				if (messageFilterSnapshot == null)
				{
					return result;
				}
				int num = messageFilterSnapshot.Length;
				Message m = Message.Create(msg.hwnd, msg.message, msg.wParam, msg.lParam);
				for (int i = 0; i < num; i++)
				{
					IMessageFilter messageFilter = messageFilterSnapshot[i];
					bool flag = messageFilter.PreFilterMessage(ref m);
					if (messageFilter is IMessageModifyAndFilter)
					{
						msg.hwnd = m.HWnd;
						msg.message = m.Msg;
						msg.wParam = m.WParam;
						msg.lParam = m.LParam;
						modified = true;
					}
					if (flag)
					{
						return true;
					}
				}
				return result;
			}
			finally
			{
				inProcessFilters--;
			}
		}

		internal bool PreTranslateMessage(ref NativeMethods.MSG msg)
		{
			bool modified = false;
			if (ProcessFilters(ref msg, out modified))
			{
				return true;
			}
			if (msg.message >= 256 && msg.message <= 264)
			{
				if (msg.message == 258)
				{
					int num = 21364736;
					if ((int)(long)msg.wParam == 3 && ((int)(long)msg.lParam & num) == num && Debugger.IsAttached)
					{
						Debugger.Break();
					}
				}
				Control control = Control.FromChildHandleInternal(msg.hwnd);
				bool flag = false;
				Message msg2 = Message.Create(msg.hwnd, msg.message, msg.wParam, msg.lParam);
				if (control != null)
				{
					if (NativeWindow.WndProcShouldBeDebuggable)
					{
						if (Control.PreProcessControlMessageInternal(control, ref msg2) == PreProcessControlState.MessageProcessed)
						{
							flag = true;
						}
					}
					else
					{
						try
						{
							if (Control.PreProcessControlMessageInternal(control, ref msg2) == PreProcessControlState.MessageProcessed)
							{
								flag = true;
							}
						}
						catch (Exception t)
						{
							OnThreadException(t);
						}
					}
				}
				else
				{
					IntPtr ancestor = UnsafeNativeMethods.GetAncestor(new HandleRef(null, msg.hwnd), 2);
					if (ancestor != IntPtr.Zero && UnsafeNativeMethods.IsDialogMessage(new HandleRef(null, ancestor), ref msg))
					{
						return true;
					}
				}
				msg.wParam = msg2.WParam;
				msg.lParam = msg2.LParam;
				if (flag)
				{
					return true;
				}
			}
			return false;
		}

		private void RevokeComponent()
		{
			if (componentManager != null && componentID != -1)
			{
				int value = componentID;
				UnsafeNativeMethods.IMsoComponentManager msoComponentManager = componentManager;
				try
				{
					msoComponentManager.FRevokeComponent((IntPtr)value);
					if (Marshal.IsComObject(msoComponentManager))
					{
						Marshal.ReleaseComObject(msoComponentManager);
					}
				}
				finally
				{
					componentManager = null;
					componentID = -1;
				}
			}
		}

		internal void SetCulture(CultureInfo culture)
		{
			if (culture != null && culture.LCID != SafeNativeMethods.GetThreadLocale())
			{
				SafeNativeMethods.SetThreadLocale(culture.LCID);
			}
		}

		private void SetState(int bit, bool value)
		{
			if (value)
			{
				threadState |= bit;
			}
			else
			{
				threadState &= ~bit;
			}
		}

		bool UnsafeNativeMethods.IMsoComponent.FDebugMessage(IntPtr hInst, int msg, IntPtr wparam, IntPtr lparam)
		{
			return false;
		}

		bool UnsafeNativeMethods.IMsoComponent.FPreTranslateMessage(ref NativeMethods.MSG msg)
		{
			return PreTranslateMessage(ref msg);
		}

		void UnsafeNativeMethods.IMsoComponent.OnEnterState(int uStateID, bool fEnter)
		{
			if (!ourModalLoop && uStateID == 1)
			{
				if (fEnter)
				{
					DisableWindowsForModalLoop(onlyWinForms: true, null);
				}
				else
				{
					EnableWindowsForModalLoop(onlyWinForms: true, null);
				}
			}
		}

		void UnsafeNativeMethods.IMsoComponent.OnAppActivate(bool fActive, int dwOtherThreadID)
		{
		}

		void UnsafeNativeMethods.IMsoComponent.OnLoseActivation()
		{
		}

		void UnsafeNativeMethods.IMsoComponent.OnActivationChange(UnsafeNativeMethods.IMsoComponent component, bool fSameComponent, int pcrinfo, bool fHostIsActivating, int pchostinfo, int dwReserved)
		{
		}

		bool UnsafeNativeMethods.IMsoComponent.FDoIdle(int grfidlef)
		{
			if (idleHandler != null)
			{
				idleHandler(Thread.CurrentThread, EventArgs.Empty);
			}
			return false;
		}

		bool UnsafeNativeMethods.IMsoComponent.FContinueMessageLoop(int reason, int pvLoopData, NativeMethods.MSG[] msgPeeked)
		{
			bool result = true;
			if (msgPeeked == null && GetState(8))
			{
				result = false;
			}
			else
			{
				switch (reason)
				{
				case 1:
				{
					SafeNativeMethods.GetWindowThreadProcessId(new HandleRef(null, UnsafeNativeMethods.GetActiveWindow()), out int lpdwProcessId);
					if (lpdwProcessId == SafeNativeMethods.GetCurrentProcessId())
					{
						result = false;
					}
					break;
				}
				case 4:
				case 5:
					if (currentForm == null || currentForm.CheckCloseDialog(closingOnly: false))
					{
						result = false;
					}
					break;
				case -2:
				case 2:
					if (!UnsafeNativeMethods.PeekMessage(ref tempMsg, NativeMethods.NullHandleRef, 0, 0, 0))
					{
						result = false;
					}
					break;
				}
			}
			return result;
		}

		bool UnsafeNativeMethods.IMsoComponent.FQueryTerminate(bool fPromptUser)
		{
			return true;
		}

		void UnsafeNativeMethods.IMsoComponent.Terminate()
		{
			if (messageLoopCount > 0 && !(ComponentManager is ComponentManager))
			{
				messageLoopCount--;
			}
			Dispose(postQuit: false);
		}

		IntPtr UnsafeNativeMethods.IMsoComponent.HwndGetWindow(int dwWhich, int dwReserved)
		{
			return IntPtr.Zero;
		}
	}

	internal sealed class MarshalingControl : Control
	{
		protected override CreateParams CreateParams
		{
			get
			{
				CreateParams createParams = base.CreateParams;
				if (Environment.OSVersion.Platform == PlatformID.Win32NT)
				{
					createParams.Parent = (IntPtr)NativeMethods.HWND_MESSAGE;
				}
				return createParams;
			}
		}

		internal MarshalingControl()
			: base(autoInstallSyncContext: false)
		{
			base.Visible = false;
			SetState2(8, value: false);
			SetTopLevel(value: true);
			CreateControl();
			CreateHandle();
		}

		protected override void OnLayout(LayoutEventArgs levent)
		{
		}

		protected override void OnSizeChanged(EventArgs e)
		{
		}
	}

	internal sealed class ParkingWindow : ContainerControl, IArrangedElement, IComponent, IDisposable
	{
		private const int WM_CHECKDESTROY = 1025;

		private int childCount;

		protected override CreateParams CreateParams
		{
			get
			{
				CreateParams createParams = base.CreateParams;
				if (Environment.OSVersion.Platform == PlatformID.Win32NT)
				{
					createParams.Parent = (IntPtr)NativeMethods.HWND_MESSAGE;
				}
				return createParams;
			}
		}

		public ParkingWindow()
		{
			SetState2(8, value: false);
			SetState(524288, value: true);
			Text = "WindowsFormsParkingWindow";
			base.Visible = false;
		}

		internal override void AddReflectChild()
		{
			if (childCount < 0)
			{
				childCount = 0;
			}
			childCount++;
		}

		internal override void RemoveReflectChild()
		{
			childCount--;
			if (childCount < 0)
			{
				childCount = 0;
			}
			if (childCount == 0 && base.IsHandleCreated)
			{
				int lpdwProcessId;
				int windowThreadProcessId = SafeNativeMethods.GetWindowThreadProcessId(new HandleRef(this, base.HandleInternal), out lpdwProcessId);
				ThreadContext threadContext = ThreadContext.FromId(windowThreadProcessId);
				if (threadContext == null || threadContext != ThreadContext.FromCurrent())
				{
					UnsafeNativeMethods.PostMessage(new HandleRef(this, base.HandleInternal), 1025, IntPtr.Zero, IntPtr.Zero);
				}
				else
				{
					CheckDestroy();
				}
			}
		}

		private void CheckDestroy()
		{
			if (childCount == 0)
			{
				IntPtr window = UnsafeNativeMethods.GetWindow(new HandleRef(this, base.Handle), 5);
				if (window == IntPtr.Zero)
				{
					DestroyHandle();
				}
			}
		}

		public void Destroy()
		{
			DestroyHandle();
		}

		internal void ParkHandle(HandleRef handle)
		{
			if (!base.IsHandleCreated)
			{
				CreateHandle();
			}
			UnsafeNativeMethods.SetParent(handle, new HandleRef(this, base.Handle));
		}

		internal void UnparkHandle(HandleRef handle)
		{
			if (base.IsHandleCreated)
			{
				CheckDestroy();
			}
		}

		protected override void OnLayout(LayoutEventArgs levent)
		{
		}

		void IArrangedElement.PerformLayout(IArrangedElement affectedElement, string affectedProperty)
		{
		}

		protected override void WndProc(ref Message m)
		{
			if (m.Msg == 24)
			{
				return;
			}
			base.WndProc(ref m);
			if (m.Msg == 528)
			{
				if (NativeMethods.Util.LOWORD((int)(long)m.WParam) == 2)
				{
					UnsafeNativeMethods.PostMessage(new HandleRef(this, base.Handle), 1025, IntPtr.Zero, IntPtr.Zero);
				}
			}
			else if (m.Msg == 1025)
			{
				CheckDestroy();
			}
		}
	}

	private sealed class ThreadWindows
	{
		private IntPtr[] windows;

		private int windowCount;

		private IntPtr activeHwnd;

		private IntPtr focusedHwnd;

		internal ThreadWindows previousThreadWindows;

		private bool onlyWinForms = true;

		internal ThreadWindows(bool onlyWinForms)
		{
			windows = new IntPtr[16];
			this.onlyWinForms = onlyWinForms;
			UnsafeNativeMethods.EnumThreadWindows(SafeNativeMethods.GetCurrentThreadId(), Callback, NativeMethods.NullHandleRef);
		}

		private bool Callback(IntPtr hWnd, IntPtr lparam)
		{
			if (SafeNativeMethods.IsWindowVisible(new HandleRef(null, hWnd)) && SafeNativeMethods.IsWindowEnabled(new HandleRef(null, hWnd)))
			{
				bool flag = true;
				if (onlyWinForms)
				{
					Control control = Control.FromHandleInternal(hWnd);
					if (control == null)
					{
						flag = false;
					}
				}
				if (flag)
				{
					if (windowCount == windows.Length)
					{
						IntPtr[] destinationArray = new IntPtr[windowCount * 2];
						Array.Copy(windows, 0, destinationArray, 0, windowCount);
						windows = destinationArray;
					}
					windows[windowCount++] = hWnd;
				}
			}
			return true;
		}

		internal void Dispose()
		{
			for (int i = 0; i < windowCount; i++)
			{
				IntPtr handle = windows[i];
				if (UnsafeNativeMethods.IsWindow(new HandleRef(null, handle)))
				{
					Control.FromHandleInternal(handle)?.Dispose();
				}
			}
		}

		internal void Enable(bool state)
		{
			if (!onlyWinForms && !state)
			{
				activeHwnd = UnsafeNativeMethods.GetActiveWindow();
				Control activatingControl = ThreadContext.FromCurrent().ActivatingControl;
				if (activatingControl != null)
				{
					focusedHwnd = activatingControl.Handle;
				}
				else
				{
					focusedHwnd = UnsafeNativeMethods.GetFocus();
				}
			}
			for (int i = 0; i < windowCount; i++)
			{
				IntPtr handle = windows[i];
				if (UnsafeNativeMethods.IsWindow(new HandleRef(null, handle)))
				{
					SafeNativeMethods.EnableWindow(new HandleRef(null, handle), state);
				}
			}
			if (!onlyWinForms && state)
			{
				if (activeHwnd != IntPtr.Zero && UnsafeNativeMethods.IsWindow(new HandleRef(null, activeHwnd)))
				{
					UnsafeNativeMethods.SetActiveWindow(new HandleRef(null, activeHwnd));
				}
				if (focusedHwnd != IntPtr.Zero && UnsafeNativeMethods.IsWindow(new HandleRef(null, focusedHwnd)))
				{
					UnsafeNativeMethods.SetFocus(new HandleRef(null, focusedHwnd));
				}
			}
		}
	}

	private class ModalApplicationContext : ApplicationContext
	{
		private delegate void ThreadWindowCallback(ThreadContext context, bool onlyWinForms);

		private ThreadContext parentWindowContext;

		public ModalApplicationContext(Form modalForm)
			: base(modalForm)
		{
		}

		public void DisableThreadWindows(bool disable, bool onlyWinForms)
		{
			Control control = null;
			if (base.MainForm != null && base.MainForm.IsHandleCreated)
			{
				IntPtr windowLong = UnsafeNativeMethods.GetWindowLong(new HandleRef(this, base.MainForm.Handle), -8);
				control = Control.FromHandleInternal(windowLong);
				if (control != null && control.InvokeRequired)
				{
					parentWindowContext = GetContextForHandle(new HandleRef(this, windowLong));
				}
				else
				{
					parentWindowContext = null;
				}
			}
			if (parentWindowContext != null)
			{
				if (control == null)
				{
					control = parentWindowContext.ApplicationContext.MainForm;
				}
				if (disable)
				{
					control.Invoke(new ThreadWindowCallback(DisableThreadWindowsCallback), parentWindowContext, onlyWinForms);
				}
				else
				{
					control.Invoke(new ThreadWindowCallback(EnableThreadWindowsCallback), parentWindowContext, onlyWinForms);
				}
			}
		}

		private void DisableThreadWindowsCallback(ThreadContext context, bool onlyWinForms)
		{
			context.DisableWindowsForModalLoop(onlyWinForms, this);
		}

		private void EnableThreadWindowsCallback(ThreadContext context, bool onlyWinForms)
		{
			context.EnableWindowsForModalLoop(onlyWinForms, this);
		}

		protected override void ExitThreadCore()
		{
		}
	}

	private static EventHandlerList eventHandlers;

	private static string startupPath;

	private static string executablePath;

	private static object appFileVersion;

	private static Type mainType;

	private static string companyName;

	private static string productName;

	private static string productVersion;

	private static string safeTopLevelCaptionSuffix;

	private static bool useVisualStyles = false;

	private static bool comCtlSupportsVisualStylesInitialized = false;

	private static bool comCtlSupportsVisualStyles = false;

	private static FormCollection forms = null;

	private static object internalSyncObject = new object();

	private static bool useWaitCursor = false;

	private static bool useEverettThreadAffinity = false;

	private static bool checkedThreadAffinity = false;

	private const string everettThreadAffinityValue = "EnableSystemEventsThreadAffinityCompatibility";

	private static bool exiting;

	private static readonly object EVENT_APPLICATIONEXIT = new object();

	private static readonly object EVENT_THREADEXIT = new object();

	private const string IEEXEC = "ieexec.exe";

	private const string CLICKONCE_APPS_DATADIRECTORY = "DataDirectory";

	private static bool parkingWindowSupportsPMAv2 = true;

	public static bool AllowQuit => ThreadContext.FromCurrent().GetAllowQuit();

	internal static bool CanContinueIdle => ThreadContext.FromCurrent().ComponentManager.FContinueIdle();

	internal static bool ComCtlSupportsVisualStyles
	{
		get
		{
			if (!comCtlSupportsVisualStylesInitialized)
			{
				comCtlSupportsVisualStyles = InitializeComCtlSupportsVisualStyles();
				comCtlSupportsVisualStylesInitialized = true;
			}
			return comCtlSupportsVisualStyles;
		}
	}

	public static RegistryKey CommonAppDataRegistry => Registry.LocalMachine.CreateSubKey(CommonAppDataRegistryKeyName);

	internal static string CommonAppDataRegistryKeyName
	{
		get
		{
			string format = "Software\\{0}\\{1}\\{2}";
			return string.Format(CultureInfo.CurrentCulture, format, new object[3]
			{
				CompanyName,
				ProductName,
				ProductVersion
			});
		}
	}

	internal static bool UseEverettThreadAffinity
	{
		get
		{
			if (!checkedThreadAffinity)
			{
				checkedThreadAffinity = true;
				try
				{
					new RegistryPermission(PermissionState.Unrestricted).Assert();
					RegistryKey registryKey = Registry.LocalMachine.OpenSubKey(CommonAppDataRegistryKeyName);
					if (registryKey != null)
					{
						object value = registryKey.GetValue("EnableSystemEventsThreadAffinityCompatibility");
						registryKey.Close();
						if (value != null && (int)value != 0)
						{
							useEverettThreadAffinity = true;
						}
					}
				}
				catch (SecurityException)
				{
				}
				catch (InvalidCastException)
				{
				}
			}
			return useEverettThreadAffinity;
		}
	}

	public static string CommonAppDataPath
	{
		get
		{
			try
			{
				if (ApplicationDeployment.IsNetworkDeployed)
				{
					string text = AppDomain.CurrentDomain.GetData("DataDirectory") as string;
					if (text != null)
					{
						return text;
					}
				}
			}
			catch (Exception ex)
			{
				if (ClientUtils.IsSecurityOrCriticalException(ex))
				{
					throw;
				}
			}
			return GetDataPath(Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData));
		}
	}

	public static string CompanyName
	{
		get
		{
			lock (internalSyncObject)
			{
				if (companyName == null)
				{
					Assembly entryAssembly = Assembly.GetEntryAssembly();
					if (entryAssembly != null)
					{
						object[] customAttributes = entryAssembly.GetCustomAttributes(typeof(AssemblyCompanyAttribute), inherit: false);
						if (customAttributes != null && customAttributes.Length != 0)
						{
							companyName = ((AssemblyCompanyAttribute)customAttributes[0]).Company;
						}
					}
					if (companyName == null || companyName.Length == 0)
					{
						companyName = GetAppFileVersionInfo().CompanyName;
						if (companyName != null)
						{
							companyName = companyName.Trim();
						}
					}
					if (companyName == null || companyName.Length == 0)
					{
						Type appMainType = GetAppMainType();
						if (appMainType != null)
						{
							string @namespace = appMainType.Namespace;
							if (!string.IsNullOrEmpty(@namespace))
							{
								int num = @namespace.IndexOf(".");
								if (num != -1)
								{
									companyName = @namespace.Substring(0, num);
								}
								else
								{
									companyName = @namespace;
								}
							}
							else
							{
								companyName = ProductName;
							}
						}
					}
				}
			}
			return companyName;
		}
	}

	public static CultureInfo CurrentCulture
	{
		get
		{
			return Thread.CurrentThread.CurrentCulture;
		}
		set
		{
			Thread.CurrentThread.CurrentCulture = value;
		}
	}

	public static InputLanguage CurrentInputLanguage
	{
		get
		{
			return InputLanguage.CurrentInputLanguage;
		}
		set
		{
			IntSecurity.AffectThreadBehavior.Demand();
			InputLanguage.CurrentInputLanguage = value;
		}
	}

	internal static bool CustomThreadExceptionHandlerAttached => ThreadContext.FromCurrent().CustomThreadExceptionHandlerAttached;

	public static string ExecutablePath
	{
		get
		{
			if (executablePath == null)
			{
				Assembly entryAssembly = Assembly.GetEntryAssembly();
				if (entryAssembly == null)
				{
					StringBuilder moduleFileNameLongPath = UnsafeNativeMethods.GetModuleFileNameLongPath(NativeMethods.NullHandleRef);
					executablePath = IntSecurity.UnsafeGetFullPath(moduleFileNameLongPath.ToString());
				}
				else
				{
					string codeBase = entryAssembly.CodeBase;
					Uri uri = new Uri(codeBase);
					if (uri.IsFile)
					{
						executablePath = uri.LocalPath + Uri.UnescapeDataString(uri.Fragment);
					}
					else
					{
						executablePath = uri.ToString();
					}
				}
			}
			Uri uri2 = new Uri(executablePath);
			if (uri2.Scheme == "file")
			{
				new FileIOPermission(FileIOPermissionAccess.PathDiscovery, executablePath).Demand();
			}
			return executablePath;
		}
	}

	public static string LocalUserAppDataPath
	{
		get
		{
			try
			{
				if (ApplicationDeployment.IsNetworkDeployed)
				{
					string text = AppDomain.CurrentDomain.GetData("DataDirectory") as string;
					if (text != null)
					{
						return text;
					}
				}
			}
			catch (Exception ex)
			{
				if (ClientUtils.IsSecurityOrCriticalException(ex))
				{
					throw;
				}
			}
			return GetDataPath(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData));
		}
	}

	public static bool MessageLoop => ThreadContext.FromCurrent().GetMessageLoop();

	public static FormCollection OpenForms
	{
		[UIPermission(SecurityAction.Demand, Window = UIPermissionWindow.AllWindows)]
		get
		{
			return OpenFormsInternal;
		}
	}

	internal static FormCollection OpenFormsInternal
	{
		get
		{
			if (forms == null)
			{
				forms = new FormCollection();
			}
			return forms;
		}
	}

	public static string ProductName
	{
		get
		{
			lock (internalSyncObject)
			{
				if (productName == null)
				{
					Assembly entryAssembly = Assembly.GetEntryAssembly();
					if (entryAssembly != null)
					{
						object[] customAttributes = entryAssembly.GetCustomAttributes(typeof(AssemblyProductAttribute), inherit: false);
						if (customAttributes != null && customAttributes.Length != 0)
						{
							productName = ((AssemblyProductAttribute)customAttributes[0]).Product;
						}
					}
					if (productName == null || productName.Length == 0)
					{
						productName = GetAppFileVersionInfo().ProductName;
						if (productName != null)
						{
							productName = productName.Trim();
						}
					}
					if (productName == null || productName.Length == 0)
					{
						Type appMainType = GetAppMainType();
						if (appMainType != null)
						{
							string @namespace = appMainType.Namespace;
							if (!string.IsNullOrEmpty(@namespace))
							{
								int num = @namespace.LastIndexOf(".");
								if (num != -1 && num < @namespace.Length - 1)
								{
									productName = @namespace.Substring(num + 1);
								}
								else
								{
									productName = @namespace;
								}
							}
							else
							{
								productName = appMainType.Name;
							}
						}
					}
				}
			}
			return productName;
		}
	}

	public static string ProductVersion
	{
		get
		{
			lock (internalSyncObject)
			{
				if (productVersion == null)
				{
					Assembly entryAssembly = Assembly.GetEntryAssembly();
					if (entryAssembly != null)
					{
						object[] customAttributes = entryAssembly.GetCustomAttributes(typeof(AssemblyInformationalVersionAttribute), inherit: false);
						if (customAttributes != null && customAttributes.Length != 0)
						{
							productVersion = ((AssemblyInformationalVersionAttribute)customAttributes[0]).InformationalVersion;
						}
					}
					if (productVersion == null || productVersion.Length == 0)
					{
						productVersion = GetAppFileVersionInfo().ProductVersion;
						if (productVersion != null)
						{
							productVersion = productVersion.Trim();
						}
					}
					if (productVersion == null || productVersion.Length == 0)
					{
						productVersion = "1.0.0.0";
					}
				}
			}
			return productVersion;
		}
	}

	public static bool RenderWithVisualStyles
	{
		get
		{
			if (ComCtlSupportsVisualStyles)
			{
				return VisualStyleRenderer.IsSupported;
			}
			return false;
		}
	}

	public static string SafeTopLevelCaptionFormat
	{
		get
		{
			if (safeTopLevelCaptionSuffix == null)
			{
				safeTopLevelCaptionSuffix = SR.GetString("SafeTopLevelCaptionFormat");
			}
			return safeTopLevelCaptionSuffix;
		}
		set
		{
			IntSecurity.WindowAdornmentModification.Demand();
			if (value == null)
			{
				value = string.Empty;
			}
			safeTopLevelCaptionSuffix = value;
		}
	}

	public static string StartupPath
	{
		get
		{
			if (startupPath == null)
			{
				StringBuilder moduleFileNameLongPath = UnsafeNativeMethods.GetModuleFileNameLongPath(NativeMethods.NullHandleRef);
				startupPath = Path.GetDirectoryName(moduleFileNameLongPath.ToString());
			}
			new FileIOPermission(FileIOPermissionAccess.PathDiscovery, startupPath).Demand();
			return startupPath;
		}
	}

	public static bool UseWaitCursor
	{
		get
		{
			return useWaitCursor;
		}
		set
		{
			lock (FormCollection.CollectionSyncRoot)
			{
				useWaitCursor = value;
				foreach (Form item in OpenFormsInternal)
				{
					item.UseWaitCursor = useWaitCursor;
				}
			}
		}
	}

	public static string UserAppDataPath
	{
		get
		{
			try
			{
				if (ApplicationDeployment.IsNetworkDeployed)
				{
					string text = AppDomain.CurrentDomain.GetData("DataDirectory") as string;
					if (text != null)
					{
						return text;
					}
				}
			}
			catch (Exception ex)
			{
				if (ClientUtils.IsSecurityOrCriticalException(ex))
				{
					throw;
				}
			}
			return GetDataPath(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData));
		}
	}

	public static RegistryKey UserAppDataRegistry
	{
		get
		{
			string format = "Software\\{0}\\{1}\\{2}";
			return Registry.CurrentUser.CreateSubKey(string.Format(CultureInfo.CurrentCulture, format, new object[3]
			{
				CompanyName,
				ProductName,
				ProductVersion
			}));
		}
	}

	internal static bool UseVisualStyles => useVisualStyles;

	internal static string WindowsFormsVersion => "WindowsForms10";

	internal static string WindowMessagesVersion => "WindowsForms12";

	public static VisualStyleState VisualStyleState
	{
		get
		{
			if (!VisualStyleInformation.IsSupportedByOS)
			{
				return VisualStyleState.NoneEnabled;
			}
			return (VisualStyleState)SafeNativeMethods.GetThemeAppProperties();
		}
		set
		{
			if (VisualStyleInformation.IsSupportedByOS)
			{
				if (!ClientUtils.IsEnumValid(value, (int)value, 0, 3) && LocalAppContextSwitches.EnableVisualStyleValidation)
				{
					throw new InvalidEnumArgumentException("value", (int)value, typeof(VisualStyleState));
				}
				SafeNativeMethods.SetThemeAppProperties((int)value);
				SafeNativeMethods.EnumThreadWindowsCallback enumThreadWindowsCallback = SendThemeChanged;
				SafeNativeMethods.EnumWindows(enumThreadWindowsCallback, IntPtr.Zero);
				GC.KeepAlive(enumThreadWindowsCallback);
			}
		}
	}

	public static event EventHandler ApplicationExit
	{
		add
		{
			AddEventHandler(EVENT_APPLICATIONEXIT, value);
		}
		remove
		{
			RemoveEventHandler(EVENT_APPLICATIONEXIT, value);
		}
	}

	public static event EventHandler Idle
	{
		add
		{
			ThreadContext threadContext = ThreadContext.FromCurrent();
			lock (threadContext)
			{
				threadContext.idleHandler = (EventHandler)Delegate.Combine(threadContext.idleHandler, value);
				object componentManager = threadContext.ComponentManager;
			}
		}
		remove
		{
			ThreadContext threadContext = ThreadContext.FromCurrent();
			lock (threadContext)
			{
				threadContext.idleHandler = (EventHandler)Delegate.Remove(threadContext.idleHandler, value);
			}
		}
	}

	[EditorBrowsable(EditorBrowsableState.Advanced)]
	public static event EventHandler EnterThreadModal
	{
		[SecurityPermission(SecurityAction.LinkDemand, Flags = SecurityPermissionFlag.UnmanagedCode)]
		add
		{
			ThreadContext threadContext = ThreadContext.FromCurrent();
			lock (threadContext)
			{
				threadContext.enterModalHandler = (EventHandler)Delegate.Combine(threadContext.enterModalHandler, value);
			}
		}
		[SecurityPermission(SecurityAction.LinkDemand, Flags = SecurityPermissionFlag.UnmanagedCode)]
		remove
		{
			ThreadContext threadContext = ThreadContext.FromCurrent();
			lock (threadContext)
			{
				threadContext.enterModalHandler = (EventHandler)Delegate.Remove(threadContext.enterModalHandler, value);
			}
		}
	}

	[EditorBrowsable(EditorBrowsableState.Advanced)]
	public static event EventHandler LeaveThreadModal
	{
		[SecurityPermission(SecurityAction.LinkDemand, Flags = SecurityPermissionFlag.UnmanagedCode)]
		add
		{
			ThreadContext threadContext = ThreadContext.FromCurrent();
			lock (threadContext)
			{
				threadContext.leaveModalHandler = (EventHandler)Delegate.Combine(threadContext.leaveModalHandler, value);
			}
		}
		[SecurityPermission(SecurityAction.LinkDemand, Flags = SecurityPermissionFlag.UnmanagedCode)]
		remove
		{
			ThreadContext threadContext = ThreadContext.FromCurrent();
			lock (threadContext)
			{
				threadContext.leaveModalHandler = (EventHandler)Delegate.Remove(threadContext.leaveModalHandler, value);
			}
		}
	}

	public static event ThreadExceptionEventHandler ThreadException
	{
		add
		{
			IntSecurity.AffectThreadBehavior.Demand();
			ThreadContext threadContext = ThreadContext.FromCurrent();
			lock (threadContext)
			{
				threadContext.threadExceptionHandler = value;
			}
		}
		remove
		{
			ThreadContext threadContext = ThreadContext.FromCurrent();
			lock (threadContext)
			{
				threadContext.threadExceptionHandler = (ThreadExceptionEventHandler)Delegate.Remove(threadContext.threadExceptionHandler, value);
			}
		}
	}

	public static event EventHandler ThreadExit
	{
		add
		{
			AddEventHandler(EVENT_THREADEXIT, value);
		}
		remove
		{
			RemoveEventHandler(EVENT_THREADEXIT, value);
		}
	}

	private Application()
	{
	}

	private static bool InitializeComCtlSupportsVisualStyles()
	{
		if (useVisualStyles && OSFeature.Feature.IsPresent(OSFeature.Themes))
		{
			return true;
		}
		IntPtr moduleHandle = UnsafeNativeMethods.GetModuleHandle("comctl32.dll");
		if (moduleHandle != IntPtr.Zero)
		{
			try
			{
				IntPtr procAddress = UnsafeNativeMethods.GetProcAddress(new HandleRef(null, moduleHandle), "ImageList_WriteEx");
				return procAddress != IntPtr.Zero;
			}
			catch
			{
			}
		}
		else
		{
			moduleHandle = UnsafeNativeMethods.LoadLibraryFromSystemPathIfAvailable("comctl32.dll");
			if (moduleHandle != IntPtr.Zero)
			{
				IntPtr procAddress2 = UnsafeNativeMethods.GetProcAddress(new HandleRef(null, moduleHandle), "ImageList_WriteEx");
				return procAddress2 != IntPtr.Zero;
			}
		}
		return false;
	}

	internal static void OpenFormsInternalAdd(Form form)
	{
		OpenFormsInternal.Add(form);
	}

	internal static void OpenFormsInternalRemove(Form form)
	{
		OpenFormsInternal.Remove(form);
	}

	[EditorBrowsable(EditorBrowsableState.Advanced)]
	[SecurityPermission(SecurityAction.LinkDemand, Flags = SecurityPermissionFlag.UnmanagedCode)]
	public static void RegisterMessageLoop(MessageLoopCallback callback)
	{
		ThreadContext.FromCurrent().RegisterMessageLoop(callback);
	}

	[EditorBrowsable(EditorBrowsableState.Advanced)]
	[SecurityPermission(SecurityAction.LinkDemand, Flags = SecurityPermissionFlag.UnmanagedCode)]
	public static void UnregisterMessageLoop()
	{
		ThreadContext.FromCurrent().RegisterMessageLoop(null);
	}

	private static bool SendThemeChanged(IntPtr handle, IntPtr extraParameter)
	{
		int currentProcessId = SafeNativeMethods.GetCurrentProcessId();
		SafeNativeMethods.GetWindowThreadProcessId(new HandleRef(null, handle), out int lpdwProcessId);
		if (lpdwProcessId == currentProcessId && SafeNativeMethods.IsWindowVisible(new HandleRef(null, handle)))
		{
			SendThemeChangedRecursive(handle, IntPtr.Zero);
			SafeNativeMethods.RedrawWindow(new HandleRef(null, handle), null, NativeMethods.NullHandleRef, 1157);
		}
		return true;
	}

	private static bool SendThemeChangedRecursive(IntPtr handle, IntPtr lparam)
	{
		UnsafeNativeMethods.EnumChildWindows(new HandleRef(null, handle), SendThemeChangedRecursive, NativeMethods.NullHandleRef);
		UnsafeNativeMethods.SendMessage(new HandleRef(null, handle), 794, 0, 0);
		return true;
	}

	private static void AddEventHandler(object key, Delegate value)
	{
		lock (internalSyncObject)
		{
			if (eventHandlers == null)
			{
				eventHandlers = new EventHandlerList();
			}
			eventHandlers.AddHandler(key, value);
		}
	}

	private static void RemoveEventHandler(object key, Delegate value)
	{
		lock (internalSyncObject)
		{
			if (eventHandlers != null)
			{
				eventHandlers.RemoveHandler(key, value);
			}
		}
	}

	public static void AddMessageFilter(IMessageFilter value)
	{
		IntSecurity.UnmanagedCode.Demand();
		ThreadContext.FromCurrent().AddMessageFilter(value);
	}

	[EditorBrowsable(EditorBrowsableState.Advanced)]
	[SecurityPermission(SecurityAction.LinkDemand, Flags = SecurityPermissionFlag.UnmanagedCode)]
	public static bool FilterMessage(ref Message message)
	{
		NativeMethods.MSG msg = default(NativeMethods.MSG);
		msg.hwnd = message.HWnd;
		msg.message = message.Msg;
		msg.wParam = message.WParam;
		msg.lParam = message.LParam;
		bool modified;
		bool result = ThreadContext.FromCurrent().ProcessFilters(ref msg, out modified);
		if (modified)
		{
			message.HWnd = msg.hwnd;
			message.Msg = msg.message;
			message.WParam = msg.wParam;
			message.LParam = msg.lParam;
		}
		return result;
	}

	internal static void BeginModalMessageLoop()
	{
		ThreadContext.FromCurrent().BeginModalMessageLoop(null);
	}

	public static void DoEvents()
	{
		ThreadContext.FromCurrent().RunMessageLoop(2, null);
	}

	internal static void DoEventsModal()
	{
		ThreadContext.FromCurrent().RunMessageLoop(-2, null);
	}

	public static void EnableVisualStyles()
	{
		string text = null;
		FileIOPermission fileIOPermission = new FileIOPermission(PermissionState.None);
		fileIOPermission.AllFiles = FileIOPermissionAccess.PathDiscovery;
		fileIOPermission.Assert();
		try
		{
			text = typeof(Application).Assembly.Location;
		}
		finally
		{
			CodeAccessPermission.RevertAssert();
		}
		if (text != null)
		{
			EnableVisualStylesInternal(text, 101);
		}
	}

	private static void EnableVisualStylesInternal(string assemblyFileName, int nativeResourceID)
	{
		useVisualStyles = UnsafeNativeMethods.ThemingScope.CreateActivationContext(assemblyFileName, nativeResourceID);
	}

	internal static void EndModalMessageLoop()
	{
		ThreadContext.FromCurrent().EndModalMessageLoop(null);
	}

	public static void Exit()
	{
		Exit(null);
	}

	[EditorBrowsable(EditorBrowsableState.Advanced)]
	public static void Exit(CancelEventArgs e)
	{
		Assembly entryAssembly = Assembly.GetEntryAssembly();
		Assembly callingAssembly = Assembly.GetCallingAssembly();
		if (entryAssembly == null || callingAssembly == null || !entryAssembly.Equals(callingAssembly))
		{
			IntSecurity.AffectThreadBehavior.Demand();
		}
		bool cancel = ExitInternal();
		if (e != null)
		{
			e.Cancel = cancel;
		}
	}

	private static bool ExitInternal()
	{
		bool flag = false;
		lock (internalSyncObject)
		{
			if (!exiting)
			{
				exiting = true;
				try
				{
					if (forms != null)
					{
						foreach (Form item in OpenFormsInternal)
						{
							if (item.RaiseFormClosingOnAppExit())
							{
								flag = true;
								break;
							}
						}
					}
					if (flag)
					{
						return flag;
					}
					if (forms != null)
					{
						while (OpenFormsInternal.Count > 0)
						{
							OpenFormsInternal[0].RaiseFormClosedOnAppExit();
						}
					}
					ThreadContext.ExitApplication();
					return flag;
				}
				finally
				{
					exiting = false;
				}
			}
			return false;
		}
	}

	public static void ExitThread()
	{
		IntSecurity.AffectThreadBehavior.Demand();
		ExitThreadInternal();
	}

	private static void ExitThreadInternal()
	{
		ThreadContext threadContext = ThreadContext.FromCurrent();
		if (threadContext.ApplicationContext != null)
		{
			threadContext.ApplicationContext.ExitThread();
		}
		else
		{
			threadContext.Dispose(postQuit: true);
		}
	}

	internal static void FormActivated(bool modal, bool activated)
	{
		if (!modal)
		{
			ThreadContext.FromCurrent().FormActivated(activated);
		}
	}

	private static FileVersionInfo GetAppFileVersionInfo()
	{
		lock (internalSyncObject)
		{
			if (appFileVersion == null)
			{
				Type appMainType = GetAppMainType();
				if (appMainType != null)
				{
					FileIOPermission fileIOPermission = new FileIOPermission(PermissionState.None);
					fileIOPermission.AllFiles = (FileIOPermissionAccess.Read | FileIOPermissionAccess.PathDiscovery);
					fileIOPermission.Assert();
					try
					{
						appFileVersion = FileVersionInfo.GetVersionInfo(appMainType.Module.FullyQualifiedName);
					}
					finally
					{
						CodeAccessPermission.RevertAssert();
					}
				}
				else
				{
					appFileVersion = FileVersionInfo.GetVersionInfo(ExecutablePath);
				}
			}
		}
		return (FileVersionInfo)appFileVersion;
	}

	private static Type GetAppMainType()
	{
		lock (internalSyncObject)
		{
			if (mainType == null)
			{
				Assembly entryAssembly = Assembly.GetEntryAssembly();
				if (entryAssembly != null)
				{
					mainType = entryAssembly.EntryPoint.ReflectedType;
				}
			}
		}
		return mainType;
	}

	private static ThreadContext GetContextForHandle(HandleRef handle)
	{
		int lpdwProcessId;
		int windowThreadProcessId = SafeNativeMethods.GetWindowThreadProcessId(handle, out lpdwProcessId);
		return ThreadContext.FromId(windowThreadProcessId);
	}

	private static string GetDataPath(string basePath)
	{
		string format = "{0}\\{1}\\{2}\\{3}";
		string text = CompanyName;
		string text2 = ProductName;
		string text3 = ProductVersion;
		string text4 = string.Format(CultureInfo.CurrentCulture, format, basePath, text, text2, text3);
		lock (internalSyncObject)
		{
			if (Directory.Exists(text4))
			{
				return text4;
			}
			Directory.CreateDirectory(text4);
			return text4;
		}
	}

	private static void RaiseExit()
	{
		if (eventHandlers != null)
		{
			Delegate @delegate = eventHandlers[EVENT_APPLICATIONEXIT];
			if ((object)@delegate != null)
			{
				((EventHandler)@delegate)(null, EventArgs.Empty);
			}
		}
	}

	private static void RaiseThreadExit()
	{
		if (eventHandlers != null)
		{
			Delegate @delegate = eventHandlers[EVENT_THREADEXIT];
			if ((object)@delegate != null)
			{
				((EventHandler)@delegate)(null, EventArgs.Empty);
			}
		}
	}

	internal static void ParkHandle(HandleRef handle, System.Windows.Forms.DpiAwarenessContext dpiAwarenessContext = System.Windows.Forms.DpiAwarenessContext.DPI_AWARENESS_CONTEXT_UNSPECIFIED)
	{
		GetContextForHandle(handle)?.GetParkingWindow(dpiAwarenessContext).ParkHandle(handle);
	}

	internal static void ParkHandle(CreateParams cp, System.Windows.Forms.DpiAwarenessContext dpiAwarenessContext = System.Windows.Forms.DpiAwarenessContext.DPI_AWARENESS_CONTEXT_UNSPECIFIED)
	{
		ThreadContext threadContext = ThreadContext.FromCurrent();
		if (threadContext != null)
		{
			cp.Parent = threadContext.GetParkingWindow(dpiAwarenessContext).Handle;
		}
	}

	public static ApartmentState OleRequired()
	{
		return ThreadContext.FromCurrent().OleRequired();
	}

	public static void OnThreadException(Exception t)
	{
		ThreadContext.FromCurrent().OnThreadException(t);
	}

	internal static void UnparkHandle(HandleRef handle, System.Windows.Forms.DpiAwarenessContext context)
	{
		GetContextForHandle(handle)?.GetParkingWindow(context).UnparkHandle(handle);
	}

	[EditorBrowsable(EditorBrowsableState.Advanced)]
	[SecurityPermission(SecurityAction.LinkDemand, Flags = SecurityPermissionFlag.UnmanagedCode)]
	public static void RaiseIdle(EventArgs e)
	{
		ThreadContext threadContext = ThreadContext.FromCurrent();
		if (threadContext.idleHandler != null)
		{
			threadContext.idleHandler(Thread.CurrentThread, e);
		}
	}

	public static void RemoveMessageFilter(IMessageFilter value)
	{
		ThreadContext.FromCurrent().RemoveMessageFilter(value);
	}

	[SecurityPermission(SecurityAction.LinkDemand, Flags = SecurityPermissionFlag.UnmanagedCode)]
	[SecurityPermission(SecurityAction.Demand, Flags = SecurityPermissionFlag.UnmanagedCode)]
	public static void Restart()
	{
		if (Assembly.GetEntryAssembly() == null)
		{
			throw new NotSupportedException(SR.GetString("RestartNotSupported"));
		}
		bool flag = false;
		Process currentProcess = Process.GetCurrentProcess();
		if (string.Equals(currentProcess.MainModule.ModuleName, "ieexec.exe", StringComparison.OrdinalIgnoreCase))
		{
			string str = string.Empty;
			new FileIOPermission(PermissionState.Unrestricted).Assert();
			try
			{
				str = Path.GetDirectoryName(typeof(object).Module.FullyQualifiedName);
			}
			finally
			{
				CodeAccessPermission.RevertAssert();
			}
			if (string.Equals(str + "\\ieexec.exe", currentProcess.MainModule.FileName, StringComparison.OrdinalIgnoreCase))
			{
				flag = true;
				ExitInternal();
				string text = AppDomain.CurrentDomain.GetData("APP_LAUNCH_URL") as string;
				if (text != null)
				{
					Process.Start(currentProcess.MainModule.FileName, text);
				}
			}
		}
		if (flag)
		{
			return;
		}
		if (ApplicationDeployment.IsNetworkDeployed)
		{
			string updatedApplicationFullName = ApplicationDeployment.CurrentDeployment.UpdatedApplicationFullName;
			uint hostTypeFromMetaData = (uint)ClickOnceUtility.GetHostTypeFromMetaData(updatedApplicationFullName);
			ExitInternal();
			UnsafeNativeMethods.CorLaunchApplication(hostTypeFromMetaData, updatedApplicationFullName, 0, null, 0, null, new UnsafeNativeMethods.PROCESS_INFORMATION());
			return;
		}
		string[] commandLineArgs = Environment.GetCommandLineArgs();
		StringBuilder stringBuilder = new StringBuilder((commandLineArgs.Length - 1) * 16);
		for (int i = 1; i < commandLineArgs.Length - 1; i++)
		{
			stringBuilder.Append('"');
			stringBuilder.Append(commandLineArgs[i]);
			stringBuilder.Append("\" ");
		}
		if (commandLineArgs.Length > 1)
		{
			stringBuilder.Append('"');
			stringBuilder.Append(commandLineArgs[commandLineArgs.Length - 1]);
			stringBuilder.Append('"');
		}
		ProcessStartInfo startInfo = Process.GetCurrentProcess().StartInfo;
		startInfo.FileName = ExecutablePath;
		if (stringBuilder.Length > 0)
		{
			startInfo.Arguments = stringBuilder.ToString();
		}
		ExitInternal();
		Process.Start(startInfo);
	}

	public static void Run()
	{
		ThreadContext.FromCurrent().RunMessageLoop(-1, new ApplicationContext());
	}

	public static void Run(Form mainForm)
	{
		ThreadContext.FromCurrent().RunMessageLoop(-1, new ApplicationContext(mainForm));
	}

	public static void Run(ApplicationContext context)
	{
		ThreadContext.FromCurrent().RunMessageLoop(-1, context);
	}

	internal static void RunDialog(Form form)
	{
		ThreadContext.FromCurrent().RunMessageLoop(4, new ModalApplicationContext(form));
	}

	public static void SetCompatibleTextRenderingDefault(bool defaultValue)
	{
		if (NativeWindow.AnyHandleCreated)
		{
			throw new InvalidOperationException(SR.GetString("Win32WindowAlreadyCreated"));
		}
		Control.UseCompatibleTextRenderingDefault = defaultValue;
	}

	public static bool SetSuspendState(PowerState state, bool force, bool disableWakeEvent)
	{
		IntSecurity.AffectMachineState.Demand();
		return UnsafeNativeMethods.SetSuspendState(state == PowerState.Hibernate, force, disableWakeEvent);
	}

	public static void SetUnhandledExceptionMode(UnhandledExceptionMode mode)
	{
		SetUnhandledExceptionMode(mode, threadScope: true);
	}

	public static void SetUnhandledExceptionMode(UnhandledExceptionMode mode, bool threadScope)
	{
		IntSecurity.AffectThreadBehavior.Demand();
		NativeWindow.SetUnhandledExceptionModeInternal(mode, threadScope);
	}
}
