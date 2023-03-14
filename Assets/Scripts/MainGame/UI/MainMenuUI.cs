using UnityEngine;
using UnityEngine.InputSystem;

namespace gamer
{
    public class MainMenuUI : MonoBehaviour
    {
        [SerializeField] GameObject _menuCamera;
        [SerializeField] GameObject _menu;
        [SerializeField] InputActionReference _menuInputAction;
        [SerializeField] PlayerInputController _inputController;
        [SerializeField] InputActionMapReference _mainMenuActionMap;

        void Start()
        {
            _menuInputAction.action.performed += HandleMenuTrigger;
            _inputController.SetActiveActionMap(_mainMenuActionMap.ActionMap);
        }

        void HandleMenuTrigger(InputAction.CallbackContext obj)
        {
            TriggerPlay();
        }

        public void TriggerPlay()
        {
            _menuCamera.SetActive(!_menuCamera.activeInHierarchy);
            _menu.SetActive(!_menu.activeInHierarchy);
            if (_menu.activeInHierarchy)
                _inputController.SetActiveActionMap(_mainMenuActionMap.ActionMap);
            else
                _inputController.RestorePreviousActionMap();
        }

        public void QuitGame()
        {
            Application.Quit();
        }
    }
}
