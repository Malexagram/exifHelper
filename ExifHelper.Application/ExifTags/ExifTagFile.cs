namespace ExifHelper.Application.ExifTags
{
    public class ExifTagFile
    {
        public string FilePath { get; set; }
        public Dictionary<string, string> ExifTags { get; set; }
    }
}
