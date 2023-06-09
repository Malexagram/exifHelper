using ExifHelper.Application.Command;
using ExifHelper.Application.ExifTags.Model;
using System.Reflection;

namespace ExifHelper.Application.ExifTags
{
    public class ExifTagsSaveTagKeysPrefencesCommand : ExifCommand<ExifTagsSaveTagKeysPrefencesCommandResponse, ExifTagsSaveTagKeysPrefencesCommandRequest>
    {
        private const string FileName = "ExifTagsToSave.json";

        public override async Task<ExifTagsSaveTagKeysPrefencesCommandResponse> PerformCommandAsync(ExifTagsSaveTagKeysPrefencesCommandRequest command)
        {
            return await Task.FromResult(PerformCommand(command));
        }

        public override ExifTagsSaveTagKeysPrefencesCommandResponse PerformCommand(ExifTagsSaveTagKeysPrefencesCommandRequest command)
        {
            var tagSaveStructure = new ExifTagSaveFileStructure
            {
                TagKeys = (from at in command.AllTagKeys
                           where at.Value
                           select at.Key).ToList()
            };

            var filePath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            filePath = Path.Combine(filePath, FileName);

            var tagSaveJson = Newtonsoft.Json.JsonConvert.SerializeObject(tagSaveStructure);
            File.WriteAllText(filePath, tagSaveJson);

            return new ExifTagsSaveTagKeysPrefencesCommandResponse
            {
                FilePath = filePath
            };
        }
    }
}
