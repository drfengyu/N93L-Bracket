using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows.Forms;
using Cognex.VisionPro.ToolBlock;
using Vision.Core;
using Vision.Projects;
using Vision.Stations;
using Vision.Tools.Interfaces;
using Vision.Tools.ToolImpls;

namespace Vision.Tools
{
    [ToolboxItem(false)]
    public partial class UcResult : UserControl
    {
        private readonly Station _station;
        private readonly ResultTool _rTool;
        private readonly List<string> _addedToolData = new List<string>();

        public UcResult(Station station, ResultTool rTool)
        {
            InitializeComponent();
            _station = station;
            _rTool = rTool;
            ProjectManager.Instance.BeforeSaveProjectEvent += InstanceBeforeSaveProjectEvent;
        }

        /// <summary>
        /// 保存项目的前置事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void InstanceBeforeSaveProjectEvent(object sender, EventArgs e)
        {
            //强制刷新表格的值
            dgv.EndEdit();
            SetToolResult();
            //检查ToolBlock中是否已经改变
            if (!CheckToolBlockOutData())
            {
                throw new Exception("输出工具中和检测工具的输出有异，请检查！");
            }
        }

        /// <summary>
        /// 获取vpp的输出
        /// </summary>
        /// <returns></returns>
        private List<string> GetTerminals()
        {
            if (_station == null) return null;
            List<string> outputs = new List<string>();
            foreach (var item in _station.ToolList)
            {
                if (item == _rTool)
                {
                    break;
                }
                if (item is CenterDetectTool detectTool)
                {
                    var terminals = detectTool.ToolBlock.Outputs;
                    foreach (CogToolBlockTerminal t in terminals)
                    {
                        outputs.Add($"{item.Name}.{t.Name}");
                    }
                }

                if (item is DetectTool dTool)
                {
                    var terminals = dTool.ToolBlock.Outputs;
                    foreach (CogToolBlockTerminal t in terminals)
                    {
                        outputs.Add($"{item.Name}.{t.Name}");
                    }
                }
            }
            return outputs;
        }

        /// <summary>
        /// 获取所有继承IPointOut接口的集合
        /// </summary>
        /// <returns></returns>
        private List<string> GetPointOut()
        {
            if (_station == null) return null;
            List<string> outputs = new List<string>();
            foreach (var item in _station.ToolList)
            {
                if (item == _rTool) { break; }

                if (item is IPointOut outTool)
                {
                    outputs.Add($"{item.Name}.X");
                    outputs.Add($"{item.Name}.Y");
                    outputs.Add($"{item.Name}.Angle");
                }
            }
            return outputs;
        }

        /// <summary>
        /// 获取vpp的输出和IPointOut的集合
        /// </summary>
        /// <returns></returns>
        private List<string> GetToolSources()
        {
            var list = GetTerminals();
            list.AddRange(GetPointOut());
            return list;
        }

        /// <summary>
        /// 检查结果和ToolBlock输出是否匹配
        /// </summary>
        /// <returns></returns>
        private bool CheckToolBlockOutData()
        {
            List<bool> list = new List<bool>();
            var toolOuts = GetTerminals();
            //detect
            var detectTool = _station.ToolList.Find(t => t is DetectTool);
            //Centerdetect工具
            var centerDetectTool = _station.ToolList.Find(t => t is CenterDetectTool);
            List<ResultInfo> toolResList = new List<ResultInfo>();

            if (detectTool != null)
            {
                //resultTool中的detect工具结果
                toolResList = _rTool.ResultData.FindAll(r => r.Source.Split('.')[0] == detectTool.Name);
            }
            else if(centerDetectTool != null) 
            {
                //resultTool中的centerdetect工具结果
                toolResList = _rTool.ResultData.FindAll(r => r.Source.Split('.')[0] == centerDetectTool.Name);
            }
           
            //遍历上一步中的结果
            foreach (var resInfo in toolResList)
            {
                var index = toolOuts.FindIndex(t => t == resInfo.Source);
                if(index == -1)
                {
                    list.Add(false);
                }
            }
            
            if (list.Contains(false))
            {
                return false;
            }
            return true;
        }

        /// <summary>
        /// 界面初始化
        /// </summary>
        private void InitUI()
        {
            dgv.Rows.Clear();
            if (_rTool.ResultData == null)
            {
                _rTool.ResultData = new List<ResultInfo>();
            }
            else
            {
                if (_rTool.ResultData.Count > 0)
                {
                    foreach (var data in _rTool.ResultData)
                    {
                        DataGridViewRow row = new DataGridViewRow();
                        row.Cells.Add(new DataGridViewComboBoxCell()
                        {
                            DataSource = GetToolSources(),
                            Value = data.Source
                        });
                        row.Cells.Add(new DataGridViewComboBoxCell()
                        {
                            DataSource = Enum.GetNames(typeof(ResultType)),
                            Value = data.Type.ToString()
                        });
                        row.Cells.Add(new DataGridViewTextBoxCell()
                        {
                            Value = data.Address,
                        });
                        dgv.Rows.Add(row);
                    }
                }
            }
        }

        /// <summary>
        /// 将表格的值赋值给工具
        /// </summary>
        private void SetToolResult()
        {
            _rTool.ResultData.Clear();
            _addedToolData.Clear();
            foreach (DataGridViewRow row in dgv.Rows)
            {
                //表格没有数据时
                if (row.Cells[0].Value == null || row.Cells[1].Value == null || row.Cells[2].Value == null)
                {
                    continue;
                }

                var rowData = row.Cells[0].FormattedValue.ToString();
                if (_addedToolData.Contains(rowData))
                {
                    string message = "请检查输出数据，有重复数据请删除";
                    message.MsgBox();
                    throw new Exception(message);
                }
                _addedToolData.Add(row.Cells[0].Value.ToString());
                _rTool.ResultData.Add(new ResultInfo(
                    row.Cells[0].FormattedValue.ToString(),
                    (ResultType)(Enum.Parse(typeof(ResultType),
                        row.Cells[1].FormattedValue.ToString())),
                    row.Cells[2].FormattedValue.ToString()));
            }
        }

        private void UcResult_Load(object sender, System.EventArgs e)
        {
            InitUI();
        }

        private void Close()
        {
            ProjectManager.Instance.BeforeSaveProjectEvent -= InstanceBeforeSaveProjectEvent;
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            DataGridViewRow row = new DataGridViewRow();
            DataGridViewComboBoxCell cell1 = new DataGridViewComboBoxCell();
            DataGridViewComboBoxCell cell2 = new DataGridViewComboBoxCell();
            DataGridViewTextBoxCell cell3 = new DataGridViewTextBoxCell();
            cell1.Items.AddRange(GetToolSources().ToArray());
            cell2.Items.AddRange(Enum.GetNames(typeof(ResultType)).ToArray());
            row.Cells.Add(cell1);
            row.Cells.Add(cell2);
            row.Cells.Add(cell3);
            dgv.Rows.Add(row);
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            dgv.Rows.RemoveAt(0);
        }

        private void dgv_DataError(object sender, DataGridViewDataErrorEventArgs e)
        {

        }
    }
}
