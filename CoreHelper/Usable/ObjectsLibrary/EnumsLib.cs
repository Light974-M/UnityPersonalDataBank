using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UPDB.CoreHelper.Usable.ObjectsLibrary
{
    public enum Axis
    {
        X,
        Y,
        Z,
        W,
        XY,
        YZ,
        XZ,
        XYZ,
        XW,
        YW,
        ZW,
        XYZW,
    }

    public enum Direction
    {
        Up,
        Down,
        Left,
        Right,
        Forward,
        Backward
    }

    public enum DisplayEditor
    {
        None,
        OnScene,
        OnSceneSelected,
        OnDrawGizmos,
        OnDrawGizmosSelected,
        OnPlay,
        OnPause,
        OnResume,
    }

    public enum PlayerAction
    {
        Idle,
        Move,
        Jump,
        Attack,
        Defend,
        Interact
    }

    public enum DifficultyLevel
    {
        Easy,
        Medium,
        Hard,
        Expert
    }

    public enum UIState
    {
        MainMenu,
        OptionsMenu,
        InGameHUD,
        PauseMenu,
        GameOverScreen,
        VictoryScreen
    }

    public enum MovementType
    {
        Walking,
        Running,
        Flying,
        Swimming,
        Crawling
    }

    public enum AnimationState
    {
        Idle,
        Walking,
        Running,
        Jumping,
        Falling,
        Attacking,
        Dying
    }

    public enum HealthState
    {
        Healthy,
        Injured,
        Critical,
        Dead
    }

    public enum WeatherType
    {
        Sunny,
        Rainy,
        Snowy,
        Windy,
        Stormy,
        Foggy
    }

    public enum ConstructionState
    {
        NotStarted,
        InProgress,
        Completed,
        Damaged,
        Destroyed
    }

    public enum SaveState
    {
        NotSaved,
        AutoSave,
        ManualSave,
        QuickSave,
        Checkpoint
    }

    public enum PositionRendredType
    {
        Centered,
        IntCentered,
        Edged,
    }

    public enum ColliderType
    {
        BoxCollider,
        CapsuleCollider,
        SphereCollider,
        MeshCollider,
        BoxCollider2D,
        CircleCollider2D,
        CapsuleCollider2D,
    }

    public enum Comparison
    {
        None,
        Equal,
        NotEqual,
        Greater,
        Less,
        GreaterOrEqual,
        LessOrEqual,
    }

    public enum VolumeTestAlgo
    {
        WindingNumber,
        BarycentricTest,
    }
}
