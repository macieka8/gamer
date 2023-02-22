using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections.Generic;

namespace gamer.tetris
{
    public class PlayerTetrisGamerInputBinder : GamerInputBinder
    {
        [SerializeField] InputActionMapReference _tetrisActionMap;
        [Header("Inputs")]
        [SerializeField] InputActionReference _moveInputAction;
        [SerializeField] InputActionReference _rotateInputAction;
        [SerializeField] InputActionReference _softDropInputAction;
        [SerializeField] InputActionReference _hardDropInputAction;
        [SerializeField] InputActionReference _savePuzzleInputAction;

        IReadOnlyDictionary<string, IInputSender> _actionNameToSender;

        public override string ActionMapName => _tetrisActionMap.Value.name;

        void OnEnable()
        {
            _moveInputAction.action.performed += HandleMoveInput;
            _moveInputAction.action.canceled += HandleMoveInput;

            _rotateInputAction.action.performed += HandleRotateInput;

            _softDropInputAction.action.started += HandleSoftDropInput;
            _softDropInputAction.action.canceled += HandleSoftDropInput;

            _hardDropInputAction.action.performed += HandleHardDrop;

            _savePuzzleInputAction.action.performed += HandlePuzzleSave;
        }

        void OnDisable()
        {
            _moveInputAction.action.performed -= HandleMoveInput;
            _moveInputAction.action.canceled -= HandleMoveInput;
            _rotateInputAction.action.performed -= HandleRotateInput;

            _softDropInputAction.action.started -= HandleSoftDropInput;
            _softDropInputAction.action.canceled -= HandleSoftDropInput;
            _hardDropInputAction.action.performed -= HandleHardDrop;

            _savePuzzleInputAction.action.performed -= HandlePuzzleSave;
        }

        void HandleHardDrop(InputAction.CallbackContext obj)
        {
            _actionNameToSender[_hardDropInputAction.action.name]
                .SendInput(null);
        }

        void HandleSoftDropInput(InputAction.CallbackContext obj)
        {
            _actionNameToSender[_softDropInputAction.action.name]
                .SendInput(obj.canceled);
        }

        void HandleRotateInput(InputAction.CallbackContext obj)
        {
            _actionNameToSender[_rotateInputAction.action.name]
                .SendInput(null);
        }

        void HandleMoveInput(InputAction.CallbackContext obj)
        {
            _actionNameToSender[_moveInputAction.action.name]
                .SendInput(obj.ReadValue<float>());
        }

        void HandlePuzzleSave(InputAction.CallbackContext ctx)
        {
            _actionNameToSender[_savePuzzleInputAction.action.name]
                .SendInput(null);
        }

        public override void Bind(IInputSenderMap inputSenderMap)
        {
            if (_tetrisActionMap.Value.name != inputSenderMap.GetActionMapName) return;
            _actionNameToSender = inputSenderMap.Map;
        }

        public override void Unbind()
        {
            _actionNameToSender = null;
        }
    }
}
