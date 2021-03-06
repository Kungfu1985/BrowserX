﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.Specialized;
using System.Runtime.Serialization;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Net;
using System.Net.Cache;
using System.Runtime.InteropServices;
using System.Security;
using System.Security.Permissions;
using System.Threading;
using System.Threading.Tasks;
using System.Diagnostics;
using MiniBlink.WebClientXEventArgs;

namespace MiniBlink.WebClientX
{
    [ComVisible(true)]
    public class WebClientX : Component
    {
        #region 其它类
        internal delegate void CompletionDelegate(byte[] responseBytes, Exception exception, object State);

        [EditorBrowsable(EditorBrowsableState.Never)]
        public delegate void WriteStreamClosedEventHandler(object sender, WriteStreamClosedEventArgs e);
        #endregion

        private class ProgressData
        {
            internal long BytesSent;

            internal long TotalBytesToSend = -1L;

            internal long BytesReceived;

            internal long TotalBytesToReceive = -1L;

            internal bool HasUploadPhase;

            internal void Reset()
            {
                BytesSent = 0L;
                TotalBytesToSend = -1L;
                BytesReceived = 0L;
                TotalBytesToReceive = -1L;
                HasUploadPhase = false;
            }
        }

        private class DownloadBitsState
        {
            internal WebClientX WebClient;

            internal Stream WriteStream;

            internal byte[] InnerBuffer;

            internal AsyncOperation AsyncOp;

            internal WebRequest Request;

            internal CompletionDelegate CompletionDelegate;

            internal Stream ReadStream;

            internal ScatterGatherBuffers SgBuffers;

            internal long ContentLength;

            internal long Length;

            internal int Offset;

            internal ProgressData Progress;

            internal bool Async => AsyncOp != null;

            internal DownloadBitsState(WebRequest request, Stream writeStream, CompletionDelegate completionDelegate, AsyncOperation asyncOp, ProgressData progress, WebClientX webClient)
            {
                WriteStream = writeStream;
                Request = request;
                AsyncOp = asyncOp;
                CompletionDelegate = completionDelegate;
                WebClient = webClient;
                Progress = progress;
            }

            internal int SetResponse(WebResponse response)
            {
                ContentLength = response.ContentLength;
                if (ContentLength == -1 || ContentLength > 65536)
                {
                    Length = 65536L;
                }
                else
                {
                    Length = ContentLength;
                }
                if (WriteStream == null)
                {
                    if (ContentLength > int.MaxValue)
                    {
                        throw new WebException("net_webstatus_MessageLengthLimitExceeded", WebExceptionStatus.MessageLengthLimitExceeded);
                    }
                    SgBuffers = new ScatterGatherBuffers(Length);
                }
                InnerBuffer = new byte[(int)Length];
                ReadStream = response.GetResponseStream();
                if (Async && response.ContentLength >= 0)
                {
                    Progress.TotalBytesToReceive = response.ContentLength;
                }
                if (Async)
                {
                    if (ReadStream == null || ReadStream == Stream.Null)
                    {
                        DownloadBitsReadCallbackState(this, null);
                    }
                    else
                    {
                        ReadStream.BeginRead(InnerBuffer, Offset, (int)Length - Offset, DownloadBitsReadCallback, this);
                    }
                    return -1;
                }
                if (ReadStream == null || ReadStream == Stream.Null)
                {
                    return 0;
                }
                return ReadStream.Read(InnerBuffer, Offset, (int)Length - Offset);
            }

            internal bool RetrieveBytes(ref int bytesRetrieved)
            {
                if (bytesRetrieved > 0)
                {
                    if (WriteStream != null)
                    {
                        WriteStream.Write(InnerBuffer, 0, bytesRetrieved);
                    }
                    else
                    {
                        SgBuffers.Write(InnerBuffer, 0, bytesRetrieved);
                    }
                    if (Async)
                    {
                        Progress.BytesReceived += bytesRetrieved;
                    }
                    if (Offset != ContentLength)
                    {
                        if (Async)
                        {
                            WebClient.PostProgressChanged(AsyncOp, Progress);
                            ReadStream.BeginRead(InnerBuffer, Offset, (int)Length - Offset, DownloadBitsReadCallback, this);
                        }
                        else
                        {
                            bytesRetrieved = ReadStream.Read(InnerBuffer, Offset, (int)Length - Offset);
                        }
                        return false;
                    }
                }
                if (Async)
                {
                    if (Progress.TotalBytesToReceive < 0)
                    {
                        Progress.TotalBytesToReceive = Progress.BytesReceived;
                    }
                    WebClient.PostProgressChanged(AsyncOp, Progress);
                }
                if (ReadStream != null)
                {
                    ReadStream.Close();
                }
                if (WriteStream != null)
                {
                    WriteStream.Close();
                }
                else if (WriteStream == null)
                {
                    byte[] array = new byte[SgBuffers.Length];
                    if (SgBuffers.Length > 0)
                    {
                        BufferOffsetSize[] buffers = SgBuffers.GetBuffers();
                        int num = 0;
                        foreach (BufferOffsetSize bufferOffsetSize in buffers)
                        {
                            Buffer.BlockCopy(bufferOffsetSize.Buffer, 0, array, num, bufferOffsetSize.Size);
                            num += bufferOffsetSize.Size;
                        }
                    }
                    InnerBuffer = array;
                }
                return true;
            }

            internal void Close()
            {
                if (WriteStream != null)
                {
                    WriteStream.Close();
                }
                if (ReadStream != null)
                {
                    ReadStream.Close();
                }
            }
        }

        private class UploadBitsState
        {
            private int m_ChunkSize;

            private int m_BufferWritePosition;

            internal WebClientX WebClient;

            internal Stream WriteStream;

            internal byte[] InnerBuffer;

            internal byte[] Header;

            internal byte[] Footer;

            internal AsyncOperation AsyncOp;

            internal WebRequest Request;

            internal CompletionDelegate UploadCompletionDelegate;

            internal CompletionDelegate DownloadCompletionDelegate;

            internal Stream ReadStream;

            internal long Length;

            internal int Offset;

            internal ProgressData Progress;

            internal bool FileUpload => ReadStream != null;

            internal bool Async => AsyncOp != null;

            internal UploadBitsState(WebRequest request, Stream readStream, byte[] buffer, int chunkSize, byte[] header, byte[] footer, CompletionDelegate uploadCompletionDelegate, CompletionDelegate downloadCompletionDelegate, AsyncOperation asyncOp, ProgressData progress, WebClientX webClient)
            {
                InnerBuffer = buffer;
                m_ChunkSize = chunkSize;
                m_BufferWritePosition = 0;
                Header = header;
                Footer = footer;
                ReadStream = readStream;
                Request = request;
                AsyncOp = asyncOp;
                UploadCompletionDelegate = uploadCompletionDelegate;
                DownloadCompletionDelegate = downloadCompletionDelegate;
                if (AsyncOp != null)
                {
                    Progress = progress;
                    Progress.HasUploadPhase = true;
                    Progress.TotalBytesToSend = ((request.ContentLength < 0) ? (-1) : request.ContentLength);
                }
                WebClient = webClient;
            }

            internal void SetRequestStream(Stream writeStream)
            {
                WriteStream = writeStream;
                byte[] array = null;
                if (Header != null)
                {
                    array = Header;
                    Header = null;
                }
                else
                {
                    array = new byte[0];
                }
                if (Async)
                {
                    Progress.BytesSent += array.Length;
                    WriteStream.BeginWrite(array, 0, array.Length, UploadBitsWriteCallback, this);
                }
                else
                {
                    WriteStream.Write(array, 0, array.Length);
                }
            }

            internal bool WriteBytes()
            {
                byte[] array = null;
                int num = 0;
                int num2 = 0;
                if (Async)
                {
                    WebClient.PostProgressChanged(AsyncOp, Progress);
                }
                if (FileUpload)
                {
                    int num3 = 0;
                    if (InnerBuffer != null)
                    {
                        num3 = ReadStream.Read(InnerBuffer, 0, InnerBuffer.Length);
                        if (num3 <= 0)
                        {
                            ReadStream.Close();
                            InnerBuffer = null;
                        }
                    }
                    if (InnerBuffer != null)
                    {
                        num = num3;
                        array = InnerBuffer;
                    }
                    else
                    {
                        if (Footer == null)
                        {
                            return true;
                        }
                        num = Footer.Length;
                        array = Footer;
                        Footer = null;
                    }
                }
                else
                {
                    if (InnerBuffer == null)
                    {
                        return true;
                    }
                    array = InnerBuffer;
                    if (m_ChunkSize != 0)
                    {
                        num2 = m_BufferWritePosition;
                        m_BufferWritePosition += m_ChunkSize;
                        num = m_ChunkSize;
                        if (m_BufferWritePosition >= InnerBuffer.Length)
                        {
                            num = InnerBuffer.Length - num2;
                            InnerBuffer = null;
                        }
                    }
                    else
                    {
                        num = InnerBuffer.Length;
                        InnerBuffer = null;
                    }
                }
                if (Async)
                {
                    Progress.BytesSent += num;
                    WriteStream.BeginWrite(array, num2, num, UploadBitsWriteCallback, this);
                }
                else
                {
                    WriteStream.Write(array, 0, num);
                }
                return false;
            }

            internal void Close()
            {
                if (WriteStream != null)
                {
                    WriteStream.Close();
                }
                if (ReadStream != null)
                {
                    ReadStream.Close();
                }
            }
        }

        private class WebClientWriteStream : Stream
        {
            private WebRequest m_request;

            private Stream m_stream;

            private WebClientX m_WebClient;

            public override bool CanRead => m_stream.CanRead;

            public override bool CanSeek => m_stream.CanSeek;

            public override bool CanWrite => m_stream.CanWrite;

            public override bool CanTimeout => m_stream.CanTimeout;

            public override int ReadTimeout
            {
                get
                {
                    return m_stream.ReadTimeout;
                }
                set
                {
                    m_stream.ReadTimeout = value;
                }
            }

            public override int WriteTimeout
            {
                get
                {
                    return m_stream.WriteTimeout;
                }
                set
                {
                    m_stream.WriteTimeout = value;
                }
            }

            public override long Length => m_stream.Length;

            public override long Position
            {
                get
                {
                    return m_stream.Position;
                }
                set
                {
                    m_stream.Position = value;
                }
            }

            public WebClientWriteStream(Stream stream, WebRequest request, WebClientX webClient)
            {
                m_request = request;
                m_stream = stream;
                m_WebClient = webClient;
            }

            [HostProtection(SecurityAction.LinkDemand, ExternalThreading = true)]
            public override IAsyncResult BeginRead(byte[] buffer, int offset, int size, AsyncCallback callback, object state)
            {
                return m_stream.BeginRead(buffer, offset, size, callback, state);
            }

            [HostProtection(SecurityAction.LinkDemand, ExternalThreading = true)]
            public override IAsyncResult BeginWrite(byte[] buffer, int offset, int size, AsyncCallback callback, object state)
            {
                return m_stream.BeginWrite(buffer, offset, size, callback, state);
            }

            protected override void Dispose(bool disposing)
            {
                try
                {
                    if (disposing)
                    {
                        m_stream.Close();
                        m_WebClient.GetWebResponse(m_request).Close();
                    }
                }
                finally
                {
                    base.Dispose(disposing);
                }
            }

            public override int EndRead(IAsyncResult result)
            {
                return m_stream.EndRead(result);
            }

            public override void EndWrite(IAsyncResult result)
            {
                m_stream.EndWrite(result);
            }

            public override void Flush()
            {
                m_stream.Flush();
            }

            public override int Read(byte[] buffer, int offset, int count)
            {
                return m_stream.Read(buffer, offset, count);
            }

            public override long Seek(long offset, SeekOrigin origin)
            {
                return m_stream.Seek(offset, origin);
            }

            public override void SetLength(long value)
            {
                m_stream.SetLength(value);
            }

            public override void Write(byte[] buffer, int offset, int count)
            {
                m_stream.Write(buffer, offset, count);
            }
        }

        private const int DefaultCopyBufferLength = 8192;

        private const int DefaultDownloadBufferLength = 65536;

        private const string DefaultUploadFileContentType = "application/octet-stream";

        private const string UploadFileContentType = "multipart/form-data";

        private const string UploadValuesContentType = "application/x-www-form-urlencoded";

        private Uri m_baseAddress;

        private ICredentials m_credentials;

        private WebHeaderCollection m_headers;

        private NameValueCollection m_requestParameters;

        private WebResponse m_WebResponse;

        private WebRequest m_WebRequest;

        private Encoding m_Encoding = Encoding.Default;

        private string m_Method;

        private long m_ContentLength = -1L;

        private bool m_InitWebClientAsync;

        private bool m_Cancelled;

        private ProgressData m_Progress;

        private IWebProxy m_Proxy;

        private bool m_ProxySet;

        private RequestCachePolicy m_CachePolicy;

        private int m_CallNesting;

        private AsyncOperation m_AsyncOp;

        private SendOrPostCallback openReadOperationCompleted;

        private SendOrPostCallback openWriteOperationCompleted;

        private SendOrPostCallback downloadStringOperationCompleted;

        private SendOrPostCallback downloadDataOperationCompleted;

        private SendOrPostCallback downloadFileOperationCompleted;

        private SendOrPostCallback uploadStringOperationCompleted;

        private SendOrPostCallback uploadDataOperationCompleted;

        private SendOrPostCallback uploadFileOperationCompleted;

        private SendOrPostCallback uploadValuesOperationCompleted;

        private SendOrPostCallback reportDownloadProgressChanged;

        private SendOrPostCallback reportUploadProgressChanged;

        [Obsolete("This API supports the .NET Framework infrastructure and is not intended to be used directly from your code.", true)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public bool AllowReadStreamBuffering
        {
            get;
            set;
        }

        [Obsolete("This API supports the .NET Framework infrastructure and is not intended to be used directly from your code.", true)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public bool AllowWriteStreamBuffering
        {
            get;
            set;
        }

        public Encoding Encoding
        {
            get
            {
                return m_Encoding;
            }
            set
            {
                if (value == null)
                {
                    throw new ArgumentNullException("Encoding");
                }
                m_Encoding = value;
            }
        }

        public string BaseAddress
        {
            get
            {
                if (!(m_baseAddress == null))
                {
                    return m_baseAddress.ToString();
                }
                return string.Empty;
            }
            set
            {
                if (value == null || value.Length == 0)
                {
                    m_baseAddress = null;
                }
                else
                {
                    try
                    {
                        m_baseAddress = new Uri(value);
                    }
                    catch (UriFormatException innerException)
                    {
                        throw new ArgumentException("net_webclient_invalid_baseaddress", "value", innerException);
                    }
                }
            }
        }

        public ICredentials Credentials
        {
            get
            {
                return m_credentials;
            }
            set
            {
                m_credentials = value;
            }
        }

        public bool UseDefaultCredentials
        {
            get
            {
                if (!(m_credentials is SystemNetworkCredential))
                {
                    return false;
                }
                return true;
            }
            set
            {
                m_credentials = (value ? CredentialCache.DefaultCredentials : null);
            }
        }

        public WebHeaderCollection Headers
        {
            get
            {
                if (m_headers == null)
                {
                    //do something
                    m_headers = new WebHeaderCollection();
                }
                return m_headers;
            }
            set
            {
                m_headers = value;
            }
        }

        public NameValueCollection QueryString
        {
            get
            {
                if (m_requestParameters == null)
                {
                    m_requestParameters = new NameValueCollection();
                }
                return m_requestParameters;
            }
            set
            {
                m_requestParameters = value;
            }
        }

        public WebHeaderCollection ResponseHeaders
        {
            get
            {
                if (m_WebResponse != null)
                {
                    return m_WebResponse.Headers;
                }
                return null;
            }
        }

        public IWebProxy Proxy
        {
            get
            {
                //do something
                //ExceptionHelper.WebPermissionUnrestricted.Demand();
                if (!m_ProxySet)
                {
                    return WebRequest.DefaultWebProxy;
                }
                return m_Proxy;
            }
            set
            {
                //ExceptionHelper.WebPermissionUnrestricted.Demand();
                m_Proxy = value;
                m_ProxySet = true;
            }
        }

        public RequestCachePolicy CachePolicy
        {
            get
            {
                return m_CachePolicy;
            }
            set
            {
                m_CachePolicy = value;
            }
        }

        public bool IsBusy => m_AsyncOp != null;

        [Obsolete("This API supports the .NET Framework infrastructure and is not intended to be used directly from your code.", true)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public event WriteStreamClosedEventHandler WriteStreamClosed
        {
            add
            {
            }
            remove
            {
            }
        }

        public event WebClientEventHandler.OpenReadCompletedEventHandler OpenReadCompleted;

        public event OpenWriteCompletedEventHandler OpenWriteCompleted;

        public event DownloadStringCompletedEventHandler DownloadStringCompleted;

        public event DownloadDataCompletedEventHandler DownloadDataCompleted;

        public event AsyncCompletedEventHandler DownloadFileCompleted;

        public event UploadStringCompletedEventHandler UploadStringCompleted;

        public event UploadDataCompletedEventHandler UploadDataCompleted;

        public event UploadFileCompletedEventHandler UploadFileCompleted;

        public event UploadValuesCompletedEventHandler UploadValuesCompleted;

        public event DownloadProgressChangedEventHandler DownloadProgressChanged;

        public event UploadProgressChangedEventHandler UploadProgressChanged;

        public WebClientX()
        {
            if (GetType() == typeof(WebClientX))
            {
                GC.SuppressFinalize(this);
            }
        }

        private void InitWebClientAsync()
        {
            if (!m_InitWebClientAsync)
            {
                openReadOperationCompleted = OpenReadOperationCompleted;
                openWriteOperationCompleted = OpenWriteOperationCompleted;
                downloadStringOperationCompleted = DownloadStringOperationCompleted;
                downloadDataOperationCompleted = DownloadDataOperationCompleted;
                downloadFileOperationCompleted = DownloadFileOperationCompleted;
                uploadStringOperationCompleted = UploadStringOperationCompleted;
                uploadDataOperationCompleted = UploadDataOperationCompleted;
                uploadFileOperationCompleted = UploadFileOperationCompleted;
                uploadValuesOperationCompleted = UploadValuesOperationCompleted;
                reportDownloadProgressChanged = ReportDownloadProgressChanged;
                reportUploadProgressChanged = ReportUploadProgressChanged;
                m_Progress = new ProgressData();
                m_InitWebClientAsync = true;
            }
        }

        private void ClearWebClientState()
        {
            if (AnotherCallInProgress(Interlocked.Increment(ref m_CallNesting)))
            {
                CompleteWebClientState();
                throw new NotSupportedException(("net_webclient_no_concurrent_io_allowed"));
            }
            m_ContentLength = -1L;
            m_WebResponse = null;
            m_WebRequest = null;
            m_Method = null;
            m_Cancelled = false;
            if (m_Progress != null)
            {
                m_Progress.Reset();
            }
        }

        private void CompleteWebClientState()
        {
            Interlocked.Decrement(ref m_CallNesting);
        }

        [Obsolete("This API supports the .NET Framework infrastructure and is not intended to be used directly from your code.", true)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        protected virtual void OnWriteStreamClosed(WriteStreamClosedEventArgs e)
        {
        }

        protected virtual WebRequest GetWebRequest(Uri address)
        {
            WebRequest webRequest = WebRequest.Create(address);
            CopyHeadersTo(webRequest);
            if (Credentials != null)
            {
                webRequest.Credentials = Credentials;
            }
            if (m_Method != null)
            {
                webRequest.Method = m_Method;
            }
            if (m_ContentLength != -1)
            {
                webRequest.ContentLength = m_ContentLength;
            }
            if (m_ProxySet)
            {
                webRequest.Proxy = m_Proxy;
            }
            if (m_CachePolicy != null)
            {
                webRequest.CachePolicy = m_CachePolicy;
            }
            return webRequest;
        }

        protected virtual WebResponse GetWebResponse(WebRequest request)
        {
            return m_WebResponse = request.GetResponse();
        }

        protected virtual WebResponse GetWebResponse(WebRequest request, IAsyncResult result)
        {
            return m_WebResponse = request.EndGetResponse(result);
        }

        public byte[] DownloadData(string address)
        {
            if (address == null)
            {
                throw new ArgumentNullException("address");
            }
            return DownloadData(GetUri(address));
        }

        public byte[] DownloadData(Uri address)
        {
            //if (Logging.On)
            //{
            //    Logging.Enter(Logging.Web, this, "DownloadData", address);
            //}
            if (address == null)
            {
                throw new ArgumentNullException("address");
            }
            ClearWebClientState();
            byte[] array = null;
            try
            {
                array = DownloadDataInternal(address, out WebRequest _);
                //if (Logging.On)
                //{
                //    Logging.Exit(Logging.Web, this, "DownloadData", array);
                //}
                return array;
            }
            finally
            {
                CompleteWebClientState();
            }
        }

        private byte[] DownloadDataInternal(Uri address, out WebRequest request)
        {
            //if (Logging.On)
            //{
            //    Logging.Enter(Logging.Web, this, "DownloadData", address);
            //}
            request = null;
            try
            {
                request = (m_WebRequest = GetWebRequest(GetUri(address)));
                return DownloadBits(request, null, null, null);
            }
            catch (Exception ex)
            {
                if (ex is ThreadAbortException || ex is StackOverflowException || ex is OutOfMemoryException)
                {
                    throw;
                }
                if (!(ex is WebException) && !(ex is SecurityException))
                {
                    ex = new WebException(("net_webclient"), ex);
                }
                AbortRequest(request);
                throw ex;
            }
        }

        public void DownloadFile(string address, string fileName)
        {
            if (address == null)
            {
                throw new ArgumentNullException("address");
            }
            DownloadFile(GetUri(address), fileName);
        }

        public void DownloadFile(Uri address, string fileName)
        {
            //if (Logging.On)
            //{
            //    Logging.Enter(Logging.Web, this, "DownloadFile", string.Concat(address, ", ", fileName));
            //}
            if (address == null)
            {
                throw new ArgumentNullException("address");
            }
            if (fileName == null)
            {
                throw new ArgumentNullException("fileName");
            }
            WebRequest request = null;
            FileStream fileStream = null;
            bool flag = false;
            ClearWebClientState();
            try
            {
                fileStream = new FileStream(fileName, FileMode.Create, FileAccess.Write);
                request = (m_WebRequest = GetWebRequest(GetUri(address)));
                DownloadBits(request, fileStream, null, null);
                flag = true;
            }
            catch (Exception ex)
            {
                if (ex is ThreadAbortException || ex is StackOverflowException || ex is OutOfMemoryException)
                {
                    throw;
                }
                if (!(ex is WebException) && !(ex is SecurityException))
                {
                    ex = new WebException(("net_webclient"), ex);
                }
                AbortRequest(request);
                throw ex;
            }
            finally
            {
                if (fileStream != null)
                {
                    fileStream.Close();
                    if (!flag)
                    {
                        File.Delete(fileName);
                    }
                    fileStream = null;
                }
                CompleteWebClientState();
            }
            //if (Logging.On)
            //{
            //    Logging.Exit(Logging.Web, this, "DownloadFile", "");
            //}
        }

        public Stream OpenRead(string address)
        {
            if (address == null)
            {
                throw new ArgumentNullException("address");
            }
            return OpenRead(GetUri(address));
        }

        public Stream OpenRead(Uri address)
        {
            //if (Logging.On)
            //{
            //    Logging.Enter(Logging.Web, this, "OpenRead", address);
            //}
            if (address == null)
            {
                throw new ArgumentNullException("address");
            }
            WebRequest request = null;
            ClearWebClientState();
            try
            {
                request = (m_WebRequest = GetWebRequest(GetUri(address)));
                Stream responseStream = (m_WebResponse = GetWebResponse(request)).GetResponseStream();
                //if (Logging.On)
                //{
                //    Logging.Exit(Logging.Web, this, "OpenRead", responseStream);
                //}
                return responseStream;
            }
            catch (Exception ex)
            {
                if (ex is ThreadAbortException || ex is StackOverflowException || ex is OutOfMemoryException)
                {
                    throw;
                }
                if (!(ex is WebException) && !(ex is SecurityException))
                {
                    ex = new WebException(("net_webclient"), ex);
                }
                AbortRequest(request);
                throw ex;
            }
            finally
            {
                CompleteWebClientState();
            }
        }

        public Stream OpenWrite(string address)
        {
            if (address == null)
            {
                throw new ArgumentNullException("address");
            }
            return OpenWrite(GetUri(address), null);
        }

        public Stream OpenWrite(Uri address)
        {
            return OpenWrite(address, null);
        }

        public Stream OpenWrite(string address, string method)
        {
            if (address == null)
            {
                throw new ArgumentNullException("address");
            }
            return OpenWrite(GetUri(address), method);
        }

        public Stream OpenWrite(Uri address, string method)
        {
            //if (Logging.On)
            //{
            //    Logging.Enter(Logging.Web, this, "OpenWrite", string.Concat(address, ", ", method));
            //}
            if (address == null)
            {
                throw new ArgumentNullException("address");
            }
            if (method == null)
            {
                method = MapToDefaultMethod(address);
            }
            WebRequest webRequest = null;
            ClearWebClientState();
            try
            {
                m_Method = method;
                webRequest = (m_WebRequest = GetWebRequest(GetUri(address)));
                WebClientWriteStream webClientWriteStream = new WebClientWriteStream(webRequest.GetRequestStream(), webRequest, this);
                //if (Logging.On)
                //{
                //    Logging.Exit(Logging.Web, this, "OpenWrite", webClientWriteStream);
                //}
                return webClientWriteStream;
            }
            catch (Exception ex)
            {
                if (ex is ThreadAbortException || ex is StackOverflowException || ex is OutOfMemoryException)
                {
                    throw;
                }
                if (!(ex is WebException) && !(ex is SecurityException))
                {
                    ex = new WebException(("net_webclient"), ex);
                }
                AbortRequest(webRequest);
                throw ex;
            }
            finally
            {
                CompleteWebClientState();
            }
        }

        public byte[] UploadData(string address, byte[] data)
        {
            if (address == null)
            {
                throw new ArgumentNullException("address");
            }
            return UploadData(GetUri(address), null, data);
        }

        public byte[] UploadData(Uri address, byte[] data)
        {
            return UploadData(address, null, data);
        }

        public byte[] UploadData(string address, string method, byte[] data)
        {
            if (address == null)
            {
                throw new ArgumentNullException("address");
            }
            return UploadData(GetUri(address), method, data);
        }

        public byte[] UploadData(Uri address, string method, byte[] data)
        {
            //if (Logging.On)
            //{
            //    Logging.Enter(Logging.Web, this, "UploadData", string.Concat(address, ", ", method));
            //}
            if (address == null)
            {
                throw new ArgumentNullException("address");
            }
            if (data == null)
            {
                throw new ArgumentNullException("data");
            }
            if (method == null)
            {
                method = MapToDefaultMethod(address);
            }
            ClearWebClientState();
            try
            {
                WebRequest request;
                byte[] array = UploadDataInternal(address, method, data, out request);
                //if (Logging.On)
                //{
                //    Logging.Exit(Logging.Web, this, "UploadData", array);
                //}
                return array;
            }
            finally
            {
                CompleteWebClientState();
            }
        }

        private byte[] UploadDataInternal(Uri address, string method, byte[] data, out WebRequest request)
        {
            request = null;
            try
            {
                m_Method = method;
                m_ContentLength = data.Length;
                request = (m_WebRequest = GetWebRequest(GetUri(address)));
                UploadBits(request, null, data, 0, null, null, null, null, null);
                return DownloadBits(request, null, null, null);
            }
            catch (Exception ex)
            {
                if (ex is ThreadAbortException || ex is StackOverflowException || ex is OutOfMemoryException)
                {
                    throw;
                }
                if (!(ex is WebException) && !(ex is SecurityException))
                {
                    ex = new WebException(("net_webclient"), ex);
                }
                AbortRequest(request);
                throw ex;
            }
        }

        private void OpenFileInternal(bool needsHeaderAndBoundary, string fileName, ref FileStream fs, ref byte[] buffer, ref byte[] formHeaderBytes, ref byte[] boundaryBytes)
        {
            fileName = Path.GetFullPath(fileName);
            if (m_headers == null)
            {
                //do something
                m_headers = new WebHeaderCollection();
                //m_headers = new WebHeaderCollection(WebHeaderCollectionType.WebRequest);
            }
            string text = m_headers["Content-Type"];
            if (text != null)
            {
                if (text.ToLower(CultureInfo.InvariantCulture).StartsWith("multipart/"))
                {
                    throw new WebException(("net_webclient_Multipart"));
                }
            }
            else
            {
                text = "application/octet-stream";
            }
            fs = new FileStream(fileName, FileMode.Open, FileAccess.Read);
            int num = 8192;
            m_ContentLength = -1L;
            if (m_Method.ToUpper(CultureInfo.InvariantCulture) == "POST")
            {
                if (needsHeaderAndBoundary)
                {
                    string text2 = "---------------------" + DateTime.Now.Ticks.ToString("x", NumberFormatInfo.InvariantInfo);
                    m_headers["Content-Type"] = "multipart/form-data; boundary=" + text2;
                    string s = "--" + text2 + "\r\nContent-Disposition: form-data; name=\"file\"; filename=\"" + Path.GetFileName(fileName) + "\"\r\nContent-Type: " + text + "\r\n\r\n";
                    formHeaderBytes = Encoding.UTF8.GetBytes(s);
                    boundaryBytes = Encoding.ASCII.GetBytes("\r\n--" + text2 + "--\r\n");
                }
                else
                {
                    formHeaderBytes = new byte[0];
                    boundaryBytes = new byte[0];
                }
                if (fs.CanSeek)
                {
                    m_ContentLength = fs.Length + formHeaderBytes.Length + boundaryBytes.Length;
                    num = (int)Math.Min(8192L, fs.Length);
                }
            }
            else
            {
                m_headers["Content-Type"] = text;
                formHeaderBytes = null;
                boundaryBytes = null;
                if (fs.CanSeek)
                {
                    m_ContentLength = fs.Length;
                    num = (int)Math.Min(8192L, fs.Length);
                }
            }
            buffer = new byte[num];
        }

        public byte[] UploadFile(string address, string fileName)
        {
            if (address == null)
            {
                throw new ArgumentNullException("address");
            }
            return UploadFile(GetUri(address), fileName);
        }

        public byte[] UploadFile(Uri address, string fileName)
        {
            return UploadFile(address, null, fileName);
        }

        public byte[] UploadFile(string address, string method, string fileName)
        {
            return UploadFile(GetUri(address), method, fileName);
        }

        public byte[] UploadFile(Uri address, string method, string fileName)
        {
            if (address == null)
            {
                throw new ArgumentNullException("address");
            }
            if (fileName == null)
            {
                throw new ArgumentNullException("fileName");
            }
            if (method == null)
            {
                method = MapToDefaultMethod(address);
            }
            FileStream fs = null;
            WebRequest request = null;
            ClearWebClientState();
            try
            {
                m_Method = method;
                byte[] formHeaderBytes = null;
                byte[] boundaryBytes = null;
                byte[] buffer = null;
                Uri uri = GetUri(address);
                bool needsHeaderAndBoundary = uri.Scheme != Uri.UriSchemeFile;
                OpenFileInternal(needsHeaderAndBoundary, fileName, ref fs, ref buffer, ref formHeaderBytes, ref boundaryBytes);
                request = (m_WebRequest = GetWebRequest(uri));
                UploadBits(request, fs, buffer, 0, formHeaderBytes, boundaryBytes, null, null, null);
                byte[] array = DownloadBits(request, null, null, null);
                return array;
            }
            catch (Exception ex)
            {
                if (fs != null)
                {
                    fs.Close();
                    fs = null;
                }
                if (ex is ThreadAbortException || ex is StackOverflowException || ex is OutOfMemoryException)
                {
                    throw;
                }
                if (!(ex is WebException) && !(ex is SecurityException))
                {
                    ex = new WebException(("net_webclient"), ex);
                }
                AbortRequest(request);
                throw ex;
            }
            finally
            {
                CompleteWebClientState();
            }
        }

        private byte[] UploadValuesInternal(NameValueCollection data)
        {
            if (m_headers == null)
            {
                //do something
                m_headers = new WebHeaderCollection();
                //m_headers = new WebHeaderCollection(WebHeaderCollectionType.WebRequest);
            }
            string text = m_headers["Content-Type"];
            if (text != null && string.Compare(text, "application/x-www-form-urlencoded", StringComparison.OrdinalIgnoreCase) != 0)
            {
                throw new WebException(("net_webclient_ContentType"));
            }
            m_headers["Content-Type"] = "application/x-www-form-urlencoded";
            string value = string.Empty;
            StringBuilder stringBuilder = new StringBuilder();
            string[] allKeys = data.AllKeys;
            foreach (string text2 in allKeys)
            {
                stringBuilder.Append(value);
                stringBuilder.Append(UrlEncode(text2));
                stringBuilder.Append("=");
                stringBuilder.Append(UrlEncode(data[text2]));
                value = "&";
            }
            byte[] bytes = Encoding.ASCII.GetBytes(stringBuilder.ToString());
            m_ContentLength = bytes.Length;
            return bytes;
        }

        public byte[] UploadValues(string address, NameValueCollection data)
        {
            if (address == null)
            {
                throw new ArgumentNullException("address");
            }
            return UploadValues(GetUri(address), null, data);
        }

        public byte[] UploadValues(Uri address, NameValueCollection data)
        {
            return UploadValues(address, null, data);
        }

        public byte[] UploadValues(string address, string method, NameValueCollection data)
        {
            if (address == null)
            {
                throw new ArgumentNullException("address");
            }
            return UploadValues(GetUri(address), method, data);
        }

        public byte[] UploadValues(Uri address, string method, NameValueCollection data)
        {
            if (address == null)
            {
                throw new ArgumentNullException("address");
            }
            if (data == null)
            {
                throw new ArgumentNullException("data");
            }
            if (method == null)
            {
                method = MapToDefaultMethod(address);
            }
            WebRequest request = null;
            ClearWebClientState();
            try
            {
                byte[] buffer = UploadValuesInternal(data);
                m_Method = method;
                request = (m_WebRequest = GetWebRequest(GetUri(address)));
                UploadBits(request, null, buffer, 0, null, null, null, null, null);
                byte[] result = DownloadBits(request, null, null, null);
                return result;
            }
            catch (Exception ex)
            {
                if (ex is ThreadAbortException || ex is StackOverflowException || ex is OutOfMemoryException)
                {
                    throw;
                }
                if (!(ex is WebException) && !(ex is SecurityException))
                {
                    ex = new WebException(("net_webclient"), ex);
                }
                AbortRequest(request);
                throw ex;
            }
            finally
            {
                CompleteWebClientState();
            }
        }

        public string UploadString(string address, string data)
        {
            if (address == null)
            {
                throw new ArgumentNullException("address");
            }
            return UploadString(GetUri(address), null, data);
        }

        public string UploadString(Uri address, string data)
        {
            return UploadString(address, null, data);
        }

        public string UploadString(string address, string method, string data)
        {
            if (address == null)
            {
                throw new ArgumentNullException("address");
            }
            return UploadString(GetUri(address), method, data);
        }

        public string UploadString(Uri address, string method, string data)
        {
            if (address == null)
            {
                throw new ArgumentNullException("address");
            }
            if (data == null)
            {
                throw new ArgumentNullException("data");
            }
            if (method == null)
            {
                method = MapToDefaultMethod(address);
            }
            ClearWebClientState();
            try
            {
                byte[] bytes = Encoding.GetBytes(data);
                WebRequest request;
                byte[] data2 = UploadDataInternal(address, method, bytes, out request);
                string stringUsingEncoding = GetStringUsingEncoding(request, data2);
                return stringUsingEncoding;
            }
            finally
            {
                CompleteWebClientState();
            }
        }

        public string DownloadString(string address)
        {
            if (address == null)
            {
                throw new ArgumentNullException("address");
            }
            return DownloadString(GetUri(address));
        }

        public string DownloadString(Uri address)
        {
            if (address == null)
            {
                throw new ArgumentNullException("address");
            }
            ClearWebClientState();
            try
            {
                WebRequest request;
                byte[] data = DownloadDataInternal(address, out request);
                string stringUsingEncoding = GetStringUsingEncoding(request, data);
                return stringUsingEncoding;
            }
            finally
            {
                CompleteWebClientState();
            }
        }

        private static void AbortRequest(WebRequest request)
        {
            try
            {
                request?.Abort();
            }
            catch (Exception ex)
            {
                if (ex is OutOfMemoryException || ex is StackOverflowException || ex is ThreadAbortException)
                {
                    throw;
                }
            }
        }

        private void CopyHeadersTo(WebRequest request)
        {
            if (m_headers != null && request is HttpWebRequest)
            {
                string text = m_headers["Accept"];
                string text2 = m_headers["Connection"];
                string text3 = m_headers["Content-Type"];
                string text4 = m_headers["Expect"];
                string text5 = m_headers["Referer"];
                string text6 = m_headers["User-Agent"];
                string text7 = m_headers["Host"];
                m_headers.Remove("Accept");
                m_headers.Remove("Connection");
                m_headers.Remove("Content-Type");
                m_headers.Remove("Expect");
                m_headers.Remove("Referer");
                m_headers.Remove("User-Agent");
                m_headers.Remove("Host");
                request.Headers = m_headers;
                if (text != null && text.Length > 0)
                {
                    ((HttpWebRequest)request).Accept = text;
                }
                if (text2 != null && text2.Length > 0)
                {
                    ((HttpWebRequest)request).Connection = text2;
                }
                if (text3 != null && text3.Length > 0)
                {
                    ((HttpWebRequest)request).ContentType = text3;
                }
                if (text4 != null && text4.Length > 0)
                {
                    ((HttpWebRequest)request).Expect = text4;
                }
                if (text5 != null && text5.Length > 0)
                {
                    ((HttpWebRequest)request).Referer = text5;
                }
                if (text6 != null && text6.Length > 0)
                {
                    ((HttpWebRequest)request).UserAgent = text6;
                }
                if (!string.IsNullOrEmpty(text7))
                {
                    ((HttpWebRequest)request).Host = text7;
                }
            }
        }

        private Uri GetUri(string path)
        {
            Uri result;
            if (m_baseAddress != null)
            {
                if (!Uri.TryCreate(m_baseAddress, path, out result))
                {
                    return new Uri(Path.GetFullPath(path));
                }
            }
            else if (!Uri.TryCreate(path, UriKind.Absolute, out result))
            {
                return new Uri(Path.GetFullPath(path));
            }
            return GetUri(result);
        }

        private Uri GetUri(Uri address)
        {
            if (address == null)
            {
                throw new ArgumentNullException("address");
            }
            Uri result = address;
            if (!address.IsAbsoluteUri && m_baseAddress != null && !Uri.TryCreate(m_baseAddress, address, out result))
            {
                return address;
            }
            if ((result.Query == null || result.Query == string.Empty) && m_requestParameters != null)
            {
                StringBuilder stringBuilder = new StringBuilder();
                string str = string.Empty;
                for (int i = 0; i < m_requestParameters.Count; i++)
                {
                    stringBuilder.Append(str + m_requestParameters.AllKeys[i] + "=" + m_requestParameters[i]);
                    str = "&";
                }
                UriBuilder uriBuilder = new UriBuilder(result);
                uriBuilder.Query = stringBuilder.ToString();
                result = uriBuilder.Uri;
            }
            return result;
        }

        private static void DownloadBitsResponseCallback(IAsyncResult result)
        {
            DownloadBitsState downloadBitsState = (DownloadBitsState)result.AsyncState;
            WebRequest request = downloadBitsState.Request;
            Exception ex = null;
            try
            {
                WebResponse webResponse = downloadBitsState.WebClient.GetWebResponse(request, result);
                downloadBitsState.WebClient.m_WebResponse = webResponse;
                downloadBitsState.SetResponse(webResponse);
            }
            catch (Exception ex2)
            {
                if (ex2 is ThreadAbortException || ex2 is StackOverflowException || ex2 is OutOfMemoryException)
                {
                    throw;
                }
                ex = ex2;
                if (!(ex2 is WebException) && !(ex2 is SecurityException))
                {
                    ex = new WebException(("net_webclient"), ex2);
                }
                AbortRequest(request);
                if (downloadBitsState != null && downloadBitsState.WriteStream != null)
                {
                    downloadBitsState.WriteStream.Close();
                }
            }
            finally
            {
                if (ex != null)
                {
                    downloadBitsState.CompletionDelegate(null, ex, downloadBitsState.AsyncOp);
                }
            }
        }

        private static void DownloadBitsReadCallback(IAsyncResult result)
        {
            DownloadBitsState state = (DownloadBitsState)result.AsyncState;
            DownloadBitsReadCallbackState(state, result);
        }

        private static void DownloadBitsReadCallbackState(DownloadBitsState state, IAsyncResult result)
        {
            Stream readStream = state.ReadStream;
            Exception ex = null;
            bool flag = false;
            try
            {
                int bytesRetrieved = 0;
                if (readStream != null && readStream != Stream.Null)
                {
                    bytesRetrieved = readStream.EndRead(result);
                }
                flag = state.RetrieveBytes(ref bytesRetrieved);
            }
            catch (Exception ex2)
            {
                flag = true;
                if (ex2 is ThreadAbortException || ex2 is StackOverflowException || ex2 is OutOfMemoryException)
                {
                    throw;
                }
                ex = ex2;
                state.InnerBuffer = null;
                if (!(ex2 is WebException) && !(ex2 is SecurityException))
                {
                    ex = new WebException(("net_webclient"), ex2);
                }
                AbortRequest(state.Request);
                if (state != null && state.WriteStream != null)
                {
                    state.WriteStream.Close();
                }
            }
            finally
            {
                if (flag)
                {
                    if (ex == null)
                    {
                        state.Close();
                    }
                    state.CompletionDelegate(state.InnerBuffer, ex, state.AsyncOp);
                }
            }
        }

        private byte[] DownloadBits(WebRequest request, Stream writeStream, CompletionDelegate completionDelegate, AsyncOperation asyncOp)
        {
            WebResponse webResponse = null;
            DownloadBitsState downloadBitsState = new DownloadBitsState(request, writeStream, completionDelegate, asyncOp, m_Progress, this);
            if (downloadBitsState.Async)
            {
                request.BeginGetResponse(DownloadBitsResponseCallback, downloadBitsState);
                return null;
            }
            int bytesRetrieved = downloadBitsState.SetResponse(m_WebResponse = GetWebResponse(request));
            while (!downloadBitsState.RetrieveBytes(ref bytesRetrieved))
            {
            }
            downloadBitsState.Close();
            return downloadBitsState.InnerBuffer;
        }

        private static void UploadBitsRequestCallback(IAsyncResult result)
        {
            UploadBitsState uploadBitsState = (UploadBitsState)result.AsyncState;
            WebRequest request = uploadBitsState.Request;
            Exception ex = null;
            try
            {
                Stream requestStream = request.EndGetRequestStream(result);
                uploadBitsState.SetRequestStream(requestStream);
            }
            catch (Exception ex2)
            {
                if (ex2 is ThreadAbortException || ex2 is StackOverflowException || ex2 is OutOfMemoryException)
                {
                    throw;
                }
                ex = ex2;
                if (!(ex2 is WebException) && !(ex2 is SecurityException))
                {
                    ex = new WebException(("net_webclient"), ex2);
                }
                AbortRequest(request);
                if (uploadBitsState != null && uploadBitsState.ReadStream != null)
                {
                    uploadBitsState.ReadStream.Close();
                }
            }
            finally
            {
                if (ex != null)
                {
                    uploadBitsState.UploadCompletionDelegate(null, ex, uploadBitsState);
                }
            }
        }

        private static void UploadBitsWriteCallback(IAsyncResult result)
        {
            UploadBitsState uploadBitsState = (UploadBitsState)result.AsyncState;
            Stream writeStream = uploadBitsState.WriteStream;
            Exception ex = null;
            bool flag = false;
            try
            {
                writeStream.EndWrite(result);
                flag = uploadBitsState.WriteBytes();
            }
            catch (Exception ex2)
            {
                flag = true;
                if (ex2 is ThreadAbortException || ex2 is StackOverflowException || ex2 is OutOfMemoryException)
                {
                    throw;
                }
                ex = ex2;
                if (!(ex2 is WebException) && !(ex2 is SecurityException))
                {
                    ex = new WebException(("net_webclient"), ex2);
                }
                AbortRequest(uploadBitsState.Request);
                if (uploadBitsState != null && uploadBitsState.ReadStream != null)
                {
                    uploadBitsState.ReadStream.Close();
                }
            }
            finally
            {
                if (flag)
                {
                    if (ex == null)
                    {
                        uploadBitsState.Close();
                    }
                    uploadBitsState.UploadCompletionDelegate(null, ex, uploadBitsState);
                }
            }
        }

        private void UploadBits(WebRequest request, Stream readStream, byte[] buffer, int chunkSize, byte[] header, byte[] footer, CompletionDelegate uploadCompletionDelegate, CompletionDelegate downloadCompletionDelegate, AsyncOperation asyncOp)
        {
            if (request.RequestUri.Scheme == Uri.UriSchemeFile)
            {
                header = (footer = null);
            }
            UploadBitsState uploadBitsState = new UploadBitsState(request, readStream, buffer, chunkSize, header, footer, uploadCompletionDelegate, downloadCompletionDelegate, asyncOp, m_Progress, this);
            if (uploadBitsState.Async)
            {
                request.BeginGetRequestStream(UploadBitsRequestCallback, uploadBitsState);
                return;
            }
            Stream requestStream = request.GetRequestStream();
            uploadBitsState.SetRequestStream(requestStream);
            while (!uploadBitsState.WriteBytes())
            {
            }
            uploadBitsState.Close();
        }

        private bool ByteArrayHasPrefix(byte[] prefix, byte[] byteArray)
        {
            if (prefix == null || byteArray == null || prefix.Length > byteArray.Length)
            {
                return false;
            }
            for (int i = 0; i < prefix.Length; i++)
            {
                if (prefix[i] != byteArray[i])
                {
                    return false;
                }
            }
            return true;
        }

        private string GetStringUsingEncoding(WebRequest request, byte[] data)
        {
            Encoding encoding = null;
            int num = -1;
            string text;
            try
            {
                text = request.ContentType;
            }
            catch (NotImplementedException)
            {
                text = null;
            }
            catch (NotSupportedException)
            {
                text = null;
            }
            if (text != null)
            {
                text = text.ToLower(CultureInfo.InvariantCulture);
                string[] array = text.Split(';', '=', ' ');
                bool flag = false;
                string[] array2 = array;
                foreach (string text2 in array2)
                {
                    if (text2 == "charset")
                    {
                        flag = true;
                    }
                    else if (flag)
                    {
                        try
                        {
                            encoding = Encoding.GetEncoding(text2);
                        }
                        catch (ArgumentException)
                        {
                            goto IL_0082;
                        }
                    }
                }
            }
            goto IL_0082;
        IL_0082:
            if (encoding == null)
            {
                Encoding[] array3 = new Encoding[4]
                {
                Encoding.UTF8,
                Encoding.UTF32,
                Encoding.Unicode,
                Encoding.BigEndianUnicode
                };
                for (int j = 0; j < array3.Length; j++)
                {
                    byte[] preamble = array3[j].GetPreamble();
                    if (ByteArrayHasPrefix(preamble, data))
                    {
                        encoding = array3[j];
                        num = preamble.Length;
                        break;
                    }
                }
            }
            if (encoding == null)
            {
                encoding = Encoding;
            }
            if (num == -1)
            {
                byte[] preamble2 = encoding.GetPreamble();
                num = (ByteArrayHasPrefix(preamble2, data) ? preamble2.Length : 0);
            }
            return encoding.GetString(data, num, data.Length - num);
        }

        private string MapToDefaultMethod(Uri address)
        {
            Uri uri = (address.IsAbsoluteUri || !(m_baseAddress != null)) ? address : new Uri(m_baseAddress, address);
            if (uri.Scheme.ToLower(CultureInfo.InvariantCulture) == "ftp")
            {
                return "STOR";
            }
            return "POST";
        }

        private static string UrlEncode(string str)
        {
            if (str == null)
            {
                return null;
            }
            return UrlEncode(str, Encoding.UTF8);
        }

        private static string UrlEncode(string str, Encoding e)
        {
            if (str == null)
            {
                return null;
            }
            return Encoding.ASCII.GetString(UrlEncodeToBytes(str, e));
        }

        private static byte[] UrlEncodeToBytes(string str, Encoding e)
        {
            if (str == null)
            {
                return null;
            }
            byte[] bytes = e.GetBytes(str);
            return UrlEncodeBytesToBytesInternal(bytes, 0, bytes.Length, alwaysCreateReturnValue: false);
        }

        private static byte[] UrlEncodeBytesToBytesInternal(byte[] bytes, int offset, int count, bool alwaysCreateReturnValue)
        {
            int num = 0;
            int num2 = 0;
            for (int i = 0; i < count; i++)
            {
                char c = (char)bytes[offset + i];
                if (c == ' ')
                {
                    num++;
                }
                else if (!IsSafe(c))
                {
                    num2++;
                }
            }
            if (!alwaysCreateReturnValue && num == 0 && num2 == 0)
            {
                return bytes;
            }
            byte[] array = new byte[count + num2 * 2];
            int num3 = 0;
            for (int j = 0; j < count; j++)
            {
                byte b = bytes[offset + j];
                char c2 = (char)b;
                if (IsSafe(c2))
                {
                    array[num3++] = b;
                    continue;
                }
                if (c2 == ' ')
                {
                    array[num3++] = 43;
                    continue;
                }
                array[num3++] = 37;
                array[num3++] = (byte)IntToHex((b >> 4) & 0xF);
                array[num3++] = (byte)IntToHex(b & 0xF);
            }
            return array;
        }

        private static char IntToHex(int n)
        {
            if (n <= 9)
            {
                return (char)(n + 48);
            }
            return (char)(n - 10 + 97);
        }

        private static bool IsSafe(char ch)
        {
            if ((ch >= 'a' && ch <= 'z') || (ch >= 'A' && ch <= 'Z') || (ch >= '0' && ch <= '9'))
            {
                return true;
            }
            switch (ch)
            {
                case '!':
                case '\'':
                case '(':
                case ')':
                case '*':
                case '-':
                case '.':
                case '_':
                    return true;
                default:
                    return false;
            }
        }

        private void InvokeOperationCompleted(AsyncOperation asyncOp, SendOrPostCallback callback, AsyncCompletedEventArgs eventArgs)
        {
            if (Interlocked.CompareExchange(ref m_AsyncOp, null, asyncOp) == asyncOp)
            {
                CompleteWebClientState();
                asyncOp.PostOperationCompleted(callback, eventArgs);
            }
        }

        private bool AnotherCallInProgress(int callNesting)
        {
            return callNesting > 1;
        }

        protected virtual void OnOpenReadCompleted(WebClientXEventArgs.OpenReadCompletedEventArgs e)
        {
            if (this.OpenReadCompleted != null)
            {
                this.OpenReadCompleted(this, e);
            }
        }

        private void OpenReadOperationCompleted(object arg)
        {
            OnOpenReadCompleted((WebClientXEventArgs.OpenReadCompletedEventArgs)arg);
        }

        private void OpenReadAsyncCallback(IAsyncResult result)
        {
            LazyAsyncResult lazyAsyncResult = (LazyAsyncResult)result;
            AsyncOperation asyncOperation = (AsyncOperation)lazyAsyncResult.AsyncState;
            WebRequest request = (WebRequest)lazyAsyncResult.AsyncObject;
            Stream result2 = null;
            Exception exception = null;
            try
            {
                result2 = (m_WebResponse = GetWebResponse(request, result)).GetResponseStream();
            }
            catch (Exception ex)
            {
                if (ex is ThreadAbortException || ex is StackOverflowException || ex is OutOfMemoryException)
                {
                    throw;
                }
                exception = ex;
                if (!(ex is WebException) && !(ex is SecurityException))
                {
                    exception = new WebException(("net_webclient"), ex);
                }
            }
            //do something
            //AsyncCompletedEventArgs eventArgs= new AsyncCompletedEventArgs(exception, m_Cancelled, asyncOperation.UserSuppliedState);
            MiniBlink.WebClientXEventArgs.OpenReadCompletedEventArgs eventArgs = new MiniBlink.WebClientXEventArgs.OpenReadCompletedEventArgs(result2, exception, m_Cancelled, asyncOperation.UserSuppliedState);
            InvokeOperationCompleted(asyncOperation, openReadOperationCompleted, eventArgs);
        }

        [HostProtection(SecurityAction.LinkDemand, ExternalThreading = true)]
        public void OpenReadAsync(Uri address)
        {
            OpenReadAsync(address, null);
        }

        [HostProtection(SecurityAction.LinkDemand, ExternalThreading = true)]
        public void OpenReadAsync(Uri address, object userToken)
        {
            //if (Logging.On)
            //{
            //    Logging.Enter(Logging.Web, this, "OpenReadAsync", address);
            //}
            if (address == null)
            {
                throw new ArgumentNullException("address");
            }
            InitWebClientAsync();
            ClearWebClientState();
            AsyncOperation asyncOperation = m_AsyncOp = AsyncOperationManager.CreateOperation(userToken);
            try
            {
                (m_WebRequest = GetWebRequest(GetUri(address))).BeginGetResponse(OpenReadAsyncCallback, asyncOperation);
            }
            catch (Exception ex)
            {
                if (ex is ThreadAbortException || ex is StackOverflowException || ex is OutOfMemoryException)
                {
                    throw;
                }
                if (!(ex is WebException) && !(ex is SecurityException))
                {
                    ex = new WebException(("net_webclient"), ex);
                }
                //do something
                //OpenReadCompletedEventArgs eventArgs = new OpenReadCompletedEventArgs(null, ex, m_Cancelled, asyncOperation.UserSuppliedState);
                AsyncCompletedEventArgs eventArgs = new AsyncCompletedEventArgs(ex, m_Cancelled, asyncOperation.UserSuppliedState);
                InvokeOperationCompleted(asyncOperation, openReadOperationCompleted, eventArgs);
            }
            //if (Logging.On)
            //{
            //    Logging.Exit(Logging.Web, this, "OpenReadAsync", null);
            //}
        }

        protected virtual void OnOpenWriteCompleted(OpenWriteCompletedEventArgs e)
        {
            if (this.OpenWriteCompleted != null)
            {
                this.OpenWriteCompleted(this, e);
            }
        }

        private void OpenWriteOperationCompleted(object arg)
        {
            OnOpenWriteCompleted((OpenWriteCompletedEventArgs)arg);
        }

        private void OpenWriteAsyncCallback(IAsyncResult result)
        {
            LazyAsyncResult lazyAsyncResult = (LazyAsyncResult)result;
            AsyncOperation asyncOperation = (AsyncOperation)lazyAsyncResult.AsyncState;
            WebRequest webRequest = (WebRequest)lazyAsyncResult.AsyncObject;
            WebClientWriteStream result2 = null;
            Exception exception = null;
            try
            {
                result2 = new WebClientWriteStream(webRequest.EndGetRequestStream(result), webRequest, this);
            }
            catch (Exception ex)
            {
                if (ex is ThreadAbortException || ex is StackOverflowException || ex is OutOfMemoryException)
                {
                    throw;
                }
                exception = ex;
                if (!(ex is WebException) && !(ex is SecurityException))
                {
                    exception = new WebException(("net_webclient"), ex);
                }
            }
            //do something
            //OpenWriteCompletedEventArgs eventArgs = new OpenWriteCompletedEventArgs(result2, exception, m_Cancelled, asyncOperation.UserSuppliedState);
            AsyncCompletedEventArgs eventArgs = new AsyncCompletedEventArgs(exception, m_Cancelled, asyncOperation.UserSuppliedState);
            InvokeOperationCompleted(asyncOperation, openWriteOperationCompleted, eventArgs);
        }

        [HostProtection(SecurityAction.LinkDemand, ExternalThreading = true)]
        public void OpenWriteAsync(Uri address)
        {
            OpenWriteAsync(address, null, null);
        }

        [HostProtection(SecurityAction.LinkDemand, ExternalThreading = true)]
        public void OpenWriteAsync(Uri address, string method)
        {
            OpenWriteAsync(address, method, null);
        }

        [HostProtection(SecurityAction.LinkDemand, ExternalThreading = true)]
        public void OpenWriteAsync(Uri address, string method, object userToken)
        {
            //if (Logging.On)
            //{
            //    Logging.Enter(Logging.Web, this, "OpenWriteAsync", string.Concat(address, ", ", method));
            //}
            if (address == null)
            {
                throw new ArgumentNullException("address");
            }
            if (method == null)
            {
                method = MapToDefaultMethod(address);
            }
            InitWebClientAsync();
            ClearWebClientState();
            AsyncOperation asyncOperation = m_AsyncOp = AsyncOperationManager.CreateOperation(userToken);
            try
            {
                m_Method = method;
                (m_WebRequest = GetWebRequest(GetUri(address))).BeginGetRequestStream(OpenWriteAsyncCallback, asyncOperation);
            }
            catch (Exception ex)
            {
                if (ex is ThreadAbortException || ex is StackOverflowException || ex is OutOfMemoryException)
                {
                    throw;
                }
                if (!(ex is WebException) && !(ex is SecurityException))
                {
                    ex = new WebException(("net_webclient"), ex);
                }
                //do something
                //OpenWriteCompletedEventArgs eventArgs = new OpenWriteCompletedEventArgs(null, ex, m_Cancelled, asyncOperation.UserSuppliedState);
                AsyncCompletedEventArgs eventArgs = new AsyncCompletedEventArgs(ex, m_Cancelled, asyncOperation.UserSuppliedState);
                InvokeOperationCompleted(asyncOperation, openWriteOperationCompleted, eventArgs);
            }
            //if (Logging.On)
            //{
            //    Logging.Exit(Logging.Web, this, "OpenWriteAsync", null);
            //}
        }

        protected virtual void OnDownloadStringCompleted(DownloadStringCompletedEventArgs e)
        {
            if (this.DownloadStringCompleted != null)
            {
                this.DownloadStringCompleted(this, e);
            }
        }

        private void DownloadStringOperationCompleted(object arg)
        {
            OnDownloadStringCompleted((DownloadStringCompletedEventArgs)arg);
        }

        private void DownloadStringAsyncCallback(byte[] returnBytes, Exception exception, object state)
        {
            AsyncOperation asyncOperation = (AsyncOperation)state;
            string result = null;
            try
            {
                if (returnBytes != null)
                {
                    result = GetStringUsingEncoding(m_WebRequest, returnBytes);
                }
            }
            catch (Exception ex)
            {
                if (ex is ThreadAbortException || ex is StackOverflowException || ex is OutOfMemoryException)
                {
                    throw;
                }
                exception = ex;
            }
            //do something
            //DownloadStringCompletedEventArgs eventArgs = new DownloadStringCompletedEventArgs(result, exception, m_Cancelled, asyncOperation.UserSuppliedState);
            AsyncCompletedEventArgs eventArgs = new AsyncCompletedEventArgs(exception, m_Cancelled, asyncOperation.UserSuppliedState);
            InvokeOperationCompleted(asyncOperation, downloadStringOperationCompleted, eventArgs);
        }

        [HostProtection(SecurityAction.LinkDemand, ExternalThreading = true)]
        public void DownloadStringAsync(Uri address)
        {
            DownloadStringAsync(address, null);
        }

        [HostProtection(SecurityAction.LinkDemand, ExternalThreading = true)]
        public void DownloadStringAsync(Uri address, object userToken)
        {
            //if (Logging.On)
            //{
            //    Logging.Enter(Logging.Web, this, "DownloadStringAsync", address);
            //}
            if (address == null)
            {
                throw new ArgumentNullException("address");
            }
            InitWebClientAsync();
            ClearWebClientState();
            AsyncOperation asyncOperation = m_AsyncOp = AsyncOperationManager.CreateOperation(userToken);
            try
            {
                DownloadBits(m_WebRequest = GetWebRequest(GetUri(address)), null, DownloadStringAsyncCallback, asyncOperation);
            }
            catch (Exception ex)
            {
                if (ex is ThreadAbortException || ex is StackOverflowException || ex is OutOfMemoryException)
                {
                    throw;
                }
                if (!(ex is WebException) && !(ex is SecurityException))
                {
                    ex = new WebException(("net_webclient"), ex);
                }
                DownloadStringAsyncCallback(null, ex, asyncOperation);
            }
            //if (Logging.On)
            //{
            //    Logging.Exit(Logging.Web, this, "DownloadStringAsync", "");
            //}
        }

        protected virtual void OnDownloadDataCompleted(DownloadDataCompletedEventArgs e)
        {
            if (this.DownloadDataCompleted != null)
            {
                this.DownloadDataCompleted(this, e);
            }
        }

        private void DownloadDataOperationCompleted(object arg)
        {
            OnDownloadDataCompleted((DownloadDataCompletedEventArgs)arg);
        }

        private void DownloadDataAsyncCallback(byte[] returnBytes, Exception exception, object state)
        {
            AsyncOperation asyncOperation = (AsyncOperation)state;
            //do something
            //DownloadDataCompletedEventArgs eventArgs = new DownloadDataCompletedEventArgs(returnBytes, exception, m_Cancelled, asyncOperation.UserSuppliedState);
            AsyncCompletedEventArgs eventArgs = new AsyncCompletedEventArgs(exception, m_Cancelled, asyncOperation.UserSuppliedState);

            InvokeOperationCompleted(asyncOperation, downloadDataOperationCompleted, eventArgs);
        }

        [HostProtection(SecurityAction.LinkDemand, ExternalThreading = true)]
        public void DownloadDataAsync(Uri address)
        {
            DownloadDataAsync(address, null);
        }

        [HostProtection(SecurityAction.LinkDemand, ExternalThreading = true)]
        public void DownloadDataAsync(Uri address, object userToken)
        {
            //if (Logging.On)
            //{
            //    Logging.Enter(Logging.Web, this, "DownloadDataAsync", address);
            //}
            if (address == null)
            {
                throw new ArgumentNullException("address");
            }
            InitWebClientAsync();
            ClearWebClientState();
            AsyncOperation asyncOperation = m_AsyncOp = AsyncOperationManager.CreateOperation(userToken);
            try
            {
                DownloadBits(m_WebRequest = GetWebRequest(GetUri(address)), null, DownloadDataAsyncCallback, asyncOperation);
            }
            catch (Exception ex)
            {
                if (ex is ThreadAbortException || ex is StackOverflowException || ex is OutOfMemoryException)
                {
                    throw;
                }
                if (!(ex is WebException) && !(ex is SecurityException))
                {
                    ex = new WebException(("net_webclient"), ex);
                }
                DownloadDataAsyncCallback(null, ex, asyncOperation);
            }
            //if (Logging.On)
            //{
            //    Logging.Exit(Logging.Web, this, "DownloadDataAsync", null);
            //}
        }

        protected virtual void OnDownloadFileCompleted(AsyncCompletedEventArgs e)
        {
            if (this.DownloadFileCompleted != null)
            {
                this.DownloadFileCompleted(this, e);
            }
        }

        private void DownloadFileOperationCompleted(object arg)
        {
            OnDownloadFileCompleted((AsyncCompletedEventArgs)arg);
        }

        private void DownloadFileAsyncCallback(byte[] returnBytes, Exception exception, object state)
        {
            AsyncOperation asyncOperation = (AsyncOperation)state;
            AsyncCompletedEventArgs eventArgs = new AsyncCompletedEventArgs(exception, m_Cancelled, asyncOperation.UserSuppliedState);
            InvokeOperationCompleted(asyncOperation, downloadFileOperationCompleted, eventArgs);
        }

        [HostProtection(SecurityAction.LinkDemand, ExternalThreading = true)]
        public void DownloadFileAsync(Uri address, string fileName)
        {
            DownloadFileAsync(address, fileName, null);
        }

        [HostProtection(SecurityAction.LinkDemand, ExternalThreading = true)]
        public void DownloadFileAsync(Uri address, string fileName, object userToken)
        {
            if (address == null)
            {
                throw new ArgumentNullException("address");
            }
            if (fileName == null)
            {
                throw new ArgumentNullException("fileName");
            }
            FileStream fileStream = null;
            InitWebClientAsync();
            ClearWebClientState();
            AsyncOperation asyncOperation = m_AsyncOp = AsyncOperationManager.CreateOperation(userToken);
            try
            {
                fileStream = new FileStream(fileName, FileMode.Create, FileAccess.Write);
                DownloadBits(m_WebRequest = GetWebRequest(GetUri(address)), fileStream, DownloadFileAsyncCallback, asyncOperation);
            }
            catch (Exception ex)
            {
                if (ex is ThreadAbortException || ex is StackOverflowException || ex is OutOfMemoryException)
                {
                    throw;
                }
                fileStream?.Close();
                if (!(ex is WebException) && !(ex is SecurityException))
                {
                    ex = new WebException(("net_webclient"), ex);
                }
                DownloadFileAsyncCallback(null, ex, asyncOperation);
            }
        }

        protected virtual void OnUploadStringCompleted(UploadStringCompletedEventArgs e)
        {
            if (this.UploadStringCompleted != null)
            {
                this.UploadStringCompleted(this, e);
            }
        }

        private void UploadStringOperationCompleted(object arg)
        {
            OnUploadStringCompleted((UploadStringCompletedEventArgs)arg);
        }

        private void StartDownloadAsync(UploadBitsState state)
        {
            try
            {
                DownloadBits(state.Request, null, state.DownloadCompletionDelegate, state.AsyncOp);
            }
            catch (Exception ex)
            {
                if (ex is ThreadAbortException || ex is StackOverflowException || ex is OutOfMemoryException)
                {
                    throw;
                }
                if (!(ex is WebException) && !(ex is SecurityException))
                {
                    ex = new WebException(("net_webclient"), ex);
                }
                state.DownloadCompletionDelegate(null, ex, state.AsyncOp);
            }
        }

        private void UploadStringAsyncWriteCallback(byte[] returnBytes, Exception exception, object state)
        {
            UploadBitsState uploadBitsState = (UploadBitsState)state;
            if (exception != null)
            {
                //do something
                //UploadStringCompletedEventArgs eventArgs = new UploadStringCompletedEventArgs(null, exception, m_Cancelled, uploadBitsState.AsyncOp.UserSuppliedState);
                AsyncCompletedEventArgs eventArgs = new AsyncCompletedEventArgs(exception, m_Cancelled, uploadBitsState.AsyncOp.UserSuppliedState);
                InvokeOperationCompleted(uploadBitsState.AsyncOp, uploadStringOperationCompleted, eventArgs);
            }
            else
            {
                StartDownloadAsync(uploadBitsState);
            }
        }

        private void UploadStringAsyncReadCallback(byte[] returnBytes, Exception exception, object state)
        {
            AsyncOperation asyncOperation = (AsyncOperation)state;
            string result = null;
            try
            {
                if (returnBytes != null)
                {
                    result = GetStringUsingEncoding(m_WebRequest, returnBytes);
                }
            }
            catch (Exception ex)
            {
                if (ex is ThreadAbortException || ex is StackOverflowException || ex is OutOfMemoryException)
                {
                    throw;
                }
                exception = ex;
            }
            //do something
            //UploadStringCompletedEventArgs eventArgs = new UploadStringCompletedEventArgs(result, exception, m_Cancelled, asyncOperation.UserSuppliedState);
            AsyncCompletedEventArgs eventArgs = new AsyncCompletedEventArgs(exception, m_Cancelled, asyncOperation.UserSuppliedState);

            InvokeOperationCompleted(asyncOperation, uploadStringOperationCompleted, eventArgs);
        }

        [HostProtection(SecurityAction.LinkDemand, ExternalThreading = true)]
        public void UploadStringAsync(Uri address, string data)
        {
            UploadStringAsync(address, null, data, null);
        }

        [HostProtection(SecurityAction.LinkDemand, ExternalThreading = true)]
        public void UploadStringAsync(Uri address, string method, string data)
        {
            UploadStringAsync(address, method, data, null);
        }

        [HostProtection(SecurityAction.LinkDemand, ExternalThreading = true)]
        public void UploadStringAsync(Uri address, string method, string data, object userToken)
        {
            //if (Logging.On)
            //{
            //    Logging.Enter(Logging.Web, this, "UploadStringAsync", address);
            //}
            if (address == null)
            {
                throw new ArgumentNullException("address");
            }
            if (data == null)
            {
                throw new ArgumentNullException("data");
            }
            if (method == null)
            {
                method = MapToDefaultMethod(address);
            }
            InitWebClientAsync();
            ClearWebClientState();
            AsyncOperation asyncOperation = m_AsyncOp = AsyncOperationManager.CreateOperation(userToken);
            try
            {
                byte[] bytes = Encoding.GetBytes(data);
                m_Method = method;
                m_ContentLength = bytes.Length;
                UploadBits(m_WebRequest = GetWebRequest(GetUri(address)), null, bytes, 0, null, null, UploadStringAsyncWriteCallback, UploadStringAsyncReadCallback, asyncOperation);
            }
            catch (Exception ex)
            {
                if (ex is ThreadAbortException || ex is StackOverflowException || ex is OutOfMemoryException)
                {
                    throw;
                }
                if (!(ex is WebException) && !(ex is SecurityException))
                {
                    ex = new WebException(("net_webclient"), ex);
                }
                //do something
                //UploadStringCompletedEventArgs eventArgs = new UploadStringCompletedEventArgs(null, ex, m_Cancelled, asyncOperation.UserSuppliedState);
                AsyncCompletedEventArgs eventArgs = new AsyncCompletedEventArgs(ex, m_Cancelled, asyncOperation.UserSuppliedState);
                InvokeOperationCompleted(asyncOperation, uploadStringOperationCompleted, eventArgs);
            }
            //if (Logging.On)
            //{
            //    Logging.Exit(Logging.Web, this, "UploadStringAsync", null);
            //}
        }

        protected virtual void OnUploadDataCompleted(UploadDataCompletedEventArgs e)
        {
            if (this.UploadDataCompleted != null)
            {
                this.UploadDataCompleted(this, e);
            }
        }

        private void UploadDataOperationCompleted(object arg)
        {
            OnUploadDataCompleted((UploadDataCompletedEventArgs)arg);
        }

        private void UploadDataAsyncWriteCallback(byte[] returnBytes, Exception exception, object state)
        {
            UploadBitsState uploadBitsState = (UploadBitsState)state;
            if (exception != null)
            {
                //do something
                //UploadDataCompletedEventArgs eventArgs = new UploadDataCompletedEventArgs(returnBytes, exception, m_Cancelled, uploadBitsState.AsyncOp.UserSuppliedState);
                AsyncCompletedEventArgs eventArgs = new AsyncCompletedEventArgs(exception, m_Cancelled, uploadBitsState.AsyncOp.UserSuppliedState);
                InvokeOperationCompleted(uploadBitsState.AsyncOp, uploadDataOperationCompleted, eventArgs);
            }
            else
            {
                StartDownloadAsync(uploadBitsState);
            }
        }

        private void UploadDataAsyncReadCallback(byte[] returnBytes, Exception exception, object state)
        {
            AsyncOperation asyncOperation = (AsyncOperation)state;
            //do something
            //UploadDataCompletedEventArgs eventArgs = new UploadDataCompletedEventArgs(returnBytes, exception, m_Cancelled, asyncOperation.UserSuppliedState);
            AsyncCompletedEventArgs eventArgs = new AsyncCompletedEventArgs(exception, m_Cancelled, asyncOperation.UserSuppliedState);
            InvokeOperationCompleted(asyncOperation, uploadDataOperationCompleted, eventArgs);
        }

        [HostProtection(SecurityAction.LinkDemand, ExternalThreading = true)]
        public void UploadDataAsync(Uri address, byte[] data)
        {
            UploadDataAsync(address, null, data, null);
        }

        [HostProtection(SecurityAction.LinkDemand, ExternalThreading = true)]
        public void UploadDataAsync(Uri address, string method, byte[] data)
        {
            UploadDataAsync(address, method, data, null);
        }

        [HostProtection(SecurityAction.LinkDemand, ExternalThreading = true)]
        public void UploadDataAsync(Uri address, string method, byte[] data, object userToken)
        {
            //if (Logging.On)
            //{
            //    Logging.Enter(Logging.Web, this, "UploadDataAsync", string.Concat(address, ", ", method));
            //}
            if (address == null)
            {
                throw new ArgumentNullException("address");
            }
            if (data == null)
            {
                throw new ArgumentNullException("data");
            }
            if (method == null)
            {
                method = MapToDefaultMethod(address);
            }
            InitWebClientAsync();
            ClearWebClientState();
            AsyncOperation asyncOperation = m_AsyncOp = AsyncOperationManager.CreateOperation(userToken);
            int chunkSize = 0;
            try
            {
                m_Method = method;
                m_ContentLength = data.Length;
                WebRequest request = m_WebRequest = GetWebRequest(GetUri(address));
                if (this.UploadProgressChanged != null)
                {
                    chunkSize = (int)Math.Min(8192L, data.Length);
                }
                UploadBits(request, null, data, chunkSize, null, null, UploadDataAsyncWriteCallback, UploadDataAsyncReadCallback, asyncOperation);
            }
            catch (Exception ex)
            {
                if (ex is ThreadAbortException || ex is StackOverflowException || ex is OutOfMemoryException)
                {
                    throw;
                }
                if (!(ex is WebException) && !(ex is SecurityException))
                {
                    ex = new WebException(("net_webclient"), ex);
                }
                //do something
                //UploadDataCompletedEventArgs eventArgs = new UploadDataCompletedEventArgs(null, ex, m_Cancelled, asyncOperation.UserSuppliedState);
                AsyncCompletedEventArgs eventArgs = new AsyncCompletedEventArgs(ex, m_Cancelled, asyncOperation.UserSuppliedState);
                InvokeOperationCompleted(asyncOperation, uploadDataOperationCompleted, eventArgs);
            }
            //if (Logging.On)
            //{
            //    Logging.Exit(Logging.Web, this, "UploadDataAsync", null);
            //}
        }

        protected virtual void OnUploadFileCompleted(UploadFileCompletedEventArgs e)
        {
            if (this.UploadFileCompleted != null)
            {
                this.UploadFileCompleted(this, e);
            }
        }

        private void UploadFileOperationCompleted(object arg)
        {
            OnUploadFileCompleted((UploadFileCompletedEventArgs)arg);
        }

        private void UploadFileAsyncWriteCallback(byte[] returnBytes, Exception exception, object state)
        {
            UploadBitsState uploadBitsState = (UploadBitsState)state;
            if (exception != null)
            {
                //do something
                //UploadFileCompletedEventArgs eventArgs = new UploadFileCompletedEventArgs(returnBytes, exception, m_Cancelled, uploadBitsState.AsyncOp.UserSuppliedState);
                AsyncCompletedEventArgs eventArgs = new AsyncCompletedEventArgs(exception, m_Cancelled, uploadBitsState.AsyncOp.UserSuppliedState);
                InvokeOperationCompleted(uploadBitsState.AsyncOp, uploadFileOperationCompleted, eventArgs);
            }
            else
            {
                StartDownloadAsync(uploadBitsState);
            }
        }

        private void UploadFileAsyncReadCallback(byte[] returnBytes, Exception exception, object state)
        {
            AsyncOperation asyncOperation = (AsyncOperation)state;
            //do something
            //UploadFileCompletedEventArgs eventArgs = new UploadFileCompletedEventArgs(returnBytes, exception, m_Cancelled, asyncOperation.UserSuppliedState);
            AsyncCompletedEventArgs eventArgs = new AsyncCompletedEventArgs(exception, m_Cancelled, asyncOperation.UserSuppliedState);
            InvokeOperationCompleted(asyncOperation, uploadFileOperationCompleted, eventArgs);
        }

        [HostProtection(SecurityAction.LinkDemand, ExternalThreading = true)]
        public void UploadFileAsync(Uri address, string fileName)
        {
            UploadFileAsync(address, null, fileName, null);
        }

        [HostProtection(SecurityAction.LinkDemand, ExternalThreading = true)]
        public void UploadFileAsync(Uri address, string method, string fileName)
        {
            UploadFileAsync(address, method, fileName, null);
        }

        [HostProtection(SecurityAction.LinkDemand, ExternalThreading = true)]
        public void UploadFileAsync(Uri address, string method, string fileName, object userToken)
        {
            //if (Logging.On)
            //{
            //    Logging.Enter(Logging.Web, this, "UploadFileAsync", string.Concat(address, ", ", method));
            //}
            if (address == null)
            {
                throw new ArgumentNullException("address");
            }
            if (fileName == null)
            {
                throw new ArgumentNullException("fileName");
            }
            if (method == null)
            {
                method = MapToDefaultMethod(address);
            }
            InitWebClientAsync();
            ClearWebClientState();
            AsyncOperation asyncOperation = m_AsyncOp = AsyncOperationManager.CreateOperation(userToken);
            FileStream fs = null;
            try
            {
                m_Method = method;
                byte[] formHeaderBytes = null;
                byte[] boundaryBytes = null;
                byte[] buffer = null;
                Uri uri = GetUri(address);
                bool needsHeaderAndBoundary = uri.Scheme != Uri.UriSchemeFile;
                OpenFileInternal(needsHeaderAndBoundary, fileName, ref fs, ref buffer, ref formHeaderBytes, ref boundaryBytes);
                UploadBits(m_WebRequest = GetWebRequest(uri), fs, buffer, 0, formHeaderBytes, boundaryBytes, UploadFileAsyncWriteCallback, UploadFileAsyncReadCallback, asyncOperation);
            }
            catch (Exception ex)
            {
                if (ex is ThreadAbortException || ex is StackOverflowException || ex is OutOfMemoryException)
                {
                    throw;
                }
                fs?.Close();
                if (!(ex is WebException) && !(ex is SecurityException))
                {
                    ex = new WebException(("net_webclient"), ex);
                }
                //do something
                //UploadFileCompletedEventArgs eventArgs = new UploadFileCompletedEventArgs(null, ex, m_Cancelled, asyncOperation.UserSuppliedState);
                AsyncCompletedEventArgs eventArgs = new AsyncCompletedEventArgs(ex, m_Cancelled, asyncOperation.UserSuppliedState);
                InvokeOperationCompleted(asyncOperation, uploadFileOperationCompleted, eventArgs);
            }
            //if (Logging.On)
            //{
            //    Logging.Exit(Logging.Web, this, "UploadFileAsync", null);
            //}
        }

        protected virtual void OnUploadValuesCompleted(UploadValuesCompletedEventArgs e)
        {
            if (this.UploadValuesCompleted != null)
            {
                this.UploadValuesCompleted(this, e);
            }
        }

        private void UploadValuesOperationCompleted(object arg)
        {
            OnUploadValuesCompleted((UploadValuesCompletedEventArgs)arg);
        }

        private void UploadValuesAsyncWriteCallback(byte[] returnBytes, Exception exception, object state)
        {
            UploadBitsState uploadBitsState = (UploadBitsState)state;
            if (exception != null)
            {
                //do something
                //UploadValuesCompletedEventArgs eventArgs = new UploadValuesCompletedEventArgs(returnBytes, exception, m_Cancelled, uploadBitsState.AsyncOp.UserSuppliedState);
                AsyncCompletedEventArgs eventArgs = new AsyncCompletedEventArgs(exception, m_Cancelled, uploadBitsState.AsyncOp.UserSuppliedState);
                InvokeOperationCompleted(uploadBitsState.AsyncOp, uploadValuesOperationCompleted, eventArgs);
            }
            else
            {
                StartDownloadAsync(uploadBitsState);
            }
        }

        private void UploadValuesAsyncReadCallback(byte[] returnBytes, Exception exception, object state)
        {
            AsyncOperation asyncOperation = (AsyncOperation)state;
            //do something
            //UploadValuesCompletedEventArgs eventArgs = new UploadValuesCompletedEventArgs(returnBytes, exception, m_Cancelled, asyncOperation.UserSuppliedState);
            AsyncCompletedEventArgs eventArgs = new AsyncCompletedEventArgs(exception, m_Cancelled, asyncOperation.UserSuppliedState);
            InvokeOperationCompleted(asyncOperation, uploadValuesOperationCompleted, eventArgs);
        }

        [HostProtection(SecurityAction.LinkDemand, ExternalThreading = true)]
        public void UploadValuesAsync(Uri address, NameValueCollection data)
        {
            UploadValuesAsync(address, null, data, null);
        }

        [HostProtection(SecurityAction.LinkDemand, ExternalThreading = true)]
        public void UploadValuesAsync(Uri address, string method, NameValueCollection data)
        {
            UploadValuesAsync(address, method, data, null);
        }

        [HostProtection(SecurityAction.LinkDemand, ExternalThreading = true)]
        public void UploadValuesAsync(Uri address, string method, NameValueCollection data, object userToken)
        {
            //if (Logging.On)
            //{
            //    Logging.Enter(Logging.Web, this, "UploadValuesAsync", string.Concat(address, ", ", method));
            //}
            if (address == null)
            {
                throw new ArgumentNullException("address");
            }
            if (data == null)
            {
                throw new ArgumentNullException("data");
            }
            if (method == null)
            {
                method = MapToDefaultMethod(address);
            }
            InitWebClientAsync();
            ClearWebClientState();
            AsyncOperation asyncOperation = m_AsyncOp = AsyncOperationManager.CreateOperation(userToken);
            int chunkSize = 0;
            try
            {
                byte[] array = UploadValuesInternal(data);
                m_Method = method;
                WebRequest request = m_WebRequest = GetWebRequest(GetUri(address));
                if (this.UploadProgressChanged != null)
                {
                    chunkSize = (int)Math.Min(8192L, array.Length);
                }
                UploadBits(request, null, array, chunkSize, null, null, UploadValuesAsyncWriteCallback, UploadValuesAsyncReadCallback, asyncOperation);
            }
            catch (Exception ex)
            {
                if (ex is ThreadAbortException || ex is StackOverflowException || ex is OutOfMemoryException)
                {
                    throw;
                }
                if (!(ex is WebException) && !(ex is SecurityException))
                {
                    ex = new WebException(("net_webclient"), ex);
                }
                //do something
                //UploadValuesCompletedEventArgs eventArgs = new UploadValuesCompletedEventArgs(null, ex, m_Cancelled, asyncOperation.UserSuppliedState);
                AsyncCompletedEventArgs eventArgs = new AsyncCompletedEventArgs(ex, m_Cancelled, asyncOperation.UserSuppliedState);
                InvokeOperationCompleted(asyncOperation, uploadValuesOperationCompleted, eventArgs);
            }
            //if (Logging.On)
            //{
            //    Logging.Exit(Logging.Web, this, "UploadValuesAsync", null);
            //}
        }

        public void CancelAsync()
        {
            WebRequest webRequest = m_WebRequest;
            m_Cancelled = true;
            AbortRequest(webRequest);
        }

        [ComVisible(false)]
        [HostProtection(SecurityAction.LinkDemand, ExternalThreading = true)]
        public Task<string> DownloadStringTaskAsync(string address)
        {
            return DownloadStringTaskAsync(GetUri(address));
        }

        [ComVisible(false)]
        [HostProtection(SecurityAction.LinkDemand, ExternalThreading = true)]
        public Task<string> DownloadStringTaskAsync(Uri address)
        {
            TaskCompletionSource<string> tcs = new TaskCompletionSource<string>(address);
            DownloadStringCompletedEventHandler handler = null;
            handler = delegate (object sender, DownloadStringCompletedEventArgs e)
            {
                HandleCompletion(tcs, e, (DownloadStringCompletedEventArgs args) => args.Result, handler, delegate (WebClientX webClient, DownloadStringCompletedEventHandler completion)
                {
                    webClient.DownloadStringCompleted -= completion;
                });
            };
            DownloadStringCompleted += handler;
            try
            {
                DownloadStringAsync(address, tcs);
            }
            catch
            {
                DownloadStringCompleted -= handler;
                throw;
            }
            return tcs.Task;
        }

        [ComVisible(false)]
        [HostProtection(SecurityAction.LinkDemand, ExternalThreading = true)]
        public Task<Stream> OpenReadTaskAsync(string address)
        {
            return OpenReadTaskAsync(GetUri(address));
        }

        [ComVisible(false)]
        [HostProtection(SecurityAction.LinkDemand, ExternalThreading = true)]
        public Task<Stream> OpenReadTaskAsync(Uri address)
        {
            TaskCompletionSource<Stream> tcs = new TaskCompletionSource<Stream>(address);
            WebClientEventHandler.OpenReadCompletedEventHandler handler = null;
            handler = delegate (object sender, WebClientXEventArgs.OpenReadCompletedEventArgs e)
            {
                HandleCompletion(tcs, e, (WebClientXEventArgs.OpenReadCompletedEventArgs args) => args.Result, handler, delegate (WebClientX webClient, WebClientEventHandler.OpenReadCompletedEventHandler completion)
                {
                    webClient.OpenReadCompleted -= completion;
                });
            };
            OpenReadCompleted += handler;
            try
            {
                OpenReadAsync(address, tcs);
            }
            catch
            {
                OpenReadCompleted -= handler;
                throw;
            }
            return tcs.Task;
        }

        [ComVisible(false)]
        [HostProtection(SecurityAction.LinkDemand, ExternalThreading = true)]
        public Task<Stream> OpenWriteTaskAsync(string address)
        {
            return OpenWriteTaskAsync(GetUri(address), null);
        }

        [ComVisible(false)]
        [HostProtection(SecurityAction.LinkDemand, ExternalThreading = true)]
        public Task<Stream> OpenWriteTaskAsync(Uri address)
        {
            return OpenWriteTaskAsync(address, null);
        }

        [ComVisible(false)]
        [HostProtection(SecurityAction.LinkDemand, ExternalThreading = true)]
        public Task<Stream> OpenWriteTaskAsync(string address, string method)
        {
            return OpenWriteTaskAsync(GetUri(address), method);
        }

        [ComVisible(false)]
        [HostProtection(SecurityAction.LinkDemand, ExternalThreading = true)]
        public Task<Stream> OpenWriteTaskAsync(Uri address, string method)
        {
            TaskCompletionSource<Stream> tcs = new TaskCompletionSource<Stream>(address);
            OpenWriteCompletedEventHandler handler = null;
            handler = delegate (object sender, OpenWriteCompletedEventArgs e)
            {
                HandleCompletion(tcs, e, (OpenWriteCompletedEventArgs args) => args.Result, handler, delegate (WebClientX webClient, OpenWriteCompletedEventHandler completion)
                {
                    webClient.OpenWriteCompleted -= completion;
                });
            };
            OpenWriteCompleted += handler;
            try
            {
                OpenWriteAsync(address, method, tcs);
            }
            catch
            {
                OpenWriteCompleted -= handler;
                throw;
            }
            return tcs.Task;
        }

        [ComVisible(false)]
        [HostProtection(SecurityAction.LinkDemand, ExternalThreading = true)]
        public Task<string> UploadStringTaskAsync(string address, string data)
        {
            return UploadStringTaskAsync(address, null, data);
        }

        [ComVisible(false)]
        [HostProtection(SecurityAction.LinkDemand, ExternalThreading = true)]
        public Task<string> UploadStringTaskAsync(Uri address, string data)
        {
            return UploadStringTaskAsync(address, null, data);
        }

        [ComVisible(false)]
        [HostProtection(SecurityAction.LinkDemand, ExternalThreading = true)]
        public Task<string> UploadStringTaskAsync(string address, string method, string data)
        {
            return UploadStringTaskAsync(GetUri(address), method, data);
        }

        [ComVisible(false)]
        [HostProtection(SecurityAction.LinkDemand, ExternalThreading = true)]
        public Task<string> UploadStringTaskAsync(Uri address, string method, string data)
        {
            TaskCompletionSource<string> tcs = new TaskCompletionSource<string>(address);
            UploadStringCompletedEventHandler handler = null;
            handler = delegate (object sender, UploadStringCompletedEventArgs e)
            {
                HandleCompletion(tcs, e, (UploadStringCompletedEventArgs args) => args.Result, handler, delegate (WebClientX webClient, UploadStringCompletedEventHandler completion)
                {
                    webClient.UploadStringCompleted -= completion;
                });
            };
            UploadStringCompleted += handler;
            try
            {
                UploadStringAsync(address, method, data, tcs);
            }
            catch
            {
                UploadStringCompleted -= handler;
                throw;
            }
            return tcs.Task;
        }

        [ComVisible(false)]
        [HostProtection(SecurityAction.LinkDemand, ExternalThreading = true)]
        public Task<byte[]> DownloadDataTaskAsync(string address)
        {
            return DownloadDataTaskAsync(GetUri(address));
        }

        [ComVisible(false)]
        [HostProtection(SecurityAction.LinkDemand, ExternalThreading = true)]
        public Task<byte[]> DownloadDataTaskAsync(Uri address)
        {
            TaskCompletionSource<byte[]> tcs = new TaskCompletionSource<byte[]>(address);
            DownloadDataCompletedEventHandler handler = null;
            handler = delegate (object sender, DownloadDataCompletedEventArgs e)
            {
                HandleCompletion(tcs, e, (DownloadDataCompletedEventArgs args) => args.Result, handler, delegate (WebClientX webClient, DownloadDataCompletedEventHandler completion)
                {
                    webClient.DownloadDataCompleted -= completion;
                });
            };
            DownloadDataCompleted += handler;
            try
            {
                DownloadDataAsync(address, tcs);
            }
            catch
            {
                DownloadDataCompleted -= handler;
                throw;
            }
            return tcs.Task;
        }

        [ComVisible(false)]
        [HostProtection(SecurityAction.LinkDemand, ExternalThreading = true)]
        public Task DownloadFileTaskAsync(string address, string fileName)
        {
            return DownloadFileTaskAsync(GetUri(address), fileName);
        }

        [ComVisible(false)]
        [HostProtection(SecurityAction.LinkDemand, ExternalThreading = true)]
        public Task DownloadFileTaskAsync(Uri address, string fileName)
        {
            TaskCompletionSource<object> tcs = new TaskCompletionSource<object>(address);
            AsyncCompletedEventHandler handler = null;
            handler = delegate (object sender, AsyncCompletedEventArgs e)
            {
                HandleCompletion(tcs, e, (AsyncCompletedEventArgs args) => null, handler, delegate (WebClientX webClient, AsyncCompletedEventHandler completion)
                {
                    webClient.DownloadFileCompleted -= completion;
                });
            };
            DownloadFileCompleted += handler;
            try
            {
                DownloadFileAsync(address, fileName, tcs);
            }
            catch
            {
                DownloadFileCompleted -= handler;
                throw;
            }
            return tcs.Task;
        }

        [ComVisible(false)]
        [HostProtection(SecurityAction.LinkDemand, ExternalThreading = true)]
        public Task<byte[]> UploadDataTaskAsync(string address, byte[] data)
        {
            return UploadDataTaskAsync(GetUri(address), null, data);
        }

        [ComVisible(false)]
        [HostProtection(SecurityAction.LinkDemand, ExternalThreading = true)]
        public Task<byte[]> UploadDataTaskAsync(Uri address, byte[] data)
        {
            return UploadDataTaskAsync(address, null, data);
        }

        [ComVisible(false)]
        [HostProtection(SecurityAction.LinkDemand, ExternalThreading = true)]
        public Task<byte[]> UploadDataTaskAsync(string address, string method, byte[] data)
        {
            return UploadDataTaskAsync(GetUri(address), method, data);
        }

        [ComVisible(false)]
        [HostProtection(SecurityAction.LinkDemand, ExternalThreading = true)]
        public Task<byte[]> UploadDataTaskAsync(Uri address, string method, byte[] data)
        {
            TaskCompletionSource<byte[]> tcs = new TaskCompletionSource<byte[]>(address);
            UploadDataCompletedEventHandler handler = null;
            handler = delegate (object sender, UploadDataCompletedEventArgs e)
            {
                HandleCompletion(tcs, e, (UploadDataCompletedEventArgs args) => args.Result, handler, delegate (WebClientX webClient, UploadDataCompletedEventHandler completion)
                {
                    webClient.UploadDataCompleted -= completion;
                });
            };
            UploadDataCompleted += handler;
            try
            {
                UploadDataAsync(address, method, data, tcs);
            }
            catch
            {
                UploadDataCompleted -= handler;
                throw;
            }
            return tcs.Task;
        }

        [ComVisible(false)]
        [HostProtection(SecurityAction.LinkDemand, ExternalThreading = true)]
        public Task<byte[]> UploadFileTaskAsync(string address, string fileName)
        {
            return UploadFileTaskAsync(GetUri(address), null, fileName);
        }

        [ComVisible(false)]
        [HostProtection(SecurityAction.LinkDemand, ExternalThreading = true)]
        public Task<byte[]> UploadFileTaskAsync(Uri address, string fileName)
        {
            return UploadFileTaskAsync(address, null, fileName);
        }

        [ComVisible(false)]
        [HostProtection(SecurityAction.LinkDemand, ExternalThreading = true)]
        public Task<byte[]> UploadFileTaskAsync(string address, string method, string fileName)
        {
            return UploadFileTaskAsync(GetUri(address), method, fileName);
        }

        [ComVisible(false)]
        [HostProtection(SecurityAction.LinkDemand, ExternalThreading = true)]
        public Task<byte[]> UploadFileTaskAsync(Uri address, string method, string fileName)
        {
            TaskCompletionSource<byte[]> tcs = new TaskCompletionSource<byte[]>(address);
            UploadFileCompletedEventHandler handler = null;
            handler = delegate (object sender, UploadFileCompletedEventArgs e)
            {
                HandleCompletion(tcs, e, (UploadFileCompletedEventArgs args) => args.Result, handler, delegate (WebClientX webClient, UploadFileCompletedEventHandler completion)
                {
                    webClient.UploadFileCompleted -= completion;
                });
            };
            UploadFileCompleted += handler;
            try
            {
                UploadFileAsync(address, method, fileName, tcs);
            }
            catch
            {
                UploadFileCompleted -= handler;
                throw;
            }
            return tcs.Task;
        }

        [ComVisible(false)]
        [HostProtection(SecurityAction.LinkDemand, ExternalThreading = true)]
        public Task<byte[]> UploadValuesTaskAsync(string address, NameValueCollection data)
        {
            return UploadValuesTaskAsync(GetUri(address), null, data);
        }

        [ComVisible(false)]
        [HostProtection(SecurityAction.LinkDemand, ExternalThreading = true)]
        public Task<byte[]> UploadValuesTaskAsync(string address, string method, NameValueCollection data)
        {
            return UploadValuesTaskAsync(GetUri(address), method, data);
        }

        [ComVisible(false)]
        [HostProtection(SecurityAction.LinkDemand, ExternalThreading = true)]
        public Task<byte[]> UploadValuesTaskAsync(Uri address, NameValueCollection data)
        {
            return UploadValuesTaskAsync(address, null, data);
        }

        [ComVisible(false)]
        [HostProtection(SecurityAction.LinkDemand, ExternalThreading = true)]
        public Task<byte[]> UploadValuesTaskAsync(Uri address, string method, NameValueCollection data)
        {
            TaskCompletionSource<byte[]> tcs = new TaskCompletionSource<byte[]>(address);
            UploadValuesCompletedEventHandler handler = null;
            handler = delegate (object sender, UploadValuesCompletedEventArgs e)
            {
                HandleCompletion(tcs, e, (UploadValuesCompletedEventArgs args) => args.Result, handler, delegate (WebClientX webClient, UploadValuesCompletedEventHandler completion)
                {
                    webClient.UploadValuesCompleted -= completion;
                });
            };
            UploadValuesCompleted += handler;
            try
            {
                UploadValuesAsync(address, method, data, tcs);
            }
            catch
            {
                UploadValuesCompleted -= handler;
                throw;
            }
            return tcs.Task;
        }

        private void HandleCompletion<TAsyncCompletedEventArgs, TCompletionDelegate, T>(TaskCompletionSource<T> tcs, TAsyncCompletedEventArgs e, Func<TAsyncCompletedEventArgs, T> getResult, TCompletionDelegate handler, Action<WebClientX, TCompletionDelegate> unregisterHandler) where TAsyncCompletedEventArgs : AsyncCompletedEventArgs
        {
            if (e.UserState == tcs)
            {
                try
                {
                    unregisterHandler(this, handler);
                }
                finally
                {
                    if (e.Error != null)
                    {
                        tcs.TrySetException(e.Error);
                    }
                    else if (e.Cancelled)
                    {
                        tcs.TrySetCanceled();
                    }
                    else
                    {
                        tcs.TrySetResult(getResult(e));
                    }
                }
            }
        }

        protected virtual void OnDownloadProgressChanged(DownloadProgressChangedEventArgs e)
        {
            if (this.DownloadProgressChanged != null)
            {
                this.DownloadProgressChanged(this, e);
            }
        }

        protected virtual void OnUploadProgressChanged(UploadProgressChangedEventArgs e)
        {
            if (this.UploadProgressChanged != null)
            {
                this.UploadProgressChanged(this, e);
            }
        }

        private void ReportDownloadProgressChanged(object arg)
        {
            OnDownloadProgressChanged((DownloadProgressChangedEventArgs)arg);
        }

        private void ReportUploadProgressChanged(object arg)
        {
            OnUploadProgressChanged((UploadProgressChangedEventArgs)arg);
        }

        private void PostProgressChanged(AsyncOperation asyncOp, ProgressData progress)
        {
            if (asyncOp != null && progress.BytesSent + progress.BytesReceived > 0)
            {
                if (progress.HasUploadPhase)
                {
                    //asyncOp.Post(arg: new UploadProgressChangedEventArgs(progress.BytesReceived, progress.TotalBytesToReceive, progress.BytesSent,  progress.TotalBytesToSend), d: reportUploadProgressChanged);
                }
                else
                {
                    int progressPercentage = (int)((progress.TotalBytesToReceive >= 0) ? ((progress.TotalBytesToReceive == 0L) ? 100 : (100 * progress.BytesReceived / progress.TotalBytesToReceive)) : 0);
                    //asyncOp.Post(reportDownloadProgressChanged, new DownloadProgressChangedEventArgs(progressPercentage, asyncOp.UserSuppliedState, progress.BytesReceived, progress.TotalBytesToReceive));
                }
            }
        }
    }

    #region 其它类


    //缺失的类
    internal class LazyAsyncResult : IAsyncResult
    {
        private class ThreadContext
        {
            internal int m_NestedIOCount;
        }

        private const int c_HighBit = int.MinValue;

        private const int c_ForceAsyncCount = 50;

        [ThreadStatic]
        private static ThreadContext t_ThreadContext;

        private object m_AsyncObject;

        private object m_AsyncState;

        private AsyncCallback m_AsyncCallback;

        private object m_Result;

        private int m_ErrorCode;

        private int m_IntCompleted;

        private bool m_EndCalled;

        private bool m_UserEvent;

        private object m_Event;

        private static ThreadContext CurrentThreadContext
        {
            get
            {
                ThreadContext threadContext = t_ThreadContext;
                if (threadContext == null)
                {
                    threadContext = (t_ThreadContext = new ThreadContext());
                }
                return threadContext;
            }
        }

        internal object AsyncObject => m_AsyncObject;

        public object AsyncState => m_AsyncState;

        protected AsyncCallback AsyncCallback
        {
            get
            {
                return m_AsyncCallback;
            }
            set
            {
                m_AsyncCallback = value;
            }
        }

        public WaitHandle AsyncWaitHandle
        {
            get
            {
                m_UserEvent = true;
                if (m_IntCompleted == 0)
                {
                    Interlocked.CompareExchange(ref m_IntCompleted, int.MinValue, 0);
                }
                ManualResetEvent waitHandle = (ManualResetEvent)m_Event;
                while (waitHandle == null)
                {
                    LazilyCreateEvent(out waitHandle);
                }
                return waitHandle;
            }
        }

        public bool CompletedSynchronously
        {
            get
            {
                int num = m_IntCompleted;
                if (num == 0)
                {
                    num = Interlocked.CompareExchange(ref m_IntCompleted, int.MinValue, 0);
                }
                return num > 0;
            }
        }

        public bool IsCompleted
        {
            get
            {
                int num = m_IntCompleted;
                if (num == 0)
                {
                    num = Interlocked.CompareExchange(ref m_IntCompleted, int.MinValue, 0);
                }
                return (num & int.MaxValue) != 0;
            }
        }

        internal bool InternalPeekCompleted => (m_IntCompleted & int.MaxValue) != 0;

        internal object Result
        {
            get
            {
                if (m_Result != DBNull.Value)
                {
                    return m_Result;
                }
                return null;
            }
            set
            {
                m_Result = value;
            }
        }

        internal bool EndCalled
        {
            get
            {
                return m_EndCalled;
            }
            set
            {
                m_EndCalled = value;
            }
        }

        internal int ErrorCode
        {
            get
            {
                return m_ErrorCode;
            }
            set
            {
                m_ErrorCode = value;
            }
        }

        internal LazyAsyncResult(object myObject, object myState, AsyncCallback myCallBack)
        {
            m_AsyncObject = myObject;
            m_AsyncState = myState;
            m_AsyncCallback = myCallBack;
            m_Result = DBNull.Value;
        }

        internal LazyAsyncResult(object myObject, object myState, AsyncCallback myCallBack, object result)
        {
            m_AsyncObject = myObject;
            m_AsyncState = myState;
            m_AsyncCallback = myCallBack;
            m_Result = result;
            m_IntCompleted = 1;
            if (m_AsyncCallback != null)
            {
                m_AsyncCallback(this);
            }
        }

        private bool LazilyCreateEvent(out ManualResetEvent waitHandle)
        {
            waitHandle = new ManualResetEvent(initialState: false);
            try
            {
                if (Interlocked.CompareExchange(ref m_Event, waitHandle, null) == null)
                {
                    if (InternalPeekCompleted)
                    {
                        waitHandle.Set();
                    }
                    return true;
                }
                waitHandle.Close();
                waitHandle = (ManualResetEvent)m_Event;
                return false;
            }
            catch
            {
                m_Event = null;
                if (waitHandle != null)
                {
                    waitHandle.Close();
                }
                throw;
            }
        }

        [Conditional("DEBUG")]
        protected void DebugProtectState(bool protect)
        {
        }

        protected void ProtectedInvokeCallback(object result, IntPtr userToken)
        {
            if (result == DBNull.Value)
            {
                throw new ArgumentNullException("result");
            }
            if ((m_IntCompleted & int.MaxValue) == 0 && (Interlocked.Increment(ref m_IntCompleted) & int.MaxValue) == 1)
            {
                if (m_Result == DBNull.Value)
                {
                    m_Result = result;
                }
                ManualResetEvent manualResetEvent = (ManualResetEvent)m_Event;
                if (manualResetEvent != null)
                {
                    try
                    {
                        manualResetEvent.Set();
                    }
                    catch (ObjectDisposedException)
                    {
                    }
                }
                Complete(userToken);
            }
        }

        internal void InvokeCallback(object result)
        {
            ProtectedInvokeCallback(result, IntPtr.Zero);
        }

        internal void InvokeCallback()
        {
            ProtectedInvokeCallback(null, IntPtr.Zero);
        }

        protected virtual void Complete(IntPtr userToken)
        {
            bool flag = false;
            ThreadContext currentThreadContext = CurrentThreadContext;
            try
            {
                currentThreadContext.m_NestedIOCount++;
                if (m_AsyncCallback != null)
                {
                    if (currentThreadContext.m_NestedIOCount >= 50)
                    {
                        ThreadPool.QueueUserWorkItem(WorkerThreadComplete);
                        flag = true;
                    }
                    else
                    {
                        m_AsyncCallback(this);
                    }
                }
            }
            finally
            {
                currentThreadContext.m_NestedIOCount--;
                if (!flag)
                {
                    Cleanup();
                }
            }
        }

        private void WorkerThreadComplete(object state)
        {
            try
            {
                m_AsyncCallback(this);
            }
            finally
            {
                Cleanup();
            }
        }

        protected virtual void Cleanup()
        {
        }

        internal object InternalWaitForCompletion()
        {
            return WaitForCompletion(snap: true);
        }

        private object WaitForCompletion(bool snap)
        {
            ManualResetEvent waitHandle = null;
            bool flag = false;
            if (!(snap ? IsCompleted : InternalPeekCompleted))
            {
                waitHandle = (ManualResetEvent)m_Event;
                if (waitHandle == null)
                {
                    flag = LazilyCreateEvent(out waitHandle);
                }
            }
            if (waitHandle != null)
            {
                try
                {
                    waitHandle.WaitOne(-1, exitContext: false);
                }
                catch (ObjectDisposedException)
                {
                }
                finally
                {
                    if (flag && !m_UserEvent)
                    {
                        ManualResetEvent manualResetEvent = (ManualResetEvent)m_Event;
                        m_Event = null;
                        if (!m_UserEvent)
                        {
                            manualResetEvent.Close();
                        }
                    }
                }
            }
            while (m_Result == DBNull.Value)
            {
                Thread.SpinWait(1);
            }
            return m_Result;
        }

        internal void InternalCleanup()
        {
            if ((m_IntCompleted & int.MaxValue) == 0 && (Interlocked.Increment(ref m_IntCompleted) & int.MaxValue) == 1)
            {
                m_Result = null;
                Cleanup();
            }
        }
    }


    #endregion 
}
