using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Test
{
    public partial class Form1 : Form
    {
        

        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            ToolStripControlHost treeViewHost;
            ToolStripDropDown dropDown = new ToolStripDropDown();
            CheckedListBox checkList = new CheckedListBox();
            checkList.BorderStyle = BorderStyle.None;
            treeViewHost = new ToolStripControlHost(checkList);
            treeViewHost.Size = new System.Drawing.Size(1000, 1000);
            dropDown.Items.Add(treeViewHost);
            dropDown.Show(this, 100, 100); 
        }

        private void button2_Click(object sender, EventArgs e)
        {
        }

        private void button3_Click(object sender, EventArgs e)
        {
            //复选框
            CheckedListBox checkedListBox;
            checkedListBox = new CheckedListBox();
            checkedListBox.Items.AddRange(new object[] {
            "1111",
            "123456789012345678901234567890123456789012345678901234567890",
            "333333"});
            this.hsComboBox1.CtlType = HsComboBox.TypeC.CheckedListBox;
            this.hsComboBox1.SetDropDown(checkedListBox);
        }

        private void button4_Click(object sender, EventArgs e)
        {
            this.panel1.Width += 10;
        }

        private void button5_Click(object sender, EventArgs e)
        {
            this.hsComboBox1.SetDropDown(this.treeView1);
            this.treeView1.AfterSelect += delegate(object sender1, TreeViewEventArgs e1)
            {
                this.hsComboBox1.Text = this.treeView1.SelectedNode.Text + " ";
            };
        }

        private void button6_Click(object sender, EventArgs e)
        {
            this.hsComboBox1.SetDropDown(this.checkedListBox1); 
        }

        private void button8_Click(object sender, EventArgs e)
        {
            this.hsComboBox1.SetDropDown(this.panel2);
        }

        private void button7_Click(object sender, EventArgs e)
        {
            //String str = "";
            //for (int i = 0; i < this.checkedListBox1.Items.Count;i++ )
            //{
            //    str = str.Length < this.checkedListBox1.Items[i].ToString().Length ? this.checkedListBox1.Items[i].ToString() : str;
            //}
            //SizeF size = this.checkedListBox1.CreateGraphics().MeasureString(str, this.checkedListBox1.Font);
            //this.checkedListBox1.Width = Convert.ToInt32(size.Width)+20;
            this.checkedListBox1.Items.Add("aaaa");
        }

        private void treeView1_AfterSelect(object sender, TreeViewEventArgs e)
        {
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void checkedListBox1_ItemCheck(object sender, ItemCheckEventArgs e)
        {

        }

        private void button7_Paint(object sender, PaintEventArgs e)
        {

        }

        private void panel1_Paint(object sender, PaintEventArgs e)
        {
        }

        private void panel2_Paint(object sender, PaintEventArgs e)
        {
        }

        private void panel2_MouseClick(object sender, MouseEventArgs e)
        {

            this.panel1.Width += 5;

            this.panel1.Height += 5;
        }

        //**********************************

        private Point pPoint; //上个鼠标坐标
        private Point cPoint; //当前鼠标坐标

        private void panel1_MouseMove(object sender, MouseEventArgs e)
        {
            cPoint = Cursor.Position;
            if (cPoint.X > this.panel1.Left + this.panel1.Width - 2 && cPoint.Y > this.panel1.Top + this.panel1.Height - 2)
            {
                if (e.Button == MouseButtons.Left)
                {
                    int x = cPoint.X - pPoint.X;
                    int y = cPoint.Y - pPoint.Y;
                    if (this.panel1.Height + y > 10)
                    {
                        this.panel1.Height += y;
                       // this.checkedListBox1.Height += y;
                    }
                    else
                    {
                        this.panel1.Height = 10;
                    }
                    if (this.panel1.Width + x > 10)
                    {
                        this.panel1.Width += x;
                        this.checkedListBox1.Width += x;
                    }
                    else
                    {
                        this.panel1.Width = 10;
                    }
                    pPoint = Cursor.Position;
                }
                else
                {
                    this.Cursor = Cursors.SizeNWSE;
                }
            }
            else
            {
                this.Cursor = Cursors.Arrow;
            }

        }

        private void panel1_MouseDown(object sender, MouseEventArgs e)
        {
            pPoint = Cursor.Position;
        }

        private void panel1_MouseLeave(object sender, EventArgs e)
        {
            this.Cursor = Cursors.Arrow;
        }

        private void label2_Click(object sender, EventArgs e)
        {

        }

        private void button1_Click_1(object sender, EventArgs e)
        {
            this.checkedListBox1.Width++;
        }


        protected override void WndProc(ref Message m)
        {

            base.WndProc(ref m);
        }

        private void button2_Click_1(object sender, EventArgs e)
        {
        }

        private void button4_Click_1(object sender, EventArgs e)
        {
            this.hsComboBox1.SetDropDown(this.listBox1);
        }

        private void comboBox1_SelectedIndexChanged_1(object sender, EventArgs e)
        {
            //comboBox1.DataSource
        }

        private void hsComboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
        }

        private void Form1_Load(object sender, EventArgs e)
        {
        }

        void checkedListBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            throw new NotImplementedException();
        }

        private void button7_Click_1(object sender, EventArgs e)
        {
            //新用法：checkedListBox
            this.hsComboBox2.CheckedListBox.Items.Add("asdasd");
        }

        private void button9_Click(object sender, EventArgs e)
        {
            //新用法：TreeView
            this.hsComboBox3.TreeView.Nodes.Add("123123");
        }

        private void button10_Click(object sender, EventArgs e)
        {
            //新用法：TreeView
            this.hsComboBox4.Items.Add("NNNN");
        }

        private void button11_Click(object sender, EventArgs e)
        {
            this.hsComboBox3.TreeView.CheckBoxes = !this.hsComboBox3.TreeView.CheckBoxes;
        }

        private void button13_Click(object sender, EventArgs e)
        {
            //复选框
            CheckedListBox checkedListBox;
            checkedListBox = new CheckedListBox();
            checkedListBox.Items.AddRange(new object[] {
            "1111",
            "1234567890",
            "333333"});
            this.hsComboBox2.CheckedListBox = checkedListBox;
            //this.hsComboBox2.SetDropDown(checkedListBox);
        }

        
    }
}
