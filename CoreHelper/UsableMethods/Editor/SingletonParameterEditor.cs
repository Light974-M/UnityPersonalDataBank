using Codice.CM.SEIDInfo;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UPDB.CoreHelper.UsableMethods;

namespace UPDB.CoreHelper.UsableMethods
{
    public class SingletonParameterEditor : EditorWindow
    {
        [MenuItem(NamespaceID.UPDB + "/" + NamespaceID.CoreHelper + "/" + NamespaceID.UsableMethods + "/SingletonParameterEditor")]
        public static void ShowMyEditor()
        {
            // This method is called when the user selects the menu item in the Editor.
            SingletonParameterEditor wnd = GetWindow<SingletonParameterEditor>(false, "Singleton Parameter Editor", true);
            wnd.titleContent = new GUIContent("My Custom Editor");

            // Limit size of the window.
            wnd.minSize = new Vector2(450, 200);
            wnd.maxSize = new Vector2(1920, 720);
        }

        private void OnEnable()
        {

        }

        private void OnGUI()
        {
            UPDBBehaviour[] singletonList = UPDBBehaviour.FindObjectsOfTypeGeneric<UPDBBehaviour>(typeof(Singleton<>));

            for (int i = 0; i < singletonList.Length; i++)
            {
                dynamic singletonDynamicInstance = singletonList[i];

                System.Type singletonType = singletonList[i].GetType();

                EditorGUILayout.LabelField(new GUIContent(singletonType.Name));
                EditorGUILayout.BeginVertical("helpBox");
                {
                    singletonDynamicInstance.HideInInspector = EditorGUILayout.Toggle(new GUIContent(nameof(singletonDynamicInstance.HideInInspector)), singletonDynamicInstance.HideInInspector);
                }
                EditorGUILayout.EndVertical();
            }
        }
    } 
}
