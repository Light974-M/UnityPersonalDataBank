using UnityEngine;
using UnityEditor;
using System.IO;
using UPDB.CoreHelper.UsableMethods;

namespace UPDB.Data.MeshSnapshot
{
    public static class MeshSnapshotGenerator
    {
        [MenuItem("UPDB/Data/Mesh Snapshot/Generate Snapshot")]
        public static void GenerateSnapshot()
        {
            // Sélectionnez l'objet dans la hiérarchie
            GameObject selectedObject = Selection.activeGameObject;
            if (selectedObject == null)
            {
                Debug.LogError("Aucun objet sélectionné !");
                return;
            }

            // Créer une caméra temporaire
            GameObject cameraObject = new GameObject("SnapshotCamera");
            Camera camera = cameraObject.AddComponent<Camera>();
            camera.backgroundColor = Color.clear; // Fond transparent
            camera.orthographic = true; // Vue orthographique pour des images plates
            camera.orthographicSize = 1; // Ajustez selon la taille de l'objet
            camera.clearFlags = CameraClearFlags.SolidColor;

            // Positionner la caméra devant l'objet
            Bounds bounds = CalculateBounds(selectedObject);
            camera.transform.position = bounds.center + Vector3.forward * bounds.size.magnitude;
            camera.transform.LookAt(bounds.center);

            // Créer une RenderTexture
            int textureSize = 512; // Taille de la texture
            RenderTexture renderTexture = new RenderTexture(textureSize, textureSize, 24);
            camera.targetTexture = renderTexture;

            // Capturer l'image
            Texture2D snapshot = new Texture2D(textureSize, textureSize, TextureFormat.ARGB32, false);
            camera.Render();
            RenderTexture.active = renderTexture;
            snapshot.ReadPixels(new Rect(0, 0, textureSize, textureSize), 0, 0);
            snapshot.Apply();

            // Sauvegarder l'image
            byte[] bytes = snapshot.EncodeToPNG();
            string path = Application.dataPath + "/MeshSnapshot.png";
            File.WriteAllBytes(path, bytes);
            Debug.Log($"Image sauvegardée dans : {path}");

            // Nettoyage
            RenderTexture.active = null;
            camera.targetTexture = null;
            UPDBBehaviour.IntelliDestroy(renderTexture);
            UPDBBehaviour.IntelliDestroy(cameraObject);
        }

        private static Bounds CalculateBounds(GameObject obj)
        {
            Renderer[] renderers = obj.GetComponentsInChildren<Renderer>();
            if (renderers.Length == 0)
                return new Bounds(obj.transform.position, Vector3.zero);

            Bounds bounds = renderers[0].bounds;
            foreach (Renderer renderer in renderers)
            {
                bounds.Encapsulate(renderer.bounds);
            }
            return bounds;
        }
    }

}
