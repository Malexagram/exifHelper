using System;
using System.IO;

namespace ExifHelper.DesktopUi
{
    public static class ExifHelperUtility
    {
        public static string? ExifLocation;
        public static string? ImageCopyPath;

        public static void InitializeApplication()
        {
            ImageCopyPath = Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData);
            ImageCopyPath = Path.Combine(ImageCopyPath, @"ExifToolUi\Images");
            Directory.CreateDirectory(ImageCopyPath);
            ClearCopiedImages();
        }

        public static void ClearCopiedImages()
        {
            if (string.IsNullOrEmpty(ImageCopyPath)) return;

            foreach (string file in Directory.GetFiles(ImageCopyPath))
            {
                File.Delete(file);
            }
        }
    }
}
