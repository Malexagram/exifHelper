using ExifHelper.Application.Command;
using ExifHelper.Application.Configuration;
using System;
using System.Diagnostics;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;

namespace ExifHelper.DesktopUi
{
    /// <summary>
    /// Interaction logic for CopyExifDataPage.xaml
    /// </summary>
    public partial class CopyExifDataPage : Page, IExifPage
    {
        BitmapImage copyToImage = null;
        private readonly IConfigurationManager _configurationManager;
        private readonly Func<object, object, IExifCommand> _exifCommands;

        public CopyExifDataPage(IConfigurationManager configurationManager, Func<object, object, IExifCommand> exifCommands)
        {
            _configurationManager = configurationManager;
            _exifCommands = exifCommands;

            InitializeComponent();

            SetExifButton();
        }

        private void CopyFromImage_Drop(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);

                var copyFromFilePath = files[0];
                CopyFromFilePath.Text = copyFromFilePath;
                var copyFromFilePathUri = new Uri(copyFromFilePath);
                CopyFromImage.Source = new BitmapImage(copyFromFilePathUri);
            }
        }

        private void CopyToImage_Drop(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);

                CopyToImage.Source = null;
                copyToImage = null;
                ExifHelperUtility.ClearCopiedImages();

                var copyToFilePath = files[0];
                var copyToCopyPath = System.IO.Path.Combine(_configurationManager.CopyImagePath, System.IO.Path.GetFileName(copyToFilePath));
                if (!System.IO.File.Exists(copyToCopyPath))
                    System.IO.File.Copy(copyToFilePath, copyToCopyPath);

                CopyToFilePath.Text = copyToFilePath;

                using (var stream = File.OpenRead(copyToCopyPath))
                {
                    copyToImage = new BitmapImage();
                    copyToImage.BeginInit();
                    copyToImage.StreamSource = stream;
                    copyToImage.CacheOption = BitmapCacheOption.OnLoad;
                    copyToImage.EndInit();
                    CopyToImage.Source = copyToImage;
                }

            }
        }

        private void CopyFromFileSelector_Click(object sender, RoutedEventArgs e)
        {
            var fileDialog = new Microsoft.Win32.OpenFileDialog();

            bool? result = fileDialog.ShowDialog();

            if (result == true)
            {
                var copyFromFilePath = fileDialog.FileName;
                CopyFromFilePath.Text = copyFromFilePath;
                var copyFromFilePathUri = new Uri(copyFromFilePath);
                CopyFromImage.Source = new BitmapImage(copyFromFilePathUri);
            }
        }

        private void CopyToFileSelector_Click(object sender, RoutedEventArgs e)
        {
            var fileDialog = new Microsoft.Win32.OpenFileDialog();

            bool? result = fileDialog.ShowDialog();

            if (result == true)
            {
                CopyToImage.Source = null;
                ExifHelperUtility.ClearCopiedImages();

                var copyToFilePath = fileDialog.FileName;
                var copyToCopyPath = System.IO.Path.Combine(_configurationManager.CopyImagePath, System.IO.Path.GetFileName(copyToFilePath));
                System.IO.File.Copy(copyToFilePath, copyToCopyPath);

                CopyToFilePath.Text = copyToFilePath;
                var copyToFilePathUri = new Uri(copyToCopyPath);
                CopyToImage.Source = new BitmapImage(copyToFilePathUri);
            }
        }

        private void GetExifFilePathButton_Click(object sender, RoutedEventArgs e)
        {
            var fileDialog = new Microsoft.Win32.OpenFileDialog();

            bool? result = fileDialog.ShowDialog();

            if (result == true)
            {
                _configurationManager.ExifApplicationPath = fileDialog.FileName;
                GetExifFilePathButton.Content = "EXIF SET - CLICK TO RESET";
            }
        }

        private void CopyExifTags_Click(object sender, RoutedEventArgs e)
        {
            if (_configurationManager.ExifApplicationPath == null || CopyToFilePath.Text == null || CopyFromFilePath.Text == null) return;

            var startInfo = new ProcessStartInfo();
            var pathFrom = System.IO.Path.GetFullPath(CopyFromFilePath.Text);
            var pathTo = System.IO.Path.GetFullPath(CopyToFilePath.Text);
            var exifDrive = System.IO.Path.GetPathRoot(_configurationManager.ExifApplicationPath);
            var exifDirectory = System.IO.Path.GetDirectoryName(_configurationManager.ExifApplicationPath);
            var exifFileName = System.IO.Path.GetFileNameWithoutExtension(_configurationManager.ExifApplicationPath);

            startInfo.CreateNoWindow = false;
            startInfo.FileName = "cmd.exe";
            startInfo.WorkingDirectory = exifDirectory;
            startInfo.WindowStyle = ProcessWindowStyle.Normal;
            startInfo.Arguments = $"/C {exifFileName} -tagsFromFile {pathFrom} {pathTo}";
            var cmdProcess = new Process();
            cmdProcess.StartInfo = startInfo;
            cmdProcess.Start();
        }

        private void SetExifButton()
        {
            if (!string.IsNullOrEmpty(_configurationManager.ExifApplicationPath))
                GetExifFilePathButton.Content = "EXIF SET - CLICK TO RESET";
        }
    }
}
