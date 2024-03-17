using System.ComponentModel;
using System.Windows.Forms;

namespace Vision.Projects
{
    [ToolboxItem(false)]
    public partial class UcWindowShow : UserControl
    {
        public UcWindowShow(Project data)
        {
            InitializeComponent();
            _projectData = data;
        }

        private TableLayoutPanel _myLayout = null;
        private readonly Project _projectData;

        /// <summary>
        /// 根据数目初始化panel
        /// </summary>
        /// <param name="count"></param>
        private void GetShowPanel(int count)
        {
            tlPanel1.Visible = false;
            tlPanel2.Visible = false;
            tlPanel4.Visible = false;
            tlPanel6.Visible = false;
            tlPanel8.Visible = false;
            _myLayout?.Controls.Clear();
            switch (count)
            {
                case 1:
                    tlPanel1.Visible = true;
                    _myLayout = tlPanel1;
                    break;
                case 2:
                    tlPanel2.Visible = true;
                    _myLayout = tlPanel2;
                    break;
                case 3:
                case 4:
                    tlPanel4.Visible = true;
                    _myLayout = tlPanel4;
                    break;
                case 5:
                case 6:
                    tlPanel6.Visible = true;
                    _myLayout = tlPanel6;
                    break;
                case 7:
                case 8:
                    tlPanel8.Visible = true;
                    _myLayout = tlPanel8;
                    break;
                default:
                    tlPanel1.Visible = true;
                    _myLayout = tlPanel1;
                    break;
            }
            _myLayout.BringToFront();
            _myLayout.Controls.Clear();
            _myLayout.Dock = DockStyle.Fill;
        }

        public void ShowUnit(string name = "All")
        {
            if (string.IsNullOrEmpty(name))
            {
                return;
            }
            if (name == "All")
            {
                GetShowPanel(_projectData.StationList.Count);
                for (var i = 0; i < _projectData.StationList.Count; i++)
                {
                    var station = _projectData[i];
                    if (station != null)
                    {
                        AddControl(_myLayout, station.DisplayView, i + 1);
                    }
                }
            }
            else
            {
                GetShowPanel(1);

                AddControl(_myLayout, _projectData.StationList.Find(x => x.Name == name).DisplayView, 1);
            }
        }

        /// <summary>
        /// 添加控件
        /// </summary>
        /// <param name="panel"></param>
        /// <param name="control"></param>
        /// <param name="index"></param>
        private void AddControl(TableLayoutPanel panel, UserControl control, int index)
        {
            int row, col;
            switch (index)
            {
                case 1:
                    row = 0;
                    col = 0;
                    break;
                case 2:
                    row = 0;
                    col = 1;
                    break;
                case 3:
                    if (this._projectData.StationList.Count > 4)
                    {
                        row = 0;
                        col = 2;
                    }
                    else
                    {
                        row = 1;
                        col = 0;
                    }
                    break;
                case 4:
                    if (this._projectData.StationList.Count > 4)
                    {
                        row = 0;
                        col = 3;
                    }
                    else
                    {
                        row = 1;
                        col = 1;
                    }
                    break;
                case 5:
                    if (_projectData.StationList.Count > 6)
                    {
                        row = 1;
                        col = 0;
                    }
                    else
                    {
                        row = 1;
                        col = 0;
                    }
                    break;
                case 6:
                    if (_projectData.StationList.Count > 6)
                    {
                        row = 1;
                        col = 1;
                    }
                    else
                    {
                        row = 1;
                        col = 1;
                    }
                    break;
                case 7:
                    row = 1;
                    col = 2;
                    break;
                case 8:
                    row = 1;
                    col = 3;
                    break;
                default:
                    row = 0;
                    col = 0;
                    break;
            }

            control.Dock = DockStyle.Fill;
            panel.Controls.Add(control, col, row);
        }
    }
}
