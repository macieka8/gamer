namespace gamer.maingame.interactable
{
    public interface IInteractableTracer
    {
        public bool TryGetInteractable(out IInteractable foundInteractable);
    }
}
