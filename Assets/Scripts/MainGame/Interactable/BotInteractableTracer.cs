using System;
using UnityEngine;

namespace gamer.maingame.interactable
{
    public class BotInteractableTracer : MonoBehaviour, IInteractableTracer
    {
        [SerializeField] LayerMask _interactableLayer;
        [SerializeField] float _scanDistance;

        IInteractable _foundInteractable;

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
            var colliders = Physics.OverlapSphere(transform.position, _scanDistance, _interactableLayer);
            if (colliders?.Length > 0)
            {
                if (colliders[0].TryGetComponent<IInteractable>(out var foundInteractable))
                {
                    _foundInteractable = foundInteractable;
                }
            }
        }

        bool IsStillFocusedOnInteractable()
        {
            return _foundInteractable != null
                && Vector3.Distance(transform.position, _foundInteractable.Transform.position) < _scanDistance;
        }

        public bool TryGetInteractable(out IInteractable foundInteractable)
        {
            foundInteractable = _foundInteractable;
            return _foundInteractable != null;
        }
    }
    }
