using System;
using System.Windows.Forms;

namespace Vision.Frm
{
    public partial class FrmRename : Form
    {
        public FrmRename()
        {
            InitializeComponent();
            this.StartPosition = FormStartPosition.CenterScreen;
            BringToFront();
        }

        public string NewName { get; private set; }

        public string OldName { get; set; }

        private void btnOk_Click(object sender,EventArgs e)
        {
            if(string.IsNullOrEmpty(tbNew.Text))
            {
                MessageBox.Show("名称不正确，请重新输入");
                tbNew.Clear();
                return;
            }

            if(NewName == OldName)
            {
                MessageBox.Show("名称不能和旧名称相同，请重新输入");
                tbNew.Clear();
                return;
            }

            NewName = tbNew.Text.Trim();
            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        private void FormNewName_Load(object sender,EventArgs e)
        {
            tbOld.Text = OldName;
            this.Focus();
        }

        private void btnCancel_Click(object sender,EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }
    }
}
