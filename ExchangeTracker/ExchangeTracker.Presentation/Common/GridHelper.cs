using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using DevExpress.Xpf.Grid;
using MahApps.Metro.Controls;

namespace ExchangeTracker.Presentation.Common
{
    public static class GridHelper
    {
        public static bool RestoreLayout(GridControl dataGrid)
        {
            try
            {
                var layoutFilePath = GetLayoutFilePath(dataGrid);
                if (File.Exists(layoutFilePath))
                {
                    dataGrid.RestoreLayoutFromXml(layoutFilePath);
                    return true;
                }
                return true;
            }
            catch { return false; }
        }

        public static bool SaveLayout(GridControl dataGrid)
        {
            try
            {
                var layoutFilePath = GetLayoutFilePath(dataGrid);
                dataGrid.SaveLayoutToXml(layoutFilePath);
                return true;
            }
            catch { return false; }

        }

        private static string GetLayoutFilePath(FrameworkElement control)
        {
            return Path.Combine(AppHelper.AppDataPath,
                string.Format("{0}_{1}{2}.xml", control.TryFindParent<UserControlBase>().Name, control.Name, AppHelper.AppLayoutVersion));
        }
    }
}
