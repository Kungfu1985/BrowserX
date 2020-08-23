// System.Windows.Forms.InternalAccessibleObject
using Accessibility;
using System;
using System.Globalization;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Security.Permissions;
using System.Windows.Forms;


internal sealed class InternalAccessibleObject : StandardOleMarshalObject, UnsafeNativeMethods.IAccessibleInternal, IReflect, UnsafeNativeMethods.IServiceProvider, UnsafeNativeMethods.IAccessibleEx, UnsafeNativeMethods.IRawElementProviderSimple, UnsafeNativeMethods.IRawElementProviderFragment, UnsafeNativeMethods.IRawElementProviderFragmentRoot, UnsafeNativeMethods.IInvokeProvider, UnsafeNativeMethods.IValueProvider, UnsafeNativeMethods.IRangeValueProvider, UnsafeNativeMethods.IExpandCollapseProvider, UnsafeNativeMethods.IToggleProvider, UnsafeNativeMethods.ITableProvider, UnsafeNativeMethods.ITableItemProvider, UnsafeNativeMethods.IGridProvider, UnsafeNativeMethods.IGridItemProvider, UnsafeNativeMethods.IEnumVariant, UnsafeNativeMethods.IOleWindow, UnsafeNativeMethods.ILegacyIAccessibleProvider, UnsafeNativeMethods.ISelectionProvider, UnsafeNativeMethods.ISelectionItemProvider, UnsafeNativeMethods.IRawElementProviderHwndOverride
{
    private IAccessible publicIAccessible;

    private UnsafeNativeMethods.IEnumVariant publicIEnumVariant;

    private UnsafeNativeMethods.IOleWindow publicIOleWindow;

    private IReflect publicIReflect;

    private UnsafeNativeMethods.IServiceProvider publicIServiceProvider;

    private UnsafeNativeMethods.IAccessibleEx publicIAccessibleEx;

    private UnsafeNativeMethods.IRawElementProviderSimple publicIRawElementProviderSimple;

    private UnsafeNativeMethods.IRawElementProviderFragment publicIRawElementProviderFragment;

    private UnsafeNativeMethods.IRawElementProviderFragmentRoot publicIRawElementProviderFragmentRoot;

    private UnsafeNativeMethods.IInvokeProvider publicIInvokeProvider;

    private UnsafeNativeMethods.IValueProvider publicIValueProvider;

    private UnsafeNativeMethods.IRangeValueProvider publicIRangeValueProvider;

    private UnsafeNativeMethods.IExpandCollapseProvider publicIExpandCollapseProvider;

    private UnsafeNativeMethods.IToggleProvider publicIToggleProvider;

    private UnsafeNativeMethods.ITableProvider publicITableProvider;

    private UnsafeNativeMethods.ITableItemProvider publicITableItemProvider;

    private UnsafeNativeMethods.IGridProvider publicIGridProvider;

    private UnsafeNativeMethods.IGridItemProvider publicIGridItemProvider;

    private UnsafeNativeMethods.ILegacyIAccessibleProvider publicILegacyIAccessibleProvider;

    private UnsafeNativeMethods.ISelectionProvider publicISelectionProvider;

    private UnsafeNativeMethods.ISelectionItemProvider publicISelectionItemProvider;

    private UnsafeNativeMethods.IRawElementProviderHwndOverride publicIRawElementProviderHwndOverride;

    Type IReflect.UnderlyingSystemType
    {
        get
        {
            IReflect reflect = publicIReflect;
            return publicIReflect.UnderlyingSystemType;
        }
    }

    UnsafeNativeMethods.ProviderOptions UnsafeNativeMethods.IRawElementProviderSimple.ProviderOptions
    {
        get
        {
            IntSecurity.UnmanagedCode.Assert();
            return publicIRawElementProviderSimple.ProviderOptions;
        }
    }

    UnsafeNativeMethods.IRawElementProviderSimple UnsafeNativeMethods.IRawElementProviderSimple.HostRawElementProvider
    {
        get
        {
            IntSecurity.UnmanagedCode.Assert();
            return publicIRawElementProviderSimple.HostRawElementProvider;
        }
    }

    NativeMethods.UiaRect UnsafeNativeMethods.IRawElementProviderFragment.BoundingRectangle
    {
        get
        {
            IntSecurity.UnmanagedCode.Assert();
            return publicIRawElementProviderFragment.BoundingRectangle;
        }
    }

    UnsafeNativeMethods.IRawElementProviderFragmentRoot UnsafeNativeMethods.IRawElementProviderFragment.FragmentRoot
    {
        get
        {
            IntSecurity.UnmanagedCode.Assert();
            if (System.AccessibilityImprovements.Level3)
            {
                return publicIRawElementProviderFragment.FragmentRoot;
            }
            return AsNativeAccessible(publicIRawElementProviderFragment.FragmentRoot) as UnsafeNativeMethods.IRawElementProviderFragmentRoot;
        }
    }

    string UnsafeNativeMethods.ILegacyIAccessibleProvider.DefaultAction
    {
        get
        {
            IntSecurity.UnmanagedCode.Assert();
            return publicILegacyIAccessibleProvider.DefaultAction;
        }
    }

    string UnsafeNativeMethods.ILegacyIAccessibleProvider.Description
    {
        get
        {
            IntSecurity.UnmanagedCode.Assert();
            return publicILegacyIAccessibleProvider.Description;
        }
    }

    string UnsafeNativeMethods.ILegacyIAccessibleProvider.Help
    {
        get
        {
            IntSecurity.UnmanagedCode.Assert();
            return publicILegacyIAccessibleProvider.Help;
        }
    }

    string UnsafeNativeMethods.ILegacyIAccessibleProvider.KeyboardShortcut
    {
        get
        {
            IntSecurity.UnmanagedCode.Assert();
            return publicILegacyIAccessibleProvider.KeyboardShortcut;
        }
    }

    string UnsafeNativeMethods.ILegacyIAccessibleProvider.Name
    {
        get
        {
            IntSecurity.UnmanagedCode.Assert();
            return publicILegacyIAccessibleProvider.Name;
        }
    }

    uint UnsafeNativeMethods.ILegacyIAccessibleProvider.Role
    {
        get
        {
            IntSecurity.UnmanagedCode.Assert();
            return publicILegacyIAccessibleProvider.Role;
        }
    }

    uint UnsafeNativeMethods.ILegacyIAccessibleProvider.State
    {
        get
        {
            IntSecurity.UnmanagedCode.Assert();
            return publicILegacyIAccessibleProvider.State;
        }
    }

    string UnsafeNativeMethods.ILegacyIAccessibleProvider.Value
    {
        get
        {
            IntSecurity.UnmanagedCode.Assert();
            return publicILegacyIAccessibleProvider.Value;
        }
    }

    int UnsafeNativeMethods.ILegacyIAccessibleProvider.ChildId
    {
        get
        {
            IntSecurity.UnmanagedCode.Assert();
            return publicILegacyIAccessibleProvider.ChildId;
        }
    }

    bool UnsafeNativeMethods.IValueProvider.IsReadOnly
    {
        get
        {
            IntSecurity.UnmanagedCode.Assert();
            return publicIValueProvider.IsReadOnly;
        }
    }

    string UnsafeNativeMethods.IValueProvider.Value
    {
        get
        {
            IntSecurity.UnmanagedCode.Assert();
            return publicIValueProvider.Value;
        }
    }

    bool UnsafeNativeMethods.IRangeValueProvider.IsReadOnly
    {
        get
        {
            IntSecurity.UnmanagedCode.Assert();
            return publicIValueProvider.IsReadOnly;
        }
    }

    double UnsafeNativeMethods.IRangeValueProvider.LargeChange
    {
        get
        {
            IntSecurity.UnmanagedCode.Assert();
            return publicIRangeValueProvider.LargeChange;
        }
    }

    double UnsafeNativeMethods.IRangeValueProvider.Maximum
    {
        get
        {
            IntSecurity.UnmanagedCode.Assert();
            return publicIRangeValueProvider.Maximum;
        }
    }

    double UnsafeNativeMethods.IRangeValueProvider.Minimum
    {
        get
        {
            IntSecurity.UnmanagedCode.Assert();
            return publicIRangeValueProvider.Minimum;
        }
    }

    double UnsafeNativeMethods.IRangeValueProvider.SmallChange
    {
        get
        {
            IntSecurity.UnmanagedCode.Assert();
            return publicIRangeValueProvider.SmallChange;
        }
    }

    double UnsafeNativeMethods.IRangeValueProvider.Value
    {
        get
        {
            IntSecurity.UnmanagedCode.Assert();
            return publicIRangeValueProvider.Value;
        }
    }

    UnsafeNativeMethods.ExpandCollapseState UnsafeNativeMethods.IExpandCollapseProvider.ExpandCollapseState
    {
        get
        {
            IntSecurity.UnmanagedCode.Assert();
            return publicIExpandCollapseProvider.ExpandCollapseState;
        }
    }

    UnsafeNativeMethods.ToggleState UnsafeNativeMethods.IToggleProvider.ToggleState
    {
        get
        {
            IntSecurity.UnmanagedCode.Assert();
            return publicIToggleProvider.ToggleState;
        }
    }

    UnsafeNativeMethods.RowOrColumnMajor UnsafeNativeMethods.ITableProvider.RowOrColumnMajor
    {
        get
        {
            IntSecurity.UnmanagedCode.Assert();
            return publicITableProvider.RowOrColumnMajor;
        }
    }

    int UnsafeNativeMethods.IGridProvider.RowCount
    {
        get
        {
            IntSecurity.UnmanagedCode.Assert();
            return publicIGridProvider.RowCount;
        }
    }

    int UnsafeNativeMethods.IGridProvider.ColumnCount
    {
        get
        {
            IntSecurity.UnmanagedCode.Assert();
            return publicIGridProvider.ColumnCount;
        }
    }

    int UnsafeNativeMethods.IGridItemProvider.Row
    {
        get
        {
            IntSecurity.UnmanagedCode.Assert();
            return publicIGridItemProvider.Row;
        }
    }

    int UnsafeNativeMethods.IGridItemProvider.Column
    {
        get
        {
            IntSecurity.UnmanagedCode.Assert();
            return publicIGridItemProvider.Column;
        }
    }

    int UnsafeNativeMethods.IGridItemProvider.RowSpan
    {
        get
        {
            IntSecurity.UnmanagedCode.Assert();
            return publicIGridItemProvider.RowSpan;
        }
    }

    int UnsafeNativeMethods.IGridItemProvider.ColumnSpan
    {
        get
        {
            IntSecurity.UnmanagedCode.Assert();
            return publicIGridItemProvider.ColumnSpan;
        }
    }

    UnsafeNativeMethods.IRawElementProviderSimple UnsafeNativeMethods.IGridItemProvider.ContainingGrid
    {
        get
        {
            IntSecurity.UnmanagedCode.Assert();
            if (System.AccessibilityImprovements.Level3)
            {
                return publicIGridItemProvider.ContainingGrid;
            }
            return AsNativeAccessible(publicIGridItemProvider.ContainingGrid) as UnsafeNativeMethods.IRawElementProviderSimple;
        }
    }

    bool UnsafeNativeMethods.ISelectionProvider.CanSelectMultiple
    {
        get
        {
            IntSecurity.UnmanagedCode.Assert();
            return publicISelectionProvider.CanSelectMultiple;
        }
    }

    bool UnsafeNativeMethods.ISelectionProvider.IsSelectionRequired
    {
        get
        {
            IntSecurity.UnmanagedCode.Assert();
            return publicISelectionProvider.IsSelectionRequired;
        }
    }

    bool UnsafeNativeMethods.ISelectionItemProvider.IsSelected
    {
        get
        {
            IntSecurity.UnmanagedCode.Assert();
            return publicISelectionItemProvider.IsSelected;
        }
    }

    UnsafeNativeMethods.IRawElementProviderSimple UnsafeNativeMethods.ISelectionItemProvider.SelectionContainer
    {
        get
        {
            IntSecurity.UnmanagedCode.Assert();
            return publicISelectionItemProvider.SelectionContainer;
        }
    }

    [SecurityPermission(SecurityAction.Demand, Flags = SecurityPermissionFlag.UnmanagedCode)]
    internal InternalAccessibleObject(AccessibleObject accessibleImplemention)
    {
        publicIAccessible = accessibleImplemention;
        publicIEnumVariant = accessibleImplemention;
        publicIOleWindow = accessibleImplemention;
        publicIReflect = accessibleImplemention;
        publicIServiceProvider = accessibleImplemention;
        publicIAccessibleEx = accessibleImplemention;
        publicIRawElementProviderSimple = accessibleImplemention;
        publicIRawElementProviderFragment = accessibleImplemention;
        publicIRawElementProviderFragmentRoot = accessibleImplemention;
        publicIInvokeProvider = accessibleImplemention;
        publicIValueProvider = accessibleImplemention;
        publicIRangeValueProvider = accessibleImplemention;
        publicIExpandCollapseProvider = accessibleImplemention;
        publicIToggleProvider = accessibleImplemention;
        publicITableProvider = accessibleImplemention;
        publicITableItemProvider = accessibleImplemention;
        publicIGridProvider = accessibleImplemention;
        publicIGridItemProvider = accessibleImplemention;
        publicILegacyIAccessibleProvider = accessibleImplemention;
        publicISelectionProvider = accessibleImplemention;
        publicISelectionItemProvider = accessibleImplemention;
        publicIRawElementProviderHwndOverride = accessibleImplemention;
    }

    private object AsNativeAccessible(object accObject)
    {
        if (accObject is AccessibleObject)
        {
            return new InternalAccessibleObject(accObject as AccessibleObject);
        }
        return accObject;
    }

    private object[] AsArrayOfNativeAccessibles(object[] accObjectArray)
    {
        if (accObjectArray != null && accObjectArray.Length != 0)
        {
            for (int i = 0; i < accObjectArray.Length; i++)
            {
                accObjectArray[i] = AsNativeAccessible(accObjectArray[i]);
            }
        }
        return accObjectArray;
    }

    void UnsafeNativeMethods.IAccessibleInternal.accDoDefaultAction(object childID)
    {
        IntSecurity.UnmanagedCode.Assert();
        publicIAccessible.accDoDefaultAction(childID);
    }

    object UnsafeNativeMethods.IAccessibleInternal.accHitTest(int xLeft, int yTop)
    {
        IntSecurity.UnmanagedCode.Assert();
        return AsNativeAccessible(publicIAccessible.accHitTest(xLeft, yTop));
    }

    void UnsafeNativeMethods.IAccessibleInternal.accLocation(out int l, out int t, out int w, out int h, object childID)
    {
        IntSecurity.UnmanagedCode.Assert();
        publicIAccessible.accLocation(out l, out t, out w, out h, childID);
    }

    object UnsafeNativeMethods.IAccessibleInternal.accNavigate(int navDir, object childID)
    {
        IntSecurity.UnmanagedCode.Assert();
        return AsNativeAccessible(publicIAccessible.accNavigate(navDir, childID));
    }

    void UnsafeNativeMethods.IAccessibleInternal.accSelect(int flagsSelect, object childID)
    {
        IntSecurity.UnmanagedCode.Assert();
        publicIAccessible.accSelect(flagsSelect, childID);
    }

    object UnsafeNativeMethods.IAccessibleInternal.get_accChild(object childID)
    {
        IntSecurity.UnmanagedCode.Assert();
        return AsNativeAccessible(publicIAccessible.get_accChild(childID));
    }

    int UnsafeNativeMethods.IAccessibleInternal.get_accChildCount()
    {
        IntSecurity.UnmanagedCode.Assert();
        return publicIAccessible.accChildCount;
    }

    string UnsafeNativeMethods.IAccessibleInternal.get_accDefaultAction(object childID)
    {
        IntSecurity.UnmanagedCode.Assert();
        return publicIAccessible.get_accDefaultAction(childID);
    }

    string UnsafeNativeMethods.IAccessibleInternal.get_accDescription(object childID)
    {
        IntSecurity.UnmanagedCode.Assert();
        return publicIAccessible.get_accDescription(childID);
    }

    object UnsafeNativeMethods.IAccessibleInternal.get_accFocus()
    {
        IntSecurity.UnmanagedCode.Assert();
        return AsNativeAccessible(publicIAccessible.accFocus);
    }

    string UnsafeNativeMethods.IAccessibleInternal.get_accHelp(object childID)
    {
        IntSecurity.UnmanagedCode.Assert();
        return publicIAccessible.get_accHelp(childID);
    }

    int UnsafeNativeMethods.IAccessibleInternal.get_accHelpTopic(out string pszHelpFile, object childID)
    {
        IntSecurity.UnmanagedCode.Assert();
        return publicIAccessible.get_accHelpTopic(out pszHelpFile, childID);
    }

    string UnsafeNativeMethods.IAccessibleInternal.get_accKeyboardShortcut(object childID)
    {
        IntSecurity.UnmanagedCode.Assert();
        return publicIAccessible.get_accKeyboardShortcut(childID);
    }

    string UnsafeNativeMethods.IAccessibleInternal.get_accName(object childID)
    {
        IntSecurity.UnmanagedCode.Assert();
        return publicIAccessible.get_accName(childID);
    }

    object UnsafeNativeMethods.IAccessibleInternal.get_accParent()
    {
        IntSecurity.UnmanagedCode.Assert();
        return AsNativeAccessible(publicIAccessible.accParent);
    }

    object UnsafeNativeMethods.IAccessibleInternal.get_accRole(object childID)
    {
        IntSecurity.UnmanagedCode.Assert();
        return publicIAccessible.get_accRole(childID);
    }

    object UnsafeNativeMethods.IAccessibleInternal.get_accSelection()
    {
        IntSecurity.UnmanagedCode.Assert();
        return AsNativeAccessible(publicIAccessible.accSelection);
    }

    object UnsafeNativeMethods.IAccessibleInternal.get_accState(object childID)
    {
        IntSecurity.UnmanagedCode.Assert();
        return publicIAccessible.get_accState(childID);
    }

    string UnsafeNativeMethods.IAccessibleInternal.get_accValue(object childID)
    {
        IntSecurity.UnmanagedCode.Assert();
        return publicIAccessible.get_accValue(childID);
    }

    void UnsafeNativeMethods.IAccessibleInternal.set_accName(object childID, string newName)
    {
        IntSecurity.UnmanagedCode.Assert();
        publicIAccessible.set_accName(childID, newName);
    }

    void UnsafeNativeMethods.IAccessibleInternal.set_accValue(object childID, string newValue)
    {
        IntSecurity.UnmanagedCode.Assert();
        publicIAccessible.set_accValue(childID, newValue);
    }

    void UnsafeNativeMethods.IEnumVariant.Clone(UnsafeNativeMethods.IEnumVariant[] v)
    {
        IntSecurity.UnmanagedCode.Assert();
        publicIEnumVariant.Clone(v);
    }

    int UnsafeNativeMethods.IEnumVariant.Next(int n, IntPtr rgvar, int[] ns)
    {
        IntSecurity.UnmanagedCode.Assert();
        return publicIEnumVariant.Next(n, rgvar, ns);
    }

    void UnsafeNativeMethods.IEnumVariant.Reset()
    {
        IntSecurity.UnmanagedCode.Assert();
        publicIEnumVariant.Reset();
    }

    void UnsafeNativeMethods.IEnumVariant.Skip(int n)
    {
        IntSecurity.UnmanagedCode.Assert();
        publicIEnumVariant.Skip(n);
    }

    int UnsafeNativeMethods.IOleWindow.GetWindow(out IntPtr hwnd)
    {
        IntSecurity.UnmanagedCode.Assert();
        return publicIOleWindow.GetWindow(out hwnd);
    }

    void UnsafeNativeMethods.IOleWindow.ContextSensitiveHelp(int fEnterMode)
    {
        IntSecurity.UnmanagedCode.Assert();
        publicIOleWindow.ContextSensitiveHelp(fEnterMode);
    }

    MethodInfo IReflect.GetMethod(string name, BindingFlags bindingAttr, Binder binder, Type[] types, ParameterModifier[] modifiers)
    {
        return publicIReflect.GetMethod(name, bindingAttr, binder, types, modifiers);
    }

    MethodInfo IReflect.GetMethod(string name, BindingFlags bindingAttr)
    {
        return publicIReflect.GetMethod(name, bindingAttr);
    }

    MethodInfo[] IReflect.GetMethods(BindingFlags bindingAttr)
    {
        return publicIReflect.GetMethods(bindingAttr);
    }

    FieldInfo IReflect.GetField(string name, BindingFlags bindingAttr)
    {
        return publicIReflect.GetField(name, bindingAttr);
    }

    FieldInfo[] IReflect.GetFields(BindingFlags bindingAttr)
    {
        return publicIReflect.GetFields(bindingAttr);
    }

    PropertyInfo IReflect.GetProperty(string name, BindingFlags bindingAttr)
    {
        return publicIReflect.GetProperty(name, bindingAttr);
    }

    PropertyInfo IReflect.GetProperty(string name, BindingFlags bindingAttr, Binder binder, Type returnType, Type[] types, ParameterModifier[] modifiers)
    {
        return publicIReflect.GetProperty(name, bindingAttr, binder, returnType, types, modifiers);
    }

    PropertyInfo[] IReflect.GetProperties(BindingFlags bindingAttr)
    {
        return publicIReflect.GetProperties(bindingAttr);
    }

    MemberInfo[] IReflect.GetMember(string name, BindingFlags bindingAttr)
    {
        return publicIReflect.GetMember(name, bindingAttr);
    }

    MemberInfo[] IReflect.GetMembers(BindingFlags bindingAttr)
    {
        return publicIReflect.GetMembers(bindingAttr);
    }

    object IReflect.InvokeMember(string name, BindingFlags invokeAttr, Binder binder, object target, object[] args, ParameterModifier[] modifiers, CultureInfo culture, string[] namedParameters)
    {
        IntSecurity.UnmanagedCode.Demand();
        return publicIReflect.InvokeMember(name, invokeAttr, binder, publicIAccessible, args, modifiers, culture, namedParameters);
    }

    int UnsafeNativeMethods.IServiceProvider.QueryService(ref Guid service, ref Guid riid, out IntPtr ppvObject)
    {
        IntSecurity.UnmanagedCode.Assert();
        ppvObject = IntPtr.Zero;
        int num = publicIServiceProvider.QueryService(ref service, ref riid, out ppvObject);
        if (num >= 0)
        {
            ppvObject = Marshal.GetComInterfaceForObject(this, typeof(UnsafeNativeMethods.IAccessibleEx));
        }
        return num;
    }

    object UnsafeNativeMethods.IAccessibleEx.GetObjectForChild(int idChild)
    {
        IntSecurity.UnmanagedCode.Assert();
        return publicIAccessibleEx.GetObjectForChild(idChild);
    }

    int UnsafeNativeMethods.IAccessibleEx.GetIAccessiblePair(out object ppAcc, out int pidChild)
    {
        IntSecurity.UnmanagedCode.Assert();
        ppAcc = this;
        pidChild = 0;
        return 0;
    }

    int[] UnsafeNativeMethods.IAccessibleEx.GetRuntimeId()
    {
        IntSecurity.UnmanagedCode.Assert();
        return publicIAccessibleEx.GetRuntimeId();
    }

    int UnsafeNativeMethods.IAccessibleEx.ConvertReturnedElement(object pIn, out object ppRetValOut)
    {
        IntSecurity.UnmanagedCode.Assert();
        return publicIAccessibleEx.ConvertReturnedElement(pIn, out ppRetValOut);
    }

    object UnsafeNativeMethods.IRawElementProviderSimple.GetPatternProvider(int patternId)
    {
        IntSecurity.UnmanagedCode.Assert();
        object patternProvider = publicIRawElementProviderSimple.GetPatternProvider(patternId);
        if (patternProvider != null)
        {
            switch (patternId)
            {
                case 10005:
                    return this;
                case 10002:
                    return this;
                default:
                    if (System.AccessibilityImprovements.Level3 && patternId == 10003)
                    {
                        return this;
                    }
                    switch (patternId)
                    {
                        case 10015:
                            return this;
                        case 10012:
                            return this;
                        case 10013:
                            return this;
                        case 10006:
                            return this;
                        case 10007:
                            return this;
                        default:
                            if (System.AccessibilityImprovements.Level3 && patternId == 10000)
                            {
                                return this;
                            }
                            if (System.AccessibilityImprovements.Level3 && patternId == 10018)
                            {
                                return this;
                            }
                            if (System.AccessibilityImprovements.Level3 && patternId == 10001)
                            {
                                return this;
                            }
                            if (System.AccessibilityImprovements.Level3 && patternId == 10010)
                            {
                                return this;
                            }
                            return null;
                    }
            }
        }
        return null;
    }

    object UnsafeNativeMethods.IRawElementProviderSimple.GetPropertyValue(int propertyID)
    {
        IntSecurity.UnmanagedCode.Assert();
        return publicIRawElementProviderSimple.GetPropertyValue(propertyID);
    }

    object UnsafeNativeMethods.IRawElementProviderFragment.Navigate(UnsafeNativeMethods.NavigateDirection direction)
    {
        IntSecurity.UnmanagedCode.Assert();
        return AsNativeAccessible(publicIRawElementProviderFragment.Navigate(direction));
    }

    int[] UnsafeNativeMethods.IRawElementProviderFragment.GetRuntimeId()
    {
        IntSecurity.UnmanagedCode.Assert();
        return publicIRawElementProviderFragment.GetRuntimeId();
    }

    object[] UnsafeNativeMethods.IRawElementProviderFragment.GetEmbeddedFragmentRoots()
    {
        IntSecurity.UnmanagedCode.Assert();
        return AsArrayOfNativeAccessibles(publicIRawElementProviderFragment.GetEmbeddedFragmentRoots());
    }

    void UnsafeNativeMethods.IRawElementProviderFragment.SetFocus()
    {
        IntSecurity.UnmanagedCode.Assert();
        publicIRawElementProviderFragment.SetFocus();
    }

    object UnsafeNativeMethods.IRawElementProviderFragmentRoot.ElementProviderFromPoint(double x, double y)
    {
        IntSecurity.UnmanagedCode.Assert();
        return AsNativeAccessible(publicIRawElementProviderFragmentRoot.ElementProviderFromPoint(x, y));
    }

    object UnsafeNativeMethods.IRawElementProviderFragmentRoot.GetFocus()
    {
        IntSecurity.UnmanagedCode.Assert();
        return AsNativeAccessible(publicIRawElementProviderFragmentRoot.GetFocus());
    }

    void UnsafeNativeMethods.ILegacyIAccessibleProvider.DoDefaultAction()
    {
        IntSecurity.UnmanagedCode.Assert();
        publicILegacyIAccessibleProvider.DoDefaultAction();
    }

    IAccessible UnsafeNativeMethods.ILegacyIAccessibleProvider.GetIAccessible()
    {
        IntSecurity.UnmanagedCode.Assert();
        return publicILegacyIAccessibleProvider.GetIAccessible();
    }

    object[] UnsafeNativeMethods.ILegacyIAccessibleProvider.GetSelection()
    {
        IntSecurity.UnmanagedCode.Assert();
        return AsArrayOfNativeAccessibles(publicILegacyIAccessibleProvider.GetSelection());
    }

    void UnsafeNativeMethods.ILegacyIAccessibleProvider.Select(int flagsSelect)
    {
        IntSecurity.UnmanagedCode.Assert();
        publicILegacyIAccessibleProvider.Select(flagsSelect);
    }

    void UnsafeNativeMethods.ILegacyIAccessibleProvider.SetValue(string szValue)
    {
        IntSecurity.UnmanagedCode.Assert();
        publicILegacyIAccessibleProvider.SetValue(szValue);
    }

    void UnsafeNativeMethods.IInvokeProvider.Invoke()
    {
        IntSecurity.UnmanagedCode.Assert();
        publicIInvokeProvider.Invoke();
    }

    void UnsafeNativeMethods.IValueProvider.SetValue(string newValue)
    {
        IntSecurity.UnmanagedCode.Assert();
        publicIValueProvider.SetValue(newValue);
    }

    void UnsafeNativeMethods.IRangeValueProvider.SetValue(double newValue)
    {
        IntSecurity.UnmanagedCode.Assert();
        publicIRangeValueProvider.SetValue(newValue);
    }

    void UnsafeNativeMethods.IExpandCollapseProvider.Expand()
    {
        IntSecurity.UnmanagedCode.Assert();
        publicIExpandCollapseProvider.Expand();
    }

    void UnsafeNativeMethods.IExpandCollapseProvider.Collapse()
    {
        IntSecurity.UnmanagedCode.Assert();
        publicIExpandCollapseProvider.Collapse();
    }

    void UnsafeNativeMethods.IToggleProvider.Toggle()
    {
        IntSecurity.UnmanagedCode.Assert();
        publicIToggleProvider.Toggle();
    }

    object[] UnsafeNativeMethods.ITableProvider.GetRowHeaders()
    {
        IntSecurity.UnmanagedCode.Assert();
        return AsArrayOfNativeAccessibles(publicITableProvider.GetRowHeaders());
    }

    object[] UnsafeNativeMethods.ITableProvider.GetColumnHeaders()
    {
        IntSecurity.UnmanagedCode.Assert();
        return AsArrayOfNativeAccessibles(publicITableProvider.GetColumnHeaders());
    }

    object[] UnsafeNativeMethods.ITableItemProvider.GetRowHeaderItems()
    {
        IntSecurity.UnmanagedCode.Assert();
        return AsArrayOfNativeAccessibles(publicITableItemProvider.GetRowHeaderItems());
    }

    object[] UnsafeNativeMethods.ITableItemProvider.GetColumnHeaderItems()
    {
        IntSecurity.UnmanagedCode.Assert();
        return AsArrayOfNativeAccessibles(publicITableItemProvider.GetColumnHeaderItems());
    }

    object UnsafeNativeMethods.IGridProvider.GetItem(int row, int column)
    {
        IntSecurity.UnmanagedCode.Assert();
        return AsNativeAccessible(publicIGridProvider.GetItem(row, column));
    }

    object[] UnsafeNativeMethods.ISelectionProvider.GetSelection()
    {
        IntSecurity.UnmanagedCode.Assert();
        return publicISelectionProvider.GetSelection();
    }

    void UnsafeNativeMethods.ISelectionItemProvider.Select()
    {
        IntSecurity.UnmanagedCode.Assert();
        publicISelectionItemProvider.Select();
    }

    void UnsafeNativeMethods.ISelectionItemProvider.AddToSelection()
    {
        IntSecurity.UnmanagedCode.Assert();
        publicISelectionItemProvider.AddToSelection();
    }

    void UnsafeNativeMethods.ISelectionItemProvider.RemoveFromSelection()
    {
        IntSecurity.UnmanagedCode.Assert();
        publicISelectionItemProvider.RemoveFromSelection();
    }

    UnsafeNativeMethods.IRawElementProviderSimple UnsafeNativeMethods.IRawElementProviderHwndOverride.GetOverrideProviderForHwnd(IntPtr hwnd)
    {
        IntSecurity.UnmanagedCode.Assert();
        return publicIRawElementProviderHwndOverride.GetOverrideProviderForHwnd(hwnd);
    }
}
