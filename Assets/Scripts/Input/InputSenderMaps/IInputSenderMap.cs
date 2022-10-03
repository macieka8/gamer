using System.Collections.Generic;

namespace gamer
{
    public interface IInputSenderMap
    {
        public string GetActionMapName { get; }
        public IReadOnlyDictionary<string, IInputSender> Map { get; }
        public IInputSender GetInputSender(string senderName);
    }
}
