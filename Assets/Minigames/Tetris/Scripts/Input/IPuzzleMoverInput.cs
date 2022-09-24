using System;

namespace gamer.tetris
{
    public interface IPuzzleMoverInput
    {
        public event Action<float> OnMovementInput;
        public event Action<bool> OnSoftDropInput;
        public event Action OnHardDropInput;
        public event Action OnRotationInput;
    }
}
