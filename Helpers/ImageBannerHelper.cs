using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;

namespace NexScore.Helpers
{
    public static class ImageBannerHelper
    {
        public const int TargetWidth = 1600;
        public const int TargetHeight = 400;
        public const double TargetAspect = 4.0;

        public const double AspectTolerance = 0.005; // 0.5%

        public static bool IsFourToOne(int width, int height, double tolerance = 0.0)
        {
            if (height <= 0) return false;
            double ratio = (double)width / height;
            return Math.Abs(ratio - TargetAspect) <= tolerance;
        }

        public static string ProcessBannerToManagedCopyPreserveName(
            string sourcePath,
            string managedFolder,
            long jpegQuality = 85L)
        {
            Directory.CreateDirectory(managedFolder);

            var destFileName = BuildSafeFileNameFromSource(sourcePath, ".jpg");
            var destPath = GetNonCollidingPath(Path.Combine(managedFolder, destFileName));

            using var original = LoadClone(sourcePath);

            using var resized = ResizeToExact(original, TargetWidth, TargetHeight);

            SaveJpeg(resized, destPath, jpegQuality);
            return destPath;
        }

        public static string ProcessBannerToManagedCopyWithBaseName(
            string sourcePath,
            string managedFolder,
            string baseName,
            long jpegQuality = 85L)
        {
            Directory.CreateDirectory(managedFolder);

            var destFileName = BuildSafeFileName(baseName, ".jpg");
            var destPath = Path.Combine(managedFolder, destFileName);

            using var original = LoadClone(sourcePath);
            using var resized = ResizeToExact(original, TargetWidth, TargetHeight);

            SaveJpeg(resized, destPath, jpegQuality);
            return destPath;
        }

        public static Image LoadClone(string path)
        {
            using var fs = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
            using var temp = Image.FromStream(fs);
            return new Bitmap(temp);
        }

        public static Image ResizeToExact(Image source, int width, int height)
        {
            if (source.Width == width && source.Height == height)
                return new Bitmap(source);

            var bmp = new Bitmap(width, height, PixelFormat.Format24bppRgb);
            bmp.SetResolution(source.HorizontalResolution, source.VerticalResolution);

            using var g = Graphics.FromImage(bmp);
            g.Clear(Color.White);
            g.CompositingQuality = CompositingQuality.HighQuality;
            g.InterpolationMode = InterpolationMode.HighQualityBicubic;
            g.SmoothingMode = SmoothingMode.HighQuality;

            g.DrawImage(source, 0, 0, width, height);
            return bmp;
        }

        public static void SaveJpeg(Image img, string path, long quality = 85L)
        {
            var jpegEncoder = ImageCodecInfo
                .GetImageDecoders()
                .First(x => x.FormatID == ImageFormat.Jpeg.Guid);

            using var encParams = new EncoderParameters(1);
            encParams.Param[0] = new EncoderParameter(Encoder.Quality, quality);

            img.Save(path, jpegEncoder, encParams);
        }

        private static string BuildSafeFileNameFromSource(string sourcePath, string newExtension)
        {
            string baseName = Path.GetFileNameWithoutExtension(sourcePath);
            foreach (var c in Path.GetInvalidFileNameChars())
                baseName = baseName.Replace(c, '_');

            baseName = baseName.Trim();
            if (string.IsNullOrEmpty(baseName))
                baseName = "banner";

            return baseName + newExtension;
        }

        private static string BuildSafeFileName(string baseName, string newExtension)
        {
            baseName = baseName ?? string.Empty;
            foreach (var c in Path.GetInvalidFileNameChars())
                baseName = baseName.Replace(c, '_');

            baseName = baseName.Trim();
            if (string.IsNullOrEmpty(baseName))
                baseName = "banner";

            return baseName + newExtension;
        }

        private static string GetNonCollidingPath(string desiredPath)
        {
            if (!File.Exists(desiredPath))
                return desiredPath;

            string dir = Path.GetDirectoryName(desiredPath)!;
            string name = Path.GetFileNameWithoutExtension(desiredPath);
            string ext = Path.GetExtension(desiredPath);

            int i = 1;
            string candidate;
            do
            {
                candidate = Path.Combine(dir, $"{name} ({i}){ext}");
                i++;
            } while (File.Exists(candidate));

            return candidate;
        }
    }
}