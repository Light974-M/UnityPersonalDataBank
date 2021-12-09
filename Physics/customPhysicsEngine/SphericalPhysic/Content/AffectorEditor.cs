using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(Affector)), CanEditMultipleObjects]
public class AffectorEditor : Editor
{
    public override void OnInspectorGUI()
    {
        Affector myTarget = (Affector)target;

        if(myTarget.DrawDefaultInspector)
            base.OnInspectorGUI();
        else
        {

        #region PHYSIC______________________________________________________________________________________
        GUILayout.BeginVertical("box");
        GUILayout.Label("PHYSIC____________________________________________________________________________________", EditorStyles.boldLabel);
        GUILayout.BeginVertical("helpBox");
        GUILayout.Space(20);
        myTarget.PhysicEnabled = EditorGUILayout.Toggle(new GUIContent("Physic Enabled", "determine if object is in the list and is detected"), myTarget.PhysicEnabled);
        myTarget.PhysicAffectorEnabled = EditorGUILayout.Toggle(new GUIContent("Physic Affector Enabled", "determine if object generate gravity and apply it to other object"), myTarget.PhysicAffectorEnabled);
        GUILayout.EndVertical();
        GUILayout.EndVertical(); 
        #endregion

        GUILayout.Space(30);

        #region AIR RESISTANCE________________________________________________________________________________________________________
        GUILayout.BeginVertical("box");
        GUILayout.Label("AIR RESISTANCE_________________________________________________________________________________________________________", EditorStyles.boldLabel);
        GUILayout.BeginVertical("helpBox");
        GUILayout.Space(20);
        myTarget.AtmosphereEnabled = EditorGUILayout.Toggle(new GUIContent("Atmosphere Enabled", "determine if object as atmosphere and generate air resistance"), myTarget.AtmosphereEnabled);
        myTarget.AtmosphereRange = EditorGUILayout.FloatField(new GUIContent("Atmosphere Range", "range of object atmosphere"), myTarget.AtmosphereRange);
        myTarget.AirDensity = EditorGUILayout.FloatField(new GUIContent("Air Density", "density of air and strength of resistance"), myTarget.AirDensity);
        myTarget.AirResistanceEnabled = EditorGUILayout.Toggle(new GUIContent("Air Resistance Enabled", "determine if the object can detect and calculate its air resistance"), myTarget.AirResistanceEnabled);
        GUILayout.EndVertical();
        GUILayout.EndVertical(); 
        #endregion

        GUILayout.Space(30);

        #region GRAVITY___________________________________________________________________________________________
        GUILayout.BeginVertical("box");
        GUILayout.Label("GRAVITY_____________________________________________________________________________________________________________________________________________", EditorStyles.boldLabel);

        #region Content___________________________________
        GUILayout.BeginVertical("helpBox");
        GUILayout.Space(20);
            GUILayout.BeginVertical("helpBox");
            GUILayout.Label("Clamp");
            GUILayout.BeginVertical("helpBox");
            myTarget.ClampGeneratedForce = EditorGUILayout.Toggle(new GUIContent("Clamp Generated Force", "determine if object should have maximum force strength to apply to other objects"), myTarget.ClampGeneratedForce);
            myTarget.MaxGeneratedForce = EditorGUILayout.FloatField(new GUIContent("Max Force", "maximum force strength that object can apply to other objects"), myTarget.MaxGeneratedForce);
            GUILayout.EndVertical();
            GUILayout.BeginVertical("helpBox");
            myTarget.ClampAffectedForces = EditorGUILayout.Toggle(new GUIContent("Clamp Affected Forces", "determine if object should clamp every forces apply to it"), myTarget.ClampAffectedForces);
            myTarget.MaxAffectedForces = EditorGUILayout.FloatField(new GUIContent("Max Forces", "maximum forces strength that object can receive from other objects"), myTarget.MaxAffectedForces);
            GUILayout.EndVertical();
            GUILayout.EndVertical();
        GUILayout.Space(20);
        myTarget.MatterDensity = EditorGUILayout.FloatField(new GUIContent("Matter Density", "density of mass object(in kg/m3, default value is 1kg for a 1 diameter sphere)"), myTarget.MatterDensity);
        myTarget.GetComponent<Rigidbody>().mass = EditorGUILayout.FloatField(new GUIContent("Mass", "mass of current object in kilogram(Kg)"), myTarget.GetComponent<Rigidbody>().mass);

        #region SetMassButtons_____________________________
        GUILayout.BeginVertical("HelpBox");

        #region ChooseCalculationType
        GUILayout.Label("calculation type");
        GUILayout.BeginHorizontal();
        
        #region DefaultCalculation
        GUILayout.BeginHorizontal("HelpBox");
        myTarget.CalculationArray[0] = EditorGUILayout.Toggle(new GUIContent("Sphere(or other)", "use sphere shape calculation for mass, densiy, and scale"), myTarget.CalculationArray[0]);
        GUILayout.EndHorizontal();
        #endregion

        #region CubeCalculation
        GUILayout.BeginHorizontal("HelpBox");
        myTarget.CalculationArray[1] = EditorGUILayout.Toggle(new GUIContent("Cube", "use cube shape calculation for mass, densiy, and scale"), myTarget.CalculationArray[1]);
        GUILayout.EndHorizontal();
        #endregion

        GUILayout.EndHorizontal();
        GUILayout.BeginHorizontal();

        #region CylinderCalculation
        GUILayout.BeginHorizontal("HelpBox");
        myTarget.CalculationArray[2] = EditorGUILayout.Toggle(new GUIContent("Cylinder", "use cylinder shape calculation for mass, densiy, and scale"), myTarget.CalculationArray[2]);
        GUILayout.EndHorizontal();
        #endregion

        #region CapsuleCalculation
        GUILayout.BeginHorizontal("HelpBox");
        myTarget.CalculationArray[3] = EditorGUILayout.Toggle(new GUIContent("Capsule", "use capsule shape calculation for mass, densiy, and scale"), myTarget.CalculationArray[3]);
        GUILayout.EndHorizontal();
        #endregion

        GUILayout.EndHorizontal();
        #endregion

        GUILayout.BeginHorizontal("box");
        if (GUILayout.Button("\nSet Mass\n"))
            myTarget.SetMass();
        if (GUILayout.Button("\nSet Matter Density\n"))
            myTarget.SetMatterDensity();
        if (GUILayout.Button("\nSet Scale\n"))
            myTarget.SetScale();
        GUILayout.EndHorizontal();
        GUILayout.EndVertical();
        #endregion

        GUILayout.Space(10);
        GUILayout.EndVertical(); 
        #endregion

        GUILayout.EndVertical();
        #endregion

        GUILayout.Space(30);

        #region CUSTOM INSPECTOR______________________________________________________________________________________
        GUILayout.BeginVertical("box");
        GUILayout.Label("CUSTOM INSPECTOR____________________________________________________________________________________", EditorStyles.boldLabel);
        GUILayout.BeginVertical("helpBox");
        GUILayout.Space(20);
        myTarget.DrawDefaultInspector = EditorGUILayout.Toggle(new GUIContent("Draw Default Inspector", "determine is, instead of custom inspector, program should render default values for editing at source variables"), myTarget.DrawDefaultInspector);
        GUILayout.EndVertical();
        GUILayout.EndVertical();
        #endregion

        }

        myTarget.switchCalculationMode();
    }
}
