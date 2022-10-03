namespace gamer
{
    public interface IInputMapController
    {
        public void SetActiveGamerState(IInputSenderMap inputSenderMap);
        public void RestoreDefaultGamerState();
    }
}
