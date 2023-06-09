using System;
using System.Windows;

namespace ExifHelper.DesktopUi
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private Func<Type, IExifPage> _exifPages;

        public MainWindow(Func<Type, IExifPage> exifPages)
        {
            _exifPages = exifPages;

            InitializeComponent();
        }

        private void CopyPageLink_Click(object sender, RoutedEventArgs e)
        {
            PageFrame.Navigate(_exifPages(typeof(CopyExifDataPage)));
        }

        private void ReadExifTagsLink_Click(object sender, RoutedEventArgs e)
        {
            PageFrame.Navigate(_exifPages(typeof(ReadExifInformationPage)));
        }
    }
}
