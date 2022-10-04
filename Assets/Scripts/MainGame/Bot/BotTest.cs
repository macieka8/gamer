using System;
using UnityEngine;
using gamer.maingame.interactable;
using gamer.maingame.movement;
using System.Collections;

namespace gamer.maingame.bot
{
    public class BotTest : MonoBehaviour, IInteractorInput
    {
        IInteractableTracer _interactableTracer;
        BotMovement _botMovement;

        bool interacted;

        public event Action OnMinigameQuitInput;
        public event Action OnInteractInput;

        void Awake()
        {
            _botMovement = GetComponent<BotMovement>();
            _interactableTracer = GetComponent<IInteractableTracer>();
        }

        void Update()
        {
            if (!interacted && _interactableTracer.TryGetInteractable(out var interactable))
            {
                _botMovement.SetDestination(interactable.Transform.position + (interactable.Transform.forward * 2f));
                if (_botMovement.RemainingDistance < 0.1f)
                {
                    StartCoroutine(TestGaming(interactable));
                }
            }
        }

        IEnumerator TestGaming(IInteractable interactable)
        {
            interacted = true;
            yield return new WaitForSeconds(0.5f);
            var gameMachine = interactable as IGameMachine;
            if (gameMachine.State == GameMachineState.Off)
                OnInteractInput?.Invoke();
            yield return new WaitForSeconds(0.5f);
            OnInteractInput?.Invoke();
        }
    }
}
