using UnityEngine;
using UnityEngine.InputSystem;

namespace gamer.pong
{
    public class PaddleComponent : MonoBehaviour
    {
        [SerializeField] float _speed;
        [SerializeField] float _maxReflectAngle;
        [SerializeField] InputActionReference _moveInputAction;

        Rigidbody2D _rigidbody;

        Vector2 _moveDirection = Vector2.zero;
        float _length;

        void Awake()
        {
            _rigidbody = GetComponent<Rigidbody2D>();
            _length = Mathf.Abs(transform.TransformVector(GetComponent<BoxCollider2D>().size).y);
        }

        void OnEnable()
        {
            transform.localPosition = Vector3.zero;

            _moveInputAction.action.performed += HandleMovementInput;
            _moveInputAction.action.canceled += HandleMovementInput;
        }

        void FixedUpdate()
        {
            _rigidbody.velocity = transform.InverseTransformDirection(_speed * _moveDirection * transform.up);
        }

        void OnCollisionEnter2D(Collision2D collision)
        {
            if (collision.collider.TryGetComponent<BallComponent>(out var _))
            {
                var t = ((collision.rigidbody.position.y - transform.position.y) / _length) + 0.5f;
                var rotation = Mathf.Lerp(-_maxReflectAngle, _maxReflectAngle, t);
                var dir = Mathf.Sign(Vector2.Dot(collision.transform.right, transform.right));
                collision.rigidbody.velocity = Quaternion.Euler(0f, 0f, dir * rotation) * transform.right;
            }
        }

        void OnDisable()
        {
            _moveInputAction.action.performed -= HandleMovementInput;
            _moveInputAction.action.canceled -= HandleMovementInput;
        }

        void HandleMovementInput(InputAction.CallbackContext ctx)
        {
            _moveDirection = new Vector2(0f, ctx.ReadValue<float>());
        }
    }
}
