using System.Windows;

namespace ExchangeTracker.Presentation.Common
{
    public class MessageBoxHelper
    {
        public static MessageBoxResult Show(string message, string caption = "", MessageBoxButton button = MessageBoxButton.OK, MessageBoxImage icon = MessageBoxImage.Information)
        {
            return MessageBox.Show(message, caption, button, icon);
        }
    }
}
