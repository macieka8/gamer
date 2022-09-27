using UnityEngine;
using UnityEngine.Events;

namespace gamer.pong
{
    public class GoalComponent : MonoBehaviour
    {
        public static readonly int MAX_SCORE = 11;

        int _score = 0;

        public event System.Action<int> OnGoalScored;
        public UnityEvent OnGameWin;

        void OnEnable()
        {
            _score = 0;
        }

        void OnTriggerEnter2D(Collider2D collider)
        {
            if (collider.TryGetComponent<BallComponent>(out var _))
            {
                _score++;
                if (_score == MAX_SCORE) OnGameWin.Invoke();
                collider.transform.localPosition = Vector2.zero;
                collider.attachedRigidbody.velocity = transform.InverseTransformDirection(
                    Mathf.Sign(collider.attachedRigidbody.velocity.x) * Vector2.right);
                OnGoalScored?.Invoke(_score);
            }
        }
    }
}
