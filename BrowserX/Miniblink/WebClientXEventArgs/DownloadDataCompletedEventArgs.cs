using System;
using System.ComponentModel;

namespace MiniBlink.WebClientXEventArgs
{
    public class DownloadDataCompletedEventArgs : AsyncCompletedEventArgs
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

        internal DownloadDataCompletedEventArgs(byte[] result, Exception exception, bool cancelled, object userToken)
            : base(exception, cancelled, userToken)
        {
            m_Result = result;
        }
    }

}
