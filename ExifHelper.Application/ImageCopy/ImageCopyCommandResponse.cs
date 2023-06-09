using ExifHelper.Application.Command;

namespace ExifHelper.Application.ImageCopy
{
    public class ImageCopyCommandResponse : IExifCommandResponse
    {
        public List<KeyValuePair<string, string>> OriginalCopiedFilePaths { get; set; }
    }
}
