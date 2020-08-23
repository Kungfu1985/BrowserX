using System;
using System.ComponentModel;

namespace MiniBlink.WebClientXEventArgs
{
    public class UploadDataCompletedEventArgs : AsyncCompletedEventArgs
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

        internal UploadDataCompletedEventArgs(byte[] result, Exception exception, bool cancelled, object userToken)
            : base(exception, cancelled, userToken)
        {
            m_Result = result;
        }
    }
}
