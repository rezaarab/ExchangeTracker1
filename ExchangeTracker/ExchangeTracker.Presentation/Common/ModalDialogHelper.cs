using System.Collections.Generic;
using System.Linq;
using System.Windows;

namespace ExchangeTracker.Presentation.Common
{
    public static class ModalDialogHelper
    {
        static readonly Stack<ModalDialog> stack = new Stack<ModalDialog>();
        public static void Show(MenuCommandObject menuCommandObject, bool isMaximaize = false)
        {
            var dialog = new ModalDialog
            {
                Content = menuCommandObject,
                ShowCloseButton = true,
                ShowInTaskbar = true,
                ShowIconOnTitleBar = false,
                ShowMinButton = false,
                ShowMaxRestoreButton = false,
                WindowStartupLocation = WindowStartupLocation.CenterScreen,
                WindowState = isMaximaize ? WindowState.Maximized : WindowState.Normal,
                ResizeMode = ResizeMode.CanResize,
                Owner = stack.FirstOrDefault(p => p.IsActive) ?? Application.Current.MainWindow,
            };
            menuCommandObject.Navigator.NavigateEnter();
            stack.Push(dialog);
            dialog.ShowDialog();
        }
        public static void Close()
        {
            var dialog = stack.Pop();
            if (dialog != null)
            {
                if ((dialog.Content as MenuCommandObject) != null && (dialog.Content as MenuCommandObject).Navigator.NavigateExit())
                    dialog.Close();
            }
        }
    }
}
