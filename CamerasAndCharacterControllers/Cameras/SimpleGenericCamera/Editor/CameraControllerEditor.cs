using UnityEditor;
using UnityEngine;

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

                GUIContent inputSystemContent = new GUIContent(nameof(myTarget.InputSystem), "do camera use input system or native input ?");
                myTarget.InputSystem = EditorGUILayout.Toggle(inputSystemContent, myTarget.InputSystem);

                GUIContent lookSpeedContent = new GUIContent(nameof(myTarget.LookSpeed), "speed of mouse look in X and Y");
                myTarget.LookSpeed = EditorGUILayout.Vector2Field(lookSpeedContent, myTarget.LookSpeed);

                GUIContent verticalBordersContent = new GUIContent(nameof(myTarget.VerticalBorders), "degrees of angles to clamp camera vertically");
                myTarget.VerticalBorders = EditorGUILayout.Vector2Field(verticalBordersContent, myTarget.VerticalBorders);

                GUIContent horizontalPivotContent = new GUIContent(nameof(myTarget.HorizontalPivot), "transform to rotate on y axis, null means this transform");
                myTarget.HorizontalPivot = (Transform)EditorGUILayout.ObjectField(horizontalPivotContent, myTarget.HorizontalPivot, typeof(Transform), true);

                GUIContent verticalPivotContent = new GUIContent(nameof(myTarget.VerticalPivot), "transform to rotate on x axis, null means this transform");
                myTarget.VerticalPivot = (Transform)EditorGUILayout.ObjectField(verticalPivotContent, myTarget.VerticalPivot, typeof(Transform), true);

                GUIContent hideCursorContent = new GUIContent(nameof(myTarget.HideCursor), "if enabled, will hide the cursor while focused");
                myTarget.HideCursor = EditorGUILayout.Toggle(hideCursorContent, myTarget.HideCursor);

                GUIContent cursorLockStateContent = new GUIContent(nameof(myTarget.CursorLockState), "type of constraint for mouse");
                myTarget.CursorLockState = (CursorLockMode)EditorGUILayout.EnumPopup(cursorLockStateContent, myTarget.CursorLockState);
            }
            EditorGUILayout.EndVertical();



            EditorGUILayout.LabelField("__________________________________________________________________________________________________________________________________________________________________________________________________________________________________________________");

            EditorGUILayout.BeginVertical("helpBox");
            {
                EditorGUILayout.LabelField("CAMERA FX");

                GUIContent cameraFXTargetContent = new GUIContent(nameof(myTarget.CameraFXTarget), "reference of camera for every camera effects, null means that there is no camera effects");
                myTarget.CameraFXTarget = (Camera)EditorGUILayout.ObjectField(cameraFXTargetContent, myTarget.CameraFXTarget, typeof(Camera), true);

                if (myTarget.CameraFXTarget)
                {
                    GUIContent fOVSystemContent = new GUIContent(nameof(myTarget.FOVSystem), "reference of camera for every camera effects, null means that there is no camera effects");
                    myTarget.FOVSystem = EditorGUILayout.Toggle(fOVSystemContent, myTarget.FOVSystem);

                    if (myTarget.FOVSystem)
                    {
                        EditorGUILayout.BeginVertical("box");
                        {
                            GUIContent defaultPOVContent = new GUIContent(nameof(myTarget.DefaultFOV), "default value of FOV, when there is no movements");
                            myTarget.DefaultFOV = EditorGUILayout.FloatField(defaultPOVContent, myTarget.DefaultFOV);

                            GUIContent fOVMinMaxContent = new GUIContent(nameof(myTarget.FOVMinMax), "minimum and maximum value reachable by FOV");
                            myTarget.FOVMinMax = EditorGUILayout.Vector2Field(fOVMinMaxContent, myTarget.FOVMinMax);

                            GUIContent fOVIntensityContent = new GUIContent(nameof(myTarget.FOVIntensity), "multiplier, the higher the value, the higher fov will increase with speed");
                            myTarget.FOVIntensity = EditorGUILayout.FloatField(fOVIntensityContent, myTarget.FOVIntensity);

                            GUIContent fOVEvolutionShapeContent = new GUIContent(nameof(myTarget.FOVEvolutionShape), "curve that give precise override values of FOV Between it's min and max value");
                            myTarget.FOVEvolutionShape = EditorGUILayout.CurveField(fOVEvolutionShapeContent, myTarget.FOVEvolutionShape);

                            GUIContent fOVVelocityClampContent = new GUIContent(nameof(myTarget.FOVVelocityClamp), "clamp FOV increasing or decreasing speed");
                            myTarget.FOVVelocityClamp = EditorGUILayout.FloatField(fOVVelocityClampContent, myTarget.FOVVelocityClamp);

                            GUIContent fOVAccelerationClampIncrementContent = new GUIContent(nameof(myTarget.FOVAccelerationClampIncrement), "make a smooth on every movements of FOV, to avoid brutal speed changing");
                            myTarget.FOVAccelerationClampIncrement = EditorGUILayout.Slider(fOVAccelerationClampIncrementContent, myTarget.FOVAccelerationClampIncrement, 0, 1);
                        }
                        EditorGUILayout.EndVertical();
                    }

                    myTarget.CameraShakeSystem = EditorGUILayout.Toggle(new GUIContent(nameof(myTarget.CameraShakeSystem), "reference of camera for every camera effects, null means that there is no camera effects"), myTarget.CameraShakeSystem);

                    if (myTarget.CameraShakeSystem)
                    {
                        EditorGUILayout.BeginVertical("box");
                        {
                            GUIContent shakeTimeContent = new GUIContent(nameof(myTarget.ShakeTime), "duration of one camera shake in sec");
                            myTarget.ShakeTime = EditorGUILayout.FloatField(shakeTimeContent, myTarget.ShakeTime);

                            GUIContent shakeIntensityContent = new GUIContent(nameof(myTarget.ShakeIntensity), "intensity of shake, means min and max angle camera can have");
                            myTarget.ShakeIntensity = EditorGUILayout.FloatField(shakeIntensityContent, myTarget.ShakeIntensity);
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

