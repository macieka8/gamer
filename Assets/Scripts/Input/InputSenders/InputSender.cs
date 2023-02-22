using UnityEngine;
using UnityEngine.InputSystem;

namespace gamer
{
    public class InputSender<T> : MonoBehaviour, IInputSender, IInputReceiver<T>
    {
        [SerializeField] InputActionReference _action;
        
        public event IInputReceiver<T>.OnInputAction OnInput;

        public string InputName => _action.action.name;
        public InputActionReference InputAction => _action;

        public void SendInput(object input)
        {
            OnInput?.Invoke((T)input);
        }
    }
}
