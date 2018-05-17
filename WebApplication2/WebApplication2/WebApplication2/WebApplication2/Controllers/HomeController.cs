using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using polylineoffset;
using System.Drawing.Drawing2D;
using System.Drawing;

namespace WebApplication2.Controllers
{
    public class HomeController : Controller
    {
        // GET: Home
        public ActionResult Index()
        {
            return View();
        }
        //四边形顶点
        Vector2d v1 = new Vector2d(33, 117);
        Vector2d v2 = new Vector2d(-92, 23);
        Vector2d v3 = new Vector2d(-43, -46);
        Vector2d v4 = new Vector2d(79, 20);
        
      
        public Dictionary<Vector2d, PointRelativePos> dic = new Dictionary<Vector2d, PointRelativePos>();

        //Point v5 = new Point(-16, -10);
        //Point v6 = new Point(40,48);
        //Point v7 = new Point(11,-4);
        //Point v8 = new Point(-33,47);

        PointF p1 = new PointF(40,49);
        PointF p2 = new PointF(-32,48);
        PointF p3 = new PointF(-33, -4);
        PointF p4 = new PointF(-40,17);
        PointF p5 = new PointF(-16,-10);
        PointF p6 = new PointF(25,18);


        public ActionResult Confirm(int type,double num)
        {
            var result = new Result();
            //原始四边形的四个顶点
            List<Vector2d> list = new List<Vector2d>();
            list.Add(v1);
            list.Add(v2);
            list.Add(v3);
            list.Add(v4);
            //向内退界后的四边形的四个顶点
            dic = PolylineOffsetUtil.OffsetPoints(list, num);
            List<Vector2d> newList = new List<Vector2d>();
            foreach (var item in dic.Keys)
            {
                newList.Add(item);
            }
            PointF[] points = new PointF[4];
            for (int i = 0; i < newList.Count; i++)
            {
                float x = (float)newList[i].x;
                float y = (float)newList[i].y;
                PointF p = new PointF(x, y);
                points[i] = p;
            }
            result.Points = points.ToList();
            #region 原始算法，四条边同时退界，判断在不在区域范围内
            //GraphicsPath gp = new GraphicsPath();
            //gp.AddPolygon(points);

            //result.IsOK = gp.IsVisible(v5) && gp.IsVisible(v6) && gp.IsVisible(v7) && gp.IsVisible(v8);
            #endregion
            #region 改进版，根据不同的边来计算
            PointF pp = new PointF();
            switch (type)
            {
                case 1:
                    result.IsOK = GetIntersection(points[0], points[1], p1, p2,ref pp) || GetIntersection(points[0], points[1], p2, p3, ref pp) || GetIntersection(points[0], points[1], p3, p4, ref pp) || GetIntersection(points[0], points[1], p4, p5, ref pp) || GetIntersection(points[0], points[1], p5, p1, ref pp);
                    break;
                case 2:
                    result.IsOK = GetIntersection(points[1], points[2], p1, p2, ref pp) || GetIntersection(points[1], points[2], p2, p3, ref pp) || GetIntersection(points[1], points[2], p3, p4, ref pp) || GetIntersection(points[1], points[2], p4, p5, ref pp) || GetIntersection(points[1], points[2], p5, p1, ref pp);
                    break;
                case 3:
                    result.IsOK = GetIntersection(points[2], points[3], p1, p2, ref pp) || GetIntersection(points[2], points[3], p2, p3, ref pp) || GetIntersection(points[2], points[3], p3, p4, ref pp) || GetIntersection(points[2], points[3], p4, p5, ref pp) || GetIntersection(points[2], points[3], p5, p1, ref pp);
                    break;
                case 4:
                    result.IsOK = GetIntersection(points[3], points[0], p1, p2, ref pp) || GetIntersection(points[3], points[0], p2, p3, ref pp) || GetIntersection(points[3], points[0], p3, p4, ref pp) || GetIntersection(points[3], points[0], p4, p5, ref pp) || GetIntersection(points[3], points[0], p5, p1, ref pp);
                    break;
                default:
                    break;
            }
            #endregion

            if (result.IsOK)
            {
                result.Message = "建筑超出红界！";
            }
            else
            {
                result.Message = "建筑未超出红界！";
            }
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public class Result
        {

            public List<PointF> Points { get; set; }
            public bool IsOK { get; set; }

            public string Message { get; set; }
        }
        private  bool GetIntersection(PointF a, PointF b, PointF c, PointF d, ref PointF intersection)
        {
            intersection.X = ((b.X - a.X) * (c.X - d.X) * (c.Y - a.Y) - c.X * (b.X - a.X) * (c.Y - d.Y) + a.X * (b.Y - a.Y) * (c.X - d.X)) / ((b.Y - a.Y) * (c.X - d.X) - (b.X - a.X) * (c.Y - d.Y));
            intersection.Y = ((b.Y - a.Y) * (c.Y - d.Y) * (c.X - a.X) - c.Y * (b.Y - a.Y) * (c.X - d.X) + a.Y * (b.X - a.X) * (c.Y - d.Y)) / ((b.X - a.X) * (c.Y - d.Y) - (b.Y - a.Y) * (c.X - d.X));
            if ((intersection.X - a.X) * (intersection.X - b.X) <= 0 && (intersection.X - c.X) * (intersection.X - d.X) <= 0 && (intersection.Y - a.Y) * (intersection.Y - b.Y) <= 0 && (intersection.Y - c.Y) * (intersection.Y - d.Y) <= 0)
            {
                return true; //'相交
            }
            else
            {
                return false; //'相交但不在线段上
            }
        }
        private double determinant(double v1, double v2, double v3, double v4)  // 行列式  
        {
            return (v1 * v3 - v2 * v4);
        }

        /// <summary>
        /// 判断两条线段是否相交，参数分别为两条线段的顶点
        /// </summary>
        /// <param name="aa"></param>
        /// <param name="bb"></param>
        /// <param name="cc"></param>
        /// <param name="dd"></param>
        /// <returns>false--不相交，true--相交</returns>
        private bool Intersect(Point aa, Point bb, Point cc, Point dd)
        {
            if (aa == cc || aa == dd || bb == cc || bb == dd)
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