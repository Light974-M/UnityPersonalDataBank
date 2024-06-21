using UnityEngine;
using UPDB.CoreHelper.UsableMethods;

namespace UPDB.CustomInput.RotationDetector
{
    ///<summary>
    /// 
    ///</summary>
    [AddComponentMenu("UPDB/Input/RotationDetector/RotationInputDetector")]
    public class RotationInputDetector : UPDBBehaviour
    {
        [SerializeField]
        private float _rotationDir = 0;

        private int i = 0;
        private Vector2 _previousVector = Vector2.zero;
        private Vector3 _cross;

        private void OnDrawGizmos()
        {
            Vector2 direction = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
            direction.Normalize();
            Debug.DrawLine(transform.position, transform.position + (Vector3)direction, Color.white);

            if (i > 5) { _previousVector = direction; i = 0; }

            _cross = Vector3.Cross(direction, _previousVector);
            i++;

            _rotationDir = _cross.z > 0 ? 1 : 0;
            _rotationDir = _cross.z < 0 ? -1 : _rotationDir;
        }
    }
}

