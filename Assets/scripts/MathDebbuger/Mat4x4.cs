using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UIElements;
namespace CustomMath
{
    [System.Serializable]
    public struct Mat4x4 : IEquatable<Mat4x4>
    {
        #region Variables

        public float m00,m01,m02,m03;
        public float m10,m11,m12,m13;
        public float m20,m21,m22,m23;
        public float m30,m31,m32,m33;

        public float inverse { get { return SqrMagnitude(this); } }
        public Vec3 determinant { get { return new Vec3(this.x / magnitude, this.y / magnitude, this.z / magnitude); } }
        public float isIdentity { get { return Magnitude(this); } }
        public float rotation { get { return Magnitude(this); } }
        public float lossyScale { get { return Magnitude(this); } }
        public float transpose { get { return Magnitude(this); } }
        #endregion

        #region constants
        public const float epsilon = 1e-05f;
        #endregion

        #region Default Values
        public static Mat4x4 Zero { get { return new Mat4x4(Vector4.zero, Vector4.zero, Vector4.zero, Vector4.zero); } }
        public static Mat4x4 Identity { get { return new Mat4x4(new Vector4(1.0f,0.0f,0.0f,0.0f), new Vector4(0.0f, 1.0f, 0.0f, 0.0f), new Vector4(0.0f, 0.0f, 1.0f, 0.0f), new Vector4(0.0f, 0.0f, 0.0f, 1.0f)); } }
        #endregion

        #region Constructors

        //Normal constructor
        public Mat4x4(Vector4 column0, Vector4 column1, Vector4 column2, Vector4 column3)
        {
            this.m00 = column0.x;
            this.m10 = column0.y;
            this.m20 = column0.z;
            this.m30 = column0.w;

            this.m01 = column1.x;
            this.m11 = column1.y;
            this.m21 = column1.z;
            this.m31 = column1.w;

            this.m02 = column2.x;
            this.m12 = column2.y;
            this.m22 = column2.z;
            this.m32 = column2.w;

            this.m03 = column3.x;
            this.m13 = column3.y;
            this.m23 = column3.z;
            this.m33 = column3.w;
        }

        //Unity constructor
        public Mat4x4(Matrix4x4 m)
        {
            this.m00 = m.m00;
            this.m01 = m.m01;
            this.m02 = m.m02;
            this.m03 = m.m03;
            this.m10 = m.m10;
            this.m11 = m.m11;
            this.m12 = m.m12;
            this.m13 = m.m13;
            this.m20 = m.m20;
            this.m21 = m.m21;
            this.m22 = m.m22;
            this.m23 = m.m23;
            this.m30 = m.m30;
            this.m31 = m.m31;
            this.m32 = m.m32;
            this.m33 = m.m33;
        }

        #endregion

        #region Operators
        public static bool operator ==(Mat4x4 left, Mat4x4 right)
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

        public static bool operator !=(Mat4x4 left, Mat4x4 right)
        {
            return !(left == right);
        }

        public static Mat4x4 operator *(Mat4x4 left, Mat4x4 right)
        {
            return new Mat4x4();
            
        }
        public static Mat4x4 operator *(Mat4x4 left, Vector4 right)
        {
            return new Mat4x4();
        }

        public static implicit operator Matrix4x4(Mat4x4 m)
        {
            Vector4 column0 = new Vector4(m.m00,m.m10, m.m20, m.m30);
            Vector4 column1 = new Vector4(m.m01,m.m11, m.m21, m.m31);
            Vector4 column2 = new Vector4(m.m02,m.m12, m.m22, m.m32);
            Vector4 column3 = new Vector4(m.m03,m.m13, m.m23, m.m33);

            return new Matrix4x4(column0,column1,column2,column3);
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
            if (!(other is Mat4x4)) return false;
            return Equals((Mat4x4)other);
        }

        public bool Equals(Mat4x4 other)
        {
            return this == other;
        }
        #endregion
    }
}