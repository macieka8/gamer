using UnityEngine;
using UnityEngine.InputSystem;

namespace gamer.maingame.movement
{
    public class CharacterMovement : MonoBehaviour
    {
        [SerializeField] InputActionReference _movementInput;
        [SerializeField] InputActionReference _jumpInput;
        [SerializeField] float _speed = 5f;
        [SerializeField] float _jumpHeight = 5f;
        [SerializeField] float _groundDistance = 0.2f;
        [SerializeField] LayerMask _groundLayer;
        [SerializeField] Transform _groundChecker;

        Rigidbody _rigidbody;
        Camera _camera;
        public Camera Cam { get {
            if (_camera == null) _camera = Camera.main;
            return _camera;
        } }

        bool _isGrounded = true;
        Vector3 _moveInput;
        Vector3 _moveDir;

        void Awake()
        {
            _rigidbody = GetComponent<Rigidbody>();
        }

        void OnEnable()
        {
            _movementInput.action.performed += HandleMovementInput;
            _movementInput.action.canceled += HandleMovementInput;
            _jumpInput.action.performed += HandleJumpInput;
        }

        void Update()
        {
            _isGrounded = Physics.CheckSphere(_groundChecker.position,
                                        _groundDistance,
                                        _groundLayer,
                                        QueryTriggerInteraction.Ignore);

            if (_moveInput.magnitude >= 0.1f)
            {
                // Angle in degrees in which we want to rotate an object based on camera position
                float angle = (Mathf.Atan2(_moveInput.x, _moveInput.z) * Mathf.Rad2Deg) + Cam.transform.eulerAngles.y;

                // New normalized movement direction multiplied by input magnitude
                _moveDir = (Quaternion.Euler(0f, angle, 0f) * Vector3.forward).normalized * _moveInput.magnitude;
            }
            transform.forward = (Quaternion.Euler(0f, Cam.transform.eulerAngles.y, 0f) * Vector3.forward).normalized;
        }

        void FixedUpdate()
        {
            var velocityDelta = _moveDir * _speed * Time.deltaTime;
            _rigidbody.MovePosition(_rigidbody.position + velocityDelta);
        }

        void OnDisable()
        {
            _movementInput.action.performed -= HandleMovementInput;
            _movementInput.action.canceled -= HandleMovementInput;
            _jumpInput.action.performed -= HandleJumpInput;
        }

        void HandleMovementInput(InputAction.CallbackContext ctx)
        {
            var input = ctx.ReadValue<Vector2>();
            _moveInput = new Vector3(input.x, 0f, input.y);
            if (_moveInput.magnitude > 1.0f)
                _moveInput.Normalize();
            _moveDir = _moveInput;
        }

        void HandleJumpInput(InputAction.CallbackContext ctx)
        {
            if (_isGrounded)
            {
                _rigidbody.AddForce(Vector3.up * Mathf.Sqrt(_jumpHeight * Physics.gravity.y * -2.0f),
                                    ForceMode.VelocityChange);
            }
        }
    }
}
