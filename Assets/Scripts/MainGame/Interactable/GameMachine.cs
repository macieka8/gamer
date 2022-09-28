using UnityEngine;

namespace gamer.maingame.interactable
{
    public enum GameMachineState
    {
        Off,
        On
    }

    public class GameMachine : MonoBehaviour, IInteractable
    {
        [SerializeField] Minigame _minigame;
        [SerializeField] Renderer _display;
        [SerializeField] GameObject _playerOnFocusedCamera;

        GameMachineState _state = GameMachineState.Off;

        public GameMachineState State => _state;
        public Minigame Minigame => _minigame;
        public GameObject PlayerOnFocusedCamera => _playerOnFocusedCamera;

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
        }

        void HandleMachineTurnedOff()
        {
            _state = GameMachineState.Off;
        }
    }
}
