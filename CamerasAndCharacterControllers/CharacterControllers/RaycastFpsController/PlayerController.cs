using UnityEngine;
using UPDB.CoreHelper;
using UPDB.CoreHelper.UsableMethods;

namespace UPDB.CamerasAndCharacterControllers.CharacterControllers.RaycastFpsController
{
    [AddComponentMenu(NamespaceID.CharacterControllersPath + "/" + NamespaceID.RaycastFpsController + "/PlayerController")]
    public class PlayerController : UPDBBehaviour
    {
        [SerializeField]
        private float _moveSpeed = 3f;

        [SerializeField]
        private float _rotateSpeed = 3f;

        private Rigidbody _rb;

        private void Awake()
        {
            MakeNonNullable(ref _rb, gameObject);
        }

        private void FixedUpdate()
        {
            if (Input.GetKey(KeyCode.UpArrow))
            {
                _rb.position += transform.forward * _moveSpeed * Time.fixedDeltaTime;
            }

            if (Input.GetKey(KeyCode.DownArrow))
            {
                _rb.position -= transform.forward * _moveSpeed * Time.fixedDeltaTime;
            }

            if (Input.GetKey(KeyCode.LeftArrow))
            {
                transform.Rotate(-Vector3.up * _rotateSpeed);
            }

            if (Input.GetKey(KeyCode.RightArrow))
            {
                transform.Rotate(Vector3.up * _rotateSpeed);
            }
        }
    }
}
