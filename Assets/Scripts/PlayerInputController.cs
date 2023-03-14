using UnityEngine;
using UnityEngine.InputSystem;

namespace gamer
{
    [CreateAssetMenu(menuName = "PlayerInputController")]
    public class PlayerInputController : ScriptableObject
    {
        [SerializeField] InputActionAsset _actions;
        [SerializeField] string _defaultActionMap;
        [SerializeField] string _globalActionMap;

        string _previousActionMap;
        InputActionMap _activeActionMap;

        public event System.Action OnActionMapChanged;

        public InputActionMap ActiveActionMap => _activeActionMap;
        public string DefaultActionMap
        {
            get => _defaultActionMap;
            set => _defaultActionMap = value;
        }

        public string GlobalActionMap
        {
            get => _globalActionMap;
            set => _globalActionMap = value;
        }

        void OnEnable()
        {
            _activeActionMap = _actions.FindActionMap(_defaultActionMap, true);
            _activeActionMap.Enable();
            if (!string.IsNullOrEmpty(_globalActionMap))
                _actions.FindActionMap(_globalActionMap, true).Enable();
        }

        public void SetActiveActionMap(string nameOrId, bool throwIfNotFound = false)
        {
            var newActionMap = _actions.FindActionMap(nameOrId, throwIfNotFound);
            if (newActionMap == null || _activeActionMap == newActionMap) return;

            _previousActionMap = _activeActionMap.name;
            _activeActionMap.Disable();
            _activeActionMap = newActionMap;
            _activeActionMap.Enable();

            OnActionMapChanged?.Invoke();
        }

        public void RestoreDefaultActionMap()
        {
            _previousActionMap = _activeActionMap.name;
            _activeActionMap.Disable();
            _activeActionMap = _actions.FindActionMap(_defaultActionMap, true);
            _activeActionMap.Enable();
        }

        public void RestorePreviousActionMap()
        {
            var newPrevious = _activeActionMap.name;
            _activeActionMap.Disable();
            _activeActionMap = _actions.FindActionMap(_previousActionMap, true);
            _previousActionMap = newPrevious;
            _activeActionMap.Enable();
        }
    }
}
