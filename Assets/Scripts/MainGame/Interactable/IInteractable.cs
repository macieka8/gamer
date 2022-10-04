using UnityEngine;

namespace gamer.maingame.interactable
{
    public interface IInteractable
    {
        public Transform Transform { get; }
        public void Interact();
    }
}
