using UnityEngine;
using UnityEngine.InputSystem;

namespace gamer
{
    [CreateAssetMenu(menuName = "PlayerInputController")]
    public class PlayerInputController : ScriptableObject
    {
        [SerializeField] InputActionAsset _asset;
        [SerializeField] string _defualtActionMapName = "MainGame";

        InputActionMap _activeActionMap;

        void OnEnable()
        {
            _activeActionMap = _asset.FindActionMap(_defualtActionMapName, true);
            _activeActionMap.Enable();
        }

        public void SetActiveActionMap(string nameOrId, bool throwIfNotFound = false)
        {
            var newActionMap = _asset.FindActionMap(nameOrId, throwIfNotFound);
            if (newActionMap == null || _activeActionMap == newActionMap) return;

            _activeActionMap.Disable();
            _activeActionMap = newActionMap;
            _activeActionMap.Enable();
        }

        public void RestoreDefaultActionMap()
        {
            _activeActionMap.Disable();
            _activeActionMap = _asset.FindActionMap(_defualtActionMapName, true);
            _activeActionMap.Enable();
        }
    }
}
