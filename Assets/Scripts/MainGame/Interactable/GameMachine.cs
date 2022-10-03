using UnityEngine;

namespace gamer.maingame.interactable
{
    public enum GameMachineState
    {
        Off,
        On
    }

    public interface IGameMachine : IInteractable
    {
        public GameMachineState State { get; }
        public Minigame Minigame { get; }
        public GameObject PlayerOnFocusedCamera { get; }

        public bool TryConnectGamer(out IInputSenderMap inputSenderMap);
        public void DisconnectGamer(IInputSenderMap inputSenderMap);
    }

    public class GameMachine : MonoBehaviour, IGameMachine
    {
        [SerializeField] Minigame _minigame;
        [SerializeField] Renderer _display;
        [SerializeField] GameObject _playerOnFocusedCamera;

        GameMachineState _state = GameMachineState.Off;
        int _currentGamersCount = 0;

        public GameMachineState State => _state;
        public Minigame Minigame => _minigame;
        public GameObject PlayerOnFocusedCamera => _playerOnFocusedCamera;

        void Start()
        {
            _display.material = _minigame.MinigameMaterial;
            _minigame.OnMinigameStopped += HandleMachineTurnedOff;
        }

        void OnDestroy()
        {
            _minigame.OnMinigameStopped -= HandleMachineTurnedOff;
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

        public bool TryConnectGamer(out IInputSenderMap inputSenderMap)
        {
            inputSenderMap = null;
            if (_currentGamersCount < _minigame.MaxPlayerCount)
            {
                _currentGamersCount++;
                inputSenderMap = _minigame.InputSenderMapManager.ClaimInputSenderMap();
                return true;
            }
            return false;
        }

        public void DisconnectGamer(IInputSenderMap inputSenderMap)
        {
            _minigame.InputSenderMapManager.ReleaseInputSenderMap(inputSenderMap);
            _currentGamersCount--;
        }
    }
}
