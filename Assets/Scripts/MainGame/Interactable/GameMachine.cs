using System.Collections.Generic;
using UnityEngine;

namespace gamer.maingame.interactable
{
    public enum GameMachineState
    {
        Off,
        On
    }

    public class GameMachine : MonoBehaviour, IGameMachine
    {
        class GamerToInputEntry
        {
            public Gamer Gamer;
            public IInputSenderMap Map;
        }

        [SerializeField] Minigame _minigame;
        [SerializeField] Renderer _display;
        [SerializeField] GameObject _playerOnFocusedCamera;
        
        Transform _transform;
        GameMachineState _state = GameMachineState.Off;
        List<GamerToInputEntry> _connectedGamers = new List<GamerToInputEntry>();
        Material _turnedOffMaterial;

        public GameMachineState State => _state;
        public Minigame Minigame => _minigame;
        public GameObject PlayerOnFocusedCamera => _playerOnFocusedCamera;

        public Transform Transform => _transform;

        void Awake()
        {
            _transform = transform;
            _turnedOffMaterial = _display.material;
        }

        void Start()
        {
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
                _display.material = _minigame.MinigameMaterial;
                _minigame.StartMinigame();
                _state = GameMachineState.On;
            }
        }

        void HandleMachineTurnedOff()
        {
            _state = GameMachineState.Off;
            for (int i = _connectedGamers.Count - 1; i >= 0; i--)
            {
                DisconnectGamer(_connectedGamers[i].Gamer);
            }

            _display.material = _turnedOffMaterial;
        }

        public bool TryConnectGamer(Gamer gamer, out IInputSenderMap inputSenderMap)
        {
            inputSenderMap = null;
            if (_connectedGamers.Count < _minigame.MaxPlayerCount && _minigame.InputSenderMapManager != null)
            {
                inputSenderMap = _minigame.InputSenderMapManager.ClaimInputSenderMap();
                _connectedGamers.Add(new GamerToInputEntry {Gamer = gamer, Map = inputSenderMap});
                return true;
            }
            return false;
        }

        public void DisconnectGamer(Gamer gamer)
        {
            var entry = _connectedGamers.Find(entry => entry.Gamer == gamer);
            if (entry == null) return;
            _minigame.InputSenderMapManager.ReleaseInputSenderMap(entry.Map);
            _connectedGamers.Remove(entry);

            gamer.UnfocusInteracable();
        }
    }
}
