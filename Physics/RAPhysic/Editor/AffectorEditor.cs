using UnityEngine;
using UnityEditor;
using UPDB.CoreHelper;

namespace UPDB.Physic.RAPhysic
{
    /// <summary>
    /// Custom inspector of monoBehaviour Affector Class for RAPhysic
    /// </summary>
    [CustomEditor(typeof(Affector)), CanEditMultipleObjects, HelpURL(URL.baseURL + "/tree/main/Physics/RAPhysic/README.md")]
    public class AffectorEditor : Editor
    {
        /// <summary>
        /// Called when and update is detected in inspector
        /// </summary>
        public override void OnInspectorGUI()
        {
            //current object being inspected
            Affector myTarget = (Affector)target;

            //if engine is in editor mode
            if (!Application.isPlaying)
                myTarget.InitVariables();

            //show custom inspector only if DrawDefaultInspector is set to false, otherwise, draw default inspector
            if (myTarget.DrawDefaultInspector)
                base.OnInspectorGUI();
            else
            {
                /*****************PHYSIC******************/
                GUILayout.BeginVertical("box");
                {
                    GUILayout.Label("PHYSIC____________________________________________________________________________________", EditorStyles.boldLabel);
                    GUILayout.BeginVertical("helpBox");
                    GUILayout.Space(20);
                    {
                        EditorGUILayout.PropertyField(serializedObject.FindProperty("_usedPhysicSystemBase"), new GUIContent("Used Physic System", "determine wich physic system to use for applying forces"));
                        serializedObject.ApplyModifiedProperties();

                        if (myTarget.UsedPhysicSystemBase == PhysicType.Rigidbody || myTarget.UsedPhysicSystemBase == PhysicType.Dynamic)
                            myTarget.Rb = (Rigidbody)EditorGUILayout.ObjectField("Rigidbody", myTarget.Rb, typeof(Rigidbody), true);

                        if (myTarget.UsedPhysicSystemBase == PhysicType.CharacterController || myTarget.UsedPhysicSystemBase == PhysicType.Dynamic)
                            myTarget.CharaController = (CharacterController)EditorGUILayout.ObjectField("Chara Controller", myTarget.CharaController, typeof(CharacterController), true);

                        if (myTarget.UsedPhysicSystemBase == PhysicType.Native || myTarget.UsedPhysicSystemBase == PhysicType.Dynamic)
                            myTarget.CustomRb = (CustomRigidbody)EditorGUILayout.ObjectField("Custom Rigidbody", myTarget.CustomRb, typeof(CustomRigidbody), true);

                        myTarget.PhysicEnabled = EditorGUILayout.Toggle(new GUIContent("Physic Enabled", "determine if object is in the list and is detected"), myTarget.PhysicEnabled);
                        myTarget.PhysicAffectorEnabled = EditorGUILayout.Toggle(new GUIContent("Physic Affector Enabled", "determine if object generate gravity and apply it to other object"), myTarget.PhysicAffectorEnabled);
                        myTarget.RotationDeformation = EditorGUILayout.Toggle(new GUIContent("Rotation Deformation", "if enabled, planets will flatten compare to their rotation speed"), myTarget.RotationDeformation);

                        if (myTarget.RotationDeformation)
                            myTarget.RotationDeformationFactor = EditorGUILayout.FloatField(new GUIContent("Deformation Factor", "factor for rotation deformation"), myTarget.RotationDeformationFactor);
                    }
                    GUILayout.EndVertical();
                }
                GUILayout.EndVertical();


                GUILayout.Space(30);


                /*****************AIR RESISTANCE******************/
                GUILayout.BeginVertical("box");
                {
                    GUILayout.Label("AIR RESISTANCE_________________________________________________________________________________________________________", EditorStyles.boldLabel);
                    GUILayout.BeginVertical("helpBox");
                    GUILayout.Space(20);
                    {
                        myTarget.AtmosphereEnabled = EditorGUILayout.Toggle(new GUIContent("Atmosphere Enabled", "determine if object as atmosphere and generate air resistance"), myTarget.AtmosphereEnabled);

                        if (myTarget.AtmosphereEnabled)
                        {
                            myTarget.AtmosphereRange = EditorGUILayout.FloatField(new GUIContent("Atmosphere Range", "range of object atmosphere"), myTarget.AtmosphereRange);
                            myTarget.AirDensity = EditorGUILayout.FloatField(new GUIContent("Air Density", "density of air and strength of resistance"), myTarget.AirDensity);
                            myTarget.GraduateAtmosphere = EditorGUILayout.Toggle(new GUIContent("Graduate Atmosphere", "if disabled, atmosphere density will be maximal everywhere"), myTarget.GraduateAtmosphere);
                            myTarget.AtmosphereGradiantShape = EditorGUILayout.CurveField(new GUIContent("Atmosphere Gradiant Shape", "choose degrade shape"), myTarget.AtmosphereGradiantShape);
                        }

                        myTarget.AirResistanceEnabled = EditorGUILayout.Toggle(new GUIContent("Air Resistance Enabled", "determine if the object can detect and calculate its air resistance"), myTarget.AirResistanceEnabled);
                    }
                    GUILayout.EndVertical();
                }
                GUILayout.EndVertical();


                GUILayout.Space(30);

                /*****************GRAVITY******************/
                GUILayout.BeginVertical("box");
                {
                    GUILayout.Label("GRAVITY_____________________________________________________________________________________________________________________________________________", EditorStyles.boldLabel);
                    GUILayout.BeginVertical("helpBox");
                    GUILayout.Space(20);
                    {
                        //CLAMP
                        myTarget.ClampForces = EditorGUILayout.Toggle(new GUIContent("Clamp Forces", "if enabled, will show clamp force panels"), myTarget.ClampForces);

                        if (myTarget.ClampForces)
                        {
                            GUILayout.BeginVertical("helpBox");
                            {
                                GUILayout.Label("Clamp");
                                GUILayout.BeginVertical("helpBox");
                                {
                                    myTarget.ClampGeneratedForce = EditorGUILayout.Toggle(new GUIContent("Clamp Generated Force", "determine if object should have maximum force strength to apply to other objects"), myTarget.ClampGeneratedForce);
                                    myTarget.MaxGeneratedForce = EditorGUILayout.FloatField(new GUIContent("Max Force", "maximum force strength that object can apply to other objects"), myTarget.MaxGeneratedForce);
                                }
                                GUILayout.EndVertical();
                                GUILayout.BeginVertical("helpBox");
                                {
                                    myTarget.ClampAffectedForces = EditorGUILayout.Toggle(new GUIContent("Clamp Affected Forces", "determine if object should clamp every forces apply to it"), myTarget.ClampAffectedForces);
                                    myTarget.MaxAffectedForces = EditorGUILayout.FloatField(new GUIContent("Max Forces", "maximum forces strength that object can receive from other objects"), myTarget.MaxAffectedForces);
                                }
                                GUILayout.EndVertical();
                            }
                            GUILayout.EndVertical();
                        }
                        GUILayout.Space(20);

                        myTarget.IgnoreDistance = EditorGUILayout.Toggle(new GUIContent("Force Ignore Distance", "used for debug, will consider gravity is applied as object is on the surface of planet or object"), myTarget.IgnoreDistance);
                        myTarget.MatterDensity = EditorGUILayout.FloatField(new GUIContent("Matter Density", "density of mass object(in kg/m3, default value is 1kg for a 1 diameter sphere)"), myTarget.MatterDensity);
                        myTarget.Mass = EditorGUILayout.FloatField(new GUIContent("Mass", "mass of current object in kilogram(Kg)"), myTarget.Mass);

                        if (myTarget.Rb)
                            myTarget.Rb.mass = myTarget.Mass;

                        myTarget.AverageRadius = EditorGUILayout.FloatField(new GUIContent("Average Radius", "average radius of the planet or object, value used to set default height(in m)"), myTarget.AverageRadius);

                        GUI.enabled = false;
                        {
                            myTarget.EquatorialRadius = EditorGUILayout.FloatField(new GUIContent("Equatorial Radius", "radius of the planet at equator level(in m), differ from average radius if _rotationDeformation is enabled"), myTarget.EquatorialRadius);
                            myTarget.PolarRadius = EditorGUILayout.FloatField(new GUIContent("Polar Radius", "radius of the planet at the pôles(in m), differ from average radius if _rotationDeformation is enabled"), myTarget.PolarRadius);
                        }
                        GUI.enabled = true;

                        if (myTarget.UsedPhysicSystem == PhysicType.CharacterController)
                            myTarget.CharaControllerCustomVelocity = EditorGUILayout.Vector3Field(new GUIContent("Velocity", "velocity used for calculation when on CharacterController mode"), myTarget.CharaControllerCustomVelocity);

                        //CALCULATION TYPE
                        GUILayout.BeginVertical("HelpBox");
                        {
                            GUILayout.Label("calculation type");
                            GUILayout.BeginHorizontal();
                            {
                                GUILayout.BeginHorizontal("HelpBox");
                                {
                                    myTarget.CalculationArray[0] = EditorGUILayout.Toggle(new GUIContent("Sphere(or other)", "use sphere shape calculation for mass, densiy, and scale"), myTarget.CalculationArray[0]);
                                }
                                GUILayout.EndHorizontal();

                                GUILayout.BeginHorizontal("HelpBox");
                                {
                                    myTarget.CalculationArray[1] = EditorGUILayout.Toggle(new GUIContent("Cube", "use cube shape calculation for mass, densiy, and scale"), myTarget.CalculationArray[1]);
                                }
                                GUILayout.EndHorizontal();
                            }
                            GUILayout.EndHorizontal();

                            GUILayout.BeginHorizontal();
                            {
                                GUILayout.BeginHorizontal("HelpBox");
                                {
                                    myTarget.CalculationArray[2] = EditorGUILayout.Toggle(new GUIContent("Cylinder", "use cylinder shape calculation for mass, densiy, and scale"), myTarget.CalculationArray[2]);
                                }
                                GUILayout.EndHorizontal();

                                GUILayout.BeginHorizontal("HelpBox");
                                {
                                    myTarget.CalculationArray[3] = EditorGUILayout.Toggle(new GUIContent("Capsule", "use capsule shape calculation for mass, densiy, and scale"), myTarget.CalculationArray[3]);
                                }
                                GUILayout.EndHorizontal();

                            }
                            GUILayout.EndHorizontal();

                            GUILayout.BeginHorizontal("box");
                            {
                                if (GUILayout.Button("\nSet Mass\n"))
                                    myTarget.SetMass();
                                if (GUILayout.Button("\nSet Matter Density\n"))
                                    myTarget.SetMatterDensity();
                                if (GUILayout.Button("\nSet Scale\n"))
                                    myTarget.SetScale();
                            }
                            GUILayout.EndHorizontal();

                            GUILayout.Space(EditorGUIUtility.singleLineHeight);

                            myTarget.AutoUpdateMass = EditorGUILayout.Toggle(new GUIContent("Auto Update Mass", "if enabled, will automatically update mass when density or scale change"), myTarget.AutoUpdateMass);
                        }
                        GUILayout.EndVertical();

                    }
                    GUILayout.Space(10);
                    GUILayout.EndVertical();

                }
                GUILayout.EndVertical();


                GUILayout.Space(30);


                #region DEBUG______________________________________________________________________________________
                GUILayout.BeginVertical("box");
                GUILayout.Label("DEBUG____________________________________________________________________________________", EditorStyles.boldLabel);
                GUILayout.BeginVertical("helpBox");
                GUILayout.Space(20);
                myTarget.DrawDefaultInspector = EditorGUILayout.Toggle(new GUIContent("Draw Default Inspector", "determine is, instead of custom inspector, program should render default values for editing at source variables"), myTarget.DrawDefaultInspector);
                myTarget.VolumetricAtmospherePreview = EditorGUILayout.Toggle(new GUIContent("Volumetric Atmosphere Preview", "if enabled, will render atmosphere preview in scene with volumes"), myTarget.VolumetricAtmospherePreview);
                GUILayout.EndVertical();
                GUILayout.EndVertical();
                #endregion

            }

            myTarget.switchCalculationMode();
        }
    }
}
