using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace TM_Image_Classifier
{
    public partial class Form2 : Form
    {
        public static int nclass;//聚类个数
        public Form2()
        {
            InitializeComponent();
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            //ok
            if (textBox1.Text == "")
            {
                MessageBox.Show("The input can not be NULL", "Warning");
                this.Close();
            }
            else
            {
                nclass = int.Parse(textBox1.Text);
                this.Close();
            }


        }

        private void button2_Click(object sender, EventArgs e)
        {
            //cancel
            this.Close();
        }
    }
}
