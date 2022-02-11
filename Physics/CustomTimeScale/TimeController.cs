using System.Collections.Generic;
using UnityEngine;

namespace UPDB.physic.TimeController
{
    ///<summary>
    /// allow full control on time axis
    ///</summary>
    [AddComponentMenu("Game/TimeController")]
    public class TimeController : MonoBehaviour
    {

        [SerializeField, Tooltip("")]
        private Rigidbody _rb;

        [SerializeField, Range(-10, 10), Tooltip("")]
        private float _timeScale = 1;

        [SerializeField, Tooltip("")]
        private Vector3 _gravityScale = Vector3.zero;

        [SerializeField, Tooltip("")]
        private float _listDefilFrequency = 1;

        private float _memoTimeScale = 0;
        private Vector3 _memoVelocity = Vector3.one;
        private Vector3 _memoAngularVelocity = Vector3.one;

        private List<Vector3> _posList = new List<Vector3>();
        private List<Quaternion> _rotList = new List<Quaternion>();

        private float _timer = 10000;
        private float _defilTimer = 0;
        private int i = 0;
        private Vector3 posToGo = Vector3.zero;
        private Quaternion rotToGo = Quaternion.Euler(0, 0, 0);

        private void Awake()
        {
            if (_rb == null)
                if (!TryGetComponent(out _rb))
                {
                    _rb = gameObject.AddComponent<Rigidbody>();
                    _rb.useGravity = false;
                }

            _posList.Clear();

            _memoTimeScale = _timeScale;
        }

        private void Start()
        {
        }

        private void FixedUpdate()
        {
            if (_timeScale > 0)
            {
                Vector3 gravity = (_gravityScale * _rb.mass) * _timeScale;
                _rb.AddForce(gravity);
            }
        }

        private void Update()
        {
            if (_timeScale >= 0)
            {
                if (_timeScale == 0)
                    _rb.constraints = RigidbodyConstraints.FreezeAll;
                else
                    _rb.constraints = RigidbodyConstraints.None;

                if (_memoTimeScale != _timeScale)
                {
                    if (_timeScale == 0)
                    {
                        _memoVelocity = _rb.velocity;
                        _memoAngularVelocity = _rb.angularVelocity;
                    }

                    if (_memoTimeScale == 0)
                    {
                        _rb.velocity = _memoVelocity * _timeScale;
                        _rb.angularVelocity = _memoAngularVelocity * _timeScale;
                    }

                    if (_timeScale != 0)
                    {
                        _memoVelocity = _rb.velocity;
                        _memoAngularVelocity = _rb.angularVelocity;
                    }

                    if (_memoTimeScale != 0)
                    {
                        _rb.velocity = _rb.velocity * (_timeScale / _memoTimeScale);
                        _rb.angularVelocity = _rb.angularVelocity * (_timeScale / _memoTimeScale);
                    }
                }

                SaveState();

                i = _posList.Count - 1;
            }
            else
            {
                _rb.constraints = RigidbodyConstraints.FreezeAll;

                if (i > 1)
                {
                    if (_defilTimer >= (1 / _listDefilFrequency) / -_timeScale)
                    {
                        posToGo = _posList[i];
                        rotToGo = _rotList[i];

                        _posList.Remove(_posList[i]);
                        _rotList.Remove(_rotList[i]);
                        i--;
                        _defilTimer = 0;
                    }
                }

                transform.position = Vector3.Lerp(_posList[i], _posList[i - 1], _defilTimer * _listDefilFrequency);
                transform.rotation = Quaternion.Lerp(_rotList[i], _rotList[i - 1], _defilTimer * _listDefilFrequency);

                _defilTimer += Time.deltaTime;
            }

            _memoTimeScale = _timeScale;
            _timer += Time.deltaTime;
        }

        private void SaveState()
        {
            if (_timeScale > 0)
            {
                if (_timer >= (1 / _listDefilFrequency) / _timeScale)
                {
                    if(_posList.Count != 0 && _rotList.Count != 0)
                    {
                        //ne capture pas si immobile(donc décale par rapport aux autres objets
                        //if (transform.position != _posList[_posList.Count - 1] || transform.rotation != _rotList[_posList.Count - 1])
                        //{
                            
                        //}

                        _posList.Add(transform.position);
                        _rotList.Add(transform.rotation);
                        _timer = 0;
                    }
                    else
                    {
                        _posList.Add(transform.position);
                        _rotList.Add(transform.rotation);
                        _timer = 0;
                    }
                }
            }
        }
    }
}
