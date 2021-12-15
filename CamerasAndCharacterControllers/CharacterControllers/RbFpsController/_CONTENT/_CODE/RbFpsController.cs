using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RbFpsController : MonoBehaviour
{
    #region Serialized And Public Variables

    #region OBJECTS

    [Header("OBJECTS")]

    #region _rb

    [SerializeField, Tooltip("rigidBody used to calculate player movements")]
    private Rigidbody _rb;
    public Rigidbody Rb
    {
        get { return _rb; }
        set { _rb = value; }
    }

    #endregion

    #region _collider

    [SerializeField, Tooltip("collider Rigidbody will use to calculate player movements")]
    private Collider _collider;
    public Collider Collider
    {
        get { return _collider; }
        set { _collider = value; }
    }

    #endregion 

    #endregion

    #endregion

    #region Private Variables

    #endregion

    public void InitVariables()
    {

    }
}
