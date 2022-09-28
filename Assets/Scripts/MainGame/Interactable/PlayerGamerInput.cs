using UnityEngine;

namespace gamer.maingame.interactable
{
    public class PlayerGamerInput : MonoBehaviour, IGamerInput
    {
        [SerializeField] PlayerInputController _playerInputController;

        public void RestoreDefaultGamerState()
        {
            _playerInputController.RestoreDefaultActionMap();
        }

        public void SetActiveGamerState(string gameName)
        {
            _playerInputController.SetActiveActionMap(gameName, true);
        }
    }
}
