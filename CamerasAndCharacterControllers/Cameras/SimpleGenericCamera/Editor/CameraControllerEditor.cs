using Codice.Client.Common.GameUI;
using UnityEditor;
using UnityEngine;
using UnityEngine.TestTools;

namespace UPDB.CamerasAndCharacterControllers.Cameras.SimpleGenericCamera
{
    ///<summary>
    /// editor to display tools and insector of cameraController component
    ///</summary>
    [CustomEditor(typeof(CameraController))]
    public class CameraControllerEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            CameraController myTarget = (CameraController)target;

            EditorGUILayout.BeginVertical("helpBox");
            {
                EditorGUILayout.LabelField("DEFAULT");

                myTarget.LookSpeed = EditorGUILayout.Vector2Field(new GUIContent(nameof(myTarget.LookSpeed), "speed of mouse look in X and Y"), myTarget.LookSpeed);
                myTarget.VerticalBorders = EditorGUILayout.Vector2Field(new GUIContent(nameof(myTarget.VerticalBorders), "degrees of angles to clamp camera vertically"), myTarget.VerticalBorders);
                myTarget.InputSystem = EditorGUILayout.Toggle(new GUIContent(nameof(myTarget.InputSystem), "do camera use input system or native input ?"), myTarget.InputSystem);
            }
            EditorGUILayout.EndVertical();

            

            EditorGUILayout.LabelField("__________________________________________________________________________________________________________________________________________________________________________________________________________________________________________________");

            EditorGUILayout.BeginVertical("helpBox");
            {
                EditorGUILayout.LabelField("CAMERA FX");

                myTarget.CameraFXTarget = (Camera)EditorGUILayout.ObjectField(new GUIContent(nameof(myTarget.CameraFXTarget), "reference of camera for every camera effects, null means that there is no camera effects"), myTarget.CameraFXTarget, typeof(Camera), true);

                if (myTarget.CameraFXTarget)
                {
                    myTarget.FOVSystem = EditorGUILayout.Toggle(new GUIContent(nameof(myTarget.FOVSystem), "reference of camera for every camera effects, null means that there is no camera effects"), myTarget.FOVSystem);

                    if (myTarget.FOVSystem)
                    {
                        EditorGUILayout.BeginVertical("box");
                        {
                            myTarget.DefaultFOV = EditorGUILayout.FloatField(new GUIContent(nameof(myTarget.DefaultFOV), "default value of FOV, when there is no movements"), myTarget.DefaultFOV);
                            myTarget.FOVMinMax = EditorGUILayout.Vector2Field(new GUIContent(nameof(myTarget.FOVMinMax), "minimum and maximum value reachable by FOV"), myTarget.FOVMinMax);
                            myTarget.FOVIntensity = EditorGUILayout.FloatField(new GUIContent(nameof(myTarget.FOVIntensity), "multiplier, the higher the value, the higher fov will increase with speed"), myTarget.FOVIntensity);
                            myTarget.FOVEvolutionShape = EditorGUILayout.CurveField(new GUIContent(nameof(myTarget.FOVEvolutionShape), "curve that give precise override values of FOV Between it's min and max value"), myTarget.FOVEvolutionShape);
                            myTarget.FOVVelocityClamp = EditorGUILayout.FloatField(new GUIContent(nameof(myTarget.FOVVelocityClamp), "clamp FOV increasing or decreasing speed"), myTarget.FOVVelocityClamp);
                            myTarget.FOVAccelerationClampIncrement = EditorGUILayout.Slider(new GUIContent(nameof(myTarget.FOVAccelerationClampIncrement), "make a smooth on every movements of FOV, to avoid brutal speed changing"), myTarget.FOVAccelerationClampIncrement, 0, 1);
                        }
                        EditorGUILayout.EndVertical();  
                    }

                    myTarget.CameraShakeSystem = EditorGUILayout.Toggle(new GUIContent(nameof(myTarget.CameraShakeSystem), "reference of camera for every camera effects, null means that there is no camera effects"), myTarget.CameraShakeSystem);

                    if (myTarget.CameraShakeSystem)
                    {
                        EditorGUILayout.BeginVertical("box");
                        {
                            myTarget.ShakeTime = EditorGUILayout.FloatField(new GUIContent(nameof(myTarget.ShakeTime), "duration of one camera shake in sec"), myTarget.ShakeTime);
                            myTarget.ShakeIntensity = EditorGUILayout.FloatField(new GUIContent(nameof(myTarget.ShakeIntensity), "intensity of shake, means min and max angle camera can have"), myTarget.ShakeIntensity);
                        }
                        EditorGUILayout.EndVertical();
                    }
                }
            }
            EditorGUILayout.EndVertical();

            //EditorGUILayout.LabelField("__________________________________________________________________________________________________________________________________________________________________________________________________________________________________________________");
            //EditorGUILayout.LabelField("DEBUG");

            //if (!myTarget.IsTesting)
            //{
            //    if (GUILayout.Button("Launch Test"))
            //    {
            //        myTarget.IsTesting = true;
            //    } 
            //}
            //else
            //{
            //    myTarget.Look();

            //    GUI.color = new Color(0.5f, 0.5f, 0.5f, 1);

            //    if (GUILayout.Button("Launch Test"))
            //    {
            //        myTarget.IsTesting = false;
            //    }

            //    GUI.color = Color.white;
            //}
        }
    }
}

