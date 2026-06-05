//-----------------------------------------------------------------------
// <copyright file="ToIcoConverter.cs" company="Lifeprojects.de">
//     Class: ToIcoConverter
//     Copyright © Lifeprojects.de 2026
// </copyright>
//
// <author>2026 - Lifeprojects.de</author>
// <email>developer@lifeprojects.de</email>
// <date>05.03.2026 18:21:36</date>
//
// <summary>
// Die Klasse ToIcoConverter konvertiert den XAML Code von DrawingImage in ein ICO-Icon.
// Es werden die Größen 16x16, 24x24, 32x32, 48x48, 64x64, 128x128 und 256x256 unterstützt.
// </summary>
//-----------------------------------------------------------------------
namespace SnippetManager.Converter
{
    using System.Collections.Generic;
    using System.IO;
    using System.Windows;
    using System.Windows.Media;
    using System.Windows.Media.Imaging;

    internal class ToIcoConverter
    {
        private const double DPI = 96.0;
        private static int[] sizes = {16, 24, 32, 48, 64, 128, 256};

        public static void Convert(DrawingImage drawingImage, string outputPath)
        {
            IEnumerable<BitmapSource> bitmaps = sizes.Select(size => RenderDrawingImage(drawingImage, size, size));
            if (bitmaps != null)
            {
                SaveIcon(outputPath, bitmaps);
            }
        }

        public static void Convert(DrawingImage drawingImage, string outputPath, int size = 256)
        {
            IEnumerable<BitmapSource> bitmaps = sizes.Select(size => RenderDrawingImage(drawingImage, size, size)).Where(s => s.Width == size);
            if (bitmaps != null)
            {
                SaveIcon(outputPath, bitmaps);
            }
        }

        private static void SaveIcon(string fileName, IEnumerable<BitmapSource> bitmaps)
        {
            var images = new List<(byte[] Data, int Width, int Height)>();

            foreach (var bitmap in bitmaps)
            {
                using MemoryStream ms = new();

                PngBitmapEncoder encoder = new();
                encoder.Frames.Add(BitmapFrame.Create(bitmap));
                encoder.Save(ms);

                images.Add(
                (
                    ms.ToArray(),
                    bitmap.PixelWidth,
                    bitmap.PixelHeight
                ));
            }

            using FileStream fs = File.Create(fileName);
            using BinaryWriter writer = new(fs);

            // ICONDIR
            writer.Write((ushort)0);
            writer.Write((ushort)1);
            writer.Write((ushort)images.Count);

            int imageOffset = 6 + images.Count * 16;

            foreach (var image in images)
            {
                writer.Write((byte)(image.Width >= 256 ? 0 : image.Width));

                writer.Write((byte)(image.Height >= 256 ? 0 : image.Height));

                writer.Write((byte)0);
                writer.Write((byte)0);
                writer.Write((ushort)1);
                writer.Write((ushort)32);

                writer.Write((uint)image.Data.Length);
                writer.Write((uint)imageOffset);

                imageOffset += image.Data.Length;
            }

            foreach (var image in images)
            {
                writer.Write(image.Data);
            }
        }

        private static RenderTargetBitmap RenderDrawingImage(DrawingImage drawingImage, int width, int height)
        {
            DrawingVisual visual = new DrawingVisual();

            using (DrawingContext dc = visual.RenderOpen())
            {
                dc.DrawImage(drawingImage, new Rect(0, 0, width, height));
            }

            RenderTargetBitmap bitmap = new RenderTargetBitmap(width, height, DPI, DPI, PixelFormats.Pbgra32);

            bitmap.Render(visual);

            return bitmap;
        }
    }
}
