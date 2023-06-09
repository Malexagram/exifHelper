using ExifHelper.Application.Command;

namespace ExifHelper.Application.ExifTags
{
    public class ExifTagsGetCommandRequest : IExifCommandRequest
    {
        public List<string> FileNames { get; set; }
    }
}
