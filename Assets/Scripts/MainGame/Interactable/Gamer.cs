using UnityEngine;

namespace gamer.maingame.interactable
{
    public class Gamer : MonoBehaviour
    {
        [SerializeField] float _raycastRange = 1f;
        [SerializeField] LayerMask _interactableLayer;

        IInputMapController _gamerInput;
        IInteractorInput _interactorInput;
        Camera _camera;
        IInputSenderMap _currentlyUsedInputSenderMap;

        public Camera Cam { get {
            if (_camera == null) _camera = Camera.main;
            return _camera;
        } }

        IGameMachine _foundGameMachine;
        bool _isFocusedOnInteractabe;

        void Awake()
        {
            _interactorInput = GetComponent<IInteractorInput>();
            _gamerInput = GetComponent<IInputMapController>();
        }

        void OnEnable()
        {
            _interactorInput.OnInteractInput += HandleInteractInput;
            _interactorInput.OnMinigameQuitInput += HandleMinigameUnfocusInput;
        }

        void Update()
        {
            if (_foundGameMachine == null)
            {
                FindInteractable();
            }
            else if (!IsStillFocusedOnInteractable())
            {
                _foundGameMachine = null;
            }
        }

        void OnDisable()
        {
            _interactorInput.OnInteractInput -= HandleInteractInput;
            _interactorInput.OnMinigameQuitInput -= HandleMinigameUnfocusInput;
        }

        void HandleInteractInput()
        {
            if (_foundGameMachine == null) return;
            if (_foundGameMachine.State == GameMachineState.On)
            {
                FocusOnInteractable();
            }

            _foundGameMachine?.Interact();
        }

        void HandleMinigameUnfocusInput()
        {
            if (_isFocusedOnInteractabe)
            {
                UnfocusOnInteractable();
            }
        }

        void FocusOnInteractable()
        {
            if (_foundGameMachine.TryConnectGamer(out var inputSenderMap))
            {
                _isFocusedOnInteractabe = true;
                _currentlyUsedInputSenderMap = inputSenderMap;
                _foundGameMachine.PlayerOnFocusedCamera.SetActive(true);
                _gamerInput.SetActiveGamerState(inputSenderMap);
            }
        }

        void UnfocusOnInteractable()
        {
            _foundGameMachine.DisconnectGamer(_currentlyUsedInputSenderMap);
            _currentlyUsedInputSenderMap = null;
            _foundGameMachine.PlayerOnFocusedCamera.SetActive(false);
            _gamerInput.RestoreDefaultGamerState();
            _isFocusedOnInteractabe = false;
        }

        void FindInteractable()
        {
            if (Physics.Raycast(
                    Cam.transform.position, Cam.transform.TransformDirection(Vector3.forward),
                    out var hit,
                    _raycastRange,
                    _interactableLayer))
                {
                    if (hit.collider.TryGetComponent<GameMachine>(out var foundInteractable))
                    {
                        _foundGameMachine = foundInteractable;
                    }
                }
        }

        bool IsStillFocusedOnInteractable()
        {
            if (Physics.Raycast(
                    Cam.transform.position, Cam.transform.TransformDirection(Vector3.forward),
                    out var hit,
                    _raycastRange,
                    _interactableLayer))
                {
                    if (hit.collider.TryGetComponent<IGameMachine>(out var foundInteractable))
                    {
                        return foundInteractable == _foundGameMachine;
                    }
                    else
                    {
                        return false;
                    }
                }
            return false;
        }

        void OnDrawGizmosSelected()
        {
            Gizmos.color = (_foundGameMachine == null) ? Color.red : Color.green;
            Gizmos.DrawLine(
                Cam.transform.position,
                Cam.transform.position + (Cam.transform.TransformDirection(Vector3.forward) * _raycastRange));
        }
    }
}
