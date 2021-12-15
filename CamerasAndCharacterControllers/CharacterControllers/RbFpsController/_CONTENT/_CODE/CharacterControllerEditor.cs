using UnityEditor;

namespace UPDB.CamerasAndCharacterControllers.CharacterControllers.RbFpsController
{
    [CustomEditor(typeof(CharacterController)), CanEditMultipleObjects]
    public class CharacterControllerEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            CharacterController myTarget = (CharacterController)target;
        }
    } 
}
