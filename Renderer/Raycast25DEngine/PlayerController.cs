using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;
using UPDB.CoreHelper.UsableMethods;

namespace UPDB.Renderers.Raycast25DEngine
{
    public class PlayerController : UPDBBehaviour
    {
        [SerializeField]
        private float _speed = 5;

        [SerializeField]
        private float _rotationSpeed = 5;

        [SerializeField]
        private int _verticalRotationSpeed = 5;

        [Header("Raycast Parameters")]
        [SerializeField]
        private float _fieldOfView = 45;

        [SerializeField]
        private int _rayNumbers = 100;

        [SerializeField]
        private float _viewDistance = 15;

        [SerializeField]
        private int _verticalPixelNumbers = 1080;

        [SerializeField]
        private LayerMask _rayLayerMask;

        [SerializeField]
        private int _verticalLookingValue = 0;

        private List<RaycastHit2D> _raysList = new List<RaycastHit2D>();

        private Rigidbody2D _rb;

        public List<RaycastHit2D> RaysList
        {
            get => _raysList;
            set => _raysList = value;
        }

        public int VerticalPixelNumbers
        {
            get => _verticalPixelNumbers;
            set => _verticalPixelNumbers = value;
        }

        public int RayNumbers => _rayNumbers;

        public int VerticalLookingValue
        {
            get => _verticalLookingValue;
        }

        private void Awake()
        {
            MakeNonNullable(ref _rb, gameObject);
        }

        private void FixedUpdate()
        {
            if (Input.GetKey(KeyCode.W))
            {
                _rb.position += new Vector2(transform.up.x, transform.up.y) * _speed * Time.fixedDeltaTime;
            }

            if (Input.GetKey(KeyCode.S))
            {
                _rb.position -= new Vector2(transform.up.x, transform.up.y) * _speed * Time.fixedDeltaTime;
            }

            if (Input.GetKey(KeyCode.A))
            {
                _rb.position -= new Vector2(transform.right.x, transform.right.y) * _speed * Time.fixedDeltaTime;
            }

            if (Input.GetKey(KeyCode.D))
            {
                _rb.position += new Vector2(transform.right.x, transform.right.y) * _speed * Time.fixedDeltaTime;
            }

            if (Input.GetKey(KeyCode.LeftArrow))
            {
                transform.Rotate(transform.forward * _rotationSpeed * Time.fixedDeltaTime);
            }

            if (Input.GetKey(KeyCode.RightArrow))
            {
                transform.Rotate(-transform.forward * _rotationSpeed * Time.fixedDeltaTime);
            }

            if (Input.GetKey(KeyCode.UpArrow))
            {
                _verticalLookingValue += _verticalRotationSpeed;
            }

            if (Input.GetKey(KeyCode.DownArrow))
            {
                _verticalLookingValue -= _verticalRotationSpeed;
            }

            _raysList.Clear();
            float angleRotate = _rayNumbers > 1 ? -_fieldOfView / 2f : 0;

            for (int i = 0; i < _rayNumbers; i++)
            {
                RaycastHit2D hit = Physics2D.Raycast(transform.position, RotateVector(transform.up, angleRotate), _viewDistance, _rayLayerMask);

                _raysList.Add(hit);

                Debug.DrawRay(transform.position, RotateVector(transform.up, angleRotate) * (hit ? hit.distance : _viewDistance), Color.blue);

                if (_rayNumbers > 1)
                    angleRotate += _fieldOfView / (float)(_rayNumbers - 1);
            }
        }
    }
}
