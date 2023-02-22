using UnityEngine;
using UnityEngine.InputSystem;

namespace gamer.pacman
{
    public class PlayerPacmanGamerInputBinder : GamerInputBinder
    {
        [SerializeField] InputActionMapReference _pacmanActionMap;
        [SerializeField] InputActionReference _moveInputAction;

        IInputSender _moveInputSender;

        public override string ActionMapName => _pacmanActionMap.Value.name;

        void OnEnable()
        {
            _moveInputAction.action.performed += HandleMoveInput;
            _moveInputAction.action.canceled += HandleMoveInput;
        }

        void OnDisable()
        {
            _moveInputAction.action.performed -= HandleMoveInput;
            _moveInputAction.action.canceled -= HandleMoveInput;
        }

        void HandleMoveInput(InputAction.CallbackContext ctx)
        {
            _moveInputSender?.SendInput(ctx.ReadValue<Vector2>());
        }

        public override void Bind(IInputSenderMap inputSenderMap)
        {
            if (_pacmanActionMap.Value.name != inputSenderMap.GetActionMapName) return;
            foreach (var entry in inputSenderMap.Map)
            {
                var inputSender = entry.Value;
                if (_moveInputAction.action.name == entry.Key)
                {
                    _moveInputSender = inputSender;
                }
            }
        }

        public override void Unbind()
        {
            _moveInputSender = null;
        }
    }
}