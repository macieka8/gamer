namespace gamer
{
    public interface IInputReceiver : IInputReceiver<object> { }
    public interface IInputReceiver<T>
    {
        public delegate void OnInputAction(T value);
        public event OnInputAction OnInput;
    }
}
