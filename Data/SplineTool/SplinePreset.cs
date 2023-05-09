using System.Collections.Generic;
using UnityEngine;

///<summary>
/// save data of a spline to use it whenever you want
///</summary>
[CreateAssetMenu(fileName = "NewSplinePreset", menuName = "Scriptables/SplinePreset")]
public class SplinePreset : ScriptableObject
{
    /*****************************************************FIRST KEY PARAMETERS**************************************************************/
    [Space, Header("FIRST KEY PARAMETERS"), Space]

    [SerializeField, Tooltip("first key angle")]
    private Quaternion _startRotation;

    [SerializeField, Tooltip("rotation mode to overwrite for start rotation")]
    private RotationMode _startRotationOverwrite = RotationMode.FollowRotation;

    [SerializeField, Tooltip("speed wich target will be at beginning")]
    private float _startSpeed = 1;


    /*****************************************************KEYS LIST**************************************************************/
    [Space, Header("KEYS LIST"), Space]

    [SerializeField, Tooltip("list of every key points of spline")]
    private KeyPoint[] _keyPoints;


    #region Private API

    private bool _isOnInspector = false;

    #endregion

    #region Public API

    public Quaternion StartRotation
    {
        get { return _startRotation; }
        set { _startRotation = value; }
    }
    public RotationMode StartRotationOverwrite => _startRotationOverwrite;
    public float StartSpeed => _startSpeed;
    public KeyPoint[] KeyPoints
    {
        get { return _keyPoints; }
        set { _keyPoints = value; }
    }
    public bool IsOnInspector
    {
        get { return _isOnInspector; }
        set { _isOnInspector = value; }
    }

    #endregion

    /*****************************************CUSTOM FUNCTIONS***********************************************/

    public void InitValuesAndCurves()
    {
        if (_keyPoints.Length >= 2)
        {
            for (int i = 0; i < _keyPoints.Length; i++)
            {
                if (_keyPoints[i].XCurve.length == 0)
                    _keyPoints[i].XCurve = AnimationCurve.Constant(0, 1, 0);

                if (_keyPoints[i].YCurve.length == 0)
                    _keyPoints[i].YCurve = AnimationCurve.Constant(0, 1, 0);

                if (_keyPoints[i].RotationLerpShape.length == 0)
                    _keyPoints[i].RotationLerpShape = AnimationCurve.Linear(0, 0, 1, 1);
            }
        }
        else
        {
            _keyPoints = new KeyPoint[2];
            _keyPoints[1] = new KeyPoint(new Vector3(1, 0, 0), Quaternion.identity);
        }
    }

    public void CallGizmos(SplineManager linkedManager)
    {
        if (_isOnInspector)
            linkedManager.CallVisualDrawing();
    }
}