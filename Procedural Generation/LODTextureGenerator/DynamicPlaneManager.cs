using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UPDB.CoreHelper.UsableMethods;

namespace UPDB.ProceduralGeneration.LODTextureGenerator
{
    public class DynamicPlaneManager : UPDBBehaviour
    {
        [SerializeField, Tooltip("camera size for object textures")]
        private float _cameraTextureSize = 1.0f;

        [SerializeField, Tooltip("path of generated images")]
        private string _imagesPath;

        [SerializeField, Tooltip("number of images to rec horizontally, and vertically")]
        private Vector2Int _imagesNumber;

        [SerializeField, Tooltip("tells if program should store each files in group in separate folders")]
        private bool _separateFolders = false;

        [SerializeField, HideInInspector]
        private List<Vector3> _cameraDirectionsSavable;

        [SerializeField]
        private List<Texture2D> _texturesSavable;

        private Vector3[][] _cameraDirectionsList;

        private Texture2D[][] _textureList;
        private Vector3 _camPosMemo = Vector3.zero;
        private MeshFilter _meshFilterRef;
        private MeshRenderer _meshRendererRef;
        private Material _matRef;

        #region Public API

        public float CameraTextureSize
        {
            get => _cameraTextureSize;
            set => _cameraTextureSize = value;
        }
        public string ImagePath
        {
            get => _imagesPath;
            set => _imagesPath = value;
        }
        public Vector2Int ImagesNumber
        {
            get => _imagesNumber;
            set => _imagesNumber = value;
        }
        public bool SeparateFolders
        {
            get => _separateFolders;
            set => _separateFolders = value;
        }

        [System.Serializable]
        public class LODConfig
        {
            [SerializeField, Tooltip("GameObject to enable")]
            private GameObject _LODobject;

            [SerializeField, Tooltip("distance range for element to be active")]
            private Vector2 _effectRange = Vector2.zero;
        }
        public Vector3[][] CameraDirectionsList
        {
            get => _cameraDirectionsList;
            set => _cameraDirectionsList = value;
        }
        public Texture2D[][] TextureList
        {
            get => _textureList;
            set => _textureList = value;
        }
        public List<Vector3> CameraDirectionsSavable
        {
            get => _cameraDirectionsSavable;
            set => _cameraDirectionsSavable = value;
        }
        public List<Texture2D> TexturesSavable
        {
            get => _texturesSavable;
            set => _texturesSavable = value;
        }

        #endregion

        protected override void OnScene()
        {
            if (Camera.current.transform.position != _camPosMemo)
                UpdateActivePlane();

            _camPosMemo = Camera.current.transform.position;
        }

        public Vector3[] GetCircleDirections(int number)
        {
            if (number == 0)
                return new Vector3[0];

            Vector3[] posList = new Vector3[number];
            Vector3 pos = Vector3.forward;

            for (int i = 0; i < posList.Length; i++)
            {
                posList[i] = pos;
                pos = RotateVector(pos, Vector3.up, 360 / (float)number);
            }

            return posList;
        }

        public Vector2Int GetClosestDirectionIndex(Vector3 position, Vector3 center, Vector3[][] directionsList)
        {
            if (directionsList == null || directionsList.Length == 0)
                return Vector2Int.zero;

            Vector3 posDir = (position - center).normalized;

            int closestY = 0;

            for (int i = 0; i < directionsList.Length; i++)
            {
                if (directionsList[i] == null || directionsList[i][0] == null)
                    return Vector2Int.zero;

                if (Mathf.Abs(directionsList[closestY][0].y - posDir.y) > Mathf.Abs(directionsList[i][0].y - posDir.y))
                    closestY = i;
            }

            int closestX = 0;

            for (int i = 0; i < directionsList[closestY].Length; i++)
            {
                if (directionsList[closestY] == null || directionsList[closestY][i] == null)
                    return Vector2Int.zero;

                if (Vector3.Distance(directionsList[closestY][closestX], posDir) > Vector3.Distance(directionsList[closestY][i], posDir))
                    closestX = i;
            }

            return new Vector2Int(closestX, closestY);
        }

        public List<T> ToSavable<T>(T[][] toSave)
        {
            List<T> toReturn = new List<T>();

            if (toSave == null)
                return toReturn;

            for (int i = 0; i < toSave.Length; i++)
                for (int j = 0; j < toSave[i].Length; j++)
                    toReturn.Add(toSave[i][j]);

            return toReturn;
        }

        public T[][] FromSavable<T>(List<T> toLoad, int xLength, int yLength)
        {
            T[][] toReturn = new T[yLength][];

            if (toLoad == null || toLoad.Count < (xLength * yLength))
                return toReturn;

            for (int i = 0; i < toReturn.Length; i++)
            {
                toReturn[i] = new T[xLength];

                for (int j = 0; j < toReturn[i].Length; j++)
                    toReturn[i][j] = toLoad[j + (toReturn[i].Length * i)];
            }

            return toReturn;
        }

        private void UpdateActivePlane()
        {
            _cameraDirectionsList = _cameraDirectionsList != null ? _cameraDirectionsList : FromSavable(_cameraDirectionsSavable, _imagesNumber.x, _imagesNumber.y);
            _textureList = _textureList != null ? _textureList : FromSavable(_texturesSavable, _imagesNumber.x, _imagesNumber.y);

            Vector2Int selectedTextureIndex = GetClosestDirectionIndex(Camera.current.transform.position, transform.position, _cameraDirectionsList);

            if (_textureList[selectedTextureIndex.y] == null || _textureList[selectedTextureIndex.y][selectedTextureIndex.x] == null)
                return;

            Texture2D selectedTexture = _textureList[selectedTextureIndex.y][selectedTextureIndex.x];

            MakeNonNullable(ref _meshFilterRef, gameObject, false);
            MakeNonNullable(ref _meshRendererRef, gameObject, false);

            if (!_matRef)
                _matRef = new Material(Shader.Find("Universal Render Pipeline/Lit"));

            _matRef.mainTexture = selectedTexture;

            if(!_meshFilterRef.mesh)
            {
                Mesh newMesh = new Mesh();

                Vector3[] vertices = new Vector3[]{new Vector3(-0.5f, -0.5f, 0), new Vector3(0.5f, -0.5f, 0), new Vector3(-0.5f, 0.5f, 0), new Vector3(0.5f, 0.5f, 0)};
                int[] triangles = new int[]{ 0, 2, 1, 2, 3, 1 };
                Vector2[] uvs = new Vector2[]{ new Vector2(0, 0), new Vector2(1, 0), new Vector2(0, 1), new Vector2(1, 1) };

                newMesh.vertices = vertices;
                newMesh.triangles = triangles;
                newMesh.uv = uvs;

                // Recalcule les normales pour l'éclairage
                newMesh.RecalculateNormals();

                _meshFilterRef.mesh = newMesh;
            }

            if(!_meshRendererRef.sharedMaterial)
                _meshRendererRef.sharedMaterial = _matRef;

            Vector3 pos = -(Camera.current.transform.position - transform.position) + transform.position;
            transform.LookAt(pos);
        }
    }
}
