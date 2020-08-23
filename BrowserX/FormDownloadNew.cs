using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace BrowserX
{
    public partial class FormDownloadNew : Form
    {
        public FormDownloadNew()
        {
            InitializeComponent();
        }

        private void chboxAddress_TextChanged(object sender, EventArgs e)
        {
            try
            {
                //记录最近一次的选择点，最后恢复时使用
                int SelectionIndex = chboxAddress.SelectionStart;
                #region "先将所有的字符显示为DarkGray颜色"
                chboxAddress.Font = new Font("微软雅黑", 10);
                chboxAddress.SelectionStart = 0;//初始位置
                chboxAddress.SelectionLength = chboxAddress.TextLength;//
                chboxAddress.SelectionColor = Color.DarkGray;//
                #endregion
                Regex reg = new Regex(@"(\w*\.(?:com.cn|com|cn\/|edu.cn|org|net))[\^]*");//设定的需要改变颜色的固定字符串
                MatchCollection mc = reg.Matches(chboxAddress.Text, 0);//获取匹配到的顶级域名和其它字符串信息
                //逐个字符串变更颜色，这里只变更顶级域名的颜色
                //chboxAddress.HideSelection = true;
                //只处理匹配到的第一个项
                foreach (Match item in mc)
                {
                    chboxAddress.SelectionStart = item.Index;
                    chboxAddress.SelectionLength = item.Value.Length;
                    chboxAddress.SelectionColor = Color.Black;
                    break;
                }
                // chboxAddress.TextLength;//回到了文本末尾
                chboxAddress.SelectionStart = SelectionIndex; //回到最近一次的选择点
                chboxAddress.SelectionLength = 0;
                //chboxAddress.HideSelection = false;
            }
            catch
            {

            }
        }

        private void btnOk_Click(object sender, EventArgs e)
        {
            // 确定按钮点击事件        
            try
            {
                if (this.chboxAddress.Text.Length <= 0)
                {
                    MessageBox.Show("错误的下载地址，请重新输入！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    this.chboxAddress.Focus();
                    return;
                }
                if (this.tboxFileName.Text.Length <= 0)
                {
                    MessageBox.Show("错误的文件名，请重新输入！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    this.tboxFileName.Focus();
                    return;
                }
                if (this.cboxSavePath.Text.Length <= 0)
                {
                    MessageBox.Show("文件保存路径不能为空，请重新输入！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    this.cboxSavePath.Focus();
                    return;
                }
                PublicModule.bolDownFlag = true;
                PublicModule.DownloadUrl = this.chboxAddress.Text.ToString();
                PublicModule.DownSavePath = this.cboxSavePath.Text.ToString();
                PublicModule.DownSaveSize = this.tboxFileSize.Text.ToString();
                PublicModule.DownSaveFile = this.tboxFileName.Text.ToString();
                this.DialogResult = DialogResult.OK;
                this.Close();
            }
            catch (Exception ex)
            {
            }
            //do something
            //MessageBox.Show( cboxSavePath.Text );
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            //
            PublicModule.bolDownFlag = false;
            this.DialogResult = DialogResult.Cancel;
            Close();
        }

        private void FormDownloadNew_Load(object sender, EventArgs e)
        {
            try
            {
                this.cboxSavePath.Visible = true;
                this.cboxSavePath.AddItem("桌面");
                this.cboxSavePath.AddItem("F:\\Download");
                this.cboxSavePath.Text = "F:\\Download";
            }
            catch (Exception ex)
            {

            }
            try
            {
                if (PublicModule.strDownloadUrl.Length > 0)
                {
                    Uri tmpUri = new Uri(PublicModule.strDownloadUrl);
                    // 下载文件完整的URL
                    this.chboxAddress.Text = HttpUtility.UrlDecode(PublicModule.strDownloadUrl);
                    // 保存文件夹
                    this.cboxSavePath.Text = "F:\\Download";
                    // 解析下载地址中的文件名
                    if (PublicModule.strDownFileName.Length > 0)
                    {
                        this.tboxFileName.Text = PublicModule.strDownFileName;
                    }                        
                    else
                    {
                        this.tboxFileName.Text = HttpUtility.UrlDecode(tmpUri.Segments.Last());
                    }

                    // 组合下载地址，传给下载界面
                    PublicModule.strDownloadFile = string.Format("{0}://{1}{2}", HttpUtility.UrlDecode(tmpUri.Scheme), HttpUtility.UrlDecode(tmpUri.Host), HttpUtility.UrlDecode(tmpUri.AbsolutePath));
                    // 测试文件大小
                    this.tboxFileSize.Text =  PublicModule.GetWebFileSize(PublicModule.strDownloadFile);
                    if (this.tboxFileSize.Text.Contains("未知"))
                    {
                        if (PublicModule.strDownFileName.Length > 0)
                        {
                            this.tboxFileSize.Text = string.Format("大小:{0}", PublicModule.GetSize(PublicModule.strDownFileNameSize));
                        }                            
                    }
                }
            }
            catch (Exception ex)
            {
            }
        }

        private void btnSelectFolder_Click(object sender, EventArgs e)
        {
            // 添加保存路径
            try
            {
                DialogResult drResult;
                FolderBrowserDialog fbDialogLibary = new FolderBrowserDialog();

                fbDialogLibary.Description = "选择下载文件保存的路径";
                drResult = fbDialogLibary.ShowDialog();
                if (drResult == DialogResult.Yes | drResult == DialogResult.OK)
                {
                    bool bolAddFlag = false;
                    string strResult = "";
                    strResult = fbDialogLibary.SelectedPath;
                    DirectoryInfo di = new DirectoryInfo(strResult);
                    if (di.Exists)
                    {
                        // 添加到文件夹列表,/*先判断是否已经存在于列表中，如果不存在则添加*/
                        this.cboxSavePath.Text = strResult;
                        this.cboxSavePath.AddItem(strResult);
                    }
                }
                fbDialogLibary.Dispose();
                fbDialogLibary = null;
            }
            catch (Exception ex)
            {
            }
        }

        private void tboxFileName_TextChanged(object sender, EventArgs e)
        {
            try
            {
                //查找文件是否存在，如果存在则按格式 【文件名(No).扩展名】重命名
                HandleRepeatFile(null);
            }
            catch(Exception ex)
            {

            }
        }


        /// <summary>
        /// 检测保存的文件是否存在，如存在则按【文件名(编号).后缀】格式更新文件名
        /// </summary>
        /// <param name="fileInfo"></param>
        public void HandleRepeatFile(FileInfo fileInfo)
        {
            int i = 1;
            string tmpDownSaveFile = string.Empty;
            string tmpFileName = string.Empty;
            tmpDownSaveFile = string.Format("{0}\\{1}", this.cboxSavePath.Text, this.tboxFileName.Text);

            do
            {
                if (File.Exists(tmpDownSaveFile))
                {
                    //获取文件名，不包含扩展名
                    string filename = string.Empty;
                    filename = Path.GetFileNameWithoutExtension(tmpDownSaveFile);
                    //如果已经是 ”file(No).XXXX"格式
                    //如果文件名是以 ")" 结尾，则取出从左边 0 位置到右往左顺序开始的第一个"("位置之间的字符串
                    if (filename.EndsWith(")"))
                    {
                        filename = filename.Substring(0, filename.LastIndexOf("("));
                    }                        
                    //获取扩展名 格式 “.XXXX"
                    string fileExtension = Path.GetExtension(tmpDownSaveFile);
                    //组合新的文件名，格式 ”file(No).XXXX"
                    tmpFileName = string.Format("{0}({1}){2}", filename, i, fileExtension);
                    //组合成新的完整的文件路径
                    tmpDownSaveFile = string.Format("{0}\\{1}", this.cboxSavePath.Text, tmpFileName);
                }

                i += 1;
            }
            while (File.Exists(tmpDownSaveFile));

            if (tmpFileName.Length > 0)
            {
                this.tboxFileName.Text = tmpFileName;
            }
                
        }

    }
}
