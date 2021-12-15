using UnityEngine;

namespace UPDB.CamerasAndCharacterControllers.CharacterControllers.Rb25DController
{
    public class CharacterController : MonoBehaviour
    {

        #region SerializedVariable

        #region Movements

        [Header(""), Header("MOVEMENTS"), SerializeField, Tooltip("player movements speed")]
        private float speed;

        [SerializeField, Tooltip("player movements slow down speed")]
        private float slowDownSpeed;

        [SerializeField, Tooltip("set respawn highness")]
        private float fallRespawnValue;

        #endregion

        #region GroundAndJump

        [SerializeField, Tooltip("set mask for the ground")]
        private LayerMask groundMask;

        [SerializeField, Tooltip("set mask for the left side")]
        private LayerMask leftMask;

        [SerializeField, Tooltip("set mask for the right side")]
        private LayerMask rightMask;

        [SerializeField, Tooltip("jump strength")]
        private float jumpStrength;

        [SerializeField, Tooltip("jump strength time")]
        private int jumpTime = 10;

        #endregion

        #endregion

        #region PrivateVariables

        #region Physic

        Rigidbody rb;

        private Transform groundCheck;

        private Transform leftCheck;

        private Transform rightCheck;

        #endregion

        #region GroundAndJump

        private float groundDistance = 0.1f;
        private float wallDistance = 0.4f;

        bool isGrounded = false;
        bool isAwayLeft = true;
        bool isAwayRight = true;

        private int jumpTimer = 0;

        #endregion

        private Vector3 initPos;

        #endregion

        private void Awake()
        {
            rb = GetComponent<Rigidbody>();
            initPos = transform.position;

            if (groundCheck == null)
                groundCheck = transform.Find(GameObject.FindWithTag("GroundCheck").transform.name);
            if (leftCheck == null)
                leftCheck = transform.Find(GameObject.FindWithTag("LeftCheck").transform.name);
            if (rightCheck == null)
                rightCheck = transform.Find(GameObject.FindWithTag("RightCheck").transform.name);
        }

        private void FixedUpdate()
        {
            Move();
            Respawn();
        }

        private void Move()
        {
            isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);
            isAwayLeft = !Physics.CheckSphere(leftCheck.position, wallDistance, leftMask);
            isAwayRight = !Physics.CheckSphere(rightCheck.position, wallDistance, rightMask);

            float joystickSpeed = Mathf.Sqrt(Mathf.Pow(Input.GetAxis("PadHorizontal") + Input.GetAxis("Horizontal"), 2));

            if (isGrounded)
                jumpTimer = 0;

            if (jumpTimer < jumpTime)
            {
                if (Input.GetAxis("Jump") == 1)
                    rb.AddForce(0, jumpStrength, 0);
                jumpTimer++;
            }



            if (isAwayLeft)
            {
                if (Input.GetAxis("Horizontal") == -1 || Input.GetAxis("PadHorizontal") < -0.25f)
                {
                    if (rb.velocity.x > -5)
                        rb.AddForce((speed * -Time.deltaTime) * joystickSpeed, 0, 0);
                }
                else
                    rb.AddForce(-rb.velocity.x * slowDownSpeed, 0, -rb.velocity.z * slowDownSpeed);

            }
            if (isAwayRight)
            {
                if (Input.GetAxis("Horizontal") == 1 || Input.GetAxis("PadHorizontal") > 0.25f)
                {
                    if (rb.velocity.x < 5)
                        rb.AddForce((speed * Time.deltaTime) * joystickSpeed, 0, 0);
                }
                else
                    rb.AddForce(-rb.velocity.x * slowDownSpeed, 0, -rb.velocity.z * slowDownSpeed);
            }
        }

        private void Respawn()
        {
            if (transform.position.y < fallRespawnValue)
                transform.position = initPos;
        }

    } 
}
