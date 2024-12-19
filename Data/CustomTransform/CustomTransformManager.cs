using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UPDB.CoreHelper;
using UPDB.CoreHelper.UsableMethods;

namespace UPDB.Data.CustomTransform
{
    [AddComponentMenu(NamespaceID.DataPath + "/" + NamespaceID.CustomTransform + "/UPDB Transform")]
    public class CustomTransformManager : UPDBBehaviour
    {
        [SerializeField, Tooltip("parent of simulated transform")]
        private Transform _parent;

        [SerializeField, Tooltip("position of transform")]
        private Vector3 _localPosition;

        [SerializeField, Tooltip("rotation of transform")]
        private Vector3 _localEulerAngles;

        [SerializeField, Tooltip("scale of transform")]
        private Vector3 _localScale = Vector3.one;

        [SerializeField, Tooltip("is constraints enabled for local scale ?")]
        private bool _proportionsConstraint = false;

        private bool _proportionsConstraintMemo = false;
        private Vector3 _lastregisteredProportions = Vector3.one;
        private Vector3 _localScaleMemo = Vector3.one;

        #region Public API

        public Vector3 LocalPosition
        {
            get => _localPosition;
            set => _localPosition = value;
        }
        public Vector3 LocalEulerAngles
        {
            get => _localEulerAngles;
            set => _localEulerAngles = value;
        }
        public Vector3 LocalScale
        {
            get
            {
                if (_proportionsConstraintMemo != _proportionsConstraint && _proportionsConstraint)
                {
                    if (_localScale.x == 0 && _localScale.y == 0 && _localScale.z == 0)
                        _lastregisteredProportions = Vector3.one;
                    else
                        _lastregisteredProportions = _localScale;

                    _localScaleMemo = _localScale;
                }

                _proportionsConstraintMemo = _proportionsConstraint;

                if (_proportionsConstraint)
                {
                    Vector3 currentScale = _localScale;

                    if (currentScale.x != _localScaleMemo.x)
                    {
                        float yFactor = (_lastregisteredProportions.y / _lastregisteredProportions.x);
                        float zFactor = (_lastregisteredProportions.z / _lastregisteredProportions.x);

                        _localScale = new Vector3(currentScale.x, currentScale.x * yFactor, currentScale.x * zFactor);
                    }
                    else if (currentScale.y != _localScaleMemo.y)
                    {
                        float xFactor = (_lastregisteredProportions.x / _lastregisteredProportions.y);
                        float zFactor = (_lastregisteredProportions.z / _lastregisteredProportions.y);

                        _localScale = new Vector3(currentScale.y * xFactor, currentScale.y, currentScale.y * zFactor);
                    }
                    else if (currentScale.z != _localScaleMemo.z)
                    {
                        float xFactor = (_lastregisteredProportions.x / _lastregisteredProportions.z);
                        float yFactor = (_lastregisteredProportions.y / _lastregisteredProportions.z);

                        _localScale = new Vector3(currentScale.z * xFactor, currentScale.z * yFactor, currentScale.z);
                    }

                    if (_localScale.x != 0 && _localScale.y != 0 && _localScale.z != 0)
                        _lastregisteredProportions = _localScale;

                    _localScaleMemo = _localScale;
                }

                return _localScale;
            }

            set => _localScale = value;
        }
        public bool ProportionsConstraint
        {
            get => _proportionsConstraint;
            set => _proportionsConstraint = value;
        }
        public Vector3 LastRegisteredproportions
        {
            get => _lastregisteredProportions;
            set
            {
                _lastregisteredProportions = value;
                _localScaleMemo = value;
            }
        }

        public Vector3 SelfPosition
        {
            get
            {
                return Point3LocalToWorld(_localPosition, Parent ? Parent.position : Vector3.zero, Right * Scale.x, Up * Scale.y, Forward * Scale.z);
            }
        }

        public Vector3 Position
        {
            get
            {
                return Point3LocalToWorld(_localPosition, Parent ? Parent.position : Vector3.zero, Parent.right * Scale.x, Parent.up * Scale.y, Parent.forward * Scale.z);
            }
        }
        public Vector3 Scale
        {
            get
            {
                Vector3 parentScale = Parent ? Parent.localScale : Vector3.one;

                return UPDBMath.VecTime(LocalScale, parentScale);
            }
        }
        public Transform Parent
        {
            get
            {
                return _parent ? _parent : _parent = transform;
            }

            set => _parent = value;
        }
        private Quaternion Rotation
        {
            get
            {
                return Quaternion.LookRotation(Forward, Up);
            }
        }
        public Vector3 Right
        {
            get
            {
                Vector3 right = Parent ? Parent.right : Vector3.right;
                Vector3 up = Parent ? Parent.up : Vector3.up;
                Vector3 forward = Parent ? Parent.forward : Vector3.forward;

                Vector3 x = RotateVector(right, right, _localEulerAngles.x);
                Vector3 y = RotateVector(x, up, _localEulerAngles.y);
                Vector3 z = RotateVector(y, forward, _localEulerAngles.z);

                return z;
            }
        }
        public Vector3 Up
        {
            get
            {
                Vector3 right = Parent ? Parent.right : Vector3.right;
                Vector3 up = Parent ? Parent.up : Vector3.up;
                Vector3 forward = Parent ? Parent.forward : Vector3.forward;

                Vector3 x = RotateVector(up, right, _localEulerAngles.x);
                Vector3 y = RotateVector(x, up, _localEulerAngles.y);
                Vector3 z = RotateVector(y, forward, _localEulerAngles.z);

                return z;
            }
        }
        public Vector3 Forward
        {
            get
            {
                Vector3 right = Parent ? Parent.right : Vector3.right;
                Vector3 up = Parent ? Parent.up : Vector3.up;
                Vector3 forward = Parent ? Parent.forward : Vector3.forward;

                Vector3 x = RotateVector(forward, right, _localEulerAngles.x);
                Vector3 y = RotateVector(x, up, _localEulerAngles.y);
                Vector3 z = RotateVector(y, forward, _localEulerAngles.z);

                return z;
            }
        }

        public delegate void OnChangeCallback();
        public event OnChangeCallback OnChange;

        #endregion

        public void OnChangeEventInvoke()
        {
            OnChange?.Invoke();
        }
    }
}
