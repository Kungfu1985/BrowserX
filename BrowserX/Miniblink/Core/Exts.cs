using MiniBlinkPinvoke;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;

namespace MiniBlink
{
    internal static class Exts
    {
        private static Dictionary<long, object> _keepref = new Dictionary<long, object>();

        public static long ToLong(this DateTime time)
        {
            var now = time.ToUniversalTime();
            var start = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            return (now.Ticks - start.Ticks) / 10000;
        }

        public static DateTime ToDate(this long time)
        {
            var start = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            time = start.Ticks + time * 10000;
            return new DateTime(time, DateTimeKind.Utc).ToLocalTime();
        }

        public static bool SW(this string str, string value)
        {
            if (str == value) return true;
            if (str == null || value == null) return false;
            return str.StartsWith(value, StringComparison.OrdinalIgnoreCase);
        }

        public static string WKEToUTF8String(this IntPtr ptr)
        {
            return BlinkBrowserPInvoke.wkeGetString(ptr).ToUTF8String();
        }

        public static string ToUTF8String(this IntPtr ptr)
        {
            var data = new List<byte>();
            var off = 0;
            while (true)
            {
                var ch = Marshal.ReadByte(ptr, off++);
                if (ch == 0)
                {
                    break;
                }
                data.Add(ch);
            }
            return Encoding.UTF8.GetString(data.ToArray());
        }

        public static string ToStringW(this IntPtr ptr)
        {
            return ptr == IntPtr.Zero ? null : Marshal.PtrToStringUni(ptr);
        }

        public static object ToValue(this long value, BlinkBrowser miniblink, IntPtr es)
        {
            if (value == 0) return null;

            wkeJSType type = BlinkBrowserPInvoke.jsTypeOf(value);
            switch (type)
            {
                case wkeJSType.JSTYPE_NULL:
                case wkeJSType.JSTYPE_UNDEFINED:
                    return null;
                case wkeJSType.JSTYPE_NUMBER:
                    return BlinkBrowserPInvoke.jsToDouble(es, value);
                case wkeJSType.JSTYPE_BOOLEAN:
                    return BlinkBrowserPInvoke.jsToBoolean(es, value);
                case wkeJSType.JSTYPE_STRING:
                    //string strvalue = string.Empty;
                    //strvalue = BlinkBrowserPInvoke.jsToTempStringW(es, value).ToStringW();
                    return BlinkBrowserPInvoke.jsToTempStringW(es, value).ToStringW();
                case wkeJSType.JSTYPE_FUNCTION:
                    return new JsFunc(new JsFuncWapper(miniblink, value, es).Call);
                case wkeJSType.JSTYPE_ARRAY:
                    var len = BlinkBrowserPInvoke.jsGetLength(es, value);
                    var array = new object[len];
                    for (var i = 0; i < array.Length; i++)
                    {
                        array[i] = BlinkBrowserPInvoke.jsGetAt(es, value, i).ToValue(miniblink, es);
                    }

                    return array;
                case wkeJSType.JSTYPE_OBJECT:
                    var ptr = BlinkBrowserPInvoke.jsGetKeys(es, value);
                    var jskeys = (jsKeys)Marshal.PtrToStructure(ptr, typeof(jsKeys));
                    var keys = Utils.PtrToStringArray(jskeys.keys, jskeys.length);
                    var exp = new ExpandoObject();
                    var map = (IDictionary<string, object>)exp;
                    foreach (var k in keys)
                    {
                        map.Add(k, BlinkBrowserPInvoke.jsGet(es, value, k).ToValue(miniblink, es));
                    }

                    return map;
                default:
                    throw new NotSupportedException();
            }
        }

        public static long ToJsValue(this object obj, BlinkBrowser miniblink, IntPtr es)
        {
            if (obj == null)
                return BlinkBrowserPInvoke.jsUndefined();
            if (obj is int)
                return BlinkBrowserPInvoke.jsInt((int)obj);
            if (obj is bool)
                return BlinkBrowserPInvoke.jsBoolean((bool)obj);
            if (obj is double)
                return BlinkBrowserPInvoke.jsDouble((double)obj);
            if (obj is decimal)
            {
                var dec = (decimal)obj;
                if (dec.ToString().Contains("."))
                {
                    return ToJsValue(Convert.ToDouble(dec.ToString()), miniblink, es);
                }
                else
                {
                    return ToJsValue(Convert.ToInt32(dec.ToString()), miniblink, es);
                }
            }
            if (obj is long)
                return ToJsValue(obj.ToString(), miniblink, es);
            if (obj is float)
                return BlinkBrowserPInvoke.jsFloat((float)obj);
            if (obj is DateTime)
                return BlinkBrowserPInvoke.jsDouble(((DateTime)obj).ToLong());
            if (obj is string)
                return BlinkBrowserPInvoke.jsString(es, obj.ToString());
            if (obj is IEnumerable)
            {
                var list = new List<object>();
                foreach (var item in (IEnumerable)obj)
                    list.Add(item);
                var array = BlinkBrowserPInvoke.jsEmptyArray(es);
                BlinkBrowserPInvoke.jsSetLength(es, array, list.Count);
                for (var i = 0; i < list.Count; i++)
                {
                    BlinkBrowserPInvoke.jsSetAt(es, array, i, list[i].ToJsValue(miniblink, es));
                }

                return array;
            }

            if (obj is Delegate)
            {
                var func = (Delegate)obj;
                var funcptr = Marshal.AllocHGlobal(Marshal.SizeOf(typeof(jsData)));
                var jsfunc = new jsCallAsFunctionCallback(
                    (fes, fobj, fargs, fcount) =>
                    {
                        if (func is TempNetFunc)
                        {
                            var fps = new List<object>();
                            for (var i = 0; i < fcount; i++)
                            {
                                fps.Add(BlinkBrowserPInvoke.jsArg(fes, i).ToValue(miniblink, fes));
                            }

                            return ((TempNetFunc)func)(fps.ToArray()).ToJsValue(miniblink, fes);
                        }
                        else
                        {
                            var fps = new object[func.Method.GetParameters().Length];
                            for (var i = 0; i < fcount && i < fps.Length; i++)
                            {
                                fps[i] = BlinkBrowserPInvoke.jsArg(fes, i).ToValue(miniblink, fes);
                            }

                            var rs = func.Method.Invoke(func.Target, fps);
                            return rs.ToJsValue(miniblink, fes);
                        }
                    });
                var funcdata = new jsData
                {
                    typeName = "function",
                    callAsFunction = jsfunc,
                    finalize = FunctionFinalize
                };
                _keepref.Add(funcptr.ToInt64(), funcdata);
                Marshal.StructureToPtr(funcdata, funcptr, false);
                return BlinkBrowserPInvoke.jsFunction(es, funcptr);
            }

            var jsobj = BlinkBrowserPInvoke.jsEmptyObject(es);
            var ps = obj.GetType().GetProperties(BindingFlags.Instance | BindingFlags.Public);
            foreach (var p in ps)
            {
                var v = p.GetValue(obj, null);
                if (v == null) continue;
                BlinkBrowserPInvoke.jsSet(es, jsobj, p.Name, v.ToJsValue(miniblink, es));
            }

            return jsobj;
        }

        private static void FunctionFinalize(IntPtr funcptr)
        {
            Marshal.FreeHGlobal(funcptr);
            var key = funcptr.ToInt64();
            _keepref.Remove(key);
        }

        public static T GetCustomAttribute<T>(this MethodInfo method)
        {
            var items = method.GetCustomAttributes(typeof(T), true);
            return items.Length > 0 ? (T)items.First() : default(T);
        }

        public static T GetCustomAttribute<T>(this Type t)
        {
            var items = t.GetCustomAttributes(typeof(T), true);
            return items.Length > 0 ? (T)items.First() : default(T);
        }

        public static Dictionary<string, string> ToDict(this wkeSlist slist)
        {
            var list = new List<string>();

            while (true)
            {
                if (slist.str == IntPtr.Zero)
                {
                    break;
                }

                list.Add(slist.str.ToUTF8String());

                if (slist.next == IntPtr.Zero)
                {
                    break;
                }
                else
                {
                    slist = (wkeSlist)Marshal.PtrToStructure(slist.next, typeof(wkeSlist));
                }
            }

            var map = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
            string k = null;

            foreach (var item in list)
            {
                if (k == null)
                {
                    k = item;
                }
                else
                {
                    map.Add(k, item);
                    k = null;
                }
            }

            return map;
        }

        //private static Dictionary<long, object> _keepref = new Dictionary<long, object>();

        //public static long ToLong(this DateTime time)
        //{
        //    var now = time.ToUniversalTime();
        //    var start = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
        //    return (now.Ticks - start.Ticks) / 10000;
        //}

        //public static DateTime ToDate(this long time)
        //{
        //    var start = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
        //    time = start.Ticks + time * 10000;
        //    return new DateTime(time, DateTimeKind.Utc).ToLocalTime();
        //}

        //public static bool SW(this string str, string value)
        //{
        //    if (str == value) return true;
        //    if (str == null || value == null) return false;
        //    return str.StartsWith(value, StringComparison.OrdinalIgnoreCase);
        //}

        //public static string WKEToUTF8String(this IntPtr ptr)
        //{
        //    return BlinkBrowserPInvoke.wkeGetString(ptr).ToUTF8String();
        //}

        //public static string ToUTF8String(this IntPtr ptr)
        //{
        //    var data = new List<byte>();
        //    var off = 0;
        //    while (true)
        //    {
        //        var ch = Marshal.ReadByte(ptr, off++);
        //        if (ch == 0)
        //        {
        //            break;
        //        }
        //        data.Add(ch);
        //    }
        //    return Encoding.UTF8.GetString(data.ToArray());
        //}

        //public static string ToStringW(this IntPtr ptr)
        //{
        //    return ptr == IntPtr.Zero ? null : Marshal.PtrToStringUni(ptr);
        //}

        //public static object ToValue(this long value, BlinkBrowser miniblink, IntPtr es)
        //{
        //    if (value == 0) return null;

        //    wkeJSType type = BlinkBrowserPInvoke.jsTypeOf(value);
        //    switch (type)
        //    {
        //        case wkeJSType.JSTYPE_NULL:
        //        case wkeJSType.JSTYPE_UNDEFINED:
        //            return null;
        //        case wkeJSType.JSTYPE_NUMBER:
        //            MessageBox.Show("调用JS前台的FUNC返回值为JSTYPE_NUMBER");
        //            return BlinkBrowserPInvoke.jsToDouble(es, value);
        //        case wkeJSType.JSTYPE_BOOLEAN:
        //            MessageBox.Show("调用JS前台的FUNC返回值为JSTYPE_BOOLEAN");
        //            return BlinkBrowserPInvoke.jsToBoolean(es, value);
        //        case wkeJSType.JSTYPE_STRING:
        //            MessageBox.Show("调用JS前台的FUNC返回值为JSTYPE_STRING");
        //            return BlinkBrowserPInvoke.jsToTempStringW(es, value).ToStringW();
        //        case wkeJSType.JSTYPE_FUNCTION:
        //            MessageBox.Show("调用JS前台的FUNC返回值为JSTYPE_FUNCTION");
        //            return new JsFunc(new JsFuncWapper(miniblink, value, es).Call);
        //        case wkeJSType.JSTYPE_ARRAY:
        //            MessageBox.Show("调用JS前台的FUNC返回值为JSTYPE_ARRAY");
        //            var len = BlinkBrowserPInvoke.jsGetLength(es, value);
        //            var array = new object[len];
        //            for (var i = 0; i < array.Length; i++)
        //            {
        //                array[i] = BlinkBrowserPInvoke.jsGetAt(es, value, i).ToValue(miniblink, es);
        //            }

        //            return array;
        //        case wkeJSType.JSTYPE_OBJECT:
        //            MessageBox.Show("调用JS前台的FUNC返回值为OBJECT");
        //            var ptr = BlinkBrowserPInvoke.jsGetKeys(es, value);
        //            var jskeys = (jsKeys) Marshal.PtrToStructure(ptr, typeof(jsKeys));
        //            var keys = Utils.PtrToStringArray(jskeys.keys, jskeys.length);
        //            var exp = new ExpandoObject();
        //            var map = (IDictionary<string, object>) exp;
        //            foreach (var k in keys)
        //            {
        //                map.Add(k, BlinkBrowserPInvoke.jsGet(es, value, k).ToValue(miniblink, es));
        //            }

        //            return map;
        //        default:
        //            throw new NotSupportedException();
        //    }
        //}

        //public static long ToJsValue(this object obj, BlinkBrowser miniblink, IntPtr es)
        //{
        //    if (obj == null)
        //        return BlinkBrowserPInvoke.jsUndefined();
        //    if (obj is int)
        //        return BlinkBrowserPInvoke.jsInt((int) obj);
        //    if (obj is bool)
        //        return BlinkBrowserPInvoke.jsBoolean((bool) obj);
        //    if (obj is double)
        //        return BlinkBrowserPInvoke.jsDouble((double)obj);
        //    if (obj is decimal)
        //    {
        //        var dec = (decimal)obj;
        //        if (dec.ToString().Contains("."))
        //        {
        //            return ToJsValue(Convert.ToDouble(dec.ToString()), miniblink, es);
        //        }
        //        else
        //        {
        //            return ToJsValue(Convert.ToInt32(dec.ToString()), miniblink, es);
        //        }
        //    }
        //    if (obj is long)
        //        return ToJsValue(obj.ToString(), miniblink, es);
        //    if (obj is float)
        //        return BlinkBrowserPInvoke.jsFloat((float) obj);
        //    if (obj is DateTime)
        //        return BlinkBrowserPInvoke.jsDouble(((DateTime) obj).ToLong());
        //    if (obj is string)
        //        return BlinkBrowserPInvoke.jsString(es, obj.ToString());
        //    if (obj is IEnumerable)
        //    {
        //        var list = new List<object>();
        //        foreach (var item in (IEnumerable) obj)
        //            list.Add(item);
        //        var array = BlinkBrowserPInvoke.jsEmptyArray(es);
        //        BlinkBrowserPInvoke.jsSetLength(es, array, list.Count);
        //        for (var i = 0; i < list.Count; i++)
        //        {
        //            BlinkBrowserPInvoke.jsSetAt(es, array, i, list[i].ToJsValue(miniblink, es));
        //        }

        //        return array;
        //    }

        //    if (obj is Delegate)
        //    {
        //        var func = (Delegate)obj;
        //        var funcptr = Marshal.AllocHGlobal(Marshal.SizeOf(typeof(wkeJSData)));
        //        var jsfunc = new jsCallAsFunctionCallback(
        //            (fes, fobj, fargs, fcount) =>
        //            {
        //                if (func is TempNetFunc)
        //                {
        //                    var fps = new List<object>();
        //                    for (var i = 0; i < fcount; i++)
        //                    {
        //                        fps.Add(BlinkBrowserPInvoke.jsArg(fes, i).ToValue(miniblink, fes));
        //                    }

        //                    return ((TempNetFunc)func)(fps.ToArray()).ToJsValue(miniblink, fes);
        //                }
        //                else
        //                {
        //                    var fps = new object[func.Method.GetParameters().Length];
        //                    for (var i = 0; i < fcount && i < fps.Length; i++)
        //                    {
        //                        fps[i] = BlinkBrowserPInvoke.jsArg(fes, i).ToValue(miniblink, fes);
        //                    }

        //                    var rs = func.Method.Invoke(func.Target, fps);
        //                    return rs.ToJsValue(miniblink, fes);
        //                }
        //            });
        //        var funcdata = new jsData
        //        {
        //            typeName = "function",
        //            callAsFunction = jsfunc,
        //            finalize = FunctionFinalize
        //        };
        //        _keepref.Add(funcptr.ToInt64(), funcdata);
        //        Marshal.StructureToPtr(funcdata, funcptr, false);
        //        return BlinkBrowserPInvoke.jsFunction(es, funcptr);
        //    }

        //    var jsobj = BlinkBrowserPInvoke.jsEmptyObject(es);
        //    var ps = obj.GetType().GetProperties(BindingFlags.Instance | BindingFlags.Public);
        //    foreach (var p in ps)
        //    {
        //        var v = p.GetValue(obj, null);
        //        if (v == null) continue;
        //        BlinkBrowserPInvoke.jsSet(es, jsobj, p.Name, v.ToJsValue(miniblink, es));
        //    }

        //    return jsobj;
        //}

        //private static void FunctionFinalize(IntPtr funcptr)
        //{
        //    Marshal.FreeHGlobal(funcptr);
        //    var key = funcptr.ToInt64();
        //    _keepref.Remove(key);
        //}

        //public static T GetCustomAttribute<T>(this MethodInfo method)
        //{
        //    var items = method.GetCustomAttributes(typeof(T), true);
        //    return items.Length > 0 ? (T) items.First() : default(T);
        //}

        //public static T GetCustomAttribute<T>(this Type t)
        //{
        //    var items = t.GetCustomAttributes(typeof(T), true);
        //    return items.Length > 0 ? (T)items.First() : default(T);
        //}

    }
}



