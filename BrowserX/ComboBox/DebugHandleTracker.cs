// System.Internal.DebugHandleTracker
using System;
using System.Collections;
using System.ComponentModel;
using System.Diagnostics;
using System.Internal;

internal class DebugHandleTracker
{
	private class HandleType
	{
		private class HandleEntry
		{
			private class StackParser
			{
				internal string releventStack;

				internal int startIndex;

				internal int endIndex;

				internal int length;

				public StackParser(string callStack)
				{
					releventStack = callStack;
					length = releventStack.Length;
				}

				private static bool ContainsString(string str, string token)
				{
					int num = str.Length;
					int num2 = token.Length;
					for (int i = 0; i < num; i++)
					{
						int j;
						for (j = 0; j < num2 && str[i + j] == token[j]; j++)
						{
						}
						if (j == num2)
						{
							return true;
						}
					}
					return false;
				}

				public void DiscardNext()
				{
					GetLine();
				}

				public void DiscardTo(string discardText)
				{
					while (startIndex < length)
					{
						string line = GetLine();
						if (line == null || ContainsString(line, discardText))
						{
							break;
						}
					}
				}

				private string GetLine()
				{
					endIndex = releventStack.IndexOf('\r', startIndex);
					if (endIndex < 0)
					{
						endIndex = length - 1;
					}
					string text = releventStack.Substring(startIndex, endIndex - startIndex);
					char c;
					while (endIndex < length && ((c = releventStack[endIndex]) == '\r' || c == '\n'))
					{
						endIndex++;
					}
					if (startIndex == endIndex)
					{
						return null;
					}
					startIndex = endIndex;
					return text.Replace('\t', ' ');
				}

				public override string ToString()
				{
					return releventStack.Substring(startIndex);
				}

				public void Truncate(int lines)
				{
					string text = "";
					while (lines-- > 0 && startIndex < length)
					{
						text = ((text != null) ? (text + ": " + GetLine()) : GetLine());
						text += Environment.NewLine;
					}
					releventStack = text;
					startIndex = 0;
					endIndex = 0;
					length = releventStack.Length;
				}
			}

			public readonly IntPtr handle;

			public HandleEntry next;

			public readonly string callStack;

			public bool ignorableAsLeak;

			public HandleEntry(HandleEntry next, IntPtr handle)
			{
				this.handle = handle;
				this.next = next;
				if (System.ComponentModel.CompModSwitches.HandleLeak.Level > TraceLevel.Off)
				{
					callStack = Environment.StackTrace;
				}
				else
				{
					callStack = null;
				}
			}

			public string ToString(HandleType type)
			{
				StackParser stackParser = new StackParser(callStack);
				stackParser.DiscardTo("HandleCollector.Add");
				stackParser.DiscardNext();
				stackParser.Truncate(40);
				string str = "";
				return Convert.ToString((int)handle, 16) + str + ": " + stackParser.ToString();
			}
		}

		public readonly string name;

		private int handleCount;

		private HandleEntry[] buckets;

		private const int BUCKETS = 10;

		public HandleType(string name)
		{
			this.name = name;
			buckets = new HandleEntry[10];
		}

		public void Add(IntPtr handle)
		{
			lock (this)
			{
				int num = ComputeHash(handle);
				if (System.ComponentModel.CompModSwitches.HandleLeak.Level >= TraceLevel.Info)
				{
					_ = System.ComponentModel.CompModSwitches.HandleLeak.Level;
					_ = 4;
				}
				for (HandleEntry handleEntry = buckets[num]; handleEntry != null; handleEntry = handleEntry.next)
				{
				}
				buckets[num] = new HandleEntry(buckets[num], handle);
				handleCount++;
			}
		}

		public void CheckLeaks()
		{
			lock (this)
			{
				bool flag = false;
				if (handleCount > 0)
				{
					for (int i = 0; i < 10; i++)
					{
						for (HandleEntry handleEntry = buckets[i]; handleEntry != null; handleEntry = handleEntry.next)
						{
							if (!handleEntry.ignorableAsLeak && !flag)
							{
								flag = true;
							}
						}
					}
				}
			}
		}

		public void IgnoreCurrentHandlesAsLeaks()
		{
			lock (this)
			{
				if (handleCount > 0)
				{
					for (int i = 0; i < 10; i++)
					{
						for (HandleEntry handleEntry = buckets[i]; handleEntry != null; handleEntry = handleEntry.next)
						{
							handleEntry.ignorableAsLeak = true;
						}
					}
				}
			}
		}

		private int ComputeHash(IntPtr handle)
		{
			return ((int)handle & 0xFFFF) % 10;
		}

		public bool Remove(IntPtr handle)
		{
			lock (this)
			{
				int num = ComputeHash(handle);
				if (System.ComponentModel.CompModSwitches.HandleLeak.Level >= TraceLevel.Info)
				{
					_ = System.ComponentModel.CompModSwitches.HandleLeak.Level;
					_ = 4;
				}
				HandleEntry handleEntry = buckets[num];
				HandleEntry handleEntry2 = null;
				while (handleEntry != null && handleEntry.handle != handle)
				{
					handleEntry2 = handleEntry;
					handleEntry = handleEntry.next;
				}
				if (handleEntry != null)
				{
					if (handleEntry2 == null)
					{
						buckets[num] = handleEntry.next;
					}
					else
					{
						handleEntry2.next = handleEntry.next;
					}
					handleCount--;
					return true;
				}
				return false;
			}
		}
	}

	private static Hashtable handleTypes;

	private static System.Internal.DebugHandleTracker tracker;

	private static object internalSyncObject;

	static DebugHandleTracker()
	{
		handleTypes = new Hashtable();
		internalSyncObject = new object();
		tracker = new System.Internal.DebugHandleTracker();
		if (System.ComponentModel.CompModSwitches.HandleLeak.Level > TraceLevel.Off || System.ComponentModel.CompModSwitches.TraceCollect.Enabled)
		{
			System.Internal.HandleCollector.HandleAdded += tracker.OnHandleAdd;
			System.Internal.HandleCollector.HandleRemoved += tracker.OnHandleRemove;
		}
	}

	private DebugHandleTracker()
	{
	}

	public static void IgnoreCurrentHandlesAsLeaks()
	{
		lock (internalSyncObject)
		{
			if (System.ComponentModel.CompModSwitches.HandleLeak.Level >= TraceLevel.Warning)
			{
				HandleType[] array = new HandleType[handleTypes.Values.Count];
				handleTypes.Values.CopyTo(array, 0);
				for (int i = 0; i < array.Length; i++)
				{
					if (array[i] != null)
					{
						array[i].IgnoreCurrentHandlesAsLeaks();
					}
				}
			}
		}
	}

	public static void CheckLeaks()
	{
		lock (internalSyncObject)
		{
			if (System.ComponentModel.CompModSwitches.HandleLeak.Level >= TraceLevel.Warning)
			{
				GC.Collect();
				GC.WaitForPendingFinalizers();
				HandleType[] array = new HandleType[handleTypes.Values.Count];
				handleTypes.Values.CopyTo(array, 0);
				for (int i = 0; i < array.Length; i++)
				{
					if (array[i] != null)
					{
						array[i].CheckLeaks();
					}
				}
			}
		}
	}

	public static void Initialize()
	{
	}

	private void OnHandleAdd(string handleName, IntPtr handle, int handleCount)
	{
		HandleType handleType = (HandleType)handleTypes[handleName];
		if (handleType == null)
		{
			handleType = new HandleType(handleName);
			handleTypes[handleName] = handleType;
		}
		handleType.Add(handle);
	}

	private void OnHandleRemove(string handleName, IntPtr handle, int HandleCount)
	{
		HandleType handleType = (HandleType)handleTypes[handleName];
		bool flag = false;
		if (handleType != null)
		{
			flag = handleType.Remove(handle);
		}
		if (!flag)
		{
			_ = System.ComponentModel.CompModSwitches.HandleLeak.Level;
			_ = 1;
		}
	}
}
