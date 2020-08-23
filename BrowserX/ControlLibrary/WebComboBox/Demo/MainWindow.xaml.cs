using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Demo
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void btnGetText_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show(cboxWebAddressList.Text);
        }

        private void btnAddText_Click(object sender, RoutedEventArgs e)
        {
            cboxWebAddressList.Items.Add("https://www.vfb.com");
            cboxTest.Items.Add("https://www.vfb.com");
        }
    }
}
