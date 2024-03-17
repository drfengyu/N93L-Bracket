using System;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Linq;
using Cognex.VisionPro;
using Vision.Core;
using Vision.Tools.Interfaces;

namespace Vision.Tools.ToolImpls
{
    [Serializable]
    [GroupInfo("图像工具",0)]
    [ToolName("图像仿真",1)]
    [Description("通过读取本地图像进行仿真测试")]
    public class ImageTool : ToolBase, IImageOut
    {
        /// <summary>
        /// 文件夹时使用的图像计数
        /// </summary>
        [NonSerialized]
        private int _imageIndex;

        /// <summary>
        /// 仿真的格式
        /// </summary>
        public EmulationType EmulationType { get; set; }

        /// <summary>
        /// 仿真的路径
        /// </summary>
        public string Path { get; set; }

        [field: NonSerialized]
        public ICogImage ImageOut { get; private set; }

        /// <summary>
        /// 图像显示事件
        /// </summary>
        [field: NonSerialized]
        public event EventHandler<ICogImage> ImageShowEvent;

        public ImageTool()
        {
        }

        /// <summary>
        /// 运行工具
        /// </summary>
        public override void Run()
        {
            if(!Enable) return;
            ImageOut = GetImage();
            if (ImageOut == null)
            {
                throw new ToolException($"[{Name}] 输出图像失败，请检查设置！");
            }
            OnImageShowEvent(ImageOut);
        }

        /// <summary>
        /// 图像显示触发事件
        /// </summary>
        /// <param name="image"></param>
        private void OnImageShowEvent(ICogImage image)
        {
            if(ImageShowEvent != null)
            {
                var e = ImageShowEvent;
                e.Invoke(this,image);
            }
        }

        /// <summary>
        /// 读取图像
        /// </summary>
        /// <returns></returns>
        private CogImage8Grey GetImage()
        {
            CogImage8Grey image = null;
            //如果是文件夹
            if(EmulationType == EmulationType.Dir)
            {
                if(string.IsNullOrEmpty(Path))
                {
                    return null;
                }
                string dir = Path;
                DirectoryInfo dirInfo = new DirectoryInfo(dir);
                if(dirInfo.Exists)
                {
                    var fileInfo = dirInfo.GetFiles();

                    var imageList = fileInfo.ToList()
                        .Where(x => x.Extension.ToLower() == ".bmp" /*|| x.Extension.ToLower() == ".jpg"*/)
                        .ToList()
                        .Select(x => x.FullName)
                        .ToList();

                    //没有图像
                    if(imageList.Count == 0)
                    {
                        return null;
                    }

                    if(_imageIndex >= imageList.Count)
                    {
                        _imageIndex = 0;
                    }
                    var file = imageList[_imageIndex];
                    var bmp = new Bitmap(file);
                    image = new CogImage8Grey(bmp);
                    _imageIndex++;
                }
            }
            else //单个图像文件
            {
                string file = Path;
                var bmp = new Bitmap(file);
                image = new CogImage8Grey(bmp);
            }
            return image;
        }
    }

    /// <summary>
    /// 仿真的类型
    /// </summary>
    public enum EmulationType
    {
        Dir,
        File,
    }
}