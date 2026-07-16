using System;
using UnityEngine;

namespace CustomMath
{
    [Serializable]
    public struct Mat4x4 : IEquatable<Mat4x4>
    {
        #region Variables
        public float m00, m10, m20, m30;
        public float m01, m11, m21, m31;
        public float m02, m12, m22, m32;
        public float m03, m13, m23, m33;
        #endregion

        #region Constants
        private const float epsilon = 1e-05f;
        #endregion

        #region Properties
        public static Mat4x4 Zero
        {
            get { return new Mat4x4(); }
        }

        public static Mat4x4 Identity
        {
            get
            {
                Mat4x4 m = new Mat4x4();
                m.m00 = 1f; m.m11 = 1f; m.m22 = 1f; m.m33 = 1f;
                return m;
            }
        }

        public Mat4x4 inverse { get { return Inverse(this); } }

        public float determinant { get { return Determinant(this); } }

        public bool isIdentity { get { return this == Identity; } }

        public Quat rotation
        {
            get
            {
                Vec3 forward = new Vec3(m02, m12, m22);
                Vec3 up = new Vec3(m01, m11, m21);
                return Quat.LookRotation(forward, up);
            }
        }

        public Vec3 lossyScale
        {
            get
            {
                float x = MathF.Sqrt(m00 * m00 + m10 * m10 + m20 * m20);
                float y = MathF.Sqrt(m01 * m01 + m11 * m11 + m21 * m21);
                float z = MathF.Sqrt(m02 * m02 + m12 * m12 + m22 * m22);
                return new Vec3(x, y, z);
            }
        }

        public Mat4x4 transpose { get { return Transpose(this); } }
        #endregion

        #region Constructors
        public Mat4x4(Vector4 column0, Vector4 column1, Vector4 column2, Vector4 column3)
        {
            this.m00 = column0.x; this.m01 = column1.x; this.m02 = column2.x; this.m03 = column3.x;
            this.m10 = column0.y; this.m11 = column1.y; this.m12 = column2.y; this.m13 = column3.y;
            this.m20 = column0.z; this.m21 = column1.z; this.m22 = column2.z; this.m23 = column3.z;
            this.m30 = column0.w; this.m31 = column1.w; this.m32 = column2.w; this.m33 = column3.w;
        }

        public Mat4x4(Matrix4x4 m)
        {
            this.m00 = m.m00; this.m01 = m.m01; this.m02 = m.m02; this.m03 = m.m03;
            this.m10 = m.m10; this.m11 = m.m11; this.m12 = m.m12; this.m13 = m.m13;
            this.m20 = m.m20; this.m21 = m.m21; this.m22 = m.m22; this.m23 = m.m23;
            this.m30 = m.m30; this.m31 = m.m31; this.m32 = m.m32; this.m33 = m.m33;
        }
        #endregion

        #region Operators
        public static Vector4 operator *(Mat4x4 lhs, Vector4 vector)
        {
            return new Vector4(
                lhs.m00 * vector.x + lhs.m01 * vector.y + lhs.m02 * vector.z + lhs.m03 * vector.w,
                lhs.m10 * vector.x + lhs.m11 * vector.y + lhs.m12 * vector.z + lhs.m13 * vector.w,
                lhs.m20 * vector.x + lhs.m21 * vector.y + lhs.m22 * vector.z + lhs.m23 * vector.w,
                lhs.m30 * vector.x + lhs.m31 * vector.y + lhs.m32 * vector.z + lhs.m33 * vector.w
            );
        }

        public static Mat4x4 operator *(Mat4x4 lhs, Mat4x4 rhs)
        {
            Mat4x4 res = new Mat4x4();

            res.m00 = lhs.m00 * rhs.m00 + lhs.m01 * rhs.m10 + lhs.m02 * rhs.m20 + lhs.m03 * rhs.m30;
            res.m01 = lhs.m00 * rhs.m01 + lhs.m01 * rhs.m11 + lhs.m02 * rhs.m21 + lhs.m03 * rhs.m31;
            res.m02 = lhs.m00 * rhs.m02 + lhs.m01 * rhs.m12 + lhs.m02 * rhs.m22 + lhs.m03 * rhs.m32;
            res.m03 = lhs.m00 * rhs.m03 + lhs.m01 * rhs.m13 + lhs.m02 * rhs.m23 + lhs.m03 * rhs.m33;

            res.m10 = lhs.m10 * rhs.m00 + lhs.m11 * rhs.m10 + lhs.m12 * rhs.m20 + lhs.m13 * rhs.m30;
            res.m11 = lhs.m10 * rhs.m01 + lhs.m11 * rhs.m11 + lhs.m12 * rhs.m21 + lhs.m13 * rhs.m31;
            res.m12 = lhs.m10 * rhs.m02 + lhs.m11 * rhs.m12 + lhs.m12 * rhs.m22 + lhs.m13 * rhs.m32;
            res.m13 = lhs.m10 * rhs.m03 + lhs.m11 * rhs.m13 + lhs.m12 * rhs.m23 + lhs.m13 * rhs.m33;

            res.m20 = lhs.m20 * rhs.m00 + lhs.m21 * rhs.m10 + lhs.m22 * rhs.m20 + lhs.m23 * rhs.m30;
            res.m21 = lhs.m20 * rhs.m01 + lhs.m21 * rhs.m11 + lhs.m22 * rhs.m21 + lhs.m23 * rhs.m31;
            res.m22 = lhs.m20 * rhs.m02 + lhs.m21 * rhs.m12 + lhs.m22 * rhs.m22 + lhs.m23 * rhs.m32;
            res.m23 = lhs.m20 * rhs.m03 + lhs.m21 * rhs.m13 + lhs.m22 * rhs.m23 + lhs.m23 * rhs.m33;

            res.m30 = lhs.m30 * rhs.m00 + lhs.m31 * rhs.m10 + lhs.m32 * rhs.m20 + lhs.m33 * rhs.m30;
            res.m31 = lhs.m30 * rhs.m01 + lhs.m31 * rhs.m11 + lhs.m32 * rhs.m21 + lhs.m33 * rhs.m31;
            res.m32 = lhs.m30 * rhs.m02 + lhs.m31 * rhs.m12 + lhs.m32 * rhs.m22 + lhs.m33 * rhs.m32;
            res.m33 = lhs.m30 * rhs.m03 + lhs.m31 * rhs.m13 + lhs.m32 * rhs.m23 + lhs.m33 * rhs.m33;

            return res;
        }

        public static bool operator ==(Mat4x4 lhs, Mat4x4 rhs)
        {
            return new Vector4(lhs.m00, lhs.m10, lhs.m20, lhs.m30) == new Vector4(rhs.m00, rhs.m10, rhs.m20, rhs.m30) &&
                   new Vector4(lhs.m01, lhs.m11, lhs.m21, lhs.m31) == new Vector4(rhs.m01, rhs.m11, rhs.m21, rhs.m31) &&
                   new Vector4(lhs.m02, lhs.m12, lhs.m22, lhs.m32) == new Vector4(rhs.m02, rhs.m12, rhs.m22, rhs.m32) &&
                   new Vector4(lhs.m03, lhs.m13, lhs.m23, lhs.m33) == new Vector4(rhs.m03, rhs.m13, rhs.m23, rhs.m33);
        }

        public static bool operator !=(Mat4x4 lhs, Mat4x4 rhs)
        {
            return !(lhs == rhs);
        }

        public static implicit operator Matrix4x4(Mat4x4 m)
        {
            Matrix4x4 mat = new Matrix4x4();
            mat.m00 = m.m00; mat.m01 = m.m01; mat.m02 = m.m02; mat.m03 = m.m03;
            mat.m10 = m.m10; mat.m11 = m.m11; mat.m12 = m.m12; mat.m13 = m.m13;
            mat.m20 = m.m20; mat.m21 = m.m21; mat.m22 = m.m22; mat.m23 = m.m23;
            mat.m30 = m.m30; mat.m31 = m.m31; mat.m32 = m.m32; mat.m33 = m.m33;
            return mat;
        }

        public static implicit operator Mat4x4(Matrix4x4 m)
        {
            return new Mat4x4(m);
        }
        #endregion

        #region Static Functions
        public static float Determinant(Mat4x4 m)
        {
            float b00 = m.m00 * m.m11 - m.m01 * m.m10;
            float b01 = m.m00 * m.m12 - m.m02 * m.m10;
            float b02 = m.m00 * m.m13 - m.m03 * m.m10;
            float b03 = m.m01 * m.m12 - m.m02 * m.m11;
            float b04 = m.m01 * m.m13 - m.m03 * m.m11;
            float b05 = m.m02 * m.m13 - m.m03 * m.m12;
            float b06 = m.m20 * m.m31 - m.m21 * m.m30;
            float b07 = m.m20 * m.m32 - m.m22 * m.m30;
            float b08 = m.m20 * m.m33 - m.m23 * m.m30;
            float b09 = m.m21 * m.m32 - m.m22 * m.m31;
            float b10 = m.m21 * m.m33 - m.m23 * m.m31;
            float b11 = m.m22 * m.m33 - m.m23 * m.m32;

            return b00 * b11 - b01 * b10 + b02 * b09 + b03 * b08 - b04 * b07 + b05 * b06;
        }

        public static Mat4x4 Inverse(Mat4x4 m)
        {
            float b00 = m.m00 * m.m11 - m.m01 * m.m10;
            float b01 = m.m00 * m.m12 - m.m02 * m.m10;
            float b02 = m.m00 * m.m13 - m.m03 * m.m10;
            float b03 = m.m01 * m.m12 - m.m02 * m.m11;
            float b04 = m.m01 * m.m13 - m.m03 * m.m11;
            float b05 = m.m02 * m.m13 - m.m03 * m.m12;
            float b06 = m.m20 * m.m31 - m.m21 * m.m30;
            float b07 = m.m20 * m.m32 - m.m22 * m.m30;
            float b08 = m.m20 * m.m33 - m.m23 * m.m30;
            float b09 = m.m21 * m.m32 - m.m22 * m.m31;
            float b10 = m.m21 * m.m33 - m.m23 * m.m31;
            float b11 = m.m22 * m.m33 - m.m23 * m.m32;

            float det = b00 * b11 - b01 * b10 + b02 * b09 + b03 * b08 - b04 * b07 + b05 * b06;

            if (MathF.Abs(det) < 1e-6f) return Zero; // Singular (not invertible)

            float invDet = 1.0f / det;

            Mat4x4 res = new Mat4x4();
            res.m00 = (m.m11 * b11 - m.m12 * b10 + m.m13 * b09) * invDet;
            res.m01 = (-m.m01 * b11 + m.m02 * b10 - m.m03 * b09) * invDet;
            res.m02 = (m.m31 * b05 - m.m32 * b04 + m.m33 * b03) * invDet;
            res.m03 = (-m.m21 * b05 + m.m22 * b04 - m.m23 * b03) * invDet;

            res.m10 = (-m.m10 * b11 + m.m12 * b08 - m.m13 * b07) * invDet;
            res.m11 = (m.m00 * b11 - m.m02 * b08 + m.m03 * b07) * invDet;
            res.m12 = (-m.m30 * b05 + m.m32 * b02 - m.m33 * b01) * invDet;
            res.m13 = (m.m20 * b05 - m.m22 * b02 + m.m23 * b01) * invDet;

            res.m20 = (m.m10 * b10 - m.m11 * b08 + m.m13 * b06) * invDet;
            res.m21 = (-m.m00 * b10 + m.m01 * b08 - m.m03 * b06) * invDet;
            res.m22 = (m.m30 * b04 - m.m31 * b02 + m.m33 * b00) * invDet;
            res.m23 = (-m.m20 * b04 + m.m21 * b02 - m.m23 * b00) * invDet;

            res.m30 = (-m.m10 * b09 + m.m11 * b07 - m.m12 * b06) * invDet;
            res.m31 = (m.m00 * b09 - m.m01 * b07 + m.m02 * b06) * invDet;
            res.m32 = (-m.m30 * b03 + m.m31 * b01 - m.m32 * b00) * invDet;
            res.m33 = (m.m20 * b03 - m.m21 * b01 + m.m22 * b00) * invDet;

            return res;
        }

        public static Mat4x4 LookAt(Vec3 from, Vec3 to, Vec3 up)
        {
            return TRS(from, Quat.LookRotation(to - from, up), new Vec3(1f, 1f, 1f));
        }

        public static Mat4x4 Rotate(Quat q)
        {
            float xx = q.x * 2f; float yy = q.y * 2f; float zz = q.z * 2f;
            float qxx = q.x * xx; float qyy = q.y * yy; float qzz = q.z * zz;
            float qxy = q.x * yy; float qxz = q.x * zz; float qyz = q.y * zz;
            float qwx = q.w * xx; float qwy = q.w * yy; float qwz = q.w * zz;

            Mat4x4 res = Identity;
            res.m00 = 1f - (qyy + qzz); res.m01 = qxy - qwz; res.m02 = qxz + qwy;
            res.m10 = qxy + qwz; res.m11 = 1f - (qxx + qzz); res.m12 = qyz - qwx;
            res.m20 = qxz - qwy; res.m21 = qyz + qwx; res.m22 = 1f - (qxx + qyy);
            return res;
        }

        public static Mat4x4 Scale(Vec3 vector)
        {
            Mat4x4 res = Identity;
            res.m00 = vector.x;
            res.m11 = vector.y;
            res.m22 = vector.z;
            return res;
        }

        public static Mat4x4 Translate(Vec3 vector)
        {
            Mat4x4 res = Identity;
            res.m03 = vector.x;
            res.m13 = vector.y;
            res.m23 = vector.z;
            return res;
        }

        public static Mat4x4 Transpose(Mat4x4 m)
        {
            Mat4x4 res = new Mat4x4();
            res.m00 = m.m00; res.m01 = m.m10; res.m02 = m.m20; res.m03 = m.m30;
            res.m10 = m.m01; res.m11 = m.m11; res.m12 = m.m21; res.m13 = m.m31;
            res.m20 = m.m02; res.m21 = m.m12; res.m22 = m.m22; res.m23 = m.m32;
            res.m30 = m.m03; res.m31 = m.m13; res.m32 = m.m23; res.m33 = m.m33;
            return res;
        }

        public static Mat4x4 TRS(Vec3 pos, Quat q, Vec3 s)
        {
            Mat4x4 res = Rotate(q);

            res.m00 *= s.x; res.m10 *= s.x; res.m20 *= s.x;
            res.m01 *= s.y; res.m11 *= s.y; res.m21 *= s.y;
            res.m02 *= s.z; res.m12 *= s.z; res.m22 *= s.z;

            res.m03 = pos.x; res.m13 = pos.y; res.m23 = pos.z;
            return res;
        }
        #endregion

        #region Instance Functions
        public Vec3 GetPosition()
        {
            return new Vec3(m03, m13, m23);
        }

        public Vector4 GetRow(int index)
        {
            switch (index)
            {
                case 0: return new Vector4(m00, m01, m02, m03);
                case 1: return new Vector4(m10, m11, m12, m13);
                case 2: return new Vector4(m20, m21, m22, m23);
                case 3: return new Vector4(m30, m31, m32, m33);
                default:
                        Debug.LogError("out of index range!");
                        return new Vector4();
                    
            }
        }

        public Vec3 MultiplyPoint(Vec3 point)
        {
            float resX = m00 * point.x + m01 * point.y + m02 * point.z + m03;
            float resY = m10 * point.x + m11 * point.y + m12 * point.z + m13;
            float resZ = m20 * point.x + m21 * point.y + m22 * point.z + m23;
            float w = m30 * point.x + m31 * point.y + m32 * point.z + m33;

            w = 1f / w;
            return new Vec3(resX * w, resY * w, resZ * w);
        }

        public Vec3 MultiplyPoint3x4(Vec3 point)
        {
            return new Vec3(
                m00 * point.x + m01 * point.y + m02 * point.z + m03,
                m10 * point.x + m11 * point.y + m12 * point.z + m13,
                m20 * point.x + m21 * point.y + m22 * point.z + m23
            );
        }

        public Vec3 MultiplyVector(Vec3 vector)
        {
            return new Vec3(
                m00 * vector.x + m01 * vector.y + m02 * vector.z,
                m10 * vector.x + m11 * vector.y + m12 * vector.z,
                m20 * vector.x + m21 * vector.y + m22 * vector.z
            );
        }

        public void SetColumn(int index, Vector4 column)
        {
            switch (index)
            {
                case 0: m00 = column.x; m10 = column.y; m20 = column.z; m30 = column.w; break;
                case 1: m01 = column.x; m11 = column.y; m21 = column.z; m31 = column.w; break;
                case 2: m02 = column.x; m12 = column.y; m22 = column.z; m32 = column.w; break;
                case 3: m03 = column.x; m13 = column.y; m23 = column.z; m33 = column.w; break;
                default:
                    Debug.LogError("out of index range!");
                    return;
            }
        }

        public void SetRow(int index, Vector4 row)
        {
            switch (index)
            {
                case 0: m00 = row.x; m01 = row.y; m02 = row.z; m03 = row.w; break;
                case 1: m10 = row.x; m11 = row.y; m12 = row.z; m13 = row.w; break;
                case 2: m20 = row.x; m21 = row.y; m22 = row.z; m23 = row.w; break;
                case 3: m30 = row.x; m31 = row.y; m32 = row.z; m33 = row.w; break;
                default:
                    Debug.LogError("out of index range!");
                    return;
            }
        }

        public void SetTRS(Vec3 pos, Quat q, Vec3 s)
        {
            this = TRS(pos, q, s);
        }

        public bool ValidTRS()
        {
            return m30 == 0f && m31 == 0f && m32 == 0f && m33 == 1f;
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

        public override int GetHashCode()
        {
            int hash1 = HashCode.Combine(m00, m01, m02, m03, m10, m11, m12, m13);
            int hash2 = HashCode.Combine(m20, m21, m22, m23, m30, m31, m32, m33);
            return HashCode.Combine(hash1, hash2);
        }

        public override string ToString()
        {
            return $"{m00:F5}\t{m01:F5}\t{m02:F5}\t{m03:F5}\n" +
                   $"{m10:F5}\t{m11:F5}\t{m12:F5}\t{m13:F5}\n" +
                   $"{m20:F5}\t{m21:F5}\t{m22:F5}\t{m23:F5}\n" +
                   $"{m30:F5}\t{m31:F5}\t{m32:F5}\t{m33:F5}";
        }
        #endregion
    }
}