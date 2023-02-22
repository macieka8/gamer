using UnityEngine;
using Unity.Mathematics;

namespace gamer.pacman
{
    public class PacmanMovementComponent : MonoBehaviour
    {
        [SerializeField] float _speed;
        [SerializeField] Vector2InputSender _movementInputSender;

        PacmanMovement _pacmanMovement;
        Transform _transform;

        void Awake()
        {
            _transform = transform;
            _pacmanMovement = new PacmanMovement(_speed);
        }

        void OnEnable()
        {
            _movementInputSender.OnInput += HandleMovementInput;

            //debug
            _movementInputSender.InputAction.action.performed += DebugMovement;
            _movementInputSender.InputAction.action.canceled += DebugMovement;
            _movementInputSender.InputAction.action.Enable();
        }

        //todo: remove debug
        void DebugMovement(UnityEngine.InputSystem.InputAction.CallbackContext obj)
        {
            HandleMovementInput(obj.ReadValue<Vector2>());
        }

        void OnDisable()
        {
            _movementInputSender.OnInput -= HandleMovementInput;

            //debug
            _movementInputSender.InputAction.action.performed -= DebugMovement;
            _movementInputSender.InputAction.action.canceled -= DebugMovement;
            _movementInputSender.InputAction.action.Disable();
        }

        void HandleMovementInput(Vector2 value)
        {
            if (value == Vector2.zero) return;
            if (value.y != 0f)
                value.x = 0f;

            _pacmanMovement.SetDesiredMoveDirection((float2)value);
        }

        void Update()
        {
            _pacmanMovement.UpdateMove(PacmanWorld.Instance.Layout);
            _transform.localPosition = (Vector2)_pacmanMovement.Position;
        }
    }
}
