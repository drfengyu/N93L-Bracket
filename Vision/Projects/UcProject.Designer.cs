namespace Vision.Projects
{
    partial class UcProject
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
            Close();
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region 组件设计器生成的代码

        /// <summary> 
        /// 设计器支持所需的方法 - 不要修改
        /// 使用代码编辑器修改此方法的内容。
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(UcProject));
            this.cmsProject = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.新增工位ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.cmsStation = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.新建工具ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.组重命名ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.删除组ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.删除全部工具ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.cmsTool = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.删除工具ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.重命名ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.上移ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.下移ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.tvProject = new System.Windows.Forms.TreeView();
            this.imageList1 = new System.Windows.Forms.ImageList(this.components);
            this.panelMain = new System.Windows.Forms.Panel();
            this.imageList2 = new System.Windows.Forms.ImageList(this.components);
            this.工位上移ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.工位下移ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.cmsProject.SuspendLayout();
            this.cmsStation.SuspendLayout();
            this.cmsTool.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.SuspendLayout();
            // 
            // cmsProject
            // 
            this.cmsProject.Font = new System.Drawing.Font("微软雅黑", 10.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.cmsProject.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.cmsProject.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.新增工位ToolStripMenuItem});
            this.cmsProject.Name = "cmsProject";
            this.cmsProject.Size = new System.Drawing.Size(153, 32);
            this.cmsProject.ItemClicked += new System.Windows.Forms.ToolStripItemClickedEventHandler(this.cmsProject_ItemClicked);
            // 
            // 新增工位ToolStripMenuItem
            // 
            this.新增工位ToolStripMenuItem.Name = "新增工位ToolStripMenuItem";
            this.新增工位ToolStripMenuItem.Size = new System.Drawing.Size(152, 28);
            this.新增工位ToolStripMenuItem.Text = "新增工位";
            // 
            // cmsStation
            // 
            this.cmsStation.Font = new System.Drawing.Font("微软雅黑", 10.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.cmsStation.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.cmsStation.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.新建工具ToolStripMenuItem,
            this.组重命名ToolStripMenuItem,
            this.工位上移ToolStripMenuItem,
            this.工位下移ToolStripMenuItem,
            this.删除组ToolStripMenuItem,
            this.删除全部工具ToolStripMenuItem});
            this.cmsStation.Name = "cmsStation";
            this.cmsStation.Size = new System.Drawing.Size(189, 172);
            this.cmsStation.ItemClicked += new System.Windows.Forms.ToolStripItemClickedEventHandler(this.cmsStation_ItemClicked);
            // 
            // 新建工具ToolStripMenuItem
            // 
            this.新建工具ToolStripMenuItem.Name = "新建工具ToolStripMenuItem";
            this.新建工具ToolStripMenuItem.Size = new System.Drawing.Size(188, 28);
            this.新建工具ToolStripMenuItem.Text = "新建工具";
            // 
            // 组重命名ToolStripMenuItem
            // 
            this.组重命名ToolStripMenuItem.Name = "组重命名ToolStripMenuItem";
            this.组重命名ToolStripMenuItem.Size = new System.Drawing.Size(188, 28);
            this.组重命名ToolStripMenuItem.Text = "工位重命名";
            // 
            // 删除组ToolStripMenuItem
            // 
            this.删除组ToolStripMenuItem.Name = "删除组ToolStripMenuItem";
            this.删除组ToolStripMenuItem.Size = new System.Drawing.Size(188, 28);
            this.删除组ToolStripMenuItem.Text = "删除工位";
            // 
            // 删除全部工具ToolStripMenuItem
            // 
            this.删除全部工具ToolStripMenuItem.Name = "删除全部工具ToolStripMenuItem";
            this.删除全部工具ToolStripMenuItem.Size = new System.Drawing.Size(188, 28);
            this.删除全部工具ToolStripMenuItem.Text = "删除全部工具";
            // 
            // cmsTool
            // 
            this.cmsTool.Font = new System.Drawing.Font("微软雅黑", 10.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.cmsTool.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.cmsTool.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.删除工具ToolStripMenuItem,
            this.重命名ToolStripMenuItem,
            this.上移ToolStripMenuItem,
            this.下移ToolStripMenuItem});
            this.cmsTool.Name = "cmsTool";
            this.cmsTool.Size = new System.Drawing.Size(153, 116);
            this.cmsTool.ItemClicked += new System.Windows.Forms.ToolStripItemClickedEventHandler(this.cmsTool_ItemClicked);
            // 
            // 删除工具ToolStripMenuItem
            // 
            this.删除工具ToolStripMenuItem.Name = "删除工具ToolStripMenuItem";
            this.删除工具ToolStripMenuItem.Size = new System.Drawing.Size(152, 28);
            this.删除工具ToolStripMenuItem.Text = "删除工具";
            // 
            // 重命名ToolStripMenuItem
            // 
            this.重命名ToolStripMenuItem.Name = "重命名ToolStripMenuItem";
            this.重命名ToolStripMenuItem.Size = new System.Drawing.Size(152, 28);
            this.重命名ToolStripMenuItem.Text = "重命名";
            // 
            // 上移ToolStripMenuItem
            // 
            this.上移ToolStripMenuItem.Name = "上移ToolStripMenuItem";
            this.上移ToolStripMenuItem.Size = new System.Drawing.Size(152, 28);
            this.上移ToolStripMenuItem.Text = "上移";
            // 
            // 下移ToolStripMenuItem
            // 
            this.下移ToolStripMenuItem.Name = "下移ToolStripMenuItem";
            this.下移ToolStripMenuItem.Size = new System.Drawing.Size(152, 28);
            this.下移ToolStripMenuItem.Text = "下移";
            // 
            // splitContainer1
            // 
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.Location = new System.Drawing.Point(0, 0);
            this.splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.tvProject);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.panelMain);
            this.splitContainer1.Size = new System.Drawing.Size(918, 654);
            this.splitContainer1.SplitterDistance = 196;
            this.splitContainer1.SplitterWidth = 8;
            this.splitContainer1.TabIndex = 3;
            // 
            // tvProject
            // 
            this.tvProject.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tvProject.Font = new System.Drawing.Font("微软雅黑", 10.8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.tvProject.ImageIndex = 2;
            this.tvProject.ImageList = this.imageList1;
            this.tvProject.Location = new System.Drawing.Point(0, 0);
            this.tvProject.Name = "tvProject";
            this.tvProject.SelectedImageIndex = 2;
            this.tvProject.Size = new System.Drawing.Size(196, 654);
            this.tvProject.TabIndex = 0;
            this.tvProject.BeforeCollapse += new System.Windows.Forms.TreeViewCancelEventHandler(this.tvProject_BeforeCollapse);
            this.tvProject.BeforeExpand += new System.Windows.Forms.TreeViewCancelEventHandler(this.tvProject_BeforeExpand);
            this.tvProject.MouseClick += new System.Windows.Forms.MouseEventHandler(this.tvProject_MouseClick);
            this.tvProject.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.tvProject_MouseDoubleClick);
            this.tvProject.MouseDown += new System.Windows.Forms.MouseEventHandler(this.tvProject_MouseDown);
            // 
            // imageList1
            // 
            this.imageList1.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imageList1.ImageStream")));
            this.imageList1.TransparentColor = System.Drawing.Color.Transparent;
            this.imageList1.Images.SetKeyName(0, "Tool.png");
            this.imageList1.Images.SetKeyName(1, "Station.png");
            this.imageList1.Images.SetKeyName(2, "Project.png");
            // 
            // panelMain
            // 
            this.panelMain.BackColor = System.Drawing.SystemColors.Window;
            this.panelMain.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panelMain.Location = new System.Drawing.Point(0, 0);
            this.panelMain.Name = "panelMain";
            this.panelMain.Size = new System.Drawing.Size(714, 654);
            this.panelMain.TabIndex = 0;
            // 
            // imageList2
            // 
            this.imageList2.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imageList2.ImageStream")));
            this.imageList2.TransparentColor = System.Drawing.Color.Transparent;
            this.imageList2.Images.SetKeyName(0, "UnStart.png");
            this.imageList2.Images.SetKeyName(1, "OK.png");
            this.imageList2.Images.SetKeyName(2, "NG.png");
            // 
            // 工位上移ToolStripMenuItem
            // 
            this.工位上移ToolStripMenuItem.Name = "工位上移ToolStripMenuItem";
            this.工位上移ToolStripMenuItem.Size = new System.Drawing.Size(188, 28);
            this.工位上移ToolStripMenuItem.Text = "工位上移";
            // 
            // 工位下移ToolStripMenuItem
            // 
            this.工位下移ToolStripMenuItem.Name = "工位下移ToolStripMenuItem";
            this.工位下移ToolStripMenuItem.Size = new System.Drawing.Size(188, 28);
            this.工位下移ToolStripMenuItem.Text = "工位下移";
            // 
            // UcProject
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.splitContainer1);
            this.Name = "UcProject";
            this.Size = new System.Drawing.Size(918, 654);
            this.Load += new System.EventHandler(this.UcProject_Load);
            this.cmsProject.ResumeLayout(false);
            this.cmsStation.ResumeLayout(false);
            this.cmsTool.ResumeLayout(false);
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.ContextMenuStrip cmsProject;
        private System.Windows.Forms.ToolStripMenuItem 新增工位ToolStripMenuItem;
        private System.Windows.Forms.ContextMenuStrip cmsStation;
        private System.Windows.Forms.ToolStripMenuItem 新建工具ToolStripMenuItem;
        private System.Windows.Forms.ContextMenuStrip cmsTool;
        private System.Windows.Forms.ToolStripMenuItem 组重命名ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem 删除全部工具ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem 删除组ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem 删除工具ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem 重命名ToolStripMenuItem;
        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.TreeView tvProject;
        private System.Windows.Forms.Panel panelMain;
        private System.Windows.Forms.ImageList imageList1;
        private System.Windows.Forms.ImageList imageList2;
        private System.Windows.Forms.ToolStripMenuItem 上移ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem 下移ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem 工位上移ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem 工位下移ToolStripMenuItem;
    }
}
