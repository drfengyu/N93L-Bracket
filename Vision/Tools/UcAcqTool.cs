using System.ComponentModel;
using System.Windows.Forms;
using Vision.Tools.ToolImpls;

namespace Vision.Tools
{
    [ToolboxItem(false)]
    public partial class UcAcqTool : UserControl
    {
        public UcAcqTool(ImageAcqTool imageAcqTool)
        {
            _acqTool = imageAcqTool;
            InitializeComponent();
        }

        private readonly ImageAcqTool _acqTool;

        private void UcAcqTool_Load(object sender,System.EventArgs e)
        {
            if (_acqTool != null)
            {
                cogAcqFifoEditV21.Subject = _acqTool.AcqFifoTool;
            }
        }
    }
}
