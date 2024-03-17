using System;
using System.Reflection;

namespace Vision.Core
{
    /// <summary>
    /// 程序集工厂类 
    /// 创建指定的类型
    /// </summary>
    public static class AssemblyFactory
    {
        public static T CreateInstance<T>(AssemblyData refFactory,object[] parameters)
        {
            try
            {
                //toolReflection.Type.FullName, Type事实上就是类的名字及全名（完全路径+名字）
                if (refFactory.Type.FullName != null)
                {
                    object ect = refFactory.Assembly.CreateInstance(refFactory.Type.FullName,true,System.Reflection.BindingFlags.Default,null,
                        parameters/*如果该参数为空，创建该实例时就会调用默认的不带参数构造函数*/,null,null);//加载程序集，创建该程序集里面的命名空间.类型名 实例
                    return (T)ect;//类型转换并返回
                }
            }
            catch(Exception)
            {
                //发生异常，返回类型的默认值
                return default;
            }

            return default;
        }
    }

    /// <summary>
    /// 程序集数据
    /// </summary>
    public class AssemblyData
    {
        public Assembly Assembly;
        public Type Type;

        public AssemblyData(Assembly assembly,Type type)
        {
            Assembly = assembly;
            Type = type;
        }
    }
}
