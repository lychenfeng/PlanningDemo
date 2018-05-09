using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace polylineoffset
{
    public struct Vector2d : IComparable<Vector2d>, IEquatable<Vector2d>
    {
        public double x;
        public double y;

        public Vector2d(double f) { x = y = f; }
        public Vector2d(double x, double y) { this.x = x; this.y = y; }
        public Vector2d(double[] v2) { x = v2[0]; y = v2[1]; }
        public Vector2d(float f) { x = y = f; }
        public Vector2d(float x, float y) { this.x = x; this.y = y; }
        public Vector2d(float[] v2) { x = v2[0]; y = v2[1]; }
        public Vector2d(Vector2d copy) { x = copy.x; y = copy.y; }

        static public readonly Vector2d Zero = new Vector2d(0.0f, 0.0f);
        static public readonly Vector2d One = new Vector2d(1.0f, 1.0f);
        static public readonly Vector2d AxisX = new Vector2d(1.0f, 0.0f);
        static public readonly Vector2d AxisY = new Vector2d(0.0f, 1.0f);
        static public readonly Vector2d MaxValue = new Vector2d(double.MaxValue, double.MaxValue);
        static public readonly Vector2d MinValue = new Vector2d(double.MinValue, double.MinValue);

        public static Vector2d FromAngleRad(double angle)
        {
            return new Vector2d(Math.Cos(angle), Math.Sin(angle));
        }

        public double this[int key]
        {
            get { return (key == 0) ? x : y; }
            set { if (key == 0) x = value; else y = value; }
        }


        public double LengthSquared
        {
            get { return x * x + y * y; }
        }
        public double Length
        {
            get { return (double)Math.Sqrt(LengthSquared); }
        }
        
        public bool IsFinite
        {
            get { double f = x + y; return double.IsNaN(f) == false && double.IsInfinity(f) == false; }
        }

        public void Round(int nDecimals)
        {
            x = Math.Round(x, nDecimals);
            y = Math.Round(y, nDecimals);
        }


        public double Dot(Vector2d v2)
        {
            return x * v2.x + y * v2.y;
        }


        /// <summary>
        /// returns cross-product of this vector with v2 (same as DotPerp)
        /// </summary>
        public double Cross(Vector2d v2)
        {
            return x * v2.y - y * v2.x;
        }


        /// <summary>
        /// returns right-perp vector, ie rotated 90 degrees to the right
        /// </summary>
		public Vector2d Perp
        {
            get { return new Vector2d(y, -x); }
        }

        /// <summary>
        /// returns dot-product of this vector with v2.Perp
        /// </summary>
		public double DotPerp(Vector2d v2)
        {
            return x * v2.y - y * v2.x;
        }        


        public double DistanceSquared(Vector2d v2)
        {
            double dx = v2.x - x, dy = v2.y - y;
            return dx * dx + dy * dy;
        }
        public double Distance(Vector2d v2)
        {
            double dx = v2.x - x, dy = v2.y - y;
            return Math.Sqrt(dx * dx + dy * dy);
        }


        public void Set(Vector2d o)
        {
            x = o.x; y = o.y;
        }
        public void Set(double fX, double fY)
        {
            x = fX; y = fY;
        }
        public void Add(Vector2d o)
        {
            x += o.x; y += o.y;
        }
        public void Subtract(Vector2d o)
        {
            x -= o.x; y -= o.y;
        }



        public static Vector2d operator -(Vector2d v)
        {
            return new Vector2d(-v.x, -v.y);
        }

        public static Vector2d operator +(Vector2d a, Vector2d o)
        {
            return new Vector2d(a.x + o.x, a.y + o.y);
        }
        public static Vector2d operator +(Vector2d a, double f)
        {
            return new Vector2d(a.x + f, a.y + f);
        }

        public static Vector2d operator -(Vector2d a, Vector2d o)
        {
            return new Vector2d(a.x - o.x, a.y - o.y);
        }
        public static Vector2d operator -(Vector2d a, double f)
        {
            return new Vector2d(a.x - f, a.y - f);
        }

        public static Vector2d operator *(Vector2d a, double f)
        {
            return new Vector2d(a.x * f, a.y * f);
        }
        public static Vector2d operator *(double f, Vector2d a)
        {
            return new Vector2d(a.x * f, a.y * f);
        }
        public static Vector2d operator /(Vector2d v, double f)
        {
            return new Vector2d(v.x / f, v.y / f);
        }
        public static Vector2d operator /(double f, Vector2d v)
        {
            return new Vector2d(f / v.x, f / v.y);
        }


        public static Vector2d operator *(Vector2d a, Vector2d b)
        {
            return new Vector2d(a.x * b.x, a.y * b.y);
        }
        public static Vector2d operator /(Vector2d a, Vector2d b)
        {
            return new Vector2d(a.x / b.x, a.y / b.y);
        }


        public static bool operator ==(Vector2d a, Vector2d b)
        {
            return (a.x == b.x && a.y == b.y);
        }
        public static bool operator !=(Vector2d a, Vector2d b)
        {
            return (a.x != b.x || a.y != b.y);
        }
        public override bool Equals(object obj)
        {
            return this == (Vector2d)obj;
        }
        public override int GetHashCode()
        {
            unchecked // Overflow is fine, just wrap
            {
                int hash = (int)2166136261;
                // Suitable nullity checks etc, of course :)
                hash = (hash * 16777619) ^ x.GetHashCode();
                hash = (hash * 16777619) ^ y.GetHashCode();
                return hash;
            }
        }
        public int CompareTo(Vector2d other)
        {
            if (x != other.x)
                return x < other.x ? -1 : 1;
            else if (y != other.y)
                return y < other.y ? -1 : 1;
            return 0;
        }
        public bool Equals(Vector2d other)
        {
            return (x == other.x && y == other.y);
        }


        public bool EpsilonEqual(Vector2d v2, double epsilon)
        {
            return Math.Abs(x - v2.x) <= epsilon &&
                   Math.Abs(y - v2.y) <= epsilon;
        }


        public static Vector2d Lerp(Vector2d a, Vector2d b, double t)
        {
            double s = 1 - t;
            return new Vector2d(s * a.x + t * b.x, s * a.y + t * b.y);
        }
        public static Vector2d Lerp(ref Vector2d a, ref Vector2d b, double t)
        {
            double s = 1 - t;
            return new Vector2d(s * a.x + t * b.x, s * a.y + t * b.y);
        }
    }
}
