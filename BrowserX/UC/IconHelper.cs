using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;

namespace BrowserX.UC
{
    public class IconHelper
    {
        /// <summary>
        ///     ''' 从字节数组数据流创建ICON图标
        ///     ''' </summary>
        ///     ''' <param name="buffer"></param>
        ///     ''' <returns></returns>
        public static Icon FromByte(byte[] buffer)
        {
            try
            {
                return Icon.FromHandle(new System.Drawing.Bitmap(new MemoryStream(buffer)).GetHicon());
            }
            catch (Exception ex)
            {
                return null;
            }
        }



        /// <summary>
        ///     ''' 从本地文件数据流创建ICON图标
        ///     ''' </summary>
        ///     ''' <param name="file"></param>
        ///     ''' <returns></returns>
        public static Icon FromFile(string file)
        {
            // 判断文件是否存在，不存在直接返回Nothing
            if (!(System.IO.File.Exists(file)))
                return null/* TODO Change to default(_) if this is not a reference type */;
            if (!(file.ToUpper().EndsWith("ICO")))
                return null/* TODO Change to default(_) if this is not a reference type */;
            if (!(Path.GetExtension(file).ToUpper().Contains("ICO")))
                return null/* TODO Change to default(_) if this is not a reference type */;

            // 定义字节数组
            byte[] buffer;
            try
            {
                // 用IO打开文件流
                using (System.IO.FileStream fs = new System.IO.FileStream(file, System.IO.FileMode.Open, System.IO.FileAccess.Read))
                {
                    // 重新定义字节数组的字节长度
                    buffer = new byte[fs.Length + 1];
                    fs.Read(buffer, 0, (int)fs.Length );
                }
            }
            catch (Exception ex)
            {
                // MsgBox("geticon.fromfile got error:" & ex.Message)
                return null/* TODO Change to default(_) if this is not a reference type */;
            }
            return Icon.FromHandle(new System.Drawing.Bitmap(new MemoryStream(buffer)).GetHicon());
        }


        /// <summary>
        ///     ''' 从网络文件数据流创建ICON图标
        ///     ''' </summary>
        ///     ''' <param name="file"></param>
        ///     ''' <returns></returns>
        public static Icon FromWebFile(string file)
        {
            // 判断文件后缀，是不是ICO图标文件
            if (!(file.ToUpper().EndsWith("ICO")))
                return null;
            if (!(Path.GetExtension(file).ToUpper().Contains("ICO")))
                return null;
            // 使用Web请求，读取图片
            System.Net.WebClient wc = new System.Net.WebClient();
            Icon resIcon = null;
            // 定义一个内存流
            Image lifIconData = null;
            try
            {
                // 使用WebClinet读取网络文件
                lifIconData = System.Drawing.Image.FromStream(wc.OpenRead(file));
                if ((lifIconData.Size.IsEmpty))
                    resIcon = null;
                else
                    resIcon = Icon.FromHandle(new System.Drawing.Bitmap(lifIconData).GetHicon());
            }
            catch (Exception ex)
            {
                resIcon = null;
            }
            lifIconData.Dispose();
            lifIconData = null;
            wc.Dispose();
            wc = null;
            return (resIcon);
        }


        public static byte[] ToByte(Icon icon)
        {
            Encoder myEncoder = Encoder.Quality;
            EncoderParameter myEncoderParameter = new EncoderParameter(myEncoder, 100);
            EncoderParameters encoders = new EncoderParameters(1);
            encoders.Param[0] = myEncoderParameter;
            ImageCodecInfo myImageCodecInfo = GetEncoderInfo("image/png");

            using (MemoryStream ms = new MemoryStream())
            {
                icon.ToBitmap().Save(ms, myImageCodecInfo, encoders);
                return ms.GetBuffer();
            }
        }

        private static ImageCodecInfo GetEncoderInfo(string mimeType)
        {
            int j;
            ImageCodecInfo[] encoders = ImageCodecInfo.GetImageEncoders();

            for (j = 0; j <= encoders.Length - 1; j++)
            {
                if (encoders[j].MimeType == mimeType)
                    return encoders[j];
            }

            return null;
        }
    }
}
