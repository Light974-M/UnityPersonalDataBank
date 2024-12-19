using UnityEngine;

namespace UPDB.CoreHelper.UsableMethods.Structures
{
    public struct Vec2
    {
        #region Private API

        private float _x;
        private float _y;

        #endregion

        #region Public API

        public float x
        {
            get { return _x; } 
            set { _x = value; }
        }

        public float y
        {
            get { return _y; }
            set { _y = value; }
        }

        #endregion

        #region Static API

        public static Vec2 zero
        {
            get
            {
                return new Vec2(0, 0);
            }
        }

        public static Vec2 one
        {
            get
            {
                return new Vec2(1, 1);
            }
        }

        public static Vec2 up
        {
            get
            {
                return new Vec2(0, 1);
            }
        }

        public static Vec2 right
        {
            get
            {
                return new Vec2(1, 0);
            }
        }

        #endregion

        #region Constructors

        public Vec2(float x, float y)
        {
            _x = x;
            _y = y;
        }

        public Vec2(float x)
        {
            _x = x;
            _y = 0;
        }

        #endregion

        #region operators

        /***************VEC TO VEC OPERATIONS******************/

        public static Vec2 operator +(Vec2 lhs, Vec2 rhs)
        {
            return new Vec2(lhs.x + rhs.x, lhs.y + rhs.y);
        }

        public static Vec2 operator -(Vec2 lhs, Vec2 rhs)
        {
            return new Vec2(lhs.x - rhs.x, lhs.y - rhs.y);
        }

        public static Vec2 operator *(Vec2 lhs, Vec2 rhs)
        {
            return new Vec2(lhs.x * rhs.x, lhs.y * rhs.y);
        }

        public static Vec2 operator /(Vec2 lhs, Vec2 rhs)
        {
            return new Vec2(rhs.x == 0 ? Mathf.Infinity : lhs.x / rhs.x, rhs.y == 0 ? Mathf.Infinity : lhs.y / rhs.y);
        }

        /***************VEC TO FLOAT OPERATIONS******************/

        public static Vec2 operator +(Vec2 lhs, float rhs)
        {
            return new Vec2(lhs.x + rhs, lhs.y + rhs);
        }

        public static Vec2 operator -(Vec2 lhs, float rhs)
        {
            return new Vec2(lhs.x - rhs, lhs.y - rhs);
        }

        public static Vec2 operator *(Vec2 lhs, float rhs)
        {
            return new Vec2(lhs.x * rhs, lhs.y * rhs);
        }

        public static Vec2 operator /(Vec2 lhs, float rhs)
        {
            return new Vec2(rhs == 0 ? Mathf.Infinity : lhs.x / rhs, rhs == 0 ? Mathf.Infinity : lhs.y / rhs);
        }

        #endregion

        #region Methods



        #endregion
    }
}
