using UnityEngine;

namespace UPDB
{
    ///<summary>
    /// 
    ///</summary>
    [AddComponentMenu("UPDB/SpriteManager")]
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
        private Sprite[] _spriteList;

        private void Awake()
        {
            InitVariables();
        }

        private void Update()
        {
            if (_spriteList.Length != 0)
                _spriteRenderer.sprite = _spriteList[CalculateSpriteToDraw()];

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
            float spritesDifference = 360 / _spriteList.Length;
            float currentRotation = _targetToLook.eulerAngles.y;
            float cameraCurrentRotation = _camera.eulerAngles.y;
            float camRotComparePlayerRot = cameraCurrentRotation - currentRotation;

            if (camRotComparePlayerRot < 0)
                camRotComparePlayerRot = (360 + cameraCurrentRotation) - currentRotation;

            for (int i = 0; i < _spriteList.Length; i++)
            {
                if(i == 0)
                {

                }
                else if(camRotComparePlayerRot > (spritesDifference * i) - spritesDifference / 2 && camRotComparePlayerRot <= (spritesDifference * i) + spritesDifference / 2)
                {
                    index = i;
                }
            }
            Debug.Log($"cam : {camRotComparePlayerRot}");

            return index;
        }

        private void SetRealRotation()
        {
            transform.LookAt(_camera.position);
        }
    }
}

