namespace Vision.Tools
{
    partial class UcCenterDetectTool
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
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabPage2 = new System.Windows.Forms.TabPage();
            this.cogToolBlockEditV21 = new Cognex.VisionPro.ToolBlock.CogToolBlockEditV2();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.comboBox1 = new System.Windows.Forms.ComboBox();
            this.tabPage3 = new System.Windows.Forms.TabPage();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.numModelX = new System.Windows.Forms.NumericUpDown();
            this.numModelY = new System.Windows.Forms.NumericUpDown();
            this.numModelA = new System.Windows.Forms.NumericUpDown();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.numRobotA = new System.Windows.Forms.NumericUpDown();
            this.numRobotY = new System.Windows.Forms.NumericUpDown();
            this.numRobotX = new System.Windows.Forms.NumericUpDown();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.tabControl1.SuspendLayout();
            this.tabPage2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.cogToolBlockEditV21)).BeginInit();
            this.tabPage1.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.tabPage3.SuspendLayout();
            this.groupBox2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numModelX)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numModelY)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numModelA)).BeginInit();
            this.groupBox3.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numRobotA)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numRobotY)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numRobotX)).BeginInit();
            this.SuspendLayout();
            // 
            // tabControl1
            // 
            this.tabControl1.Appearance = System.Windows.Forms.TabAppearance.FlatButtons;
            this.tabControl1.Controls.Add(this.tabPage2);
            this.tabControl1.Controls.Add(this.tabPage1);
            this.tabControl1.Controls.Add(this.tabPage3);
            this.tabControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabControl1.Font = new System.Drawing.Font("微软雅黑", 10.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.tabControl1.ItemSize = new System.Drawing.Size(80, 32);
            this.tabControl1.Location = new System.Drawing.Point(0, 0);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(830, 581);
            this.tabControl1.TabIndex = 0;
            // 
            // tabPage2
            // 
            this.tabPage2.Controls.Add(this.cogToolBlockEditV21);
            this.tabPage2.Location = new System.Drawing.Point(4, 36);
            this.tabPage2.Name = "tabPage2";
            this.tabPage2.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage2.Size = new System.Drawing.Size(822, 541);
            this.tabPage2.TabIndex = 1;
            this.tabPage2.Text = "视觉程序";
            this.tabPage2.UseVisualStyleBackColor = true;
            // 
            // cogToolBlockEditV21
            // 
            this.cogToolBlockEditV21.AllowDrop = true;
            this.cogToolBlockEditV21.ContextMenuCustomizer = null;
            this.cogToolBlockEditV21.Dock = System.Windows.Forms.DockStyle.Fill;
            this.cogToolBlockEditV21.Location = new System.Drawing.Point(3, 3);
            this.cogToolBlockEditV21.MinimumSize = new System.Drawing.Size(489, 0);
            this.cogToolBlockEditV21.Name = "cogToolBlockEditV21";
            this.cogToolBlockEditV21.ShowNodeToolTips = true;
            this.cogToolBlockEditV21.Size = new System.Drawing.Size(816, 535);
            this.cogToolBlockEditV21.SuspendElectricRuns = false;
            this.cogToolBlockEditV21.TabIndex = 0;
            this.cogToolBlockEditV21.Load += new System.EventHandler(this.cogToolBlockEditV21_Load);
            // 
            // tabPage1
            // 
            this.tabPage1.Controls.Add(this.groupBox1);
            this.tabPage1.Location = new System.Drawing.Point(4, 36);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage1.Size = new System.Drawing.Size(822, 541);
            this.tabPage1.TabIndex = 0;
            this.tabPage1.Text = "图像源";
            this.tabPage1.UseVisualStyleBackColor = true;
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.comboBox1);
            this.groupBox1.Dock = System.Windows.Forms.DockStyle.Top;
            this.groupBox1.Font = new System.Drawing.Font("微软雅黑", 10.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.groupBox1.Location = new System.Drawing.Point(3, 3);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(816, 130);
            this.groupBox1.TabIndex = 0;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "图像源";
            // 
            // comboBox1
            // 
            this.comboBox1.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBox1.Font = new System.Drawing.Font("微软雅黑", 10.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.comboBox1.FormattingEnabled = true;
            this.comboBox1.Location = new System.Drawing.Point(163, 53);
            this.comboBox1.Name = "comboBox1";
            this.comboBox1.Size = new System.Drawing.Size(401, 32);
            this.comboBox1.TabIndex = 0;
            this.comboBox1.SelectedIndexChanged += new System.EventHandler(this.comboBox1_SelectedIndexChanged);
            // 
            // tabPage3
            // 
            this.tabPage3.Controls.Add(this.groupBox3);
            this.tabPage3.Controls.Add(this.groupBox2);
            this.tabPage3.Font = new System.Drawing.Font("微软雅黑", 10.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.tabPage3.Location = new System.Drawing.Point(4, 36);
            this.tabPage3.Name = "tabPage3";
            this.tabPage3.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage3.Size = new System.Drawing.Size(822, 541);
            this.tabPage3.TabIndex = 2;
            this.tabPage3.Text = "模板点位";
            this.tabPage3.UseVisualStyleBackColor = true;
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.numModelA);
            this.groupBox2.Controls.Add(this.numModelY);
            this.groupBox2.Controls.Add(this.numModelX);
            this.groupBox2.Controls.Add(this.label2);
            this.groupBox2.Controls.Add(this.label1);
            this.groupBox2.Controls.Add(this.label7);
            this.groupBox2.Dock = System.Windows.Forms.DockStyle.Top;
            this.groupBox2.Font = new System.Drawing.Font("微软雅黑", 10.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.groupBox2.Location = new System.Drawing.Point(3, 3);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(816, 161);
            this.groupBox2.TabIndex = 2;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "模板点位";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(372, 69);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(58, 24);
            this.label2.TabIndex = 20;
            this.label2.Text = "angle";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(59, 40);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(19, 24);
            this.label1.TabIndex = 16;
            this.label1.Text = "x";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(59, 99);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(20, 24);
            this.label7.TabIndex = 17;
            this.label7.Text = "y";
            // 
            // numModelX
            // 
            this.numModelX.DecimalPlaces = 3;
            this.numModelX.Increment = new decimal(new int[] {
            1,
            0,
            0,
            131072});
            this.numModelX.Location = new System.Drawing.Point(134, 38);
            this.numModelX.Maximum = new decimal(new int[] {
            10000,
            0,
            0,
            0});
            this.numModelX.Minimum = new decimal(new int[] {
            10000,
            0,
            0,
            -2147483648});
            this.numModelX.Name = "numModelX";
            this.numModelX.Size = new System.Drawing.Size(138, 31);
            this.numModelX.TabIndex = 22;
            this.numModelX.ValueChanged += new System.EventHandler(this.numModelX_ValueChanged);
            // 
            // numModelY
            // 
            this.numModelY.DecimalPlaces = 3;
            this.numModelY.Increment = new decimal(new int[] {
            1,
            0,
            0,
            131072});
            this.numModelY.Location = new System.Drawing.Point(134, 99);
            this.numModelY.Maximum = new decimal(new int[] {
            10000,
            0,
            0,
            0});
            this.numModelY.Minimum = new decimal(new int[] {
            10000,
            0,
            0,
            -2147483648});
            this.numModelY.Name = "numModelY";
            this.numModelY.Size = new System.Drawing.Size(138, 31);
            this.numModelY.TabIndex = 23;
            this.numModelY.ValueChanged += new System.EventHandler(this.numModelY_ValueChanged);
            // 
            // numModelA
            // 
            this.numModelA.DecimalPlaces = 3;
            this.numModelA.Increment = new decimal(new int[] {
            1,
            0,
            0,
            131072});
            this.numModelA.Location = new System.Drawing.Point(444, 67);
            this.numModelA.Maximum = new decimal(new int[] {
            10000,
            0,
            0,
            0});
            this.numModelA.Minimum = new decimal(new int[] {
            10000,
            0,
            0,
            -2147483648});
            this.numModelA.Name = "numModelA";
            this.numModelA.Size = new System.Drawing.Size(135, 31);
            this.numModelA.TabIndex = 24;
            this.numModelA.ValueChanged += new System.EventHandler(this.numModelA_ValueChanged);
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.numRobotA);
            this.groupBox3.Controls.Add(this.numRobotY);
            this.groupBox3.Controls.Add(this.numRobotX);
            this.groupBox3.Controls.Add(this.label3);
            this.groupBox3.Controls.Add(this.label4);
            this.groupBox3.Controls.Add(this.label5);
            this.groupBox3.Dock = System.Windows.Forms.DockStyle.Top;
            this.groupBox3.Font = new System.Drawing.Font("微软雅黑", 10.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.groupBox3.Location = new System.Drawing.Point(3, 164);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(816, 161);
            this.groupBox3.TabIndex = 3;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "机械手示教点位";
            // 
            // numRobotA
            // 
            this.numRobotA.DecimalPlaces = 3;
            this.numRobotA.Increment = new decimal(new int[] {
            1,
            0,
            0,
            131072});
            this.numRobotA.Location = new System.Drawing.Point(444, 67);
            this.numRobotA.Maximum = new decimal(new int[] {
            10000,
            0,
            0,
            0});
            this.numRobotA.Minimum = new decimal(new int[] {
            10000,
            0,
            0,
            -2147483648});
            this.numRobotA.Name = "numRobotA";
            this.numRobotA.Size = new System.Drawing.Size(135, 31);
            this.numRobotA.TabIndex = 24;
            this.numRobotA.ValueChanged += new System.EventHandler(this.numRobotA_ValueChanged);
            // 
            // numRobotY
            // 
            this.numRobotY.DecimalPlaces = 3;
            this.numRobotY.Increment = new decimal(new int[] {
            1,
            0,
            0,
            131072});
            this.numRobotY.Location = new System.Drawing.Point(134, 99);
            this.numRobotY.Maximum = new decimal(new int[] {
            10000,
            0,
            0,
            0});
            this.numRobotY.Minimum = new decimal(new int[] {
            10000,
            0,
            0,
            -2147483648});
            this.numRobotY.Name = "numRobotY";
            this.numRobotY.Size = new System.Drawing.Size(138, 31);
            this.numRobotY.TabIndex = 23;
            this.numRobotY.ValueChanged += new System.EventHandler(this.numRobotY_ValueChanged);
            // 
            // numRobotX
            // 
            this.numRobotX.DecimalPlaces = 3;
            this.numRobotX.Increment = new decimal(new int[] {
            1,
            0,
            0,
            131072});
            this.numRobotX.Location = new System.Drawing.Point(134, 38);
            this.numRobotX.Maximum = new decimal(new int[] {
            10000,
            0,
            0,
            0});
            this.numRobotX.Minimum = new decimal(new int[] {
            10000,
            0,
            0,
            -2147483648});
            this.numRobotX.Name = "numRobotX";
            this.numRobotX.Size = new System.Drawing.Size(138, 31);
            this.numRobotX.TabIndex = 22;
            this.numRobotX.ValueChanged += new System.EventHandler(this.numRobotX_ValueChanged);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(372, 69);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(58, 24);
            this.label3.TabIndex = 20;
            this.label3.Text = "angle";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(59, 40);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(19, 24);
            this.label4.TabIndex = 16;
            this.label4.Text = "x";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(59, 99);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(20, 24);
            this.label5.TabIndex = 17;
            this.label5.Text = "y";
            // 
            // UcCenterDetectTool
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.tabControl1);
            this.Name = "UcCenterDetectTool";
            this.Size = new System.Drawing.Size(830, 581);
            this.Load += new System.EventHandler(this.UcDetectTool_Load);
            this.tabControl1.ResumeLayout(false);
            this.tabPage2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.cogToolBlockEditV21)).EndInit();
            this.tabPage1.ResumeLayout(false);
            this.groupBox1.ResumeLayout(false);
            this.tabPage3.ResumeLayout(false);
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numModelX)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numModelY)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numModelA)).EndInit();
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numRobotA)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numRobotY)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numRobotX)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage tabPage1;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.TabPage tabPage2;
        private Cognex.VisionPro.ToolBlock.CogToolBlockEditV2 cogToolBlockEditV21;
        private System.Windows.Forms.ComboBox comboBox1;
        private System.Windows.Forms.TabPage tabPage3;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.NumericUpDown numModelA;
        private System.Windows.Forms.NumericUpDown numModelY;
        private System.Windows.Forms.NumericUpDown numModelX;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.NumericUpDown numRobotA;
        private System.Windows.Forms.NumericUpDown numRobotY;
        private System.Windows.Forms.NumericUpDown numRobotX;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label5;
    }
}
