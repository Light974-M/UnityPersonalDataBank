using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UPDB.CoreHelper.UsableMethods;

namespace UPDB.CoreHelper.Usable.CustomFieldsAndStructs
{
    public class MaterialAutoTilling : UPDBBehaviour
    {
        [SerializeField, Tooltip("scale to maintain")]
        private Vector2 _scale = Vector2.one;

        [SerializeField]
        private Axis _xTillingLinkedScale = Axis.X;

        [SerializeField]
        private Axis _yTillingLinkedScale = Axis.Z;

        private MeshRenderer _renderer = null;

        private MeshRenderer Renderer
        {
            get
            {
                return MakeNonNullable(ref _renderer, gameObject);
            }
        }

        private Material _objectMaterial;

        #region Public API

        public Material ObjectMaterial
        {
            get
            {
                if (_objectMaterial)
                    return _objectMaterial;

                if (Renderer.sharedMaterial)
                {
                    Material newMat = new Material(Renderer.sharedMaterial);
                    Renderer.sharedMaterial = newMat;
                    return _objectMaterial = newMat;
                }

                Debug.LogWarning("warning : no materials were found in meshRenderer, please specify one");
                return null;
            }

            set
            {
                _objectMaterial = value;
            }
        } 
        public Vector2 Scale
        {
            get => _scale;
            set => _scale = value;
        }
        public Axis XTillingLinkedScale
        {
            get => _xTillingLinkedScale;
            set => _xTillingLinkedScale = value;
        }
        public Axis YTillingLinkedScale
        {
            get => _yTillingLinkedScale;
            set => _yTillingLinkedScale = value;
        }

        public enum Axis
        {
            X,
            Y,
            Z,
        }

        #endregion

        protected override void OnScene()
        {
            List<Transform> parentsList = new List<Transform>();
            parentsList.Add(transform);
            int autoBreak = 0;

            while (parentsList[parentsList.Count - 1].parent != null && autoBreak < 1000)
            {
                parentsList.Add(parentsList[parentsList.Count - 1].parent);
                autoBreak++;
            }

            Vector3 scale = Vector3.one;

            foreach (Transform parent in parentsList)
            {
                scale = UPDBMath.VecTime(scale, parent.localScale);
            }

            ObjectMaterial.mainTextureScale = new Vector2(scale.x * _scale.x, scale.z * _scale.y);
        }
    }
}
