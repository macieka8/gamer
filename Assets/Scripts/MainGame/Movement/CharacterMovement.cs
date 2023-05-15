using UnityEngine;

namespace gamer.maingame.movement
{
    public class CharacterMovement : MonoBehaviour
    {
        [SerializeField] float _acceleration = 40f;
        [SerializeField] float _speed = 5f;
        [SerializeField] float _jumpHeight = 5f;
        [SerializeField] float _groundDistance = 0.2f;
        [SerializeField] LayerMask _groundLayer;
        [SerializeField] Transform _groundChecker;

        ICharacterMovementInput _input;
        Rigidbody _rigidbody;
        Camera _camera;

        float _sqrSpeed;

        public Camera Cam { get {
            if (_camera == null) _camera = Camera.main;
            return _camera;
        } }

        bool _isGrounded = true;
        Vector3 _moveInput;
        Vector3 _moveDir;

        void Awake()
        {
            _sqrSpeed = _speed * _speed;
            _input = GetComponent<ICharacterMovementInput>();
            _rigidbody = GetComponent<Rigidbody>();
        }

        void OnEnable()
        {
            _input.OnMovementInput += HandleMovementInput;
            _input.OnJumpInput += HandleJumpInput;
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
            var velocityDelta = _moveDir * _acceleration;
            _rigidbody.AddForce(velocityDelta, ForceMode.Acceleration);
            var horiznotalVelocity = new Vector2(_rigidbody.velocity.x, _rigidbody.velocity.z);
            if (horiznotalVelocity.sqrMagnitude > _sqrSpeed)
            {
                var newHorizontalVelocity = horiznotalVelocity.normalized * _speed;
                var newVelocity = new Vector3(newHorizontalVelocity.x, _rigidbody.velocity.y, newHorizontalVelocity.y);
                _rigidbody.velocity = newVelocity;
            }
        }

        void OnDisable()
        {
            _input.OnMovementInput -= HandleMovementInput;
            _input.OnJumpInput -= HandleJumpInput;
        }

        void HandleMovementInput(Vector2 moveInput)
        {
            _moveInput = new Vector3(moveInput.x, 0f, moveInput.y);
            if (_moveInput.magnitude > 1.0f)
                _moveInput.Normalize();
            _moveDir = _moveInput;
        }

        void HandleJumpInput()
        {
            if (_isGrounded)
            {
                _rigidbody.AddForce(Vector3.up * Mathf.Sqrt(_jumpHeight * Physics.gravity.y * -2.0f),
                                    ForceMode.VelocityChange);
            }
        }
    }
}
