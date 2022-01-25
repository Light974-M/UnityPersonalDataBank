using UnityEngine;
using UPDB.CamerasAndCharacterControllers.Cameras.SimpleFpsCamera;

namespace UPDB.CamerasAndCharacterControllers.CharacterControllers.RbFpsController
{
    [HelpURL(URL.baseURL + "/tree/main/CamerasAndCharacterControllers/CharacterControllers/RbFpsController/README.md"), AddComponentMenu("UPDB/CamerasAndCharacterControllers/CharacterControllers/Rigidbody FPS Controller")]
    public class PlayerController : MonoBehaviour
    {
        #region Serialized And Public Variables

        [SerializeField, Tooltip("rigidBody used to calculate player movements")]
        private Rigidbody _rb;

        [SerializeField, Tooltip("collider Rigidbody will use to calculate player movements")]
        private Collider _collider;

        [SerializeField, Tooltip("speed of character movements")]
        private float _speed = 1;


        #endregion

        #region Private Variables

        

        #endregion

        #region Accessors

        /// <inheritdoc cref="_rb"/>
        public Rigidbody Rb
        {
            get { return _rb; }
            set { _rb = value; }
        }

        public Collider Collider
        {
            get { return _collider; }
            set { _collider = value; }
        }

        public float Speed
        {
            get { return _speed; }
            set { _speed = value; }
        }

        #endregion


        private void Awake()
        {
            InitVariables();

            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }

        private void Update()
        {
            if (_rb != null)
                Move();
            else
                Debug.LogWarning("warning : there is no rigidbody attached to this Component");

             _rb.drag = Mathf.Clamp(_rb.drag, 0, 50);
        }

        public void InitVariables()
        {
            if (_rb == null)
            {
                if (!TryGetComponent(out _rb))
                {
                    _rb = gameObject.AddComponent<Rigidbody>();
                    _rb.constraints = RigidbodyConstraints.FreezeRotation;
                    _rb.angularDrag = 0;
                    _rb.drag = 10;
                }

            }
            GameObject camObject;

            if (!transform.Find("Camera"))
            {
                camObject = new GameObject("Camera");
                camObject.transform.SetParent(transform);
                camObject.transform.localPosition = new Vector3(0, 0.5f, 0.48f);
                camObject.transform.localEulerAngles = Vector3.zero;
                camObject.AddComponent<CameraController>();
                camObject.AddComponent<AudioListener>();
            }
        }

        private void Move()
        {
            Vector2 input = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
            Vector3 move = transform.right * input.x + transform.forward * input.y;
            float speedMultiplier = (_speed * 5000);
            
            _rb.AddForce(move * speedMultiplier * Time.deltaTime);
        }
    }
}
