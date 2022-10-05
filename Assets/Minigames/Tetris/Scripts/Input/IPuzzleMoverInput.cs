using System;

namespace gamer.tetris
{
    public interface IPuzzleMoverInput
    {
        public event IInputReceiver<float>.OnInputAction OnMovementInput;
        public event IInputReceiver.OnInputAction OnRotationInput;
        public event IInputReceiver<bool>.OnInputAction OnSoftDropInput;
        public event IInputReceiver.OnInputAction OnHardDropInput;
        public event IInputReceiver.OnInputAction OnSavePuzzleInput;
    }
}
