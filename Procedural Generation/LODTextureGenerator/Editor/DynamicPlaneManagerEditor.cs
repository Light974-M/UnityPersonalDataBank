using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;
using UPDB.CoreHelper.UsableMethods;

namespace UPDB.ProceduralGeneration.LODTextureGenerator
{
	[CustomEditor(typeof(DynamicPlaneManager))]
	public class DynamicPlaneManagerEditor : Editor
	{
        public override void OnInspectorGUI()
        {
            DynamicPlaneManager myTarget = (DynamicPlaneManager)target;

            if (GUILayout.Button("GENERATE IMAGES"))
                GenerateSnapshot(myTarget);

            GUIContent cameraTextureSizeContent = new GUIContent(nameof(myTarget.CameraTextureSize), "camera size for object textures");
            myTarget.CameraTextureSize = EditorGUILayout.FloatField(cameraTextureSizeContent, myTarget.CameraTextureSize);

            GUIContent imagePathContent = new GUIContent(nameof(myTarget.ImagePath), "path of generated images");
            myTarget.ImagePath = EditorGUILayout.TextField(imagePathContent, myTarget.ImagePath);

            GUIContent imagesNumberContent = new GUIContent(nameof(myTarget.ImagesNumber), "number of images to rec horizontally, and vertically");
            myTarget.ImagesNumber = EditorGUILayout.Vector2IntField(imagesNumberContent, myTarget.ImagesNumber);

            GUIContent separateFoldersContent = new GUIContent(nameof(myTarget.SeparateFolders), "tells if program should store each files in group in separate folders");
            myTarget.SeparateFolders = EditorGUILayout.Toggle(separateFoldersContent, myTarget.SeparateFolders);

            base.OnInspectorGUI();
        }

        public void GenerateSnapshot(DynamicPlaneManager target)
        {
            GameObject selectedObject = target.gameObject;

            // create a temporary camera
            GameObject cameraObject = new GameObject("SnapshotCamera");
            Camera camera = cameraObject.AddComponent<Camera>();
            camera.backgroundColor = Color.clear; // clear background
            camera.orthographic = true;
            camera.orthographicSize = target.CameraTextureSize; // Ajustez selon la taille de l'objet
            camera.clearFlags = CameraClearFlags.SolidColor;

            target.CameraDirectionsList = UPDBBehaviour.GetSphereVerticeDirections(target.ImagesNumber.x, target.ImagesNumber.y);
            target.TextureList = new Texture2D[target.CameraDirectionsList.Length][];
            target.CameraDirectionsSavable = target.ToSavable(target.CameraDirectionsList);

            Vector3[] cameraTopAndBottomUpDirection = target.GetCircleDirections(target.ImagesNumber.x);

            string basePath = Application.dataPath + "/" + target.ImagePath;

            if (!Directory.Exists(basePath))
            {
                // Crée le dossier (et les sous-dossiers si nécessaire)
                Directory.CreateDirectory(basePath);
                Debug.Log($"Le dossier a été créé : {basePath}");
            }

            for (int i = 0; i < target.CameraDirectionsList.Length; i++)
            {
                target.TextureList[i] = new Texture2D[target.CameraDirectionsList[i].Length];

                string horizontalPath = target.SeparateFolders ? basePath + "/turn#" + i : basePath;
                string localHorizontalPath = target.SeparateFolders ? target.ImagePath + "/turn#" + i : target.ImagePath;

                if (target.SeparateFolders && !Directory.Exists(horizontalPath))
                {
                    // Crée le dossier (et les sous-dossiers si nécessaire)
                    Directory.CreateDirectory(horizontalPath);
                    Debug.Log($"Le dossier a été créé : {horizontalPath}");
                }

                for (int j = 0; j < target.CameraDirectionsList[i].Length; j++)
                {
                    Bounds bounds = CalculateBounds(selectedObject);
                    camera.transform.position = bounds.center + target.CameraDirectionsList[i][j] * target.CameraTextureSize;
                    Debug.Log(bounds.center + target.CameraDirectionsList[i][j]);
                    if (i == 0 || i == target.CameraDirectionsList.Length - 1)
                        camera.transform.LookAt(bounds.center, cameraTopAndBottomUpDirection[j]);
                    else
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

                    // Create asset path for image
                    string path = target.SeparateFolders ? horizontalPath + $"/MeshTexture_{j}.png" : horizontalPath + $"/MeshTexture_{j + (10 * i)}.png";
                    string localPath = target.SeparateFolders ? localHorizontalPath + $"/MeshTexture_{j}.png" : localHorizontalPath + $"/MeshTexture_{j + (10 * i)}.png";

                    //verify if image is already there, and delete it if so
                    if (AssetDatabase.LoadAssetAtPath<Object>(localPath) != null)
                        AssetDatabase.DeleteAsset(localPath);

                    //save image
                    byte[] bytes = snapshot.EncodeToPNG();
                    File.WriteAllBytes(path, bytes);
                    target.TextureList[i][j] = AssetDatabase.LoadAssetAtPath<Texture2D>("Assets/" + localPath);

                    camera.targetTexture = null;
                    RenderTexture.active = null;
                    UPDBBehaviour.IntelliDestroy(renderTexture);
                }
            }

            Debug.Log($"Images saved in : {basePath}");

            target.TexturesSavable = target.ToSavable(target.TextureList);

            // Nettoyage
            RenderTexture.active = null;
            camera.targetTexture = null;
            UPDBBehaviour.IntelliDestroy(cameraObject);
        }

        private Bounds CalculateBounds(GameObject obj)
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
