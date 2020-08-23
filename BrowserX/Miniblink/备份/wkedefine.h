/*
*
* wolar@qq.com
* http://miniblink.net
* https://github.com/weolar/miniblink49
* licence MIT
*
*/

#ifndef WKE_DEFINE_H
#define WKE_DEFINE_H

#include <windows.h>
//////////////////////////////////////////////////////////////////////////
#define WKE_CALL_TYPE __cdecl
//////////////////////////////////////////////////////////////////////////

typedef struct {
	int x;
	int y;
	int w;
	int h;
} wkeRect;

typedef enum {
	WKE_LBUTTON = 0x01,
	WKE_RBUTTON = 0x02,
	WKE_SHIFT = 0x04,
	WKE_CONTROL = 0x08,
	WKE_MBUTTON = 0x10,
} wkeMouseFlags;

typedef enum {
	WKE_EXTENDED = 0x0100,
	WKE_REPEAT = 0x4000,
} wkeKeyFlags;

typedef enum {
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
} wkeMouseMsg;

#if !defined(__cplusplus)
#ifndef HAVE_WCHAR_T
typedef unsigned short wchar_t;
#endif

#ifndef HAVE_BOOL
typedef unsigned char bool;
#define true 1
#define false 0
#endif
#endif

typedef char utf8;
#if !defined(__cplusplus)
typedef void* jsExecState;
#else
struct JsExecStateInfo;
typedef JsExecStateInfo* jsExecState;
#endif
typedef __int64 jsValue;

#if defined(__cplusplus)
namespace wke { class CWebView; };
typedef wke::CWebView* wkeWebView;

namespace wke { class CString; };
typedef wke::CString* wkeString;

#else
struct _tagWkeWebView;
typedef struct _tagWkeWebView* wkeWebView;

struct _tagWkeString;
typedef struct _tagWkeString* wkeString;
#endif

typedef enum {
	WKE_PROXY_NONE,
	WKE_PROXY_HTTP,
	WKE_PROXY_SOCKS4,
	WKE_PROXY_SOCKS4A,
	WKE_PROXY_SOCKS5,
	WKE_PROXY_SOCKS5HOSTNAME
} wkeProxyType;

typedef struct {
	wkeProxyType type;
	char hostname[100];
	unsigned short port;
	char username[50];
	char password[50];
} wkeProxy;

enum wkeSettingMask {
	WKE_SETTING_PROXY = 1,
	WKE_SETTING_PAINTCALLBACK_IN_OTHER_THREAD = 1 << 2,
	WKE_ENABLE_NODEJS = 1 << 3,
	WKE_ENABLE_DISABLE_H5VIDEO = 1 << 4,
	WKE_ENABLE_DISABLE_PDFVIEW = 1 << 5,
	WKE_ENABLE_DISABLE_CC = 1 << 6
};

typedef struct {
	wkeProxy proxy;
	unsigned int mask;
} wkeSettings;

typedef void* wkeWebFrameHandle;

typedef void* (*FILE_OPEN_) (const char* path);
typedef void(*FILE_CLOSE_) (void* handle);
typedef size_t(*FILE_SIZE) (void* handle);
typedef int(*FILE_READ) (void* handle, void* buffer, size_t size);
typedef int(*FILE_SEEK) (void* handle, int offset, int origin);

typedef FILE_OPEN_ WKE_FILE_OPEN;
typedef FILE_CLOSE_ WKE_FILE_CLOSE;
typedef FILE_SIZE WKE_FILE_SIZE;
typedef FILE_READ WKE_FILE_READ;
typedef FILE_SEEK WKE_FILE_SEEK;
typedef bool(*WKE_EXISTS_FILE)(const char * path);

typedef void(*ON_TITLE_CHANGED) (const struct _wkeClientHandler* clientHandler, const wkeString title);
typedef void(*ON_URL_CHANGED) (const struct _wkeClientHandler* clientHandler, const wkeString url);

typedef struct _wkeClientHandler {
	ON_TITLE_CHANGED onTitleChanged;
	ON_URL_CHANGED onURLChanged;
} wkeClientHandler;

typedef bool(*wkeCookieVisitor)(
	void* params,
	const char* name,
	const char* value,
	const char* domain,
	const char* path, // If |path| is non-empty only URLs at or below the path will get the cookie value.
	int secure, // If |secure| is true the cookie will only be sent for HTTPS requests.
	int httpOnly, // If |httponly| is true the cookie will only be sent for HTTP requests.
	int* expires // The cookie expiration date is only valid if |has_expires| is true.
	);

enum wkeCookieCommand {
	wkeCookieCommandClearAllCookies,
	wkeCookieCommandClearSessionCookies,
	wkeCookieCommandFlushCookiesToFile,
	wkeCookieCommandReloadCookiesFromFile,
};

typedef enum {
	WKE_NAVIGATION_TYPE_LINKCLICK,
	WKE_NAVIGATION_TYPE_FORMSUBMITTE,
	WKE_NAVIGATION_TYPE_BACKFORWARD,
	WKE_NAVIGATION_TYPE_RELOAD,
	WKE_NAVIGATION_TYPE_FORMRESUBMITT,
	WKE_NAVIGATION_TYPE_OTHER
} wkeNavigationType;

enum WkeCursorInfoType {
	WkeCursorInfoPointer,
	WkeCursorInfoCross,
	WkeCursorInfoHand,
	WkeCursorInfoIBeam,
	WkeCursorInfoWait,
	WkeCursorInfoHelp,
	WkeCursorInfoEastResize,
	WkeCursorInfoNorthResize,
	WkeCursorInfoNorthEastResize,
	WkeCursorInfoNorthWestResize,
	WkeCursorInfoSouthResize,
	WkeCursorInfoSouthEastResize,
	WkeCursorInfoSouthWestResize,
	WkeCursorInfoWestResize,
	WkeCursorInfoNorthSouthResize,
	WkeCursorInfoEastWestResize,
	WkeCursorInfoNorthEastSouthWestResize,
	WkeCursorInfoNorthWestSouthEastResize,
	WkeCursorInfoColumnResize,
	WkeCursorInfoRowResize,
	WkeCursorInfoMiddlePanning,
	WkeCursorInfoEastPanning,
	WkeCursorInfoNorthPanning,
	WkeCursorInfoNorthEastPanning,
	WkeCursorInfoNorthWestPanning,
	WkeCursorInfoSouthPanning,
	WkeCursorInfoSouthEastPanning,
	WkeCursorInfoSouthWestPanning,
	WkeCursorInfoWestPanning,
	WkeCursorInfoMove,
	WkeCursorInfoVerticalText,
	WkeCursorInfoCell,
	WkeCursorInfoContextMenu,
	WkeCursorInfoAlias,
	WkeCursorInfoProgress,
	WkeCursorInfoNoDrop,
	WkeCursorInfoCopy,
	WkeCursorInfoNone,
	WkeCursorInfoNotAllowed,
	WkeCursorInfoZoomIn,
	WkeCursorInfoZoomOut,
	WkeCursorInfoGrab,
	WkeCursorInfoGrabbing,
	WkeCursorInfoCustom
};

typedef struct {
	int x;
	int y;
	int width;
	int height;

	bool menuBarVisible;
	bool statusBarVisible;
	bool toolBarVisible;
	bool locationBarVisible;
	bool scrollbarsVisible;
	bool resizable;
	bool fullscreen;
} wkeWindowFeatures;

typedef struct _wkeTempCallbackInfo {
    int size;
    wkeWebFrameHandle frame;
    wkeWillSendRequestInfo* willSendRequestInfo;
    const char* url;
    wkePostBodyElements* postBody;
    wkeNetJob job;
} wkeTempCallbackInfo;

typedef void(*wkeTitleChangedCallback)(wkeWebView webView, void* param, const wkeString title);
typedef void(*wkeNetGetFaviconCallback)(wkeWebView webView, void* param, const utf8* url, wkeMemBuf* buf);
typedef wkeTempCallbackInfo(*wkeGetTempCallbackInfo)(wkeWebView webView);


typedef void(*wkeURLChangedCallback)(wkeWebView webView, void* param, const wkeString url);
typedef void(*wkeURLChangedCallback2)(wkeWebView webView, void* param, wkeWebFrameHandle frameId, const wkeString url);
typedef void(*wkePaintUpdatedCallback)(wkeWebView webView, void* param, const HDC hdc, int x, int y, int cx, int cy);
typedef void(*wkeAlertBoxCallback)(wkeWebView webView, void* param, const wkeString msg);
typedef bool(*wkeConfirmBoxCallback)(wkeWebView webView, void* param, const wkeString msg);
typedef bool(*wkePromptBoxCallback)(wkeWebView webView, void* param, const wkeString msg, const wkeString defaultResult, wkeString result);
typedef bool(*wkeNavigationCallback)(wkeWebView webView, void* param, wkeNavigationType navigationType, const wkeString url);
typedef wkeWebView(*wkeCreateViewCallback)(wkeWebView webView, void* param, wkeNavigationType navigationType, const wkeString url, const wkeWindowFeatures* windowFeatures);
typedef void(*wkeDocumentReadyCallback)(wkeWebView webView, void* param);
typedef void(*wkeDocumentReady2Callback)(wkeWebView webView, void* param, wkeWebFrameHandle frameId);

typedef wkeWebView(*wkeCreateViewCallback)(wkeWebView webView, void* param, wkeNavigationType navigationType, const wkeString url, const wkeWindowFeatures* windowFeatures);
typedef void(*wkeDocumentReadyCallback)(wkeWebView webView, void* param);
typedef void(*wkeDocumentReady2Callback)(wkeWebView webView, void* param, wkeWebFrameHandle frameId);

typedef enum {
	WKE_LOADING_SUCCEEDED,
	WKE_LOADING_FAILED,
	WKE_LOADING_CANCELED
} wkeLoadingResult;

typedef enum _wkeDownloadOpt {
	kWkeDownloadOptCancel,
	kWkeDownloadOptCacheData,
} wkeDownloadOpt;

typedef void* wkeNetJob;

typedef void(WKE_CALL_TYPE* wkeNetJobDataRecvCallback)(void* ptr, wkeNetJob job, const char* data, int length);
typedef void(WKE_CALL_TYPE* wkeNetJobDataFinishCallback)(void* ptr, wkeNetJob job, wkeLoadingResult result);


typedef struct _wkeNetJobDataBind {
	void* param;
	wkeNetJobDataRecvCallback recvCallback;
	wkeNetJobDataFinishCallback finishCallback;
}wkeNetJobDataBind;


typedef void(*wkeLoadingFinishCallback)(wkeWebView webView, void* param, const wkeString url, wkeLoadingResult result, const wkeString failedReason);
typedef bool(*wkeDownloadCallback)(wkeWebView webView, void* param, const char* url);
typedef wkeDownloadOpt(WKE_CALL_TYPE*wkeDownload2Callback)(
	wkeWebView webView,
	void* param,
	size_t expectedContentLength,
	const char* url,
	const char* mime,
	const char* disposition,
	wkeNetJob job,
	wkeNetJobDataBind* dataBind);

typedef enum {
	wkeLevelDebug = 4,
	wkeLevelLog = 1,
	wkeLevelInfo = 5,
	wkeLevelWarning = 2,
	wkeLevelError = 3,
	wkeLevelRevokedError = 6,
	wkeLevelLast = wkeLevelInfo
} wkeConsoleLevel;
typedef void(*wkeConsoleCallback)(wkeWebView webView, void* param, wkeConsoleLevel level, const wkeString message, const wkeString sourceName, unsigned sourceLine, const wkeString stackTrace);

typedef void(*wkeOnCallUiThread)(wkeWebView webView, void* paramOnInThread);
typedef void(*wkeCallUiThread)(wkeWebView webView, wkeOnCallUiThread func, void* param);

//wkeNet--------------------------------------------------------------------------------------
typedef bool(*wkeLoadUrlBeginCallback)(wkeWebView webView, void* param, const char *url, void *job);
typedef void(*wkeLoadUrlEndCallback)(wkeWebView webView, void* param, const char *url, void *job, void* buf, int len);
typedef void(*wkeDidCreateScriptContextCallback)(wkeWebView webView, void* param, wkeWebFrameHandle frameId, void* context, int extensionGroup, int worldId);
typedef void(*wkeWillReleaseScriptContextCallback)(wkeWebView webView, void* param, wkeWebFrameHandle frameId, void* context, int worldId);
typedef bool(*wkeNetResponseCallback)(wkeWebView webView, void* param, const char* url, void* job);

typedef void* v8ContextPtr;
typedef void* v8Isolate;

//wkewindow-----------------------------------------------------------------------------------
typedef enum {
	WKE_WINDOW_TYPE_POPUP,
	WKE_WINDOW_TYPE_TRANSPARENT,
	WKE_WINDOW_TYPE_CONTROL
} wkeWindowType;

typedef bool(*wkeWindowClosingCallback)(wkeWebView webWindow, void* param);
typedef void(*wkeWindowDestroyCallback)(wkeWebView webWindow, void* param);

//JavaScript Bind-----------------------------------------------------------------------------------
#define JS_CALL __fastcall
typedef jsValue(JS_CALL* jsNativeFunction) (jsExecState es);

typedef jsValue(*wkeJsNativeFunction) (jsExecState es, void* param);

typedef enum {
	JSTYPE_NUMBER,
	JSTYPE_STRING,
	JSTYPE_BOOLEAN,
	JSTYPE_OBJECT,
	JSTYPE_FUNCTION,
	JSTYPE_UNDEFINED,
	JSTYPE_ARRAY,
} jsType;

// cexer JS���󡢺�����֧��
typedef jsValue(*jsGetPropertyCallback)(jsExecState es, jsValue object, const char* propertyName);
typedef bool(*jsSetPropertyCallback)(jsExecState es, jsValue object, const char* propertyName, jsValue value);
typedef jsValue(*jsCallAsFunctionCallback)(jsExecState es, jsValue object, jsValue* args, int argCount);
typedef void(*jsFinalizeCallback)(struct tagjsData* data);

typedef struct tagjsData {
	char typeName[100];
	jsGetPropertyCallback propertyGet;
	jsSetPropertyCallback propertySet;
	jsFinalizeCallback finalize;
	jsCallAsFunctionCallback callAsFunction;
} jsData;


namespace wke {

	class IWebView {
	public:
		virtual void destroy() = 0;

		virtual const char* name() const = 0;
		virtual void setName(const char* name) = 0;

		virtual bool isTransparent() const = 0;
		virtual void setTransparent(bool transparent) = 0;

		virtual void loadURL(const utf8* url) = 0;
		virtual void loadURL(const wchar_t* url) = 0;

		virtual void loadHTML(const utf8* html) = 0;
		virtual void loadHTML(const wchar_t* html) = 0;

		virtual void loadFile(const utf8* filename) = 0;
		virtual void loadFile(const wchar_t* filename) = 0;

		virtual const utf8* url() const = 0;

		virtual bool isLoading() const = 0;        /*document load sucessed*/
		virtual bool isLoadingFailed() const = 0;    /*document load failed*/
		virtual bool isLoadingSucceeded() const = 0;  /*document load complete*/
		virtual bool isDocumentReady() const = 0; /*document ready*/
		virtual void stopLoading() = 0;
		virtual void reload() = 0;

		virtual const utf8* title() = 0;
		virtual const wchar_t* titleW() = 0;

		virtual void resize(int w, int h) = 0;
		virtual int width() const = 0;   /*viewport width*/
		virtual int height() const = 0;  /*viewport height*/

		virtual int contentsWidth() const = 0;  /*contents width*/
		virtual int contentsHeight() const = 0; /*contents height*/

		virtual void setDirty(bool dirty) = 0;
		virtual bool isDirty() const = 0;
		virtual void addDirtyArea(int x, int y, int w, int h) = 0;

		virtual void layoutIfNeeded() = 0;
		virtual void paint(void* bits, int pitch) = 0;

		virtual bool canGoBack() const = 0;
		virtual bool goBack() = 0;
		virtual bool canGoForward() const = 0;
		virtual bool goForward() = 0;

		virtual void editorSelectAll() = 0;
		virtual void editorUnSelect() = 0;
		virtual void editorCopy() = 0;
		virtual void editorCut() = 0;
		virtual void editorPaste() = 0;
		virtual void editorDelete() = 0;
		virtual void editorUndo() = 0;
		virtual void editorRedo() = 0;

		virtual void setCookieEnabled(bool enable) = 0;
		virtual bool isCookieEnabled() const = 0;

		virtual void setMediaVolume(float volume) = 0;
		virtual float mediaVolume() const = 0;

		virtual bool fireMouseEvent(unsigned int message, int x, int y, unsigned int flags) = 0;
		virtual bool fireContextMenuEvent(int x, int y, unsigned int flags) = 0;
		virtual bool fireMouseWheelEvent(int x, int y, int delta, unsigned int flags) = 0;
		virtual bool fireKeyUpEvent(unsigned int virtualKeyCode, unsigned int flags, bool systemKey) = 0;
		virtual bool fireKeyDownEvent(unsigned int virtualKeyCode, unsigned int flags, bool systemKey) = 0;
		virtual bool fireKeyPressEvent(unsigned int virtualKeyCode, unsigned int flags, bool systemKey) = 0;

		virtual void setFocus() = 0;
		virtual void killFocus() = 0;

		virtual wkeRect getCaret() = 0;

		virtual jsValue runJS(const utf8* script) = 0;
		virtual jsValue runJS(const wchar_t* script) = 0;
		virtual jsExecState globalExec() = 0;

		virtual void sleep() = 0; //moveOffscreen
		virtual void wake() = 0; //moveOnscreen
		virtual bool isAwake() const = 0;

		virtual void setZoomFactor(float factor) = 0;
		virtual float zoomFactor() const = 0;

		virtual void setEditable(bool editable) = 0;

		virtual void setClientHandler(const wkeClientHandler* handler) = 0;
		virtual const wkeClientHandler* getClientHandler() const = 0;
	};

}

//////////////////////////////////////////////////////////////////////////

#define WKE_CALL_TYPE __cdecl

#define WKE_DEFINE_ITERATOR0(returnVal, name, description) \
    typedef returnVal(* FN_##name)(); \
    __declspec(selectany) FN_##name name;

#define WKE_DEFINE_ITERATOR1(returnVal, name, p1, description) \
    typedef returnVal(WKE_CALL_TYPE* FN_##name)(p1); \
    __declspec(selectany) FN_##name name;

#define WKE_DEFINE_ITERATOR2(returnVal, name, p1, p2, description) \
    typedef returnVal(WKE_CALL_TYPE* FN_##name)(p1, p2); \
    __declspec(selectany) FN_##name name;

#define WKE_DEFINE_ITERATOR3(returnVal, name, p1, p2, p3, description) \
    typedef returnVal(WKE_CALL_TYPE* FN_##name)(p1, p2, p3); \
    __declspec(selectany) FN_##name name;

#define WKE_DEFINE_ITERATOR4(returnVal, name, p1, p2, p3, p4, description) \
    typedef returnVal(WKE_CALL_TYPE* FN_##name)(p1, p2, p3, p4); \
    __declspec(selectany) FN_##name name;

#define WKE_DEFINE_ITERATOR5(returnVal, name, p1, p2, p3, p4, p5, description) \
    typedef returnVal(WKE_CALL_TYPE* FN_##name)(p1, p2, p3, p4, p5); \
    __declspec(selectany) FN_##name name;

#define WKE_DEFINE_ITERATOR6(returnVal, name, p1, p2, p3, p4, p5, p6, description) \
    typedef returnVal(WKE_CALL_TYPE* FN_##name)(p1, p2, p3, p4, p5, p6); \
    __declspec(selectany) FN_##name name;

#define WKE_DEFINE_ITERATOR11(returnVal, name, p1, p2, p3, p4, p5, p6, p7, p8, p9, p10, p11, description) \
    typedef returnVal(WKE_CALL_TYPE* FN_##name)(p1, p2, p3, p4, p5, p6, p7, p8, p9, p10, p11); \
    __declspec(selectany) FN_##name name;

// ---

#define WKE_DECLARE_ITERATOR0(returnVal, name, description) \
    extern "C" __declspec(dllexport) returnVal WKE_CALL_TYPE name();

#define WKE_DECLARE_ITERATOR1(returnVal, name, p1, description) \
    extern "C" __declspec(dllexport) returnVal WKE_CALL_TYPE name(p1);

#define WKE_DECLARE_ITERATOR2(returnVal, name, p1, p2, description) \
    extern "C" __declspec(dllexport) returnVal WKE_CALL_TYPE name(p1, p2);

#define WKE_DECLARE_ITERATOR3(returnVal, name, p1, p2, p3, description) \
    extern "C" __declspec(dllexport) returnVal WKE_CALL_TYPE name(p1, p2, p3);

#define WKE_DECLARE_ITERATOR4(returnVal, name, p1, p2, p3, p4, description) \
    extern "C" __declspec(dllexport) returnVal WKE_CALL_TYPE name(p1, p2, p3, p4);

#define WKE_DECLARE_ITERATOR5(returnVal, name, p1, p2, p3, p4, p5, description) \
    extern "C" __declspec(dllexport) returnVal WKE_CALL_TYPE name(p1, p2, p3, p4, p5);

#define WKE_DECLARE_ITERATOR6(returnVal, name, p1, p2, p3, p4, p5, p6, description) \
    extern "C" __declspec(dllexport) returnVal WKE_CALL_TYPE name(p1, p2, p3, p4, p5, p6);

#define WKE_DECLARE_ITERATOR11(returnVal, name, p1, p2, p3, p4, p5, p6, p7, p8, p9, p10, p11, description) \
    extern "C" __declspec(dllexport) returnVal WKE_CALL_TYPE name(p1, p2, p3, p4, p5, p6, p7, p8, p9, p10, p11);

// ---

#define WKE_GET_PTR_ITERATOR(name) \
    name = (FN_##name)::GetProcAddress(hMod, #name); \
    if (!name) \
        MessageBoxA(nullptr, "wke api not found", #name, 0);

#define WKE_GET_PTR_ITERATOR0(returnVal, name, description) \
    WKE_GET_PTR_ITERATOR(name);

#define WKE_GET_PTR_ITERATOR1(returnVal, name, p1, description) \
    WKE_GET_PTR_ITERATOR(name);

#define WKE_GET_PTR_ITERATOR2(returnVal, name, p1, p2, description) \
    WKE_GET_PTR_ITERATOR(name);

#define WKE_GET_PTR_ITERATOR3(returnVal, name, p1, p2, p3, description) \
    WKE_GET_PTR_ITERATOR(name);

#define WKE_GET_PTR_ITERATOR4(returnVal, name, p1, p2, p3, p4, description) \
    WKE_GET_PTR_ITERATOR(name);

#define WKE_GET_PTR_ITERATOR5(returnVal, name, p1, p2, p3, p4, p5, description) \
    WKE_GET_PTR_ITERATOR(name);

#define WKE_GET_PTR_ITERATOR6(returnVal, name, p1, p2, p3, p4, p5, p6, description) \
    WKE_GET_PTR_ITERATOR(name);

#define WKE_GET_PTR_ITERATOR11(returnVal, name, p1, p2, p3, p4, p5, p6, p7, p8, p9, p10, p11, description) \
    WKE_GET_PTR_ITERATOR(name);

// ������wke�ĵ�����������ʽ���ա��������͡�������������������������

#define WKE_FOR_EACH_DEFINE_FUNCTION(ITERATOR0, ITERATOR1, ITERATOR2, ITERATOR3, ITERATOR4, ITERATOR5, ITERATOR6, ITERATOR11) \
    ITERATOR0(void, wkeShutdown, "") \
    \
    ITERATOR0(unsigned int, wkeVersion, "") \
    ITERATOR0(const utf8*, wkeVersionString, "") \
    ITERATOR2(void, wkeGC, wkeWebView webView, long delayMs, "") \
    ITERATOR5(void, wkeSetFileSystem, WKE_FILE_OPEN pfnOpen, WKE_FILE_CLOSE pfnClose, WKE_FILE_SIZE pfnSize, WKE_FILE_READ pfnRead, WKE_FILE_SEEK pfnSeek, "") \
    \
    ITERATOR1(const char*, wkeWebViewName, wkeWebView webView, "") \
    ITERATOR2(void, wkeSetWebViewName, wkeWebView webView, const char* name, "") \
    \
    ITERATOR1(bool, wkeIsLoaded, wkeWebView webView, "") \
    ITERATOR1(bool, wkeIsLoadFailed, wkeWebView webView, "") \
    ITERATOR1(bool, wkeIsLoadComplete, wkeWebView webView, "") \
    \
    ITERATOR1(const utf8*, wkeGetSource, wkeWebView webView, "") \
    ITERATOR1(const utf8*, wkeTitle, wkeWebView webView, "") \
    ITERATOR1(const wchar_t*, wkeTitleW, wkeWebView webView, "") \
    ITERATOR1(int, wkeWidth, wkeWebView webView, "") \
    ITERATOR1(int, wkeHeight, wkeWebView webView, "") \
    ITERATOR1(int, wkeContentsWidth, wkeWebView webView, "") \
    ITERATOR1(int, wkeContentsHeight, wkeWebView webView, "") \
    \
    ITERATOR1(void, wkeSelectAll, wkeWebView webView, "") \
    ITERATOR1(void, wkeCopy, wkeWebView webView, "") \
    ITERATOR1(void, wkeCut, wkeWebView webView, "") \
    ITERATOR1(void, wkePaste, wkeWebView webView, "") \
    ITERATOR1(void, wkeDelete, wkeWebView webView, "") \
    \
    ITERATOR1(bool, wkeCookieEnabled, wkeWebView webView, "") \
    ITERATOR1(float, wkeMediaVolume, wkeWebView webView, "") \
    \
    ITERATOR5(bool, wkeMouseEvent, wkeWebView webView, unsigned int message, int x, int y, unsigned int flags, "") \
    ITERATOR4(bool, wkeContextMenuEvent, wkeWebView webView, int x, int y, unsigned int flags, "") \
    ITERATOR5(bool, wkeMouseWheel, wkeWebView webView, int x, int y, int delta, unsigned int flags, "") \
    ITERATOR4(bool, wkeKeyUp, wkeWebView webView, unsigned int virtualKeyCode, unsigned int flags, bool systemKey, "") \
    ITERATOR4(bool, wkeKeyDown, wkeWebView webView, unsigned int virtualKeyCode, unsigned int flags, bool systemKey, "") \
    ITERATOR4(bool, wkeKeyPress, wkeWebView webView, unsigned int virtualKeyCode, unsigned int flags, bool systemKey, "") \
    \
    ITERATOR1(void, wkeFocus, wkeWebView webView, "") \
    ITERATOR1(void, wkeUnfocus, wkeWebView webView, "") \
    \
    ITERATOR1(wkeRect, wkeGetCaret, wkeWebView webView, "") \
    \
    ITERATOR1(void, wkeAwaken, wkeWebView webView, "") \
    \
    ITERATOR1(float, wkeZoomFactor, wkeWebView webView, "") \
    \
    ITERATOR2(void, wkeSetClientHandler, wkeWebView webView, const wkeClientHandler* handler, "") \
    ITERATOR1(const wkeClientHandler*, wkeGetClientHandler, wkeWebView webView, "") \
    \
    ITERATOR1(const utf8*, wkeToString, const wkeString string, "") \
    ITERATOR1(const wchar_t*, wkeToStringW, const wkeString string, "") \
    \
    ITERATOR2(const utf8*, jsToString, jsExecState es, jsValue v, "") \
    ITERATOR2(const wchar_t*, jsToStringW, jsExecState es, jsValue v, "") \
    \
    ITERATOR1(void, wkeConfigure, const wkeSettings* settings, "") \
    ITERATOR0(bool, wkeIsInitialize, "") \
    \
    ITERATOR2(void, wkeSetDebugConfig, wkeWebView webView, const char* debugString, "") \
    \
    ITERATOR0(void, wkeFinalize, "") \
    ITERATOR0(void, wkeUpdate, "") \
    ITERATOR0(unsigned int, wkeGetVersion, "") \
    ITERATOR0(const utf8*, wkeGetVersionString, "") \
    \
    ITERATOR0(wkeWebView, wkeCreateWebView, "") \
    ITERATOR1(void, wkeDestroyWebView, wkeWebView webView, "") \
    \
    ITERATOR2(void, wkeSetMemoryCacheEnable, wkeWebView webView, bool b, "") \
    ITERATOR2(void, wkeSetTouchEnabled, wkeWebView webView, bool b, "") \
    ITERATOR2(void, wkeSetNavigationToNewWindowEnable, wkeWebView webView, bool b, "") \
    ITERATOR2(void, wkeSetCspCheckEnable, wkeWebView webView, bool b, "") \
    ITERATOR2(void, wkeSetNpapiPluginsEnabled, wkeWebView webView, bool b, "") \
    ITERATOR2(void, wkeSetHeadlessEnabled, wkeWebView webView, bool b, "���Թر���Ⱦ") \
    ITERATOR2(void, wkeSetViewNetInterface, wkeWebView webView, const char* netInterface, "") \
    \
    ITERATOR1(void, wkeSetProxy, const wkeProxy* proxy, "") \
    ITERATOR2(void, wkeSetViewProxy, wkeWebView webView, wkeProxy *proxy, "") \
    \
    ITERATOR1(const char*, wkeGetName, wkeWebView webView, "") \
    ITERATOR2(void, wkeSetName, wkeWebView webView, const char* name, "") \
    \
    ITERATOR2(void, wkeSetHandle, wkeWebView webView, HWND wnd, "") \
    ITERATOR3(void, wkeSetHandleOffset, wkeWebView webView, int x, int y, "") \
    \
    ITERATOR1(bool, wkeIsTransparent, wkeWebView webView, "") \
    ITERATOR2(void, wkeSetTransparent, wkeWebView webView, bool transparent, "") \
    \
    ITERATOR2(void, wkeSetUserAgent, wkeWebView webView, const utf8* userAgent, "") \
    ITERATOR2(void, wkeSetUserAgentW, wkeWebView webView, const wchar_t* userAgent, "") \
    \
    ITERATOR2(void, wkeLoadW, wkeWebView webView, const wchar_t* url, "") \
    ITERATOR2(void, wkeLoadURL, wkeWebView webView, const utf8* url, "") \
    ITERATOR2(void, wkeLoadURLW, wkeWebView webView, const wchar_t* url, "") \
    ITERATOR4(void, wkePostURL, wkeWebView wkeView, const utf8* url, const char* postData, int postLen, "") \
    ITERATOR4(void, wkePostURLW, wkeWebView wkeView, const wchar_t* url, const char* postData, int postLen, "") \
    \
    ITERATOR2(void, wkeLoadHTML, wkeWebView webView, const utf8* html, "") \
    ITERATOR2(void, wkeLoadHTMLW, wkeWebView webView, const wchar_t* html, "") \
    \
    ITERATOR2(void, wkeLoadFile, wkeWebView webView, const utf8* filename, "") \
    ITERATOR2(void, wkeLoadFileW, wkeWebView webView, const wchar_t* filename, "") \
    \
    ITERATOR1(const utf8*, wkeGetURL, wkeWebView webView, "") \
    \
    ITERATOR1(bool, wkeIsLoading, wkeWebView webView, "") \
    ITERATOR1(bool, wkeIsLoadingSucceeded, wkeWebView webView, "") \
    ITERATOR1(bool, wkeIsLoadingFailed, wkeWebView webView, "") \
    ITERATOR1(bool, wkeIsLoadingCompleted, wkeWebView webView, "") \
    ITERATOR1(bool, wkeIsDocumentReady, wkeWebView webView, "") \
    ITERATOR1(void, wkeStopLoading, wkeWebView webView, "") \
    ITERATOR1(void, wkeReload, wkeWebView webView, "") \
    \
    ITERATOR1(const utf8*, wkeGetTitle, wkeWebView webView, "") \
    ITERATOR1(const wchar_t*, wkeGetTitleW, wkeWebView webView, "") \
    \
    ITERATOR3(wkeMemBuf*, wkeCreateMemBuf, wkeWebView webView, void* buf, size_t length, "") \
    ITERATOR1(void, wkeFreeMemBuf, wkeMemBuf* buf, "") \
    \
    ITERATOR3(void, wkeResize, wkeWebView webView, int w, int h, "") \
    ITERATOR1(int, wkeGetWidth, wkeWebView webView, "") \
    ITERATOR1(int, wkeGetHeight, wkeWebView webView, "") \
    ITERATOR1(int, wkeGetContentWidth, wkeWebView webView, "") \
    ITERATOR1(int, wkeGetContentHeight, wkeWebView webView, "") \
    \
    ITERATOR2(void, wkeSetDirty, wkeWebView webView, bool dirty, "") \
    ITERATOR1(bool, wkeIsDirty, wkeWebView webView, "") \
    ITERATOR5(void, wkeAddDirtyArea, wkeWebView webView, int x, int y, int w, int h, "") \
    ITERATOR1(void, wkeLayoutIfNeeded, wkeWebView webView, "") \
    ITERATOR11(void, wkePaint2, wkeWebView webView, void* bits, int bufWid, int bufHei, int xDst, int yDst, int w, int h, int xSrc, int ySrc, bool bCopyAlpha, "") \
    ITERATOR3(void, wkePaint, wkeWebView webView, void* bits, int pitch, "") \
    ITERATOR1(void, wkeRepaintIfNeeded, wkeWebView webView, "") \
    ITERATOR1(HDC, wkeGetViewDC, wkeWebView webView, "") \
    ITERATOR1(HWND, wkeGetHostHWND, wkeWebView webView, "") \
    \
    ITERATOR1(bool, wkeCanGoBack, wkeWebView webView, "") \
    ITERATOR1(bool, wkeGoBack, wkeWebView webView, "") \
    ITERATOR1(bool, wkeCanGoForward, wkeWebView webView, "") \
    ITERATOR1(bool, wkeGoForward, wkeWebView webView, "") \
    \
    ITERATOR1(void, wkeEditorSelectAll, wkeWebView webView, "") \
    ITERATOR1(void, wkeEditorUnSelect, wkeWebView webView, "") \
    ITERATOR1(void, wkeEditorCopy, wkeWebView webView, "") \
    ITERATOR1(void, wkeEditorCut, wkeWebView webView, "") \
    ITERATOR1(void, wkeEditorPaste, wkeWebView webView, "") \
    ITERATOR1(void, wkeEditorDelete, wkeWebView webView, "") \
    ITERATOR1(void, wkeEditorUndo, wkeWebView webView, "") \
    ITERATOR1(void, wkeEditorRedo, wkeWebView webView, "") \
    \
    ITERATOR1(const wchar_t*, wkeGetCookieW, wkeWebView webView, "") \
    ITERATOR1(const utf8*, wkeGetCookie, wkeWebView webView, "") \
    ITERATOR3(void, wkeSetCookie, wkeWebView webView, const utf8* url, const utf8* cookie, "cookie��ʽ������:Set-cookie: PRODUCTINFO=webxpress; domain=.fidelity.com; path=/; secure") \
    ITERATOR2(void, wkeVisitAllCookie, void* params, wkeCookieVisitor visitor, "") \
    ITERATOR1(void, wkePerformCookieCommand, wkeCookieCommand command, "") \
    ITERATOR2(void, wkeSetCookieEnabled, wkeWebView webView, bool enable, "") \
    ITERATOR1(bool, wkeIsCookieEnabled, wkeWebView webView, "") \
    ITERATOR2(void, wkeSetCookieJarPath, wkeWebView webView, const WCHAR* path, "") \
    ITERATOR2(void, wkeSetCookieJarFullPath, wkeWebView webView, const WCHAR* path, "") \
    ITERATOR2(void, wkeSetLocalStorageFullPath, wkeWebView webView, const WCHAR* path, "") \
    \
    ITERATOR2(void, wkeSetMediaVolume, wkeWebView webView, float volume, "") \
    ITERATOR1(float, wkeGetMediaVolume, wkeWebView webView, "") \
    \
    ITERATOR5(bool, wkeFireMouseEvent, wkeWebView webView, unsigned int message, int x, int y, unsigned int flags, "") \
    ITERATOR4(bool, wkeFireContextMenuEvent, wkeWebView webView, int x, int y, unsigned int flags, "") \
    ITERATOR5(bool, wkeFireMouseWheelEvent, wkeWebView webView, int x, int y, int delta, unsigned int flags, "") \
    ITERATOR4(bool, wkeFireKeyUpEvent, wkeWebView webView, unsigned int virtualKeyCode, unsigned int flags, bool systemKey, "") \
    ITERATOR4(bool, wkeFireKeyDownEvent, wkeWebView webView, unsigned int virtualKeyCode, unsigned int flags, bool systemKey, "") \
    ITERATOR4(bool, wkeFireKeyPressEvent, wkeWebView webView, unsigned int charCode, unsigned int flags, bool systemKey, "") \
    ITERATOR6(bool, wkeFireWindowsMessage, wkeWebView webView, HWND hWnd, UINT message, WPARAM wParam, LPARAM lParam, LRESULT* result, "") \
    \
    ITERATOR1(void, wkeSetFocus, wkeWebView webView, "") \
    ITERATOR1(void, wkeKillFocus, wkeWebView webView, "") \
    \
    ITERATOR1(wkeRect, wkeGetCaretRect, wkeWebView webView, "") \
    \
    ITERATOR2(jsValue, wkeRunJS, wkeWebView webView, const utf8* script, "") \
    ITERATOR2(jsValue, wkeRunJSW, wkeWebView webView, const wchar_t* script, "") \
    \
    ITERATOR1(jsExecState, wkeGlobalExec, wkeWebView webView, "") \
    \
    ITERATOR1(void, wkeSleep, wkeWebView webView, "") \
    ITERATOR1(void, wkeWake, wkeWebView webView, "") \
    ITERATOR1(bool, wkeIsAwake, wkeWebView webView, "") \
    \
    ITERATOR2(void, wkeSetZoomFactor, wkeWebView webView, float factor, "") \
    ITERATOR1(float, wkeGetZoomFactor, wkeWebView webView, "") \
    \
    ITERATOR2(void, wkeSetEditable, wkeWebView webView, bool editable, "") \
    \
    ITERATOR1(const utf8*, wkeGetString, const wkeString string, "") \
    ITERATOR1(const wchar_t*, wkeGetStringW, const wkeString string, "") \
    \
    ITERATOR3(void, wkeSetString, wkeString string, const utf8* str, size_t len, "") \
    ITERATOR3(void, wkeSetStringW, wkeString string, const wchar_t* str, size_t len, "") \
    \
    ITERATOR2(wkeString, wkeCreateStringW, const wchar_t* str, size_t len, "") \
    ITERATOR1(void, wkeDeleteString, wkeString str, "") \
    \
    ITERATOR0(wkeWebView, wkeGetWebViewForCurrentContext, "") \
    ITERATOR3(void, wkeSetUserKeyValue, wkeWebView webView, const char* key, void* value, "") \
    ITERATOR2(void*, wkeGetUserKeyValue, wkeWebView webView, const char* key, "") \
    \
    ITERATOR1(int, wkeGetCursorInfoType, wkeWebView webView, "") \
	ITERATOR1(wkeTempCallbackInfo*, wkeGetTempCallbackInfo, wkeWebView webView, "") \
    ITERATOR5(void, wkeSetDragFiles, wkeWebView webView, const POINT* clintPos, const POINT* screenPos, wkeString files[], int filesCount, "") \
    \
    ITERATOR3(void, wkeOnMouseOverUrlChanged, wkeWebView webView, wkeTitleChangedCallback callback, void* callbackParam, "") \
    ITERATOR3(void, wkeOnTitleChanged, wkeWebView webView, wkeTitleChangedCallback callback, void* callbackParam, "") \
	ITERATOR3(void, wkeOnNetGetFavicon, mbWebView webView, wkeNetGetFaviconCallback callback, void* param, "") \
    ITERATOR3(void, wkeOnURLChanged, wkeWebView webView, wkeURLChangedCallback callback, void* callbackParam, "") \
    ITERATOR3(void, wkeOnURLChanged2, wkeWebView webView, wkeURLChangedCallback2 callback, void* callbackParam, "") \
    ITERATOR3(void, wkeOnPaintUpdated, wkeWebView webView, wkePaintUpdatedCallback callback, void* callbackParam, "") \
    ITERATOR3(void, wkeOnAlertBox, wkeWebView webView, wkeAlertBoxCallback callback, void* callbackParam, "") \
    ITERATOR3(void, wkeOnConfirmBox, wkeWebView webView, wkeConfirmBoxCallback callback, void* callbackParam, "") \
    ITERATOR3(void, wkeOnPromptBox, wkeWebView webView, wkePromptBoxCallback callback, void* callbackParam, "") \
    ITERATOR3(void, wkeOnNavigation, wkeWebView webView, wkeNavigationCallback callback, void* param, "") \
    ITERATOR3(void, wkeOnCreateView, wkeWebView webView, wkeCreateViewCallback callback, void* param, "") \
    ITERATOR3(void, wkeOnDocumentReady, wkeWebView webView, wkeDocumentReadyCallback callback, void* param, "") \
    ITERATOR3(void, wkeOnDocumentReady2, wkeWebView webView, wkeDocumentReady2Callback callback, void* param, "") \
    ITERATOR3(void, wkeOnLoadingFinish, wkeWebView webView, wkeLoadingFinishCallback callback, void* param, "") \
    ITERATOR3(void, wkeOnDownload, wkeWebView webView, wkeDownloadCallback callback, void* param, "") \
	ITERATOR3(void, wkeOnDownload2, wkeWebView webView, wkeDownload2Callback callback, void* param, "") \
    ITERATOR3(void, wkeOnConsole, wkeWebView webView, wkeConsoleCallback callback, void* param, "") \
    ITERATOR3(void, wkeSetUIThreadCallback, wkeWebView webView, wkeCallUiThread callback, void* param, "") \
    ITERATOR3(void, wkeOnLoadUrlBegin, wkeWebView webView, wkeLoadUrlBeginCallback callback, void* callbackParam, "") \
    ITERATOR3(void, wkeOnLoadUrlEnd, wkeWebView webView, wkeLoadUrlEndCallback callback, void* callbackParam, "") \
    ITERATOR3(void, wkeOnDidCreateScriptContext, wkeWebView webView, wkeDidCreateScriptContextCallback callback, void* callbackParam, "") \
    ITERATOR3(void, wkeOnWillReleaseScriptContext, wkeWebView webView, wkeWillReleaseScriptContextCallback callback, void* callbackParam, "") \
    ITERATOR3(void, wkeOnWindowClosing, wkeWebView webWindow, wkeWindowClosingCallback callback, void* param, "") \
    ITERATOR3(void, wkeOnWindowDestroy, wkeWebView webWindow, wkeWindowDestroyCallback callback, void* param, "") \
    \
    ITERATOR2(void, wkeNetSetMIMEType, void* job, char *type, "") \
    ITERATOR4(void, wkeNetSetHTTPHeaderField, void* job, wchar_t* key, wchar_t* value, bool response, "") \
    ITERATOR2(void, wkeNetSetURL, void* job, const char *url, "") \
    ITERATOR3(void, wkeNetSetData, void* job, void *buf, int len, "���ô˺�����,������յ����ݻ�洢��һbuf��,����������ɺ���ӦOnLoadUrlEnd�¼�.#�˵�������Ӱ������,����" \
        "�˺�����wkeNetSetData�������ǣ�wkeNetHookRequest���ڽ��ܵ������������ݺ��ٵ��ûص���������ص��޸��������ݡ�"\
        "��wkeNetSetData�����������ݻ�û���͵�ʱ���޸�") \
    ITERATOR1(void, wkeNetHookRequest, void *job, "") \
    ITERATOR3(void, wkeNetOnResponse, wkeWebView webView, wkeNetResponseCallback callback, void* param, "") \
    ITERATOR2(void, wkeNetGetMIMEType, void* job, wkeString mime, "") \
    \
    ITERATOR2(bool, wkeIsMainFrame, wkeWebView webView, wkeWebFrameHandle frameId, "") \
    ITERATOR2(bool, wkeIsWebRemoteFrame, wkeWebView webView, wkeWebFrameHandle frameId, "") \
    ITERATOR1(wkeWebFrameHandle, wkeWebFrameGetMainFrame, wkeWebView webView, "") \
    ITERATOR4(jsValue, wkeRunJsByFrame, wkeWebView webView, wkeWebFrameHandle frameId, const utf8* script, bool isInClosure, "") \
    \
    ITERATOR3(void, wkeWebFrameGetMainWorldScriptContext, wkeWebView webView, wkeWebFrameHandle webFrameId, v8ContextPtr contextOut, "") \
    \
    ITERATOR0(v8Isolate, wkeGetBlinkMainThreadIsolate, "") \
    \
    ITERATOR6(wkeWebView, wkeCreateWebWindow, wkeWindowType type, HWND parent, int x, int y, int width, int height, "") \
    ITERATOR1(void, wkeDestroyWebWindow, wkeWebView webWindow, "") \
    ITERATOR1(HWND, wkeGetWindowHandle, wkeWebView webWindow, "") \
    \
    ITERATOR2(void, wkeShowWindow, wkeWebView webWindow, bool show, "") \
    ITERATOR2(void, wkeEnableWindow, wkeWebView webWindow, bool enable, "") \
    \
    ITERATOR5(void, wkeMoveWindow, wkeWebView webWindow, int x, int y, int width, int height, "") \
    ITERATOR1(void, wkeMoveToCenter, wkeWebView webWindow, "") \
    ITERATOR3(void, wkeResizeWindow, wkeWebView webWindow, int width, int height, "") \
    \
    ITERATOR2(void, wkeSetWindowTitle, wkeWebView webWindow, const utf8* title, "") \
    ITERATOR2(void, wkeSetWindowTitleW, wkeWebView webWindow, const wchar_t* title, "") \
    \
    ITERATOR3(void, jsBindFunction, const char* name, jsNativeFunction fn, unsigned int argCount, "") \
    ITERATOR2(void, jsBindGetter, const char* name, jsNativeFunction fn, "") \
    ITERATOR2(void, jsBindSetter, const char* name, jsNativeFunction fn, "") \
    \
    ITERATOR4(void, wkeJsBindFunction, const char* name, wkeJsNativeFunction fn, void* param, unsigned int argCount, "") \
    ITERATOR3(void, wkeJsBindGetter, const char* name, wkeJsNativeFunction fn, void* param, "") \
    ITERATOR3(void, wkeJsBindSetter, const char* name, wkeJsNativeFunction fn, void* param, "") \
    \
    ITERATOR1(int, jsArgCount, jsExecState es, "") \
    ITERATOR2(jsType, jsArgType, jsExecState es, int argIdx, "") \
    ITERATOR2(jsValue, jsArg, jsExecState es, int argIdx, "") \
    \
    ITERATOR1(jsType, jsTypeOf, jsValue v, "") \
    ITERATOR1(bool, jsIsNumber, jsValue v, "") \
    ITERATOR1(bool, jsIsString, jsValue v, "") \
    ITERATOR1(bool, jsIsBoolean, jsValue v, "") \
    ITERATOR1(bool, jsIsObject, jsValue v, "") \
    ITERATOR1(bool, jsIsFunction, jsValue v, "") \
    ITERATOR1(bool, jsIsUndefined, jsValue v, "") \
    ITERATOR1(bool, jsIsNull, jsValue v, "") \
    ITERATOR1(bool, jsIsArray, jsValue v, "") \
    ITERATOR1(bool, jsIsTrue, jsValue v, "") \
    ITERATOR1(bool, jsIsFalse, jsValue v, "") \
    \
    ITERATOR2(int, jsToInt, jsExecState es, jsValue v, "") \
    ITERATOR2(float, jsToFloat, jsExecState es, jsValue v, "") \
    ITERATOR2(double, jsToDouble, jsExecState es, jsValue v, "") \
    ITERATOR2(bool, jsToBoolean, jsExecState es, jsValue v, "") \
    ITERATOR2(const utf8*, jsToTempString, jsExecState es, jsValue v, "") \
    ITERATOR2(const wchar_t*, jsToTempStringW, jsExecState es, jsValue v, "") \
    \
    ITERATOR1(jsValue, jsInt, int n, "") \
    ITERATOR1(jsValue, jsFloat, float f, "") \
    ITERATOR1(jsValue, jsDouble, double d, "") \
    ITERATOR1(jsValue, jsBoolean, bool b, "") \
    \
    ITERATOR0(jsValue, jsUndefined, "") \
    ITERATOR0(jsValue, jsNull, "") \
    ITERATOR0(jsValue, jsTrue, "") \
    ITERATOR0(jsValue, jsFalse, "") \
    \
    ITERATOR2(jsValue, jsString, jsExecState es, const utf8* str, "") \
    ITERATOR2(jsValue, jsStringW, jsExecState es, const wchar_t* str, "") \
    ITERATOR1(jsValue, jsEmptyObject, jsExecState es, "") \
    ITERATOR1(jsValue, jsEmptyArray, jsExecState es, "") \
    \
    ITERATOR2(jsValue, jsObject, jsExecState es, jsData* obj, "") \
    ITERATOR2(jsValue, jsFunction, jsExecState es, jsData* obj, "") \
    ITERATOR2(jsData*, jsGetData, jsExecState es, jsValue object, "") \
    \
    ITERATOR3(jsValue, jsGet, jsExecState es, jsValue object, const char* prop, "") \
    ITERATOR4(void, jsSet, jsExecState es, jsValue object, const char* prop, jsValue v, "") \
    \
    ITERATOR3(jsValue, jsGetAt, jsExecState es, jsValue object, int index, "") \
    ITERATOR4(void, jsSetAt, jsExecState es, jsValue object, int index, jsValue v, "") \
    \
    ITERATOR2(int, jsGetLength, jsExecState es, jsValue object, "") \
    ITERATOR3(void, jsSetLength, jsExecState es, jsValue object, int length, "") \
    \
    ITERATOR1(jsValue, jsGlobalObject, jsExecState es, "") \
    ITERATOR1(wkeWebView, jsGetWebView, jsExecState es, "") \
    \
    ITERATOR2(jsValue, jsEval, jsExecState es, const utf8* str, "") \
    ITERATOR2(jsValue, jsEvalW, jsExecState es, const wchar_t* str, "") \
    ITERATOR3(jsValue, jsEvalExW, jsExecState es, const wchar_t* str, bool isInClosure, "") \
    \
    ITERATOR5(jsValue, jsCall, jsExecState es, jsValue func, jsValue thisObject, jsValue* args, int argCount, "") \
    ITERATOR4(jsValue, jsCallGlobal, jsExecState es, jsValue func, jsValue* args, int argCount, "") \
    \
    ITERATOR2(jsValue, jsGetGlobal, jsExecState es, const char* prop, "") \
    ITERATOR3(void, jsSetGlobal, jsExecState es, const char* prop, jsValue v, "") \
    \
    ITERATOR0(void, jsGC, "")

#if ENABLE_WKE == 1

extern "C" __declspec(dllexport) void wkeInit();
extern "C" __declspec(dllexport) void wkeInitialize();
extern "C" __declspec(dllexport) void wkeInitializeEx(const wkeSettings* settings);

WKE_FOR_EACH_DEFINE_FUNCTION(WKE_DECLARE_ITERATOR0, WKE_DECLARE_ITERATOR1, WKE_DECLARE_ITERATOR2, \
	WKE_DECLARE_ITERATOR3, WKE_DECLARE_ITERATOR4, WKE_DECLARE_ITERATOR5, WKE_DECLARE_ITERATOR6, WKE_DECLARE_ITERATOR11)

#else

WKE_FOR_EACH_DEFINE_FUNCTION(WKE_DEFINE_ITERATOR0, WKE_DEFINE_ITERATOR1, WKE_DEFINE_ITERATOR2, \
	WKE_DEFINE_ITERATOR3, WKE_DEFINE_ITERATOR4, WKE_DEFINE_ITERATOR5, WKE_DEFINE_ITERATOR6, WKE_DEFINE_ITERATOR11)

	typedef void (WKE_CALL_TYPE *FN_wkeInitializeEx)(const wkeSettings* settings);

__declspec(selectany) const wchar_t* kWkeDllPath = L"node.dll";

inline void wkeSetWkeDllPath(const wchar_t* dllPath)
{
	kWkeDllPath = dllPath;
}

inline void wkeInitializeEx(const wkeSettings* settings)
{
	HMODULE hMod = ::LoadLibraryW(kWkeDllPath);

	FN_wkeInitializeEx wkeInitializeExFunc = (FN_wkeInitializeEx)::GetProcAddress(hMod, "wkeInitializeEx");
	wkeInitializeExFunc(settings);

	WKE_FOR_EACH_DEFINE_FUNCTION(WKE_GET_PTR_ITERATOR0, WKE_GET_PTR_ITERATOR1, WKE_GET_PTR_ITERATOR2, WKE_GET_PTR_ITERATOR3, \
		WKE_GET_PTR_ITERATOR4, WKE_GET_PTR_ITERATOR5, WKE_GET_PTR_ITERATOR6, WKE_GET_PTR_ITERATOR11);

	return;
}

inline void wkeInit()
{
	wkeInitializeEx(nullptr);
	return;
}

inline void wkeInitialize()
{
	wkeInitializeEx(nullptr);
	return;
}

#endif



#endif // WKE_DEFINE_H

