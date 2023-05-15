using UnityEngine;
using UnityEngine.InputSystem;

namespace gamer.maingame.movement
{
    public class PlayerCharacterMovementInput : MonoBehaviour, ICharacterMovementInput
    {
        [SerializeField] InputActionReference _movementInput;
        [SerializeField] InputActionReference _jumpInput;

        public event System.Action<Vector2> OnMovementInput;
        public event System.Action OnJumpInput;

        void Start()
        {
            _movementInput.action.performed += HandleMovementInput;
            _movementInput.action.canceled += HandleMovementInput;
            _jumpInput.action.performed += HandleJumpInput;
        }

        void OnDestroy()
        {
            _movementInput.action.performed -= HandleMovementInput;
            _movementInput.action.canceled -= HandleMovementInput;
            _jumpInput.action.performed -= HandleJumpInput;
        }

        void HandleMovementInput(InputAction.CallbackContext ctx)
        {
            OnMovementInput?.Invoke(ctx.ReadValue<Vector2>());
        }

        void HandleJumpInput(InputAction.CallbackContext ctx)
        {
            OnJumpInput?.Invoke();
        }
    }
}
