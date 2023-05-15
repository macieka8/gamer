using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections.Generic;

namespace gamer.tetris
{
    public class PuzzleMoverInputSenders : MonoBehaviour, IPuzzleMoverInput
    {
        [SerializeField] InputActionReference _moveInputAction;
        [SerializeField] InputActionReference _rotateInputAction;
        [SerializeField] InputActionReference _softDropInputAction;
        [SerializeField] InputActionReference _hardDropInputAction;
        [SerializeField] InputActionReference _savePuzzleInputAction;

        InputSenderReceiver<float> _moveSender;
        InputSenderReceiver _rotateSender;
        InputSenderReceiver<bool> _softDropSender;
        InputSenderReceiver _hardDropSender;
        InputSenderReceiver _savePuzzleSender;

        bool _isInitialized;

        public event IInputReceiver<float>.OnInputAction OnMovementInput
        {
            add => _moveSender.OnInput += value;
            remove => _moveSender.OnInput -= value;
        }

        public event IInputReceiver.OnInputAction OnRotationInput
        {
            add => _rotateSender.OnInput += value;
            remove => _rotateSender.OnInput -= value;
        }
        public event IInputReceiver<bool>.OnInputAction OnSoftDropInput
        {
            add => _softDropSender.OnInput += value;
            remove => _softDropSender.OnInput -= value;
        }
        public event IInputReceiver.OnInputAction OnHardDropInput
        {
            add => _hardDropSender.OnInput += value;
            remove => _hardDropSender.OnInput -= value;
        }
        public event IInputReceiver.OnInputAction OnSavePuzzleInput
        {
            add => _savePuzzleSender.OnInput += value;
            remove => _savePuzzleSender.OnInput -= value;
        }

        void Awake()
        {
            if (!_isInitialized)
            {
                InitSenders();
            }
        }

        void InitSenders()
        {
            _isInitialized = true;
            _moveSender = new InputSenderReceiver<float>(_moveInputAction);
            _rotateSender = new InputSenderReceiver(_rotateInputAction);
            _softDropSender = new InputSenderReceiver<bool>(_softDropInputAction);
            _hardDropSender = new InputSenderReceiver(_hardDropInputAction);
            _savePuzzleSender = new InputSenderReceiver(_savePuzzleInputAction);
        }

        public Dictionary<string, IInputSender> ToInputSenderDictionary()
        {
            if (!_isInitialized) InitSenders();

            var sendersDict = new Dictionary<string, IInputSender>
            {
                { _moveSender.InputName, _moveSender },
                { _rotateSender.InputName, _rotateSender },
                { _softDropSender.InputName, _softDropSender },
                { _hardDropSender.InputName, _hardDropSender },
                { _savePuzzleSender.InputName, _savePuzzleSender }
            };

            return sendersDict;
        }
    }
}
