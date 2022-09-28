using UnityEngine;

namespace gamer.maingame.interactable
{
    public class Gamer : MonoBehaviour
    {
        [SerializeField] float _raycastRange = 1f;
        [SerializeField] LayerMask _interactableLayer;

        IGamerInput _gamerInput;
        IInteractorInput _interactorInput;
        Camera _camera;

        public Camera Cam { get {
            if (_camera == null) _camera = Camera.main;
            return _camera;
        } }

        GameMachine _foundInteractable;
        bool _isFocusedOnInteractabe;

        void Awake()
        {
            _interactorInput = GetComponent<IInteractorInput>();
            _gamerInput = GetComponent<IGamerInput>();
        }

        void OnEnable()
        {
            _interactorInput.OnInteractInput += HandleInteractInput;
            _interactorInput.OnMinigameQuitInput += HandleMinigameUnfocusInput;
        }

        void Update()
        {
            if (_foundInteractable == null)
            {
                FindInteractable();
            }
            else if (!IsStillFocusedOnInteractable())
            {
                _foundInteractable = null;
            }
        }

        void OnDisable()
        {
            _interactorInput.OnInteractInput -= HandleInteractInput;
            _interactorInput.OnMinigameQuitInput -= HandleMinigameUnfocusInput;
        }

        void HandleInteractInput()
        {
            if (_foundInteractable.State == GameMachineState.On)
            {
                FocusOnInteractable();
            }

            _foundInteractable?.Interact();
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
                _isFocusedOnInteractabe = true;
                _foundInteractable.PlayerOnFocusedCamera.SetActive(true);
                _gamerInput.SetActiveGamerState(_foundInteractable.Minigame.InputActionMapName);
        }

        void UnfocusOnInteractable()
        {
                _gamerInput.RestoreDefaultGamerState();
                _foundInteractable.PlayerOnFocusedCamera.SetActive(false);
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
                        _foundInteractable = foundInteractable;
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
                    if (hit.collider.TryGetComponent<GameMachine>(out var foundInteractable))
                    {
                        return foundInteractable == _foundInteractable;
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
            Gizmos.color = (_foundInteractable == null) ? Color.red : Color.green;
            Gizmos.DrawLine(
                Cam.transform.position,
                Cam.transform.position + (Cam.transform.TransformDirection(Vector3.forward) * _raycastRange));
        }
    }
}
