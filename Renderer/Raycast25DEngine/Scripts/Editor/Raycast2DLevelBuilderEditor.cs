using UnityEditor;
using UnityEngine;

namespace UPDB.Renderers.Raycast25DEngine
{
	[CustomEditor(typeof(Raycast2DLevelBuilder))]
	public class Raycast2DLevelBuilderEditor : Editor
	{
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            Raycast2DLevelBuilder myTarget = (Raycast2DLevelBuilder)target;
        }
    } 
}
