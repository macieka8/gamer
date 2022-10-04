using UnityEngine;

namespace gamer.maingame.interactable
{

    public interface IGameMachine : IInteractable
    {
        public GameMachineState State { get; }
        public Minigame Minigame { get; }
        public GameObject PlayerOnFocusedCamera { get; }

        public bool TryConnectGamer(out IInputSenderMap inputSenderMap);
        public void DisconnectGamer(IInputSenderMap inputSenderMap);
    }
}
