using Emgu.CV;
using Emgu.CV.CvEnum;
using Emgu.CV.Structure;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;



namespace TM_Image_Classifier
{
    public partial class Form1 : Form
    {

        struct Band_point
        {
            public double B1;
            public double B2;
            public double B3;
            public double B4;
            public double B5;
            public double B7;
        }

        private string PicWorkPath; //工作路径
        private List<string> tif_files = new List<string>();//tif文件
        private string pic_mix_file; //合成后图片
        Image<Bgr, byte> img;
        private Point mouseDownPoint = new Point();//鼠标点下时的坐标
        private Point pic_coordinate = new Point();//鼠标相对图片坐标
        private Point pic_coordinate_rec = new Point(); //矩形框选点时的左上角点
        private Point pic_cor_rec_end = new Point();   //矩形框选点时的右下角点
        private bool isSelected;  //判断鼠标是否点下
        private bool isSelected_right; // 判断右键是否按下
        private enum ground_obj { Vegetation, Waterbody, City, Soil };
        private ground_obj obj;
        private int samplepoint_color;  //样本点颜色
        private bool isdrawsample;     //是否处于画点模式
        private bool ismousemove;
        private bool is_rectangle;      //是否处于矩形选点模式
        private List<List<Point>> sample_point = new List<List<Point>>();
        private List<Band_point[,]> all_tif_data = new List<Band_point[,]>();
        private List<List<Band_point>> selected_sample = new List<List<Band_point>>();
        private Bitmap[] pic = new Bitmap[6];  //存储tif影像六个波段的数据
        private double[,] d_cov1_opp = new double[6, 6];
        private double[,] d_cov2_opp = new double[6, 6];
        private double[,] d_cov3_opp = new double[6, 6];
        private double[,] d_cov4_opp = new double[6, 6];
        //协方差阵
        private Matrix<float>[] cov = new Matrix<float>[4];
        //均值
        private Matrix<float>[] avg = new Matrix<float>[4];

        //K-meas variable
        private int nclass = 0;
        private Band_point[] p_centers;  //聚类中心
        private List<List<Band_point>> clustered_data = new List<List<Band_point>>(); //聚类后结果
        private int iteration;

        //ISODATA variable
        private int K_class;   //所要求的的聚类中心数
        private int C_class;   //当前聚类中心数
        private int least_samples;  //一个类别至少应具有的样本数目
        private double sbs; //一个类别标准差阈值
        private double cds; //聚类中心之间距离的阈值
        private int L;  //一次迭代中可以归并的类别的最多对数
        private int max_iteration;  //最大迭代数
        public static bool isdefault;


        private static int sample_selected_count = 0;
        public Form1()
        {
            InitializeComponent();

            //设置全屏显示
            Screen[] screens = Screen.AllScreens;// 显示设备的集合
            Screen screen = screens[0];// 获取第一个显示设备

            this.Left = 0;
            this.Top = 0;
            this.Height = screen.WorkingArea.Height;//获取桌面的工作区   高度
            this.Width = screen.WorkingArea.Width;//获取桌面的工作区 宽度  

            initAttribute();

        }

        public void initAttribute()
        {
            isSelected = false;
            label1.Visible = false;
            label2.Visible = false;
            X_position.Visible = false;
            Y_position.Visible = false;
            label3.Visible = false;
            listBox1.Visible = false;

            isdrawsample = false;
            ismousemove = false;
            is_rectangle = false;

            button1.Visible = false;
            button2.Visible = false;

            for (int i = 0; i < 4; i++)
            {
                //初始化四个地物类别
                List<Point> p = new List<Point>();
                sample_point.Add(p);
            }

            sample_selected_count = 0;

            cov[0] = new Matrix<float>(6, 6);
            cov[1] = new Matrix<float>(6, 6);
            cov[2] = new Matrix<float>(6, 6);
            cov[3] = new Matrix<float>(6, 6);

            avg[0] = new Matrix<float>(6, 1);
            avg[1] = new Matrix<float>(6, 1);
            avg[2] = new Matrix<float>(6, 1);
            avg[3] = new Matrix<float>(6, 1);
        }

        public void change_status(bool x)
        {
            label1.Visible = x;
            label2.Visible = x;
            X_position.Visible = x;
            Y_position.Visible = x;

        }
        private void importToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //弹出import窗口并选择一个文件夹进行导入


            FolderBrowserDialog folder = new FolderBrowserDialog();

            if (folder.ShowDialog() == DialogResult.OK)
            {
                //读取tif文件名
                PicWorkPath = folder.SelectedPath;
                DirectoryInfo dir = new DirectoryInfo(PicWorkPath);
                //FileSystemInfo[] files = dir.GetFileSystemInfos();
                foreach (FileInfo fChild in dir.GetFiles("*.tif"))
                {
                    tif_files.Add(fChild.FullName);
                }
                tif_files.Sort();
                //读取合成后图像文件名
                FileInfo fmix = dir.GetFiles("*.jpg")[0];
                pic_mix_file = fmix.FullName;

                //显示图片
                picture_open();

            }


        }

        public void picture_open()
        {
            if (img != null)
            {
                img.Dispose();
            }
            img = new Image<Bgr, byte>(pic_mix_file);
            this.pictureBox1.Top = 10;
            this.pictureBox1.Left = 10;
            pictureBox1.Image = img.Bitmap;
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {

        }

        private void pictureBox1_MouseDown(object sender, MouseEventArgs e)
        {
            if (img == null) return;

            if (e.Button == MouseButtons.Left)
            {
                mouseDownPoint = Cursor.Position;
                pic_coordinate = pictureBox1.PointToClient(Cursor.Position);
                change_status(true);   //显示坐标控件                
                isSelected = true;
            }
            if (e.Button == MouseButtons.Right)
            {
                pic_coordinate_rec = pictureBox1.PointToClient(Cursor.Position);
                change_status(true);   //显示坐标控件   
                isSelected_right = true;
            }

        }

        private void pictureBox1_MouseMove(object sender, MouseEventArgs e)
        {
            if (img == null) return;
            change_status(true);   //显示坐标控件     
            //显示鼠标随移动坐标  
            if (isSelected == false)
            {
                //存储并显示相对于图片的坐标
                pic_coordinate = pictureBox1.PointToClient(Cursor.Position);
                X_position.Text = pic_coordinate.X.ToString();
                Y_position.Text = pic_coordinate.Y.ToString();
                ismousemove = false;
            }


            if (isSelected && IsMouseInPanel())
            {
                //相对于设备坐标系的坐标
                this.pictureBox1.Left = this.pictureBox1.Left + (Cursor.Position.X - mouseDownPoint.X);
                this.pictureBox1.Top = this.pictureBox1.Top + (Cursor.Position.Y - mouseDownPoint.Y);
                mouseDownPoint.X = Cursor.Position.X;
                mouseDownPoint.Y = Cursor.Position.Y;
                ismousemove = true;
            }
            if (is_rectangle && IsMouseInPanel() & isSelected_right)
            {
                //使用矩形框选点
                Point p_shifting = new Point();   //移动画矩形
                p_shifting.X = pictureBox1.PointToClient(Cursor.Position).X;
                p_shifting.Y = pictureBox1.PointToClient(Cursor.Position).Y;
                Graphics formGraphics = Graphics.FromImage(img.Bitmap); //画矩形
                SolidBrush[] myBrush = new SolidBrush[4];
                myBrush[0] = new SolidBrush(Color.Green);
                myBrush[1] = new SolidBrush(Color.Blue);
                myBrush[2] = new SolidBrush(Color.Red);
                myBrush[3] = new SolidBrush(Color.Yellow);
                if (samplepoint_color == 0)
                {
                    formGraphics.FillRectangle(myBrush[0], new Rectangle(pic_coordinate_rec.X, pic_coordinate_rec.Y, p_shifting.X - pic_coordinate_rec.X, p_shifting.Y - pic_coordinate_rec.Y));
                }
                if (samplepoint_color == 1)
                {
                    formGraphics.FillRectangle(myBrush[1], new Rectangle(pic_coordinate_rec.X, pic_coordinate_rec.Y, p_shifting.X - pic_coordinate_rec.X, p_shifting.Y - pic_coordinate_rec.Y));

                }
                if (samplepoint_color == 2)
                {
                    formGraphics.FillRectangle(myBrush[2], new Rectangle(pic_coordinate_rec.X, pic_coordinate_rec.Y, p_shifting.X - pic_coordinate_rec.X, p_shifting.Y - pic_coordinate_rec.Y));

                }
                if (samplepoint_color == 3)
                {
                    formGraphics.FillRectangle(myBrush[3], new Rectangle(pic_coordinate_rec.X, pic_coordinate_rec.Y, p_shifting.X - pic_coordinate_rec.X, p_shifting.Y - pic_coordinate_rec.Y));

                }

                pictureBox1.Image = img.Bitmap;


            }

        }

        private bool IsMouseInPanel()
        {
            if (this.panel1.Left < PointToClient(Cursor.Position).X && PointToClient(Cursor.Position).X < this.panel1.Left + this.panel1.Width && this.panel1.Top < PointToClient(Cursor.Position).Y && PointToClient(Cursor.Position).Y < this.panel1.Top + this.panel1.Height)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        private void panel1_MouseEnter(object sender, EventArgs e)
        {
            pictureBox1.Focus();
        }

        private void pictureBox1_MouseEnter(object sender, EventArgs e)
        {
            pictureBox1.Focus();
        }

        private void pictureBox1_MouseUp(object sender, MouseEventArgs e)
        {
            if (img == null) return;
            isSelected = false;
            isSelected_right = false;

            if (isdrawsample && ismousemove == false && is_rectangle == false)
            {
                //画并存储样本点
                Graphics formGraphics = Graphics.FromImage(img.Bitmap);
                SolidBrush[] myBrush = new SolidBrush[4];
                myBrush[0] = new SolidBrush(Color.Green);
                myBrush[1] = new SolidBrush(Color.Blue);
                myBrush[2] = new SolidBrush(Color.Red);
                myBrush[3] = new SolidBrush(Color.Yellow);
                if (samplepoint_color == 0)
                {
                    formGraphics.FillEllipse(myBrush[0], new Rectangle(pic_coordinate.X - 4, pic_coordinate.Y - 4, 8, 8));
                    sample_point[0].Add(pic_coordinate);
                }
                if (samplepoint_color == 1)
                {
                    formGraphics.FillEllipse(myBrush[1], new Rectangle(pic_coordinate.X - 4, pic_coordinate.Y - 4, 8, 8));
                    sample_point[1].Add(pic_coordinate);
                }
                if (samplepoint_color == 2)
                {
                    formGraphics.FillEllipse(myBrush[2], new Rectangle(pic_coordinate.X - 4, pic_coordinate.Y - 4, 8, 8));
                    sample_point[2].Add(pic_coordinate);
                }
                if (samplepoint_color == 3)
                {
                    formGraphics.FillEllipse(myBrush[3], new Rectangle(pic_coordinate.X - 4, pic_coordinate.Y - 4, 8, 8));
                    sample_point[3].Add(pic_coordinate);
                }
                pictureBox1.Image = img.Bitmap;
                button1.Visible = true;

                sample_selected_count++;
            }

            if (is_rectangle && isdrawsample)
            {
                //存储矩形框选的点
                pic_cor_rec_end = pictureBox1.PointToClient(Cursor.Position);

                if (samplepoint_color == 0)
                {
                    for (int i = pic_coordinate_rec.X; i < pic_cor_rec_end.X; i++)
                    {
                        for (int j = pic_coordinate_rec.Y; j < pic_cor_rec_end.Y; j++)
                        {
                            Point p = new Point();
                            p.X = i;
                            p.Y = j;
                            sample_point[0].Add(p);
                        }
                    }

                }
                if (samplepoint_color == 1)
                {
                    for (int i = pic_coordinate_rec.X; i < pic_cor_rec_end.X; i++)
                    {
                        for (int j = pic_coordinate_rec.Y; j < pic_cor_rec_end.Y; j++)
                        {
                            Point p = new Point();
                            p.X = i;
                            p.Y = j;
                            sample_point[1].Add(p);
                        }
                    }
                }
                if (samplepoint_color == 2)
                {
                    for (int i = pic_coordinate_rec.X; i < pic_cor_rec_end.X; i++)
                    {
                        for (int j = pic_coordinate_rec.Y; j < pic_cor_rec_end.Y; j++)
                        {
                            Point p = new Point();
                            p.X = i;
                            p.Y = j;
                            sample_point[2].Add(p);
                        }
                    }
                }
                if (samplepoint_color == 3)
                {
                    for (int i = pic_coordinate_rec.X; i < pic_cor_rec_end.X; i++)
                    {
                        for (int j = pic_coordinate_rec.Y; j < pic_cor_rec_end.Y; j++)
                        {
                            Point p = new Point();
                            p.X = i;
                            p.Y = j;
                            sample_point[3].Add(p);
                        }
                    }
                }
                button1.Visible = true;
            }



        }

        private void supervisedClassificationToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (img == null) return;
            //控件状态更改
            label3.Visible = true;
            listBox1.Visible = true;
            button2.Visible = true;

        }

        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            string index = listBox1.SelectedItem.ToString();
            if (index == "Vegetation")
                obj = (ground_obj)0;
            else if (index == "Waterbody")
                obj = (ground_obj)1;
            else if (index == "City")
                obj = (ground_obj)2;
            else
                obj = (ground_obj)3;
            isdrawsample = true;
            select_samplepoint();
        }
        public bool MatrixMultiply(double[,] a, double[,] b, ref double[,] c)
        {
            //矩阵相乘
            if (a.GetLength(1) != b.GetLength(0))
                return false;
            if (a.GetLength(0) != c.GetLength(0) || b.GetLength(1) != c.GetLength(1))
                return false;
            for (int i = 0; i < a.GetLength(0); i++)
            {
                for (int j = 0; j < b.GetLength(1); j++)
                {
                    c[i, j] = 0;
                    for (int k = 0; k < b.GetLength(0); k++)
                    {
                        c[i, j] += a[i, k] * b[k, j];
                    }
                }
            }

            return true;
        }
        public double MatrixSurplus(double[,] a)
        {
            //矩阵求行列式
            int i, j, k, p, r, m, n;
            m = a.GetLength(0);
            n = a.GetLength(1);
            double X, temp = 1, temp1 = 1, s = 0, s1 = 0;

            if (n == 2)
            {
                for (i = 0; i < m; i++)
                    for (j = 0; j < n; j++)
                        if ((i + j) % 2 > 0) temp1 *= a[i, j];
                        else temp *= a[i, j];
                X = temp - temp1;
            }
            else
            {
                for (k = 0; k < n; k++)
                {
                    for (i = 0, j = k; i < m && j < n; i++, j++)
                        temp *= a[i, j];
                    if (m - i > 0)
                    {
                        for (p = m - i, r = m - 1; p > 0; p--, r--)
                            temp *= a[r, p - 1];
                    }
                    s += temp;
                    temp = 1;
                }

                for (k = n - 1; k >= 0; k--)
                {
                    for (i = 0, j = k; i < m && j >= 0; i++, j--)
                        temp1 *= a[i, j];
                    if (m - i > 0)
                    {
                        for (p = m - 1, r = i; r < m; p--, r++)
                            temp1 *= a[r, p];
                    }
                    s1 += temp1;
                    temp1 = 1;
                }

                X = s - s1;
            }
            return X;
        }
        public bool MatrixInver(double[,] a, ref double[,] b)
        {
            //矩阵转置
            if (a.GetLength(0) != b.GetLength(1) || a.GetLength(1) != b.GetLength(0))
                return false;
            for (int i = 0; i < a.GetLength(1); i++)
                for (int j = 0; j < a.GetLength(0); j++)
                    b[i, j] = a[j, i];

            return true;
        }
        public bool MatrixOpp(double[,] a, ref double[,] b)
        {
            //矩阵求逆
            double X = MatrixSurplus(a);
            if (X == 0) return false;
            X = 1 / X;

            double[,] B = new double[a.GetLength(0), a.GetLength(1)];
            double[,] SP = new double[a.GetLength(0), a.GetLength(1)];
            double[,] AB = new double[a.GetLength(0), a.GetLength(1)];

            for (int i = 0; i < a.GetLength(0); i++)
                for (int j = 0; j < a.GetLength(1); j++)
                {
                    for (int m = 0; m < a.GetLength(0); m++)
                        for (int n = 0; n < a.GetLength(1); n++)
                            B[m, n] = a[m, n];
                    {
                        for (int x = 0; x < a.GetLength(1); x++)
                            B[i, x] = 0;
                        for (int y = 0; y < a.GetLength(0); y++)
                            B[y, j] = 0;
                        B[i, j] = 1;
                        SP[i, j] = MatrixSurplus(B);
                        AB[i, j] = X * SP[i, j];
                    }
                }
            MatrixInver(AB, ref b);

            return true;
        }
        public void select_samplepoint()
        {
            //选择画样本点所使用的颜色
            switch (obj)
            {
                case ground_obj.Vegetation:
                    samplepoint_color = 0;
                    break;
                case ground_obj.Waterbody:
                    samplepoint_color = 1;
                    break;
                case ground_obj.City:
                    samplepoint_color = 2;
                    break;
                case ground_obj.Soil:
                    samplepoint_color = 3;
                    break;
                default:
                    MessageBox.Show("The selected ground object doesen't exist!");
                    break;
            }
        }

        public void cov_avg_cal(int samples_count, int n_class, ref Matrix<float> cov, ref Matrix<float> avg)
        {
            //计算协方差和均值
            Matrix<float> input = new Matrix<float>(samples_count, 6);
            Matrix<float>[] input2 = new Matrix<float>[samples_count];
            float[] temp = new float[6];

            for (int i = 0; i < samples_count; i++)
            {
                input.Data[i, 0] = (float)selected_sample[n_class][i].B1;
                input.Data[i, 1] = (float)selected_sample[n_class][i].B2;
                input.Data[i, 2] = (float)selected_sample[n_class][i].B3;
                input.Data[i, 3] = (float)selected_sample[n_class][i].B4;
                input.Data[i, 4] = (float)selected_sample[n_class][i].B5;
                input.Data[i, 5] = (float)selected_sample[n_class][i].B7;
            }


            for (int j = 0; j < samples_count; j++)
            {
                for (int i = 0; i < 6; i++)
                {
                    temp[i] = input.Data[j, i];
                }
                input2[j] = new Matrix<float>(temp);
            }
            IntPtr[] imageptr = Array.ConvertAll<Matrix<float>, IntPtr>(input2, delegate (Matrix<float> mat) { return mat.Ptr; });


            CvInvoke.cvCalcCovarMatrix(imageptr, samples_count, cov, avg, COVAR_METHOD.CV_COVAR_NORMAL);
            cov = cov / (samples_count - 1);
        }
        public void sample_save()
        {
            //初始化存储样本点六个波段像素值的数组
            for (int k = 0; k < 4; k++)
            {
                List<Band_point> b = new List<Band_point>();
                selected_sample.Add(b);
            }

            //获取六张tif影像的数据
            pic[0] = new Bitmap(tif_files[0]);
            pic[1] = new Bitmap(tif_files[1]);
            pic[2] = new Bitmap(tif_files[2]);
            pic[3] = new Bitmap(tif_files[3]);
            pic[4] = new Bitmap(tif_files[4]);
            pic[5] = new Bitmap(tif_files[6]);

            int x, y;
            for (int i = 0; i < 4; i++)
            {
                for (int j = 0; j < sample_point[i].Count; j++)
                {
                    //将选取的样本点对应六个波段的像素值存储
                    x = sample_point[i][j].X;
                    y = sample_point[i][j].Y;
                    Band_point p = new Band_point();
                    p.B1 = pic[0].GetPixel(x, y).R;
                    p.B2 = pic[1].GetPixel(x, y).R;
                    p.B3 = pic[2].GetPixel(x, y).R;
                    p.B4 = pic[3].GetPixel(x, y).R;
                    p.B5 = pic[4].GetPixel(x, y).R;
                    p.B7 = pic[5].GetPixel(x, y).R;
                    selected_sample[i].Add(p);
                }
            }


        }
        public double p_cal(int n_class)
        {
            double p_w;
            double all_count = 0;
            for (int i = 0; i < selected_sample.Count; i++)
            {
                all_count += selected_sample[i].Count;
            }
            p_w = (double)selected_sample[n_class].Count / all_count;
            return p_w;
        }
        public void cov_opp()
        {
            double[,] d_cov = new double[6, 6];
            for (int n = 0; n < 4; n++)
            {
                for (int i = 0; i < 6; i++)
                {
                    for (int j = 0; j < 6; j++)
                    {
                        d_cov[i, j] = cov[n][i, j];
                    }
                }
                if (n == 0)
                    MatrixOpp(d_cov, ref d_cov1_opp);
                else if (n == 1)
                    MatrixOpp(d_cov, ref d_cov2_opp);
                else if (n == 2)
                    MatrixOpp(d_cov, ref d_cov3_opp);
                else
                    MatrixOpp(d_cov, ref d_cov4_opp);
            }


        }
        public double Discriminant(double x1, double x2, int n_class)
        {
            //计算判别函数
            double g_x;
            double[,] x_x1 = new double[1, 6];

            //cov_avg_cal(selected_sample[n_class].Count, n_class, ref cov_n, ref avg_n);
            x_x1[0, 0] = pic[0].GetPixel((int)x1, (int)x2).R - avg[n_class][0, 0];
            x_x1[0, 1] = pic[1].GetPixel((int)x1, (int)x2).R - avg[n_class][1, 0];
            x_x1[0, 2] = pic[2].GetPixel((int)x1, (int)x2).R - avg[n_class][2, 0];
            x_x1[0, 3] = pic[3].GetPixel((int)x1, (int)x2).R - avg[n_class][3, 0];
            x_x1[0, 4] = pic[4].GetPixel((int)x1, (int)x2).R - avg[n_class][4, 0];
            x_x1[0, 5] = pic[5].GetPixel((int)x1, (int)x2).R - avg[n_class][5, 0];

            double[,] x_x1_inv = new double[6, 1];
            MatrixInver(x_x1, ref x_x1_inv);
            //将协方差阵的类型由Matrix转化为double[,]
            double[,] d_cov = new double[6, 6];
            for (int i = 0; i < 6; i++)
            {
                for (int j = 0; j < 6; j++)
                {
                    d_cov[i, j] = cov[n_class][i, j];
                }
            }
            double[,] d_cov_opp = new double[6, 6];
            //MatrixOpp(d_cov, ref d_cov_opp);
            switch (n_class)
            {
                case 0:
                    d_cov_opp = d_cov1_opp;
                    break;
                case 1:
                    d_cov_opp = d_cov2_opp;
                    break;
                case 2:
                    d_cov_opp = d_cov3_opp;
                    break;
                case 3:
                    d_cov_opp = d_cov4_opp;
                    break;
                default:
                    break;
            }
            double[,] temp1 = new double[x_x1.GetLength(0), d_cov.GetLength(1)];
            MatrixMultiply(x_x1, d_cov_opp, ref temp1);
            double[,] temp2 = new double[temp1.GetLength(0), x_x1_inv.GetLength(1)];
            MatrixMultiply(temp1, x_x1_inv, ref temp2);

            double cov_det = MatrixSurplus(d_cov);
            double ln_w = Math.Log(cov_det);
            double var1 = temp2[0, 0];
            //double p_w = p_cal(n_class);
            //double ln_p_w = Math.Log(p_w);

            //判别函数
            //g_x = -1 / 2.0 * var1 - 1 / 2.0 * ln_w + ln_p_w;   //先验概率不等
            g_x = -1 / 2.0 * var1 - 1 / 2.0 * ln_w;

            return g_x;
        }
        private void button1_Click(object sender, EventArgs e)
        {
            //保存样本点
            sample_save();

            //开始训练并绘制分类结果
            Image<Bgr, byte> classified_img = new Image<Bgr, byte>(pic[0].Height, pic[0].Width);
            pictureBox1.Image = null;
            Graphics formGraphics = Graphics.FromImage(img.Bitmap);
            SolidBrush[] myBrush = new SolidBrush[4];
            myBrush[0] = new SolidBrush(Color.Green);
            myBrush[1] = new SolidBrush(Color.Blue);
            myBrush[2] = new SolidBrush(Color.Red);
            myBrush[3] = new SolidBrush(Color.Yellow);
            for (int m = 0; m < 4; m++)
            {
                cov_avg_cal(selected_sample[m].Count, m, ref cov[m], ref avg[m]);
            }
            cov_opp();
            for (int i = 0; i < pic[0].Height; i++)
            {
                for (int j = 0; j < pic[0].Width; j++)
                {
                    double max_gx_class = 0;
                    double max_gx = 0, temp;
                    max_gx = Discriminant(i, j, 0);
                    for (int k = 0; k < selected_sample.Count; k++)
                    {
                        temp = Discriminant(i, j, k);
                        if (temp >= max_gx)
                        {
                            max_gx = temp;
                            max_gx_class = k;
                        }
                    }
                    formGraphics.FillEllipse(myBrush[(int)max_gx_class], new Rectangle(i - 1, j - 1, 2, 2));

                }

            }
            pictureBox1.Image = img.ToBitmap();
            CvInvoke.cvSaveImage("result_supervise.jpg", img, img);

        }

        private void button2_Click(object sender, EventArgs e)
        {
            //使用矩形框选点
            is_rectangle = true;

        }

        //K-means
        private void kmeansToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            //重置按钮可视性
            reset_vis(false);
            //弹出选择聚类个数对话框
            numofclass_input();
            //存储TM影像六个波段的数据
            data_save();
            //聚类核心算法
            k_means();

        }

        private void k_means()
        {

            int center;
            iteration = 0;
            //初始化绘图环境

            SolidBrush[] myBrush = new SolidBrush[7];
            myBrush[0] = new SolidBrush(Color.Green);
            myBrush[1] = new SolidBrush(Color.Blue);
            myBrush[2] = new SolidBrush(Color.Red);
            myBrush[3] = new SolidBrush(Color.Yellow);
            myBrush[4] = new SolidBrush(Color.Brown);
            myBrush[5] = new SolidBrush(Color.Black);
            myBrush[6] = new SolidBrush(Color.DeepPink);
            //初始化nclass组用于存储聚类元素的列表
            for (int i = 0; i < nclass; i++)
            {
                List<Band_point> p = new List<Band_point>();
                clustered_data.Add(p);
            }
            //选择初始聚类中心
            init_cluster_center(nclass);
            Band_point[] p_temp_centers = new Band_point[nclass];
            p_temp_centers = (Band_point[])p_centers.Clone();
            double[,,] test = new double[pic[0].Height, pic[0].Width, 6];
            for (int i = 0; i < pic[0].Height; i++)
            {
                for (int j = 0; j < pic[0].Width; j++)
                {
                    for (int k = 0; k < 6; k++)
                    {
                        test[i, j, k] = pic[k].GetPixel(i, j).R;
                    }
                }
            }
            do
            {
                //初始化结果专题图
                Image<Bgr, byte> classified_img = new Image<Bgr, byte>(pic[0].Height, pic[0].Width);
                pictureBox1.Image = null;
                Graphics formGraphics = Graphics.FromImage(img.Bitmap);
                //将更新后的聚类中心赋给初始数组
                p_centers = (Band_point[])p_temp_centers.Clone();
                for (int i = 0; i < nclass; i++)
                {
                    clustered_data[i].Clear();
                }
                Band_point p = new Band_point();

                //计算各点到聚类中心的欧式距离并进行聚类
                for (int i = 0; i < pic[0].Height; i++)
                {
                    for (int j = 0; j < pic[0].Width; j++)
                    {


                        double[] ndis = new double[nclass];
                        distance_cal(i, j, ref ndis, test,nclass); // 计算一点到各个聚类中心的距离
                        center = point_classify(i, j, ndis,nclass); //  找出距离最小的并将该点进行分类

                        p.B1 = test[i, j, 0];
                        p.B2 = test[i, j, 1];
                        p.B3 = test[i, j, 2];
                        p.B4 = test[i, j, 3];
                        p.B5 = test[i, j, 4];
                        p.B7 = test[i, j, 5];
                        clustered_data[center].Add(p);
                        //绘制该点
                        formGraphics.FillEllipse(myBrush[center], new Rectangle(i - 1, j - 1, 2, 2));
                    }
                }
                //将临时数组设为0
                for (int m = 0; m < nclass; m++)
                {
                    p_temp_centers[m].B1 = 0;
                    p_temp_centers[m].B2 = 0;
                    p_temp_centers[m].B3 = 0;
                    p_temp_centers[m].B4 = 0;
                    p_temp_centers[m].B5 = 0;
                    p_temp_centers[m].B7 = 0;

                }
                //建立新的聚类中心
                new_center_cal(ref p_temp_centers,nclass);
                iteration++;
            }
            while (iscenter_change(p_temp_centers));  //以新旧聚类中心是否相等为循环条件

            pictureBox1.Image = img.ToBitmap();
            CvInvoke.cvSaveImage("result_k_means2.jpg", img, img);

        }

        private bool iscenter_change(Band_point[] p_temp_centers)
        {
            for (int n = 0; n < nclass; n++)
            {
                if (p_centers[n].B1 - p_temp_centers[n].B1 > 1 || p_centers[n].B2 - p_temp_centers[n].B2 > 1 || p_centers[n].B3 - p_temp_centers[n].B3 > 1 || p_centers[n].B4 - p_temp_centers[n].B4 > 1 || p_centers[n].B5 - p_temp_centers[n].B5 > 1 || p_centers[n].B7 - p_temp_centers[n].B7 > 1)
                {
                    return true;
                }
            }
            return false;
        }

        private void new_center_cal(ref Band_point[] p_temp_centers, int nc)
        {
            //建立新的聚类中心
            for (int n = 0; n < nc; n++)
            {
                int ncount = clustered_data[n].Count; //记录各个聚类的元素个数
                for (int i = 0; i < ncount; i++)
                {
                    //计算每个聚类的均指向量
                    p_temp_centers[n].B1 += clustered_data[n][i].B1;
                    p_temp_centers[n].B2 += clustered_data[n][i].B2;
                    p_temp_centers[n].B3 += clustered_data[n][i].B3;
                    p_temp_centers[n].B4 += clustered_data[n][i].B4;
                    p_temp_centers[n].B5 += clustered_data[n][i].B5;
                    p_temp_centers[n].B7 += clustered_data[n][i].B7;
                }
                p_temp_centers[n].B1 = p_temp_centers[n].B1 / (double)ncount;
                p_temp_centers[n].B2 = p_temp_centers[n].B2 / (double)ncount;
                p_temp_centers[n].B3 = p_temp_centers[n].B3 / (double)ncount;
                p_temp_centers[n].B4 = p_temp_centers[n].B4 / (double)ncount;
                p_temp_centers[n].B5 = p_temp_centers[n].B5 / (double)ncount;
                p_temp_centers[n].B7 = p_temp_centers[n].B7 / (double)ncount;

            }
        }

        private int point_classify(int i, int j, double[] ndis, int nb)
        {
            double min_dis = ndis[0];
            int center = 0;
            for (int n = 0; n < nb; n++)
            {
                //找出到该点距离最小的聚类中心
                if (ndis[n] <= min_dis)
                {
                    min_dis = ndis[n];
                    center = n;
                }
            }
            return center;

        }

        private void distance_cal(int i, int j, ref double[] ndis, double[,,] test,int nc)
        {
            //计算点（i，j）到各聚类中心的距离
            for (int n = 0; n < nc; n++)
            {
                ndis[n] = Math.Pow(test[i, j, 0] - p_centers[n].B1, 2) + Math.Pow(test[i, j, 1] - p_centers[n].B2, 2) + Math.Pow(test[i, j, 2] - p_centers[n].B3, 2) + Math.Pow(test[i, j, 3] - p_centers[n].B4, 2) + Math.Pow(test[i, j, 4] - p_centers[n].B5, 2) + Math.Pow(test[i, j, 5] - p_centers[n].B7, 2);

            }
        }



        private void init_cluster_center(int n)
        {
            //初始化聚类中心
            p_centers = new Band_point[n];
            for (int i = 0; i < n; i++)
            {

                p_centers[i].B1 = pic[0].GetPixel(0, i).R;
                p_centers[i].B2 = pic[1].GetPixel(0, i).R;
                p_centers[i].B3 = pic[2].GetPixel(0, i).R;
                p_centers[i].B4 = pic[3].GetPixel(0, i).R;
                p_centers[i].B5 = pic[4].GetPixel(0, i).R;
                p_centers[i].B7 = pic[5].GetPixel(0, i).R;

            }
        }

        private void reset_vis(bool v)
        {
            listBox1.Visible = v;
            button1.Visible = v;
            button2.Visible = v;
            label3.Visible = v;
        }

        private void numofclass_input()
        {
            //弹出输入聚类个数对话框
            Form2 form = new Form2();
            form.ShowDialog();
            nclass = Form2.nclass;
        }

        private void data_save()
        {
            pic[0] = new Bitmap(tif_files[0]);
            pic[1] = new Bitmap(tif_files[1]);
            pic[2] = new Bitmap(tif_files[2]);
            pic[3] = new Bitmap(tif_files[3]);
            pic[4] = new Bitmap(tif_files[4]);
            pic[5] = new Bitmap(tif_files[6]);

        }

        private void iSOdataToolStripMenuItem_Click(object sender, EventArgs e)
        {
            isdefault = false;
            //重置按钮可视性
            reset_vis(false);
            //存储TM影像六个波段的数据
            data_save();
            //初始参数设定
            
            set_coe();
            if (isdefault == true)
            {
                default_coe();
            }
            //ISODATA 核心算法 
            ISODATA();
        }

        private void set_coe()
        {
            Form3 form3 = new Form3();
            form3.ShowDialog();
            if(isdefault)
            {
                return;
            }
            K_class = Form3.K_class;
            least_samples = Form3.least_samples;
            sbs = Form3.sbs;
            cds = Form3.cds;
            L = Form3.L;
            max_iteration = Form3.max_iteration;
            C_class = Form3.C_class;
        }

        private void default_coe()
        {
            K_class = 4;
            least_samples = 100000;
            sbs = 15;
            cds = 10;
            L = 1;
            max_iteration = 20;
            C_class = 1;
        }

        private void ISODATA()
        {
            
            int cur_iteration = 1;
            bool islastiteration = false;
            //初始化绘图环境
            SolidBrush[] myBrush = new SolidBrush[7];
            myBrush[0] = new SolidBrush(Color.Green);
            myBrush[1] = new SolidBrush(Color.Blue);
            myBrush[2] = new SolidBrush(Color.Red);
            myBrush[3] = new SolidBrush(Color.Yellow);
            myBrush[4] = new SolidBrush(Color.Brown);
            myBrush[5] = new SolidBrush(Color.Black);
            myBrush[6] = new SolidBrush(Color.DeepPink);
            //初始化nclass组用于存储聚类元素的列表
            for (int i = 0; i < K_class; i++)
            {
                List<Band_point> p = new List<Band_point>();
                clustered_data.Add(p);
            }
            //选择初始聚类中心
            init_cluster_center(K_class);
            //初始化存储图像的数组
            Band_point[] p_temp_centers = new Band_point[C_class];
            p_temp_centers = (Band_point[])p_centers.Clone();
            double[,,] test = new double[pic[0].Height, pic[0].Width, 6];
            for (int i = 0; i < pic[0].Height; i++)
            {
                for (int j = 0; j < pic[0].Width; j++)
                {
                    for (int k = 0; k < 6; k++)
                    {
                        test[i, j, k] = pic[k].GetPixel(i, j).R;
                    }
                }
            }

            bool issplit = false;

            while (cur_iteration <= max_iteration)//设置循环条件
            {
                //将n个样本按最小距离分类
                mindis_classify(p_temp_centers, test,myBrush);
                //step 4

                //修正聚类中心值
                new_center_cal(ref p_temp_centers, C_class);
                //绘图
                pictureBox1.Image = img.ToBitmap();
                //计算样本到聚类中心距离
                double d_all_avg = 0;
                double[] d_sin_avg = new double[C_class];
                for(int i = 0; i < C_class; i++)
                {
                    d_sin_avg[i] = 0;
                }
                sampleToCenter(ref d_sin_avg,ref d_all_avg,p_temp_centers,test);
                //判断迭代次数是否达到上限
                if(cur_iteration == max_iteration)
                {
                    islastiteration = true;
                    break;
                }
                //判断聚类中心数目是否达到规定数目二分之一
                if(C_class <= (double)K_class / 2.0 || (C_class % 2 != 0 && C_class < (double)K_class ) )
                {
                    //对现有类别进行分裂
                    double[,] standard_bias = new double[C_class,6];
                    for(int i = 0; i < 6; i++)
                    {
                        for(int j = 0; j < C_class; j++)
                        {
                            standard_bias[j, i] = 0;
                        }
                    }
                    class_division(test, p_temp_centers, ref standard_bias);
                    //对每一标准差向量求其最大分量
                    double[] max_stb = new double[C_class];
                    double[] max_stb_index = new double[C_class];
                    max_standardbias(standard_bias,ref max_stb,ref max_stb_index);
                    //判断分裂的聚类
                    for(int i = 0; i < C_class; i++)
                    {
                        if(max_stb[i] > sbs && ((d_sin_avg[i] > d_all_avg  && clustered_data[i].Count > 2*(least_samples + 1) || C_class <= K_class)))
                        {
                            //进行分裂
                            split(ref p_temp_centers,max_stb,i, max_stb_index);
                            break;
                        }
                    }
                    C_class++;
                    issplit = true;
                }
                if(issplit == false)
                {
                    //合并

                }


                cur_iteration++;
            }
            pictureBox1.Image = img.ToBitmap();
            CvInvoke.cvSaveImage("result_ISODATA.jpg", img, img);

        }

        private void split(ref Band_point[] p_temp_centers, double[] max_stb, int i,double[] max_stb_index)
        {
            //将原聚类进行分类
            
            double s = max_stb_index[i];
            double max_st = max_stb[0];
            int index = 0;
            for(int m = 0; m < C_class;m++)
            {
                if (max_stb[m] > max_st)
                {
                    max_st = max_stb[m];
                    index = m;
                }
            }
            p_temp_centers[C_class] = p_temp_centers[index];
            if (s == 0)
            {
                p_temp_centers[index].B1 += max_st;
                p_temp_centers[C_class].B1 = p_temp_centers[index].B1 - max_st;
            }
            if (s == 1)
            {
                p_temp_centers[index].B2 += max_st;
                p_temp_centers[C_class].B2 = p_temp_centers[index].B2 - max_st;
            }
            if (s == 2)
            {
                p_temp_centers[index].B3 += max_st;
                p_temp_centers[C_class].B3 = p_temp_centers[index].B3 - max_st;
            }
            if (s == 4)
            {
                p_temp_centers[index].B4 += max_st;
                p_temp_centers[C_class].B4 = p_temp_centers[index].B4 - max_st;
            }
            if (s == 5)
            {
                p_temp_centers[index].B5 += max_st;
                p_temp_centers[C_class].B5 = p_temp_centers[index].B5 - max_st;
            }
            if (s == 6)
            {
                p_temp_centers[index].B7 += max_st;
                p_temp_centers[C_class].B7 = p_temp_centers[index].B7 - max_st;
            }



        }

        private void max_standardbias(double[,] standard_bias, ref double[] max_stb,ref double[] max_stb_index)
        {
            for(int j = 0; j < C_class; j++)
            {
                max_stb[j] = standard_bias[j,0];
                for(int i = 0; i < 6; i++)
                {
                    if (standard_bias[j, i] >= max_stb[j])
                    {
                        max_stb[j] = standard_bias[j, i];
                        max_stb_index[j] = i;
                    }
                }
            }
        }

        private void class_division(double[,,] test, Band_point[] p_temp_centers, ref double[,] standard_bias)
        {
            for(int i = 0; i < 6; i++)
            {
                for(int j = 0; j < C_class; j++)
                {
                    for(int k = 0; k < clustered_data[j].Count; k++)
                    {
                        if (i == 0)
                            standard_bias[j, i] += Math.Pow(clustered_data[j][k].B1 - p_temp_centers[j].B1, 2);
                        if (i == 1)
                            standard_bias[j, i] += Math.Pow(clustered_data[j][k].B2 - p_temp_centers[j].B2, 2);
                        if (i == 2)
                            standard_bias[j, i] += Math.Pow(clustered_data[j][k].B3 - p_temp_centers[j].B3, 2);
                        if (i == 3)
                            standard_bias[j, i] += Math.Pow(clustered_data[j][k].B4 - p_temp_centers[j].B4, 2);
                        if (i == 4)
                            standard_bias[j, i] += Math.Pow(clustered_data[j][k].B5 - p_temp_centers[j].B5, 2);
                        if (i == 5)
                            standard_bias[j, i] += Math.Pow(clustered_data[j][k].B7 - p_temp_centers[j].B7, 2);

                    }
                    standard_bias[j, i] /= clustered_data[j].Count;
                    standard_bias[j, i] = Math.Sqrt(standard_bias[j, i]);
                }
            }
        }

        private void sampleToCenter(ref double[] d_sin_avg, ref double d_all_avg, Band_point[] p_temp_centers,double[,,] test)
        {
            //计算样本到聚类中心距离
            int all_count = 0;
            for (int i = 0; i < C_class;i++)
            {
                for (int j = 0; j < clustered_data[i].Count; j++)
                {
                    d_sin_avg[i] += Math.Pow(clustered_data[i][j].B1 - p_centers[i].B1, 2) + Math.Pow(clustered_data[i][j].B2 - p_centers[i].B2, 2) + Math.Pow(clustered_data[i][j].B3 - p_centers[i].B3, 2) + Math.Pow(clustered_data[i][j].B4 - p_centers[i].B4, 2) + Math.Pow(clustered_data[i][j].B5 - p_centers[i].B5, 2) + Math.Pow(clustered_data[i][j].B7 - p_centers[i].B7, 2);
                }
                d_sin_avg[i] /= clustered_data[i].Count;
            }
            for(int i = 0; i < C_class; i++)
            {
                d_all_avg += clustered_data[i].Count * d_sin_avg[i];
                all_count += clustered_data[i].Count;
            }
            d_all_avg /= all_count;
            
            
        }

        private void mindis_classify(Band_point[] p_temp_centers, double[,,] test, SolidBrush[] myBrush)
        {
            
            int center;
            //将更新后的聚类中心赋给初始数组
            p_centers = (Band_point[])p_temp_centers.Clone();
            for (int i = 0; i < C_class; i++)
            {
                clustered_data[i].Clear();
            }
            Band_point p = new Band_point();
            Image<Bgr, byte> classified_img = new Image<Bgr, byte>(pic[0].Height, pic[0].Width);
            pictureBox1.Image = null;
            Graphics formGraphics = Graphics.FromImage(img.Bitmap);
            //计算各点到聚类中心的欧式距离并进行聚类
            for (int i = 0; i < pic[0].Height; i++)
            {
                for (int j = 0; j < pic[0].Width; j++)
                {


                    double[] ndis = new double[C_class];
                    distance_cal(i, j, ref ndis, test,C_class); // 计算一点到各个聚类中心的距离
                    center = point_classify(i, j, ndis,C_class); //  找出距离最小的并将该点进行分类

                    p.B1 = test[i, j, 0];
                    p.B2 = test[i, j, 1];
                    p.B3 = test[i, j, 2];
                    p.B4 = test[i, j, 3];
                    p.B5 = test[i, j, 4];
                    p.B7 = test[i, j, 5];
                    clustered_data[center].Add(p);
                    formGraphics.FillEllipse(myBrush[center], new Rectangle(i - 1, j - 1, 2, 2));
                }
            }
            
        }
    }
}
