using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GalaSoft.MvvmLight.Ioc;
using PersianCalendar = FarsiLibrary.FX.Utils.PersianCalendar;

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

        public static void ExtractEmbededResource(string outputDir, string resourceLocation, List<string> files)
        {
            foreach (var file in files)
            {
                var executingAssembly = System.Reflection.Assembly.GetExecutingAssembly();
                using (var stream = executingAssembly.GetManifestResourceStream(executingAssembly.GetName().Name + "." + resourceLocation + "." + file))
                {
                    using (var fileStream = new FileStream(Path.Combine(outputDir, file), FileMode.Create))
                    {
                        for (var i = 0; i < stream.Length; i++)
                        {
                            fileStream.WriteByte((byte)stream.ReadByte());
                        }
                        fileStream.Close();
                    }
                }
            }
        }
        public static DateTime ConvertPersianToEnglish(string persianDate)
        {
            string[] formats = { "yyyy/MM/dd", "yyyy/M/d", "yyyy/MM/d", "yyyy/M/dd" };
            var d1 = DateTime.ParseExact(persianDate, formats,
                                              CultureInfo.CurrentCulture, DateTimeStyles.None);
            var persian_date = new PersianCalendar();
            DateTime dt = persian_date.ToDateTime(d1.Year, d1.Month, d1.Day, 0, 0, 0, 0, 0);
            return dt;
        }

    }
}
