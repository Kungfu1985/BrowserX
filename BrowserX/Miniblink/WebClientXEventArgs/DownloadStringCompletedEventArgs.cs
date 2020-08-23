using System;
using System.ComponentModel;

namespace MiniBlink.WebClientXEventArgs
{
    public class DownloadStringCompletedEventArgs : AsyncCompletedEventArgs
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

        internal DownloadStringCompletedEventArgs(string result, Exception exception, bool cancelled, object userToken)
            : base(exception, cancelled, userToken)
        {
            m_Result = result;
        }
    }
}
