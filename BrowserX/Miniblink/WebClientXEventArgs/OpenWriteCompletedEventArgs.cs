using System;
using System.ComponentModel;
using System.IO;

namespace MiniBlink.WebClientXEventArgs
{
    public class OpenWriteCompletedEventArgs : AsyncCompletedEventArgs
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

        internal OpenWriteCompletedEventArgs(Stream result, Exception exception, bool cancelled, object userToken)
            : base(exception, cancelled, userToken)
        {
            m_Result = result;
        }
    }
}
