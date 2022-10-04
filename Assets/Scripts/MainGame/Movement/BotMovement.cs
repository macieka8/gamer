using UnityEngine;
using UnityEngine.AI;

namespace gamer.maingame.movement
{
    public class BotMovement : MonoBehaviour
    {
        NavMeshAgent _agent;
        public float RemainingDistance => _agent.remainingDistance;

        void Awake()
        {
            _agent = GetComponent<NavMeshAgent>();
        }

        public bool SetDestination(Vector3 destination)
        {
            return _agent.SetDestination(destination);
        }
        
    }
}
