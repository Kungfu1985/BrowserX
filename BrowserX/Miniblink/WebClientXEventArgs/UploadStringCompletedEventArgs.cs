using System;
using System.ComponentModel;

namespace MiniBlink.WebClientXEventArgs
{
    public class UploadStringCompletedEventArgs : AsyncCompletedEventArgs
    {
        private string m_Result;

        public string Result
        {
            get
            {
                RaiseExceptionIfNecessary();
                return m_Result;
            }
        }

        internal UploadStringCompletedEventArgs(string result, Exception exception, bool cancelled, object userToken)
            : base(exception, cancelled, userToken)
        {
            m_Result = result;
        }
    }

}
