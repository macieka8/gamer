using UnityEngine;

namespace gamer.maingame.interactable
{
    public interface IGamerInput
    {
        public void SetActiveGamerState(string gameName);
        public void RestoreDefaultGamerState();
    }
}
