using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace polylineoffset
{
    public struct Vector3d : IComparable<Vector3d>, IEquatable<Vector3d>
    {
        public double x;
        public double y;
        public double z;

        public Vector3d(double f) { x = y = z = f; }
        public Vector3d(double x, double y, double z) { this.x = x; this.y = y; this.z = z; }
        public Vector3d(double[] v2) { x = v2[0]; y = v2[1]; z = v2[2]; }
        public Vector3d(Vector3d copy) { x = copy.x; y = copy.y; z = copy.z; }
        

        public double LengthSquared
        {
            get { return x * x + y * y + z * z; }
        }
        public double Length
        {
            get { return Math.Sqrt(LengthSquared); }
        }

        public double LengthL1
        {
            get { return Math.Abs(x) + Math.Abs(y) + Math.Abs(z); }
        }

        public double Max
        {
            get { return Math.Max(x, Math.Max(y, z)); }
        }
        public double Min
        {
            get { return Math.Min(x, Math.Min(y, z)); }
        }
        public double MaxAbs
        {
            get { return Math.Max(Math.Abs(x), Math.Max(Math.Abs(y), Math.Abs(z))); }
        }
        public double MinAbs
        {
            get { return Math.Min(Math.Abs(x), Math.Min(Math.Abs(y), Math.Abs(z))); }
        }

        public Vector3d Abs
        {
            get { return new Vector3d(Math.Abs(x), Math.Abs(y), Math.Abs(z)); }
        }
        
        public bool IsFinite
        {
            get { double f = x + y + z; return double.IsNaN(f) == false && double.IsInfinity(f) == false; }
        }

        public void Round(int nDecimals)
        {
            x = Math.Round(x, nDecimals);
            y = Math.Round(y, nDecimals);
            z = Math.Round(z, nDecimals);
        }


        public double Dot(Vector3d v2)
        {
            return x * v2.x + y * v2.y + z * v2.z;
        }
        public double Dot(ref Vector3d v2)
        {
            return x * v2.x + y * v2.y + z * v2.z;
        }

        public static double Dot(Vector3d v1, Vector3d v2)
        {
            return v1.Dot(v2);
        }

        public Vector3d Cross(Vector3d v2)
        {
            return new Vector3d(
                y * v2.z - z * v2.y,
                z * v2.x - x * v2.z,
                x * v2.y - y * v2.x);
        }
        public static Vector3d Cross(Vector3d v1, Vector3d v2)
        {
            return v1.Cross(v2);
        }

        public double DistanceSquared(Vector3d v2)
        {
            double dx = v2.x - x, dy = v2.y - y, dz = v2.z - z;
            return dx * dx + dy * dy + dz * dz;
        }
        public double DistanceSquared(ref Vector3d v2)
        {
            double dx = v2.x - x, dy = v2.y - y, dz = v2.z - z;
            return dx * dx + dy * dy + dz * dz;
        }

        public double Distance(Vector3d v2)
        {
            double dx = v2.x - x, dy = v2.y - y, dz = v2.z - z;
            return Math.Sqrt(dx * dx + dy * dy + dz * dz);
        }
        public double Distance(ref Vector3d v2)
        {
            double dx = v2.x - x, dy = v2.y - y, dz = v2.z - z;
            return Math.Sqrt(dx * dx + dy * dy + dz * dz);
        }

        public void Set(Vector3d o)
        {
            x = o.x; y = o.y; z = o.z;
        }
        public void Set(double fX, double fY, double fZ)
        {
            x = fX; y = fY; z = fZ;
        }
        public void Add(Vector3d o)
        {
            x += o.x; y += o.y; z += o.z;
        }
        public void Subtract(Vector3d o)
        {
            x -= o.x; y -= o.y; z -= o.z;
        }



        public static Vector3d operator -(Vector3d v)
        {
            return new Vector3d(-v.x, -v.y, -v.z);
        }

        public static Vector3d operator *(double f, Vector3d v)
        {
            return new Vector3d(f * v.x, f * v.y, f * v.z);
        }
        public static Vector3d operator *(Vector3d v, double f)
        {
            return new Vector3d(f * v.x, f * v.y, f * v.z);
        }
        public static Vector3d operator /(Vector3d v, double f)
        {
            return new Vector3d(v.x / f, v.y / f, v.z / f);
        }
        public static Vector3d operator /(double f, Vector3d v)
        {
            return new Vector3d(f / v.x, f / v.y, f / v.z);
        }

        public static Vector3d operator *(Vector3d a, Vector3d b)
        {
            return new Vector3d(a.x * b.x, a.y * b.y, a.z * b.z);
        }
        public static Vector3d operator /(Vector3d a, Vector3d b)
        {
            return new Vector3d(a.x / b.x, a.y / b.y, a.z / b.z);
        }


        public static Vector3d operator +(Vector3d v0, Vector3d v1)
        {
            return new Vector3d(v0.x + v1.x, v0.y + v1.y, v0.z + v1.z);
        }
        public static Vector3d operator +(Vector3d v0, double f)
        {
            return new Vector3d(v0.x + f, v0.y + f, v0.z + f);
        }

        public static Vector3d operator -(Vector3d v0, Vector3d v1)
        {
            return new Vector3d(v0.x - v1.x, v0.y - v1.y, v0.z - v1.z);
        }
        public static Vector3d operator -(Vector3d v0, double f)
        {
            return new Vector3d(v0.x - f, v0.y - f, v0.z - f);
        }



        public static bool operator ==(Vector3d a, Vector3d b)
        {
            return (a.x == b.x && a.y == b.y && a.z == b.z);
        }
        public static bool operator !=(Vector3d a, Vector3d b)
        {
            return (a.x != b.x || a.y != b.y || a.z != b.z);
        }
        public override bool Equals(object obj)
        {
            return this == (Vector3d)obj;
        }
        public override int GetHashCode()
        {
            unchecked // Overflow is fine, just wrap
            {
                int hash = (int)2166136261;
                // Suitable nullity checks etc, of course :)
                hash = (hash * 16777619) ^ x.GetHashCode();
                hash = (hash * 16777619) ^ y.GetHashCode();
                hash = (hash * 16777619) ^ z.GetHashCode();
                return hash;
            }
        }
        public int CompareTo(Vector3d other)
        {
            if (x != other.x)
                return x < other.x ? -1 : 1;
            else if (y != other.y)
                return y < other.y ? -1 : 1;
            else if (z != other.z)
                return z < other.z ? -1 : 1;
            return 0;
        }
        public bool Equals(Vector3d other)
        {
            return (x == other.x && y == other.y && z == other.z);
        }

        public bool IsAlmostEqualTo(Vector3d other, double eps = 1e-6)
        {
            return (Math.Abs(x - other.x) < eps &&
                    Math.Abs(y - other.y) < eps &&
                    Math.Abs(z - other.z) < eps);
        }
    }
}