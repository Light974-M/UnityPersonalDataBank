using UnityEngine;

/// <summary>
/// singleton manager of scenes global properties, unique scene properties are stored in children classes of levelManager
/// </summary>
public abstract class LevelManager : MonoBehaviour
{
    #region Singleton Initialize

    /// <summary>
    /// unique instance of GameManager class
    /// </summary>
    private static LevelManager _instance;

    public static LevelManager Instance
    {
        get
        {
            if (_instance == null)
                _instance = FindObjectOfType<LevelManager>();

            return _instance;
        }

        set
        {
            _instance = value;
        }
    }

    #endregion

    /// <summary>
    /// awake is called when script instance is being loaded
    /// </summary>
    private void Awake()
    {
        Initialize();
    }

    /// <summary>
    /// OnDrawGizmos is called each time scene refresh
    /// </summary>
    private void OnDrawGizmos()
    {
        if (!Application.isPlaying)
            Initialize();
    }

    private void Initialize()
    {
        if (_instance == null)
        {
            _instance = this;
        }
        else if (_instance != this)
        {
            if (Application.isPlaying)
                Destroy(this);
            else
                DestroyImmediate(this);
        }
    }
}
