using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.IO.Compression;

namespace MiniBlink
{
    ///<summary>
    /// 压缩、解压帮助类
    /// </summary>
    public class ZipHelper
    {
        #region 单例模式
        private volatile static ZipHelper _instance = null;
        private static readonly object lockHelper = new object();//线程锁
        public static ZipHelper Instance
        {
            get
            {
                if (_instance == null)
                {
                    lock (lockHelper)
                    {
                        if (_instance == null)
                        {
                            _instance = new ZipHelper();
                        }
                    }
                }
                return _instance;
            }
        }
        #endregion 单例模式

        #region 构造函数
        public ZipHelper()
        {

        }
        #endregion 构造函数

        #region 方法
        /// <summary>
        /// 简单压缩方法
        /// </summary>
        /// <param name="filepath">压缩内容路径</param>
        /// <param name="zippath">压缩后文件保存路径</param>
        /// <returns></returns>
        public bool Compress(string filepath, string zippath)
        {
            try
            {
                if (!Directory.Exists(filepath)) {
                    return false;
                }
                CreateDirectory(zippath);
                ZipFile.CreateFromDirectory(filepath, zippath);
            }
            catch (Exception ex)
            {
                return false;
            }
            return true;
        }


        /// <summary>
        /// 简单解压方法
        /// </summary>
        /// <param name="zippath">压缩文件所在路径</param>
        /// <param name="savepath">解压后保存路径</param>
        /// <returns></returns>
        public bool DeCompress(string zippath, string savepath)
        {
            try
            {
                if (!Directory.Exists(savepath)) {
                    Directory.CreateDirectory(savepath); 
                }
                ZipFile.ExtractToDirectory(zippath, savepath);
            }
            catch (Exception ex)
            {
                System.Windows.Forms.MessageBox.Show(ex.Message);
                return false;
            }
            return true;
        }


        /// <summary>
        /// 指定目录下压缩指定类型文件
        /// </summary>
        /// <param name="filepath">指定目录</param>
        /// <param name="zippath">压缩后保存路径</param>
        /// <param name="folderName">压缩文件内部文件夹名</param>
        /// <param name="fileType">指定类型 格式如：*.dll</param>
        /// <returns></returns>
        public bool Compress(string filepath, string zippath, string folderName, string fileType)
        {
            try
            {
                IEnumerable<string> files = Directory.EnumerateFiles(filepath, fileType);
                using (ZipArchive zipArchive = ZipFile.Open(zippath, ZipArchiveMode.Create))
                {
                    foreach (string file in files)
                    {
                        var entryName = System.IO.Path.Combine(folderName, System.IO.Path.GetFileName(file));
                        zipArchive.CreateEntryFromFile(file, entryName);
                    }
                }
            }
            catch (Exception ex)
            {
                return false;
            }
            return true;
        }

        #region 调用方法
        /// <summary>
        /// 创建父级路径
        /// </summary>
        /// <param name="infoPath"></param>
        private void CreateDirectory(string infoPath)
        {
            DirectoryInfo directoryInfo = Directory.GetParent(infoPath);
            if (!directoryInfo.Exists)
            {
                directoryInfo.Create();
            }
        }

        #endregion
        #endregion 方法
    }

}
