using UnityEngine;
using UnityEngine.InputSystem;
using UnityEditor;
namespace gamer
{
    [CreateAssetMenu(menuName = "PlayerInputActionMap")]
    public class InputActionMapReference : ScriptableObject
    {
        [SerializeField] InputActionAsset _actions;
        [SerializeField] string _actionMap;
        public string ActionMap {
            get => _actionMap;
            set => _actionMap = value;
        }

        public InputActionMap Value => _actions.FindActionMap(_actionMap, true);
    }
}
