using ExifHelper.Application.Command;

namespace ExifHelper.Application.ImageCopy
{
    public class ImageCopyCommandRequest : IExifCommandRequest
    {
        public string[] FilePaths { get; set; }

    }
}
