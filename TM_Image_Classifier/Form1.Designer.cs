namespace TM_Image_Classifier
{
    partial class Form1
    {
        /// <summary>
        /// 必需的设计器变量。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 清理所有正在使用的资源。
        /// </summary>
        /// <param name="disposing">如果应释放托管资源，为 true；否则为 false。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows 窗体设计器生成的代码

        /// <summary>
        /// 设计器支持所需的方法 - 不要修改
        /// 使用代码编辑器修改此方法的内容。
        /// </summary>
        private void InitializeComponent()
        {
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.importToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.classificationToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.supervisedClassificationToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.kmeansToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.kmeansToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.panel2 = new System.Windows.Forms.Panel();
            this.button2 = new System.Windows.Forms.Button();
            this.button1 = new System.Windows.Forms.Button();
            this.listBox1 = new System.Windows.Forms.ListBox();
            this.label3 = new System.Windows.Forms.Label();
            this.Y_position = new System.Windows.Forms.Label();
            this.X_position = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.panel1 = new System.Windows.Forms.Panel();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.iSOdataToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.menuStrip1.SuspendLayout();
            this.panel2.SuspendLayout();
            this.panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.SuspendLayout();
            // 
            // menuStrip1
            // 
            this.menuStrip1.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.importToolStripMenuItem,
            this.classificationToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(1006, 28);
            this.menuStrip1.TabIndex = 0;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // importToolStripMenuItem
            // 
            this.importToolStripMenuItem.Name = "importToolStripMenuItem";
            this.importToolStripMenuItem.Size = new System.Drawing.Size(71, 24);
            this.importToolStripMenuItem.Text = "Import";
            this.importToolStripMenuItem.Click += new System.EventHandler(this.importToolStripMenuItem_Click);
            // 
            // classificationToolStripMenuItem
            // 
            this.classificationToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.supervisedClassificationToolStripMenuItem,
            this.kmeansToolStripMenuItem});
            this.classificationToolStripMenuItem.Name = "classificationToolStripMenuItem";
            this.classificationToolStripMenuItem.Size = new System.Drawing.Size(115, 24);
            this.classificationToolStripMenuItem.Text = "Classification";
            // 
            // supervisedClassificationToolStripMenuItem
            // 
            this.supervisedClassificationToolStripMenuItem.Name = "supervisedClassificationToolStripMenuItem";
            this.supervisedClassificationToolStripMenuItem.Size = new System.Drawing.Size(277, 26);
            this.supervisedClassificationToolStripMenuItem.Text = "supervised classification";
            this.supervisedClassificationToolStripMenuItem.Click += new System.EventHandler(this.supervisedClassificationToolStripMenuItem_Click);
            // 
            // kmeansToolStripMenuItem
            // 
            this.kmeansToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.kmeansToolStripMenuItem1,
            this.iSOdataToolStripMenuItem});
            this.kmeansToolStripMenuItem.Name = "kmeansToolStripMenuItem";
            this.kmeansToolStripMenuItem.Size = new System.Drawing.Size(277, 26);
            this.kmeansToolStripMenuItem.Text = "unsupervised classification";
            // 
            // kmeansToolStripMenuItem1
            // 
            this.kmeansToolStripMenuItem1.Name = "kmeansToolStripMenuItem1";
            this.kmeansToolStripMenuItem1.Size = new System.Drawing.Size(181, 26);
            this.kmeansToolStripMenuItem1.Text = "K-means";
            this.kmeansToolStripMenuItem1.Click += new System.EventHandler(this.kmeansToolStripMenuItem1_Click);
            // 
            // panel2
            // 
            this.panel2.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.panel2.Controls.Add(this.button2);
            this.panel2.Controls.Add(this.button1);
            this.panel2.Controls.Add(this.listBox1);
            this.panel2.Controls.Add(this.label3);
            this.panel2.Controls.Add(this.Y_position);
            this.panel2.Controls.Add(this.X_position);
            this.panel2.Controls.Add(this.label2);
            this.panel2.Controls.Add(this.label1);
            this.panel2.Dock = System.Windows.Forms.DockStyle.Right;
            this.panel2.Location = new System.Drawing.Point(720, 28);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(286, 536);
            this.panel2.TabIndex = 2;
            // 
            // button2
            // 
            this.button2.Font = new System.Drawing.Font("宋体", 13.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.button2.Location = new System.Drawing.Point(25, 321);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(173, 49);
            this.button2.TabIndex = 7;
            this.button2.Text = "Rectangle";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.button2_Click);
            // 
            // button1
            // 
            this.button1.Font = new System.Drawing.Font("宋体", 13.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.button1.Location = new System.Drawing.Point(25, 410);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(137, 50);
            this.button1.TabIndex = 6;
            this.button1.Text = "Classify";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // listBox1
            // 
            this.listBox1.Font = new System.Drawing.Font("宋体", 16.2F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.listBox1.FormattingEnabled = true;
            this.listBox1.ItemHeight = 27;
            this.listBox1.Items.AddRange(new object[] {
            "Vegetation",
            "Waterbody",
            "City",
            "Soil"});
            this.listBox1.Location = new System.Drawing.Point(25, 163);
            this.listBox1.Name = "listBox1";
            this.listBox1.Size = new System.Drawing.Size(162, 139);
            this.listBox1.TabIndex = 5;
            this.listBox1.SelectedIndexChanged += new System.EventHandler(this.listBox1_SelectedIndexChanged);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("宋体", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label3.Location = new System.Drawing.Point(11, 127);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(268, 15);
            this.label3.TabIndex = 4;
            this.label3.Text = "Please Select the Sample Area";
            // 
            // Y_position
            // 
            this.Y_position.AutoSize = true;
            this.Y_position.Font = new System.Drawing.Font("宋体", 16.2F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.Y_position.Location = new System.Drawing.Point(93, 56);
            this.Y_position.Name = "Y_position";
            this.Y_position.Size = new System.Drawing.Size(26, 28);
            this.Y_position.TabIndex = 3;
            this.Y_position.Text = "0";
            // 
            // X_position
            // 
            this.X_position.AutoSize = true;
            this.X_position.Font = new System.Drawing.Font("宋体", 16.2F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.X_position.Location = new System.Drawing.Point(93, 19);
            this.X_position.Name = "X_position";
            this.X_position.Size = new System.Drawing.Size(26, 28);
            this.X_position.TabIndex = 2;
            this.X_position.Text = "0";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("宋体", 16.2F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label2.Location = new System.Drawing.Point(20, 56);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(26, 28);
            this.label2.TabIndex = 1;
            this.label2.Text = "Y";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("宋体", 16.2F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label1.Location = new System.Drawing.Point(20, 19);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(26, 28);
            this.label1.TabIndex = 0;
            this.label1.Text = "X";
            // 
            // panel1
            // 
            this.panel1.AutoScroll = true;
            this.panel1.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.panel1.Controls.Add(this.pictureBox1);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel1.Location = new System.Drawing.Point(0, 28);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(720, 536);
            this.panel1.TabIndex = 3;
            this.panel1.MouseEnter += new System.EventHandler(this.panel1_MouseEnter);
            // 
            // pictureBox1
            // 
            this.pictureBox1.Location = new System.Drawing.Point(209, 108);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(242, 236);
            this.pictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.AutoSize;
            this.pictureBox1.TabIndex = 0;
            this.pictureBox1.TabStop = false;
            this.pictureBox1.Click += new System.EventHandler(this.pictureBox1_Click);
            this.pictureBox1.MouseDown += new System.Windows.Forms.MouseEventHandler(this.pictureBox1_MouseDown);
            this.pictureBox1.MouseEnter += new System.EventHandler(this.pictureBox1_MouseEnter);
            this.pictureBox1.MouseMove += new System.Windows.Forms.MouseEventHandler(this.pictureBox1_MouseMove);
            this.pictureBox1.MouseUp += new System.Windows.Forms.MouseEventHandler(this.pictureBox1_MouseUp);
            // 
            // iSOdataToolStripMenuItem
            // 
            this.iSOdataToolStripMenuItem.Name = "iSOdataToolStripMenuItem";
            this.iSOdataToolStripMenuItem.Size = new System.Drawing.Size(181, 26);
            this.iSOdataToolStripMenuItem.Text = "ISODATA";
            this.iSOdataToolStripMenuItem.Click += new System.EventHandler(this.iSOdataToolStripMenuItem_Click);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1006, 564);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.panel2);
            this.Controls.Add(this.menuStrip1);
            this.MainMenuStrip = this.menuStrip1;
            this.Name = "Form1";
            this.Text = "TM_Image_Classifier";
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.panel2.ResumeLayout(false);
            this.panel2.PerformLayout();
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem importToolStripMenuItem;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.Label Y_position;
        private System.Windows.Forms.Label X_position;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ToolStripMenuItem classificationToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem supervisedClassificationToolStripMenuItem;
        private System.Windows.Forms.ListBox listBox1;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.ToolStripMenuItem kmeansToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem kmeansToolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem iSOdataToolStripMenuItem;
    }
}

