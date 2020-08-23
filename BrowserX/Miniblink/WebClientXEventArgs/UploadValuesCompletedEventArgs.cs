using System;
using System.ComponentModel;
namespace MiniBlink.WebClientXEventArgs
{
    public class UploadValuesCompletedEventArgs : AsyncCompletedEventArgs
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

        internal UploadValuesCompletedEventArgs(byte[] result, Exception exception, bool cancelled, object userToken)
            : base(exception, cancelled, userToken)
        {
            m_Result = result;
        }
    }

}
