using System;
using Type.Utils;
using UnityEngine;

namespace Type
{


    public struct Vector2Byte : IEquatable<Vector2Byte>
    {
        private static readonly Vector2Byte zeroVector = new Vector2Byte(0, 0);
        public byte x { get; set; }
        public byte y { get; set; }

        public Vector2Byte(byte x, byte y)
        {
            this.x = x;
            this.y = y;
        }
        public override int GetHashCode()
        {
            return HashCode.Combine(x, y);
        }

        //
        public bool Equals(Vector2Byte other)
        {
            return x == other.x && y == other.y;
        }
        public override bool Equals(object obj)
        {
            return obj is Vector2Byte other && Equals(other);
        }
        public Vector2Byte Clump()
        {
            // byte는 0-255이므로, 1-255로 제한하기 위해 적용
            byte clampedX = (byte)Mathf.Clamp(this.x, 1, 255);
            byte clampedY = (byte)Mathf.Clamp(this.y, 1, 255);
            return new Vector2Byte(clampedX, clampedY);
        }


        public static implicit operator Vector2Byte(Vector2 target)
        {
            return new Vector2Byte((byte)target.x, (byte)target.y);
        }

        public static implicit operator Vector2Byte(Vector2Int target)
        {
            return new Vector2Byte((byte)target.x, (byte)target.y);
        }

        public static implicit operator Vector2(Vector2Byte target)
        {
            return new Vector2(target.x, target.y);
        }

        public static Vector2Byte operator /(Vector2Byte value, byte divisor)
        {
            return new Vector2Byte((byte)(value.x / divisor), (byte)(value.y / divisor));
        }
        public static Vector2Byte operator *(Vector2Byte value, float op)
        {
            return new Vector2Byte((byte)(value.x * op), (byte)(value.y * op));
        }

        public static bool operator ==(Vector2Byte a, Vector2Byte b)
        {
            return a.Equals(b);
        }

        public static bool operator !=(Vector2Byte a, Vector2Byte b)
        {
            return !a.Equals(b);
        }

        public static Vector2Byte operator +(Vector2Byte a, Vector2Byte b)
        {
            return new Vector2Byte((byte)(a.x + b.x), (byte)(a.y + b.y));
        }
        public static Vector2Byte operator +(Vector2Byte a, byte b)
        {
            return new Vector2Byte((byte)(a.x + b), (byte)(a.y + b));
        }
        public static Vector2Byte operator -(Vector2Byte a, Vector2Byte b)
        {
            return new Vector2Byte((byte)(a.x - b.x), (byte)(a.y - b.y));
        }
        public static Vector2Byte operator -(Vector2Byte a)
        {
            return new Vector2Byte((byte)(a.x * -1), (byte)(a.y * -1));
        }

        public static explicit operator Vector3(Vector2Byte v)
        {
            return new Vector3(v.x, v.y, 0);
        }

        public static Vector2Byte zero => zeroVector;
    }

    public struct Vector2SByte : IEquatable<Vector2SByte>
    {
        private static readonly Vector2SByte zeroVector = new Vector2SByte(0, 0);

        public sbyte x { get; set; }
        public sbyte y { get; set; }

        public Vector2SByte(sbyte x, sbyte y)
        {
            this.x = x;
            this.y = y;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(x, y);
        }

        public bool Equals(Vector2SByte other)
        {
            return x == other.x && y == other.y;
        }

        public override bool Equals(object obj)
        {
            return obj is Vector2SByte other && Equals(other);
        }

        public static implicit operator Vector2SByte(Vector2 target)
        {
            return new Vector2SByte((sbyte)target.x, (sbyte)target.y);
        }

        public static implicit operator Vector2SByte(Vector2Int target)
        {
            return new Vector2SByte((sbyte)target.x, (sbyte)target.y);
        }

        public static implicit operator Vector2(Vector2SByte target)
        {
            return new Vector2(target.x, target.y);
        }

        public static bool operator ==(Vector2SByte a, Vector2SByte b)
        {
            return a.Equals(b);
        }

        public static bool operator !=(Vector2SByte a, Vector2SByte b)
        {
            return !a.Equals(b);
        }

        public static Vector2SByte zero => zeroVector;
    }

    public struct MSConfig
    {
        public int seed;
        public Vector2Byte size;
        public byte diff;
    }
}