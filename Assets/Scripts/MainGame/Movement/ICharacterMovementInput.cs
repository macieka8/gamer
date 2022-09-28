using UnityEngine;

namespace gamer.maingame.movement
{
    public interface ICharacterMovementInput
    {
        public event System.Action<Vector2> OnMovementInput;
        public event System.Action OnJumpInput;
    }
}
