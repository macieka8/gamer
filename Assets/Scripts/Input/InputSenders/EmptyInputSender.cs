using UnityEngine;
using UnityEngine.InputSystem;
using System;

namespace gamer
{
    public class EmptyInputSender : MonoBehaviour, IInputSender, IInputReceiver<object>
    {
        [SerializeField] InputActionReference _action;
        public event IInputReceiver<object>.OnInputAction OnInput;
        public string InputName => _action.action.name;

        public void SendInput(object input = null)
        {
            OnInput?.Invoke(null);
        }
    }
}
