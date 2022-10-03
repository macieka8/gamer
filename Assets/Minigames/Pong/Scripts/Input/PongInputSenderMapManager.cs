using System.Collections.Generic;
using UnityEngine;

namespace gamer.pong
{
    public class PongInputSenderMapManager : MonoBehaviour, IInputSenderMapManager
    {
        [SerializeField] InputActionMapReference _pongActionMap;
        [SerializeField] Minigame _minigame;
        [SerializeField] FloatInputSender _leftPaddle;
        [SerializeField] FloatInputSender _rightPaddle;

        Queue<IInputSenderMap> _unusedInputMaps = new Queue<IInputSenderMap>();

        void Start()
        {
            _unusedInputMaps.Enqueue(new InputSenderMap(_pongActionMap.Value.name, _leftPaddle));
            _unusedInputMaps.Enqueue(new InputSenderMap(_pongActionMap.Value.name, _rightPaddle));

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
