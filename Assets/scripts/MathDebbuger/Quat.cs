using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UIElements;
namespace CustomMath
{
    [System.Serializable]
    public struct Quat : IEquatable<Quat>
    {
        #region Variables
        public float x;
        public float y;
        public float z;
        public float w;

        //public float sqrMagnitude { get { return SqrMagnitude(this); } }
        //public Vec3 normalized { get { return new Vec3(this.x / magnitude, this.y / magnitude, this.z / magnitude); } }
        //public float magnitude { get { return Magnitude(this); } }
        #endregion

        #region constants
        public const float epsilon = 1e-05f;
        #endregion

        #region Default Values
        public static Quat Identity { get { return new Quat(0.0f, 0.0f, 0.0f,1.0f); } }
     #endregion                                                                                                                                                                               

        #region Constructors

        //Normal constructor
        public Quat(float x, float y, float z, float w)
        {
            this.x = x;
            this.y = y;
            this.z = z;
            this.w = w;
        }

        //Copy constructor
        public Quat(Quat q)
        {
            this.x = q.x;
            this.y = q.y;
            this.z = q.z;
            this.w = q.w;
        }


        //Unity quaternion constructor
        public Quat(Quaternion q)
        {
            this.x = q.x;
            this.y = q.y;
            this.z = q.z;
            this.w = q.w;
        }

        #endregion

        #region Operators
        public static bool operator ==(Quat left, Quat right)
        {
            float diff_x = left.x - right.x;
            float diff_y = left.y - right.y;
            float diff_z = left.z - right.z;
            float diff_w = left.w - right.w;

            float sqrmag = diff_x * diff_x + diff_y * diff_y + diff_z * diff_z + diff_w * diff_w;

            return sqrmag < epsilon * epsilon;

            // Could also compare each difference independantly, like
            // bool same = (std::Abs(left.x - right.x) < epsilon)
            // But the way we're doing it allows for more presition
            // As we're using epsilon^2
        }

        public static bool operator !=(Quat left, Quat right)
        {
            return !(left == right);
        }

        public static Quat operator *(Quat q, Vec3 point)
        {
            return new Quat(v3.x * scalar, v3.y * scalar, v3.z * scalar);
        }
        public static Quat operator *(Quat left, Quat right)
        {
            return new Quat(v3.x * scalar, v3.y * scalar, v3.z * scalar);
        }

        public static implicit operator Quaternion(Quat q)
        {
            return new Quaternion(q.x, q.y,q.z,q.w);
        }
        #endregion

        #region Functions
        public override string ToString()
        {
            return "X = " + x.ToString() + " Y = " + y.ToString() + " Z = " + z.ToString() + " W = " + w.ToString();
        }
        public static float Angle(Vec3 from, Vec3 to)
        {
            return System.MathF.Acos(Dot(from.normalized, to.normalized)) * Mathf.Rad2Deg;
        }
        public static Vec3 ClampMagnitude(Vec3 vector, float maxLength)
        {
            return ((vector.magnitude <= maxLength) ? vector : (vector.normalized * maxLength));
        }
        public static float Magnitude(Vec3 vector)
        {
            return System.MathF.Sqrt((vector.x * vector.x) + (vector.y * vector.y) + (vector.z * vector.z));
        }
        public static Vec3 Cross(Vec3 a, Vec3 b)
        {
            return new Vec3(a.y * b.z - a.z * b.y, a.z * b.x - a.x * b.z, a.x * b.y - a.y * b.x);
        }
        public static float Distance(Vec3 a, Vec3 b)
        {
            return Magnitude(a - b);
        }
        public static float Dot(Vec3 a, Vec3 b)
        {
            return (a.x * b.x + a.y * b.y + a.z * b.z);
        }
        public static Vec3 Lerp(Vec3 a, Vec3 b, float t)
        {
            t = System.Math.Clamp(t, 0.0f, 1.0f);

            //return new Vec3(a * (1.0f - t) + b * t); //mi implementacion
            return new Vec3(a + (b - a) * t);
        }
        public static Vec3 LerpUnclamped(Vec3 a, Vec3 b, float t)
        {
            //return new Vec3(a * (1.0f - t) + b * t); //mi implementacion
            return new Vec3(a + (b - a) * t);
        }
        public static Vec3 Max(Vec3 a, Vec3 b)
        {
            //return a.magnitude > b.magnitude ? a : b;
            return new Vec3(Mathf.Max(a.x, b.x), Mathf.Max(a.y, b.y), Mathf.Max(a.z, b.z));
        }
        public static Vec3 Min(Vec3 a, Vec3 b)
        {
            //return a.magnitude < b.magnitude ? a : b;
            return new Vec3(Mathf.Min(a.x, b.x), Mathf.Min(a.y, b.y), Mathf.Min(a.z, b.z));
        }
        public static float SqrMagnitude(Vec3 vector)
        {
            return ((vector.x * vector.x) + (vector.y * vector.y) + (vector.z * vector.z));
        }
        public static Vec3 Project(Vec3 vector, Vec3 onNormal)
        {
            Vec3 normalized = onNormal.normalized;
            float mag = Dot(vector, normalized);
            return (normalized * mag);
        }
        public static Vec3 Reflect(Vec3 inDirection, Vec3 inNormal)
        {
            //return inDirection - 2 * inNormal.normalized * Dot(inDirection,inNormal.normalized); //resultado de Unity

            return (Project(-inDirection, inNormal.normalized) * 2) + inDirection;
        }
        public void Set(float newX, float newY, float newZ)
        {
            this.x = newX;
            this.y = newY;
            this.z = newZ;
            return;
        }
        #endregion

        #region Internals
        public override bool Equals(object other)
        {
            if (!(other is Quat)) return false;
            return Equals((Quat)other);
        }

        public bool Equals(Quat other)
        {
            return this == other;
        }

        public override int GetHashCode()
        {
            return x.GetHashCode() ^ (y.GetHashCode() << 2) ^ (z.GetHashCode() >> 2) ^ (w.GetHashCode() << 2);
        }
        #endregion
    }
}