using Unity.Mathematics;
using UnityEngine;

namespace gamer.tetris
{
    public class UpdateSystem
    {
        public event System.Action OnUpdate;

        public void Update()
        {
            OnUpdate?.Invoke();
        }
    }
}
