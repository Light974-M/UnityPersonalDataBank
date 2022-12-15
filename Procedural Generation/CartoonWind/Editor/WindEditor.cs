using UnityEditor;
using UnityEngine;

namespace UPDB.ProceduralGeneration.CartoonWind
{
    [CustomEditor(typeof(WindInstancier))]
    public class WindEditor : Editor
    {
        private BuiltInConfig _presetConfigState = BuiltInConfig.FrequentLoop;
        private BuiltInConfig _loopState = BuiltInConfig.NormalLength;
        private BuiltInConfig _widthState = BuiltInConfig.FrequentLoop;
        private BuiltInConfig _lengthState = BuiltInConfig.FrequentLoop;

        public override void OnInspectorGUI()
        {
            WindInstancier myTarget = target as WindInstancier;

            EditorGUILayout.BeginVertical("helpBox");
            {
                EditorGUILayout.LabelField("PRESETS", EditorStyles.boldLabel);

                EditorGUILayout.BeginHorizontal();
                {
                    if (_presetConfigState != BuiltInConfig.LowWind)
                    {
                        if (GUILayout.Button("Low Wind"))
                        {
                            myTarget.SetBuiltInConfig(BuiltInConfig.LowWind);
                            _presetConfigState = BuiltInConfig.LowWind;
                        }
                    }
                    else
                    {
                        GUI.enabled = false;
                        if (GUILayout.Button("Low Wind"))
                        {

                        }
                        GUI.enabled = true;
                    }

                    if (_presetConfigState != BuiltInConfig.MediumWind)
                    {
                        if (GUILayout.Button("Medium Wind"))
                        {
                            myTarget.SetBuiltInConfig(BuiltInConfig.MediumWind);
                            _presetConfigState = BuiltInConfig.MediumWind;
                        }
                    }
                    else
                    {
                        GUI.enabled = false;
                        if (GUILayout.Button("Medium Wind"))
                        {

                        }
                        GUI.enabled = true;
                    }

                    if (_presetConfigState != BuiltInConfig.HighWind)
                    {
                        if (GUILayout.Button("High Wind"))
                        {
                            myTarget.SetBuiltInConfig(BuiltInConfig.HighWind);
                            _presetConfigState = BuiltInConfig.HighWind;
                        }
                    }
                    else
                    {
                        GUI.enabled = false;
                        if (GUILayout.Button("High Wind"))
                        {

                        }
                        GUI.enabled = true;
                    }


                }
                EditorGUILayout.EndHorizontal();

                EditorGUILayout.LabelField("Debug Presets");

                EditorGUILayout.BeginHorizontal();
                {
                    if (_presetConfigState != BuiltInConfig.EveryFrameDebug)
                    {
                        if (GUILayout.Button("Every Frame Debug"))
                        {
                            myTarget.SetBuiltInConfig(BuiltInConfig.EveryFrameDebug);
                            _presetConfigState = BuiltInConfig.EveryFrameDebug;
                        }
                    }
                    else
                    {
                        GUI.enabled = false;
                        if (GUILayout.Button("Every Frame Debug"))
                        {

                        }
                        GUI.enabled = true;
                    }

                    if (_presetConfigState != BuiltInConfig.LoopDebug)
                    {
                        if (GUILayout.Button("Loop Debug"))
                        {
                            myTarget.SetBuiltInConfig(BuiltInConfig.LoopDebug);
                            _presetConfigState = BuiltInConfig.LoopDebug;
                        }
                    }
                    else
                    {
                        GUI.enabled = false;
                        if (GUILayout.Button("Loop Debug"))
                        {

                        }
                        GUI.enabled = true;
                    }
                }
                EditorGUILayout.EndHorizontal();

                EditorGUILayout.LabelField("preset blend");

                EditorGUILayout.BeginHorizontal();
                {
                    if (_loopState != BuiltInConfig.NoLoop)
                    {
                        if (GUILayout.Button("No Loop"))
                        {
                            myTarget.SetBuiltInConfig(BuiltInConfig.NoLoop);
                            _loopState = BuiltInConfig.NoLoop;
                        }
                    }
                    else
                    {
                        GUI.enabled = false;
                        if (GUILayout.Button("No Loop"))
                        {

                        }
                        GUI.enabled = true;
                    }

                    if (_loopState != BuiltInConfig.NormalLoop)
                    {
                        if (GUILayout.Button("Normal Loop"))
                        {
                            myTarget.SetBuiltInConfig(BuiltInConfig.NormalLoop);
                            _loopState = BuiltInConfig.NormalLoop;
                        }
                    }
                    else
                    {
                        GUI.enabled = false;
                        if (GUILayout.Button("Normal Loop"))
                        {

                        }
                        GUI.enabled = true;
                    }

                    if (_loopState != BuiltInConfig.FrequentLoop)
                    {
                        if (GUILayout.Button("Frequent Loop"))
                        {
                            myTarget.SetBuiltInConfig(BuiltInConfig.FrequentLoop);
                            _loopState = BuiltInConfig.FrequentLoop;
                        }
                    }
                    else
                    {
                        GUI.enabled = false;
                        if (GUILayout.Button("Frequent Loop"))
                        {

                        }
                        GUI.enabled = true;
                    }
                }
                EditorGUILayout.EndHorizontal();

                EditorGUILayout.BeginHorizontal();
                {
                    if (_widthState != BuiltInConfig.SmallWidth)
                    {
                        if (GUILayout.Button("Small Width"))
                        {
                            myTarget.SetBuiltInConfig(BuiltInConfig.SmallWidth);
                            _widthState = BuiltInConfig.SmallWidth;
                        }
                    }
                    else
                    {
                        GUI.enabled = false;
                        if (GUILayout.Button("Small Width"))
                        {

                        }
                        GUI.enabled = true;
                    }


                    if (_widthState != BuiltInConfig.NormalWidth)
                    {
                        if (GUILayout.Button("Normal Width"))
                        {
                            myTarget.SetBuiltInConfig(BuiltInConfig.NormalWidth);
                            _widthState = BuiltInConfig.NormalWidth;
                        }
                    }
                    else
                    {
                        GUI.enabled = false;
                        if (GUILayout.Button("Normal Width"))
                        {

                        }
                        GUI.enabled = true;
                    }

                    if (_widthState != BuiltInConfig.LargeWidth)
                    {
                        if (GUILayout.Button("Large Width"))
                        {
                            myTarget.SetBuiltInConfig(BuiltInConfig.LargeWidth);
                            _widthState = BuiltInConfig.LargeWidth;
                        }
                    }
                    else
                    {
                        GUI.enabled = false;
                        if (GUILayout.Button("Large Width"))
                        {

                        }
                        GUI.enabled = true;
                    }
                }
                EditorGUILayout.EndHorizontal();

                EditorGUILayout.BeginHorizontal();
                {
                    if (_lengthState != BuiltInConfig.SmallLength)
                    {
                        if (GUILayout.Button("Small Length"))
                        {
                            myTarget.SetBuiltInConfig(BuiltInConfig.SmallLength);
                            _lengthState = BuiltInConfig.SmallLength;
                        }
                    }
                    else
                    {
                        GUI.enabled = false;
                        if (GUILayout.Button("Small Length"))
                        {

                        }
                        GUI.enabled = true;
                    }

                    if (_lengthState != BuiltInConfig.NormalLength)
                    {
                        if (GUILayout.Button("Normal Length"))
                        {
                            myTarget.SetBuiltInConfig(BuiltInConfig.NormalLength);
                            _lengthState = BuiltInConfig.NormalLength;
                        }
                    }
                    else
                    {
                        GUI.enabled = false;
                        if (GUILayout.Button("Normal Length"))
                        {

                        }
                        GUI.enabled = true;
                    }

                    if (_lengthState != BuiltInConfig.HighLength)
                    {
                        if (GUILayout.Button("High Length"))
                        {
                            myTarget.SetBuiltInConfig(BuiltInConfig.HighLength);
                            _lengthState = BuiltInConfig.HighLength;
                        }
                    }
                    else
                    {
                        GUI.enabled = false;
                        if (GUILayout.Button("High Length"))
                        {

                        }
                        GUI.enabled = true;
                    }
                }
                EditorGUILayout.EndHorizontal();
            }
            EditorGUILayout.EndVertical();

            EditorGUILayout.Space(20);

            EditorGUILayout.BeginVertical("helpBox");
            {
                EditorGUILayout.LabelField("ACTIVE CONFIG", EditorStyles.boldLabel);
                myTarget.WindConfig = (WindAsset)EditorGUILayout.ObjectField("Wind Config", myTarget.WindConfig, typeof(WindAsset), true);
            }
            EditorGUILayout.EndVertical();

            EditorGUILayout.Space(10);
            EditorGUILayout.LabelField("___________________________________________________________________________________________________________________________________________________________________________________________");
            EditorGUILayout.Space(10);

            if (GUILayout.Button("\nClear All\n"))
                myTarget.ClearConfig();
        }
    }
    //public enum BuiltInConfig
    //{
    //    LowWind,
    //    MediumWind,
    //    HighWind,
    //    EveryFrameDebug,
    //    LoopDebug,
    //    NoLoop,
    //    NormalLoop,
    //    FrequentLoop,
    //    SmallWidth,
    //    NormalWidth,
    //    LargeWidth,
    //    SmallLength,
    //    NormalLength,
    //    HighLength,
    //}
}

