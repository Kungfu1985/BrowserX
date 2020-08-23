using System;
using System.ComponentModel;
using System.IO;

namespace MiniBlink.WebClientXEventArgs
{
    public class OpenReadCompletedEventArgs : AsyncCompletedEventArgs
    {
        private Stream m_Result;

        public Stream Result
        {
            get
            {
                RaiseExceptionIfNecessary();
                return m_Result;
            }
        }

        internal OpenReadCompletedEventArgs(Stream result, Exception exception, bool cancelled, object userToken)
            : base(exception, cancelled, userToken)
        {
            m_Result = result;
        }
    }
}
