using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UPDB.CoreHelper.CustomPropertyAttributes;
using UPDB.CoreHelper.UsableMethods;

namespace UPDB.ProceduralGeneration.ProShapeBuilder
{
    public class LODFadeManager : UPDBBehaviour
    {
        [SerializeField, Tooltip("choose what value is attributed to a distance of LOD system")]
        private List<LODKey> _lODFadeShape;

        [SerializeField, Tooltip("camera used to render LOD")]
        private Camera _usedCamera;

        private int _currentCameraRangeIndex = 0;

        #region Public API

        [Serializable]
        public class LODKey
        {
            [SerializeField, Tooltip("distance affected, range is between this value and next value")]
            private float _distanceMin;

            [ReadOnly]
            [SerializeField, Tooltip("display next element value as max bound")]
            private float _distanceMax;

            [SerializeField, Tooltip("value that will be affeted within the range")]
            private int _value;

            #region Public API

            public float DistanceMin
            {
                get => _distanceMin;
                set => _distanceMin = value;
            }

            public float DistanceMax
            {
                get => _distanceMax;
                set => _distanceMax = value;
            }

            public int Value
            {
                get => _value;
                set => _value = value;
            }

            #endregion
        }

        public delegate void LODRebuildCallback(int value);
        public event LODRebuildCallback OnLODRebuild;

        private Camera CameraCurrent
        {
            get
            {
                if (Application.isPlaying)
                    return _usedCamera;

                return Camera.current;
            }
        }

        #endregion

        protected override void OnScene()
        {
            base.OnScene();

            if (!_usedCamera)
            {
                if (Camera.main != null)
                    _usedCamera = Camera.main;
            }

            if (_lODFadeShape == null || _lODFadeShape.Count == 0)
                return;

            FormatArray();
            LODUpdate();
        }

        private void Update()
        {
            LODUpdate();
        }

        private void FormatArray()
        {
            _lODFadeShape[0].DistanceMin = 0;

            for (int i = 0; i < _lODFadeShape.Count - 1; i++)
            {
                if (_lODFadeShape[i].DistanceMin > _lODFadeShape[i + 1].DistanceMin)
                {
                    LODKey valuetoReplace = _lODFadeShape[i];
                    _lODFadeShape[i] = _lODFadeShape[i + 1];
                    _lODFadeShape[i + 1] = valuetoReplace;
                }
            }

            for (int i = 0; i < _lODFadeShape.Count - 1; i++)
                _lODFadeShape[i].DistanceMax = _lODFadeShape[i + 1].DistanceMin;

            _lODFadeShape[_lODFadeShape.Count - 1].DistanceMax = Mathf.Infinity;
        }

        private void LODUpdate()
        {
            float dist = Vector3.Distance(CameraCurrent.transform.position, transform.position);

            if (!IsInRange(dist, _lODFadeShape[_currentCameraRangeIndex].DistanceMin, _lODFadeShape[_currentCameraRangeIndex].DistanceMax))
            {
                int newIndex = SearchForCameraDistanceIndex(dist);

                if (newIndex < 0)
                {
                    Debug.LogError("ERROR : no distance found for camera LOD system of" + gameObject.name);
                    return;
                }

                _currentCameraRangeIndex = newIndex;

                OnLODRebuild?.Invoke(_lODFadeShape[_currentCameraRangeIndex].Value);
            }
        }

        private int SearchForCameraDistanceIndex(float dist)
        {
            for (int i = 0; i < _lODFadeShape.Count; i++)
                if(IsInRange(dist, _lODFadeShape[i].DistanceMin, _lODFadeShape[i].DistanceMax))
                    return i;

            return -1;
        }
    } 

}
