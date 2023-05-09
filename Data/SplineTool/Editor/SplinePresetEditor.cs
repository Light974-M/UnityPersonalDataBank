using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

///<summary>
/// 
///</summary>
[CustomEditor(typeof(SplinePreset))]
public class SplinePresetEditor : Editor
{
    SplinePreset myTarget;

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        myTarget = (SplinePreset)target;

        myTarget.IsOnInspector = true;
    }

    private void OnDisable()
    {
        if (myTarget)
            myTarget.IsOnInspector = false;
    }
}