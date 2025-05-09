using UnityEngine;
using UPDB.CoreHelper;
using UPDB.CoreHelper.Templates;
using UPDB.CoreHelper.Usable;
using UPDB.CoreHelper.UsableMethods;

namespace UPDB.CamerasAndCharacterControllers.Cameras.TpsCamera
{
    /// <summary>
    /// simple tps camera controller, can be used with fps controller or alone
    /// </summary>
    [HelpURL(URL.baseURL + "/tree/main/CamerasAndCharacterControllers/Cameras/TpsCamera/README.md")]
    [AddComponentMenu(NamespaceID.CamerasPath + "/" + NamespaceID.TpsCamera + "/Tps Camera Controller")]
    public class CameraController : UPDBBehaviour
    {
        [SerializeField, Tooltip("Camera pivot linked to this Player(where you have to put camera script)")]
        private Transform _cameraPivot;

        [SerializeField, Tooltip("speed of mouse look in X and Y")]
        private Vector2 _lookSpeed = Vector2.one;


        private Vector2 _rotation = Vector2.zero;


        public Vector2 LookSpeed
        {
            get { return _lookSpeed; }
            set { _lookSpeed = value; }
        }

        public Transform CameraPivot
        {
            get { return _cameraPivot; }
            set { _cameraPivot = value; }
        }

        private void Awake()
        {
            InitVariables();
        }

        private void Update()
        {
            if (!GameManager.Instance.IsCharacterControllable || TemplateLevelManager.Instance.IsPaused)
                return;

            Look();
        }

        public void InitVariables()
        {
            if (_cameraPivot == null)
                _cameraPivot = transform;
        }

        private void Look()
        {
            Vector2 mouse = new Vector2(Input.GetAxis("Mouse X") * _lookSpeed.x, Input.GetAxis("Mouse Y") * _lookSpeed.y);
            _rotation += new Vector2(-mouse.y, mouse.x);

            _rotation.x = Mathf.Clamp(_rotation.x, -89, 89);

            _cameraPivot.eulerAngles = new Vector3(_rotation.x, _rotation.y, 0.0f);
        }
    } 
}
