using System.Runtime.CompilerServices;
using UnityEngine;
using UPDB.CoreHelper.UsableMethods;

namespace UPDB.Physic.CustomForce
{
    /// <summary>
    /// help debugging and managing forces, like constant force, but with more options
    /// </summary>
    [AddComponentMenu("UPDB/Physics/CustomForce")]
    public class CustomForce : UPDBBehaviour
    {
        [Header("Actions")]
        [SerializeField, Tooltip("set global force(in m/s")]
        private bool _setGlobal = false;

        [SerializeField, Tooltip("set local force(in m/s")]
        private bool _setLocal = false;

        [SerializeField, Tooltip("add velocity(in m/s")]
        private bool _add = false;

        [SerializeField,  Tooltip("add force(weighted")]
        private bool _addForce = false;

        [SerializeField, Tooltip("set constant speed")]
        private bool _setConstantSpeed = false;

        [SerializeField, Tooltip("set constant speed")]
        private bool _setConstantAngularSpeed = false;

        [SerializeField, Tooltip("set constant speed")]
        private bool _setConstantLocalSpeed = false;

        [SerializeField, Tooltip("set constant speed")]
        private bool _setConstantLocalAngularSpeed = false;


        [Header("set once and constant set values")]
        [SerializeField, Tooltip("global speed")]
        private Vector3 _speed;

        [SerializeField, Tooltip("local speed")]
        private Vector3 _relativeSpeed;

        [SerializeField, Tooltip("global angular speed")]
        private Vector3 _torque;

        [SerializeField, Tooltip("local angular speed")]
        private Vector3 _relativeTorque;


        [Header("Constant Accelerations")]
        [SerializeField, Tooltip("global speed")]
        private Vector3 _speedAcceleration;

        [SerializeField, Tooltip("local speed")]
        private Vector3 _relativeSpeedAcceleration;

        [SerializeField, Tooltip("global angular speed")]
        private Vector3 _angularAcceleration;

        [SerializeField, Tooltip("local angular speed")]
        private Vector3 _relativeAngularAcceleration;


        [Header("Constant Forces")]
        [SerializeField, Tooltip("global speed")]
        private Vector3 _speedForce;

        [SerializeField, Tooltip("local speed")]
        private Vector3 _relativeSpeedForce;

        [SerializeField, Tooltip("global angular speed")]
        private Vector3 _torqueForce;

        [SerializeField, Tooltip("local angular speed")]
        private Vector3 _relativeTorqueForce;


        [Header("Constant Decelerations")]
        [SerializeField, Tooltip("global speed")]
        private float _speedDeceleration;

        [SerializeField, Tooltip("global angular speed")]
        private float _angularDeceleration;

        private Rigidbody _rb;

        void Awake()
        {
            MakeNonNullable(ref _rb, gameObject);
        }

        // Update is called once per frame
        void FixedUpdate()
        {
            if(_setConstantSpeed)
            {
                _rb.linearVelocity = _speed;
            }
            if (_setConstantAngularSpeed)
            {
                _rb.angularVelocity = _torque;
            }
            if (_setConstantLocalSpeed)
            {
                _rb.linearVelocity = transform.TransformDirection(_relativeSpeed);
            }
            if (_setConstantLocalAngularSpeed)
            {
                _rb.angularVelocity = transform.TransformDirection(_relativeTorque);
            }

            SetOnce();
            ConstantAcceleration();
            ConstantForce();
            ConstantDeceleration();
        }

        private void SetOnce()
        {
            if (_setConstantSpeed || _setConstantAngularSpeed || _setConstantLocalSpeed || _setConstantLocalAngularSpeed)
                return;

            if (_setGlobal)
            {
                _setGlobal = false;

                SetGlobal();
            }

            if (_setLocal)
            {
                _setLocal = false;

                SetLocal();
            }

            if (_add)
            {
                _add = false;

                Add();
            }

            if (_addForce)
            {
                _addForce = false;

                AddForce();
            }
        }

        private void ConstantAcceleration()
        {
            _rb.AddForce((_speedAcceleration * _rb.mass) * Time.fixedDeltaTime);
            _rb.AddRelativeForce((_relativeSpeedAcceleration * _rb.mass) * Time.fixedDeltaTime);
            _rb.AddTorque((_angularAcceleration * _rb.mass) * Time.fixedDeltaTime);
            _rb.AddRelativeTorque((_relativeAngularAcceleration * _rb.mass) * Time.fixedDeltaTime);
        }

        private void ConstantForce()
        {
            _rb.AddForce((_speedAcceleration) * Time.fixedDeltaTime);
            _rb.AddRelativeForce((_relativeSpeedAcceleration) * Time.fixedDeltaTime);
            _rb.AddTorque((_angularAcceleration) * Time.fixedDeltaTime);
            _rb.AddRelativeTorque((_relativeAngularAcceleration) * Time.fixedDeltaTime);
        }

        private void ConstantDeceleration()
        {
            _rb.linearVelocity = _rb.linearVelocity.normalized * (Mathf.Clamp(_rb.linearVelocity.magnitude - (_speedDeceleration * Time.fixedDeltaTime), 0, Mathf.Infinity));
            _rb.angularVelocity = _rb.angularVelocity.normalized * (Mathf.Clamp(_rb.angularVelocity.magnitude - (_angularDeceleration * Time.fixedDeltaTime), 0, Mathf.Infinity));
        }

        private void SetGlobal()
        {
            _rb.linearVelocity = _speed;
            _rb.angularVelocity = _torque;
        }

        private void SetLocal()
        {
            _rb.linearVelocity = transform.TransformDirection(_relativeSpeed);
            _rb.angularVelocity = transform.TransformDirection(_relativeTorque);
        }

        private void Add()
        {
            _rb.AddForce(_speed * _rb.mass);
            _rb.AddRelativeForce(_relativeSpeed * _rb.mass);
            _rb.AddTorque(_torque * _rb.mass);
            _rb.AddRelativeTorque(_relativeTorque * _rb.mass);
        }

        private void AddForce()
        {
            _rb.AddForce(_speed);
            _rb.AddRelativeForce(_relativeSpeed);
            _rb.AddTorque(_torque);
            _rb.AddRelativeTorque(_relativeTorque);
        }
    } 
}
