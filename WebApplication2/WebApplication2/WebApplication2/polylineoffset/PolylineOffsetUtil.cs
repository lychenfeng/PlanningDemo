using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Quadrant = System.Int32;

namespace polylineoffset
{
    public class Line
    {
        public Line()
        {
        }

        public Line(Vector3d startPt, Vector3d endPt)
        {
            _startPoint = startPt;
            _endPoint = endPt;
        }

        private Vector3d _startPoint;
        private Vector3d _endPoint;

        public Vector3d StartPoint
        {
            get { return _startPoint; }
        }

        public Vector3d EndPoint
        {
            get { return _endPoint; }
        }

        public const double LINE_MINIZE_LENGTH = 0.0001;
    }
    
    
    /// <summary>
    /// Point position relative to region 
    /// </summary>
    /// <owner>
    /// Triaphoo Fu
    /// </owner>
    public enum PointRelativePos
    {
        Outer = 0,
        Inner = 1,
        Unknown,
    }

    public class PolylineOffsetUtil
    {
        public static bool IsAlmostEqual(double d1, double d2, double eps = 1e-6)
        {
            return Math.Abs(d1 - d2) < eps;
        }

        public static bool IsAlmostZero(double d1, double eps = 1e-6)
        {
            return IsAlmostEqual(d1, 0.0, eps);
        }

        static public Vector2d Flatten(Vector3d point)
        {
            return new Vector2d(point.x, point.y);
        }

        static public List<Vector2d> Flatten(IList<Vector3d> polygon)
        {
            List<Vector2d> a = new List<Vector2d>(polygon.Count);
            foreach (Vector3d p in polygon)
            {
                a.Add(Flatten(p));
            }

            return a;
        }

        static public Vector3d Inflatten(Vector2d point)
        {
            return new Vector3d(point.x, point.y, 0.0);
        }

        static public List<Vector3d> Inflatten(IList<Vector2d> polygon)
        {
            List<Vector3d> a = new List<Vector3d>(polygon.Count);
            foreach (Vector2d p in polygon)
            {
                a.Add(Inflatten(p));
            }

            return a;
        }

        /// <summary>
        /// Add of two points(or vectors), get a new point(vector) 
        /// </summary>
        /// <param name="p1">the first point(vector)</param>
        /// <param name="p2">the first point(vector)</param>
        /// <returns>a new vector(point)</returns>
        public static Vector3d AddXYZ(Vector3d p1, Vector3d p2)
        {
            double x = p1.x + p2.x;
            double y = p1.y + p2.y;
            double z = p1.z + p2.z;

            return new Vector3d(x, y, z);
        }

        /// <summary>
        /// Subtraction of two points(or vectors), get a new vector 
        /// </summary>
        /// <param name="p1">the first point(vector)</param>
        /// <param name="p2">the second point(vector)</param>
        /// <returns>return a new vector from point p2 to p1</returns>
        public static Vector3d SubXYZ(Vector3d p1, Vector3d p2)
        {
            double x = p1.x - p2.x;
            double y = p1.y - p2.y;
            double z = p1.z - p2.z;

            return new Vector3d(x, y, z);
        }

        /// <summary>
        /// Set the vector into unit length
        /// </summary>
        /// <param name="vector">the input vector</param>
        /// <returns>the vector in unit length</returns>
        public static Vector3d UnitVector(Vector3d vector)
        {
            // calculate the distance from grid origin to the XYZ
            double length = vector.Length;
            if (length == 1e-9)
            {
                return new Vector3d(0, 0, 0);
            }

            // changed the vector into the unit length
            double x = vector.x / length;
            double y = vector.y / length;
            double z = vector.z / length;

            return new Vector3d(x, y, z);
        }

        /// <summary>
        /// Multiply a vector with a number
        /// </summary>
        /// <param name="vector">a vector</param>
        /// <param name="rate">the rate number</param>
        /// <returns></returns>
        public static Vector3d MultiplyVector(Vector3d vector, double rate)
        {
            double x = vector.x * rate;
            double y = vector.y * rate;
            double z = vector.z * rate;

            return new Vector3d(x, y, z);
        }

        #region Point in Polygon

        /// <summary>
        /// Determine whether given 2D point lies within 
        /// the polygon. If the point lies in the curve of the polygon, the return is false. 
        /// 
        /// Written by Jeremy Tammik, Autodesk, 2009-09-23, 
        /// based on code that I wrote back in 1996 in C++, 
        /// which in turn was based on C code from the 
        /// article "An Incremental Angle Point in Polygon 
        /// Test" by Kevin Weiler, Autodesk, in "Graphics 
        /// Gems IV", Academic Press, 1994.
        /// 
        /// Copyright (C) 2009 by Jeremy Tammik. All 
        /// rights reserved.
        /// 
        /// This code may be freely used. Please preserve 
        /// this comment.
        /// </summary>
        public static bool PolygonContains(IList<Vector2d> polygon, Vector2d point)
        {
            // initialize
            Quadrant quad = GetQuadrant(polygon[0], point);

            Quadrant angle = 0;

            // loop on all vertices of polygon
            Quadrant next_quad, delta;
            int n = polygon.Count;
            for (int i = 0; i < n; ++i)
            {
                Vector2d vertex = polygon[i];

                Vector2d next_vertex = polygon[(i + 1 < n) ? i + 1 : 0];

                // calculate quadrant and delta from last quadrant

                next_quad = GetQuadrant(next_vertex, point);
                delta = next_quad - quad;

                AdjustDelta(ref delta, vertex, next_vertex, point);

                // add delta to total angle sum
                angle = angle + delta;

                // increment for next step
                quad = next_quad;
            }

            // complete 360 degrees (angle of + 4 or -4 ) 
            // means inside

            return (angle == +4) || (angle == -4);

            // odd number of windings rule:
            // if (angle & 4) return INSIDE; else return OUTSIDE;
            // non-zero winding rule:
            // if (angle != 0) return INSIDE; else return OUTSIDE;
        }

        // http://thebuildingcoder.typepad.com/blog/2008/12/polygon-transformation.html
        // 
        public static bool PolygonContains(IList<Vector3d> polygon, Vector2d point)
        {
            Quadrant quad = GetQuadrant(new Vector2d(polygon[0].x, polygon[0].y), point);

            Quadrant angle = 0;

            // loop on all vertices of polygon
            Quadrant next_quad, delta;
            int n = polygon.Count;
            for (int i = 0; i < n; ++i)
            {
                Vector2d vertex = new Vector2d(polygon[i].x, polygon[i].y);

                Vector3d temp = polygon[(i + 1 < n) ? i + 1 : 0];
                Vector2d next_vertex = new Vector2d(temp.x, temp.y);

                // calculate quadrant and delta from last quadrant
                next_quad = GetQuadrant(next_vertex, point);
                delta = next_quad - quad;

                AdjustDelta(ref delta, vertex, next_vertex, point);

                // add delta to total angle sum
                angle = angle + delta;

                // increment for next step
                quad = next_quad;
            }

            // complete 360 degrees (angle of + 4 or -4 ) 
            // means inside

            return (angle == +4) || (angle == -4);

            // odd number of windings rule:
            // if (angle & 4) return INSIDE; else return OUTSIDE;
            // non-zero winding rule:
            // if (angle != 0) return INSIDE; else return OUTSIDE;
        }

        /// <summary>
        /// Determine the quadrant of a polygon vertex 
        /// relative to the test point.
        /// </summary>
        private static Quadrant GetQuadrant(Vector2d vertex, Vector2d p)
        {
            return (vertex.x > p.x) ? ((vertex.y > p.y) ? 0 : 3) : ((vertex.y > p.y) ? 1 : 2);
        }

        /// <summary>
        /// Determine the X intercept of a polygon edge 
        /// with a horizontal line at the Y value of the 
        /// test point.
        /// </summary>
        private static double X_intercept(Vector2d p, Vector2d q, double y)
        {
            return q.x - ((q.y - y) * ((p.x - q.x) / (p.y - q.y)));
        }

        private static void AdjustDelta(ref int delta, Vector2d vertex, Vector2d next_vertex, Vector2d p)
        {
            switch (delta)
            {
                // make quadrant deltas wrap around:
                case 3:
                    delta = -1;
                    break;
                case -3:
                    delta = 1;
                    break;
                // check if went around point cw or ccw:
                case 2:
                case -2:
                    if (X_intercept(vertex, next_vertex, p.y) > p.x)
                    {
                        delta = -delta;
                    }
                    break;
            }
        }

        #endregion Point in Polygon

        /// <summary>
        /// Change the swept profile edges from EdgeArray type to line list
        /// </summary>
        /// <param name="oriPoints">the swept profile edges</param>
        /// <returns>the line list which stores the swept profile edges</returns>
        /// <owner> Triaphoo </owner>>
        public static List<Line> ChangeProfilePointListToLine(IList<Vector3d> oriPoints)
        {
            // create the line list instance.
            List<Line> edgeLines = new List<Line>();
            Vector3d length;

            // get each edge from swept profile,
            // and changed the geometry information in line list
            for (int i = 0; i < oriPoints.Count - 1; i++)
            {
                Vector3d first = oriPoints[i];
                Vector3d second = oriPoints[i + 1];

                // create new line and add them into line list
                length = second - first;
                if (length.Length > Line.LINE_MINIZE_LENGTH)
                {
                    Line edge = new Line(first, second);
                    edgeLines.Add(edge);
                }
            }
            length = oriPoints[0] - oriPoints[oriPoints.Count - 1];
            if (length.Length > Line.LINE_MINIZE_LENGTH)
            {
                Line edge = new Line(oriPoints[oriPoints.Count - 1], oriPoints[0]);
                edgeLines.Add(edge);
            }

            return edgeLines;
        }

        /// <summary>
        /// Offset the points of the swept profile to make the points inside swept profile
        /// </summary>
        /// <param name="oriPoints">Point list</param>
        /// <param name="offset">indicate how long to offset on two directions</param>
        /// <returns>the offset points</returns>
        public static Dictionary<Vector3d, PointRelativePos> OffsetPoints(IList<Vector3d> oriPoints, double offset)
        {
            if (IsAlmostZero(offset))
            {
                Dictionary<Vector3d, PointRelativePos> map = OffsetPoints(oriPoints, 1.0);
                Dictionary<Vector3d, PointRelativePos> result = new Dictionary<Vector3d, PointRelativePos>();
                int i = 0;
                foreach (KeyValuePair<Vector3d, PointRelativePos> keyValue in map)
                {
                    result.Add(oriPoints[i++], keyValue.Value);
                }

                return result;
            }

            // Initialize the offset point list.
            Dictionary<Vector3d, PointRelativePos> points = new Dictionary<Vector3d, PointRelativePos>();
            IList<Line> lineList = PolylineOffsetUtil.ChangeProfilePointListToLine(oriPoints);

            IList<Vector2d> oriPointsVector2d = PolylineOffsetUtil.Flatten(oriPoints);

            // Get all points of the swept profile, and offset it in two related direction
            foreach (Vector3d point in oriPoints)
            {
                // Get two related directions
                IList<Vector3d> directions = GetRelatedVectors(lineList, point);
                Vector3d firstDir = directions[0];
                Vector3d secondDir = directions[1];

                // offset the point in two direction
                Vector3d movedPoint = PolylineOffsetUtil.OffsetPoint(point, firstDir, offset);
                movedPoint = PolylineOffsetUtil.OffsetPoint(movedPoint, secondDir, offset);

                PointRelativePos posFlag = PointRelativePos.Inner;

                Vector2d pointVector2d = PolylineOffsetUtil.Flatten(movedPoint);
                if (offset > 0)
                {
                    if (!(PolygonContains(oriPointsVector2d, pointVector2d)))
                    {
                        movedPoint = PolylineOffsetUtil.OffsetPoint(point, firstDir, offset * -1.0);
                        movedPoint = PolylineOffsetUtil.OffsetPoint(movedPoint, secondDir, offset * -1.0);

                        posFlag = PointRelativePos.Outer;
                    }
                }
                else
                {
                    if (PolygonContains(oriPointsVector2d, pointVector2d))
                    {
                        movedPoint = PolylineOffsetUtil.OffsetPoint(point, firstDir, offset * -1.0);
                        movedPoint = PolylineOffsetUtil.OffsetPoint(movedPoint, secondDir, offset * -1.0);

                        posFlag = PointRelativePos.Outer;
                    }
                }

                // add the offset point into the array
                points.Add(movedPoint, posFlag);
            }

            return points;
        }

        public static Dictionary<Vector2d, PointRelativePos> OffsetPoints(IList<Vector2d> oriPoints, double offset)
        {
            if (PolylineOffsetUtil.IsAlmostZero(offset))
            {
                Dictionary<Vector2d, PointRelativePos> map = OffsetPoints(oriPoints, 1.0);
                Dictionary<Vector2d, PointRelativePos> result = new Dictionary<Vector2d, PointRelativePos>();
                int i = 0;
                foreach (KeyValuePair<Vector2d, PointRelativePos> keyValue in map)
                {
                    result.Add(oriPoints[i++], keyValue.Value);
                }

                return result;
            }

            // Initialize the offset point list.
            Dictionary<Vector2d, PointRelativePos> points = new Dictionary<Vector2d, PointRelativePos>();
            IList<Vector3d> oriPointsVector3d = PolylineOffsetUtil.Inflatten(oriPoints);
            IList<Line> lineList = PolylineOffsetUtil.ChangeProfilePointListToLine(oriPointsVector3d);

            // Get all points of the swept profile, and offset it in two related direction
            foreach (Vector3d point in oriPointsVector3d)
            {
                // Get two related directions
                IList<Vector3d> directions = GetRelatedVectors(lineList, point);
                Vector3d firstDir = directions[0];
                Vector3d secondDir = directions[1];

                // offset the point in two direction
                Vector3d movedPoint = PolylineOffsetUtil.OffsetPoint(point, firstDir, offset);
                movedPoint = PolylineOffsetUtil.OffsetPoint(movedPoint, secondDir, offset);

                PointRelativePos posFlag = PointRelativePos.Inner;
                Vector2d pointVector2d = PolylineOffsetUtil.Flatten(movedPoint);
                if (offset > 0)
                {
                    if (!(PolygonContains(oriPoints, pointVector2d)))
                    {
                        movedPoint = PolylineOffsetUtil.OffsetPoint(point, firstDir, offset * -1.0);
                        movedPoint = PolylineOffsetUtil.OffsetPoint(movedPoint, secondDir, offset * -1.0);

                        posFlag = PointRelativePos.Outer;
                    }
                }
                else
                {
                    if (PolygonContains(oriPoints, pointVector2d))
                    {
                        movedPoint = PolylineOffsetUtil.OffsetPoint(point, firstDir, offset * -1.0);
                        movedPoint = PolylineOffsetUtil.OffsetPoint(movedPoint, secondDir, offset * -1.0);

                        posFlag = PointRelativePos.Outer;
                    }
                }

                // add the offset point into the array
                points.Add(new Vector2d(movedPoint.x, movedPoint.y), posFlag);
            }

            return points;
        }

        /// <summary>
        /// Move a point a give offset along a given direction
        /// </summary>
        /// <param name="point">the point need to move</param>
        /// <param name="direction">the direction the point move to</param>
        /// <param name="offset">indicate how long to move</param>
        /// <returns>the moved point</returns>
        public static Vector3d OffsetPoint(Vector3d point, Vector3d direction, double offset)
        {
            Vector3d directUnit = UnitVector(direction);
            Vector3d offsetVect = MultiplyVector(directUnit, offset);

            return AddXYZ(point, offsetVect);
        }

        /// <summary>
        /// Get two vectors, which indicate some edge direction which contain given point, 
        /// set the given point as the start point, the other end point of the edge as end
        /// </summary>
        /// <param name="edges">polygon</param>
        /// <param name="point">a point of the polygon</param>
        /// <returns>two vectors indicate edge direction</returns>
        public static IList<Vector3d> GetRelatedVectors(IList<Line> edges, Vector3d point)
        {
            // Initialize the return vector list.
            List<Vector3d> vectors = new List<Vector3d>();

            // Get all the edge which contain this point.
            // And get the vector from this point to another point
            foreach (Line line in edges)
            {
                if (point.IsAlmostEqualTo(line.StartPoint))
                {
                    Vector3d vector = PolylineOffsetUtil.SubXYZ(line.EndPoint, line.StartPoint);
                    vectors.Add(vector);
                }
                if (point.IsAlmostEqualTo(line.EndPoint))
                {
                    Vector3d vector = PolylineOffsetUtil.SubXYZ(line.StartPoint, line.EndPoint);
                    vectors.Add(vector);
                }
            }

            // only two vector(direction) should be found
            if (2 != vectors.Count)
            {
                throw new Exception("a point on polygon should have only two direction.");
            }

            return vectors;
        }

    }
}
