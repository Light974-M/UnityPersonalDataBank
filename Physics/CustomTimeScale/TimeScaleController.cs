using UnityEngine;
using UnityEngine.UIElements;
using UPDB.CoreHelper.UsableMethods;

public class TimeScaleController : UPDBBehaviour
{
    [SerializeField, Tooltip("time scale value")]
    private float _timeScale = 1;

    //[SerializeField, Tooltip("frame to be based on")]
    //private float _fps = 60;

    [SerializeField, Tooltip("tell if script control time scale")]
    private bool _useTimeScale = true;

    /// <summary>
    /// 
    /// </summary>
    void FixedUpdate()
    {
        if(_useTimeScale )
        {
            Time.timeScale = _timeScale;
            Time.fixedDeltaTime = (1f / 60f) * Time.timeScale;
        }
    }
}
