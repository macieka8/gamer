using UnityEngine;
using Unity.Mathematics;

namespace gamer.pacman
{
    public class PacmanMovementComponent : MonoBehaviour
    {
        [SerializeField] float _speed;
        Vector2InputSender _movementInputSender;

        PacmanMovement _pacmanMovement;
        Transform _transform;

        public PacmanMovement Movement => _pacmanMovement;

        void Awake()
        {
            _transform = transform;
            _pacmanMovement = new PacmanMovement(_speed);
        }

        void Update()
        {
            _pacmanMovement.UpdateMove(PacmanWorld.Instance.Layout);
            _transform.localPosition = (Vector2)_pacmanMovement.Position;
        }

        void HandleMovementInput(Vector2 value)
        {
            if (value == Vector2.zero) return;
            if (value.y != 0f)
                value.x = 0f;

            _pacmanMovement.SetDesiredMoveDirection((float2)value);
        }

        public void SetInputSender(Vector2InputSender inputSender)
        {
            _movementInputSender = inputSender;
            _movementInputSender.OnInput += HandleMovementInput;
        }
    }
}
