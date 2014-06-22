using System.Windows.Input;

namespace ExchangeTracker.Presentation.Common
{
    public class CommandObject
    {
        public CommandObject(ICommand command, string key)
        {
            this.Command = command;
            this.Key = key;
        }
        public ICommand Command { get; set; }
        public string Key { get; set; }
    }
}
