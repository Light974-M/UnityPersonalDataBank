using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace UPDB.ProceduralGeneration.ProShapeBuilder
{
    [CustomEditor(typeof(OctahedronMeshBuilder))]
    public class OctahedronMeshBuilderEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            Color backgroundColor = Color.white;
            OctahedronMeshBuilder myTarget = (OctahedronMeshBuilder)target;

            if (GUILayout.Button("CLEAR ALL"))
                myTarget.ClearAll();

            GUI.backgroundColor = Color.red;

            if (GUILayout.Button("BUILD"))
                myTarget.BuildMesh();

            GUI.backgroundColor = backgroundColor;

            GUILayout.Space(EditorGUIUtility.singleLineHeight);

            EditorGUILayout.BeginVertical("helpBox");
            {
                GUIContent baseParameterContent = new GUIContent("BASE PARAMETER", "parameters of base mesh procedural class");
                EditorGUILayout.LabelField(baseParameterContent);

                GUIContent scaleFactorContent = new GUIContent(nameof(myTarget.ScaleFactor), "scale of default mesh in meters");
                myTarget.ScaleFactor = EditorGUILayout.FloatField(scaleFactorContent, myTarget.ScaleFactor);

                GUILayout.Space(EditorGUIUtility.singleLineHeight);

                GUIContent baseScaleTypeContent = new GUIContent(nameof(myTarget.BaseScaleType), "tell what base scale should look like");
                myTarget.BaseScaleType = (BaseScaleUnitOfSolid)EditorGUILayout.EnumPopup(baseScaleTypeContent, myTarget.BaseScaleType);

                //GUIContent quadNumberContent = new GUIContent(nameof(myTarget.QuadNumber), "number of quad section in each axis");
                //myTarget.QuadNumber = EditorGUILayout.Vector3IntField(quadNumberContent, myTarget.QuadNumber);

                //GUIContent faceToDrawContent = new GUIContent(nameof(myTarget.FaceToDraw), "tell wich of cube faces to draw");
                //myTarget.FaceToDraw = (Face)EditorGUILayout.EnumFlagsField(faceToDrawContent, myTarget.FaceToDraw);

                GUIContent doubleSidedMeshContent = new GUIContent(nameof(myTarget.DoubleSidedMesh), "tell to build an opposite triangle to every triangles, pretty heavy on performances");
                myTarget.DoubleSidedMesh = EditorGUILayout.Toggle(doubleSidedMeshContent, myTarget.DoubleSidedMesh);

                GUIContent flipNormalsContent = new GUIContent(nameof(myTarget.FlipNormals), "draw everything upside down");
                myTarget.FlipNormals = EditorGUILayout.Toggle(flipNormalsContent, myTarget.FlipNormals);

                GUIContent pivotPositionContent = new GUIContent(nameof(myTarget.PivotPosition), "set position of pivot compared to mesh");
                myTarget.PivotPosition = (Pivot)EditorGUILayout.EnumPopup(pivotPositionContent, myTarget.PivotPosition);

                GUIContent hardEdgesContent = new GUIContent(nameof(myTarget.HardEdges), "draw separate vertices at edges");
                myTarget.HardEdges = EditorGUILayout.Toggle(hardEdgesContent, myTarget.HardEdges);

                //GUIContent preserveQuadsRatioWithScaleContent = new GUIContent(nameof(myTarget.PreserveQuadsRatioWithScale), "draw separate vertices at edges");
                //myTarget.PreserveQuadsRatioWithScale = EditorGUILayout.Toggle(preserveQuadsRatioWithScaleContent, myTarget.PreserveQuadsRatioWithScale);
            }
            EditorGUILayout.EndVertical();
        }
    }
}
