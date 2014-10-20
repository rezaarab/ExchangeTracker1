using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GalaSoft.MvvmLight.Ioc;

namespace ExchangeTracker.Presentation.Common
{
    public static class AppHelper
    {
        static AppHelper()
        {
            if (!Directory.Exists(AppDataPath))
                Directory.CreateDirectory(AppDataPath);
        }
        public static string AppDataPath = "C:\\ExchangeTracker\\";
        public static string AppLayoutVersion = "1";
        public static string AppStaticDataVersion = "1";

        public static MainWindow MainWindow
        {
            get { return SimpleIoc.Default.GetInstance<MainWindow>(); }
        }
    }
}
