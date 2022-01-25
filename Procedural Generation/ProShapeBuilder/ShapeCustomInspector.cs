using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(ShapeGenerator)), CanEditMultipleObjects]
public class ShapeCustomInspector : Editor
{
    private bool drawGizmoFirstChange = false;

    public override void OnInspectorGUI()
    {
        ShapeGenerator myTarget = (ShapeGenerator)target;

        #region DRAW DEFAULT INSPECTOR
        if (myTarget.DrawDefaultInspector)
        {
            GUILayout.BeginVertical("HelpBox");
            GUILayout.Label("DEFAULT INSPECTOR", EditorStyles.boldLabel);

            base.OnInspectorGUI();

            GUILayout.EndVertical();
            GUILayout.Space(20);
        }
        #endregion

        #region MESH TRANSFORM
        GUILayout.BeginVertical("HelpBox");
        GUILayout.Label("MESH TRANSFORM", EditorStyles.boldLabel);

        myTarget.Scale = EditorGUILayout.Vector3Field(new GUIContent("Scale", "mesh scale(disctinct from default scale, is used to resize mesh before every calculations)"), myTarget.Scale);

        GUILayout.EndVertical();
        GUILayout.Space(20);
        #endregion

        #region ELEMENTS & VARIABLES
        GUILayout.BeginVertical("helpBox");
        GUILayout.Label("ELEMENTS & VARIABLES", EditorStyles.boldLabel);

        if (GUILayout.Button("Clean"))
            myTarget.Clean();

        if (GUILayout.Button("Initialize Variables"))
            myTarget.LoadElements();

        GUILayout.EndVertical();
        GUILayout.Space(20);
        #endregion

        #region MESH ASSIGNMENT
        GUILayout.BeginVertical("helpBox");
        GUILayout.Label("MESH ASSIGNMENT", EditorStyles.boldLabel);

        if (GUILayout.Button("Clear Vertices & Triangles"))
            myTarget.ClearVerticesAndTriangles();

        if (GUILayout.Button("Clear Mesh"))
            myTarget.ClearMesh();

        if (GUILayout.Button("Clear Mesh Completely"))
        {
            myTarget.ClearVerticesAndTriangles();
            myTarget.UpdateMesh();
        }

        if (GUILayout.Button("Update Mesh"))
            myTarget.UpdateMesh();

        GUILayout.EndVertical();
        GUILayout.Space(20);
        #endregion

        #region TERRAIN PARAMETERS
        GUILayout.BeginVertical("HelpBox");
        GUILayout.Label("TERRAIN PARAMETERS", EditorStyles.boldLabel);

        myTarget.VerticesNumber = EditorGUILayout.IntField(new GUIContent("Vertices Number", "number of vertices in X"), myTarget.VerticesNumber);
        myTarget.EnableProceduralAnimations = EditorGUILayout.Toggle(new GUIContent("Enable Procedural Animations(play mode)", "can shape move and update procedurally in play mode ?"), myTarget.EnableProceduralAnimations);

        GUILayout.EndVertical();
        GUILayout.Space(20);
        #endregion

        #region GIZMOS
        GUILayout.BeginVertical("HelpBox");
        GUILayout.Label("GIZMOS", EditorStyles.boldLabel);

        myTarget.DrawVerticesGizmo = EditorGUILayout.Toggle(new GUIContent("Draw Gizmos", "does engine draw vertices gizmos"), myTarget.DrawVerticesGizmo);

        if (myTarget.DrawVerticesGizmo)
        {
            if (drawGizmoFirstChange)
            {
                myTarget.UpdateMesh();
                drawGizmoFirstChange = false;
            }

            GUILayout.BeginHorizontal();
            GUILayout.Label("Gizmo Size");
            myTarget.GizmoSize = EditorGUILayout.Slider(myTarget.GizmoSize, 0, 5);
            GUILayout.EndHorizontal();

            myTarget.FixedGizmos = EditorGUILayout.Toggle(new GUIContent("Fixed Gizmos", "draw fixed sized gizmos"), myTarget.FixedGizmos);

            GUILayout.BeginHorizontal();
            GUILayout.Label("Constant Gizmo Size");
            myTarget.ConstantGizmoSize = EditorGUILayout.Slider(myTarget.ConstantGizmoSize, 0, 1);
            GUILayout.EndHorizontal();
        }
        else
        {
            if (!drawGizmoFirstChange)
            {
                myTarget.UpdateMesh();
                drawGizmoFirstChange = true;
            }
        }

        GUILayout.EndVertical();
        GUILayout.Space(20);
        #endregion

        #region MESH GENERATION
        GUILayout.BeginVertical("HelpBox");
        GUILayout.Label("MESH GENERATION", EditorStyles.boldLabel);

        myTarget.GenerateEveryFrame = EditorGUILayout.Toggle(new GUIContent("Generate Every Frame", "does game generate new shape every frame ? (can be used for procedural animations preview)"), myTarget.GenerateEveryFrame);

        if (!myTarget.GenerateEveryFrame)
        {
            if (GUILayout.Button("\nGENERATE MESH\n"))
            {
                myTarget.LoadElements();
                myTarget.CreateShape();
                myTarget.UpdateMesh();
            }
        }
        else
        {
            myTarget.LoadElements();
            myTarget.CreateShape();
            myTarget.UpdateMesh();
        }

        GUILayout.EndVertical();
        GUILayout.Space(20);
        #endregion

        #region CLEAR ALL
        if (GUILayout.Button("\nCLEAR ALL\n"))
        {
            myTarget.ClearAll();
        }

        GUILayout.Space(20);
        #endregion

        #region CUSTOM INSPECTOR PARAMETERS
        GUILayout.BeginVertical("HelpBox");
        GUILayout.Label("CUSTOM INSPECTOR PARAMETERS", EditorStyles.boldLabel);

        myTarget.DrawDefaultInspector = EditorGUILayout.Toggle(new GUIContent("Draw Default Inspector", "acces to root variables)"), myTarget.DrawDefaultInspector);

        GUILayout.EndVertical();
        #endregion


    }
}
