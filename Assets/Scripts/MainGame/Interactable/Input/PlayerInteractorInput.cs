using System;
using UnityEngine;
using UnityEngine.InputSystem;

namespace gamer.maingame.interactable
{

    public class PlayerInteractorInput : MonoBehaviour, IInteractorInput
    {
        [SerializeField] InputActionReference _quitMinigameInputAction;
        [SerializeField] InputActionReference _interactInputAction;

        public event Action OnMinigameQuitInput;
        public event Action OnInteractInput;

        void Start()
        {
            _interactInputAction.action.performed += HandleInteractInput;
            _quitMinigameInputAction.action.performed += HandleMinigameQuitInput;
        }

        void OnDestroy()
        {
            _interactInputAction.action.performed -= HandleInteractInput;
            _quitMinigameInputAction.action.performed -= HandleMinigameQuitInput;
        }

        void HandleInteractInput(InputAction.CallbackContext ctx)
        {
            OnInteractInput?.Invoke();
        }

        void HandleMinigameQuitInput(InputAction.CallbackContext ctx)
        {
            OnMinigameQuitInput?.Invoke();
        }
    }
}
