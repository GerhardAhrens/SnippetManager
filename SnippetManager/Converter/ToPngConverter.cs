//-----------------------------------------------------------------------
// <copyright file="ToPngConverter.cs" company="Lifeprojects.de">
//     Class: ToPngConverter
//     Copyright © Lifeprojects.de 2026
// </copyright>
//
// <author>2026 - Lifeprojects.de</author>
// <email>developer@lifeprojects.de</email>
// <date>05.03.2026 18:21:36</date>
//
// <summary>
// Die Klasse ToPngConverter konvertiert den XAML Code von DrawingImage in ein PNG-Bild.
// </summary>
//-----------------------------------------------------------------------

namespace SnippetManager.Converter
{
    using System.Windows;
    using System.Windows.Media;
    using System.Windows.Media.Imaging;

    internal sealed class ToPngConverter
    {
        private const double DPI  = 96.0;

        public static void Convert(DrawingImage drawingImage, int width, int height, string outputPath)
        {
            RenderTargetBitmap bitmap = RenderDrawingImage(drawingImage, width, height);
            PngBitmapEncoder encoder = new PngBitmapEncoder();
            encoder.Frames.Add(BitmapFrame.Create(bitmap));
            using (var stream = System.IO.File.Create(outputPath))
            {
                encoder.Save(stream);
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
