using System.Collections.Generic;

namespace gamer
{
    public class InputSenderMap : IInputSenderMap
    {
        Dictionary<string, IInputSender> _inputSenders;
        string _actionMapName;

        public string GetActionMapName => _actionMapName;

        public IReadOnlyDictionary<string, IInputSender> Map => _inputSenders;

        public InputSenderMap(string actionMapName, Dictionary<string, IInputSender> inputSenders)
        {
            _actionMapName = actionMapName;
            _inputSenders = inputSenders;
        }

        public InputSenderMap(string actionMapName, params IInputSender[] inputSenders)
        {
            _actionMapName = actionMapName;
            _inputSenders = new Dictionary<string, IInputSender>();
            foreach (var inputSender in inputSenders)
            {
                _inputSenders.Add(inputSender.InputName, inputSender);
            }
        }

        public IInputSender GetInputSender(string senderName)
        {
            if (_inputSenders.TryGetValue(senderName, out var foundInputSender))
            {
                return foundInputSender;
            }
            return null;
        }
    }
}
