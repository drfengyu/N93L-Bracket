using System;

namespace Vision.Core
{
    /// <summary>
    /// 不包含角度的点位类
    /// </summary>
    [Serializable]
    public class PointD
    {
        public double X { get; set; }
        public double Y { get; set; }

        public PointD(double x, double y)
        {
            X = x;
            Y = y;
        }

        public PointD()
        {

        }

        public override string ToString()
        {
            return $"{X},{Y}";
        }
    }

    /// <summary>
    /// 包含角度的点位类
    /// </summary>
    [Serializable]
    public class PointA
    {
        public double X { get; set; }
        public double Y { get; set; }
        public double Angle { get; set; }

        public PointA(double x, double y,double a)
        {
            X = x;
            Y = y;
            Angle = a;
        }

        public PointA()
        {

        }

        public override string ToString()
        {
            return $"x:{X.ToString("0.00")},y:{Y.ToString("0.00")},angle：{Angle.ToString("0.00")}";
        }
    }
}
