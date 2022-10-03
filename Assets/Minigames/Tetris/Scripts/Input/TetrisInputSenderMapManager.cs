using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections.Generic;

namespace gamer.tetris
{
    public class TetrisInputSenderMapManager : MonoBehaviour, IInputSenderMapManager
    {
        [SerializeField] InputActionMapReference _tetrisActionMap;
        [SerializeField] Minigame _minigame;
        [SerializeField] FloatInputSender _moveSender;
        [SerializeField] EmptyInputSender _rotateSender;
        [SerializeField] BoolInputSender _softDropSender;
        [SerializeField] EmptyInputSender _hardDropSender;
        [SerializeField] EmptyInputSender _savePuzzleSender;

        Queue<IInputSenderMap> _unusedInputMaps = new Queue<IInputSenderMap>();

        void Start()
        {
            var sendersDict = new Dictionary<string, IInputSender>();
            sendersDict.Add(_moveSender.InputName, _moveSender);
            sendersDict.Add(_rotateSender.InputName, _rotateSender);
            sendersDict.Add(_softDropSender.InputName, _softDropSender);
            sendersDict.Add(_hardDropSender.InputName, _hardDropSender);
            sendersDict.Add(_savePuzzleSender.InputName, _savePuzzleSender);

            _unusedInputMaps.Enqueue(new InputSenderMap(_tetrisActionMap.Value.name, sendersDict));

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
