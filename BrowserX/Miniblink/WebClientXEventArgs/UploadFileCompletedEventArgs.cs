using System;
using System.ComponentModel;

namespace MiniBlink.WebClientXEventArgs
{
    public class UploadFileCompletedEventArgs : AsyncCompletedEventArgs
    {
        private byte[] m_Result;

        public byte[] Result
        {
            get
            {
                RaiseExceptionIfNecessary();
                return m_Result;
            }
        }

        internal UploadFileCompletedEventArgs(byte[] result, Exception exception, bool cancelled, object userToken)
            : base(exception, cancelled, userToken)
        {
            m_Result = result;
        }
    }

}
