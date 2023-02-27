using UnityEngine;

namespace gamer.pacman
{
    public class GhostComponent : MonoBehaviour
    {
        [SerializeField] float _speed;

        Ghost _ghost;

        public PacmanMovement Movement => _ghost.Movement;
        public Ghost Ghost => _ghost;

        void Awake()
        {
            _ghost = new Ghost(_speed, (Vector2)transform.localPosition);
            Movement.SetDesiredMoveDirection(PacmanLayout.GetRandomMoveDirection());
        }

        void OnEnable()
        {
            Movement.OnTargetPositionReached += HandleTargetReached;
        }

        void OnDisable()
        {
            Movement.OnTargetPositionReached -= HandleTargetReached;
        }

        void Update()
        {
            Ghost.UpdateFear(Time.deltaTime);
            Ghost.Movement.UpdateMove(PacmanWorld.Instance.Layout);
            transform.localPosition = (Vector2)Movement.Position;
        }

        void HandleTargetReached()
        {
            _ghost.ChooseNextDirection(PacmanWorld.Instance.Layout, PacmanWorld.Instance.Player);
        }
    }
}
