using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UPDB.CoreHelper.UsableMethods;

namespace UPDB.CoreHelper.Usable.CustomFieldsAndStructs
{
    public class MaterialAutoTilling : UPDBBehaviour
    {
        [SerializeField, Tooltip("material used to create material instance of tilling")]
        private Material _referenceMaterial;

        [SerializeField, Tooltip("scale to maintain")]
        private Vector2 _scale = Vector2.one;

        [SerializeField]
        private Axis _xTillingLinkedScale = Axis.X;

        [SerializeField]
        private Axis _yTillingLinkedScale = Axis.Z;

        private MeshRenderer _renderer = null;

        private Material _objectMaterial;

        #region Public API

        public MeshRenderer Renderer
        {
            get
            {
                return MakeNonNullable(ref _renderer, gameObject);
            }
        }
        public Material ObjectMaterial
        {
            get
            {
                if (_objectMaterial)
                {
                    if (Renderer.sharedMaterial == _objectMaterial)
                    {
                        return _objectMaterial;
                    }
                    else
                    {
                        _referenceMaterial = Renderer.sharedMaterial;
                        Material newMat = new Material(Renderer.sharedMaterial);
                        Renderer.sharedMaterial = newMat;
                        _objectMaterial = newMat;

                        return _objectMaterial;
                    }
                }

                if(_referenceMaterial)
                {
                    Material newMat = new Material(_referenceMaterial);
                    Renderer.sharedMaterial = newMat;
                    return _objectMaterial = newMat;
                }

                if (Renderer.sharedMaterial)
                {
                    _referenceMaterial = Renderer.sharedMaterial;
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
        public Material ReferenceMaterial
        {
            get => _referenceMaterial;
            set => _referenceMaterial = value;
        }

        #endregion

        protected override void OnScene()
        {
            if (ObjectMaterial == null)
                return;

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
                scale = UPDBMath.VecTime(scale, parent.localScale);

            Vector2 scaleToApply = Vector2.one;

            if (XTillingLinkedScale == Axis.X)
                scaleToApply.x = scale.x * _scale.x;

            if (XTillingLinkedScale == Axis.Y)
                scaleToApply.x = scale.y * _scale.x;

            if (XTillingLinkedScale == Axis.Z)
                scaleToApply.x = scale.z * _scale.x;

            if (YTillingLinkedScale == Axis.X)
                scaleToApply.y = scale.x * _scale.y;

            if (YTillingLinkedScale == Axis.Y)
                scaleToApply.y = scale.y * _scale.y;

            if (YTillingLinkedScale == Axis.Z)
                scaleToApply.y = scale.z * _scale.y;


            ObjectMaterial.mainTextureScale = scaleToApply;
        }
    }

    public enum Axis
    {
        X,
        Y,
        Z,
    }
}
