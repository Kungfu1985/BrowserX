using System;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Text;

namespace MiniBlinkPinvoke
{

    [StructLayout(LayoutKind.Sequential)]
    public struct wkeRect
    {
        public int x;
        public int y;
        public int w;
        public int h;
        public Rectangle ToRectangle()
        {
            return new Rectangle(this.x, this.y, this.w, this.h);
        }
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct wkeJSData
    {
        public IntPtr userdata;

        public wkeJSGetPropertyCallback propertyGet;

        public wkeJSSetPropertyCallback propertySet;

        public wkeJSFinalizeCallback finalize;

        public wkeJSCallAsFunctionCallback callAsFunction;
    }

    internal struct jsData
    {
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 100)]
        public string typeName;
        [MarshalAs(UnmanagedType.FunctionPtr)]
        public jsGetPropertyCallback propertyGet;
        [MarshalAs(UnmanagedType.FunctionPtr)]
        public jsSetPropertyCallback propertySet;
        [MarshalAs(UnmanagedType.FunctionPtr)]
        public jsFinalizeCallback finalize;
        [MarshalAs(UnmanagedType.FunctionPtr)]
        public jsCallAsFunctionCallback callAsFunction;
    }

    internal struct wkeSlist
    {
        public IntPtr str;
        public IntPtr next;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct wkeProxy
    {
        public wkeProxyType type;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 100)]
        public string hostname;
        public ushort port;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 50)]
        public string username;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 50)]
        public string password;
    }
    [StructLayout(LayoutKind.Sequential)]
    public struct wkeSettings
    {
        public wkeProxy proxy;
        public wkeSettingMask mask;
    }
    [StructLayout(LayoutKind.Sequential)]
    public struct wkePostBodyElement
    {
        public int size;
        public wkeHttBodyElementType type;
        /// <summary>
        /// 转 wkeMemBuf
        /// </summary>
        public IntPtr data;
        /// <summary>
        /// wkeString
        /// </summary>
        public IntPtr filePath;
        public Int64 fileStart;
        public Int64 fileLength;
    }
    [StructLayout(LayoutKind.Sequential)]
    public struct wkeMemBuf
    {
        public int size;
        public IntPtr data;
        public Int32 length;
    }
    [StructLayout(LayoutKind.Sequential)]
    public struct wkePostBodyElements
    {
        public int size;
        /// <summary>
        /// wkePostBodyElement**
        /// </summary>
        public IntPtr element;
        public int elementSize;
        [MarshalAs(UnmanagedType.I1)]
        public bool isDirty;
    }
    public struct wkeScreenshotSettings
    {
        public int structSize;
        public int width;
        public int height;
    }

    public struct wkeWillSendRequestInfo
    {
        IntPtr url;
        IntPtr newUrl;
        wkeResourceType resourceType;
        int httpResponseCode;
        IntPtr method;
        IntPtr referrer;
        IntPtr headers;
    }

    public struct wkeTempCallbackInfo
    {
        public int size;
        public IntPtr wkeWebFrameHandle;
        wkeWillSendRequestInfo willSendRequestInfo;
        IntPtr url;
        wkePostBodyElements postBody;
        IntPtr job;
    }

    //[StructLayout(LayoutKind.Sequential)]
    //public struct wkeNetJobDataBind
    //{
    //    public IntPtr param;
    //    [MarshalAs(UnmanagedType.FunctionPtr)]
    //    public IntPtr recvCallback;
    //    [MarshalAs(UnmanagedType.FunctionPtr)]
    //    public IntPtr finishCallback;
    //}

    internal struct wkeNetJobDataBind
    {
        IntPtr param;
        [MarshalAs(UnmanagedType.FunctionPtr)]
        public wkeNetJobDataRecvCallback recvCallback;
        [MarshalAs(UnmanagedType.FunctionPtr)]
        public wkeNetJobDataFinishCallback finishCallback;
    }

    internal struct DownloadJobData
    {
        public bool flag;
        public string url;
        public string mime;
        public string disposition;
        public string filename;
        public int datalength;
    }


    [StructLayout(LayoutKind.Sequential)]
    internal struct jsKeys
    {
        public int length;
        public IntPtr keys;
    }

}
