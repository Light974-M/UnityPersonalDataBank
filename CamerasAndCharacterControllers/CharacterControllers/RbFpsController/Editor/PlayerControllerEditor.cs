using UnityEditor;
using UnityEngine;

namespace UPDB.CamerasAndCharacterControllers.CharacterControllers.RbFpsController
{
    [CustomEditor(typeof(PlayerController)), CanEditMultipleObjects]
    public class PlayerControllerEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            PlayerController myTarget = (PlayerController)target;

            DrawCustomInspector(myTarget);

            if(!Application.isPlaying)
                myTarget.InitVariables();

            myTarget.Rb.linearDamping = Mathf.Clamp(myTarget.Rb.linearDamping, 0, 50);
        }

        private void DrawCustomInspector(PlayerController myTarget)
        {
            myTarget.Rb = (Rigidbody)EditorGUILayout.ObjectField("RigidBody", myTarget.Rb, typeof(Rigidbody), true);
            //myTarget.Collider = (Collider)EditorGUILayout.ObjectField("Collider linked", myTarget.Rb, typeof(Rigidbody), true);
            myTarget.Speed = EditorGUILayout.FloatField(new GUIContent("Speed"), myTarget.Speed);

            if(myTarget.Rb != null)
                myTarget.Rb.linearDamping = EditorGUILayout.FloatField(new GUIContent("Drag"), myTarget.Rb.linearDamping);
        }
    }
}
