using System.Collections.Generic;
using UnityEngine;

namespace gamer.pacman
{
    public class PacmanInputSenderMapManager : MonoBehaviour, IInputSenderMapManager
    {
        [SerializeField] InputActionMapReference _pacmanActionMap;
        [SerializeField] Minigame _minigame;

        [SerializeField] Vector2InputSender _pacmanInputSender;

        Queue<IInputSenderMap> _unusedInputMaps = new Queue<IInputSenderMap>();

        void Start()
        {
            var map = new InputSenderMap(_pacmanActionMap.Value.name, _pacmanInputSender);
            _unusedInputMaps.Enqueue(map);

            _minigame.RegisterSenderMap(this);
        }

        void OnDestroy()
        {
            _minigame.UnregisterSenderMap(this);
        }

        public IInputSenderMap ClaimInputSenderMap()
        {
            var inputSenderMap = _unusedInputMaps.Dequeue();
            return inputSenderMap;
        }

        public void ReleaseInputSenderMap(IInputSenderMap map)
        {
            _unusedInputMaps.Enqueue(map);
        }
    }
}