using System;
using System.IO;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;
using FarsiLibrary.FX.Utils;

namespace ExchangeTracker.Presentation.Common
{
    public static class ExceptionHelper
    {
        public static void ReportSimpleException(Exception ex)
        {
            string messageBoxText = string.Format("{0} {1} {2}", "Error", Environment.NewLine, GetExceptionMessages(ex));
            //MessageBoxHelper.Show(ex.Message, "هشدار", MessageBoxButton.OK, MessageBoxImage.Warning);
            try
            {
                File.AppendAllText("C:\\ExchangeTracker\\Errors.txt",
                    string.Format("{0} {1}{2}{3}{2}{2}{2}",
                    CultureHelper.GetCurrentDate(),
                    CultureHelper.GetCurrentTime(),
                    Environment.NewLine,
                    messageBoxText));
            }
            catch { }
        }

        public static void ReportException(Exception ex, string headerError)
        {
            string messageBoxText = string.Format("{0} {1} {2}", headerError, Environment.NewLine, GetExceptionMessages(ex));
            //MessageBoxHelper.Show(messageBoxText, "هشدار", MessageBoxButton.OK, MessageBoxImage.Warning);
            try
            {
                File.AppendAllText("C:\\ExchangeTracker\\Errors.txt",
                    string.Format("{0} {1}{2}{3}{2}{2}{2}",
                    CultureHelper.GetCurrentDate(),
                    CultureHelper.GetCurrentTime(),
                    Environment.NewLine,
                    messageBoxText));
            }
            catch { }
        }

        public static string GetExceptionMessages(Exception ex)
        {
            var message = ex.Message;
            while (ex.InnerException != null)
            {
                message += string.Format("{0} InnerException:{0}{1}", Environment.NewLine, ex.InnerException.Message);
                ex = ex.InnerException;
            }
            return message;
        }
        public static void DoAction(Action action)
        {
            try
            {
                action();
            }
            catch (Exception e)
            {
                ReportSimpleException(e);
            }
        }

    }
}
