using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AutoRotate : MonoBehaviour
{
    //SPEED MANAGER

    //notice : global and local rotations, direction, or speed, can add up each other
    //direction of global rotation of object
    #region rotateDir

    [Header("SPEED MANAGER"), SerializeField, Tooltip("set rotation direction of object on the 3 axis")]
    private Vector3 _rotateDir;
    public Vector3 RotateDir
    {
        get { return _rotateDir; }
        set { _rotateDir = value; }
    }

    #endregion

    //speed of global rotation of object
    #region rotateSpeed

    [SerializeField, Tooltip("set rotation speed of object")]
    private float _rotateSpeed = 1;
    public float RotateSpeed
    {
        get { return _localRotateSpeed; }
        set { _localRotateSpeed = value; }
    }

    #endregion

    //direction of local rotation of object
    #region localRotateDir

    [Header(""), SerializeField, Tooltip("direction of rotation locally to parent gameObject")]
    private Vector3 _localRotateDir;
    public Vector3 LocalRotateDir
    {
        get { return _localRotateDir; }
        set { _localRotateDir = value; }
    }

    #endregion

    //speed of local rotation of object
    #region localRotateSpeed

    [SerializeField, Tooltip("set rotation speed of object")]
    private float _localRotateSpeed = 1;
    public float LocalRotateSpeed
    {
        get { return _localRotateSpeed; }
        set { _localRotateSpeed = value; }
    }

    #endregion


    //TIME

    //time used to render every movement, none means that it will call Rotate every frame depend on computer frame rate, noneFixed mean it will depends on timeStep, deltaTime mean it will move constantly trough real time, fixedDeltaTime mean it will be called in fixed but not depend of timeStep.
    #region timeUsed

    [Header("TIME"), SerializeField, Tooltip("if enabled, game will make constant speed rotation trough real time, if disabled, game will use Physics Fixed Update value.")]
    private _timeUsedEnum _timeUsed = _timeUsedEnum.deltaTime;
    public _timeUsedEnum TimeUsed
    {
        get { return _timeUsed; }
        set { _timeUsed = value; }
    }
    public enum _timeUsedEnum
    {
        none,
        noneFixed,
        deltaTime,
        fixedDeltaTime
    } 

    #endregion

    //called every frame
    private void Update()
    {
        //call in update if enum is set to none or delta time.
        if(_timeUsed == _timeUsedEnum.none || _timeUsed == _timeUsedEnum.deltaTime)
            RotateApply(Time.deltaTime);
    }

    //called every physics updates, depending on timeStep.
    private void FixedUpdate()
    {
        //call in fixedUpdate if enum is set to noneFixed or fixed delta time.
        if (_timeUsed == _timeUsedEnum.noneFixed || _timeUsed == _timeUsedEnum.fixedDeltaTime)
            RotateApply(Time.fixedDeltaTime);
    }

    /// <summary>
    /// RotateApply,
    /// 
    /// apply a rotation depending of directions and local directions, speed and local speed, and time mode.
    /// </summary>
    /// <param name="deltaTimeUsed"></param>
    private void RotateApply(float deltaTimeUsed)
    {
        //multiplier used, depend on timeUsed enum value.
        float _deltaTime = 0.02f;

        //if enum is set to a delta time or fixed deltaTime, if not, _deltaTime will be set to 1
        if (_timeUsed == _timeUsedEnum.deltaTime || _timeUsed == _timeUsedEnum.fixedDeltaTime)
            _deltaTime = deltaTimeUsed;
        Debug.Log(_deltaTime);

        //rotate trough global axis
        transform.RotateAround(transform.position, _rotateDir, _rotateSpeed * _deltaTime);

        //rotate trough object local axis
        transform.Rotate((_localRotateDir.normalized) * (_localRotateSpeed * _deltaTime));
    }
}
