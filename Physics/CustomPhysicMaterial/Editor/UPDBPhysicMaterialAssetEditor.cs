using Codice.CM.SEIDInfo;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace UPDB.Physic.CustomPhysicMaterial
{
    [CustomEditor (typeof(UPDBPhysicMaterialAsset))]
	public class UPDBPhysicMaterialAssetEditor : Editor
	{
        public override void OnInspectorGUI()
        {
            UPDBPhysicMaterialAsset myTarget = (UPDBPhysicMaterialAsset)target;

            GUIContent dynamicFrictionContent = new GUIContent(nameof(myTarget.DynamicFriction), "How Much Friction the Collider's surface has when moving, while in contact with another Collider. [-Infinity, Infinity]");
            myTarget.DynamicFriction = EditorGUILayout.FloatField(dynamicFrictionContent, myTarget.DynamicFriction);

            GUIContent staticFrictionContent = new GUIContent(nameof(myTarget.StaticFriction), "How Much Friction the Collider's surface has when stationary, while in contact with another Collider. [-Infinity, Infinity]");
            myTarget.StaticFriction = EditorGUILayout.FloatField(staticFrictionContent, myTarget.StaticFriction);

            GUIContent bouncinessContent = new GUIContent(nameof(myTarget.Bounciness), "How bouncy the Collider's surface is, defined by how much speed the other Collider retains after collision, values over 1 may not be realistic(not possible to have more energy than previously) [0, Infinity]");
            myTarget.Bounciness = Mathf.Clamp(EditorGUILayout.FloatField(bouncinessContent, myTarget.Bounciness), 0, Mathf.Infinity);

            GUIContent bounceCombineContent = new GUIContent(nameof(myTarget.BounceCombine), "how bounciness strength is calculated");
            myTarget.BounceCombine = (BounceCombineMode)EditorGUILayout.EnumPopup(bounceCombineContent, myTarget.BounceCombine);
        }
    } 
}
