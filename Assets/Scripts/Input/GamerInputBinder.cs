using UnityEngine;

namespace gamer
{
    public abstract class GamerInputBinder : MonoBehaviour
    {
        public abstract string ActionMapName { get; }
        public abstract void Bind(IInputSenderMap inputSenderMap);
        public abstract void Unbind();
    }
}
