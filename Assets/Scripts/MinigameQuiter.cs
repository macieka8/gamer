using UnityEngine;
using UnityEngine.InputSystem;

namespace gamer
{
    public class MinigameQuiter : MonoBehaviour
    {
        [SerializeField] InputActionReference _quitMinigameInputAction;
        [SerializeField] PlayerInputController _playerInputController;

        void Start()
        {
            _quitMinigameInputAction.action.performed += HandleMinigameQuit;
        }

        void OnDestroy()
        {
            _quitMinigameInputAction.action.performed -= HandleMinigameQuit;
        }

        void HandleMinigameQuit(InputAction.CallbackContext ctx)
        {
            _playerInputController.RestoreDefaultActionMap();
        }
    }
}
