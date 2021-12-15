using UnityEngine;

namespace UPDB.CamerasAndCharacterControllers.CharacterControllers.RbFpsController
{
    public class CharacterController : MonoBehaviour
    {
        #region Serialized And Public Variables

        [Header("OBJECTS")]


        [SerializeField, Tooltip("rigidBody used to calculate player movements")]
        private Rigidbody _rb;

        [SerializeField, Tooltip("collider Rigidbody will use to calculate player movements")]
        private Collider _collider;

        [SerializeField, Tooltip("speed of character movements")]
        private float _speed = 0;

        [SerializeField, Tooltip("speed of mouse look in X and Y")]
        private Vector2 _lookSpeed = Vector2.zero;


        #endregion

        #region Private Variables

        private float xRotation = 0;
        private float yRotation = 0;

        #endregion

        #region Accessors

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

        public Vector2 LookSpeed
        {
            get { return _lookSpeed; }
            set { _lookSpeed = value; }
        }

        #endregion

        public void InitVariables()
        {

        }

        private void Update()
        {
            
        }

        private void Move()
        {
            float x = Input.GetAxis("Horizontal");
            float z = Input.GetAxis("Vertical");

            Vector3 move = transform.right * x + transform.forward * z;

            _rb.AddForce(move * _speed * Time.deltaTime);
        }

        private void Look()
        {
            float mouseX = Input.GetAxis("Mouse X") * _lookSpeed.x;
            float mouseY = Input.GetAxis("Mouse Y") * _lookSpeed.y;

            yRotation += mouseX;
            xRotation -= mouseY;
            xRotation = Mathf.Clamp(xRotation, -90, 90);

            transform.eulerAngles = new Vector3(0.0f, yRotation, 0.0f);
            Camera.main.transform.eulerAngles = new Vector3(xRotation, Camera.main.transform.eulerAngles.y, 0.0f);
        }
    } 
}
