using UnityEditor;

/// <summary>
/// check conditional compilation for input system package, if experimenting issues for package detection by visual, go to edit/preferences/external tools/regenerate project files, with registry packages enabled
/// </summary>
[InitializeOnLoad]
public class InputSystemChecker
{
    // Le symbole de compilation conditionnelle que nous allons définir
    private const string InputSystemDefine = "INPUT_SYSTEM_PRESENT";

    // Constructeur statique, appelé automatiquement par Unity à l'initialisation
    [System.Obsolete]
    static InputSystemChecker()
    {
        CheckAndSetInputSystemDefine();
    }

    // Vérifie et définit le symbole de compilation conditionnelle
    [System.Obsolete]
    private static void CheckAndSetInputSystemDefine()
    {
        // Obtient tous les packages installés
        var packages = UnityEditor.PackageManager.PackageInfo.GetAllRegisteredPackages();
        bool inputSystemExists = false;

        // Vérifie si le package "com.unity.inputsystem" est présent
        foreach (var package in packages)
        {
            if (package.name == "com.unity.inputsystem")
            {
                inputSystemExists = true;
                break;
            }
        }

        // Obtient les symboles de compilation actuels pour le groupe de build sélectionné
        var defines = PlayerSettings.GetScriptingDefineSymbolsForGroup(EditorUserBuildSettings.selectedBuildTargetGroup);

        // Ajoute ou retire le symbole de compilation conditionnelle en fonction de la présence du package
        if (inputSystemExists && !defines.Contains(InputSystemDefine))
        {
            defines += ";" + InputSystemDefine;
            PlayerSettings.SetScriptingDefineSymbolsForGroup(EditorUserBuildSettings.selectedBuildTargetGroup, defines);
        }
        else if (!inputSystemExists && defines.Contains(InputSystemDefine))
        {
            defines = defines.Replace(InputSystemDefine, "").Replace(";;", ";");
            PlayerSettings.SetScriptingDefineSymbolsForGroup(EditorUserBuildSettings.selectedBuildTargetGroup, defines);
        }
    }
}
