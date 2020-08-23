using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace BroswerX.UC
{
    public partial class BrowserContainer : UserControl
    {
        

        public BrowserContainer()
        {
            InitializeComponent();

     
        }

        private void button1_Click(object sender, EventArgs e)
        {
            browserTabHeader1.AddNewTab(Guid.NewGuid().ToString(), this.Font, "www.bing.com");
        }

        private void textBox1_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == Convert.ToChar(13))
            {
                e.Handled = true;
            }
        }

       
    }
}
