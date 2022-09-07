using UnityEngine;
using UnityEngine.UI;
using gamer;

namespace gamer.maingame.interactable
{
    public class GameMachine : MonoBehaviour, IInteractable
    {
        [SerializeField] Minigame _minigame;
        [SerializeField] RawImage _display;

        void Start()
        {
            _display.texture = _minigame.MinigameTexture;
        }

        public void Interact()
        {
            _minigame.StartMinigame();
        }
    }
}
