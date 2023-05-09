using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// represent a key point in spline 
/// </summary>
[System.Serializable]
public class KeyPoint
{
    /*****************************************************POSITION**************************************************************/
    [Space, Header("POSITION"), Space]

    [SerializeField, Tooltip("position of key")]
    private Vector3 _keyPosition;

    [SerializeField, Tooltip("shape in x dimension")]
    private AnimationCurve _xCurve;

    [SerializeField, Tooltip("shape in y dimension")]
    private AnimationCurve _yCurve;


    /*****************************************************ROTATION**************************************************************/
    [Space, Header("ROTATION"), Space]

    [SerializeField, Tooltip("quaternion rotation of key")]
    private Quaternion _keyRotation;

    [SerializeField, Tooltip("choose rotation mode to overwrite rotation keys values")]
    private RotationMode _rotationOverwrite = RotationMode.FollowRotation;

    [SerializeField, Tooltip("time for rotation to lerp to next rotation")]
    private float _rotationLerpTime = 1;

    [SerializeField, Tooltip("shape of lerp trough value to smooth transition")]
    private AnimationCurve _rotationLerpShape;

    /*****************************************************MOVEMENTS**************************************************************/
    [Space, Header("MOVEMENTS"), Space]

    [SerializeField, Tooltip("speed of target in current key segment")]
    private float _speed = 1;

    [SerializeField, Tooltip("time to pass to a speed to another")]
    private float _speedLerpTime = 1;

    [SerializeField, Tooltip("give a time to pause movements")]
    private Vector2 _pauseTime = Vector2.zero;

    #region Public API

    public Vector3 KeyPosition
    {
        get => _keyPosition;
        set => _keyPosition = value;
    }
    public AnimationCurve XCurve
    {
        get => _xCurve;
        set => _xCurve = value;
    }
    public AnimationCurve YCurve
    {
        get => _yCurve;
        set => _yCurve = value;
    }

    public Quaternion KeyRotation
    {
        get => _keyRotation;
        set => _keyRotation = value;
    }
    public RotationMode RotationOverwrite => _rotationOverwrite;
    public float RotationLerpTime => _rotationLerpTime;
    public AnimationCurve RotationLerpShape
    {
        get => _rotationLerpShape;
        set => _rotationLerpShape = value;
    }
    public float Speed => _speed;
    public float SpeedLerpTime => _speedLerpTime;
    public Vector2 PauseTime => _pauseTime;

    #endregion

    public KeyPoint(Vector3 keyPosition, Quaternion keyRotation)
    {
        _keyPosition = keyPosition;
        _keyRotation = keyRotation;
    }

    public KeyPoint()
    {
        _keyPosition = Vector3.zero;
        _keyRotation = Quaternion.identity;
    }
}

/// <summary>
/// choose between free rotation, rotation following rail, or rotation traightly aiming next key
/// </summary>
public enum RotationMode
{
    FollowRotation,
    StraightRotation,
    FreeRotation,
}
