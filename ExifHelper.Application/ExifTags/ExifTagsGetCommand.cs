using ExifHelper.Application.Command;
using ExifHelper.Application.Configuration;
using System.Diagnostics;

namespace ExifHelper.Application.ExifTags
{
    public class ExifTagsGetCommand : ExifCommand<ExifTagsGetCommandResponse, ExifTagsGetCommandRequest>
    {
        private readonly IConfigurationManager _configurationManager;

        private List<string> _lastExifTags;

        public ExifTagsGetCommand(IConfigurationManager configurationManager)
        {
            _configurationManager = configurationManager;
            _lastExifTags = new List<string>();
        }

        public override ExifTagsGetCommandResponse PerformCommand(ExifTagsGetCommandRequest command)
        {
            var response = new ExifTagsGetCommandResponse
            {
                ExifTagFiles = new()
            };

            if (_configurationManager.ExifApplicationPath == null || !(command.FileNames?.Any() ?? false)) return response;

            var exifDrive = Path.GetPathRoot(_configurationManager.ExifApplicationPath);
            var exifDirectory = Path.GetDirectoryName(_configurationManager.ExifApplicationPath);
            var exifFileName = Path.GetFileNameWithoutExtension(_configurationManager.ExifApplicationPath);

            foreach (var fileName in command.FileNames)
            {
                var startInfo = new ProcessStartInfo();
                var pathToGetExif = Path.GetFullPath(fileName);

                startInfo.RedirectStandardOutput = true;
                startInfo.CreateNoWindow = false;
                startInfo.FileName = "cmd.exe";
                startInfo.WorkingDirectory = exifDirectory;
                startInfo.WindowStyle = ProcessWindowStyle.Normal;
                startInfo.Arguments = $"/C {exifFileName} {fileName}";
                var cmdProcess = new Process();
                cmdProcess.StartInfo = startInfo;
                cmdProcess.OutputDataReceived += ReadOutput;
                cmdProcess.Start();
                cmdProcess.BeginOutputReadLine();
                cmdProcess.WaitForExit();

                response.ExifTagFiles.Add(GetExifTagFile(fileName, _lastExifTags));
                _lastExifTags = new();
            }

            return response;
        }

        public override async Task<ExifTagsGetCommandResponse> PerformCommandAsync(ExifTagsGetCommandRequest command)
        {
            return await Task.FromResult(PerformCommand(command));
        }

        private ExifTagFile GetExifTagFile(string filePath, List<string> fileTags)
        {
            var tagFile = new ExifTagFile { FilePath = filePath, ExifTags = new() };

            foreach (var fileTag in fileTags)
            {
                var splitFileTag = fileTag.Split(':', 2);
                var tagKey = splitFileTag[0].Trim();
                var tagValue = splitFileTag[1].Trim();

                if (!tagFile.ExifTags.ContainsKey(tagKey))
                    tagFile.ExifTags.Add(tagKey, tagValue);
            }

            return tagFile;
        }

        private void ReadOutput(object sender, DataReceivedEventArgs e)
        {
            if (e.Data != null)
                _lastExifTags.Add(e.Data);
        }
    }
}
