using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp1
{
    class Program
    {
        static void Main(string[] args)
        {
            PointF v1 = new PointF(33, 117);
            PointF v2 = new PointF(-92, 23);
            PointF v3 = new PointF(-43, -46);
            PointF v4 = new PointF(79, 20);
            List<PointF> PointFs = new List<PointF>();
            PointFs.Add(v1);
            PointFs.Add(v2);
            PointFs.Add(v3);
            PointFs.Add(v4);

            PointF p1 = new PointF(40, 49);
            PointF p2 = new PointF(-32, 48);
            PointF p3 = new PointF(-40, 17);
            PointF p4 = new PointF(-33, -4);
            PointF p5 = new PointF(-16, -10);
            PointF p6 = new PointF(12, -4);
            PointF p7 = new PointF(25, 18);
            PointF pp5 = new PointF();
            bool flag1 = GetIntersection(PointFs[2], PointFs[3], p1, p2,ref pp5);
            bool flag2 = GetIntersection(PointFs[2], PointFs[3], p2, p3,ref pp5);
            bool flag3 = GetIntersection(PointFs[2], PointFs[3], p3, p4,ref pp5);
            bool flag4 = GetIntersection(PointFs[2], PointFs[3], p4, p5,ref pp5);
            bool flag5 = GetIntersection(PointFs[2], PointFs[3], p5, p6,ref pp5);
            bool flag6 = GetIntersection(PointFs[2], PointFs[3], p6, p7,ref pp5);
            bool flag7 = GetIntersection(PointFs[2], PointFs[3], p7, p1, ref pp5);



            PointF pp1 = new PointF(1, 1);
            PointF pp2 = new PointF(2, 2);
            PointF pp3 = new PointF(2, 1);
            PointF pp4 = new PointF(3, 0);
            
            bool f = GetIntersection(pp1, pp2, pp3, pp4,ref pp5);
            Console.ReadKey();
        }

            /// <summary>
            /// 判断两条线是否相交
            /// </summary>
            /// <param name="a">线段1起点坐标</param>
            /// <param name="b">线段1终点坐标</param>
            /// <param name="c">线段2起点坐标</param>
            /// <param name="d">线段2终点坐标</param>
            /// <param name="intersection">相交点坐标</param>
            /// <returns>是否相交 0:两线平行  -1:不平行且未相交  1:两线相交</returns>
            private static bool GetIntersection(PointF a, PointF b, PointF c, PointF d, ref PointF intersection)
            {
                intersection.X = ((b.X - a.X) * (c.X - d.X) * (c.Y - a.Y) - c.X* (b.X - a.X) * (c.Y - d.Y) + a.X* (b.Y - a.Y) * (c.X - d.X)) / ((b.Y - a.Y) * (c.X - d.X) - (b.X - a.X) * (c.Y - d.Y));
                intersection.Y = ((b.Y - a.Y) * (c.Y - d.Y) * (c.X - a.X) - c.Y* (b.Y - a.Y) * (c.X - d.X) + a.Y* (b.X - a.X) * (c.Y - d.Y)) / ((b.X - a.X) * (c.Y - d.Y) - (b.Y - a.Y) * (c.X - d.X));
                if ((intersection.X - a.X) * (intersection.X - b.X) <= 0 && (intersection.X - c.X) * (intersection.X - d.X) <= 0 && (intersection.Y - a.Y) * (intersection.Y - b.Y) <= 0 && (intersection.Y - c.Y) * (intersection.Y - d.Y) <= 0)
                {
                    return true; //'相交
                }
                else
                {
                    return false; //'相交但不在线段上
                }
            }


        static  double determinant(double v1, double v2, double v3, double v4)  // 行列式  
        {
            return (v1 * v3 - v2 * v4);
        }

        static bool Intersect(PointF aa, PointF bb, PointF cc, PointF dd)
        {
            if (aa==cc||aa==dd||bb==cc||bb==dd)
            {
                return true;
            }
            double delta = determinant(bb.X - aa.X, cc.X - dd.X, bb.Y - aa.Y, cc.Y - dd.Y);
            if (delta <= (1e-6) && delta >= -(1e-6))  // delta=0，表示两线段重合或平行  
            {
                return false;
            }
            double namenda = determinant(cc.X - aa.X, cc.X - dd.X, cc.Y - aa.Y, cc.Y - dd.Y) / delta;
            if (namenda > 1 || namenda < 0)
            {
                return false;
            }
            double miu = determinant(bb.X - aa.X, cc.X - aa.X, bb.Y - aa.Y, cc.Y - aa.Y) / delta;
            if (miu > 1 || miu < 0)
            {
                return false;
            }
            return true;
        }
    }
}
