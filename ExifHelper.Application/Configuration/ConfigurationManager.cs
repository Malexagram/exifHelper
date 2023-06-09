namespace ExifHelper.Application.Configuration
{
    public class ConfigurationManager : IConfigurationManager
    {
        private readonly Model.Configuration _configuration;

        public ConfigurationManager(Model.Configuration configuration)
        {
            _configuration = configuration;
        }

        public string ExifApplicationPath 
        { 
            get => _configuration.ExifApplicationPath; 
            set => _configuration.ExifApplicationPath = value; 
        }

        public string CopyImagePath
        {
            get
            {
                if (string.IsNullOrEmpty(_configuration.CopyImageDirectoryPath))
                    SetDefaultImagePath();

                return _configuration.CopyImageDirectoryPath;
            }
            set
            {
                ClearCopiedImages();
                _configuration.CopyImageDirectoryPath = value;
            }
        }

        public void ClearCopiedImages()
        {
            if (string.IsNullOrEmpty(CopyImagePath)) return;

            foreach (string file in Directory.GetFiles(CopyImagePath))
            {
                File.Delete(file);
            }

            return;
        }

        private void SetDefaultImagePath()
        {
            var appDataPath = Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData);
            _configuration.CopyImageDirectoryPath = Path.Combine(appDataPath, @"ExifToolUi\Images");
            Directory.CreateDirectory(_configuration.CopyImageDirectoryPath);
        }
    }
}
