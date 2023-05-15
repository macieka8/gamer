using UnityEngine;

namespace gamer.maingame.interactable
{
    public class Gamer : MonoBehaviour
    {
        [SerializeField] bool _isPlayer;
        IInteractableTracer _interactableTracer;
        IInputMapController _inputMapController;
        IInteractorInput _interactorInput;

        bool _isFocusedOnInteractabe;
        IInputSenderMap _currentlyUsedInputSenderMap;
        IGameMachine _foundGameMachine;

        void Awake()
        {
            _interactableTracer = GetComponent<IInteractableTracer>();
            _inputMapController = GetComponent<IInputMapController>();
            _interactorInput = GetComponent<IInteractorInput>();
        }

        void OnEnable()
        {
            _interactorInput.OnInteractInput += HandleInteractInput;
            _interactorInput.OnMinigameQuitInput += HandleMinigameUnfocusInput;
        }

        void OnDisable()
        {
            _interactorInput.OnInteractInput -= HandleInteractInput;
            _interactorInput.OnMinigameQuitInput -= HandleMinigameUnfocusInput;
        }

        void HandleInteractInput()
        {
            if (_interactableTracer.TryGetInteractable(out var foundInteractable))
            {
                if (!(foundInteractable is IGameMachine)) return;
                _foundGameMachine = foundInteractable as IGameMachine;
                if (_foundGameMachine.State == GameMachineState.On)
                {
                    FocusOnInteractable();
                }

                _foundGameMachine?.Interact();
            }
        }

        void HandleMinigameUnfocusInput()
        {
            if (_isFocusedOnInteractabe)
            {
                _foundGameMachine.DisconnectGamer(this);
            }
        }

        void FocusOnInteractable()
        {
            if (_foundGameMachine.TryConnectGamer(this, out var inputSenderMap))
            {
                _currentlyUsedInputSenderMap = inputSenderMap;
                _inputMapController.SetActiveGamerState(inputSenderMap);
                if (_isPlayer)
                    _foundGameMachine.PlayerOnFocusedCamera.SetActive(true);
                _isFocusedOnInteractabe = true;
            }
        }

        public void UnfocusInteracable()
        {
            _currentlyUsedInputSenderMap = null;
            _inputMapController.RestoreDefaultGamerState();
            if (_isPlayer)
                _foundGameMachine.PlayerOnFocusedCamera.SetActive(false);
            _isFocusedOnInteractabe = false;
        }
    }
}
