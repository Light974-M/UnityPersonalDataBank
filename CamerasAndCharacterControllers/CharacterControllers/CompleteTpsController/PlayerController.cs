using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// Controller of player movements and actions
/// </summary>
public class PlayerController : MonoBehaviour
{
    [SerializeField, Tooltip("character controller component used to move player")]
    private CharacterController _controller;

    [SerializeField, Tooltip("GameObject that represent player components in one parent object")]
    private Transform _playerComponentsPivot;

    [SerializeField, Tooltip("speed of player")]
    private Vector2 _speed = Vector2.zero;

    [SerializeField, Tooltip("avoid player to instant break velocity, 0 is disabled, 1 is permanent speed")]
    [Range(0f, 1f)]
    private float _smoothVelocityFallDown = 0;

    [SerializeField, Tooltip("set player rotation mode while walking")]
    private PlayerRotationMode _rotationMode;

    [SerializeField, Tooltip("determine time for player to reach max fall speed")]
    private float _fallDownHardeness = 1;

    [SerializeField, Tooltip("speed of player to turn arround")]
    private float _rotationSpeed = 1;

    private Vector2 _inputValue = Vector2.zero;
    private bool _isGrounded = false;
    private float _timer = 0;

    private float _normalizedInputMagnitudeMemo = 0;
    private Vector2 _inputValueNotNullable = Vector2.zero;

    private float _inputLerpTimer = 0;
    private Vector2 _lerpStart = Vector2.zero;
    private Vector2 _smoothedInputValue = Vector2.zero;

    #region Public API

    public enum PlayerRotationMode
    {
        Free,
        Clamped,
        SmoothlyClamped,
    }

    #endregion

    /// <summary>
    /// awake is called when script instance is being loaded
    /// </summary>
    private void Awake()
    {
        Init();

        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    /// <summary>
    /// Update is called once per frame
    /// </summary>
    private void Update()
    {

    }

    /// <summary>
    /// FixedUpdate is called once per Physics Update
    /// </summary>
    private void FixedUpdate()
    {
        if (!GameManager.Instance.IsPaused && !GameManager.Instance.IsGameOver)
        {
            LookDir();
            Move();
            GravityApply();

            _isGrounded = Physics.CheckSphere(new Vector3(transform.position.x, transform.position.y - 0.5f, transform.position.z), 0.5f);
        }
    }

    /// <summary>
    /// OnDrawGizmos is called each time scene refresh
    /// </summary>
    private void OnDrawGizmos()
    {
        if (!Application.isPlaying)
            Init();
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(new Vector3(transform.position.x, transform.position.y - 0.5f, transform.position.z), 0.5f);
        Gizmos.color = Color.white;
    }

    /// <summary>
    /// init is called in editor mode and at awake, to ensure every variables and states are initialized and set
    /// </summary>
    private void Init()
    {
        if (_controller == null)
            if (!TryGetComponent(out _controller))
                _controller = gameObject.AddComponent<CharacterController>();

        if (_playerComponentsPivot == null)
        {
            for (int i = 0; i < transform.childCount; i++)
                if (transform.GetChild(i).name == "PlayerComponentsPivot")
                    _playerComponentsPivot = transform.GetChild(i);

            if (_playerComponentsPivot == null)
                _playerComponentsPivot = new GameObject("PlayerComponentsPivot").transform.parent = transform;
        }
    }

    /// <summary>
    /// manage where player turn arround(affect movements direction !)
    /// </summary>
    private void LookDir()
    {
        if (_inputValue.magnitude != 0)
        {
            _smoothedInputValue = AutoLerp(_lerpStart, _inputValue, _rotationSpeed, ref _inputLerpTimer);

            PreventRotationFromClipping(ref _smoothedInputValue);

            transform.LookAt(new Vector3(Camera.main.transform.position.x, transform.position.y, Camera.main.transform.position.z));
            transform.eulerAngles += new Vector3(0, 180, 0);

            if (_rotationMode == PlayerRotationMode.Free)
            {
                Vector3 dir = transform.forward * _smoothedInputValue.y + transform.right * _smoothedInputValue.x;
                Vector3 position = transform.position + dir;
                _playerComponentsPivot.LookAt(position);
            }
            else if (_rotationMode == PlayerRotationMode.Clamped)
                _playerComponentsPivot.localEulerAngles = Vector3.zero;
            else if (_rotationMode == PlayerRotationMode.SmoothlyClamped)
                _playerComponentsPivot.localRotation = Quaternion.Lerp(_playerComponentsPivot.localRotation, new Quaternion(_playerComponentsPivot.localRotation.x, _inputValue.x * 0.5f, _playerComponentsPivot.localRotation.z, _playerComponentsPivot.localRotation.w), 0.2f);
        }
    }

    /// <summary>
    /// manage movements of player
    /// </summary>
    private void Move()
    {
        float normalizedInputMagnitude = Mathf.Clamp(_inputValue.magnitude, -1, 1);

        if (_normalizedInputMagnitudeMemo > normalizedInputMagnitude)
            normalizedInputMagnitude = Mathf.Lerp(normalizedInputMagnitude, _normalizedInputMagnitudeMemo, _smoothVelocityFallDown);

        _normalizedInputMagnitudeMemo = normalizedInputMagnitude;
        if (_inputValue != Vector2.zero)
            _inputValueNotNullable = _inputValue;

        Vector3 dir = ((transform.forward * _inputValueNotNullable.y) + (transform.right * _inputValueNotNullable.x)).normalized;
        _controller.Move(new Vector3(dir.x * _speed.x, dir.y, dir.z * _speed.y) * normalizedInputMagnitude * Time.fixedDeltaTime);
    }

    /// <summary>
    /// apply gravity to character controller
    /// </summary>
    private void GravityApply()
    {
        if (!_controller.isGrounded)
        {
            _controller.Move(Physics.gravity * Time.fixedDeltaTime * _timer);

            if (_timer < 1)
                _timer += Time.fixedDeltaTime / _fallDownHardeness;
            else
                _timer = 1;
        }
        else
        {
            _timer = 0;
        }

    }

    /// <summary>
    /// make a lerp automatically(based on delta time update rate) with Vector2, between a and b, in a specific time
    /// </summary>
    /// <param name="a">start point</param>
    /// <param name="b">end point</param>
    /// <param name="lerpTime">time to make lerp</param>
    /// <param name="timer">timer to store lerp progression</param>
    /// <returns></returns>
    private Vector2 AutoLerp(Vector2 a, Vector2 b, float lerpTime, ref float timer)
    {
        Vector2 value = Vector2.zero;

        if (timer < lerpTime)
        {
            value = Vector2.Lerp(a, b, timer / lerpTime);
            timer += Time.deltaTime;
        }
        else
        {
            timer = lerpTime;
            value = b;
        }

        return value;
    }

    private void PreventRotationFromClipping(ref Vector2 input)
    {
        if (Mathf.Abs(input.x) <= 0.05f || Mathf.Abs(input.y) <= 0.05f)
        {
            if (Mathf.Abs(input.x) < Mathf.Abs(input.y))
                input.x = AvoidClipping(input.x, input.y);
            else
                input.y = AvoidClipping(input.y, input.x);


            float AvoidClipping(float inputToChange, float otherInput)
            {
                if (inputToChange >= 0)
                    otherInput = Mathf.Abs(otherInput);
                else
                    otherInput = -Mathf.Abs(otherInput);

                inputToChange = -Mathf.Pow(otherInput, 2) + 1;

                return inputToChange;
            }
        }


    }

    #region Input System Functions

    /// <summary>
    /// Call by Input System, manage basic movements of player
    /// </summary>
    public void GetMove(InputAction.CallbackContext callback)
    {
        if (_inputValue.normalized != callback.ReadValue<Vector2>().normalized)
        {
            _inputLerpTimer = 0;
            _lerpStart = _smoothedInputValue;
        }

        _inputValue = callback.ReadValue<Vector2>();
    }


    #endregion
}
