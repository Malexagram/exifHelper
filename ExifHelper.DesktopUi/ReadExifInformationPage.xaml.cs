using ExifHelper.Application.Command;
using ExifHelper.Application.ExifTags;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace ExifHelper.DesktopUi
{
    /// <summary>
    /// Interaction logic for ReadExifInformationPage.xaml
    /// </summary>
    public partial class ReadExifInformationPage : Page, IExifPage
    {
        private readonly Func<object, object, IExifCommand> _exifCommands;

        private List<string> _fileNames;
        private List<string> _tagKeys;
        private Dictionary<string, bool> _tagCheckedState;
        private ExifTagsGetCommandResponse _exifTagResponse = new ExifTagsGetCommandResponse();
        private const string CheckboxPrefix = "ExifTagCheckbox";

        public ReadExifInformationPage(Func<object, object, IExifCommand> exifCommands)
        {
            _fileNames = new();
            _exifCommands = exifCommands;
            _tagCheckedState = new Dictionary<string, bool>();
            InitializeComponent();
        }

        private void GetExifDataImagePanel_Drop(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);
                _fileNames = files.ToList();

                using (var stream = File.OpenRead(_fileNames[0]))
                {
                    var exifTagDataImage = new BitmapImage();
                    exifTagDataImage.BeginInit();
                    exifTagDataImage.StreamSource = stream;
                    exifTagDataImage.CacheOption = BitmapCacheOption.OnLoad;
                    exifTagDataImage.EndInit();
                    GetExifDataImage.Source = exifTagDataImage;
                }
            }
        }

        private void ImageFileSelecter_Click(object sender, EventArgs e)
        {
            var fileDialog = new Microsoft.Win32.OpenFileDialog();

            bool? result = fileDialog.ShowDialog();

            if (result == true)
            {
                var getExifDataImageFilePath = fileDialog.FileName;
                ExifImageFilePath.Text = getExifDataImageFilePath;
                var getExifDataImageFilePathUri = new Uri(getExifDataImageFilePath);
                GetExifDataImage.Source = new BitmapImage(getExifDataImageFilePathUri);
            }
        }

        private void GetExifData_Click(object sender, RoutedEventArgs e)
        {
            ExifTagsGetCommandRequest request = new ExifTagsGetCommandRequest
            {
                FileNames = _fileNames
            };
            IExifCommand exifCommand = _exifCommands(_exifTagResponse, request);
            _exifTagResponse = exifCommand.PerformCommand(request) as ExifTagsGetCommandResponse 
                ?? new ExifTagsGetCommandResponse();

            LoadExifTags(_exifTagResponse);
        }

        private void ExifTagCheckbox_Clicked(object sender, RoutedEventArgs e)
        {
            var clickedCheckbox = sender as CheckBox;
            if (clickedCheckbox == null)
                throw new InvalidOperationException();

            var tagIndexString = clickedCheckbox.Name.Replace(CheckboxPrefix, string.Empty);

            if (!int.TryParse(tagIndexString, out var tagIndex)) return;

            var tagKey = _tagKeys[tagIndex];
            var checkedState = clickedCheckbox.IsChecked ?? default;
            
            if (_tagCheckedState.ContainsKey(tagKey))
            {
                _tagCheckedState[tagKey] = checkedState;
                return;
            }
            
            _tagCheckedState.Add(tagKey, checkedState);
        }

        private void LoadExifTags(ExifTagsGetCommandResponse? response)
        {
            if (!(response?.ExifTagFiles?.Any() ?? false)) return;
            var currentRowCount = 0;
            _tagKeys = new List<string>();

            foreach (var item in response.ExifTagFiles[0].ExifTags)
            {
                var rowDefinition = new RowDefinition() { Height = new GridLength(20) };
                ExifTagGrid.RowDefinitions.Add(rowDefinition);
                var checkbox = new CheckBox();
                checkbox.Margin = new Thickness(0, 0, 0, 0);
                checkbox.Padding = new Thickness(2, 2, 2, 2);
                checkbox.Name = CheckboxPrefix + currentRowCount;
                _tagKeys.Add(item.Key);
                checkbox.Click += ExifTagCheckbox_Clicked;

                var tagKeyLabel = new Label();
                tagKeyLabel.Content = item.Key;
                tagKeyLabel.Margin = new Thickness(0, 0, 0, 0);
                tagKeyLabel.Padding = new Thickness(2, 2, 2, 2);

                var valueLabel = new Label();
                valueLabel.Content = item.Value;
                valueLabel.Margin = new Thickness(0, 0, 0, 0);
                valueLabel.Padding = new Thickness(2, 2, 2, 2);

                Grid.SetRow(checkbox, currentRowCount);
                Grid.SetColumn(checkbox, 0);
                ExifTagGrid.Children.Add(checkbox);

                Grid.SetRow(tagKeyLabel, currentRowCount);
                Grid.SetColumn(tagKeyLabel, 1);
                ExifTagGrid.Children.Add(tagKeyLabel);

                Grid.SetRow(valueLabel, currentRowCount);
                Grid.SetColumn(valueLabel, 2);
                ExifTagGrid.Children.Add(valueLabel);

                currentRowCount++;
            }
        }

        private void SaveSelectedTagKeysButton_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
