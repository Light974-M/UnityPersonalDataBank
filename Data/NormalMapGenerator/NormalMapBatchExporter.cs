using UnityEngine;
using UnityEditor;
using System.IO;

public class NormalMapBatchExporter : EditorWindow
{
    Texture2D[] sourceTextures;
    Shader normalGenShader;
    Material material;
    string extensionName = "";
    TypeToGenerate generateType;

    public enum TypeToGenerate
    {
        Normal,
        Smoothness,
    }

    [MenuItem("Tools/Batch Export Normal Maps")]
    static void Init()
    {
        GetWindow<NormalMapBatchExporter>("Normal Map Exporter");
    }

    void OnGUI()
    {
        generateType = (TypeToGenerate)EditorGUILayout.EnumPopup("Generate Type", generateType);

        if (GUILayout.Button("Load Shader"))
        {
            if (generateType == TypeToGenerate.Normal)
                normalGenShader = Shader.Find("Hidden/NormalMapGenerator");
            if (generateType == TypeToGenerate.Smoothness)
                normalGenShader = Shader.Find("Hidden/SmoothnessMapGenerator");

            material = new Material(normalGenShader);
        }

        if (GUILayout.Button("Select Textures"))
        {
            string[] guids = Selection.assetGUIDs;
            sourceTextures = new Texture2D[guids.Length];
            for (int i = 0; i < guids.Length; i++)
                sourceTextures[i] = AssetDatabase.LoadAssetAtPath<Texture2D>(AssetDatabase.GUIDToAssetPath(guids[i]));
        }

        if (GUILayout.Button("Generate and Save Normal Maps"))
        {
            foreach (var tex in sourceTextures)
            {
                string path = AssetDatabase.GetAssetPath(tex);
                string outPath = Path.GetDirectoryName(path) + "/" + Path.GetFileNameWithoutExtension(path) + extensionName + ".png";

                GenerateNormalMap(tex, outPath);
            }

            AssetDatabase.Refresh();
        }

        extensionName = EditorGUILayout.TextField("Extension Name", extensionName);
    }

    void GenerateNormalMap(Texture2D source, string outPath)
    {
        int w = source.width;
        int h = source.height;

        RenderTexture rt = RenderTexture.GetTemporary(w, h, 0, RenderTextureFormat.ARGB32);
        material.SetTexture("_MainTex", source);
        material.SetFloat("_Strength", 2f); // intensité du relief

        // Utilisation correcte de 'source' au lieu de 'tex'
        material.SetVector("_Resolution", new Vector4(source.width, source.height, 0, 0));

        // Blit la texture
        Graphics.Blit(null, rt, material);

        RenderTexture.active = rt;
        Texture2D result = new Texture2D(w, h, TextureFormat.RGBA32, false);
        result.ReadPixels(new Rect(0, 0, w, h), 0, 0);
        result.Apply();
        RenderTexture.active = null;

        // Sauvegarde de la normal map en PNG
        File.WriteAllBytes(outPath, result.EncodeToPNG());
        RenderTexture.ReleaseTemporary(rt);
    }
}