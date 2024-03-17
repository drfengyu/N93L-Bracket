namespace Vision.Frm
{
    partial class FrmToolBox
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FrmToolBox));
            this.tbDescription = new System.Windows.Forms.TextBox();
            this.tvTools = new System.Windows.Forms.TreeView();
            this.imageList1 = new System.Windows.Forms.ImageList(this.components);
            this.SuspendLayout();
            // 
            // tbDescription
            // 
            this.tbDescription.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.tbDescription.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tbDescription.Location = new System.Drawing.Point(0, 406);
            this.tbDescription.Multiline = true;
            this.tbDescription.Name = "tbDescription";
            this.tbDescription.ReadOnly = true;
            this.tbDescription.Size = new System.Drawing.Size(311, 140);
            this.tbDescription.TabIndex = 0;
            // 
            // tvTools
            // 
            this.tvTools.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tvTools.Font = new System.Drawing.Font("微软雅黑", 10.8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.tvTools.ImageIndex = 0;
            this.tvTools.ImageList = this.imageList1;
            this.tvTools.Location = new System.Drawing.Point(0, 0);
            this.tvTools.Name = "tvTools";
            this.tvTools.SelectedImageIndex = 0;
            this.tvTools.Size = new System.Drawing.Size(311, 406);
            this.tvTools.TabIndex = 1;
            this.tvTools.MouseClick += new System.Windows.Forms.MouseEventHandler(this.tvTools_MouseClick);
            this.tvTools.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.tvTools_MouseDoubleClick);
            // 
            // imageList1
            // 
            this.imageList1.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imageList1.ImageStream")));
            this.imageList1.TransparentColor = System.Drawing.Color.Transparent;
            this.imageList1.Images.SetKeyName(0, "Group.png");
            this.imageList1.Images.SetKeyName(1, "Tool.png");
            this.imageList1.Images.SetKeyName(2, "检测工具.png");
            this.imageList1.Images.SetKeyName(3, "标定工具.png");
            this.imageList1.Images.SetKeyName(4, "结果工具.png");
            this.imageList1.Images.SetKeyName(5, "9点标定.png");
            this.imageList1.Images.SetKeyName(6, "相机采集.png");
            this.imageList1.Images.SetKeyName(7, "旋转标定.png");
            this.imageList1.Images.SetKeyName(8, "图像仿真.png");
            this.imageList1.Images.SetKeyName(9, "结果编辑.png");
            this.imageList1.Images.SetKeyName(10, "Root.png");
            // 
            // FrmToolBox
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(311, 546);
            this.Controls.Add(this.tvTools);
            this.Controls.Add(this.tbDescription);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "FrmToolBox";
            this.Text = "工具箱";
            this.Load += new System.EventHandler(this.FormTools_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox tbDescription;
        private System.Windows.Forms.TreeView tvTools;
        private System.Windows.Forms.ImageList imageList1;
    }
}