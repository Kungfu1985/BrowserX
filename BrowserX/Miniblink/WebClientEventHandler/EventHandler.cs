using MiniBlink.WebClientXEventArgs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MiniBlink.WebClientEventHandler
{
    public delegate void OpenReadCompletedEventHandler(object sender, WebClientXEventArgs.OpenReadCompletedEventArgs e);

    public delegate void OpenWriteCompletedEventHandler(object sender, WebClientXEventArgs.OpenWriteCompletedEventArgs e);

    public delegate void DownloadStringCompletedEventHandler(object sender, WebClientXEventArgs.DownloadStringCompletedEventArgs e);

    public delegate void DownloadDataCompletedEventHandler(object sender, WebClientXEventArgs.DownloadDataCompletedEventArgs e);

    public delegate void UploadStringCompletedEventHandler(object sender, WebClientXEventArgs.UploadStringCompletedEventArgs e);

    public delegate void UploadDataCompletedEventHandler(object sender, WebClientXEventArgs.UploadDataCompletedEventArgs e);

    public delegate void UploadFileCompletedEventHandler(object sender, WebClientXEventArgs.UploadFileCompletedEventArgs e);

    public delegate void UploadValuesCompletedEventHandler(object sender, WebClientXEventArgs.UploadValuesCompletedEventArgs e);

    public delegate void DownloadProgressChangedEventHandler(object sender, WebClientXEventArgs.DownloadProgressChangedEventArgs e);

    public delegate void UploadProgressChangedEventHandler(object sender, WebClientXEventArgs.UploadProgressChangedEventArgs e);


}
