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
    public partial class Form3 : Form
    {
        public static int K_class;
        public static int least_samples;
        public static int sbs;
        public static int cds;
        public static int L;
        public static int max_iteration;
        public static int C_class;
        public Form3()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            K_class = int.Parse(textBox1.Text);
            least_samples = int.Parse(textBox2.Text);
            sbs = int.Parse(textBox3.Text);
            cds = int.Parse(textBox4.Text);
            L = int.Parse(textBox5.Text);
            max_iteration = int.Parse(textBox6.Text);
            C_class = int.Parse(textBox7.Text);
            this.Close();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            //使用默认设定
            Form1.isdefault = true;
            this.Close(); 

        }
    }
}
