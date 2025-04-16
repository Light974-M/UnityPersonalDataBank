using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(ThrowerManager))]
public class ThrowerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        ThrowerManager myTarget = (ThrowerManager)target;

        if(GUILayout.Button("Shoot"))
        {
            myTarget.ShootBullet();
        }

        GUIContent activeTargetContent = new GUIContent(nameof(myTarget.ActiveTarget), "active target to shoot");
        myTarget.ActiveTarget = (Transform)EditorGUILayout.ObjectField(activeTargetContent, myTarget.ActiveTarget, typeof(Transform), true);

        GUIContent bulletPrefabContent = new GUIContent(nameof(myTarget.BulletPrefab), "bullet prefab");
        myTarget.BulletPrefab = (GameObject)EditorGUILayout.ObjectField(bulletPrefabContent, myTarget.BulletPrefab, typeof(GameObject), true);

        GUIContent bulletSpawnPointContent = new GUIContent(nameof(myTarget.BulletSpawnPoint), "bullet spawn point");
        myTarget.BulletSpawnPoint = (Transform)EditorGUILayout.ObjectField(bulletSpawnPointContent, myTarget.BulletSpawnPoint, typeof(Transform), true);

        GUIContent imprecisionRadiusContent = new GUIContent(nameof(myTarget.ImprecisionRadius), "radius within the enemy can aim arround targeted pos");
        myTarget.ImprecisionRadius = EditorGUILayout.FloatField(imprecisionRadiusContent, myTarget.ImprecisionRadius);

        GUIContent imprecisionDistanceMultiplierContent = new GUIContent(nameof(myTarget.ImprecisionDistanceMultiplier), "multiplier for distance affected");
        myTarget.ImprecisionDistanceMultiplier = EditorGUILayout.FloatField(imprecisionDistanceMultiplierContent, myTarget.ImprecisionDistanceMultiplier);

        GUIContent imprecisionDistancePercentageContent = new GUIContent(nameof(myTarget.ImprecisionDistancePercentage), "fade between static imprecision and distance affected imprecision");
        myTarget.ImprecisionDistancePercentage = EditorGUILayout.Slider(imprecisionDistancePercentageContent, myTarget.ImprecisionDistancePercentage, 0, 1);

        SerializedProperty _bulletColliderExcludedListProperty = serializedObject.FindProperty("_bulletColliderExcludedList");

        serializedObject.Update();

        EditorGUILayout.PropertyField(_bulletColliderExcludedListProperty);

        serializedObject.ApplyModifiedProperties();
    }
}
