using UnityEngine;

namespace gamer
{
    public class PlayerInputMapController : MonoBehaviour, IInputMapController
    {
        [SerializeField] PlayerInputController _playerInputController;
        [SerializeField] GamerInputBinder[] _binders;

        public void RestoreDefaultGamerState()
        {
            _playerInputController.RestoreDefaultActionMap();
            foreach (var binder in _binders)
            {
                binder.Unbind();
            }
        }

        public void SetActiveGamerState(IInputSenderMap inputSenderMap)
        {
            _playerInputController.SetActiveActionMap(inputSenderMap.GetActionMapName, true);
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
