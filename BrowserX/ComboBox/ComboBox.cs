// System.Windows.Forms.ComboBox
using Accessibility;
using System;
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Design;
using System.Globalization;
using System.Runtime.InteropServices;
using System.Security;
using System.Security.Permissions;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using System.Windows.Forms.Internal;
using System.Windows.Forms.Layout;

[ComVisible(true)]
[ClassInterface(ClassInterfaceType.AutoDispatch)]
[DefaultEvent("SelectedIndexChanged")]
[DefaultProperty("Items")]
[DefaultBindingProperty("Text")]
[Designer("System.Windows.Forms.Design.ComboBoxDesigner, System.Design, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a")]
[SRDescription("DescriptionComboBox")]
public class ComboBox : ListControl
{
	[ComVisible(true)]
	internal class ComboBoxChildNativeWindow : NativeWindow
	{
		private ComboBox _owner;

		private InternalAccessibleObject _accessibilityObject;

		private ChildWindowType _childWindowType;

		public ComboBoxChildNativeWindow(ComboBox comboBox, ChildWindowType childWindowType)
		{
			_owner = comboBox;
			_childWindowType = childWindowType;
		}

		protected override void WndProc(ref Message m)
		{
			switch (m.Msg)
			{
			case 61:
				WmGetObject(ref m);
				break;
			case 512:
				if (_childWindowType == ChildWindowType.DropDownList)
				{
					object selectedItem = _owner.SelectedItem;
					DefWndProc(ref m);
					object selectedItem2 = _owner.SelectedItem;
					if (selectedItem != selectedItem2)
					{
						(_owner.AccessibilityObject as ComboBoxUiaProvider).SetComboBoxItemFocus();
					}
				}
				else
				{
					_owner.ChildWndProc(ref m);
				}
				break;
			default:
				if (_childWindowType == ChildWindowType.DropDownList)
				{
					DefWndProc(ref m);
				}
				else
				{
					_owner.ChildWndProc(ref m);
				}
				break;
			}
		}

		private ChildAccessibleObject GetChildAccessibleObject(ChildWindowType childWindowType)
		{
			switch (childWindowType)
			{
			case ChildWindowType.Edit:
				return _owner.ChildEditAccessibleObject;
			case ChildWindowType.ListBox:
			case ChildWindowType.DropDownList:
				return _owner.ChildListAccessibleObject;
			default:
				return new ChildAccessibleObject(_owner, base.Handle);
			}
		}

		private void WmGetObject(ref Message m)
		{
			if (System.AccessibilityImprovements.Level3 && m.LParam == (IntPtr)(-25) && (_childWindowType == ChildWindowType.ListBox || _childWindowType == ChildWindowType.DropDownList))
			{
				AccessibleObject childAccessibleObject = GetChildAccessibleObject(_childWindowType);
				IntSecurity.UnmanagedCode.Assert();
				InternalAccessibleObject el;
				try
				{
					el = new InternalAccessibleObject(childAccessibleObject);
				}
				finally
				{
					CodeAccessPermission.RevertAssert();
				}
				m.Result = UnsafeNativeMethods.UiaReturnRawElementProvider(new HandleRef(this, base.Handle), m.WParam, m.LParam, el);
			}
			else if (-4 == (int)(long)m.LParam)
			{
				Guid refiid = new Guid("{618736E0-3C3D-11CF-810C-00AA00389B71}");
				try
				{
					AccessibleObject accessibleObject = null;
					UnsafeNativeMethods.IAccessibleInternal accessibleInternal = null;
					if (_accessibilityObject == null)
					{
						IntSecurity.UnmanagedCode.Assert();
						try
						{
							accessibleObject = (System.AccessibilityImprovements.Level3 ? GetChildAccessibleObject(_childWindowType) : new ChildAccessibleObject(_owner, base.Handle));
							_accessibilityObject = new InternalAccessibleObject(accessibleObject);
						}
						finally
						{
							CodeAccessPermission.RevertAssert();
						}
					}
					accessibleInternal = _accessibilityObject;
					IntPtr iUnknownForObject = Marshal.GetIUnknownForObject(accessibleInternal);
					IntSecurity.UnmanagedCode.Assert();
					try
					{
						m.Result = UnsafeNativeMethods.LresultFromObject(ref refiid, m.WParam, new HandleRef(this, iUnknownForObject));
					}
					finally
					{
						CodeAccessPermission.RevertAssert();
						Marshal.Release(iUnknownForObject);
					}
				}
				catch (Exception innerException)
				{
					throw new InvalidOperationException(SR.GetString("RichControlLresult"), innerException);
				}
			}
			else
			{
				DefWndProc(ref m);
			}
		}
	}

	private sealed class ItemComparer : IComparer
	{
		private ComboBox comboBox;

		public ItemComparer(ComboBox comboBox)
		{
			this.comboBox = comboBox;
		}

		public int Compare(object item1, object item2)
		{
			if (item1 == null)
			{
				if (item2 == null)
				{
					return 0;
				}
				return -1;
			}
			if (item2 == null)
			{
				return 1;
			}
			string itemText = comboBox.GetItemText(item1);
			string itemText2 = comboBox.GetItemText(item2);
			CompareInfo compareInfo = Application.CurrentCulture.CompareInfo;
			return compareInfo.Compare(itemText, itemText2, CompareOptions.StringSort);
		}
	}

	[ListBindable(false)]
	public class ObjectCollection : IList, ICollection, IEnumerable
	{
		private ComboBox owner;

		private ArrayList innerList;

		private IComparer comparer;

		private IComparer Comparer
		{
			get
			{
				if (comparer == null)
				{
					comparer = new ItemComparer(owner);
				}
				return comparer;
			}
		}

		private ArrayList InnerList
		{
			get
			{
				if (innerList == null)
				{
					innerList = new ArrayList();
				}
				return innerList;
			}
		}

		public int Count => InnerList.Count;

		object ICollection.SyncRoot => this;

		bool ICollection.IsSynchronized => false;

		bool IList.IsFixedSize => false;

		public bool IsReadOnly => false;

		[Browsable(false)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public virtual object this[int index]
		{
			get
			{
				if (index < 0 || index >= InnerList.Count)
				{
					throw new ArgumentOutOfRangeException("index", SR.GetString("InvalidArgument", "index", index.ToString(CultureInfo.CurrentCulture)));
				}
				return InnerList[index];
			}
			set
			{
				owner.CheckNoDataSource();
				SetItemInternal(index, value);
			}
		}

		public ObjectCollection(ComboBox owner)
		{
			this.owner = owner;
		}

		public int Add(object item)
		{
			owner.CheckNoDataSource();
			int result = AddInternal(item);
			if (owner.UpdateNeeded() && owner.AutoCompleteSource == AutoCompleteSource.ListItems)
			{
				owner.SetAutoComplete(reset: false, recreate: false);
			}
			return result;
		}

		private int AddInternal(object item)
		{
			if (item == null)
			{
				throw new ArgumentNullException("item");
			}
			int num = -1;
			if (!owner.sorted)
			{
				InnerList.Add(item);
			}
			else
			{
				num = InnerList.BinarySearch(item, Comparer);
				if (num < 0)
				{
					num = ~num;
				}
				InnerList.Insert(num, item);
			}
			bool flag = false;
			try
			{
				if (owner.sorted)
				{
					if (owner.IsHandleCreated)
					{
						owner.NativeInsert(num, item);
					}
				}
				else
				{
					num = InnerList.Count - 1;
					if (owner.IsHandleCreated)
					{
						owner.NativeAdd(item);
					}
				}
				flag = true;
				return num;
			}
			finally
			{
				if (!flag)
				{
					InnerList.Remove(item);
				}
			}
		}

		int IList.Add(object item)
		{
			return Add(item);
		}

		public void AddRange(object[] items)
		{
			owner.CheckNoDataSource();
			owner.BeginUpdate();
			try
			{
				AddRangeInternal(items);
			}
			finally
			{
				owner.EndUpdate();
			}
		}

		internal void AddRangeInternal(IList items)
		{
			if (items == null)
			{
				throw new ArgumentNullException("items");
			}
			foreach (object item in items)
			{
				AddInternal(item);
			}
			if (owner.AutoCompleteSource == AutoCompleteSource.ListItems)
			{
				owner.SetAutoComplete(reset: false, recreate: false);
			}
		}

		public void Clear()
		{
			owner.CheckNoDataSource();
			ClearInternal();
		}

		internal void ClearInternal()
		{
			if (owner.IsHandleCreated)
			{
				owner.NativeClear();
			}
			InnerList.Clear();
			owner.selectedIndex = -1;
			if (owner.AutoCompleteSource == AutoCompleteSource.ListItems)
			{
				owner.SetAutoComplete(reset: false, recreate: true);
			}
		}

		public bool Contains(object value)
		{
			return IndexOf(value) != -1;
		}

		public void CopyTo(object[] destination, int arrayIndex)
		{
			InnerList.CopyTo(destination, arrayIndex);
		}

		void ICollection.CopyTo(Array destination, int index)
		{
			InnerList.CopyTo(destination, index);
		}

		public IEnumerator GetEnumerator()
		{
			return InnerList.GetEnumerator();
		}

		public int IndexOf(object value)
		{
			if (value == null)
			{
				throw new ArgumentNullException("value");
			}
			return InnerList.IndexOf(value);
		}

		public void Insert(int index, object item)
		{
			owner.CheckNoDataSource();
			if (item == null)
			{
				throw new ArgumentNullException("item");
			}
			if (index < 0 || index > InnerList.Count)
			{
				throw new ArgumentOutOfRangeException("index", SR.GetString("InvalidArgument", "index", index.ToString(CultureInfo.CurrentCulture)));
			}
			if (owner.sorted)
			{
				Add(item);
				return;
			}
			InnerList.Insert(index, item);
			if (owner.IsHandleCreated)
			{
				bool flag = false;
				try
				{
					owner.NativeInsert(index, item);
					flag = true;
				}
				finally
				{
					if (flag)
					{
						if (owner.AutoCompleteSource == AutoCompleteSource.ListItems)
						{
							owner.SetAutoComplete(reset: false, recreate: false);
						}
					}
					else
					{
						InnerList.RemoveAt(index);
					}
				}
			}
		}

		public void RemoveAt(int index)
		{
			owner.CheckNoDataSource();
			if (index < 0 || index >= InnerList.Count)
			{
				throw new ArgumentOutOfRangeException("index", SR.GetString("InvalidArgument", "index", index.ToString(CultureInfo.CurrentCulture)));
			}
			if (owner.IsHandleCreated)
			{
				owner.NativeRemoveAt(index);
			}
			InnerList.RemoveAt(index);
			if (!owner.IsHandleCreated && index < owner.selectedIndex)
			{
				owner.selectedIndex--;
			}
			if (owner.AutoCompleteSource == AutoCompleteSource.ListItems)
			{
				owner.SetAutoComplete(reset: false, recreate: false);
			}
		}

		public void Remove(object value)
		{
			int num = InnerList.IndexOf(value);
			if (num != -1)
			{
				RemoveAt(num);
			}
		}

		internal void SetItemInternal(int index, object value)
		{
			if (value == null)
			{
				throw new ArgumentNullException("value");
			}
			if (index < 0 || index >= InnerList.Count)
			{
				throw new ArgumentOutOfRangeException("index", SR.GetString("InvalidArgument", "index", index.ToString(CultureInfo.CurrentCulture)));
			}
			InnerList[index] = value;
			if (!owner.IsHandleCreated)
			{
				return;
			}
			bool flag = index == owner.SelectedIndex;
			if (string.Compare(owner.GetItemText(value), owner.NativeGetItemText(index), ignoreCase: true, CultureInfo.CurrentCulture) != 0)
			{
				owner.NativeRemoveAt(index);
				owner.NativeInsert(index, value);
				if (flag)
				{
					owner.SelectedIndex = index;
					owner.UpdateText();
				}
				if (owner.AutoCompleteSource == AutoCompleteSource.ListItems)
				{
					owner.SetAutoComplete(reset: false, recreate: false);
				}
			}
			else if (flag)
			{
				owner.OnSelectedItemChanged(EventArgs.Empty);
				owner.OnSelectedIndexChanged(EventArgs.Empty);
			}
		}
	}

	[ComVisible(true)]
	public class ChildAccessibleObject : AccessibleObject
	{
		private ComboBox owner;

		public override string Name => owner.AccessibilityObject.Name;

		[SecurityPermission(SecurityAction.Demand, Flags = SecurityPermissionFlag.UnmanagedCode)]
		public ChildAccessibleObject(ComboBox owner, IntPtr handle)
		{
			this.owner = owner;
			UseStdAccessibleObjects(handle);
		}
	}

	[ComVisible(true)]
	internal class ComboBoxAccessibleObject : ControlAccessibleObject
	{
		private const int COMBOBOX_ACC_ITEM_INDEX = 1;

		public ComboBoxAccessibleObject(Control ownerControl)
			: base(ownerControl)
		{
		}

		internal override string get_accNameInternal(object childID)
		{
			ValidateChildID(ref childID);
			if (childID != null && (int)childID == 1)
			{
				return Name;
			}
			return base.get_accNameInternal(childID);
		}

		internal override string get_accKeyboardShortcutInternal(object childID)
		{
			ValidateChildID(ref childID);
			if (childID != null && (int)childID == 1)
			{
				return KeyboardShortcut;
			}
			return base.get_accKeyboardShortcutInternal(childID);
		}
	}

	[ComVisible(true)]
	internal class ComboBoxExAccessibleObject : ComboBoxAccessibleObject
	{
		private ComboBox ownerItem;

		internal override int[] RuntimeId
		{
			get
			{
				if (ownerItem != null)
				{
					return new int[3]
					{
						42,
						(int)(long)ownerItem.Handle,
						ownerItem.GetHashCode()
					};
				}
				return base.RuntimeId;
			}
		}

		internal override UnsafeNativeMethods.ExpandCollapseState ExpandCollapseState
		{
			get
			{
				if (!ownerItem.DroppedDown)
				{
					return UnsafeNativeMethods.ExpandCollapseState.Collapsed;
				}
				return UnsafeNativeMethods.ExpandCollapseState.Expanded;
			}
		}

		private void ComboBoxDefaultAction(bool expand)
		{
			if (ownerItem.DroppedDown != expand)
			{
				ownerItem.DroppedDown = expand;
			}
		}

		public ComboBoxExAccessibleObject(ComboBox ownerControl)
			: base(ownerControl)
		{
			ownerItem = ownerControl;
		}

		internal override bool IsIAccessibleExSupported()
		{
			if (ownerItem != null)
			{
				return true;
			}
			return base.IsIAccessibleExSupported();
		}

		internal override bool IsPatternSupported(int patternId)
		{
			switch (patternId)
			{
			case 10005:
				if (ownerItem.DropDownStyle == ComboBoxStyle.Simple)
				{
					return false;
				}
				return true;
			case 10002:
				if (ownerItem.DropDownStyle == ComboBoxStyle.DropDownList)
				{
					return System.AccessibilityImprovements.Level3;
				}
				return true;
			default:
				return base.IsPatternSupported(patternId);
			}
		}

		internal override object GetPropertyValue(int propertyID)
		{
			switch (propertyID)
			{
			case 30005:
				return Name;
			case 30028:
				return IsPatternSupported(10005);
			case 30043:
				return IsPatternSupported(10002);
			default:
				return base.GetPropertyValue(propertyID);
			}
		}

		internal override void Expand()
		{
			ComboBoxDefaultAction(expand: true);
		}

		internal override void Collapse()
		{
			ComboBoxDefaultAction(expand: false);
		}
	}

	[ComVisible(true)]
	internal class ComboBoxItemAccessibleObject : AccessibleObject
	{
		private ComboBox _owningComboBox;

		private object _owningItem;

		private IAccessible _systemIAccessible;

		public override Rectangle Bounds
		{
			get
			{
				ChildAccessibleObject childListAccessibleObject = _owningComboBox.ChildListAccessibleObject;
				int currentIndex = GetCurrentIndex();
				Rectangle boundingRectangle = childListAccessibleObject.BoundingRectangle;
				int left = boundingRectangle.Left;
				int y = boundingRectangle.Top + _owningComboBox.ItemHeight * currentIndex;
				int width = boundingRectangle.Width;
				int itemHeight = _owningComboBox.ItemHeight;
				return new Rectangle(left, y, width, itemHeight);
			}
		}

		public override string DefaultAction => _systemIAccessible.get_accDefaultAction((object)GetChildId());

		internal override UnsafeNativeMethods.IRawElementProviderFragmentRoot FragmentRoot => _owningComboBox.AccessibilityObject;

		public override string Help => _systemIAccessible.get_accHelp((object)GetChildId());

		public override string Name
		{
			get
			{
				if (_owningComboBox != null)
				{
					return _owningItem.ToString();
				}
				return base.Name;
			}
			set
			{
				base.Name = value;
			}
		}

		public override AccessibleRole Role => (AccessibleRole)_systemIAccessible.get_accRole((object)GetChildId());

		internal override int[] RuntimeId => new int[4]
		{
			42,
			(int)(long)_owningComboBox.Handle,
			_owningComboBox.GetListNativeWindowRuntimeIdPart(),
			_owningItem.GetHashCode()
		};

		public override AccessibleStates State => (AccessibleStates)_systemIAccessible.get_accState((object)GetChildId());

		internal override bool IsItemSelected => (State & AccessibleStates.Selected) != 0;

		internal override UnsafeNativeMethods.IRawElementProviderSimple ItemSelectionContainer => _owningComboBox.ChildListAccessibleObject;

		public ComboBoxItemAccessibleObject(ComboBox owningComboBox, object owningItem)
		{
			_owningComboBox = owningComboBox;
			_owningItem = owningItem;
			_systemIAccessible = _owningComboBox.ChildListAccessibleObject.GetSystemIAccessibleInternal();
		}

		internal override UnsafeNativeMethods.IRawElementProviderFragment FragmentNavigate(UnsafeNativeMethods.NavigateDirection direction)
		{
			switch (direction)
			{
			case UnsafeNativeMethods.NavigateDirection.Parent:
				return _owningComboBox.ChildListAccessibleObject;
			case UnsafeNativeMethods.NavigateDirection.NextSibling:
			{
				int currentIndex = GetCurrentIndex();
				ComboBoxChildListUiaProvider comboBoxChildListUiaProvider = _owningComboBox.ChildListAccessibleObject as ComboBoxChildListUiaProvider;
				if (comboBoxChildListUiaProvider != null)
				{
					int childFragmentCount2 = comboBoxChildListUiaProvider.GetChildFragmentCount();
					int num2 = currentIndex + 1;
					if (childFragmentCount2 > num2)
					{
						return comboBoxChildListUiaProvider.GetChildFragment(num2);
					}
				}
				break;
			}
			case UnsafeNativeMethods.NavigateDirection.PreviousSibling:
			{
				int currentIndex = GetCurrentIndex();
				ComboBoxChildListUiaProvider comboBoxChildListUiaProvider = _owningComboBox.ChildListAccessibleObject as ComboBoxChildListUiaProvider;
				if (comboBoxChildListUiaProvider != null)
				{
					int childFragmentCount = comboBoxChildListUiaProvider.GetChildFragmentCount();
					int num = currentIndex - 1;
					if (num >= 0)
					{
						return comboBoxChildListUiaProvider.GetChildFragment(num);
					}
				}
				break;
			}
			}
			return base.FragmentNavigate(direction);
		}

		private int GetCurrentIndex()
		{
			return _owningComboBox.Items.IndexOf(_owningItem);
		}

		internal override int GetChildId()
		{
			return GetCurrentIndex() + 1;
		}

		internal override object GetPropertyValue(int propertyID)
		{
			switch (propertyID)
			{
			case 30000:
				return RuntimeId;
			case 30001:
				return BoundingRectangle;
			case 30003:
				return 50007;
			case 30005:
				return Name;
			case 30007:
				return KeyboardShortcut ?? string.Empty;
			case 30008:
				return _owningComboBox.Focused && _owningComboBox.SelectedIndex == GetCurrentIndex();
			case 30009:
				return (State & AccessibleStates.Focusable) == AccessibleStates.Focusable;
			case 30010:
				return _owningComboBox.Enabled;
			case 30013:
				return Help ?? string.Empty;
			case 30016:
				return true;
			case 30017:
				return true;
			case 30019:
				return false;
			case 30022:
				return (State & AccessibleStates.Offscreen) == AccessibleStates.Offscreen;
			case 30036:
				return true;
			case 30079:
				return (State & AccessibleStates.Selected) != 0;
			case 30080:
				return _owningComboBox.ChildListAccessibleObject;
			default:
				return base.GetPropertyValue(propertyID);
			}
		}

		internal override bool IsPatternSupported(int patternId)
		{
			if (patternId == 10018 || patternId == 10000 || patternId == 10010)
			{
				return true;
			}
			return base.IsPatternSupported(patternId);
		}

		internal override void SetFocus()
		{
			RaiseAutomationEvent(20005);
			base.SetFocus();
		}

		internal override void SelectItem()
		{
			_owningComboBox.SelectedIndex = GetCurrentIndex();
			SafeNativeMethods.InvalidateRect(new HandleRef(this, _owningComboBox.GetListHandle()), null, erase: false);
		}

		internal override void AddToSelection()
		{
			SelectItem();
		}

		internal override void RemoveFromSelection()
		{
		}
	}

	internal class ComboBoxItemAccessibleObjectCollection : Hashtable
	{
		private ComboBox _owningComboBoxBox;

		public override object this[object key]
		{
			get
			{
				if (!ContainsKey(key))
				{
					ComboBoxItemAccessibleObject comboBoxItemAccessibleObject2 = (ComboBoxItemAccessibleObject)(base[key] = new ComboBoxItemAccessibleObject(_owningComboBoxBox, key));
				}
				return base[key];
			}
			set
			{
				base[key] = value;
			}
		}

		public ComboBoxItemAccessibleObjectCollection(ComboBox owningComboBoxBox)
		{
			_owningComboBoxBox = owningComboBoxBox;
		}
	}

	[ComVisible(true)]
	internal class ComboBoxUiaProvider : ComboBoxExAccessibleObject
	{
		private ComboBoxChildDropDownButtonUiaProvider _dropDownButtonUiaProvider;

		private ComboBoxItemAccessibleObjectCollection _itemAccessibleObjects;

		private ComboBox _owningComboBox;

		public ComboBoxItemAccessibleObjectCollection ItemAccessibleObjects => _itemAccessibleObjects;

		public ComboBoxChildDropDownButtonUiaProvider DropDownButtonUiaProvider => _dropDownButtonUiaProvider ?? new ComboBoxChildDropDownButtonUiaProvider(_owningComboBox, _owningComboBox.Handle);

		internal override UnsafeNativeMethods.IRawElementProviderFragmentRoot FragmentRoot => this;

		public ComboBoxUiaProvider(ComboBox owningComboBox)
			: base(owningComboBox)
		{
			_owningComboBox = owningComboBox;
			_itemAccessibleObjects = new ComboBoxItemAccessibleObjectCollection(owningComboBox);
		}

		internal override UnsafeNativeMethods.IRawElementProviderFragment FragmentNavigate(UnsafeNativeMethods.NavigateDirection direction)
		{
			switch (direction)
			{
			case UnsafeNativeMethods.NavigateDirection.FirstChild:
				return GetChildFragment(0);
			case UnsafeNativeMethods.NavigateDirection.LastChild:
			{
				int childFragmentCount = GetChildFragmentCount();
				if (childFragmentCount > 0)
				{
					return GetChildFragment(childFragmentCount - 1);
				}
				break;
			}
			}
			return base.FragmentNavigate(direction);
		}

		internal override UnsafeNativeMethods.IRawElementProviderSimple GetOverrideProviderForHwnd(IntPtr hwnd)
		{
			if (hwnd == _owningComboBox.childEdit.Handle)
			{
				return _owningComboBox.ChildEditAccessibleObject;
			}
			if (hwnd == _owningComboBox.childListBox.Handle || hwnd == _owningComboBox.dropDownHandle)
			{
				return _owningComboBox.ChildListAccessibleObject;
			}
			return null;
		}

		internal AccessibleObject GetChildFragment(int index)
		{
			if (_owningComboBox.DropDownStyle == ComboBoxStyle.DropDownList)
			{
				if (index == 0)
				{
					return _owningComboBox.ChildTextAccessibleObject;
				}
				index--;
			}
			if (index == 0 && _owningComboBox.DropDownStyle != 0)
			{
				return DropDownButtonUiaProvider;
			}
			return null;
		}

		internal int GetChildFragmentCount()
		{
			int num = 0;
			if (_owningComboBox.DropDownStyle == ComboBoxStyle.DropDownList)
			{
				num++;
			}
			if (_owningComboBox.DropDownStyle != 0)
			{
				num++;
			}
			return num;
		}

		internal override object GetPropertyValue(int propertyID)
		{
			switch (propertyID)
			{
			case 30003:
				return 50003;
			case 30008:
				return _owningComboBox.Focused;
			case 30020:
				return _owningComboBox.Handle;
			default:
				return base.GetPropertyValue(propertyID);
			}
		}

		internal void ResetListItemAccessibleObjects()
		{
			_itemAccessibleObjects.Clear();
		}

		internal void SetComboBoxItemFocus()
		{
			object selectedItem = _owningComboBox.SelectedItem;
			if (selectedItem != null)
			{
				(ItemAccessibleObjects[selectedItem] as ComboBoxItemAccessibleObject)?.SetFocus();
			}
		}

		internal void SetComboBoxItemSelection()
		{
			object selectedItem = _owningComboBox.SelectedItem;
			if (selectedItem != null)
			{
				(ItemAccessibleObjects[selectedItem] as ComboBoxItemAccessibleObject)?.RaiseAutomationEvent(20012);
			}
		}

		internal override void SetFocus()
		{
			base.SetFocus();
			RaiseAutomationEvent(20005);
		}
	}

	[ComVisible(true)]
	internal class ComboBoxChildEditUiaProvider : ChildAccessibleObject
	{
		private const string COMBO_BOX_EDIT_AUTOMATION_ID = "1001";

		private ComboBox _owner;

		private IntPtr _handle;

		internal override UnsafeNativeMethods.IRawElementProviderFragmentRoot FragmentRoot => _owner.AccessibilityObject;

		internal override UnsafeNativeMethods.IRawElementProviderSimple HostRawElementProvider
		{
			get
			{
				if (System.AccessibilityImprovements.Level3)
				{
					UnsafeNativeMethods.UiaHostProviderFromHwnd(new HandleRef(this, _handle), out UnsafeNativeMethods.IRawElementProviderSimple provider);
					return provider;
				}
				return base.HostRawElementProvider;
			}
		}

		internal override int ProviderOptions => 1;

		internal override int[] RuntimeId => new int[2]
		{
			42,
			GetHashCode()
		};

		public ComboBoxChildEditUiaProvider(ComboBox owner, IntPtr childEditControlhandle)
			: base(owner, childEditControlhandle)
		{
			_owner = owner;
			_handle = childEditControlhandle;
		}

		internal override UnsafeNativeMethods.IRawElementProviderFragment FragmentNavigate(UnsafeNativeMethods.NavigateDirection direction)
		{
			switch (direction)
			{
			case UnsafeNativeMethods.NavigateDirection.Parent:
				return _owner.AccessibilityObject;
			case UnsafeNativeMethods.NavigateDirection.NextSibling:
			{
				if (_owner.DropDownStyle == ComboBoxStyle.Simple)
				{
					return null;
				}
				ComboBoxUiaProvider comboBoxUiaProvider = _owner.AccessibilityObject as ComboBoxUiaProvider;
				if (comboBoxUiaProvider != null)
				{
					int childFragmentCount = comboBoxUiaProvider.GetChildFragmentCount();
					if (childFragmentCount > 1)
					{
						return comboBoxUiaProvider.GetChildFragment(childFragmentCount - 1);
					}
				}
				return null;
			}
			case UnsafeNativeMethods.NavigateDirection.PreviousSibling:
			{
				ComboBoxUiaProvider comboBoxUiaProvider = _owner.AccessibilityObject as ComboBoxUiaProvider;
				if (comboBoxUiaProvider != null)
				{
					AccessibleObject childFragment = comboBoxUiaProvider.GetChildFragment(0);
					if (RuntimeId != childFragment.RuntimeId)
					{
						return childFragment;
					}
				}
				return null;
			}
			default:
				return base.FragmentNavigate(direction);
			}
		}

		internal override object GetPropertyValue(int propertyID)
		{
			switch (propertyID)
			{
			case 30000:
				return RuntimeId;
			case 30001:
				return Bounds;
			case 30003:
				return 50004;
			case 30005:
				return Name;
			case 30007:
				return string.Empty;
			case 30008:
				return _owner.Focused;
			case 30009:
				return (State & AccessibleStates.Focusable) == AccessibleStates.Focusable;
			case 30010:
				return _owner.Enabled;
			case 30011:
				return "1001";
			case 30013:
				return Help ?? string.Empty;
			case 30019:
				return false;
			case 30020:
				return _handle;
			case 30022:
				return false;
			default:
				return base.GetPropertyValue(propertyID);
			}
		}

		internal override bool IsIAccessibleExSupported()
		{
			return true;
		}
	}

	[ComVisible(true)]
	internal class ComboBoxChildListUiaProvider : ChildAccessibleObject
	{
		private const string COMBO_BOX_LIST_AUTOMATION_ID = "1000";

		private ComboBox _owningComboBox;

		private IntPtr _childListControlhandle;

		internal override UnsafeNativeMethods.IRawElementProviderFragmentRoot FragmentRoot => _owningComboBox.AccessibilityObject;

		internal override bool CanSelectMultiple => false;

		internal override bool IsSelectionRequired => true;

		internal override UnsafeNativeMethods.IRawElementProviderSimple HostRawElementProvider
		{
			get
			{
				if (System.AccessibilityImprovements.Level3)
				{
					UnsafeNativeMethods.UiaHostProviderFromHwnd(new HandleRef(this, _childListControlhandle), out UnsafeNativeMethods.IRawElementProviderSimple provider);
					return provider;
				}
				return base.HostRawElementProvider;
			}
		}

		internal override int[] RuntimeId => new int[3]
		{
			42,
			(int)(long)_owningComboBox.Handle,
			_owningComboBox.GetListNativeWindowRuntimeIdPart()
		};

		public override AccessibleStates State
		{
			get
			{
				AccessibleStates accessibleStates = AccessibleStates.Focusable;
				if (_owningComboBox.Focused)
				{
					accessibleStates |= AccessibleStates.Focused;
				}
				return accessibleStates;
			}
		}

		public ComboBoxChildListUiaProvider(ComboBox owningComboBox, IntPtr childListControlhandle)
			: base(owningComboBox, childListControlhandle)
		{
			_owningComboBox = owningComboBox;
			_childListControlhandle = childListControlhandle;
		}

		internal override UnsafeNativeMethods.IRawElementProviderFragment ElementProviderFromPoint(double x, double y)
		{
			if (System.AccessibilityImprovements.Level3)
			{
				IAccessible systemIAccessibleInternal = GetSystemIAccessibleInternal();
				if (systemIAccessibleInternal != null)
				{
					object obj = systemIAccessibleInternal.accHitTest((int)x, (int)y);
					if (obj is int)
					{
						int num = (int)obj;
						return GetChildFragment(num - 1);
					}
					return null;
				}
			}
			return base.ElementProviderFromPoint(x, y);
		}

		internal override UnsafeNativeMethods.IRawElementProviderFragment FragmentNavigate(UnsafeNativeMethods.NavigateDirection direction)
		{
			switch (direction)
			{
			case UnsafeNativeMethods.NavigateDirection.FirstChild:
				return GetChildFragment(0);
			case UnsafeNativeMethods.NavigateDirection.LastChild:
			{
				int childFragmentCount = GetChildFragmentCount();
				if (childFragmentCount > 0)
				{
					return GetChildFragment(childFragmentCount - 1);
				}
				return null;
			}
			default:
				return base.FragmentNavigate(direction);
			}
		}

		public AccessibleObject GetChildFragment(int index)
		{
			if (index < 0 || index >= _owningComboBox.Items.Count)
			{
				return null;
			}
			object key = _owningComboBox.Items[index];
			ComboBoxUiaProvider comboBoxUiaProvider = _owningComboBox.AccessibilityObject as ComboBoxUiaProvider;
			return comboBoxUiaProvider.ItemAccessibleObjects[key] as AccessibleObject;
		}

		public int GetChildFragmentCount()
		{
			return _owningComboBox.Items.Count;
		}

		internal override object GetPropertyValue(int propertyID)
		{
			switch (propertyID)
			{
			case 30000:
				return RuntimeId;
			case 30001:
				return Bounds;
			case 30003:
				return 50008;
			case 30005:
				return Name;
			case 30007:
				return string.Empty;
			case 30008:
				return false;
			case 30009:
				return (State & AccessibleStates.Focusable) == AccessibleStates.Focusable;
			case 30010:
				return _owningComboBox.Enabled;
			case 30011:
				return "1000";
			case 30013:
				return Help ?? string.Empty;
			case 30019:
				return false;
			case 30020:
				return _childListControlhandle;
			case 30022:
				return false;
			case 30037:
				return true;
			case 30060:
				return CanSelectMultiple;
			case 30061:
				return IsSelectionRequired;
			default:
				return base.GetPropertyValue(propertyID);
			}
		}

		internal override UnsafeNativeMethods.IRawElementProviderFragment GetFocus()
		{
			return GetFocused();
		}

		public override AccessibleObject GetFocused()
		{
			int selectedIndex = _owningComboBox.SelectedIndex;
			return GetChildFragment(selectedIndex);
		}

		internal override UnsafeNativeMethods.IRawElementProviderSimple[] GetSelection()
		{
			int selectedIndex = _owningComboBox.SelectedIndex;
			AccessibleObject childFragment = GetChildFragment(selectedIndex);
			if (childFragment != null)
			{
				return new UnsafeNativeMethods.IRawElementProviderSimple[1]
				{
					childFragment
				};
			}
			return new UnsafeNativeMethods.IRawElementProviderSimple[0];
		}

		internal override bool IsPatternSupported(int patternId)
		{
			if (patternId == 10018 || patternId == 10001)
			{
				return true;
			}
			return base.IsPatternSupported(patternId);
		}
	}

	[ComVisible(true)]
	internal class ComboBoxChildTextUiaProvider : AccessibleObject
	{
		private const int COMBOBOX_TEXT_ACC_ITEM_INDEX = 1;

		private ComboBox _owner;

		public override Rectangle Bounds => _owner.AccessibilityObject.Bounds;

		public override string Name
		{
			get
			{
				return _owner.AccessibilityObject.Name ?? string.Empty;
			}
			set
			{
			}
		}

		internal override UnsafeNativeMethods.IRawElementProviderFragmentRoot FragmentRoot => _owner.AccessibilityObject;

		internal override int[] RuntimeId => new int[5]
		{
			42,
			(int)(long)_owner.Handle,
			_owner.GetHashCode(),
			GetHashCode(),
			GetChildId()
		};

		public override AccessibleStates State
		{
			get
			{
				AccessibleStates accessibleStates = AccessibleStates.Focusable;
				if (_owner.Focused)
				{
					accessibleStates |= AccessibleStates.Focused;
				}
				return accessibleStates;
			}
		}

		public ComboBoxChildTextUiaProvider(ComboBox owner)
		{
			_owner = owner;
		}

		internal override int GetChildId()
		{
			return 1;
		}

		internal override UnsafeNativeMethods.IRawElementProviderFragment FragmentNavigate(UnsafeNativeMethods.NavigateDirection direction)
		{
			switch (direction)
			{
			case UnsafeNativeMethods.NavigateDirection.Parent:
				return _owner.AccessibilityObject;
			case UnsafeNativeMethods.NavigateDirection.NextSibling:
			{
				ComboBoxUiaProvider comboBoxUiaProvider = _owner.AccessibilityObject as ComboBoxUiaProvider;
				if (comboBoxUiaProvider != null)
				{
					int childFragmentCount = comboBoxUiaProvider.GetChildFragmentCount();
					if (childFragmentCount > 1)
					{
						return comboBoxUiaProvider.GetChildFragment(childFragmentCount - 1);
					}
				}
				return null;
			}
			case UnsafeNativeMethods.NavigateDirection.PreviousSibling:
			{
				ComboBoxUiaProvider comboBoxUiaProvider = _owner.AccessibilityObject as ComboBoxUiaProvider;
				if (comboBoxUiaProvider != null)
				{
					AccessibleObject childFragment = comboBoxUiaProvider.GetChildFragment(0);
					if (RuntimeId != childFragment.RuntimeId)
					{
						return childFragment;
					}
				}
				return null;
			}
			default:
				return base.FragmentNavigate(direction);
			}
		}

		internal override object GetPropertyValue(int propertyID)
		{
			switch (propertyID)
			{
			case 30000:
				return RuntimeId;
			case 30001:
				return Bounds;
			case 30003:
				return 50020;
			case 30005:
				return Name;
			case 30007:
				return string.Empty;
			case 30008:
				return _owner.Focused;
			case 30009:
				return (State & AccessibleStates.Focusable) == AccessibleStates.Focusable;
			case 30010:
				return _owner.Enabled;
			case 30013:
				return Help ?? string.Empty;
			case 30019:
			case 30022:
				return false;
			default:
				return base.GetPropertyValue(propertyID);
			}
		}
	}

	[ComVisible(true)]
	internal class ComboBoxChildDropDownButtonUiaProvider : AccessibleObject
	{
		private const int COMBOBOX_DROPDOWN_BUTTON_ACC_ITEM_INDEX = 2;

		private ComboBox _owner;

		public override string Name
		{
			get
			{
				return get_accNameInternal(2);
			}
			set
			{
				IAccessible systemIAccessibleInternal = GetSystemIAccessibleInternal();
				systemIAccessibleInternal.set_accName((object)2, value);
			}
		}

		public override Rectangle Bounds
		{
			get
			{
				IAccessible systemIAccessibleInternal = GetSystemIAccessibleInternal();
				systemIAccessibleInternal.accLocation(out int pxLeft, out int pyTop, out int pcxWidth, out int pcyHeight, 2);
				return new Rectangle(pxLeft, pyTop, pcxWidth, pcyHeight);
			}
		}

		public override string DefaultAction
		{
			get
			{
				IAccessible systemIAccessibleInternal = GetSystemIAccessibleInternal();
				return systemIAccessibleInternal.get_accDefaultAction((object)2);
			}
		}

		internal override UnsafeNativeMethods.IRawElementProviderFragmentRoot FragmentRoot => _owner.AccessibilityObject;

		public override string Help
		{
			get
			{
				IAccessible systemIAccessibleInternal = GetSystemIAccessibleInternal();
				return systemIAccessibleInternal.get_accHelp((object)2);
			}
		}

		public override string KeyboardShortcut
		{
			get
			{
				IAccessible systemIAccessibleInternal = GetSystemIAccessibleInternal();
				return systemIAccessibleInternal.get_accKeyboardShortcut((object)2);
			}
		}

		public override AccessibleRole Role
		{
			get
			{
				IAccessible systemIAccessibleInternal = GetSystemIAccessibleInternal();
				return (AccessibleRole)systemIAccessibleInternal.get_accRole((object)2);
			}
		}

		internal override int[] RuntimeId => new int[5]
		{
			42,
			(int)(long)_owner.Handle,
			_owner.GetHashCode(),
			61453,
			2
		};

		public override AccessibleStates State
		{
			get
			{
				IAccessible systemIAccessibleInternal = GetSystemIAccessibleInternal();
				return (AccessibleStates)systemIAccessibleInternal.get_accState((object)2);
			}
		}

		public ComboBoxChildDropDownButtonUiaProvider(ComboBox owner, IntPtr comboBoxControlhandle)
		{
			_owner = owner;
			UseStdAccessibleObjects(comboBoxControlhandle);
		}

		internal override UnsafeNativeMethods.IRawElementProviderFragment FragmentNavigate(UnsafeNativeMethods.NavigateDirection direction)
		{
			switch (direction)
			{
			case UnsafeNativeMethods.NavigateDirection.Parent:
				return _owner.AccessibilityObject;
			case UnsafeNativeMethods.NavigateDirection.PreviousSibling:
			{
				ComboBoxUiaProvider comboBoxUiaProvider = _owner.AccessibilityObject as ComboBoxUiaProvider;
				if (comboBoxUiaProvider != null)
				{
					int childFragmentCount = comboBoxUiaProvider.GetChildFragmentCount();
					if (childFragmentCount > 1)
					{
						return comboBoxUiaProvider.GetChildFragment(childFragmentCount - 1);
					}
				}
				return null;
			}
			default:
				return base.FragmentNavigate(direction);
			}
		}

		internal override int GetChildId()
		{
			return 2;
		}

		internal override object GetPropertyValue(int propertyID)
		{
			switch (propertyID)
			{
			case 30000:
				return RuntimeId;
			case 30001:
				return BoundingRectangle;
			case 30003:
				return 50000;
			case 30005:
				return Name;
			case 30007:
				return KeyboardShortcut;
			case 30008:
				return _owner.Focused;
			case 30009:
				return (State & AccessibleStates.Focusable) == AccessibleStates.Focusable;
			case 30010:
				return _owner.Enabled;
			case 30013:
				return Help ?? string.Empty;
			case 30019:
				return false;
			case 30022:
				return (State & AccessibleStates.Offscreen) == AccessibleStates.Offscreen;
			default:
				return base.GetPropertyValue(propertyID);
			}
		}

		internal override bool IsPatternSupported(int patternId)
		{
			if (patternId == 10018 || patternId == 10000)
			{
				return true;
			}
			return base.IsPatternSupported(patternId);
		}
	}

	private sealed class ACNativeWindow : NativeWindow
	{
		internal static int inWndProcCnt;

		private static Hashtable ACWindows = new Hashtable();

		internal bool Visible => SafeNativeMethods.IsWindowVisible(new HandleRef(this, base.Handle));

		internal static bool AutoCompleteActive
		{
			get
			{
				if (inWndProcCnt > 0)
				{
					return true;
				}
				foreach (object value in ACWindows.Values)
				{
					ACNativeWindow aCNativeWindow = value as ACNativeWindow;
					if (aCNativeWindow != null && aCNativeWindow.Visible)
					{
						return true;
					}
				}
				return false;
			}
		}

		internal ACNativeWindow(IntPtr acHandle)
		{
			AssignHandle(acHandle);
			ACWindows.Add(acHandle, this);
			UnsafeNativeMethods.EnumChildWindows(new HandleRef(this, acHandle), RegisterACWindowRecursive, NativeMethods.NullHandleRef);
		}

		private static bool RegisterACWindowRecursive(IntPtr handle, IntPtr lparam)
		{
			if (!ACWindows.ContainsKey(handle))
			{
				ACNativeWindow aCNativeWindow = new ACNativeWindow(handle);
			}
			return true;
		}

		protected override void WndProc(ref Message m)
		{
			inWndProcCnt++;
			try
			{
				base.WndProc(ref m);
			}
			finally
			{
				inWndProcCnt--;
			}
			if (m.Msg == 130)
			{
				ACWindows.Remove(base.Handle);
			}
		}

		internal static void RegisterACWindow(IntPtr acHandle, bool subclass)
		{
			if (subclass && ACWindows.ContainsKey(acHandle) && ACWindows[acHandle] == null)
			{
				ACWindows.Remove(acHandle);
			}
			if (!ACWindows.ContainsKey(acHandle))
			{
				if (subclass)
				{
					ACNativeWindow aCNativeWindow = new ACNativeWindow(acHandle);
				}
				else
				{
					ACWindows.Add(acHandle, null);
				}
			}
		}

		internal static void ClearNullACWindows()
		{
			ArrayList arrayList = new ArrayList();
			foreach (DictionaryEntry aCWindow in ACWindows)
			{
				if (aCWindow.Value == null)
				{
					arrayList.Add(aCWindow.Key);
				}
			}
			foreach (IntPtr item in arrayList)
			{
				ACWindows.Remove(item);
			}
		}
	}

	private class AutoCompleteDropDownFinder
	{
		private const int MaxClassName = 256;

		private const string AutoCompleteClassName = "Auto-Suggest Dropdown";

		private bool shouldSubClass;

		internal void FindDropDowns()
		{
			FindDropDowns(subclass: true);
		}

		internal void FindDropDowns(bool subclass)
		{
			if (!subclass)
			{
				ACNativeWindow.ClearNullACWindows();
			}
			shouldSubClass = subclass;
			UnsafeNativeMethods.EnumThreadWindows(SafeNativeMethods.GetCurrentThreadId(), Callback, new HandleRef(null, IntPtr.Zero));
		}

		private bool Callback(IntPtr hWnd, IntPtr lParam)
		{
			HandleRef hRef = new HandleRef(null, hWnd);
			if (GetClassName(hRef) == "Auto-Suggest Dropdown")
			{
				ACNativeWindow.RegisterACWindow(hRef.Handle, shouldSubClass);
			}
			return true;
		}

		private static string GetClassName(HandleRef hRef)
		{
			StringBuilder stringBuilder = new StringBuilder(256);
			UnsafeNativeMethods.GetClassName(hRef, stringBuilder, 256);
			return stringBuilder.ToString();
		}
	}

	internal class FlatComboAdapter
	{
		private Rectangle outerBorder;

		private Rectangle innerBorder;

		private Rectangle innerInnerBorder;

		internal Rectangle dropDownRect;

		private Rectangle whiteFillRect;

		private Rectangle clientRect;

		private RightToLeft origRightToLeft;

		private const int WhiteFillRectWidth = 5;

		private static bool isScalingInitialized = false;

		private static int OFFSET_2PIXELS = 2;

		protected static int Offset2Pixels = OFFSET_2PIXELS;

		public FlatComboAdapter(ComboBox comboBox, bool smallButton)
		{
			if ((!isScalingInitialized && System.Windows.Forms.DpiHelper.IsScalingRequired) || System.Windows.Forms.DpiHelper.EnableDpiChangedMessageHandling)
			{
				Offset2Pixels = comboBox.LogicalToDeviceUnits(OFFSET_2PIXELS);
				isScalingInitialized = true;
			}
			clientRect = comboBox.ClientRectangle;
			int horizontalScrollBarArrowWidthForDpi = SystemInformation.GetHorizontalScrollBarArrowWidthForDpi(comboBox.deviceDpi);
			outerBorder = new Rectangle(clientRect.Location, new Size(clientRect.Width - 1, clientRect.Height - 1));
			innerBorder = new Rectangle(outerBorder.X + 1, outerBorder.Y + 1, outerBorder.Width - horizontalScrollBarArrowWidthForDpi - 2, outerBorder.Height - 2);
			innerInnerBorder = new Rectangle(innerBorder.X + 1, innerBorder.Y + 1, innerBorder.Width - 2, innerBorder.Height - 2);
			dropDownRect = new Rectangle(innerBorder.Right + 1, innerBorder.Y, horizontalScrollBarArrowWidthForDpi, innerBorder.Height + 1);
			if (smallButton)
			{
				whiteFillRect = dropDownRect;
				whiteFillRect.Width = 5;
				dropDownRect.X += 5;
				dropDownRect.Width -= 5;
			}
			origRightToLeft = comboBox.RightToLeft;
			if (origRightToLeft == RightToLeft.Yes)
			{
				innerBorder.X = clientRect.Width - innerBorder.Right;
				innerInnerBorder.X = clientRect.Width - innerInnerBorder.Right;
				dropDownRect.X = clientRect.Width - dropDownRect.Right;
				whiteFillRect.X = clientRect.Width - whiteFillRect.Right + 1;
			}
		}

		public bool IsValid(ComboBox combo)
		{
			if (combo.ClientRectangle == clientRect)
			{
				return combo.RightToLeft == origRightToLeft;
			}
			return false;
		}

		public virtual void DrawFlatCombo(ComboBox comboBox, Graphics g)
		{
			if (comboBox.DropDownStyle == ComboBoxStyle.Simple)
			{
				return;
			}
			Color outerBorderColor = GetOuterBorderColor(comboBox);
			Color innerBorderColor = GetInnerBorderColor(comboBox);
			bool flag = comboBox.RightToLeft == RightToLeft.Yes;
			DrawFlatComboDropDown(comboBox, g, dropDownRect);
			if (!LayoutUtils.IsZeroWidthOrHeight(whiteFillRect))
			{
				using (Brush brush = new SolidBrush(innerBorderColor))
				{
					g.FillRectangle(brush, whiteFillRect);
				}
			}
			if (outerBorderColor.IsSystemColor)
			{
				Pen pen = SystemPens.FromSystemColor(outerBorderColor);
				g.DrawRectangle(pen, outerBorder);
				if (flag)
				{
					g.DrawRectangle(pen, new Rectangle(outerBorder.X, outerBorder.Y, dropDownRect.Width + 1, outerBorder.Height));
				}
				else
				{
					g.DrawRectangle(pen, new Rectangle(dropDownRect.X, outerBorder.Y, outerBorder.Right - dropDownRect.X, outerBorder.Height));
				}
			}
			else
			{
				using (Pen pen2 = new Pen(outerBorderColor))
				{
					g.DrawRectangle(pen2, outerBorder);
					if (flag)
					{
						g.DrawRectangle(pen2, new Rectangle(outerBorder.X, outerBorder.Y, dropDownRect.Width + 1, outerBorder.Height));
					}
					else
					{
						g.DrawRectangle(pen2, new Rectangle(dropDownRect.X, outerBorder.Y, outerBorder.Right - dropDownRect.X, outerBorder.Height));
					}
				}
			}
			if (innerBorderColor.IsSystemColor)
			{
				Pen pen3 = SystemPens.FromSystemColor(innerBorderColor);
				g.DrawRectangle(pen3, innerBorder);
				g.DrawRectangle(pen3, innerInnerBorder);
			}
			else
			{
				using (Pen pen4 = new Pen(innerBorderColor))
				{
					g.DrawRectangle(pen4, innerBorder);
					g.DrawRectangle(pen4, innerInnerBorder);
				}
			}
			if (!comboBox.Enabled || comboBox.FlatStyle == FlatStyle.Popup)
			{
				bool focused = comboBox.ContainsFocus || comboBox.MouseIsOver;
				Color popupOuterBorderColor = GetPopupOuterBorderColor(comboBox, focused);
				using (Pen pen5 = new Pen(popupOuterBorderColor))
				{
					Pen pen6 = comboBox.Enabled ? pen5 : SystemPens.Control;
					if (flag)
					{
						g.DrawRectangle(pen6, new Rectangle(outerBorder.X, outerBorder.Y, dropDownRect.Width + 1, outerBorder.Height));
					}
					else
					{
						g.DrawRectangle(pen6, new Rectangle(dropDownRect.X, outerBorder.Y, outerBorder.Right - dropDownRect.X, outerBorder.Height));
					}
					g.DrawRectangle(pen5, outerBorder);
				}
			}
		}

		protected virtual void DrawFlatComboDropDown(ComboBox comboBox, Graphics g, Rectangle dropDownRect)
		{
			g.FillRectangle(SystemBrushes.Control, dropDownRect);
			Brush brush = comboBox.Enabled ? SystemBrushes.ControlText : SystemBrushes.ControlDark;
			Point point = new Point(dropDownRect.Left + dropDownRect.Width / 2, dropDownRect.Top + dropDownRect.Height / 2);
			if (origRightToLeft == RightToLeft.Yes)
			{
				point.X -= dropDownRect.Width % 2;
			}
			else
			{
				point.X += dropDownRect.Width % 2;
			}
			g.FillPolygon(brush, new Point[3]
			{
				new Point(point.X - Offset2Pixels, point.Y - 1),
				new Point(point.X + Offset2Pixels + 1, point.Y - 1),
				new Point(point.X, point.Y + Offset2Pixels)
			});
		}

		protected virtual Color GetOuterBorderColor(ComboBox comboBox)
		{
			if (!comboBox.Enabled)
			{
				return SystemColors.ControlDark;
			}
			return SystemColors.Window;
		}

		protected virtual Color GetPopupOuterBorderColor(ComboBox comboBox, bool focused)
		{
			if (!comboBox.Enabled)
			{
				return SystemColors.ControlDark;
			}
			if (!focused)
			{
				return SystemColors.Window;
			}
			return SystemColors.ControlDark;
		}

		protected virtual Color GetInnerBorderColor(ComboBox comboBox)
		{
			if (!comboBox.Enabled)
			{
				return SystemColors.Control;
			}
			return comboBox.BackColor;
		}

		public void ValidateOwnerDrawRegions(ComboBox comboBox, Rectangle updateRegionBox)
		{
			if (comboBox == null)
			{
				Rectangle r = new Rectangle(0, 0, comboBox.Width, innerBorder.Top);
				Rectangle r2 = new Rectangle(0, innerBorder.Bottom, comboBox.Width, comboBox.Height - innerBorder.Bottom);
				Rectangle r3 = new Rectangle(0, 0, innerBorder.Left, comboBox.Height);
				Rectangle r4 = new Rectangle(innerBorder.Right, 0, comboBox.Width - innerBorder.Right, comboBox.Height);
				if (r.IntersectsWith(updateRegionBox))
				{
					NativeMethods.RECT rect = new NativeMethods.RECT(r);
					SafeNativeMethods.ValidateRect(new HandleRef(comboBox, comboBox.Handle), ref rect);
				}
				if (r2.IntersectsWith(updateRegionBox))
				{
					NativeMethods.RECT rect = new NativeMethods.RECT(r2);
					SafeNativeMethods.ValidateRect(new HandleRef(comboBox, comboBox.Handle), ref rect);
				}
				if (r3.IntersectsWith(updateRegionBox))
				{
					NativeMethods.RECT rect = new NativeMethods.RECT(r3);
					SafeNativeMethods.ValidateRect(new HandleRef(comboBox, comboBox.Handle), ref rect);
				}
				if (r4.IntersectsWith(updateRegionBox))
				{
					NativeMethods.RECT rect = new NativeMethods.RECT(r4);
					SafeNativeMethods.ValidateRect(new HandleRef(comboBox, comboBox.Handle), ref rect);
				}
			}
		}
	}

	internal enum ChildWindowType
	{
		ListBox,
		Edit,
		DropDownList
	}

	private static readonly object EVENT_DROPDOWN = new object();

	private static readonly object EVENT_DRAWITEM = new object();

	private static readonly object EVENT_MEASUREITEM = new object();

	private static readonly object EVENT_SELECTEDINDEXCHANGED = new object();

	private static readonly object EVENT_SELECTIONCHANGECOMMITTED = new object();

	private static readonly object EVENT_SELECTEDITEMCHANGED = new object();

	private static readonly object EVENT_DROPDOWNSTYLE = new object();

	private static readonly object EVENT_TEXTUPDATE = new object();

	private static readonly object EVENT_DROPDOWNCLOSED = new object();

	private static readonly int PropMaxLength = PropertyStore.CreateKey();

	private static readonly int PropItemHeight = PropertyStore.CreateKey();

	private static readonly int PropDropDownWidth = PropertyStore.CreateKey();

	private static readonly int PropDropDownHeight = PropertyStore.CreateKey();

	private static readonly int PropStyle = PropertyStore.CreateKey();

	private static readonly int PropDrawMode = PropertyStore.CreateKey();

	private static readonly int PropMatchingText = PropertyStore.CreateKey();

	private static readonly int PropFlatComboAdapter = PropertyStore.CreateKey();

	private const int DefaultSimpleStyleHeight = 150;

	private const int DefaultDropDownHeight = 106;

	private const int AutoCompleteTimeout = 10000000;

	private bool autoCompleteDroppedDown;

	private FlatStyle flatStyle = FlatStyle.Standard;

	private int updateCount;

	private long autoCompleteTimeStamp;

	private int selectedIndex = -1;

	private bool allowCommit = true;

	private int requestedHeight;

	private ComboBoxChildNativeWindow childDropDown;

	private ComboBoxChildNativeWindow childEdit;

	private ComboBoxChildNativeWindow childListBox;

	private IntPtr dropDownHandle;

	private ObjectCollection itemsCollection;

	private short prefHeightCache = -1;

	private short maxDropDownItems = 8;

	private bool integralHeight = true;

	private bool mousePressed;

	private bool mouseEvents;

	private bool mouseInEdit;

	private bool sorted;

	private bool fireSetFocus = true;

	private bool fireLostFocus = true;

	private bool mouseOver;

	private bool suppressNextWindosPos;

	private bool canFireLostFocus;

	private string currentText = "";

	private string lastTextChangedValue;

	private bool dropDown;

	private AutoCompleteDropDownFinder finder = new AutoCompleteDropDownFinder();

	private bool selectedValueChangedFired;

	private AutoCompleteMode autoCompleteMode;

	private AutoCompleteSource autoCompleteSource = AutoCompleteSource.None;

	private AutoCompleteStringCollection autoCompleteCustomSource;

	private StringSource stringSource;

	private bool fromHandleCreate;

	private ComboBoxChildListUiaProvider childListAccessibleObject;

	private ComboBoxChildEditUiaProvider childEditAccessibleObject;

	private ComboBoxChildTextUiaProvider childTextAccessibleObject;

	private bool dropDownWillBeClosed;

	[DefaultValue(AutoCompleteMode.None)]
	[SRDescription("ComboBoxAutoCompleteModeDescr")]
	[Browsable(true)]
	[EditorBrowsable(EditorBrowsableState.Always)]
	public AutoCompleteMode AutoCompleteMode
	{
		get
		{
			return autoCompleteMode;
		}
		set
		{
			if (!ClientUtils.IsEnumValid(value, (int)value, 0, 3))
			{
				throw new InvalidEnumArgumentException("value", (int)value, typeof(AutoCompleteMode));
			}
			if (DropDownStyle == ComboBoxStyle.DropDownList && AutoCompleteSource != AutoCompleteSource.ListItems && value != 0)
			{
				throw new NotSupportedException(SR.GetString("ComboBoxAutoCompleteModeOnlyNoneAllowed"));
			}
			if (Application.OleRequired() != 0)
			{
				throw new ThreadStateException(SR.GetString("ThreadMustBeSTA"));
			}
			bool reset = false;
			if (autoCompleteMode != 0 && value == AutoCompleteMode.None)
			{
				reset = true;
			}
			autoCompleteMode = value;
			SetAutoComplete(reset, recreate: true);
		}
	}

	[DefaultValue(AutoCompleteSource.None)]
	[SRDescription("ComboBoxAutoCompleteSourceDescr")]
	[Browsable(true)]
	[EditorBrowsable(EditorBrowsableState.Always)]
	public AutoCompleteSource AutoCompleteSource
	{
		get
		{
			return autoCompleteSource;
		}
		set
		{
			if (!ClientUtils.IsEnumValid_NotSequential(value, (int)value, 128, 7, 6, 64, 1, 32, 2, 256, 4))
			{
				throw new InvalidEnumArgumentException("value", (int)value, typeof(AutoCompleteSource));
			}
			if (DropDownStyle == ComboBoxStyle.DropDownList && AutoCompleteMode != 0 && value != AutoCompleteSource.ListItems)
			{
				throw new NotSupportedException(SR.GetString("ComboBoxAutoCompleteSourceOnlyListItemsAllowed"));
			}
			if (Application.OleRequired() != 0)
			{
				throw new ThreadStateException(SR.GetString("ThreadMustBeSTA"));
			}
			if (value != AutoCompleteSource.None && value != AutoCompleteSource.CustomSource && value != AutoCompleteSource.ListItems)
			{
				FileIOPermission fileIOPermission = new FileIOPermission(PermissionState.Unrestricted);
				fileIOPermission.AllFiles = FileIOPermissionAccess.PathDiscovery;
				fileIOPermission.Demand();
			}
			autoCompleteSource = value;
			SetAutoComplete(reset: false, recreate: true);
		}
	}

	[DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
	[Localizable(true)]
	[SRDescription("ComboBoxAutoCompleteCustomSourceDescr")]
	[Editor("System.Windows.Forms.Design.ListControlStringCollectionEditor, System.Design, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a", typeof(UITypeEditor))]
	[Browsable(true)]
	[EditorBrowsable(EditorBrowsableState.Always)]
	public AutoCompleteStringCollection AutoCompleteCustomSource
	{
		get
		{
			if (autoCompleteCustomSource == null)
			{
				autoCompleteCustomSource = new AutoCompleteStringCollection();
				autoCompleteCustomSource.CollectionChanged += OnAutoCompleteCustomSourceChanged;
			}
			return autoCompleteCustomSource;
		}
		set
		{
			if (autoCompleteCustomSource != value)
			{
				if (autoCompleteCustomSource != null)
				{
					autoCompleteCustomSource.CollectionChanged -= OnAutoCompleteCustomSourceChanged;
				}
				autoCompleteCustomSource = value;
				if (autoCompleteCustomSource != null)
				{
					autoCompleteCustomSource.CollectionChanged += OnAutoCompleteCustomSourceChanged;
				}
				SetAutoComplete(reset: false, recreate: true);
			}
		}
	}

	public override Color BackColor
	{
		get
		{
			if (ShouldSerializeBackColor())
			{
				return base.BackColor;
			}
			return SystemColors.Window;
		}
		set
		{
			base.BackColor = value;
		}
	}

	[Browsable(false)]
	[EditorBrowsable(EditorBrowsableState.Never)]
	public override Image BackgroundImage
	{
		get
		{
			return base.BackgroundImage;
		}
		set
		{
			base.BackgroundImage = value;
		}
	}

	[Browsable(false)]
	[EditorBrowsable(EditorBrowsableState.Never)]
	public override ImageLayout BackgroundImageLayout
	{
		get
		{
			return base.BackgroundImageLayout;
		}
		set
		{
			base.BackgroundImageLayout = value;
		}
	}

	internal ChildAccessibleObject ChildEditAccessibleObject
	{
		get
		{
			if (childEditAccessibleObject == null)
			{
				childEditAccessibleObject = new ComboBoxChildEditUiaProvider(this, childEdit.Handle);
			}
			return childEditAccessibleObject;
		}
	}

	internal ChildAccessibleObject ChildListAccessibleObject
	{
		get
		{
			if (childListAccessibleObject == null)
			{
				childListAccessibleObject = new ComboBoxChildListUiaProvider(this, (DropDownStyle == ComboBoxStyle.Simple) ? childListBox.Handle : dropDownHandle);
			}
			return childListAccessibleObject;
		}
	}

	internal AccessibleObject ChildTextAccessibleObject
	{
		get
		{
			if (childTextAccessibleObject == null)
			{
				childTextAccessibleObject = new ComboBoxChildTextUiaProvider(this);
			}
			return childTextAccessibleObject;
		}
	}

	protected override CreateParams CreateParams
	{
		[SecurityPermission(SecurityAction.LinkDemand, Flags = SecurityPermissionFlag.UnmanagedCode)]
		get
		{
			CreateParams createParams = base.CreateParams;
			createParams.ClassName = "COMBOBOX";
			createParams.Style |= 2097728;
			createParams.ExStyle |= 512;
			if (!integralHeight)
			{
				createParams.Style |= 1024;
			}
			switch (DropDownStyle)
			{
			case ComboBoxStyle.Simple:
				createParams.Style |= 1;
				break;
			case ComboBoxStyle.DropDown:
				createParams.Style |= 2;
				createParams.Height = PreferredHeight;
				break;
			case ComboBoxStyle.DropDownList:
				createParams.Style |= 3;
				createParams.Height = PreferredHeight;
				break;
			}
			switch (DrawMode)
			{
			case DrawMode.OwnerDrawFixed:
				createParams.Style |= 16;
				break;
			case DrawMode.OwnerDrawVariable:
				createParams.Style |= 32;
				break;
			}
			return createParams;
		}
	}

	protected override Size DefaultSize => new Size(121, PreferredHeight);

	[SRCategory("CatData")]
	[DefaultValue(null)]
	[RefreshProperties(RefreshProperties.Repaint)]
	[AttributeProvider(typeof(IListSource))]
	[SRDescription("ListControlDataSourceDescr")]
	public new object DataSource
	{
		get
		{
			return base.DataSource;
		}
		set
		{
			base.DataSource = value;
		}
	}

	[SRCategory("CatBehavior")]
	[DefaultValue(DrawMode.Normal)]
	[SRDescription("ComboBoxDrawModeDescr")]
	[RefreshProperties(RefreshProperties.Repaint)]
	public DrawMode DrawMode
	{
		get
		{
			bool found;
			int integer = base.Properties.GetInteger(PropDrawMode, out found);
			if (found)
			{
				return (DrawMode)integer;
			}
			return DrawMode.Normal;
		}
		set
		{
			if (DrawMode != value)
			{
				if (!ClientUtils.IsEnumValid(value, (int)value, 0, 2))
				{
					throw new InvalidEnumArgumentException("value", (int)value, typeof(DrawMode));
				}
				ResetHeightCache();
				base.Properties.SetInteger(PropDrawMode, (int)value);
				RecreateHandle();
			}
		}
	}

	[SRCategory("CatBehavior")]
	[SRDescription("ComboBoxDropDownWidthDescr")]
	public int DropDownWidth
	{
		get
		{
			bool found;
			int integer = base.Properties.GetInteger(PropDropDownWidth, out found);
			if (found)
			{
				return integer;
			}
			return base.Width;
		}
		set
		{
			if (value < 1)
			{
				throw new ArgumentOutOfRangeException("DropDownWidth", SR.GetString("InvalidArgument", "DropDownWidth", value.ToString(CultureInfo.CurrentCulture)));
			}
			if (base.Properties.GetInteger(PropDropDownWidth) != value)
			{
				base.Properties.SetInteger(PropDropDownWidth, value);
				if (base.IsHandleCreated)
				{
					SendMessage(352, value, 0);
				}
			}
		}
	}

	[SRCategory("CatBehavior")]
	[SRDescription("ComboBoxDropDownHeightDescr")]
	[Browsable(true)]
	[EditorBrowsable(EditorBrowsableState.Always)]
	[DefaultValue(106)]
	public int DropDownHeight
	{
		get
		{
			bool found;
			int integer = base.Properties.GetInteger(PropDropDownHeight, out found);
			if (found)
			{
				return integer;
			}
			return 106;
		}
		set
		{
			if (value < 1)
			{
				throw new ArgumentOutOfRangeException("DropDownHeight", SR.GetString("InvalidArgument", "DropDownHeight", value.ToString(CultureInfo.CurrentCulture)));
			}
			if (base.Properties.GetInteger(PropDropDownHeight) != value)
			{
				base.Properties.SetInteger(PropDropDownHeight, value);
				IntegralHeight = false;
			}
		}
	}

	[Browsable(false)]
	[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
	[SRDescription("ComboBoxDroppedDownDescr")]
	public bool DroppedDown
	{
		get
		{
			if (base.IsHandleCreated)
			{
				return (int)(long)SendMessage(343, 0, 0) != 0;
			}
			return false;
		}
		set
		{
			if (!base.IsHandleCreated)
			{
				CreateHandle();
			}
			SendMessage(335, value ? (-1) : 0, 0);
		}
	}

	[SRCategory("CatAppearance")]
	[DefaultValue(FlatStyle.Standard)]
	[Localizable(true)]
	[SRDescription("ComboBoxFlatStyleDescr")]
	public FlatStyle FlatStyle
	{
		get
		{
			return flatStyle;
		}
		set
		{
			if (!ClientUtils.IsEnumValid(value, (int)value, 0, 3))
			{
				throw new InvalidEnumArgumentException("value", (int)value, typeof(FlatStyle));
			}
			flatStyle = value;
			Invalidate();
		}
	}

	public override bool Focused
	{
		get
		{
			if (base.Focused)
			{
				return true;
			}
			IntPtr focus = UnsafeNativeMethods.GetFocus();
			if (focus != IntPtr.Zero)
			{
				if (childEdit == null || !(focus == childEdit.Handle))
				{
					if (childListBox != null)
					{
						return focus == childListBox.Handle;
					}
					return false;
				}
				return true;
			}
			return false;
		}
	}

	public override Color ForeColor
	{
		get
		{
			if (ShouldSerializeForeColor())
			{
				return base.ForeColor;
			}
			return SystemColors.WindowText;
		}
		set
		{
			base.ForeColor = value;
		}
	}

	[SRCategory("CatBehavior")]
	[DefaultValue(true)]
	[Localizable(true)]
	[SRDescription("ComboBoxIntegralHeightDescr")]
	public bool IntegralHeight
	{
		get
		{
			return integralHeight;
		}
		set
		{
			if (integralHeight != value)
			{
				integralHeight = value;
				RecreateHandle();
			}
		}
	}

	[SRCategory("CatBehavior")]
	[Localizable(true)]
	[SRDescription("ComboBoxItemHeightDescr")]
	public int ItemHeight
	{
		get
		{
			DrawMode drawMode = DrawMode;
			if (drawMode == DrawMode.OwnerDrawFixed || drawMode == DrawMode.OwnerDrawVariable || !base.IsHandleCreated)
			{
				bool found;
				int integer = base.Properties.GetInteger(PropItemHeight, out found);
				if (found)
				{
					return integer;
				}
				return base.FontHeight + 2;
			}
			int num = (int)(long)SendMessage(340, 0, 0);
			if (num == -1)
			{
				throw new Win32Exception();
			}
			return num;
		}
		set
		{
			if (value < 1)
			{
				throw new ArgumentOutOfRangeException("ItemHeight", SR.GetString("InvalidArgument", "ItemHeight", value.ToString(CultureInfo.CurrentCulture)));
			}
			ResetHeightCache();
			if (base.Properties.GetInteger(PropItemHeight) != value)
			{
				base.Properties.SetInteger(PropItemHeight, value);
				if (DrawMode != 0)
				{
					UpdateItemHeight();
				}
			}
		}
	}

	[SRCategory("CatData")]
	[DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
	[Localizable(true)]
	[SRDescription("ComboBoxItemsDescr")]
	[Editor("System.Windows.Forms.Design.ListControlStringCollectionEditor, System.Design, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a", typeof(UITypeEditor))]
	[MergableProperty(false)]
	public ObjectCollection Items
	{
		get
		{
			if (itemsCollection == null)
			{
				itemsCollection = new ObjectCollection(this);
			}
			return itemsCollection;
		}
	}

	private string MatchingText
	{
		get
		{
			string text = (string)base.Properties.GetObject(PropMatchingText);
			if (text != null)
			{
				return text;
			}
			return string.Empty;
		}
		set
		{
			if (value != null || base.Properties.ContainsObject(PropMatchingText))
			{
				base.Properties.SetObject(PropMatchingText, value);
			}
		}
	}

	[SRCategory("CatBehavior")]
	[DefaultValue(8)]
	[Localizable(true)]
	[SRDescription("ComboBoxMaxDropDownItemsDescr")]
	public int MaxDropDownItems
	{
		get
		{
			return maxDropDownItems;
		}
		set
		{
			if (value < 1 || value > 100)
			{
				throw new ArgumentOutOfRangeException("MaxDropDownItems", SR.GetString("InvalidBoundArgument", "MaxDropDownItems", value.ToString(CultureInfo.CurrentCulture), 1.ToString(CultureInfo.CurrentCulture), 100.ToString(CultureInfo.CurrentCulture)));
			}
			maxDropDownItems = (short)value;
		}
	}

	public override Size MaximumSize
	{
		get
		{
			return base.MaximumSize;
		}
		set
		{
			base.MaximumSize = new Size(value.Width, 0);
		}
	}

	public override Size MinimumSize
	{
		get
		{
			return base.MinimumSize;
		}
		set
		{
			base.MinimumSize = new Size(value.Width, 0);
		}
	}

	[SRCategory("CatBehavior")]
	[DefaultValue(0)]
	[Localizable(true)]
	[SRDescription("ComboBoxMaxLengthDescr")]
	public int MaxLength
	{
		get
		{
			return base.Properties.GetInteger(PropMaxLength);
		}
		set
		{
			if (value < 0)
			{
				value = 0;
			}
			if (MaxLength != value)
			{
				base.Properties.SetInteger(PropMaxLength, value);
				if (base.IsHandleCreated)
				{
					SendMessage(321, value, 0);
				}
			}
		}
	}

	internal bool MouseIsOver
	{
		get
		{
			return mouseOver;
		}
		set
		{
			if (mouseOver != value)
			{
				mouseOver = value;
				if ((!base.ContainsFocus || !Application.RenderWithVisualStyles) && FlatStyle == FlatStyle.Popup)
				{
					Invalidate();
					Update();
				}
			}
		}
	}

	[Browsable(false)]
	[EditorBrowsable(EditorBrowsableState.Never)]
	[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
	public new Padding Padding
	{
		get
		{
			return base.Padding;
		}
		set
		{
			base.Padding = value;
		}
	}

	[Browsable(false)]
	[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
	[SRDescription("ComboBoxPreferredHeightDescr")]
	public int PreferredHeight
	{
		get
		{
			if (!base.FormattingEnabled)
			{
				prefHeightCache = (short)(TextRenderer.MeasureText(LayoutUtils.TestString, Font, new Size(32767, (int)((double)base.FontHeight * 1.25)), TextFormatFlags.SingleLine).Height + SystemInformation.BorderSize.Height * 8 + Padding.Size.Height);
				return prefHeightCache;
			}
			if (prefHeightCache < 0)
			{
				Size size = TextRenderer.MeasureText(LayoutUtils.TestString, Font, new Size(32767, (int)((double)base.FontHeight * 1.25)), TextFormatFlags.SingleLine);
				if (DropDownStyle == ComboBoxStyle.Simple)
				{
					int num = Items.Count + 1;
					prefHeightCache = (short)(size.Height * num + SystemInformation.BorderSize.Height * 16 + Padding.Size.Height);
				}
				else
				{
					prefHeightCache = (short)GetComboHeight();
				}
			}
			return prefHeightCache;
		}
	}

	[Browsable(false)]
	[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
	[SRDescription("ComboBoxSelectedIndexDescr")]
	public override int SelectedIndex
	{
		get
		{
			if (base.IsHandleCreated)
			{
				return (int)(long)SendMessage(327, 0, 0);
			}
			return selectedIndex;
		}
		set
		{
			if (SelectedIndex != value)
			{
				int num = 0;
				if (itemsCollection != null)
				{
					num = itemsCollection.Count;
				}
				if (value < -1 || value >= num)
				{
					throw new ArgumentOutOfRangeException("SelectedIndex", SR.GetString("InvalidArgument", "SelectedIndex", value.ToString(CultureInfo.CurrentCulture)));
				}
				if (base.IsHandleCreated)
				{
					SendMessage(334, value, 0);
				}
				else
				{
					selectedIndex = value;
				}
				UpdateText();
				if (base.IsHandleCreated)
				{
					OnTextChanged(EventArgs.Empty);
				}
				OnSelectedItemChanged(EventArgs.Empty);
				OnSelectedIndexChanged(EventArgs.Empty);
			}
		}
	}

	[Browsable(false)]
	[Bindable(true)]
	[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
	[SRDescription("ComboBoxSelectedItemDescr")]
	public object SelectedItem
	{
		get
		{
			int num = SelectedIndex;
			if (num != -1)
			{
				return Items[num];
			}
			return null;
		}
		set
		{
			int num = -1;
			if (itemsCollection != null)
			{
				if (value != null)
				{
					num = itemsCollection.IndexOf(value);
				}
				else
				{
					SelectedIndex = -1;
				}
			}
			if (num != -1)
			{
				SelectedIndex = num;
			}
		}
	}

	[Browsable(false)]
	[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
	[SRDescription("ComboBoxSelectedTextDescr")]
	public string SelectedText
	{
		get
		{
			if (DropDownStyle == ComboBoxStyle.DropDownList)
			{
				return "";
			}
			return Text.Substring(SelectionStart, SelectionLength);
		}
		set
		{
			if (DropDownStyle != ComboBoxStyle.DropDownList)
			{
				string lParam = (value == null) ? "" : value;
				CreateControl();
				if (base.IsHandleCreated && childEdit != null)
				{
					UnsafeNativeMethods.SendMessage(new HandleRef(this, childEdit.Handle), 194, NativeMethods.InvalidIntPtr, lParam);
				}
			}
		}
	}

	[Browsable(false)]
	[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
	[SRDescription("ComboBoxSelectionLengthDescr")]
	public int SelectionLength
	{
		get
		{
			int[] array = new int[1];
			int[] array2 = new int[1];
			UnsafeNativeMethods.SendMessage(new HandleRef(this, base.Handle), 320, array2, array);
			return array[0] - array2[0];
		}
		set
		{
			Select(SelectionStart, value);
		}
	}

	[Browsable(false)]
	[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
	[SRDescription("ComboBoxSelectionStartDescr")]
	public int SelectionStart
	{
		get
		{
			int[] array = new int[1];
			UnsafeNativeMethods.SendMessage(new HandleRef(this, base.Handle), 320, array, null);
			return array[0];
		}
		set
		{
			if (value < 0)
			{
				throw new ArgumentOutOfRangeException("SelectionStart", SR.GetString("InvalidArgument", "SelectionStart", value.ToString(CultureInfo.CurrentCulture)));
			}
			Select(value, SelectionLength);
		}
	}

	[SRCategory("CatBehavior")]
	[DefaultValue(false)]
	[SRDescription("ComboBoxSortedDescr")]
	public bool Sorted
	{
		get
		{
			return sorted;
		}
		set
		{
			if (sorted != value)
			{
				if (DataSource != null && value)
				{
					throw new ArgumentException(SR.GetString("ComboBoxSortWithDataSource"));
				}
				sorted = value;
				RefreshItems();
				SelectedIndex = -1;
			}
		}
	}

	[SRCategory("CatAppearance")]
	[DefaultValue(ComboBoxStyle.DropDown)]
	[SRDescription("ComboBoxStyleDescr")]
	[RefreshProperties(RefreshProperties.Repaint)]
	public ComboBoxStyle DropDownStyle
	{
		get
		{
			bool found;
			int integer = base.Properties.GetInteger(PropStyle, out found);
			if (found)
			{
				return (ComboBoxStyle)integer;
			}
			return ComboBoxStyle.DropDown;
		}
		set
		{
			if (DropDownStyle != value)
			{
				if (!ClientUtils.IsEnumValid(value, (int)value, 0, 2))
				{
					throw new InvalidEnumArgumentException("value", (int)value, typeof(ComboBoxStyle));
				}
				if (value == ComboBoxStyle.DropDownList && AutoCompleteSource != AutoCompleteSource.ListItems && AutoCompleteMode != 0)
				{
					AutoCompleteMode = AutoCompleteMode.None;
				}
				ResetHeightCache();
				base.Properties.SetInteger(PropStyle, (int)value);
				if (base.IsHandleCreated)
				{
					RecreateHandle();
				}
				OnDropDownStyleChanged(EventArgs.Empty);
			}
		}
	}

	[Localizable(true)]
	[Bindable(true)]
	public override string Text
	{
		get
		{
			if (SelectedItem != null && !base.BindingFieldEmpty)
			{
				if (!base.FormattingEnabled)
				{
					return FilterItemOnProperty(SelectedItem).ToString();
				}
				string itemText = GetItemText(SelectedItem);
				if (!string.IsNullOrEmpty(itemText) && string.Compare(itemText, base.Text, ignoreCase: true, CultureInfo.CurrentCulture) == 0)
				{
					return itemText;
				}
			}
			return base.Text;
		}
		set
		{
			if (DropDownStyle == ComboBoxStyle.DropDownList && !base.IsHandleCreated && !string.IsNullOrEmpty(value) && FindStringExact(value) == -1)
			{
				return;
			}
			base.Text = value;
			object obj = null;
			obj = SelectedItem;
			if (base.DesignMode)
			{
				return;
			}
			if (value == null)
			{
				SelectedIndex = -1;
			}
			else if (value != null && (obj == null || string.Compare(value, GetItemText(obj), ignoreCase: false, CultureInfo.CurrentCulture) != 0))
			{
				int num = FindStringIgnoreCase(value);
				if (num != -1)
				{
					SelectedIndex = num;
				}
			}
		}
	}

	internal override bool SupportsUiaProviders
	{
		get
		{
			if (System.AccessibilityImprovements.Level3)
			{
				return !base.DesignMode;
			}
			return false;
		}
	}

	private bool SystemAutoCompleteEnabled
	{
		get
		{
			if (autoCompleteMode != 0)
			{
				return DropDownStyle != ComboBoxStyle.DropDownList;
			}
			return false;
		}
	}

	private FlatComboAdapter FlatComboBoxAdapter
	{
		get
		{
			FlatComboAdapter flatComboAdapter = base.Properties.GetObject(PropFlatComboAdapter) as FlatComboAdapter;
			if (flatComboAdapter == null || !flatComboAdapter.IsValid(this))
			{
				flatComboAdapter = CreateFlatComboAdapterInstance();
				base.Properties.SetObject(PropFlatComboAdapter, flatComboAdapter);
			}
			return flatComboAdapter;
		}
	}

	[Browsable(false)]
	[EditorBrowsable(EditorBrowsableState.Never)]
	public new event EventHandler BackgroundImageChanged
	{
		add
		{
			base.BackgroundImageChanged += value;
		}
		remove
		{
			base.BackgroundImageChanged -= value;
		}
	}

	[Browsable(false)]
	[EditorBrowsable(EditorBrowsableState.Never)]
	public new event EventHandler BackgroundImageLayoutChanged
	{
		add
		{
			base.BackgroundImageLayoutChanged += value;
		}
		remove
		{
			base.BackgroundImageLayoutChanged -= value;
		}
	}

	[Browsable(false)]
	[EditorBrowsable(EditorBrowsableState.Never)]
	public new event EventHandler PaddingChanged
	{
		add
		{
			base.PaddingChanged += value;
		}
		remove
		{
			base.PaddingChanged -= value;
		}
	}

	[Browsable(false)]
	[EditorBrowsable(EditorBrowsableState.Never)]
	public new event EventHandler DoubleClick
	{
		add
		{
			base.DoubleClick += value;
		}
		remove
		{
			base.DoubleClick -= value;
		}
	}

	[SRCategory("CatBehavior")]
	[SRDescription("drawItemEventDescr")]
	public event DrawItemEventHandler DrawItem
	{
		add
		{
			base.Events.AddHandler(EVENT_DRAWITEM, value);
		}
		remove
		{
			base.Events.RemoveHandler(EVENT_DRAWITEM, value);
		}
	}

	[SRCategory("CatBehavior")]
	[SRDescription("ComboBoxOnDropDownDescr")]
	public event EventHandler DropDown
	{
		add
		{
			base.Events.AddHandler(EVENT_DROPDOWN, value);
		}
		remove
		{
			base.Events.RemoveHandler(EVENT_DROPDOWN, value);
		}
	}

	[SRCategory("CatBehavior")]
	[SRDescription("measureItemEventDescr")]
	public event MeasureItemEventHandler MeasureItem
	{
		add
		{
			base.Events.AddHandler(EVENT_MEASUREITEM, value);
			UpdateItemHeight();
		}
		remove
		{
			base.Events.RemoveHandler(EVENT_MEASUREITEM, value);
			UpdateItemHeight();
		}
	}

	[SRCategory("CatBehavior")]
	[SRDescription("selectedIndexChangedEventDescr")]
	public event EventHandler SelectedIndexChanged
	{
		add
		{
			base.Events.AddHandler(EVENT_SELECTEDINDEXCHANGED, value);
		}
		remove
		{
			base.Events.RemoveHandler(EVENT_SELECTEDINDEXCHANGED, value);
		}
	}

	[SRCategory("CatBehavior")]
	[SRDescription("selectionChangeCommittedEventDescr")]
	public event EventHandler SelectionChangeCommitted
	{
		add
		{
			base.Events.AddHandler(EVENT_SELECTIONCHANGECOMMITTED, value);
		}
		remove
		{
			base.Events.RemoveHandler(EVENT_SELECTIONCHANGECOMMITTED, value);
		}
	}

	[SRCategory("CatBehavior")]
	[SRDescription("ComboBoxDropDownStyleChangedDescr")]
	public event EventHandler DropDownStyleChanged
	{
		add
		{
			base.Events.AddHandler(EVENT_DROPDOWNSTYLE, value);
		}
		remove
		{
			base.Events.RemoveHandler(EVENT_DROPDOWNSTYLE, value);
		}
	}

	[Browsable(false)]
	[EditorBrowsable(EditorBrowsableState.Never)]
	public new event PaintEventHandler Paint
	{
		add
		{
			base.Paint += value;
		}
		remove
		{
			base.Paint -= value;
		}
	}

	[SRCategory("CatBehavior")]
	[SRDescription("ComboBoxOnTextUpdateDescr")]
	public event EventHandler TextUpdate
	{
		add
		{
			base.Events.AddHandler(EVENT_TEXTUPDATE, value);
		}
		remove
		{
			base.Events.RemoveHandler(EVENT_TEXTUPDATE, value);
		}
	}

	[SRCategory("CatBehavior")]
	[SRDescription("ComboBoxOnDropDownClosedDescr")]
	public event EventHandler DropDownClosed
	{
		add
		{
			base.Events.AddHandler(EVENT_DROPDOWNCLOSED, value);
		}
		remove
		{
			base.Events.RemoveHandler(EVENT_DROPDOWNCLOSED, value);
		}
	}

	public ComboBox()
	{
		SetStyle(ControlStyles.UserPaint | ControlStyles.StandardClick | ControlStyles.UseTextForAccessibility, value: false);
		requestedHeight = 150;
		SetState2(2048, value: true);
	}

	private int GetComboHeight()
	{
		int num = 0;
		Size size = Size.Empty;
		using (WindowsFont font = WindowsFont.FromFont(Font))
		{
			size = WindowsGraphicsCacheManager.MeasurementGraphics.GetTextExtent("0", font);
		}
		int num2 = size.Height + SystemInformation.Border3DSize.Height;
		if (DrawMode != 0)
		{
			num2 = ItemHeight;
		}
		return 2 * SystemInformation.FixedFrameBorderSize.Height + num2;
	}

	private string[] GetStringsForAutoComplete(IList collection)
	{
		if (collection is AutoCompleteStringCollection)
		{
			string[] array = new string[AutoCompleteCustomSource.Count];
			for (int i = 0; i < AutoCompleteCustomSource.Count; i++)
			{
				array[i] = AutoCompleteCustomSource[i];
			}
			return array;
		}
		if (collection is ObjectCollection)
		{
			string[] array2 = new string[itemsCollection.Count];
			for (int j = 0; j < itemsCollection.Count; j++)
			{
				array2[j] = GetItemText(itemsCollection[j]);
			}
			return array2;
		}
		return new string[0];
	}

	private int FindStringIgnoreCase(string value)
	{
		int num = FindStringExact(value, -1, ignorecase: false);
		if (num == -1)
		{
			num = FindStringExact(value, -1, ignorecase: true);
		}
		return num;
	}

	private void NotifyAutoComplete()
	{
		NotifyAutoComplete(setSelectedIndex: true);
	}

	private void NotifyAutoComplete(bool setSelectedIndex)
	{
		string text = Text;
		bool flag = text != lastTextChangedValue;
		bool flag2 = false;
		if (setSelectedIndex)
		{
			int num = FindStringIgnoreCase(text);
			if (num != -1 && num != SelectedIndex)
			{
				SelectedIndex = num;
				SelectionStart = 0;
				SelectionLength = text.Length;
				flag2 = true;
			}
		}
		if (flag && !flag2)
		{
			OnTextChanged(EventArgs.Empty);
		}
		lastTextChangedValue = text;
	}

	[Obsolete("This method has been deprecated.  There is no replacement.  http://go.microsoft.com/fwlink/?linkid=14202")]
	protected virtual void AddItemsCore(object[] value)
	{
		if (value != null && value.Length != 0)
		{
			BeginUpdate();
			try
			{
				Items.AddRangeInternal(value);
			}
			finally
			{
				EndUpdate();
			}
		}
	}

	public void BeginUpdate()
	{
		updateCount++;
		BeginUpdateInternal();
	}

	private void CheckNoDataSource()
	{
		if (DataSource != null)
		{
			throw new ArgumentException(SR.GetString("DataSourceLocksItems"));
		}
	}

	protected override AccessibleObject CreateAccessibilityInstance()
	{
		if (System.AccessibilityImprovements.Level3)
		{
			return new ComboBoxUiaProvider(this);
		}
		if (System.AccessibilityImprovements.Level1)
		{
			return new ComboBoxExAccessibleObject(this);
		}
		return new ComboBoxAccessibleObject(this);
	}

	internal bool UpdateNeeded()
	{
		return updateCount == 0;
	}

	internal Point EditToComboboxMapping(Message m)
	{
		if (childEdit == null)
		{
			return new Point(0, 0);
		}
		NativeMethods.RECT rect = default(NativeMethods.RECT);
		UnsafeNativeMethods.GetWindowRect(new HandleRef(this, base.Handle), ref rect);
		NativeMethods.RECT rect2 = default(NativeMethods.RECT);
		UnsafeNativeMethods.GetWindowRect(new HandleRef(this, childEdit.Handle), ref rect2);
		int x = NativeMethods.Util.SignedLOWORD(m.LParam) + (rect2.left - rect.left);
		int y = NativeMethods.Util.SignedHIWORD(m.LParam) + (rect2.top - rect.top);
		return new Point(x, y);
	}

	private void ChildWndProc(ref Message m)
	{
		switch (m.Msg)
		{
		case 258:
			if (DropDownStyle == ComboBoxStyle.Simple && m.HWnd == childListBox.Handle)
			{
				DefChildWndProc(ref m);
			}
			else if (PreProcessControlMessage(ref m) != 0 && !ProcessKeyMessage(ref m))
			{
				DefChildWndProc(ref m);
			}
			break;
		case 262:
			if (DropDownStyle == ComboBoxStyle.Simple && m.HWnd == childListBox.Handle)
			{
				DefChildWndProc(ref m);
			}
			else if (PreProcessControlMessage(ref m) != 0 && !ProcessKeyEventArgs(ref m))
			{
				DefChildWndProc(ref m);
			}
			break;
		case 256:
		case 260:
			if (SystemAutoCompleteEnabled && !ACNativeWindow.AutoCompleteActive)
			{
				finder.FindDropDowns(subclass: false);
			}
			if (AutoCompleteMode != 0)
			{
				switch ((ushort)(long)m.WParam)
				{
				case 27:
					DroppedDown = false;
					break;
				case 13:
					if (DroppedDown)
					{
						UpdateText();
						OnSelectionChangeCommittedInternal(EventArgs.Empty);
						DroppedDown = false;
					}
					break;
				}
			}
			if (DropDownStyle == ComboBoxStyle.Simple && m.HWnd == childListBox.Handle)
			{
				DefChildWndProc(ref m);
			}
			else if (PreProcessControlMessage(ref m) != 0 && !ProcessKeyMessage(ref m))
			{
				DefChildWndProc(ref m);
			}
			break;
		case 81:
			DefChildWndProc(ref m);
			break;
		case 257:
		case 261:
			if (DropDownStyle == ComboBoxStyle.Simple && m.HWnd == childListBox.Handle)
			{
				DefChildWndProc(ref m);
			}
			else if (PreProcessControlMessage(ref m) != 0)
			{
				if (ProcessKeyMessage(ref m))
				{
					break;
				}
				DefChildWndProc(ref m);
			}
			if (SystemAutoCompleteEnabled && !ACNativeWindow.AutoCompleteActive)
			{
				finder.FindDropDowns();
			}
			break;
		case 8:
			if (!base.DesignMode)
			{
				OnImeContextStatusChanged(m.HWnd);
			}
			DefChildWndProc(ref m);
			if (fireLostFocus)
			{
				InvokeLostFocus(this, EventArgs.Empty);
			}
			if (FlatStyle == FlatStyle.Popup)
			{
				Invalidate();
			}
			break;
		case 7:
			if (!base.DesignMode)
			{
				ImeContext.SetImeStatus(base.CachedImeMode, m.HWnd);
			}
			if (!base.HostedInWin32DialogManager)
			{
				IContainerControl containerControlInternal = GetContainerControlInternal();
				if (containerControlInternal != null)
				{
					ContainerControl containerControl = containerControlInternal as ContainerControl;
					if (containerControl != null && !containerControl.ActivateControlInternal(this, originator: false))
					{
						break;
					}
				}
			}
			DefChildWndProc(ref m);
			if (fireSetFocus)
			{
				InvokeGotFocus(this, EventArgs.Empty);
			}
			if (FlatStyle == FlatStyle.Popup)
			{
				Invalidate();
			}
			break;
		case 48:
			DefChildWndProc(ref m);
			if (childEdit != null && m.HWnd == childEdit.Handle)
			{
				UnsafeNativeMethods.SendMessage(new HandleRef(this, childEdit.Handle), 211, 3, 0);
			}
			break;
		case 515:
		{
			mousePressed = true;
			mouseEvents = true;
			base.CaptureInternal = true;
			DefChildWndProc(ref m);
			Point point7 = EditToComboboxMapping(m);
			OnMouseDown(new MouseEventArgs(MouseButtons.Left, 1, point7.X, point7.Y, 0));
			break;
		}
		case 521:
		{
			mousePressed = true;
			mouseEvents = true;
			base.CaptureInternal = true;
			DefChildWndProc(ref m);
			Point point4 = EditToComboboxMapping(m);
			OnMouseDown(new MouseEventArgs(MouseButtons.Middle, 1, point4.X, point4.Y, 0));
			break;
		}
		case 518:
		{
			mousePressed = true;
			mouseEvents = true;
			base.CaptureInternal = true;
			DefChildWndProc(ref m);
			Point point5 = EditToComboboxMapping(m);
			OnMouseDown(new MouseEventArgs(MouseButtons.Right, 1, point5.X, point5.Y, 0));
			break;
		}
		case 513:
		{
			mousePressed = true;
			mouseEvents = true;
			base.CaptureInternal = true;
			DefChildWndProc(ref m);
			Point point2 = EditToComboboxMapping(m);
			OnMouseDown(new MouseEventArgs(MouseButtons.Left, 1, point2.X, point2.Y, 0));
			break;
		}
		case 514:
		{
			NativeMethods.RECT rect = default(NativeMethods.RECT);
			UnsafeNativeMethods.GetWindowRect(new HandleRef(this, base.Handle), ref rect);
			Rectangle rectangle = new Rectangle(rect.left, rect.top, rect.right - rect.left, rect.bottom - rect.top);
			int x = NativeMethods.Util.SignedLOWORD(m.LParam);
			int y = NativeMethods.Util.SignedHIWORD(m.LParam);
			Point p = new Point(x, y);
			p = PointToScreen(p);
			if (mouseEvents && !base.ValidationCancelled)
			{
				mouseEvents = false;
				if (mousePressed)
				{
					if (rectangle.Contains(p))
					{
						mousePressed = false;
						OnClick(new MouseEventArgs(MouseButtons.Left, 1, NativeMethods.Util.SignedLOWORD(m.LParam), NativeMethods.Util.SignedHIWORD(m.LParam), 0));
						OnMouseClick(new MouseEventArgs(MouseButtons.Left, 1, NativeMethods.Util.SignedLOWORD(m.LParam), NativeMethods.Util.SignedHIWORD(m.LParam), 0));
					}
					else
					{
						mousePressed = false;
						mouseInEdit = false;
						OnMouseLeave(EventArgs.Empty);
					}
				}
			}
			DefChildWndProc(ref m);
			base.CaptureInternal = false;
			p = EditToComboboxMapping(m);
			OnMouseUp(new MouseEventArgs(MouseButtons.Left, 1, p.X, p.Y, 0));
			break;
		}
		case 519:
		{
			mousePressed = true;
			mouseEvents = true;
			base.CaptureInternal = true;
			DefChildWndProc(ref m);
			Point point8 = EditToComboboxMapping(m);
			OnMouseDown(new MouseEventArgs(MouseButtons.Middle, 1, point8.X, point8.Y, 0));
			break;
		}
		case 516:
		{
			mousePressed = true;
			mouseEvents = true;
			if (ContextMenu != null || ContextMenuStrip != null)
			{
				base.CaptureInternal = true;
			}
			DefChildWndProc(ref m);
			Point point6 = EditToComboboxMapping(m);
			OnMouseDown(new MouseEventArgs(MouseButtons.Right, 1, point6.X, point6.Y, 0));
			break;
		}
		case 520:
			mousePressed = false;
			mouseEvents = false;
			base.CaptureInternal = false;
			DefChildWndProc(ref m);
			OnMouseUp(new MouseEventArgs(MouseButtons.Middle, 1, NativeMethods.Util.SignedLOWORD(m.LParam), NativeMethods.Util.SignedHIWORD(m.LParam), 0));
			break;
		case 517:
		{
			mousePressed = false;
			mouseEvents = false;
			if (ContextMenu != null)
			{
				base.CaptureInternal = false;
			}
			DefChildWndProc(ref m);
			Point point3 = EditToComboboxMapping(m);
			OnMouseUp(new MouseEventArgs(MouseButtons.Right, 1, point3.X, point3.Y, 0));
			break;
		}
		case 123:
			if (ContextMenu != null || ContextMenuStrip != null)
			{
				UnsafeNativeMethods.SendMessage(new HandleRef(this, base.Handle), 123, m.WParam, m.LParam);
			}
			else
			{
				DefChildWndProc(ref m);
			}
			break;
		case 512:
		{
			Point point = EditToComboboxMapping(m);
			DefChildWndProc(ref m);
			OnMouseEnterInternal(EventArgs.Empty);
			OnMouseMove(new MouseEventArgs(Control.MouseButtons, 0, point.X, point.Y, 0));
			break;
		}
		case 32:
			if (Cursor != DefaultCursor && childEdit != null && m.HWnd == childEdit.Handle && NativeMethods.Util.LOWORD(m.LParam) == 1)
			{
				Cursor.CurrentInternal = Cursor;
			}
			else
			{
				DefChildWndProc(ref m);
			}
			break;
		case 675:
			DefChildWndProc(ref m);
			OnMouseLeaveInternal(EventArgs.Empty);
			break;
		default:
			DefChildWndProc(ref m);
			break;
		}
	}

	private void OnMouseEnterInternal(EventArgs args)
	{
		if (!mouseInEdit)
		{
			OnMouseEnter(args);
			mouseInEdit = true;
		}
	}

	private void OnMouseLeaveInternal(EventArgs args)
	{
		NativeMethods.RECT rect = default(NativeMethods.RECT);
		UnsafeNativeMethods.GetWindowRect(new HandleRef(this, base.Handle), ref rect);
		Rectangle rectangle = new Rectangle(rect.left, rect.top, rect.right - rect.left, rect.bottom - rect.top);
		Point mousePosition = Control.MousePosition;
		if (!rectangle.Contains(mousePosition))
		{
			OnMouseLeave(args);
			mouseInEdit = false;
		}
	}

	private void DefChildWndProc(ref Message m)
	{
		if (childEdit != null)
		{
			((m.HWnd == childEdit.Handle) ? childEdit : ((!System.AccessibilityImprovements.Level3 || !(m.HWnd == dropDownHandle)) ? childListBox : childDropDown))?.DefWndProc(ref m);
		}
	}

	protected override void Dispose(bool disposing)
	{
		if (disposing)
		{
			if (autoCompleteCustomSource != null)
			{
				autoCompleteCustomSource.CollectionChanged -= OnAutoCompleteCustomSourceChanged;
			}
			if (stringSource != null)
			{
				stringSource.ReleaseAutoComplete();
				stringSource = null;
			}
		}
		base.Dispose(disposing);
	}

	public void EndUpdate()
	{
		updateCount--;
		if (updateCount == 0 && AutoCompleteSource == AutoCompleteSource.ListItems)
		{
			SetAutoComplete(reset: false, recreate: false);
		}
		if (EndUpdateInternal())
		{
			if (childEdit != null && childEdit.Handle != IntPtr.Zero)
			{
				SafeNativeMethods.InvalidateRect(new HandleRef(this, childEdit.Handle), null, erase: false);
			}
			if (childListBox != null && childListBox.Handle != IntPtr.Zero)
			{
				SafeNativeMethods.InvalidateRect(new HandleRef(this, childListBox.Handle), null, erase: false);
			}
		}
	}

	public int FindString(string s)
	{
		return FindString(s, -1);
	}

	public int FindString(string s, int startIndex)
	{
		if (s == null)
		{
			return -1;
		}
		if (itemsCollection == null || itemsCollection.Count == 0)
		{
			return -1;
		}
		if (startIndex < -1 || startIndex >= itemsCollection.Count)
		{
			throw new ArgumentOutOfRangeException("startIndex");
		}
		return FindStringInternal(s, Items, startIndex, exact: false);
	}

	public int FindStringExact(string s)
	{
		return FindStringExact(s, -1, ignorecase: true);
	}

	public int FindStringExact(string s, int startIndex)
	{
		return FindStringExact(s, startIndex, ignorecase: true);
	}

	internal int FindStringExact(string s, int startIndex, bool ignorecase)
	{
		if (s == null)
		{
			return -1;
		}
		if (itemsCollection == null || itemsCollection.Count == 0)
		{
			return -1;
		}
		if (startIndex < -1 || startIndex >= itemsCollection.Count)
		{
			throw new ArgumentOutOfRangeException("startIndex");
		}
		return FindStringInternal(s, Items, startIndex, exact: true, ignorecase);
	}

	internal override Rectangle ApplyBoundsConstraints(int suggestedX, int suggestedY, int proposedWidth, int proposedHeight)
	{
		if (DropDownStyle == ComboBoxStyle.DropDown || DropDownStyle == ComboBoxStyle.DropDownList)
		{
			proposedHeight = PreferredHeight;
		}
		return base.ApplyBoundsConstraints(suggestedX, suggestedY, proposedWidth, proposedHeight);
	}

	protected override void ScaleControl(SizeF factor, BoundsSpecified specified)
	{
		if (factor.Width != 1f && factor.Height != 1f)
		{
			ResetHeightCache();
		}
		base.ScaleControl(factor, specified);
	}

	public int GetItemHeight(int index)
	{
		if (DrawMode != DrawMode.OwnerDrawVariable)
		{
			return ItemHeight;
		}
		if (index < 0 || itemsCollection == null || index >= itemsCollection.Count)
		{
			throw new ArgumentOutOfRangeException("index", SR.GetString("InvalidArgument", "index", index.ToString(CultureInfo.CurrentCulture)));
		}
		if (base.IsHandleCreated)
		{
			int num = (int)(long)SendMessage(340, index, 0);
			if (num == -1)
			{
				throw new Win32Exception();
			}
			return num;
		}
		return ItemHeight;
	}

	internal IntPtr GetListHandle()
	{
		if (DropDownStyle != 0)
		{
			return dropDownHandle;
		}
		return childListBox.Handle;
	}

	internal NativeWindow GetListNativeWindow()
	{
		if (DropDownStyle != 0)
		{
			return childDropDown;
		}
		return childListBox;
	}

	internal int GetListNativeWindowRuntimeIdPart()
	{
		return GetListNativeWindow()?.GetHashCode() ?? 0;
	}

	internal override IntPtr InitializeDCForWmCtlColor(IntPtr dc, int msg)
	{
		if (msg == 312 && !ShouldSerializeBackColor())
		{
			return IntPtr.Zero;
		}
		if (msg == 308 && GetStyle(ControlStyles.UserPaint))
		{
			SafeNativeMethods.SetTextColor(new HandleRef(null, dc), ColorTranslator.ToWin32(ForeColor));
			SafeNativeMethods.SetBkColor(new HandleRef(null, dc), ColorTranslator.ToWin32(BackColor));
			return base.BackColorBrush;
		}
		return base.InitializeDCForWmCtlColor(dc, msg);
	}

	private bool InterceptAutoCompleteKeystroke(Message m)
	{
		if (m.Msg == 256)
		{
			if ((int)(long)m.WParam == 46)
			{
				MatchingText = "";
				autoCompleteTimeStamp = DateTime.Now.Ticks;
				if (Items.Count > 0)
				{
					SelectedIndex = 0;
				}
				return false;
			}
		}
		else if (m.Msg == 258)
		{
			char c = (char)(long)m.WParam;
			switch (c)
			{
			case '\b':
				if (DateTime.Now.Ticks - autoCompleteTimeStamp > 10000000 || MatchingText.Length <= 1)
				{
					MatchingText = "";
					if (Items.Count > 0)
					{
						SelectedIndex = 0;
					}
				}
				else
				{
					MatchingText = MatchingText.Remove(MatchingText.Length - 1);
					SelectedIndex = FindString(MatchingText);
				}
				autoCompleteTimeStamp = DateTime.Now.Ticks;
				return false;
			case '\u001b':
				MatchingText = "";
				break;
			}
			if (c != '\u001b' && c != '\r' && !DroppedDown && AutoCompleteMode != AutoCompleteMode.Append)
			{
				DroppedDown = true;
			}
			string text;
			if (DateTime.Now.Ticks - autoCompleteTimeStamp > 10000000)
			{
				text = new string(c, 1);
				if (FindString(text) != -1)
				{
					MatchingText = text;
				}
				autoCompleteTimeStamp = DateTime.Now.Ticks;
				return false;
			}
			text = MatchingText + c;
			int num = FindString(text);
			if (num != -1)
			{
				MatchingText = text;
				if (num != SelectedIndex)
				{
					SelectedIndex = num;
				}
			}
			autoCompleteTimeStamp = DateTime.Now.Ticks;
			return true;
		}
		return false;
	}

	private void InvalidateEverything()
	{
		SafeNativeMethods.RedrawWindow(new HandleRef(this, base.Handle), null, NativeMethods.NullHandleRef, 1157);
	}

	protected override bool IsInputKey(Keys keyData)
	{
		Keys keys = keyData & (Keys.KeyCode | Keys.Alt);
		if (keys == Keys.Return || keys == Keys.Escape)
		{
			if (DroppedDown || autoCompleteDroppedDown)
			{
				return true;
			}
			if (SystemAutoCompleteEnabled && ACNativeWindow.AutoCompleteActive)
			{
				autoCompleteDroppedDown = true;
				return true;
			}
		}
		return base.IsInputKey(keyData);
	}

	private int NativeAdd(object item)
	{
		int num = (int)(long)SendMessage(323, 0, GetItemText(item));
		if (num < 0)
		{
			throw new OutOfMemoryException(SR.GetString("ComboBoxItemOverflow"));
		}
		return num;
	}

	private void NativeClear()
	{
		string text = null;
		if (DropDownStyle != ComboBoxStyle.DropDownList)
		{
			text = WindowText;
		}
		SendMessage(331, 0, 0);
		if (text != null)
		{
			WindowText = text;
		}
	}

	private string NativeGetItemText(int index)
	{
		int num = (int)(long)SendMessage(329, index, 0);
		StringBuilder stringBuilder = new StringBuilder(num + 1);
		UnsafeNativeMethods.SendMessage(new HandleRef(this, base.Handle), 328, index, stringBuilder);
		return stringBuilder.ToString();
	}

	private int NativeInsert(int index, object item)
	{
		int num = (int)(long)SendMessage(330, index, GetItemText(item));
		if (num < 0)
		{
			throw new OutOfMemoryException(SR.GetString("ComboBoxItemOverflow"));
		}
		return num;
	}

	private void NativeRemoveAt(int index)
	{
		if (DropDownStyle == ComboBoxStyle.DropDownList && SelectedIndex == index)
		{
			Invalidate();
		}
		SendMessage(324, index, 0);
	}

	internal override void RecreateHandleCore()
	{
		string windowText = WindowText;
		base.RecreateHandleCore();
		if (!string.IsNullOrEmpty(windowText) && string.IsNullOrEmpty(WindowText))
		{
			WindowText = windowText;
		}
	}

	protected override void CreateHandle()
	{
		using (new LayoutTransaction(ParentInternal, this, PropertyNames.Bounds))
		{
			base.CreateHandle();
		}
	}

	protected override void OnHandleCreated(EventArgs e)
	{
		base.OnHandleCreated(e);
		if (MaxLength > 0)
		{
			SendMessage(321, MaxLength, 0);
		}
		if (childEdit == null && childListBox == null && DropDownStyle != ComboBoxStyle.DropDownList)
		{
			IntPtr window = UnsafeNativeMethods.GetWindow(new HandleRef(this, base.Handle), 5);
			if (window != IntPtr.Zero)
			{
				if (DropDownStyle == ComboBoxStyle.Simple)
				{
					childListBox = new ComboBoxChildNativeWindow(this, ChildWindowType.ListBox);
					childListBox.AssignHandle(window);
					window = UnsafeNativeMethods.GetWindow(new HandleRef(this, window), 2);
				}
				childEdit = new ComboBoxChildNativeWindow(this, ChildWindowType.Edit);
				childEdit.AssignHandle(window);
				UnsafeNativeMethods.SendMessage(new HandleRef(this, childEdit.Handle), 211, 3, 0);
			}
		}
		bool found;
		int integer = base.Properties.GetInteger(PropDropDownWidth, out found);
		if (found)
		{
			SendMessage(352, integer, 0);
		}
		found = false;
		int integer2 = base.Properties.GetInteger(PropItemHeight, out found);
		if (found)
		{
			UpdateItemHeight();
		}
		if (DropDownStyle == ComboBoxStyle.Simple)
		{
			base.Height = requestedHeight;
		}
		try
		{
			fromHandleCreate = true;
			SetAutoComplete(reset: false, recreate: false);
		}
		finally
		{
			fromHandleCreate = false;
		}
		if (itemsCollection != null)
		{
			foreach (object item in itemsCollection)
			{
				NativeAdd(item);
			}
			if (selectedIndex >= 0)
			{
				SendMessage(334, selectedIndex, 0);
				UpdateText();
				selectedIndex = -1;
			}
		}
	}

	protected override void OnHandleDestroyed(EventArgs e)
	{
		dropDownHandle = IntPtr.Zero;
		if (base.Disposing)
		{
			itemsCollection = null;
			selectedIndex = -1;
		}
		else
		{
			selectedIndex = SelectedIndex;
		}
		if (stringSource != null)
		{
			stringSource.ReleaseAutoComplete();
			stringSource = null;
		}
		base.OnHandleDestroyed(e);
	}

	protected virtual void OnDrawItem(DrawItemEventArgs e)
	{
		((DrawItemEventHandler)base.Events[EVENT_DRAWITEM])?.Invoke(this, e);
	}

	protected virtual void OnDropDown(EventArgs e)
	{
		((EventHandler)base.Events[EVENT_DROPDOWN])?.Invoke(this, e);
		if (System.AccessibilityImprovements.Level3)
		{
			base.AccessibilityObject.RaiseAutomationPropertyChangedEvent(30070, UnsafeNativeMethods.ExpandCollapseState.Collapsed, UnsafeNativeMethods.ExpandCollapseState.Expanded);
			(base.AccessibilityObject as ComboBoxUiaProvider)?.SetComboBoxItemFocus();
		}
	}

	[EditorBrowsable(EditorBrowsableState.Advanced)]
	protected override void OnKeyDown(KeyEventArgs e)
	{
		if (SystemAutoCompleteEnabled)
		{
			if (e.KeyCode == Keys.Return)
			{
				NotifyAutoComplete(setSelectedIndex: true);
			}
			else if (e.KeyCode == Keys.Escape && autoCompleteDroppedDown)
			{
				NotifyAutoComplete(setSelectedIndex: false);
			}
			autoCompleteDroppedDown = false;
		}
		base.OnKeyDown(e);
	}

	protected override void OnKeyPress(KeyPressEventArgs e)
	{
		base.OnKeyPress(e);
		if (!e.Handled && (e.KeyChar == '\r' || e.KeyChar == '\u001b') && DroppedDown)
		{
			dropDown = false;
			if (base.FormattingEnabled)
			{
				Text = WindowText;
				SelectAll();
				e.Handled = false;
			}
			else
			{
				DroppedDown = false;
				e.Handled = true;
			}
		}
	}

	protected virtual void OnMeasureItem(MeasureItemEventArgs e)
	{
		((MeasureItemEventHandler)base.Events[EVENT_MEASUREITEM])?.Invoke(this, e);
	}

	protected override void OnMouseEnter(EventArgs e)
	{
		base.OnMouseEnter(e);
		MouseIsOver = true;
	}

	protected override void OnMouseLeave(EventArgs e)
	{
		base.OnMouseLeave(e);
		MouseIsOver = false;
	}

	private void OnSelectionChangeCommittedInternal(EventArgs e)
	{
		if (allowCommit)
		{
			try
			{
				allowCommit = false;
				OnSelectionChangeCommitted(e);
			}
			finally
			{
				allowCommit = true;
			}
		}
	}

	protected virtual void OnSelectionChangeCommitted(EventArgs e)
	{
		((EventHandler)base.Events[EVENT_SELECTIONCHANGECOMMITTED])?.Invoke(this, e);
		if (dropDown)
		{
			dropDownWillBeClosed = true;
		}
	}

	protected override void OnSelectedIndexChanged(EventArgs e)
	{
		base.OnSelectedIndexChanged(e);
		((EventHandler)base.Events[EVENT_SELECTEDINDEXCHANGED])?.Invoke(this, e);
		if (dropDownWillBeClosed)
		{
			dropDownWillBeClosed = false;
		}
		else if (System.AccessibilityImprovements.Level3)
		{
			ComboBoxUiaProvider comboBoxUiaProvider = base.AccessibilityObject as ComboBoxUiaProvider;
			if (comboBoxUiaProvider != null && (DropDownStyle == ComboBoxStyle.DropDownList || DropDownStyle == ComboBoxStyle.DropDown))
			{
				if (dropDown)
				{
					comboBoxUiaProvider.SetComboBoxItemFocus();
				}
				comboBoxUiaProvider.SetComboBoxItemSelection();
			}
		}
		if (base.DataManager != null && base.DataManager.Position != SelectedIndex && (!base.FormattingEnabled || SelectedIndex != -1))
		{
			base.DataManager.Position = SelectedIndex;
		}
	}

	protected override void OnSelectedValueChanged(EventArgs e)
	{
		base.OnSelectedValueChanged(e);
		selectedValueChangedFired = true;
	}

	protected virtual void OnSelectedItemChanged(EventArgs e)
	{
		((EventHandler)base.Events[EVENT_SELECTEDITEMCHANGED])?.Invoke(this, e);
	}

	protected virtual void OnDropDownStyleChanged(EventArgs e)
	{
		((EventHandler)base.Events[EVENT_DROPDOWNSTYLE])?.Invoke(this, e);
	}

	protected override void OnParentBackColorChanged(EventArgs e)
	{
		base.OnParentBackColorChanged(e);
		if (DropDownStyle == ComboBoxStyle.Simple)
		{
			Invalidate();
		}
	}

	protected override void OnFontChanged(EventArgs e)
	{
		base.OnFontChanged(e);
		ResetHeightCache();
		if (AutoCompleteMode == AutoCompleteMode.None)
		{
			UpdateControl(recreate: true);
		}
		else
		{
			RecreateHandle();
		}
		CommonProperties.xClearPreferredSizeCache(this);
	}

	private void OnAutoCompleteCustomSourceChanged(object sender, CollectionChangeEventArgs e)
	{
		if (AutoCompleteSource == AutoCompleteSource.CustomSource)
		{
			if (AutoCompleteCustomSource.Count == 0)
			{
				SetAutoComplete(reset: true, recreate: true);
			}
			else
			{
				SetAutoComplete(reset: true, recreate: false);
			}
		}
	}

	protected override void OnBackColorChanged(EventArgs e)
	{
		base.OnBackColorChanged(e);
		UpdateControl(recreate: false);
	}

	protected override void OnForeColorChanged(EventArgs e)
	{
		base.OnForeColorChanged(e);
		UpdateControl(recreate: false);
	}

	[EditorBrowsable(EditorBrowsableState.Advanced)]
	protected override void OnGotFocus(EventArgs e)
	{
		if (!canFireLostFocus)
		{
			base.OnGotFocus(e);
			canFireLostFocus = true;
		}
	}

	[EditorBrowsable(EditorBrowsableState.Advanced)]
	protected override void OnLostFocus(EventArgs e)
	{
		if (canFireLostFocus)
		{
			if (AutoCompleteMode != 0 && AutoCompleteSource == AutoCompleteSource.ListItems && DropDownStyle == ComboBoxStyle.DropDownList)
			{
				MatchingText = "";
			}
			base.OnLostFocus(e);
			canFireLostFocus = false;
		}
	}

	[EditorBrowsable(EditorBrowsableState.Advanced)]
	protected override void OnTextChanged(EventArgs e)
	{
		if (SystemAutoCompleteEnabled)
		{
			string text = Text;
			if (text != lastTextChangedValue)
			{
				base.OnTextChanged(e);
				lastTextChangedValue = text;
			}
		}
		else
		{
			base.OnTextChanged(e);
		}
	}

	[EditorBrowsable(EditorBrowsableState.Advanced)]
	protected override void OnValidating(CancelEventArgs e)
	{
		if (SystemAutoCompleteEnabled)
		{
			NotifyAutoComplete();
		}
		base.OnValidating(e);
	}

	private void UpdateControl(bool recreate)
	{
		ResetHeightCache();
		if (base.IsHandleCreated)
		{
			if (DropDownStyle == ComboBoxStyle.Simple && recreate)
			{
				RecreateHandle();
				return;
			}
			UpdateItemHeight();
			InvalidateEverything();
		}
	}

	protected override void OnResize(EventArgs e)
	{
		base.OnResize(e);
		if (DropDownStyle == ComboBoxStyle.Simple)
		{
			InvalidateEverything();
		}
	}

	protected override void OnDataSourceChanged(EventArgs e)
	{
		if (Sorted && DataSource != null && base.Created)
		{
			DataSource = null;
			throw new InvalidOperationException(SR.GetString("ComboBoxDataSourceWithSort"));
		}
		if (DataSource == null)
		{
			BeginUpdate();
			SelectedIndex = -1;
			Items.ClearInternal();
			EndUpdate();
		}
		if (!Sorted && base.Created)
		{
			base.OnDataSourceChanged(e);
		}
		RefreshItems();
	}

	protected override void OnDisplayMemberChanged(EventArgs e)
	{
		base.OnDisplayMemberChanged(e);
		RefreshItems();
	}

	protected virtual void OnDropDownClosed(EventArgs e)
	{
		((EventHandler)base.Events[EVENT_DROPDOWNCLOSED])?.Invoke(this, e);
		if (System.AccessibilityImprovements.Level3)
		{
			if (DropDownStyle == ComboBoxStyle.DropDown)
			{
				base.AccessibilityObject.RaiseAutomationEvent(20005);
			}
			base.AccessibilityObject.RaiseAutomationPropertyChangedEvent(30070, UnsafeNativeMethods.ExpandCollapseState.Expanded, UnsafeNativeMethods.ExpandCollapseState.Collapsed);
			dropDownWillBeClosed = false;
		}
	}

	protected virtual void OnTextUpdate(EventArgs e)
	{
		((EventHandler)base.Events[EVENT_TEXTUPDATE])?.Invoke(this, e);
	}

	[SecurityPermission(SecurityAction.LinkDemand, Flags = SecurityPermissionFlag.UnmanagedCode)]
	protected override bool ProcessKeyEventArgs(ref Message m)
	{
		if (AutoCompleteMode != 0 && AutoCompleteSource == AutoCompleteSource.ListItems && DropDownStyle == ComboBoxStyle.DropDownList && InterceptAutoCompleteKeystroke(m))
		{
			return true;
		}
		return base.ProcessKeyEventArgs(ref m);
	}

	private void ResetHeightCache()
	{
		prefHeightCache = -1;
	}

	protected override void RefreshItems()
	{
		int num = SelectedIndex;
		ObjectCollection objectCollection = itemsCollection;
		itemsCollection = null;
		object[] array = null;
		if (base.DataManager != null && base.DataManager.Count != -1)
		{
			array = new object[base.DataManager.Count];
			for (int i = 0; i < array.Length; i++)
			{
				array[i] = base.DataManager[i];
			}
		}
		else if (objectCollection != null)
		{
			array = new object[objectCollection.Count];
			objectCollection.CopyTo(array, 0);
		}
		BeginUpdate();
		try
		{
			if (base.IsHandleCreated)
			{
				NativeClear();
			}
			if (array != null)
			{
				Items.AddRangeInternal(array);
			}
			if (base.DataManager != null)
			{
				SelectedIndex = base.DataManager.Position;
			}
			else
			{
				SelectedIndex = num;
			}
		}
		finally
		{
			EndUpdate();
		}
	}

	protected override void RefreshItem(int index)
	{
		Items.SetItemInternal(index, Items[index]);
	}

	private void ReleaseChildWindow()
	{
		if (childEdit != null)
		{
			childEdit.ReleaseHandle();
			childEdit = null;
		}
		if (childListBox != null)
		{
			if (System.AccessibilityImprovements.Level3)
			{
				ReleaseUiaProvider(childListBox.Handle);
			}
			childListBox.ReleaseHandle();
			childListBox = null;
		}
		if (childDropDown != null)
		{
			if (System.AccessibilityImprovements.Level3)
			{
				ReleaseUiaProvider(childDropDown.Handle);
			}
			childDropDown.ReleaseHandle();
			childDropDown = null;
		}
	}

	internal override void ReleaseUiaProvider(IntPtr handle)
	{
		base.ReleaseUiaProvider(handle);
		(base.AccessibilityObject as ComboBoxUiaProvider)?.ResetListItemAccessibleObjects();
	}

	private void ResetAutoCompleteCustomSource()
	{
		AutoCompleteCustomSource = null;
	}

	private void ResetDropDownWidth()
	{
		base.Properties.RemoveInteger(PropDropDownWidth);
	}

	private void ResetItemHeight()
	{
		base.Properties.RemoveInteger(PropItemHeight);
	}

	public override void ResetText()
	{
		base.ResetText();
	}

	private void SetAutoComplete(bool reset, bool recreate)
	{
		if (!base.IsHandleCreated || childEdit == null)
		{
			return;
		}
		if (AutoCompleteMode != 0)
		{
			if (!fromHandleCreate && recreate && base.IsHandleCreated)
			{
				AutoCompleteMode autoCompleteMode = AutoCompleteMode;
				this.autoCompleteMode = AutoCompleteMode.None;
				RecreateHandle();
				this.autoCompleteMode = autoCompleteMode;
			}
			if (AutoCompleteSource == AutoCompleteSource.CustomSource)
			{
				if (AutoCompleteCustomSource == null)
				{
					return;
				}
				if (AutoCompleteCustomSource.Count == 0)
				{
					int flags = -1610612736;
					SafeNativeMethods.SHAutoComplete(new HandleRef(this, childEdit.Handle), flags);
				}
				else if (stringSource == null)
				{
					stringSource = new StringSource(GetStringsForAutoComplete(AutoCompleteCustomSource));
					if (!stringSource.Bind(new HandleRef(this, childEdit.Handle), (int)AutoCompleteMode))
					{
						throw new ArgumentException(SR.GetString("AutoCompleteFailure"));
					}
				}
				else
				{
					stringSource.RefreshList(GetStringsForAutoComplete(AutoCompleteCustomSource));
				}
			}
			else if (AutoCompleteSource == AutoCompleteSource.ListItems)
			{
				if (DropDownStyle != ComboBoxStyle.DropDownList)
				{
					if (itemsCollection == null)
					{
						return;
					}
					if (itemsCollection.Count == 0)
					{
						int flags2 = -1610612736;
						SafeNativeMethods.SHAutoComplete(new HandleRef(this, childEdit.Handle), flags2);
					}
					else if (stringSource == null)
					{
						stringSource = new StringSource(GetStringsForAutoComplete(Items));
						if (!stringSource.Bind(new HandleRef(this, childEdit.Handle), (int)AutoCompleteMode))
						{
							throw new ArgumentException(SR.GetString("AutoCompleteFailureListItems"));
						}
					}
					else
					{
						stringSource.RefreshList(GetStringsForAutoComplete(Items));
					}
				}
				else
				{
					int flags3 = -1610612736;
					SafeNativeMethods.SHAutoComplete(new HandleRef(this, childEdit.Handle), flags3);
				}
			}
			else
			{
				try
				{
					int num = 0;
					if (AutoCompleteMode == AutoCompleteMode.Suggest)
					{
						num |= -1879048192;
					}
					if (AutoCompleteMode == AutoCompleteMode.Append)
					{
						num |= 0x60000000;
					}
					if (AutoCompleteMode == AutoCompleteMode.SuggestAppend)
					{
						num |= 0x10000000;
						num |= 0x40000000;
					}
					int num2 = SafeNativeMethods.SHAutoComplete(new HandleRef(this, childEdit.Handle), (int)AutoCompleteSource | num);
				}
				catch (SecurityException)
				{
				}
			}
		}
		else if (reset)
		{
			int flags4 = -1610612736;
			SafeNativeMethods.SHAutoComplete(new HandleRef(this, childEdit.Handle), flags4);
		}
	}

	public void Select(int start, int length)
	{
		if (start < 0)
		{
			throw new ArgumentOutOfRangeException("start", SR.GetString("InvalidArgument", "start", start.ToString(CultureInfo.CurrentCulture)));
		}
		int num = start + length;
		if (num < 0)
		{
			throw new ArgumentOutOfRangeException("length", SR.GetString("InvalidArgument", "length", length.ToString(CultureInfo.CurrentCulture)));
		}
		SendMessage(322, 0, NativeMethods.Util.MAKELPARAM(start, num));
	}

	public void SelectAll()
	{
		Select(0, int.MaxValue);
	}

	protected override void SetBoundsCore(int x, int y, int width, int height, BoundsSpecified specified)
	{
		if ((specified & BoundsSpecified.Height) != 0)
		{
			requestedHeight = height;
		}
		base.SetBoundsCore(x, y, width, height, specified);
	}

	protected override void SetItemsCore(IList value)
	{
		BeginUpdate();
		Items.ClearInternal();
		Items.AddRangeInternal(value);
		if (base.DataManager != null)
		{
			if (DataSource is ICurrencyManagerProvider)
			{
				selectedValueChangedFired = false;
			}
			if (base.IsHandleCreated)
			{
				SendMessage(334, base.DataManager.Position, 0);
			}
			else
			{
				selectedIndex = base.DataManager.Position;
			}
			if (!selectedValueChangedFired)
			{
				OnSelectedValueChanged(EventArgs.Empty);
				selectedValueChangedFired = false;
			}
		}
		EndUpdate();
	}

	protected override void SetItemCore(int index, object value)
	{
		Items.SetItemInternal(index, value);
	}

	private bool ShouldSerializeAutoCompleteCustomSource()
	{
		if (autoCompleteCustomSource != null)
		{
			return autoCompleteCustomSource.Count > 0;
		}
		return false;
	}

	internal bool ShouldSerializeDropDownWidth()
	{
		return base.Properties.ContainsInteger(PropDropDownWidth);
	}

	internal bool ShouldSerializeItemHeight()
	{
		return base.Properties.ContainsInteger(PropItemHeight);
	}

	internal override bool ShouldSerializeText()
	{
		if (SelectedIndex == -1)
		{
			return base.ShouldSerializeText();
		}
		return false;
	}

	public override string ToString()
	{
		string str = base.ToString();
		return str + ", Items.Count: " + ((itemsCollection == null) ? 0.ToString(CultureInfo.CurrentCulture) : itemsCollection.Count.ToString(CultureInfo.CurrentCulture));
	}

	private void UpdateDropDownHeight()
	{
		if (dropDownHandle != IntPtr.Zero)
		{
			int num = DropDownHeight;
			if (num == 106)
			{
				int val = (itemsCollection != null) ? itemsCollection.Count : 0;
				int num2 = Math.Min(Math.Max(val, 1), maxDropDownItems);
				num = ItemHeight * num2 + 2;
			}
			SafeNativeMethods.SetWindowPos(new HandleRef(this, dropDownHandle), NativeMethods.NullHandleRef, 0, 0, DropDownWidth, num, 6);
		}
	}

	private void UpdateItemHeight()
	{
		if (!base.IsHandleCreated)
		{
			CreateControl();
		}
		if (DrawMode == DrawMode.OwnerDrawFixed)
		{
			SendMessage(339, -1, ItemHeight);
			SendMessage(339, 0, ItemHeight);
		}
		else
		{
			if (DrawMode != DrawMode.OwnerDrawVariable)
			{
				return;
			}
			SendMessage(339, -1, ItemHeight);
			Graphics graphics = CreateGraphicsInternal();
			for (int i = 0; i < Items.Count; i++)
			{
				int num = (int)(long)SendMessage(340, i, 0);
				MeasureItemEventArgs measureItemEventArgs = new MeasureItemEventArgs(graphics, i, num);
				OnMeasureItem(measureItemEventArgs);
				if (measureItemEventArgs.ItemHeight != num)
				{
					SendMessage(339, i, measureItemEventArgs.ItemHeight);
				}
			}
			graphics.Dispose();
		}
	}

	private void UpdateText()
	{
		string text = null;
		if (SelectedIndex != -1)
		{
			object obj = Items[SelectedIndex];
			if (obj != null)
			{
				text = GetItemText(obj);
			}
		}
		Text = text;
		if (DropDownStyle == ComboBoxStyle.DropDown && childEdit != null && childEdit.Handle != IntPtr.Zero)
		{
			UnsafeNativeMethods.SendMessage(new HandleRef(this, childEdit.Handle), 12, IntPtr.Zero, text);
		}
	}

	private void WmEraseBkgnd(ref Message m)
	{
		if (DropDownStyle == ComboBoxStyle.Simple && ParentInternal != null)
		{
			NativeMethods.RECT rect = default(NativeMethods.RECT);
			SafeNativeMethods.GetClientRect(new HandleRef(this, base.Handle), ref rect);
			Control parentInternal = ParentInternal;
			Graphics graphics = Graphics.FromHdcInternal(m.WParam);
			if (parentInternal != null)
			{
				Brush brush = new SolidBrush(parentInternal.BackColor);
				graphics.FillRectangle(brush, rect.left, rect.top, rect.right - rect.left, rect.bottom - rect.top);
				brush.Dispose();
			}
			else
			{
				graphics.FillRectangle(SystemBrushes.Control, rect.left, rect.top, rect.right - rect.left, rect.bottom - rect.top);
			}
			graphics.Dispose();
			m.Result = (IntPtr)1;
		}
		else
		{
			base.WndProc(ref m);
		}
	}

	private void WmParentNotify(ref Message m)
	{
		base.WndProc(ref m);
		if ((int)(long)m.WParam != 65536001)
		{
			return;
		}
		dropDownHandle = m.LParam;
		if (System.AccessibilityImprovements.Level3)
		{
			if (childDropDown != null)
			{
				ReleaseUiaProvider(childListBox.Handle);
				childDropDown.ReleaseHandle();
			}
			childDropDown = new ComboBoxChildNativeWindow(this, ChildWindowType.DropDownList);
			childDropDown.AssignHandle(dropDownHandle);
			childListAccessibleObject = null;
		}
	}

	private void WmReflectCommand(ref Message m)
	{
		switch (NativeMethods.Util.HIWORD(m.WParam))
		{
		case 2:
		case 3:
		case 4:
			break;
		case 6:
			OnTextUpdate(EventArgs.Empty);
			break;
		case 8:
			OnDropDownClosed(EventArgs.Empty);
			if (base.FormattingEnabled && Text != currentText && dropDown)
			{
				OnTextChanged(EventArgs.Empty);
			}
			dropDown = false;
			break;
		case 7:
			currentText = Text;
			dropDown = true;
			OnDropDown(EventArgs.Empty);
			UpdateDropDownHeight();
			break;
		case 5:
			OnTextChanged(EventArgs.Empty);
			break;
		case 1:
			UpdateText();
			OnSelectedIndexChanged(EventArgs.Empty);
			break;
		case 9:
			OnSelectionChangeCommittedInternal(EventArgs.Empty);
			break;
		}
	}

	private void WmReflectDrawItem(ref Message m)
	{
		NativeMethods.DRAWITEMSTRUCT dRAWITEMSTRUCT = (NativeMethods.DRAWITEMSTRUCT)m.GetLParam(typeof(NativeMethods.DRAWITEMSTRUCT));
		IntPtr intPtr = Control.SetUpPalette(dRAWITEMSTRUCT.hDC, force: false, realizePalette: false);
		try
		{
			Graphics graphics = Graphics.FromHdcInternal(dRAWITEMSTRUCT.hDC);
			try
			{
				OnDrawItem(new DrawItemEventArgs(graphics, Font, Rectangle.FromLTRB(dRAWITEMSTRUCT.rcItem.left, dRAWITEMSTRUCT.rcItem.top, dRAWITEMSTRUCT.rcItem.right, dRAWITEMSTRUCT.rcItem.bottom), dRAWITEMSTRUCT.itemID, (DrawItemState)dRAWITEMSTRUCT.itemState, ForeColor, BackColor));
			}
			finally
			{
				graphics.Dispose();
			}
		}
		finally
		{
			if (intPtr != IntPtr.Zero)
			{
				SafeNativeMethods.SelectPalette(new HandleRef(this, dRAWITEMSTRUCT.hDC), new HandleRef(null, intPtr), 0);
			}
		}
		m.Result = (IntPtr)1;
	}

	private void WmReflectMeasureItem(ref Message m)
	{
		NativeMethods.MEASUREITEMSTRUCT mEASUREITEMSTRUCT = (NativeMethods.MEASUREITEMSTRUCT)m.GetLParam(typeof(NativeMethods.MEASUREITEMSTRUCT));
		if (DrawMode == DrawMode.OwnerDrawVariable && mEASUREITEMSTRUCT.itemID >= 0)
		{
			Graphics graphics = CreateGraphicsInternal();
			MeasureItemEventArgs measureItemEventArgs = new MeasureItemEventArgs(graphics, mEASUREITEMSTRUCT.itemID, ItemHeight);
			OnMeasureItem(measureItemEventArgs);
			mEASUREITEMSTRUCT.itemHeight = measureItemEventArgs.ItemHeight;
			graphics.Dispose();
		}
		else
		{
			mEASUREITEMSTRUCT.itemHeight = ItemHeight;
		}
		Marshal.StructureToPtr((object)mEASUREITEMSTRUCT, m.LParam, fDeleteOld: false);
		m.Result = (IntPtr)1;
	}

	[SecurityPermission(SecurityAction.LinkDemand, Flags = SecurityPermissionFlag.UnmanagedCode)]
	protected override void WndProc(ref Message m)
	{
		switch (m.Msg)
		{
		case 7:
			try
			{
				fireSetFocus = false;
				base.WndProc(ref m);
			}
			finally
			{
				fireSetFocus = true;
			}
			break;
		case 8:
			try
			{
				fireLostFocus = false;
				base.WndProc(ref m);
				if (!Application.RenderWithVisualStyles && !GetStyle(ControlStyles.UserPaint) && DropDownStyle == ComboBoxStyle.DropDownList && (FlatStyle == FlatStyle.Flat || FlatStyle == FlatStyle.Popup))
				{
					UnsafeNativeMethods.PostMessage(new HandleRef(this, base.Handle), 675, 0, 0);
				}
			}
			finally
			{
				fireLostFocus = true;
			}
			break;
		case 307:
		case 308:
			m.Result = InitializeDCForWmCtlColor(m.WParam, m.Msg);
			break;
		case 20:
			WmEraseBkgnd(ref m);
			break;
		case 528:
			WmParentNotify(ref m);
			break;
		case 8465:
			WmReflectCommand(ref m);
			break;
		case 8235:
			WmReflectDrawItem(ref m);
			break;
		case 8236:
			WmReflectMeasureItem(ref m);
			break;
		case 513:
			mouseEvents = true;
			base.WndProc(ref m);
			break;
		case 514:
		{
			NativeMethods.RECT rect = default(NativeMethods.RECT);
			UnsafeNativeMethods.GetWindowRect(new HandleRef(this, base.Handle), ref rect);
			Rectangle rectangle = new Rectangle(rect.left, rect.top, rect.right - rect.left, rect.bottom - rect.top);
			int x = NativeMethods.Util.SignedLOWORD(m.LParam);
			int y = NativeMethods.Util.SignedHIWORD(m.LParam);
			Point p = new Point(x, y);
			p = PointToScreen(p);
			if (mouseEvents && !base.ValidationCancelled)
			{
				mouseEvents = false;
				if (base.Capture && rectangle.Contains(p))
				{
					OnClick(new MouseEventArgs(MouseButtons.Left, 1, NativeMethods.Util.SignedLOWORD(m.LParam), NativeMethods.Util.SignedHIWORD(m.LParam), 0));
					OnMouseClick(new MouseEventArgs(MouseButtons.Left, 1, NativeMethods.Util.SignedLOWORD(m.LParam), NativeMethods.Util.SignedHIWORD(m.LParam), 0));
				}
				base.WndProc(ref m);
			}
			else
			{
				base.CaptureInternal = false;
				DefWndProc(ref m);
			}
			break;
		}
		case 675:
			DefWndProc(ref m);
			OnMouseLeaveInternal(EventArgs.Empty);
			break;
		case 15:
			if (!GetStyle(ControlStyles.UserPaint) && (FlatStyle == FlatStyle.Flat || FlatStyle == FlatStyle.Popup))
			{
				using (WindowsRegion windowsRegion2 = new WindowsRegion(FlatComboBoxAdapter.dropDownRect))
				{
					using (WindowsRegion windowsRegion = new WindowsRegion(base.Bounds))
					{
						NativeMethods.RegionFlags updateRgn = (NativeMethods.RegionFlags)SafeNativeMethods.GetUpdateRgn(new HandleRef(this, base.Handle), new HandleRef(this, windowsRegion.HRegion), fErase: true);
						windowsRegion2.CombineRegion(windowsRegion, windowsRegion2, RegionCombineMode.DIFF);
						Rectangle updateRegionBox = windowsRegion.ToRectangle();
						FlatComboBoxAdapter.ValidateOwnerDrawRegions(this, updateRegionBox);
						NativeMethods.PAINTSTRUCT lpPaint = default(NativeMethods.PAINTSTRUCT);
						bool flag = false;
						IntPtr intPtr;
						if (m.WParam == IntPtr.Zero)
						{
							intPtr = UnsafeNativeMethods.BeginPaint(new HandleRef(this, base.Handle), ref lpPaint);
							flag = true;
						}
						else
						{
							intPtr = m.WParam;
						}
						using (DeviceContext dc = DeviceContext.FromHdc(intPtr))
						{
							using (WindowsGraphics windowsGraphics = new WindowsGraphics(dc))
							{
								if (updateRgn != 0)
								{
									windowsGraphics.DeviceContext.SetClip(windowsRegion2);
								}
								m.WParam = intPtr;
								DefWndProc(ref m);
								if (updateRgn != 0)
								{
									windowsGraphics.DeviceContext.SetClip(windowsRegion);
								}
								using (Graphics g = Graphics.FromHdcInternal(intPtr))
								{
									FlatComboBoxAdapter.DrawFlatCombo(this, g);
								}
							}
						}
						if (flag)
						{
							UnsafeNativeMethods.EndPaint(new HandleRef(this, base.Handle), ref lpPaint);
						}
					}
				}
			}
			else
			{
				base.WndProc(ref m);
			}
			break;
		case 792:
			if ((!GetStyle(ControlStyles.UserPaint) && FlatStyle == FlatStyle.Flat) || FlatStyle == FlatStyle.Popup)
			{
				DefWndProc(ref m);
				if (((int)(long)m.LParam & 4) == 4)
				{
					if ((!GetStyle(ControlStyles.UserPaint) && FlatStyle == FlatStyle.Flat) || FlatStyle == FlatStyle.Popup)
					{
						using (Graphics g2 = Graphics.FromHdcInternal(m.WParam))
						{
							FlatComboBoxAdapter.DrawFlatCombo(this, g2);
						}
					}
					break;
				}
			}
			base.WndProc(ref m);
			break;
		case 32:
			base.WndProc(ref m);
			break;
		case 48:
			if (base.Width == 0)
			{
				suppressNextWindosPos = true;
			}
			base.WndProc(ref m);
			break;
		case 71:
			if (!suppressNextWindosPos)
			{
				base.WndProc(ref m);
			}
			suppressNextWindosPos = false;
			break;
		case 130:
			base.WndProc(ref m);
			ReleaseChildWindow();
			break;
		default:
			if (m.Msg == NativeMethods.WM_MOUSEENTER)
			{
				DefWndProc(ref m);
				OnMouseEnterInternal(EventArgs.Empty);
			}
			else
			{
				base.WndProc(ref m);
			}
			break;
		}
	}

	internal virtual FlatComboAdapter CreateFlatComboAdapterInstance()
	{
		return new FlatComboAdapter(this, smallButton: false);
	}
}
