using UnityEngine;
using UnityEngine.InputSystem;

namespace gamer
{
    [CreateAssetMenu(menuName = "PlayerInputController")]
    public class PlayerInputController : ScriptableObject
    {
        [SerializeField] InputActionAsset _actions;
        [SerializeField] string _defaultActionMap;

        InputActionMap _activeActionMap;

        public InputActionMap ActiveActionMap => _activeActionMap;
        public string DefaultActionMap
        {
            get => _defaultActionMap;
            set => _defaultActionMap = value;
        }

        void OnEnable()
        {
            _activeActionMap = _actions.FindActionMap(_defaultActionMap, true);
            _activeActionMap.Enable();
        }

        public void SetActiveActionMap(string nameOrId, bool throwIfNotFound = false)
        {
            var newActionMap = _actions.FindActionMap(nameOrId, throwIfNotFound);
            if (newActionMap == null || _activeActionMap == newActionMap) return;

            _activeActionMap.Disable();
            _activeActionMap = newActionMap;
            _activeActionMap.Enable();
        }

        public void RestoreDefaultActionMap()
        {
            _activeActionMap.Disable();
            _activeActionMap = _actions.FindActionMap(_defaultActionMap, true);
            _activeActionMap.Enable();
        }
    }
}
