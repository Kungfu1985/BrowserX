using System.ComponentModel;

namespace MiniBlink.WebClientXEventArgs
{
    public class DownloadProgressChangedEventArgs : ProgressChangedEventArgs
    {
        private long m_BytesReceived;

        private long m_TotalBytesToReceive;

        public long BytesReceived => m_BytesReceived;

        public long TotalBytesToReceive => m_TotalBytesToReceive;

        internal DownloadProgressChangedEventArgs(int progressPercentage, object userToken, long bytesReceived, long totalBytesToReceive)
            : base(progressPercentage, userToken)
        {
            m_BytesReceived = bytesReceived;
            m_TotalBytesToReceive = totalBytesToReceive;
        }
    }
}
