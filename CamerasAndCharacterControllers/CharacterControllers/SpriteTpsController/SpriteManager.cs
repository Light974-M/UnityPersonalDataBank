using UnityEngine;
using UPDB.CoreHelper.UsableMethods;

namespace UPDB.CamerasAndCharacterControllers.CharacterControllers.SpriteTpsController
{
    ///<summary>
    /// 
    ///</summary>
    [AddComponentMenu("UPDB/CamerasAndCharacterControllers/CharacterControllers/SpriteTpsController/SpriteManager")]
    public class SpriteManager : UPDBBehaviour
    {
        [SerializeField, Tooltip("")]
        private SpriteRenderer _spriteRenderer;

        [SerializeField, Tooltip("")]
        private Transform _targetToLook;

        [SerializeField, Tooltip("")]
        private Transform _camera;

        [SerializeField, Tooltip("")]
        private Transform _player;

        [SerializeField, Tooltip("")]
        private bool _onlyYAxis = false;

        [SerializeField, Tooltip("")]
        private SpriteListAsset[] _spriteListAssetList;


        private void Awake()
        {
            InitVariables();
        }

        private void Update()
        {
            if (_spriteListAssetList.Length != 0)
                _spriteRenderer.sprite = _spriteListAssetList[0].SpriteList[CalculateSpriteToDraw()];

            SetRealRotation();
        }

        private void OnDrawGizmos()
        {
            if (!Application.isPlaying)
            {
                InitVariables(); 
            }
        }

        private void InitVariables()
        {
            if (_spriteRenderer == null)
                if (!TryGetComponent(out _spriteRenderer))
                    _spriteRenderer = gameObject.AddComponent<SpriteRenderer>();

            if (_camera == null)
                _camera = transform;

            if(_player == null)
            {
                _player = transform.parent;

                if (transform.parent == null)
                    _player = transform;
            }

        }

        private int CalculateSpriteToDraw()
        {
            int index = 0;
            float spritesDifference = 360 / _spriteListAssetList[0].SpriteList.Length;
            float currentRotation = _targetToLook.eulerAngles.y;
            float cameraCurrentRotation = _camera.eulerAngles.y;
            float camRotComparePlayerRot = cameraCurrentRotation - currentRotation;

            if (camRotComparePlayerRot < 0)
                camRotComparePlayerRot = (360 + cameraCurrentRotation) - currentRotation;

            for (int i = 0; i < _spriteListAssetList[0].SpriteList.Length; i++)
            {
                if(i == 0)
                {

                }
                else if(camRotComparePlayerRot > (spritesDifference * i) - spritesDifference / 2 && camRotComparePlayerRot <= (spritesDifference * i) + spritesDifference / 2)
                {
                    index = i;
                }
            }

            return index;
        }

        private void SetRealRotation()
        {
            transform.LookAt(_camera.position);

            if (_onlyYAxis)
                transform.eulerAngles = new Vector3(0, transform.eulerAngles.y, 0);
        }
    }
}

