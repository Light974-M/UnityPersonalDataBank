using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(TerrainGenerator)), CanEditMultipleObjects]
public class TerrainCustomInspector : Editor
{
    private bool drawGizmoFirstChange = false;
    private bool perlinNoiseScaleFirstChange = true;
    private Vector2 noiseMemoScale = Vector3.zero;

    public override void OnInspectorGUI()
    {
        TerrainGenerator myTarget = (TerrainGenerator)target;

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

        myTarget.XSize = EditorGUILayout.IntField(new GUIContent("XSize", "number of vertices in X"), myTarget.XSize);
        myTarget.ZSize = EditorGUILayout.IntField(new GUIContent("ZSize", "number of vertices in Z"), myTarget.ZSize);
        myTarget.ResizeAndNoiseScale = EditorGUILayout.Toggle(new GUIContent("Resize & Noise Scale", "does it resize noise scale while resize x and z values"), myTarget.ResizeAndNoiseScale);
        myTarget.EnableProceduralAnimations = EditorGUILayout.Toggle(new GUIContent("Enable Procedural Animations(play mode)", "can shape move and update procedurally in play mode ?"), myTarget.EnableProceduralAnimations);
        myTarget.NoiseScale = EditorGUILayout.Vector2Field(new GUIContent("Noise Scale","scale of the noise generated(fully disconected from other scale values)"), myTarget.NoiseScale);
        myTarget.NoiseOffset = EditorGUILayout.Vector2Field(new GUIContent("Noise Offset","offset of noise"), myTarget.NoiseOffset);
        myTarget.HeightColor = EditorGUILayout.Toggle(new GUIContent("Height Color", "does game generated height color texture"), myTarget.HeightColor);

        if (myTarget.ResizeAndNoiseScale)
        {
            if(perlinNoiseScaleFirstChange)
            {
                noiseMemoScale = new Vector2((float)myTarget.XSize / myTarget.NoiseScale.x, (float)myTarget.ZSize / myTarget.NoiseScale.y);
                perlinNoiseScaleFirstChange = false;
            }

            myTarget.NoiseScale = new Vector2((float)myTarget.XSize / noiseMemoScale.x, (float)myTarget.ZSize / noiseMemoScale.y);
        }
        else
        {
            perlinNoiseScaleFirstChange = true;
        }
        GUILayout.EndVertical();
        GUILayout.Space(20);
        #endregion

        #region GIZMOS
        GUILayout.BeginVertical("HelpBox");
        GUILayout.Label("GIZMOS", EditorStyles.boldLabel);

        myTarget.DrawVerticesGizmo = EditorGUILayout.Toggle(new GUIContent("Draw Gizmos","does engine draw vertices gizmos"), myTarget.DrawVerticesGizmo);

        if(myTarget.DrawVerticesGizmo)
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

        GUILayout.Space(20);
        GUILayout.BeginVertical("HelpBox");
        GUILayout.Label("PERLIN NOISE GENERATION", EditorStyles.boldLabel);

        myTarget.NoiseEveryFrame = EditorGUILayout.Toggle(new GUIContent("Noise Every Frame", "does game generate noise every frame ? (can be used for procedural animations preview)"), myTarget.NoiseEveryFrame);
        GUILayout.BeginHorizontal("box");

        if (!myTarget.NoiseEveryFrame)
        {
            if (GUILayout.Button("GENERATE PERLIN NOISE"))
            {
                myTarget.LoadElements();
                myTarget.CreatePerlinNoise();
                myTarget.UpdateMesh();
            }
            if (GUILayout.Button("RESET AND GENERATE"))
            {
                myTarget.LoadElements();
                myTarget.CreateShape();
                myTarget.CreatePerlinNoise();
                myTarget.UpdateMesh();
            } 
        }
        else
        {
            myTarget.LoadElements();
            myTarget.CreateShape();
            myTarget.CreatePerlinNoise();
            myTarget.UpdateMesh();
        }

        GUILayout.EndHorizontal();
        GUILayout.EndVertical();
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