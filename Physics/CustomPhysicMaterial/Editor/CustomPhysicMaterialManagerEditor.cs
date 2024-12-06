using System.Collections;
using System.Collections.Generic;
using System.Security.Policy;
using UnityEditor;
using UnityEngine;

namespace UPDB.Physic.CustomPhysicMaterial
{
	[CustomEditor(typeof(CustomPhysicMaterialManager))]
	public class CustomPhysicMaterialManagerEditor : Editor
	{
        public override void OnInspectorGUI()
        {
            CustomPhysicMaterialManager myTarget = (CustomPhysicMaterialManager)target;

            myTarget.Init();
            base.OnInspectorGUI();
        }
    } 
}
