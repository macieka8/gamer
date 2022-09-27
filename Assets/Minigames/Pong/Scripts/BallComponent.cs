using UnityEngine;

namespace gamer.pong
{
    public class BallComponent : MonoBehaviour
    {
        [SerializeField] float _speed;

        Rigidbody2D _rigidbody;
        Collider2D _lastCollider;
        bool _isLastColliderSet = false;

        void Awake()
        {
            _rigidbody = GetComponent<Rigidbody2D>();
        }

        void OnEnable()
        {
            _rigidbody.velocity = transform.InverseTransformDirection(Vector2.left);
        }

        void FixedUpdate()
        {
            _rigidbody.velocity = transform.InverseTransformDirection(_rigidbody.velocity.normalized * _speed);
        }

        void OnCollisionEnter2D(Collision2D collision)
        {
            if (collision.collider.TryGetComponent<PaddleComponent>(out var _))
            {
                Physics2D.IgnoreCollision(collision.collider, collision.otherCollider, true);
                if (_isLastColliderSet)
                    Physics2D.IgnoreCollision(_lastCollider, collision.otherCollider, false);
                _lastCollider = collision.collider;
                _isLastColliderSet = true;
            }
        }
    }
}
