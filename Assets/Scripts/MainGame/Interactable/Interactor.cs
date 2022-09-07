using UnityEngine;
using UnityEngine.InputSystem;

namespace gamer.maingame.interactable
{
    public class Interactor : MonoBehaviour
    {
        [SerializeField] InputActionReference _interactInputAction;
        [SerializeField] float _raycastRange = 1f;
        [SerializeField] LayerMask _interactableLayer;

        Camera _camera;

        public Camera Cam { get {
            if (_camera == null) _camera = Camera.main;
            return _camera;
        } }

        IInteractable _focusedInteractable;

        void Start()
        {
            _interactInputAction.action.performed += HandleInteract;
            _interactInputAction.action.Enable();
        }

        void Update()
        {
            if (_focusedInteractable == null)
            {
                FindInteractable();
            }
            else if (!IsStillFocusedOnInteractable())
            {
                _focusedInteractable = null;
            }
        }

        void OnDestroy()
        {
            _interactInputAction.action.performed -= HandleInteract;
            _interactInputAction.action.Disable();
        }

        void HandleInteract(InputAction.CallbackContext ctx)
        {
            _focusedInteractable?.Interact();
        }

        void FindInteractable()
        {
            if (Physics.Raycast(
                    Cam.transform.position, Cam.transform.TransformDirection(Vector3.forward),
                    out var hit,
                    _raycastRange,
                    _interactableLayer))
                {
                    if (hit.collider.TryGetComponent<IInteractable>(out var foundInteractable))
                    {
                        _focusedInteractable = foundInteractable;
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
                    if (hit.collider.TryGetComponent<IInteractable>(out var foundInteractable))
                    {
                        return foundInteractable == _focusedInteractable;
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
            Gizmos.color = (_focusedInteractable == null) ? Color.red : Color.green;
            Gizmos.DrawLine(
                Cam.transform.position,
                Cam.transform.position + (Cam.transform.TransformDirection(Vector3.forward) * _raycastRange));
        }
    }
}
