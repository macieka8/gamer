using UnityEngine;

namespace gamer.maingame.interactable
{
    public class PlayerInteractableTracer : MonoBehaviour, IInteractableTracer
    {
        [SerializeField] float _raycastRange = 1f;
        [SerializeField] LayerMask _interactableLayer;

        Camera _camera;
        IInteractable _foundInteractable;

        public Camera Cam { get {
            if (_camera == null) _camera = Camera.main;
            return _camera;
        } }

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
                    if (hit.collider.TryGetComponent<IGameMachine>(out var foundInteractable))
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

        public bool TryGetInteractable(out IInteractable foundInteractable)
        {
            if (_foundInteractable != null)
            {
                foundInteractable = _foundInteractable;
                return true;
            }
            foundInteractable = null;
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
