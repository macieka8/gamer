using UnityEngine;
using UnityEngine.InputSystem;

namespace gamer.pong
{
    public class BotPongGamerInputBinder : GamerInputBinder
    {
        [SerializeField] InputActionMapReference _pongActionMap;
        [SerializeField] InputActionReference _moveInputAction;

        FloatInputSender _moveInputSender;
        ContactFilter2D _filter = new ContactFilter2D().NoFilter();
        BallComponent _ball;

        public override string ActionMapName => _pongActionMap.Value.name;

        void Update()
        {
            if (_moveInputSender != null)
            {
                if (_ball == null)
                {
                    var results = new Collider2D[20];
                    var resultCount = Physics2D.OverlapCircle(_moveInputSender.transform.position, 20f, _filter, results);
                    for (int i = 0; i < resultCount; i++)
                    {
                        if (results[i].TryGetComponent<BallComponent>(out var foundBall))
                        {
                            _ball = foundBall;
                            break;
                        }
                    }
                }
                if (_ball == null) return;
                var distanceToBall = _ball.transform.position.y - _moveInputSender.transform.position.y;
                if (distanceToBall > 0f)
                {
                    _moveInputSender.SendInput(1f);
                }
                else if (distanceToBall < 0f)
                {
                    _moveInputSender.SendInput(-1f);
                }
            }
        }

        public override void Bind(IInputSenderMap inputSenderMap)
        {
            if (_pongActionMap.Value.name != inputSenderMap.GetActionMapName) return;
            foreach (var entry in inputSenderMap.Map)
            {
                var inputSender = entry.Value;
                if (_moveInputAction.action.name == entry.Key)
                {
                    _moveInputSender = inputSender as FloatInputSender;
                }
            }
        }

        public override void Unbind()
        {
            _moveInputSender = null;
        }
    }
}
