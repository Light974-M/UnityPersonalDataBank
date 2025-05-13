using UnityEngine;

[CreateAssetMenu(fileName = "ArcadeBorderAsset", menuName = "UPDB/Physic/Arcade Border/ArcadeBorderAsset")]
public class ArcadeBorderAsset : ScriptableObject
{
    [SerializeField]
    private Vector2 _loopMinRect = Vector2.one * -1;

    [SerializeField]
    private Vector2 _loopMaxRect = Vector2.one * 1;

    [SerializeField]
    private Vector2 _cloneMinRect = Vector2.one * -1;

    [SerializeField]
    private Vector2 _cloneMaxRect = Vector2.one * 1;

    [SerializeField]
    private Vector2 _borderMinRect = Vector2.one * -1;

    [SerializeField]
    private Vector2 _borderMaxRect = Vector2.one * 1;

    [SerializeField]
    private Vector2 _visibilityBorderMinRect = Vector2.one * -1;

    [SerializeField]
    private Vector2 _visibilityBorderMaxRect = Vector2.one * 1;

    [SerializeField]
    private Vector2 _enemySimulationMinRect = Vector2.one * -1;

    [SerializeField]
    private Vector2 _enemySimulationMaxRect = Vector2.one * -1;

    [SerializeField]
    private Vector2 _playerSimulationMinRect = Vector2.one * -1;

    [SerializeField]
    private Vector2 _playerSimulationMaxRect = Vector2.one * -1;

    [SerializeField, Tooltip("choose wich borders side to enable")]
    private BordersMatrix _bordersEnableMatrix;

    #region Public API

    public Vector2 LoopMinRect
    {
        get => _loopMinRect;
        set => _loopMinRect = value;
    }

    public Vector2 LoopMaxRect
    {
        get => _loopMaxRect;
        set => _loopMaxRect = value;
    }

    public Vector2 CloneMinRect
    {
        get => _cloneMinRect;
        set => _cloneMinRect = value;
    }

    public Vector2 CloneMaxRect
    {
        get => _cloneMaxRect;
        set => _cloneMaxRect = value;
    }

    public Vector2 BorderMinRect
    {
        get => _borderMinRect;
        set => _borderMinRect = value;
    }

    public Vector2 BorderMaxRect
    {
        get => _borderMaxRect;
        set => _borderMaxRect = value;
    }

    public Vector2 VisibilityBorderMinRect
    {
        get => _visibilityBorderMinRect;
        set => _visibilityBorderMinRect = value;
    }

    public Vector2 VisibilityBorderMaxRect
    {
        get => _visibilityBorderMaxRect;
        set => _visibilityBorderMaxRect = value;
    }

    public Vector2 EnemySimulationMinRect
    {
        get => _enemySimulationMinRect;
        set => _enemySimulationMinRect = value;
    }

    public Vector2 EnemySimulationMaxRect
    {
        get => _enemySimulationMaxRect;
        set => _enemySimulationMaxRect = value;
    }

    public Vector2 PlayerSimulationMinRect
    {
        get => _playerSimulationMinRect;
        set => _playerSimulationMinRect = value;
    }

    public Vector2 PlayerSimulationMaxRect
    {
        get => _playerSimulationMaxRect;
        set => _playerSimulationMaxRect = value;
    }

    public BordersMatrix BordersEnableMatrix
    {
        get => _bordersEnableMatrix;
        set => _bordersEnableMatrix = value;
    }

    #endregion
}

[System.Serializable]
public struct BordersMatrix
{
    [SerializeField, Tooltip("enable loop border for x axis")]
    private bool _loopX;

    [SerializeField, Tooltip("enable loop border for y axis")]
    private bool _loopY;

    [SerializeField, Tooltip("enable fixed border for min x side")]
    private bool _borderMinX;

    [SerializeField, Tooltip("enable fixed border for min y side")]
    private bool _borderMinY;

    [SerializeField, Tooltip("enable fixed border for max x side")]
    private bool _borderMaxX;

    [SerializeField, Tooltip("enable fixed border for max y side")]
    private bool _borderMaxY;

    #region Public API

    public bool LoopX => _loopX;
    public bool LoopY => _loopY;
    public bool BorderMinX => _borderMinX;
    public bool BorderMinY => _borderMinY;
    public bool BorderMaxX => _borderMaxX;
    public bool BorderMaxY => _borderMaxY;

    #endregion
}
