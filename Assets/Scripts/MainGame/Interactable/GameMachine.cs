using UnityEngine;
using gamer;

namespace gamer.maingame.interactable
{
    public enum GameMachineState
    {
        Off,
        On,
    }

    public class GameMachine : MonoBehaviour, IInteractable
    {
        [SerializeField] PlayerInputController _input;
        [SerializeField] Minigame _minigame;
        [SerializeField] Renderer _display;

        GameMachineState _state = GameMachineState.Off;

        void Start()
        {
            _display.material = _minigame.MinigameMaterial;
            _minigame.onMinigameStopped += HandleMachineTurnedOff;
        }

        void OnDestroy()
        {
            _minigame.onMinigameStopped -= HandleMachineTurnedOff;
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
                _input.SetActiveActionMap(_minigame.InputActionMapName);
            }
        }

        void HandleMachineTurnedOff()
        {
            _state = GameMachineState.Off;
        }
    }
}
