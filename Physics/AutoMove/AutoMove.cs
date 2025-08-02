using UnityEngine;
using UPDB.CoreHelper.UsableMethods;

namespace UPDB.Physic.AutoMove
{
    public class AutoMove : UPDBBehaviour
    {
        [SerializeField]
        private bool _isLocal = true;

        [SerializeField]
        private Vector3 _posA;

        [SerializeField]
        private Vector3 _posB;

        [SerializeField]
        private float _speed;

        private float _timer = 0;

        private void Update()
        {
            Vector3 pos = Vector3.Lerp(_posA, _posB, (Mathf.Sin(_timer) + 1) / 2f);

            if (_isLocal)
                transform.localPosition = pos;
            else
                transform.position = pos;

            _timer += (Time.deltaTime / Mathf.PI) * _speed;
        }
    }
}
