using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UPDB.CoreHelper;
using UPDB.CoreHelper.Templates;
using UPDB.CoreHelper.Usable;

namespace UPDB.CamerasAndCharacterControllers.Cameras.SimpleFreeCamera
{
    /// <summary>
    /// simple fps camera controller, can be used with fps controller or alone
    /// </summary>
    [HelpURL(URL.baseURL + "/tree/main/CamerasAndCharacterControllers/Cameras/SimpleFreeCamera/README.md")]
    [AddComponentMenu(NamespaceID.CamerasPath + "/" + NamespaceID.SimpleFreeCamera + "/Free Camera Controller")]
    public class SimpleFreeCamera : MonoBehaviour
    {
        [SerializeField, Tooltip("speed of cam moves")]
        private float _moveSpeed = 1;

        [SerializeField, Tooltip("speed of mouse look in X and Y")]
        private Vector2 _lookSpeed = Vector2.one;

        private Vector2 _rotation = Vector2.zero;

        // Start is called before the first frame update
        void Start()
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }

        // Update is called once per frame
        void Update()
        {
            if (!GameManager.Instance.IsCharacterControllable || TemplateLevelManager.Instance.IsPaused)
                return;

            if (Input.GetKey(KeyCode.W))
            {
                transform.position += transform.forward * _moveSpeed * Time.deltaTime;
            }

            if (Input.GetKey(KeyCode.A))
            {
                transform.position -= transform.right * _moveSpeed * Time.deltaTime;
            }

            if (Input.GetKey(KeyCode.S))
            {
                transform.position -= transform.forward * _moveSpeed * Time.deltaTime;
            }

            if (Input.GetKey(KeyCode.D))
            {
                transform.position += transform.right * _moveSpeed * Time.deltaTime;
            }

            if (Input.GetKey(KeyCode.LeftShift))
            {
                transform.position -= transform.up * _moveSpeed * Time.deltaTime;
            }

            if (Input.GetKey(KeyCode.Space))
            {
                transform.position += transform.up * _moveSpeed * Time.deltaTime;
            }

            if (Input.mouseScrollDelta.y > 0)
            {
                _moveSpeed++;
            }

            if (Input.mouseScrollDelta.y < 0)
            {
                _moveSpeed--;
            }

            Look();
        }

        private void Look()
        {
            Vector2 mouse = new Vector2(Input.GetAxis("Mouse X") * _lookSpeed.x, Input.GetAxis("Mouse Y") * _lookSpeed.y);
            _rotation += new Vector2(-mouse.y, mouse.x);

            _rotation.x = Mathf.Clamp(_rotation.x, -90, 90);

            transform.eulerAngles = new Vector3(0.0f, _rotation.y, 0.0f);
            transform.eulerAngles = new Vector3(_rotation.x, transform.eulerAngles.y, 0.0f);
        }
    } 
}
