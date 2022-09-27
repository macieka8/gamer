using UnityEngine;
using UnityEngine.InputSystem;
using Unity.Mathematics;
using System;

namespace gamer.tetris
{
    public class PlayerPuzzleMovementInput : MonoBehaviour, IPuzzleMoverInput
    {
        [Header("Player Inputs")]
        [SerializeField] InputActionReference _moveInputAction;
        [SerializeField] InputActionReference _rotateInputAction;
        [SerializeField] InputActionReference _softDropInputAction;
        [SerializeField] InputActionReference _hardDropInputAction;
        [SerializeField] InputActionReference _savePuzzleInputAction;

        public event Action<float> OnMovementInput;
        public event Action<bool> OnSoftDropInput;
        public event Action OnHardDropInput;
        public event Action OnRotationInput;
        public event Action OnSavePuzzleInput;

        void OnEnable()
        {
            //todo: remove enable
            _moveInputAction.action.performed += HandleMoveInput;
            _moveInputAction.action.canceled += HandleMoveInput;
            _moveInputAction.action.Enable();

            _rotateInputAction.action.performed += HandleRotateInput;
            _rotateInputAction.action.Enable();

            _softDropInputAction.action.started += HandleSoftDropInput;
            _softDropInputAction.action.canceled += HandleSoftDropInput;
            _softDropInputAction.action.Enable();

            _hardDropInputAction.action.performed += HandleHardDrop;
            _hardDropInputAction.action.Enable();

            _savePuzzleInputAction.action.performed += HandlePuzzleSave;
            _savePuzzleInputAction.action.Enable();
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
            OnHardDropInput?.Invoke();
        }

        void HandleSoftDropInput(InputAction.CallbackContext obj)
        {
            OnSoftDropInput?.Invoke(obj.canceled);
        }

        void HandleRotateInput(InputAction.CallbackContext obj)
        {
            OnRotationInput?.Invoke();
        }

        void HandleMoveInput(InputAction.CallbackContext obj)
        {
            OnMovementInput?.Invoke(obj.ReadValue<float>());
        }

        void HandlePuzzleSave(InputAction.CallbackContext ctx)
        {
            OnSavePuzzleInput?.Invoke();
        }
    }
}
