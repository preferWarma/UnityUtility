using UnityEngine;

namespace Homework
{
    [RequireComponent(typeof(CharacterController))]
    public class FirstPersonController : MonoBehaviour
    {
        public float speed = 5.0f;
        public float mouseSensitivity = 3.0f;
        public float jumpHeight = 2.0f;
        public float gravity = -9.81f;

        private CharacterController _characterController;
        private float _verticalRotation;
        private float _verticalVelocity;

        private void Start()
        {
            _characterController = GetComponent<CharacterController>();
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }

        private void Update()
        {
            // 鼠标水平旋转
            var horizontalRotation = Input.GetAxis("Mouse X") * mouseSensitivity;
            transform.Rotate(0, horizontalRotation, 0);

            // 鼠标垂直旋转
            _verticalRotation -= Input.GetAxis("Mouse Y") * mouseSensitivity;
            _verticalRotation = Mathf.Clamp(_verticalRotation, -60f, 60f);
            Camera.main!.transform.localRotation = Quaternion.Euler(_verticalRotation, 0, 0);

            // 移动
            var forwardSpeed = Input.GetAxis("Vertical") * speed;
            var sideSpeed = Input.GetAxis("Horizontal") * speed;
            _verticalVelocity += gravity * Time.deltaTime;

            if (_characterController.isGrounded && Input.GetButtonDown("Jump"))
            {
                _verticalVelocity = Mathf.Sqrt(jumpHeight * -2f * gravity);
            }

            var speedVector = new Vector3(sideSpeed, _verticalVelocity, forwardSpeed);
            speedVector = transform.rotation * speedVector;

            _characterController.Move(speedVector * Time.deltaTime);
        }
    }
}