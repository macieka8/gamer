using UnityEngine;
using UnityEngine.InputSystem;

namespace gamer
{
    public class InputSenderReceiver : IInputSender, IInputReceiver<object>
    {
        InputActionReference _action;

        public event IInputReceiver<object>.OnInputAction OnInput;
        public string InputName => _action.action.name;

        public InputSenderReceiver(InputActionReference inputAction)
        {
            _action = inputAction;
        }

        public void SendInput(object input)
        {
            OnInput?.Invoke(null);
        }
    }
    public class InputSenderReceiver<T> : IInputSender, IInputReceiver<T>
    {
        InputActionReference _action;

        public event IInputReceiver<T>.OnInputAction OnInput;
        public string InputName => _action.action.name;

        public InputSenderReceiver(InputActionReference inputAction)
        {
            _action = inputAction;
        }

        public void SendInput(object input)
        {
            OnInput?.Invoke((T)input);
        }
    }
}
