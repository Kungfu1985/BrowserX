using System;
using System.Collections.Generic;
using System.Text;

namespace MiniBlinkPinvoke
{
    public enum wkeProxyType
    {
        WKE_PROXY_NONE,
        WKE_PROXY_HTTP,
        WKE_PROXY_SOCKS4,
        WKE_PROXY_SOCKS4A,
        WKE_PROXY_SOCKS5,
        WKE_PROXY_SOCKS5HOSTNAME
    }
    public enum wkeSettingMask
    {
        WKE_SETTING_PROXY = 1,
        WKE_SETTING_PAINTCALLBACK_IN_OTHER_THREAD = 1 << 2,
        WKE_ENABLE_NODEJS = 1 << 3,
        WKE_ENABLE_DISABLE_H5VIDEO = 1 << 4,
        WKE_ENABLE_DISABLE_PDFVIEW = 1 << 5,
        WKE_ENABLE_DISABLE_CC = 1 << 6
    }
    public enum wkeMouseMessage : uint
    {
        WKE_MSG_MOUSEMOVE = 0x0200,
        WKE_MSG_LBUTTONDOWN = 0x0201,
        WKE_MSG_LBUTTONUP = 0x0202,
        WKE_MSG_LBUTTONDBLCLK = 0x0203,
        WKE_MSG_RBUTTONDOWN = 0x0204,
        WKE_MSG_RBUTTONUP = 0x0205,
        WKE_MSG_RBUTTONDBLCLK = 0x0206,
        WKE_MSG_MBUTTONDOWN = 0x0207,
        WKE_MSG_MBUTTONUP = 0x0208,
        WKE_MSG_MBUTTONDBLCLK = 0x0209,
        WKE_MSG_MOUSEWHEEL = 0x020A,
    }
    public enum wkeMouseFlags
    {
        WKE_LBUTTON = 0x01,
        WKE_RBUTTON = 0x02,
        WKE_SHIFT = 0x04,
        WKE_CONTROL = 0x08,
        WKE_MBUTTON = 0x10,
    }
    public enum wkeWindowType
    {
        WKE_WINDOW_TYPE_POPUP,
        WKE_WINDOW_TYPE_TRANSPARENT,
        WKE_WINDOW_TYPE_CONTROL
    }
    
    public enum wkeJSType
    {
        JSTYPE_NUMBER,
        JSTYPE_STRING,
        JSTYPE_BOOLEAN,
        JSTYPE_OBJECT,
        JSTYPE_FUNCTION,
        JSTYPE_UNDEFINED,
        JSTYPE_ARRAY,
        JSTYPE_NULL
    }

    public enum jsType
    {
        NUMBER = 0,
        STRING = 1,
        BOOLEAN = 2,
        OBJECT = 3,
        FUNCTION = 4,
        UNDEFINED = 5,
        ARRAY = 6,
        NULL = 7
    }

    public enum wkeMessageSource
    {
        WKE_MESSAGE_SOURCE_HTML,
        WKE_MESSAGE_SOURCE_XML,
        WKE_MESSAGE_SOURCE_JS,
        WKE_MESSAGE_SOURCE_NETWORK,
        WKE_MESSAGE_SOURCE_CONSOLE_API,
        WKE_MESSAGE_SOURCE_OTHER
    }
    public enum wkeMessageType
    {
        WKE_MESSAGE_TYPE_LOG,
        WKE_MESSAGE_TYPE_DIR,
        WKE_MESSAGE_TYPE_DIR_XML,
        WKE_MESSAGE_TYPE_TRACE,
        WKE_MESSAGE_TYPE_START_GROUP,
        WKE_MESSAGE_TYPE_START_GROUP_COLLAPSED,
        WKE_MESSAGE_TYPE_END_GROUP,
        WKE_MESSAGE_TYPE_ASSERT
    }
    public enum wkeMessageLevel
    {
        WKE_MESSAGE_LEVEL_TIP,
        WKE_MESSAGE_LEVEL_LOG,
        WKE_MESSAGE_LEVEL_WARNING,
        WKE_MESSAGE_LEVEL_ERROR,
        WKE_MESSAGE_LEVEL_DEBUG
    }
    public enum wkeConsoleLevel
    {
        wkeLevelDebug = 4,
        wkeLevelLog = 1,
        wkeLevelInfo = 5,
        wkeLevelWarning = 2,
        wkeLevelError = 3,
        wkeLevelRevokedError = 6,
        wkeLevelLast = wkeLevelInfo
    }
    public enum wkeNavigationAction
    {
        WKE_NAVIGATION_CONTINUE,
        WKE_NAVIGATION_ABORT,
        WKE_NAVIGATION_DOWNLOAD
    }
    public enum wkeNavigationType
    {
        WKE_NAVIGATION_TYPE_LINKCLICK,
        WKE_NAVIGATION_TYPE_FORMSUBMITTE,
        WKE_NAVIGATION_TYPE_BACKFORWARD,
        WKE_NAVIGATION_TYPE_RELOAD,
        WKE_NAVIGATION_TYPE_FORMRESUBMITT,
        WKE_NAVIGATION_TYPE_OTHER
    }
    /// <summary>
    /// Cookie命令
    /// </summary>
    public enum wkeCookieCommand
    {
        /// <summary>
        /// 清空所有Cookies
        /// </summary>
        ClearAllCookies,
        /// <summary>
        /// 清空会话Cookies
        /// </summary>
        ClearSessionCookies,
        /// <summary>
        /// 将Cookies刷新到文件
        /// </summary>
        FlushCookiesToFile,
        /// <summary>
        /// 从文件重新载入Cookies
        /// </summary>
        ReloadCookiesFromFile
    }

    public enum WkeCursorInfo
    {
        WkeCursorInfoPointer = 0,
        WkeCursorInfoCross = 1,
        WkeCursorInfoHand = 2,
        WkeCursorInfoIBeam = 3,
        WkeCursorInfoWait = 4,
        WkeCursorInfoHelp = 5,
        WkeCursorInfoEastResize = 6,
        WkeCursorInfoNorthResize = 7,
        WkeCursorInfoNorthEastResize = 8,
        WkeCursorInfoNorthWestResize = 9,
        WkeCursorInfoSouthResize = 10,
        WkeCursorInfoSouthEastResize = 11,
        WkeCursorInfoSouthWestResize = 12,
        WkeCursorInfoWestResize = 13,
        WkeCursorInfoNorthSouthResize = 14,
        WkeCursorInfoEastWestResize = 15,
        WkeCursorInfoNorthEastSouthWestResize = 16,
        WkeCursorInfoNorthWestSouthEastResize = 17,
        WkeCursorInfoColumnResize = 18,
        WkeCursorInfoRowResize = 19
    }
    public enum wkeLoadingResult
    {
        WKE_LOADING_SUCCEEDED,
        WKE_LOADING_FAILED,
        WKE_LOADING_CANCELED
    }

    public enum wkeDownloadOpt
    {
        kWkeDownloadOptCancel,
        kWkeDownloadOptCacheData
    }

    public enum wkeHttBodyElementType
    {
        wkeHttBodyElementTypeData,
        wkeHttBodyElementTypeFile
    }
    public enum wkeRequestType
    {
        kWkeRequestTypeInvalidation,
        kWkeRequestTypeGet,
        kWkeRequestTypePost,
        kWkeRequestTypePut
    }

    public enum wkeResourceType
    {
        WKE_RESOURCE_TYPE_MAIN_FRAME = 0,       // top level page
        WKE_RESOURCE_TYPE_SUB_FRAME = 1,        // frame or iframe
        WKE_RESOURCE_TYPE_STYLESHEET = 2,       // a CSS stylesheet
        WKE_RESOURCE_TYPE_SCRIPT = 3,           // an external script
        WKE_RESOURCE_TYPE_IMAGE = 4,            // an image (jpg/gif/png/etc)
        WKE_RESOURCE_TYPE_FONT_RESOURCE = 5,    // a font
        WKE_RESOURCE_TYPE_SUB_RESOURCE = 6,     // an "other" subresource.
        WKE_RESOURCE_TYPE_OBJECT = 7,           // an object (or embed) tag for a plugin,
                                                // or a resource that a plugin requested.
        WKE_RESOURCE_TYPE_MEDIA = 8,            // a media resource.
        WKE_RESOURCE_TYPE_WORKER = 9,           // the main resource of a dedicated
                                                // worker.
        WKE_RESOURCE_TYPE_SHARED_WORKER = 10,   // the main resource of a shared worker.
        WKE_RESOURCE_TYPE_PREFETCH = 11,        // an explicitly requested prefetch
        WKE_RESOURCE_TYPE_FAVICON = 12,         // a favicon
        WKE_RESOURCE_TYPE_XHR = 13,             // a XMLHttpRequest
        WKE_RESOURCE_TYPE_PING = 14,            // a ping request for <a ping>
        WKE_RESOURCE_TYPE_SERVICE_WORKER = 15,  // the main resource of a service worker.
        WKE_RESOURCE_TYPE_LAST_TYPE
    }
    

}
