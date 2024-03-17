using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace 卓汇数据追溯系统
{
    public class ProductConfig2
    {

        #region 系统参数
        //CCD权限等级
        public string Name;
        public string ID;
        public string Level;
        #endregion

    }

    public class IniProductFile2
    {
        #region 初始化
        private string _path;
        private IniOperation _iniOperation;
        private ProductConfig2 _productconfig;

        #endregion

        #region Property
        public ProductConfig2 productconfig
        {
            get { return _productconfig; }
            set { _productconfig = value; }
        }

        #endregion

        public IniProductFile2(string path)
        {
            this._path = path;
            _iniOperation = new IniOperation(_path);
            _productconfig = new ProductConfig2();
            ReadProductConfigSection();
        }

        public void ReadProductConfigSection()
        {
            string sectionName = "UserLogin";
            _productconfig.Name= _iniOperation.ReadValue(sectionName, "Name");
            _productconfig.ID = _iniOperation.ReadValue(sectionName, "ID");
            _productconfig.Level = _iniOperation.ReadValue(sectionName, "Level");
        }

        public void WriteProductConfigSection()
        {
            string sectionName = "UserLogin";
            _iniOperation.WriteValue(sectionName, "Name", _productconfig.Name);
            _iniOperation.WriteValue(sectionName, "ID", _productconfig.ID);
            _iniOperation.WriteValue(sectionName, "Level", _productconfig.Level);
        }

        public void WriteProductnumSection()
        {
            string sectionName = "UserLogin";
            _iniOperation.WriteValue(sectionName, "Name", _productconfig.Name);
            _iniOperation.WriteValue(sectionName, "ID", _productconfig.ID);
            _iniOperation.WriteValue(sectionName, "Level", _productconfig.Level);
        }
    }
}
