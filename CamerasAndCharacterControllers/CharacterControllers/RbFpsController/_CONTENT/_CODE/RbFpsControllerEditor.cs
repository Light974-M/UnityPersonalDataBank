using UnityEditor;

[CustomEditor(typeof(RbFpsController)), CanEditMultipleObjects]
public class RbFpsControllerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        RbFpsController myTarget = (RbFpsController)target;


    }
}
