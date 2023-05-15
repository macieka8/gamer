using System;

namespace gamer.maingame.interactable
{
    public interface IInteractorInput
    {
        public event Action OnMinigameQuitInput;
        public event Action OnInteractInput;
    }
}
