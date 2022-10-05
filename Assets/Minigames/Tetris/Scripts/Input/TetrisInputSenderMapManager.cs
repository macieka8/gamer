using UnityEngine;
using System.Collections.Generic;

namespace gamer.tetris
{
    public class TetrisInputSenderMapManager : MonoBehaviour, IInputSenderMapManager
    {
        [SerializeField] InputActionMapReference _tetrisActionMap;
        [SerializeField] Minigame _minigame;
        [SerializeField] PuzzleMoverInputSenders _puzzleMoverInputSenders;

        Queue<IInputSenderMap> _unusedInputMaps = new Queue<IInputSenderMap>();

        void Start()
        {
            var map = new InputSenderMap(
                _tetrisActionMap.Value.name, _puzzleMoverInputSenders.ToInputSenderDictionary());
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
