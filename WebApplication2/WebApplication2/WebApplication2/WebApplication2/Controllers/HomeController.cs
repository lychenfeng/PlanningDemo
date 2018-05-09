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

        Point v11 = new Point(0, 0);
        Point v12 = new Point(2, 0);
        Point v13 = new Point(2, 2);
        Point v14 = new Point(0, 2);
        Point v15 = new Point(3, 3);
        Point v16 = new Point(1, 1);

        //四边形顶点
        Vector2d v1 = new Vector2d(33, 117);
        Vector2d v2 = new Vector2d(-92, 23);
        Vector2d v3 = new Vector2d(-43, -46);
        Vector2d v4 = new Vector2d(79, 20);
        
      
        public Dictionary<Vector2d, PointRelativePos> dic = new Dictionary<Vector2d, PointRelativePos>();

        Point v5 = new Point(-16, -10);
        Point v6 = new Point(40,48);
        Point v7 = new Point(11,-4);
        Point v8 = new Point(-33,47);
        //Vector2d v8 = new Vector2d(-37944.700469466596, 10086.344912146214);
        //Vector2d v9 = new Vector2d(16006.413845788142, 13266.940104242221);
        public ActionResult Confirm(int type,double num)
        {

            var result = new Result();
            //num = num;

            #region Test
            //List<Vector2d> list = new List<Vector2d>();
            //list.Add(v1);
            //list.Add(v2);
            //list.Add(v3);
            //list.Add(v4);
            //Point[] points = new Point[] { v11,v12,v13,v14};
            //GraphicsPath gp = new GraphicsPath();
            //gp.AddPolygon(points);
            //bool flag1 = gp.IsVisible(v15);
            //bool flag2 = gp.IsVisible(v16);


            #endregion
            //if (type == 2)
            //{
            //    num = - num;
            //}
            List<Vector2d> list = new List<Vector2d>();
            list.Add(v1);
            list.Add(v2);
            list.Add(v3);
            list.Add(v4);
            dic = PolylineOffsetUtil.OffsetPoints(list, num);
            List<Vector2d> newList = new List<Vector2d>();
            foreach (var item in dic.Keys)
            {
                newList.Add(item);
            }
            Point[] points = new Point[4];
            for (int i = 0; i < newList.Count; i++)
            {
                int x= (int)newList[i].x;
                int y = (int)newList[i].y;
                Point p = new Point(x, y);
                points[i] = p;
            }
            GraphicsPath gp = new GraphicsPath();
            gp.AddPolygon(points);
            result.Points = points.ToList();
            result.IsOK = gp.IsVisible(v5) && gp.IsVisible(v6) && gp.IsVisible(v7) && gp.IsVisible(v8);
            //result.IsOK = PolylineOffsetUtil.PolygonContains(newList, v5) && PolylineOffsetUtil.PolygonContains(newList, v6) && PolylineOffsetUtil.PolygonContains(newList, v7);
            if (!result.IsOK)
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

            public List<Point> Points { get; set; }
            public bool IsOK { get; set; }

            public string Message { get; set; }
        }
    }
}