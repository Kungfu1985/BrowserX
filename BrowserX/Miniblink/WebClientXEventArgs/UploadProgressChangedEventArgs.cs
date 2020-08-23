using System.ComponentModel;

namespace MiniBlink.WebClientXEventArgs
{
    public class UploadProgressChangedEventArgs : ProgressChangedEventArgs
    {
        private long m_BytesReceived;

        private long m_TotalBytesToReceive;

        private long m_BytesSent;

        private long m_TotalBytesToSend;

        public long BytesReceived => m_BytesReceived;

        public long TotalBytesToReceive => m_TotalBytesToReceive;

        public long BytesSent => m_BytesSent;

        public long TotalBytesToSend => m_TotalBytesToSend;

        internal UploadProgressChangedEventArgs(int progressPercentage, object userToken, long bytesSent, long totalBytesToSend, long bytesReceived, long totalBytesToReceive)
            : base(progressPercentage, userToken)
        {
            m_BytesReceived = bytesReceived;
            m_TotalBytesToReceive = totalBytesToReceive;
            m_BytesSent = bytesSent;
            m_TotalBytesToSend = totalBytesToSend;
        }
    }
}
