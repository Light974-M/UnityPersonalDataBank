using System;
using System.Globalization;
using UnityEngine;

namespace UPDB.CoreHelper.UsableMethods.Structures
{
    /// <summary>
    /// A 2D Rectangle defined by X and Y position, width and height.
    /// </summary>
    [System.Serializable]
    public struct Rect2
    {
        #region Serialized API

        [SerializeField, Tooltip("x min bound of rect")]
        private float _xMin;

        [SerializeField, Tooltip("y min bound of rect")]
        private float _yMin;

        [SerializeField, Tooltip("width of rect")]
        private float _width;

        [SerializeField, Tooltip("height of rect")]
        private float _height;

        #endregion

        #region Static API

        /// <summary>
        /// Shorthand for writing new Rect(0,0,0,0).
        /// </summary>
        public static Rect2 zero => new Rect2(0f, 0f, 0f, 0f);

        /// <summary>
        /// Shorthand for writing new Rect(0,0,1,1).
        /// </summary>
        public static Rect2 one => new Rect2(0f, 0f, 1f, 1f);

        #endregion

        #region Public API

        //Base Accessor
        /// <summary>
        /// The X coordinate of the rectangle.
        /// </summary>
        public float x
        {
            get
            {
                return _xMin;
            }
            set
            {
                _xMin = value;
            }
        }

        /// <summary>
        ///  The Y coordinate of the rectangle.
        /// </summary>
        public float y
        {
            get
            {
                return _yMin;
            }
            set
            {
                _yMin = value;
            }
        }

        /// <summary>
        /// The width of the rectangle, measured from the X position.
        /// </summary>
        public float width
        {
            get
            {
                return _width;
            }
            set
            {
                _width = value;
            }
        }

        /// <summary>
        /// The height of the rectangle, measured from the Y position.
        /// </summary>
        public float height
        {
            get
            {
                return _height;
            }
            set
            {
                _height = value;
            }
        }

        //min and max accessors, changing other values dynamically on set
        /// <summary>
        /// The minimum X coordinate of the rectangle.
        /// </summary>
        public float xMin
        {
            get
            {
                return _xMin;
            }
            set
            {
                float num = xMax;
                _xMin = value;
                _width = num - _xMin;
                OrderMinMax(this);
            }
        }

        /// <summary>
        /// The minimum Y coordinate of the rectangle.
        /// </summary>
        public float yMin
        {
            get
            {
                return _yMin;
            }
            set
            {
                float num = yMax;
                _yMin = value;
                _height = num - _yMin;
                OrderMinMax(this);
            }
        }

        /// <summary>
        /// The maximum X coordinate of the rectangle.
        /// </summary>
        public float xMax
        {
            get
            {
                return _width + _xMin;
            }
            set
            {
                _width = value - _xMin;
                OrderMinMax(this);
            }
        }

        /// <summary>
        /// The maximum Y coordinate of the rectangle.
        /// </summary>
        public float yMax
        {
            get
            {
                return _height + _yMin;
            }
            set
            {
                _height = value - _yMin;
                OrderMinMax(this);
            }
        }

        //Vector2 overrides and utility
        /// <summary>
        /// The X and Y position of the rectangle.
        /// </summary>
        public Vector2 position
        {
            get
            {
                return new Vector2(_xMin, _yMin);
            }
            set
            {
                _xMin = value.x;
                _yMin = value.y;
            }
        }

        /// <summary>
        /// The position of the center of the rectangle.
        /// </summary>
        public Vector2 center
        {
            get
            {
                return new Vector2(x + _width / 2f, y + _height / 2f);
            }
            set
            {
                _xMin = value.x - _width / 2f;
                _yMin = value.y - _height / 2f;
            }
        }

        /// <summary>
        /// The position of the minimum corner of the rectangle.
        /// </summary>
        public Vector2 min
        {
            get
            {
                return new Vector2(xMin, yMin);
            }
            set
            {
                xMin = value.x;
                yMin = value.y;
            }
        }

        /// <summary>
        /// The position of the maximum corner of the rectangle.
        /// </summary>
        public Vector2 max
        {
            get
            {
                return new Vector2(xMax, yMax);
            }
            set
            {
                xMax = value.x;
                yMax = value.y;
            }
        }

        /// <summary>
        /// The width and height of the rectangle.
        /// </summary>
        public Vector2 size
        {
            get
            {
                return new Vector2(_width, _height);
            }
            set
            {
                _width = value.x;
                _height = value.y;
            }
        }

        #endregion

        #region Constructors

        /// <summary>
        /// Creates a new rectangle.
        /// </summary>
        /// <param name="x">The X value the rect is measured from.</param>
        /// <param name="y">The Y value the rect is measured from.</param>
        /// <param name="width">The width of the rectangle.</param>
        /// <param name="height">The height of the rectangle.</param>
        public Rect2(float x, float y, float width, float height)
        {
            _xMin = x;
            _yMin = y;
            _width = width;
            _height = height;
        }

        /// <summary>
        /// Creates a rectangle given a size and position.
        /// </summary>
        /// <param name="position"> The position of the minimum corner of the rect.</param>
        /// <param name="size">The width and height of the rect.</param>
        public Rect2(Vector2 position, Vector2 size)
        {
            _xMin = position.x;
            _yMin = position.y;
            _width = size.x;
            _height = size.y;
        }

        /// <summary>
        /// create a rectangle given another rectangle
        /// </summary>
        /// <param name="source">the rectangle to read from</param>
        public Rect2(Rect2 source)
        {
            _xMin = source._xMin;
            _yMin = source._yMin;
            _width = source._width;
            _height = source._height;
        }

        #endregion

        #region Custom Operators

        public static bool operator !=(Rect2 lhs, Rect2 rhs)
        {
            return !(lhs == rhs);
        }

        public static bool operator ==(Rect2 lhs, Rect2 rhs)
        {
            return lhs.x == rhs.x && lhs.y == rhs.y && lhs.width == rhs.width && lhs.height == rhs.height;
        }

        #endregion

        /// <summary>
        /// Creates a rectangle from min/max coordinate values.
        /// </summary>
        /// <param name="xmin">The minimum X coordinate.</param>
        /// <param name="ymin">The minimum Y coordinate.</param>
        /// <param name="xmax">The maximum X coordinate.</param>
        /// <param name="ymax">The maximum Y coordinate.</param>
        /// <returns>A rectangle matching the specified coordinates.</returns>
        public static Rect2 MinMaxRect(float xmin, float ymin, float xmax, float ymax)
        {
            return new Rect2(xmin, ymin, xmax - xmin, ymax - ymin);
        }


        /*************************************METHODS UTILITY*************************************/

        /// <summary>
        /// Set components of an existing Rect.
        /// </summary>
        /// <param name="x">value to set to x</param>
        /// <param name="y">value to set to y</param>
        /// <param name="width">value to set to width</param>
        /// <param name="height">value to set to height</param>
        public void Set(float x, float y, float width, float height)
        {
            _xMin = x;
            _yMin = y;
            _width = width;
            _height = height;
        }

        /// <summary>
        /// Returns true if the x and y components of point is a point inside this rectangle.
        /// If allowInverse is present and true, the width and height of the Rect are allowed
        /// to take negative values (ie, the min value is greater than the max), and the
        /// test will still work.
        /// </summary>
        /// <param name="point">Point to test.</param>
        /// <returns>True if the point lies within the specified rectangle.</returns>
        public bool Contains(Vector2 point)
        {
            return point.x >= xMin && point.x < xMax && point.y >= yMin && point.y < yMax;
        }

        /// <summary>
        /// Returns true if the x and y components of point is a point inside this rectangle.
        /// If allowInverse is present and true, the width and height of the Rect are allowed
        /// to take negative values (ie, the min value is greater than the max), and the
        /// test will still work.
        /// </summary>
        /// <param name="point">Point to test.</param>
        /// <returns>True if the point lies within the specified rectangle.</returns>
        public bool Contains(Vector3 point)
        {
            return point.x >= xMin && point.x < xMax && point.y >= yMin && point.y < yMax;
        }

        /// <summary>
        /// Returns true if the x and y components of point is a point inside this rectangle.
        /// If allowInverse is present and true, the width and height of the Rect are allowed
        /// to take negative values (ie, the min value is greater than the max), and the
        /// test will still work.
        /// </summary>
        /// <param name="point">Point to test.</param>
        /// <param name="allowInverse">Does the test allow the Rect's width and height to be negative?</param>
        /// <returns>True if the point lies within the specified rectangle.</returns>
        public bool Contains(Vector3 point, bool allowInverse)
        {
            if (!allowInverse)
            {
                return Contains(point);
            }

            bool flag = (width < 0f && point.x <= xMin && point.x > xMax) || (width >= 0f && point.x >= xMin && point.x < xMax);
            bool flag2 = (height < 0f && point.y <= yMin && point.y > yMax) || (height >= 0f && point.y >= yMin && point.y < yMax);
            return flag && flag2;
        }

        /// <summary>
        /// call if you think min is superior to max in any direction (wich shouldn't be the case) to order everything
        /// </summary>
        /// <param name="rect">the rect to order</param>
        /// <returns>the ordered rect, inversing min and max values if they're not in order</returns>
        private static Rect2 OrderMinMax(Rect2 rect)
        {
            if (rect.xMin > rect.xMax)
            {
                float num = rect.xMin;
                rect.xMin = rect.xMax;
                rect.xMax = num;
            }

            if (rect.yMin > rect.yMax)
            {
                float num2 = rect.yMin;
                rect.yMin = rect.yMax;
                rect.yMax = num2;
            }

            return rect;
        }

        /// <summary>
        /// Returns true if the other rectangle overlaps this one. If allowInverse is present
        /// and true, the widths and heights of the Rects are allowed to take negative values
        /// (ie, the min value is greater than the max), and the test will still work.
        /// </summary>
        /// <param name="other">Other rectangle to test overlapping with.</param>
        /// <returns>if rect overlap with this rect, meaning if they're "touching" in any case</returns>
        public bool Overlaps(Rect2 other)
        {
            return other.xMax > xMin && other.xMin < xMax && other.yMax > yMin && other.yMin < yMax;
        }

        /// <summary>
        /// Returns true if the other rectangle overlaps this one. If allowInverse is present
        /// and true, the widths and heights of the Rects are allowed to take negative values
        /// (ie, the min value is greater than the max), and the test will still work.
        /// </summary>
        /// <param name="other">Other rectangle to test overlapping with.</param>
        /// <param name="allowInverse">Does the test allow the widths and heights of the Rects to be negative?</param>
        /// <returns>if rect overlap with this rect, meaning if they're "touching" in any case</returns>
        public bool Overlaps(Rect2 other, bool allowInverse)
        {
            Rect2 rect = this;
            if (allowInverse)
            {
                rect = OrderMinMax(rect);
                other = OrderMinMax(other);
            }

            return rect.Overlaps(other);
        }

        /// <summary>
        /// Returns a point inside a rectangle, given normalized coordinates.
        /// </summary>
        /// <param name="rectangle">Rectangle to get a point inside.</param>
        /// <param name="normalizedRectCoordinates">Normalized coordinates to get a point for.</param>
        /// <returns>the normalized vector between rectangle bounds</returns>
        public static Vector2 NormalizedToPoint(Rect2 rectangle, Vector2 normalizedRectCoordinates)
        {
            return new Vector2(Mathf.Lerp(rectangle.x, rectangle.xMax, normalizedRectCoordinates.x), Mathf.Lerp(rectangle.y, rectangle.yMax, normalizedRectCoordinates.y));
        }

        /// <summary>
        /// Returns the normalized coordinates cooresponding the the point.
        /// </summary>
        /// <param name="rectangle">Rectangle to get normalized coordinates inside.</param>
        /// <param name="point">A point inside the rectangle to get normalized coordinates for.</param>
        /// <returns>the point between 0 and 1 wich leads to point</returns>
        public static Vector2 PointToNormalized(Rect2 rectangle, Vector2 point)
        {
            return new Vector2(Mathf.InverseLerp(rectangle.x, rectangle.xMax, point.x), Mathf.InverseLerp(rectangle.y, rectangle.yMax, point.y));
        }


        #region UnityEngin Base Utility

        public override int GetHashCode()
        {
            return x.GetHashCode() ^ (width.GetHashCode() << 2) ^ (y.GetHashCode() >> 2) ^ (height.GetHashCode() >> 1);
        }

        public override bool Equals(object other)
        {
            if (!(other is Rect2))
            {
                return false;
            }

            return Equals((Rect2)other);
        }

        public bool Equals(Rect2 other)
        {
            return x.Equals(other.x) && y.Equals(other.y) && width.Equals(other.width) && height.Equals(other.height);
        } 

        #endregion
    }
}
