using ExifHelper.Application.Command;
using ExifHelper.Application.Configuration;

namespace ExifHelper.Application.ImageCopy
{
    public class ImageCopyCommand : ExifCommand<ImageCopyCommandResponse, ImageCopyCommandRequest>
    {
        private readonly IConfigurationManager _configurationManager;

        public ImageCopyCommand(IConfigurationManager configurationManager)
        {
            _configurationManager = configurationManager;
        }

        public override async Task<ImageCopyCommandResponse> PerformCommandAsync(ImageCopyCommandRequest command)
        {
            return await Task.FromResult(PerformCommand(command));
        }

        public override ImageCopyCommandResponse PerformCommand(ImageCopyCommandRequest command)
        {
            var result = new ImageCopyCommandResponse { OriginalCopiedFilePaths = new() };

            foreach (var filePath in command.FilePaths)
            {
                var copyToCopyPath = Path.Combine(_configurationManager.CopyImagePath, Path.GetFileName(filePath));
                var safeFilePath = SetFilePath(copyToCopyPath);
                File.Copy(filePath, safeFilePath);
                result.OriginalCopiedFilePaths.Add(new KeyValuePair<string, string>(filePath, safeFilePath));
            }

            return result;
        }

        private string SetFilePath(string initialFilePath)
        {
            if (string.IsNullOrEmpty(initialFilePath))
                throw new ArgumentNullException(nameof(initialFilePath));

            var hasFilePathBeenSet = !File.Exists(initialFilePath);
            var fileName = Path.GetFileNameWithoutExtension(initialFilePath);
            var fileNumber = 1;

            while (!hasFilePathBeenSet)
            {
                var currentNumberString = fileNumber == 1 ? string.Empty : fileNumber.ToString();
                var fileNumberString = fileName.Substring(fileName.Length - currentNumberString.Length, currentNumberString.Length);

                if (int.TryParse(fileNumberString, out int currentNumber))
                {
                    if (currentNumber == fileNumber)
                    {
                        fileNumber++;
                    }
                }

                var pureFileName = fileName.Substring(0, fileName.Length - currentNumberString.Length);
                fileName = pureFileName + fileNumber.ToString();
                initialFilePath = Path.Combine(Path.GetDirectoryName(initialFilePath), fileName, Path.GetExtension(initialFilePath));
                hasFilePathBeenSet = !File.Exists(initialFilePath);
            } 

            return initialFilePath;
        }
    }
}
