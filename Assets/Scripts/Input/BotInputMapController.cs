using UnityEngine;

namespace gamer
{
    public class BotInputMapController : MonoBehaviour, IInputMapController
    {
        [SerializeField] GamerInputBinder[] _binders;

        public void RestoreDefaultGamerState()
        {
            foreach (var binder in _binders)
            {
                binder.Unbind();
            }
        }

        public void SetActiveGamerState(IInputSenderMap inputSenderMap)
        {
            foreach (var binder in _binders)
            {
                if (binder.ActionMapName == inputSenderMap.GetActionMapName)
                {
                    binder.Bind(inputSenderMap);
                    break;
                }
            }
        }
    }
}
