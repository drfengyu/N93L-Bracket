using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Windows.Forms;
using Cognex.VisionPro;
using Cognex.VisionPro.Display;
using Cognex.VisionPro.ImageFile;
using Cognex.VisionPro.Implementation.Internal;
using Cognex.VisionPro.ToolBlock;
using Vision.Core;

namespace Vision.Stations
{
    [ToolboxItem(false)]
    public partial class CogDisplayView : UserControl
    {
        public CogDisplayView()
        {
            InitializeComponent();
        }

        private bool _showOne = false;

        public event EventHandler<StationShowChangedEventArgs> ShowDisplay;

        private delegate void AutoFitDelegate();

        private delegate void ClearDisplayDelegate();

        private delegate void GraphicCreateLabelDelegate(string label,double x,double y,int size,
            CogColorConstants color,CogGraphicLabelAlignmentConstants alignment,string selectedNameSpace);

        private delegate void SaveImageDelegate(string filePath,string name,ImageType imageType);

        private delegate void SetResultGraphicOnRecordDisplayDelegate(CogToolBlock toolBlock,string recordName);

        private delegate void SetTimeDelegate(TimeSpan time);

        private delegate void SetTitleDelegate(string title);

        /// <summary>
        /// 图像显示适应窗体
        /// </summary>
        public void AutoFit()
        {
            if(InvokeRequired)
            {
                Invoke(new AutoFitDelegate(AutoFit));
                return;
            }

            cogRecordDisplay1.Fit();
        }

        /// <summary>
        /// 清除显示界面
        /// </summary>
        public void ClearDisplay()
        {
            if(InvokeRequired)
            {
                Invoke(new ClearDisplayDelegate(ClearDisplay));
                return;
            }

            cogRecordDisplay1.StaticGraphics.Clear();
            cogRecordDisplay1.InteractiveGraphics.Clear();
            cogRecordDisplay1.Image = null;
        }

        /// <summary>
        /// 显示文字
        /// </summary>
        /// <param name="label"></param>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="size"></param>
        /// <param name="color"></param>
        /// <param name="alignment"></param>
        /// <param name="selectedNameSpace"></param>
        public void GraphicCreateLabel(string label,double x,double y,int size,CogColorConstants color,CogGraphicLabelAlignmentConstants alignment,string selectedNameSpace)
        {
            if(InvokeRequired)
            {
                cogRecordDisplay1.Invoke(new GraphicCreateLabelDelegate(GraphicCreateLabel),label,x,y,size,color, alignment,selectedNameSpace);
                return;
            }

            var myLabel = new CogGraphicLabel();
            var font = new Font("宋体",size,FontStyle.Regular);
            myLabel.GraphicDOFEnable = CogGraphicLabelDOFConstants.None;
            myLabel.Interactive = false;
            myLabel.Font = font;
            myLabel.Alignment = alignment;
            myLabel.Color = color;
            myLabel.SetXYText(x,y,label);
            myLabel.SelectedSpaceName = selectedNameSpace;

            cogRecordDisplay1.StaticGraphics.Add(myLabel,"");
        }

        /// <summary>
        /// 保存带图形的图像
        /// </summary>
        /// <param name="dirPath"></param>
        /// <param name="name"></param>
        /// <param name="imageType"></param>
        public void SaveScreenImage(string dirPath,string name,ImageType imageType = ImageType.JPG)
        {
            if(InvokeRequired)
            {
                Invoke(new SaveImageDelegate(SaveScreenImage),dirPath,name,imageType);
                return;
            }

            if(!Directory.Exists(dirPath)) Directory.CreateDirectory(dirPath);

            //按天进行保存
            dirPath = Path.Combine(dirPath,DateTime.Now.ToString("yy_MM_dd"));
            string file = Path.Combine(dirPath,name + "." + imageType.ToString());
            if(!Directory.Exists(dirPath))
            {
                Directory.CreateDirectory(dirPath);
            }
            try
            {
                switch(imageType)
                {
                    case ImageType.JPG:
                        cogRecordDisplay1.CreateContentBitmap(CogDisplayContentBitmapConstants.Image)
                   .Save(file,ImageFormat.Jpeg);

                        break;

                    case ImageType.BMP:

                        cogRecordDisplay1.CreateContentBitmap(CogDisplayContentBitmapConstants.Image)
                   .Save(file,ImageFormat.Bmp);
                        break;

                    default: break;
                }
            }
            catch(Exception ex)
            {
                ex.MsgBox();
            }
        }

        /// <summary>
        /// 保存原始图像
        /// </summary>
        /// <param name="dirPath"></param>
        /// <param name="name"></param>
        /// <param name="imageType"></param>
        public void SaveOriginImage(string dirPath,string name,ImageType imageType = ImageType.BMP)
        {
            if(InvokeRequired)
            {
                Invoke(new SaveImageDelegate(SaveOriginImage),dirPath,name,imageType);
                return;
            }
            try
            {
                //按天进行保存
                dirPath = Path.Combine(dirPath,DateTime.Now.ToString("yy_MM_dd"));
                string file = Path.Combine(dirPath,name + "." + imageType.ToString());

                ICogImage orignalImage = cogRecordDisplay1.Image;

                //图像不存在 直接退出
                if(orignalImage == null) return;

                if(!Directory.Exists(dirPath))
                {
                    Directory.CreateDirectory(dirPath);
                }

                CogImageFile ImageFile = new CogImageFile();
                if(!File.Exists(file))
                {
                    ImageFile.Open(file,CogImageFileModeConstants.Write);
                }
                else
                {
                    ImageFile.Open(file,CogImageFileModeConstants.Update);
                }
                ImageFile.Append(orignalImage);
                ImageFile.Close();
            }
            catch
            {
                LogNetHelper.Log("图像保存失败！");
            }
        }

        /// <summary>
        /// 显示recordDisplay
        /// </summary>
        /// <param name="toolBlock"></param>
        /// <param name="recordName"></param>
        public void SetResultGraphicOnRecordDisplay(CogToolBlock toolBlock,string recordName)
        {
            if(InvokeRequired)
            {
                cogRecordDisplay1
                    .Invoke(new SetResultGraphicOnRecordDisplayDelegate(SetResultGraphicOnRecordDisplay),
                    toolBlock,recordName);
                return;
            }
            try
            {
                toolBlock.LastRunRecordEnable = CogUserToolLastRunRecordConstants.CompositeSubToolRecords;
                var lastRecord = toolBlock.CreateLastRunRecord();
                if(cogRecordDisplay1 == null) return;
                if(lastRecord != null && lastRecord.SubRecords.ContainsKey(recordName))
                {
                    cogRecordDisplay1.Record = lastRecord.SubRecords[recordName];
                    cogRecordDisplay1.AutoFit = true;
                }
                else
                {
                    cogRecordDisplay1.Record = null;
                    LogUI.AddLog("输出图像未设置,请检查!");
                }
            }
            catch
            {
                // ignored
                LogUI.AddLog("图像显示失败！");
            }
        }

        /// <summary>
        /// 设置运行时间
        /// </summary>
        /// <param name="time"></param>
        public void SetTime(TimeSpan time)
        {
            if(InvokeRequired)
            {
                Invoke(new SetTimeDelegate(SetTime),time);
                return;
            }

            labelRunTime.Text = $"{time.TotalMilliseconds:f2} ms";
        }

        /// <summary>
        /// 设置窗口标题
        /// </summary>
        /// <param name="title"></param>
        public void SetTitle(string title)
        {
            if(InvokeRequired)
            {
                Invoke(new SetTitleDelegate(SetTitle),title);
                return;
            }

            labelTitle.Text = title;
        }

        private void CogDisplayView_Load(object sender,System.EventArgs e)
        {
            //cogDisplayStatusBarV21.Display = cogRecordDisplay1;
        }

        private void cogRecordDisplay1_DoubleClick(object sender, EventArgs e)
        {
            if (ShowDisplay != null)
            {
                _showOne = !_showOne;
                ShowDisplay.Invoke(this, new StationShowChangedEventArgs("All", _showOne));
            }
        }
    }
}