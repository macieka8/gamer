namespace gamer
{
    public interface IInputSender
    {
        public string InputName { get; }
        public void SendInput(object input);
    }
}
