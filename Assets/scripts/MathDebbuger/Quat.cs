using System;
using UnityEngine;
using UnityEngine.Internal;

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
        #endregion

        #region constants
        public const float epsilon = 1e-05f;
        private const float radToDeg = 57.2957795f;
        private const float degToRad = 0.0174532924f;
        #endregion

        #region Properties
        public static Quat identity { get { return new Quat(0.0f, 0.0f, 0.0f,1.0f); } }

        public Vec3 eulerAngles
        {
            get { return ToEulerAngles(this); }
            set { this = Euler(value); }
        }

        public Quat normalized { get { return Normalize(this); } }
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
        public static bool operator ==(Quat lhs, Quat rhs)
        {
            return MathF.Abs(Dot(lhs, rhs)) >= 1.0f - epsilon;
        }

        public static bool operator !=(Quat lhs, Quat rhs)
        {
            return !(lhs == rhs);
        }

        public static Vec3 operator *(Quat rotation, Vec3 point)
        {
            float num = rotation.x * 2f;
            float num2 = rotation.y * 2f;
            float num3 = rotation.z * 2f;
            float num4 = rotation.x * num;
            float num5 = rotation.y * num2;
            float num6 = rotation.z * num3;
            float num7 = rotation.x * num2;
            float num8 = rotation.x * num3;
            float num9 = rotation.y * num3;
            float num10 = rotation.w * num;
            float num11 = rotation.w * num2;
            float num12 = rotation.w * num3;
            Vec3 result = default;
            result.x = (1f - (num5 + num6)) * point.x + (num7 - num12) * point.y + (num8 + num11) * point.z;
            result.y = (num7 + num12) * point.x + (1f - (num4 + num6)) * point.y + (num9 - num10) * point.z;
            result.z = (num8 - num11) * point.x + (num9 + num10) * point.y + (1f - (num4 + num5)) * point.z;
            return result;
        }

        public static Quat operator *(Quat lhs, Quat rhs)
        {
            return new Quat(
                lhs.w * rhs.x + lhs.x * rhs.w + lhs.y * rhs.z - lhs.z * rhs.y,
                lhs.w * rhs.y + lhs.y * rhs.w + lhs.z * rhs.x - lhs.x * rhs.z,
                lhs.w * rhs.z + lhs.z * rhs.w + lhs.x * rhs.y - lhs.y * rhs.x,
                lhs.w * rhs.w - lhs.x * rhs.x - lhs.y * rhs.y - lhs.z * rhs.z
            );
        }

        //Implicit to Unity Quaternion
        public static implicit operator Quaternion(Quat q)
        {
            return new Quaternion(q.x, q.y, q.z, q.w);
        }

        //Implicit from Unity Quaternion
        public static implicit operator Quat(Quaternion q)
        {
            return new Quat(q.x, q.y, q.z, q.w);
        }
        #endregion

        #region Static Functions
        public static float Angle(Quat a, Quat b)
        {
            float dot = Dot(a, b);
            return MathF.Acos(MathF.Min(MathF.Abs(dot), 1f)) * 2f * radToDeg;
        }

        public static Quat AngleAxis(float angle, Vec3 axis)
        {
            Vec3 normAxis = axis.normalized;
            float halfAngleRad = angle * 0.5f * degToRad;
            float sinHalfAngle = MathF.Sin(halfAngleRad);

            return new Quat(
                normAxis.x * sinHalfAngle,
                normAxis.y * sinHalfAngle,
                normAxis.z * sinHalfAngle,
                MathF.Cos(halfAngleRad)
            );
        }

        public static float Dot(Quat a, Quat b)
        {
            return a.x * b.x + a.y * b.y + a.z * b.z + a.w * b.w;
        }

        public static Quat Euler(Vec3 euler)
        {
            float halfX = euler.x * degToRad * 0.5f;
            float halfY = euler.y * degToRad * 0.5f;
            float halfZ = euler.z * degToRad * 0.5f;

            float cx = MathF.Cos(halfX); float sx = MathF.Sin(halfX);
            float cy = MathF.Cos(halfY); float sy = MathF.Sin(halfY);
            float cz = MathF.Cos(halfZ); float sz = MathF.Sin(halfZ);

            return new Quat(
                sx * cy * cz + cx * sy * sz,
                cx * sy * cz - sx * cy * sz,
                cx * cy * sz - sx * sy * cz,
                cx * cy * cz + sx * sy * sz
            );
        }

        public static Quat EulerAngles(Vec3 euler) { return Euler(euler); }
        public static Quat EulerRotation(Vec3 euler) { return Euler(euler); }

        public static Quat FromToRotation(Vec3 fromDirection, Vec3 toDirection)
        {
            Vec3 from = fromDirection.normalized;
            Vec3 to = toDirection.normalized;
            float dot = Vec3.Dot(from, to);

            if (dot < -(1.0f - epsilon))
            {
                Vec3 ortho = MathF.Abs(from.x) > MathF.Abs(from.y) ? new Vec3(from.z, 0f, -from.x) : new Vec3(0f, -from.z, -from.y);
                return new Quat(ortho.normalized.x, ortho.normalized.y, ortho.normalized.z, 0f);
            }
            if (dot > (1.0f - epsilon)) return identity;

            Vec3 cross = Vec3.Cross(from, to);
            Quat q = new Quat(cross.x, cross.y, cross.z, 1f + dot);
            return q.normalized;
        }

        public static Quat Inverse(Quat rotation)
        {
            return new Quat(-rotation.x, -rotation.y, -rotation.z, rotation.w);
        }

        public static Quat Lerp(Quat a, Quat b, float t)
        {
            return LerpUnclamped(a, b, Math.Clamp(t, 0f, 1f));
        }

        public static Quat LerpUnclamped(Quat a, Quat b, float t)
        {
            Quat result = new Quat();
            float dot = Dot(a, b);

            float sign = dot < 0f ? -1f : 1f;

            result.x = a.x + (b.x * sign - a.x) * t;
            result.y = a.y + (b.y * sign - a.y) * t;
            result.z = a.z + (b.z * sign - a.z) * t;
            result.w = a.w + (b.w * sign - a.w) * t;

            return result.normalized;
        }

        public static Quat LookRotation(Vec3 forward)
        {
            return LookRotation(forward, Vec3.Up);
        }

        public static Quat LookRotation(Vec3 forward, [DefaultValue("Vec3.up")] Vec3 upwards)
        {
            Vec3 z = forward.normalized;
            Vec3 x = Vec3.Cross(upwards, z).normalized;
            Vec3 y = Vec3.Cross(z, x);

            float m00 = x.x; float m01 = x.y; float m02 = x.z;
            float m10 = y.x; float m11 = y.y; float m12 = y.z;
            float m20 = z.x; float m21 = z.y; float m22 = z.z;

            float num8 = (m00 + m11) + m22;
            Quat q = new Quat();
            if (num8 > 0f)
            {
                float num = MathF.Sqrt(num8 + 1f);
                q.w = num * 0.5f;
                num = 0.5f / num;
                q.x = (m12 - m21) * num;
                q.y = (m20 - m02) * num;
                q.z = (m01 - m10) * num;
            }
            else if ((m00 >= m11) && (m00 >= m22))
            {
                float num7 = MathF.Sqrt(((1f + m00) - m11) - m22);
                float num4 = 0.5f / num7;
                q.x = 0.5f * num7;
                q.y = (m01 + m10) * num4;
                q.z = (m02 + m20) * num4;
                q.w = (m12 - m21) * num4;
            }
            else if (m11 > m22)
            {
                float num6 = MathF.Sqrt(((1f + m11) - m00) - m22);
                float num3 = 0.5f / num6;
                q.x = (m10 + m01) * num3;
                q.y = 0.5f * num6;
                q.z = (m21 + m12) * num3;
                q.w = (m20 - m02) * num3;
            }
            else
            {
                float num5 = MathF.Sqrt(((1f + m22) - m00) - m11);
                float num2 = 0.5f / num5;
                q.x = (m20 + m02) * num2;
                q.y = (m21 + m12) * num2;
                q.z = 0.5f * num5;
                q.w = (m01 - m10) * num2;
            }
            return q;
        }

        public static Quat Normalize(Quat q)
        {
            float mag = MathF.Sqrt(Dot(q, q));
            if (mag < epsilon) return identity;
            return new Quat(q.x / mag, q.y / mag, q.z / mag, q.w / mag);
        }

        public static Quat RotateTowards(Quat from, Quat to, float maxDegreesDelta)
        {
            float angle = Angle(from, to);
            if (angle == 0f) return to;
            return SlerpUnclamped(from, to, MathF.Min(1f, maxDegreesDelta / angle));
        }

        public static Quat Slerp(Quat a, Quat b, float t)
        {
            return SlerpUnclamped(a, b, Math.Clamp(t, 0f, 1f));
        }

        public static Quat SlerpUnclamped(Quat a, Quat b, float t)
        {
            float dot = Dot(a, b);
            Quat b2 = b;

            if (dot < 0f)
            {
                dot = -dot;
                b2 = new Quat(-b.x, -b.y, -b.z, -b.w);
            }

            if (dot > (1.0f-epsilon)) return LerpUnclamped(a, b2, t);

            float theta0 = MathF.Acos(dot);
            float theta = theta0 * t;
            float sinTheta = MathF.Sin(theta);
            float sinTheta0 = MathF.Sin(theta0);

            float s0 = MathF.Cos(theta) - dot * sinTheta / sinTheta0;
            float s1 = sinTheta / sinTheta0;

            return new Quat(
                s0 * a.x + s1 * b2.x,
                s0 * a.y + s1 * b2.y,
                s0 * a.z + s1 * b2.z,
                s0 * a.w + s1 * b2.w
            );
        }

        public static Vec3 ToEulerAngles(Quat rotation)
        {
            //Nose gracias unity
            return new Vec3(((Quaternion)rotation).eulerAngles);
        }
        #endregion

        #region Instance Functions
        public void Normalize()
        {
            this = Normalize(this);
        }

        public void Set(float newX, float newY, float newZ, float newW)
        {
            this.x = newX;
            this.y = newY;
            this.z = newZ;
            this.w = newW;
        }

        public void SetAxisAngle(Vec3 axis, float angle)
        {
            this = AngleAxis(angle, axis);
        }

        public void SetEulerAngles(Vec3 euler)
        {
            this = Euler(euler);
        }

        public void SetEulerRotation(Vec3 euler)
        {
            this = Euler(euler);
        }

        public void SetFromToRotation(Vec3 fromDirection, Vec3 toDirection)
        {
            this = FromToRotation(fromDirection, toDirection);
        }

        public void SetLookRotation(Vec3 view)
        {
            this = LookRotation(view);
        }

        public void SetLookRotation(Vec3 view, [DefaultValue("Vec3.up")] Vec3 up)
        {
            this = LookRotation(view, up);
        }

        public void ToAngleAxis(out float angle, out Vec3 axis)
        {
            float sqrMag = x * x + y * y + z * z;
            if (sqrMag > epsilon)
            {
                angle = 2f * MathF.Acos(w) * radToDeg;
                float invMag = 1f / MathF.Sqrt(sqrMag);
                axis = new Vec3(x * invMag, y * invMag, z * invMag);
            }
            else
            {
                angle = 0f;
                axis = Vec3.Right;
            }
        }

        public Vec3 ToEuler()
        {
            return eulerAngles;
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
            return x == other.x && y == other.y && z == other.z && w == other.w;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(x, y, z, w);
        }

        public override string ToString()
        {
            return $"({x:F5}, {y:F5}, {z:F5}, {w:F5})";
        }
        #endregion
    }
}