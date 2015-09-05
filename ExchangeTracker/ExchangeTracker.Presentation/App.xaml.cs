using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Threading;
using System.Windows;
using DevExpress.Xpf.Core;
using DevExpress.Xpf.Grid;
using ExchangeTracker.Presentation.Common;
using ExchangeTracker.Presentation.Views;
using FarsiLibrary.FX.Utils;

namespace ExchangeTracker.Presentation
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App
    {
        private bool _doHandle = true;

        /// <summary>
        /// Raises the <see cref="E:System.Windows.Application.Startup"/> event.
        /// </summary>
        /// <param name="e">A <see cref="T:System.Windows.StartupEventArgs"/> that contains the event data.</param>
        protected override void OnStartup(StartupEventArgs e)
        {
            CopyDataBase();
            DataViewBase.DisableOptimizedModeVerification = true;
            DXGridDataController.DisableThreadingProblemsDetection = true;
            Thread.CurrentThread.CurrentUICulture =
            Thread.CurrentThread.CurrentCulture = CultureHelper.FarsiCulture;
            Thread.CurrentThread.CurrentCulture.NumberFormat = Thread.CurrentThread.CurrentUICulture.NumberFormat =
                new NumberFormatInfo();
            AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;
        }

        private void CopyDataBase()
        {
            const string outputDir = @"C:\ExchangeTracker";
            const string file = "ExchangeDbCe.sdf";
            if (!File.Exists(Path.Combine(outputDir, file)))
            {
                AppHelper.ExtractEmbededResource(outputDir, "Assets", new List<string> { file });
            }
        }

        void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            var ex = e.ExceptionObject as Exception;
            ExceptionHelper.ReportException(ex, "خطای سیستمی");
        }

        public bool DoHandle
        {
            get { return _doHandle; }
            set { _doHandle = value; }
        }

        private void Application_DispatcherUnhandledException(object sender,
                               System.Windows.Threading.DispatcherUnhandledExceptionEventArgs e)
        {
            e.Handled = DoHandle;
            ExceptionHelper.ReportException(e.Exception, "خطای سیستمی");
            if (!DoHandle)
                MessageBoxHelper.Show("برنامه به دلیل خطای سیستمی بسته خواهد شد! ", "خطای سیستمی");
        }
    }
}
