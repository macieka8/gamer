using UnityEngine;
using gamer;
using UnityEngine.InputSystem;

namespace gamer.maingame.interactable
{
    public enum GameMachineState
    {
        Off,
        On,
        Focused
    }

    public class GameMachine : MonoBehaviour, IInteractable
    {
        [SerializeField] InputActionReference _quitMinigameInputAction;
        [SerializeField] PlayerInputController _input;
        [SerializeField] Minigame _minigame;
        [SerializeField] Renderer _display;
        [SerializeField] GameObject _onFocusedCamera;

        GameMachineState _state = GameMachineState.Off;

        void Start()
        {
            _display.material = _minigame.MinigameMaterial;
            _minigame.onMinigameStopped += HandleMachineTurnedOff;
            _quitMinigameInputAction.action.performed += HandleMinigameUnfocus;
        }

        void OnDestroy()
        {
            _minigame.onMinigameStopped -= HandleMachineTurnedOff;
            _quitMinigameInputAction.action.performed -= HandleMinigameUnfocus;
        }

        public void Interact()
        {
            if (_state == GameMachineState.Off)
            {
                _minigame.StartMinigame();
                _state = GameMachineState.On;
            }
            else if (_state == GameMachineState.On)
            {
                _state = GameMachineState.Focused;
                _onFocusedCamera.SetActive(true);
                _input.SetActiveActionMap(_minigame.InputActionMapName);
            }
        }

        void HandleMachineTurnedOff()
        {
            _state = GameMachineState.Off;
        }

        void HandleMinigameUnfocus(InputAction.CallbackContext ctx)
        {
            if (_state == GameMachineState.Focused)
            {
                _input.RestoreDefaultActionMap();
                _state = GameMachineState.On;
                _onFocusedCamera.SetActive(false);
            }
        }
    }
}
