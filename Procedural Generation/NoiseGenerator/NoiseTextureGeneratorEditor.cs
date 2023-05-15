using UnityEditor;
using UnityEngine;
using System.IO;
using System.Reflection;
using System.Collections.Generic;
using System;

namespace UPDB.ProceduralGeneration.NoiseGenerator
{
    ///<summary>
    /// 
    ///</summary>
    public class NoiseTextureGeneratorEditor : EditorWindow
    {
        private const float INSPECTOR_WIDHT = 280f;
        private const float MAXSIZE_FOR_CONSTANT_REPAINT = 100000;

        private bool _constantRepaint = false;

        [SerializeField]
        private string _selectedGeneratorName = string.Empty;

        private Type[] _generatorTypes = null;
        private string[] _generatorTypesNames = null;

        private NoiseTextureGenerator _activeGenerator = null;
        private Editor _activeGeneratorEditor = null;

        private Texture2D _texture = null;

        [MenuItem("Tools/Noise Texture Generator")]
        private static void Open()
        {
            NoiseTextureGeneratorEditor window = GetWindow<NoiseTextureGeneratorEditor>(false, "Noise Texture Generator", true);
            window.Show();
        }

        private void OnEnable()
        {
            List<Type> generatorTypesList = new List<Type>();
            List<string> generatorTypesNamesList = new List<string>();

            foreach (Type type in typeof(NoiseTextureGenerator).Assembly.GetTypes())
            {
                //Debug.Log(type.FullName);
                if (type.IsSubclassOf(typeof(NoiseTextureGenerator)))
                {
                    generatorTypesList.Add(type);
                    generatorTypesNamesList.Add(type.FullName);
                }
            }

            _generatorTypes = generatorTypesList.ToArray();
            _generatorTypesNames = generatorTypesNamesList.ToArray();
        }

        private void OnDisable()
        {
            DestroyImmediate(_activeGenerator);
            DestroyImmediate(_activeGeneratorEditor);
        }
        private void OnGUI()
        {
            int previousIndex = ArrayUtility.FindIndex(_generatorTypesNames, typeName => typeName == _selectedGeneratorName);
            if (previousIndex < 0)
            {
                previousIndex = 0;
            }

            int newIndex = EditorGUILayout.Popup("Noise Generator", previousIndex, _generatorTypesNames);
            if (previousIndex != newIndex)
            {
                SelectGenerator(_generatorTypes[newIndex]);
            }


            if (_activeGenerator == null)
            {
                SelectGenerator(_generatorTypes[newIndex]);
            }

            _constantRepaint = EditorGUILayout.Toggle("Constant repaint", _constantRepaint);

            EditorGUILayout.Space();

            using (new EditorGUILayout.HorizontalScope())
            {
                using (new GUILayout.VerticalScope(GUILayout.Width(INSPECTOR_WIDHT)))
                {
                    DrawActiveGeneratorInspector();
                }

                using (new GUILayout.VerticalScope())
                {
                    DrawTexturePreview();
                }
            }

            if(_activeGenerator != null && (_activeGenerator.TextureSize.x * _activeGenerator.TextureSize.y) > MAXSIZE_FOR_CONSTANT_REPAINT)
            {
                _constantRepaint = false;
            }

            if(_constantRepaint)
            {
                _texture = _activeGenerator.Generate();
                Repaint();
            }
        }

        private void DrawActiveGeneratorInspector()
        {
            EditorGUILayout.LabelField("Selected Generator Settings", EditorStyles.largeLabel);
            EditorGUILayout.Space();
            _activeGeneratorEditor.OnInspectorGUI();

            EditorGUILayout.Space();
            if (GUILayout.Button("Generate"))
            {
                _texture = _activeGenerator.Generate();
            }

            if (GUILayout.Button("Save as PNG"))
            {
                string path = EditorUtility.SaveFilePanel("Save Texture as PNG", null, _activeGenerator.GetType() + ".png", "png");

                if (!string.IsNullOrEmpty(path))
                {
                    byte[] textureBytes = _texture.EncodeToPNG();
                    File.WriteAllBytes(path, textureBytes);
                    AssetDatabase.Refresh();
                }
            }
        }
        private void DrawTexturePreview()
        {
            if (_texture == null)
            {
                _texture = _activeGenerator.Generate();
            }

            Rect rect = EditorGUILayout.GetControlRect(GUILayout.ExpandHeight(true));
            GUI.DrawTexture(rect, _texture, ScaleMode.ScaleToFit);
        }

        private void SelectGenerator(Type generatorType)
        {
            _selectedGeneratorName = generatorType.FullName;

            if (_activeGenerator != null)
            {
                DestroyImmediate(_activeGenerator);
                DestroyImmediate(_activeGeneratorEditor);
            }

            _activeGenerator = CreateInstance(generatorType) as NoiseTextureGenerator;
            _activeGeneratorEditor = Editor.CreateEditor(_activeGenerator);
        }
    }
}

