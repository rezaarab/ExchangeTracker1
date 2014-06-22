using System.Windows;

namespace ExchangeTracker.Presentation.Common
{
    /// <summary>
    /// Interaction logic for LayoutItem.xaml
    /// </summary>
    public partial class LayoutItem
    {
        public LayoutItem()
        {
            InitializeComponent();
        }

        public static readonly DependencyProperty LabelProperty =
            DependencyProperty.Register("Label", typeof(string), typeof(LayoutItem), new PropertyMetadata(default(string)));

        public string Label
        {
            get { return (string)GetValue(LabelProperty); }
            set { SetValue(LabelProperty, value); }
        }
    }
}
