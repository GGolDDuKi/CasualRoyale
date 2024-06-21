using System;
using System.Collections.Generic;
using System.Text;

namespace Server.Define
{
    public struct Vector2
    {
        public float x;
        public float y;

        public Vector2(float x, float y) { this.x = x; this.y = y; }

        public static Vector2 Up { get { return new Vector2(0, 1); } }
        public static Vector2 Down { get { return new Vector2(0, -1); } }
        public static Vector2 Left { get { return new Vector2(-1, 0); } }
        public static Vector2 Right { get { return new Vector2(1, 0); } }

        public static Vector2 operator +(Vector2 a, Vector2 b)
        {
            return new Vector2(a.x + b.x, a.y + b.y);
        }

        public static Vector2 operator -(Vector2 a, Vector2 b)
        {
            return new Vector2(a.x - b.x, a.y - b.y);
        }

        public static bool operator ==(Vector2 a, Vector2 b)
        {
            if (a.x == b.x && a.y == b.y)
                return true;
            else
                return false;
        }

        public static bool operator !=(Vector2 a, Vector2 b)
        {
            if (a.x == b.x && a.y == b.y)
                return false;
            else
                return true;
        }

        public static Vector2 operator *(Vector2 a, float b)
        {
            return new Vector2(a.x * b, a.y * b);
        }

        public static Vector2 operator /(Vector2 a, float b)
        {
            return new Vector2(a.x / b, a.y / b);
        }

        public float magnitude { get { return (float)Math.Sqrt(sqrMagnitude); } }

        public float sqrMagnitude { get { return (x * x + y * y); } }

        public Vector2 normalized
        {
            get
            {
                float length = (float)Math.Sqrt(x * x + y * y);

                if (length == 0)
                {
                    return new Vector2(0, 0);
                }

                // 각 구성 요소를 길이로 나누어 정규화
                return new Vector2(x / length, y / length);
            }
        }

        public static Vector2 Lerp(Vector2 start, Vector2 end, float t)
        {
            return new Vector2(start.x + (end.x - start.x) * t, start.y + (end.y - start.y) * t);
        }
    }

    public struct BoxCollider2D
    {
        public float x;
        public float y;
        public float width;
        public float height;

        public BoxCollider2D(float x, float y, float width, float height)
        {
            this.x = x;
            this.y = y;
            this.width = width;
            this.height = height;
        }

        //충돌이면 true
        public static bool Collision(BoxCollider2D a, BoxCollider2D b)
        {
            return 
                !(
                b.x - (b.width / 2) > a.x + (a.width / 2) ||
                b.x + (b.width / 2) < a.x - (a.width / 2) ||
                b.y - (b.height / 2) > a.y + (a.height / 2) ||
                b.y + (b.height / 2) < a.y - (a.height / 2)
                );
        }
    }
}
