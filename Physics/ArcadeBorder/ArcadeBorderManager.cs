using UnityEngine;
using UnityEngine.Events;
using UPDB.CoreHelper.Usable;
using UPDB.CoreHelper.UsableMethods;

public class ArcadeBorderManager : Singleton<ArcadeBorderManager>
{
    [SerializeField]
    private ArcadeBorderAsset _borderAsset;

    #region Public API

    public ArcadeBorderAsset BorderAsset => _borderAsset;

    #endregion

    

    protected override void OnSceneSelected()
    {
        base.OnSceneSelected();

        if (_borderAsset.BordersEnableMatrix.LoopX && !_borderAsset.BordersEnableMatrix.LoopY)
        {
            Debug.DrawLine(new Vector2(_borderAsset.LoopMinRect.x, -1000000), new Vector2(_borderAsset.LoopMinRect.x, 1000000), Color.blue);
            Debug.DrawLine(new Vector2(_borderAsset.LoopMaxRect.x, -1000000), new Vector2(_borderAsset.LoopMaxRect.x, 1000000), Color.blue);

            Debug.DrawLine(new Vector2(_borderAsset.CloneMinRect.x, -1000000), new Vector2(_borderAsset.CloneMinRect.x, 1000000), Color.blue);
            Debug.DrawLine(new Vector2(_borderAsset.CloneMaxRect.x, -1000000), new Vector2(_borderAsset.CloneMaxRect.x, 1000000), Color.blue);
        }

        if (_borderAsset.BordersEnableMatrix.LoopY && !_borderAsset.BordersEnableMatrix.LoopX)
        {
            Debug.DrawLine(new Vector2(-1000000, _borderAsset.LoopMinRect.y), new Vector2(1000000, _borderAsset.LoopMinRect.y), Color.blue);
            Debug.DrawLine(new Vector2(-1000000, _borderAsset.LoopMaxRect.y), new Vector2(1000000, _borderAsset.LoopMaxRect.y), Color.blue);

            Debug.DrawLine(new Vector2(-1000000, _borderAsset.CloneMinRect.y), new Vector2(1000000, _borderAsset.CloneMinRect.y), Color.blue);
            Debug.DrawLine(new Vector2(-1000000, _borderAsset.CloneMaxRect.y), new Vector2(1000000, _borderAsset.CloneMaxRect.y), Color.blue);
        }

        if (_borderAsset.BordersEnableMatrix.LoopX && _borderAsset.BordersEnableMatrix.LoopY)
        {
            Debug.DrawLine(new Vector2(_borderAsset.LoopMinRect.x, _borderAsset.LoopMinRect.y), new Vector2(_borderAsset.LoopMinRect.x, _borderAsset.LoopMaxRect.y), Color.blue);
            Debug.DrawLine(new Vector2(_borderAsset.LoopMaxRect.x, _borderAsset.LoopMinRect.y), new Vector2(_borderAsset.LoopMaxRect.x, _borderAsset.LoopMaxRect.y), Color.blue);
            Debug.DrawLine(new Vector2(_borderAsset.LoopMinRect.x, _borderAsset.LoopMinRect.y), new Vector2(_borderAsset.LoopMaxRect.x, _borderAsset.LoopMinRect.y), Color.blue);
            Debug.DrawLine(new Vector2(_borderAsset.LoopMinRect.x, _borderAsset.LoopMaxRect.y), new Vector2(_borderAsset.LoopMaxRect.x, _borderAsset.LoopMaxRect.y), Color.blue);

            Debug.DrawLine(new Vector2(_borderAsset.CloneMinRect.x, _borderAsset.CloneMinRect.y), new Vector2(_borderAsset.CloneMinRect.x, _borderAsset.CloneMaxRect.y), Color.blue);
            Debug.DrawLine(new Vector2(_borderAsset.CloneMaxRect.x, _borderAsset.CloneMinRect.y), new Vector2(_borderAsset.CloneMaxRect.x, _borderAsset.CloneMaxRect.y), Color.blue);
            Debug.DrawLine(new Vector2(_borderAsset.CloneMinRect.x, _borderAsset.CloneMinRect.y), new Vector2(_borderAsset.CloneMaxRect.x, _borderAsset.CloneMinRect.y), Color.blue);
            Debug.DrawLine(new Vector2(_borderAsset.CloneMinRect.x, _borderAsset.CloneMaxRect.y), new Vector2(_borderAsset.CloneMaxRect.x, _borderAsset.CloneMaxRect.y), Color.blue);
        }

        if (_borderAsset.BordersEnableMatrix.BorderMinX)
        {
            if (_borderAsset.BordersEnableMatrix.BorderMinY && _borderAsset.BordersEnableMatrix.BorderMaxY)
                Debug.DrawLine(new Vector2(_borderAsset.BorderMinRect.x, _borderAsset.BorderMinRect.y), new Vector2(_borderAsset.BorderMinRect.x, _borderAsset.BorderMaxRect.y), Color.red);

            if (_borderAsset.BordersEnableMatrix.BorderMinY && !_borderAsset.BordersEnableMatrix.BorderMaxY)
                Debug.DrawLine(new Vector2(_borderAsset.BorderMinRect.x, _borderAsset.BorderMinRect.y), new Vector2(_borderAsset.BorderMinRect.x, 1000000), Color.red);

            if (!_borderAsset.BordersEnableMatrix.BorderMinY && _borderAsset.BordersEnableMatrix.BorderMaxY)
                Debug.DrawLine(new Vector2(_borderAsset.BorderMinRect.x, -1000000), new Vector2(_borderAsset.BorderMinRect.x, _borderAsset.BorderMaxRect.y), Color.red);

            if (!_borderAsset.BordersEnableMatrix.BorderMinY && !_borderAsset.BordersEnableMatrix.BorderMaxY)
                Debug.DrawLine(new Vector2(_borderAsset.BorderMinRect.x, -1000000), new Vector2(_borderAsset.BorderMinRect.x, 1000000), Color.red);
        }

        if (_borderAsset.BordersEnableMatrix.BorderMaxX)
        {
            if (_borderAsset.BordersEnableMatrix.BorderMinY && _borderAsset.BordersEnableMatrix.BorderMaxY)
                Debug.DrawLine(new Vector2(_borderAsset.BorderMaxRect.x, _borderAsset.BorderMinRect.y), new Vector2(_borderAsset.BorderMaxRect.x, _borderAsset.BorderMaxRect.y), Color.red);

            if (_borderAsset.BordersEnableMatrix.BorderMinY && !_borderAsset.BordersEnableMatrix.BorderMaxY)
                Debug.DrawLine(new Vector2(_borderAsset.BorderMaxRect.x, _borderAsset.BorderMinRect.y), new Vector2(_borderAsset.BorderMaxRect.x, 1000000), Color.red);

            if (!_borderAsset.BordersEnableMatrix.BorderMinY && _borderAsset.BordersEnableMatrix.BorderMaxY)
                Debug.DrawLine(new Vector2(_borderAsset.BorderMaxRect.x, -1000000), new Vector2(_borderAsset.BorderMaxRect.x, _borderAsset.BorderMaxRect.y), Color.red);

            if (!_borderAsset.BordersEnableMatrix.BorderMinY && !_borderAsset.BordersEnableMatrix.BorderMaxY)
                Debug.DrawLine(new Vector2(_borderAsset.BorderMaxRect.x, -1000000), new Vector2(_borderAsset.BorderMaxRect.x, 1000000), Color.red);
        }

        if (_borderAsset.BordersEnableMatrix.BorderMinY)
        {
            if (_borderAsset.BordersEnableMatrix.BorderMinX && _borderAsset.BordersEnableMatrix.BorderMaxX)
                Debug.DrawLine(new Vector2(_borderAsset.BorderMinRect.x, _borderAsset.BorderMinRect.y), new Vector2(_borderAsset.BorderMaxRect.x, _borderAsset.BorderMinRect.y), Color.red);

            if (_borderAsset.BordersEnableMatrix.BorderMinX && !_borderAsset.BordersEnableMatrix.BorderMaxX)
                Debug.DrawLine(new Vector2(_borderAsset.BorderMinRect.x, _borderAsset.BorderMinRect.y), new Vector2(1000000, _borderAsset.BorderMinRect.y), Color.red);

            if (!_borderAsset.BordersEnableMatrix.BorderMinX && _borderAsset.BordersEnableMatrix.BorderMaxX)
                Debug.DrawLine(new Vector2(-1000000, _borderAsset.BorderMinRect.y), new Vector2(_borderAsset.BorderMaxRect.x, _borderAsset.BorderMinRect.y), Color.red);

            if (!_borderAsset.BordersEnableMatrix.BorderMinX && !_borderAsset.BordersEnableMatrix.BorderMaxX)
                Debug.DrawLine(new Vector2(-1000000, _borderAsset.BorderMinRect.y), new Vector2(1000000, _borderAsset.BorderMinRect.y), Color.red);
        }

        if (_borderAsset.BordersEnableMatrix.BorderMaxY)
        {
            if (_borderAsset.BordersEnableMatrix.BorderMinX && _borderAsset.BordersEnableMatrix.BorderMaxX)
                Debug.DrawLine(new Vector2(_borderAsset.BorderMinRect.x, _borderAsset.BorderMaxRect.y), new Vector2(_borderAsset.BorderMaxRect.x, _borderAsset.BorderMaxRect.y), Color.red);

            if (_borderAsset.BordersEnableMatrix.BorderMinX && !_borderAsset.BordersEnableMatrix.BorderMaxX)
                Debug.DrawLine(new Vector2(_borderAsset.BorderMinRect.x, _borderAsset.BorderMaxRect.y), new Vector2(1000000, _borderAsset.BorderMaxRect.y), Color.red);

            if (!_borderAsset.BordersEnableMatrix.BorderMinX && _borderAsset.BordersEnableMatrix.BorderMaxX)
                Debug.DrawLine(new Vector2(-1000000, _borderAsset.BorderMaxRect.y), new Vector2(_borderAsset.BorderMaxRect.x, _borderAsset.BorderMaxRect.y), Color.red);

            if (!_borderAsset.BordersEnableMatrix.BorderMinX && !_borderAsset.BordersEnableMatrix.BorderMaxX)
                Debug.DrawLine(new Vector2(-1000000, _borderAsset.BorderMaxRect.y), new Vector2(1000000, _borderAsset.BorderMaxRect.y), Color.red);
        }


        Debug.DrawLine(new Vector2(_borderAsset.VisibilityBorderMinRect.x, _borderAsset.VisibilityBorderMinRect.y), new Vector2(_borderAsset.VisibilityBorderMinRect.x, _borderAsset.VisibilityBorderMaxRect.y), Color.black);
        Debug.DrawLine(new Vector2(_borderAsset.VisibilityBorderMaxRect.x, _borderAsset.VisibilityBorderMinRect.y), new Vector2(_borderAsset.VisibilityBorderMaxRect.x, _borderAsset.VisibilityBorderMaxRect.y), Color.black);
        Debug.DrawLine(new Vector2(_borderAsset.VisibilityBorderMinRect.x, _borderAsset.VisibilityBorderMinRect.y), new Vector2(_borderAsset.VisibilityBorderMaxRect.x, _borderAsset.VisibilityBorderMinRect.y), Color.black);
        Debug.DrawLine(new Vector2(_borderAsset.VisibilityBorderMinRect.x, _borderAsset.VisibilityBorderMaxRect.y), new Vector2(_borderAsset.VisibilityBorderMaxRect.x, _borderAsset.VisibilityBorderMaxRect.y), Color.black);

        Debug.DrawLine(new Vector2(_borderAsset.EnemySimulationMinRect.x, _borderAsset.EnemySimulationMinRect.y), new Vector2(_borderAsset.EnemySimulationMinRect.x, _borderAsset.EnemySimulationMaxRect.y), Color.magenta);
        Debug.DrawLine(new Vector2(_borderAsset.EnemySimulationMaxRect.x, _borderAsset.EnemySimulationMinRect.y), new Vector2(_borderAsset.EnemySimulationMaxRect.x, _borderAsset.EnemySimulationMaxRect.y), Color.magenta);
        Debug.DrawLine(new Vector2(_borderAsset.EnemySimulationMinRect.x, _borderAsset.EnemySimulationMinRect.y), new Vector2(_borderAsset.EnemySimulationMaxRect.x, _borderAsset.EnemySimulationMinRect.y), Color.magenta);
        Debug.DrawLine(new Vector2(_borderAsset.EnemySimulationMinRect.x, _borderAsset.EnemySimulationMaxRect.y), new Vector2(_borderAsset.EnemySimulationMaxRect.x, _borderAsset.EnemySimulationMaxRect.y), Color.magenta);

        Debug.DrawLine(new Vector2(_borderAsset.PlayerSimulationMinRect.x, _borderAsset.PlayerSimulationMinRect.y), new Vector2(_borderAsset.PlayerSimulationMinRect.x, _borderAsset.PlayerSimulationMaxRect.y), Color.magenta);
        Debug.DrawLine(new Vector2(_borderAsset.PlayerSimulationMaxRect.x, _borderAsset.PlayerSimulationMinRect.y), new Vector2(_borderAsset.PlayerSimulationMaxRect.x, _borderAsset.PlayerSimulationMaxRect.y), Color.magenta);
        Debug.DrawLine(new Vector2(_borderAsset.PlayerSimulationMinRect.x, _borderAsset.PlayerSimulationMinRect.y), new Vector2(_borderAsset.PlayerSimulationMaxRect.x, _borderAsset.PlayerSimulationMinRect.y), Color.magenta);
        Debug.DrawLine(new Vector2(_borderAsset.PlayerSimulationMinRect.x, _borderAsset.PlayerSimulationMaxRect.y), new Vector2(_borderAsset.PlayerSimulationMaxRect.x, _borderAsset.PlayerSimulationMaxRect.y), Color.magenta);
    }

    public bool IsContained(Vector2 pos)
    {
        return IsContainedInFixed(pos) && IsContainedInLoop(pos);
    }

    public bool IsContainedInFixed(Vector2 pos)
    {
        return pos.x >= _borderAsset.BorderMinRect.x && pos.x <= _borderAsset.BorderMaxRect.x && pos.y >= _borderAsset.BorderMinRect.y && pos.y <= _borderAsset.BorderMaxRect.y;
    }

    public bool IsContainedInLoop(Vector2 pos)
    {
        return pos.x >= _borderAsset.LoopMinRect.x && pos.x <= _borderAsset.LoopMaxRect.x && pos.y >= _borderAsset.LoopMinRect.y && pos.y <= _borderAsset.LoopMaxRect.y;
    }

    public bool IsContainedInVisible(Vector2 pos)
    {
        return pos.x >= _borderAsset.VisibilityBorderMinRect.x && pos.x <= _borderAsset.VisibilityBorderMaxRect.x && pos.y >= _borderAsset.VisibilityBorderMinRect.y && pos.y <= _borderAsset.VisibilityBorderMaxRect.y;
    }

    public bool IsContainedInPlayerSimulation(Vector2 pos)
    {
        return pos.x >= _borderAsset.PlayerSimulationMinRect.x && pos.x <= _borderAsset.PlayerSimulationMaxRect.x && pos.y >= _borderAsset.PlayerSimulationMinRect.y && pos.y <= _borderAsset.PlayerSimulationMaxRect.y;
    }
}
