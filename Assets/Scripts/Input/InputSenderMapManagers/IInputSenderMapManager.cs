namespace gamer
{
    public interface IInputSenderMapManager
    {
        public IInputSenderMap ClaimInputSenderMap();
        public void ReleaseInputSenderMap(IInputSenderMap map);
    }
}
