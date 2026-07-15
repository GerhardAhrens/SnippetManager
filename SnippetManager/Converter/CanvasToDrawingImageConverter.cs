//-----------------------------------------------------------------------
// <copyright file="CanvasToDrawingImageConverter.cs" company="Lifeprojects.de">
//     Class: CanvasToDrawingImageConverter
//     Copyright © Lifeprojects.de 2026
// </copyright>
//
// <author>GERHARD-G6\gerha - Lifeprojects.de</author>
// <email>developer@lifeprojects.de</email>
// <date>15.07.2026</date>
//
// <summary>
// Template für eine neue C# Standard-Klasse
// </summary>
//-----------------------------------------------------------------------

namespace SnippetManager.Converter
{
    using System;
    using System.Globalization;
    using System.IO;
    using System.Security;
    using System.Text;
    using System.Xml.Linq;

    public static class CanvasToDrawingImageConverter
    {
        private static readonly XNamespace Ns = "http://schemas.microsoft.com/winfx/2006/xaml/presentation";

        public static string Convert(string xamlFileName)
        {
            if (string.IsNullOrWhiteSpace(xamlFileName))
            {
                throw new ArgumentException("Im Parameter wird keine gefüllte Variable übergeben", nameof(xamlFileName));
            }

            if (File.Exists(xamlFileName) == false)
            {
                return string.Empty;
            }

            XDocument doc = XDocument.Load(xamlFileName);

            XElement canvas = doc.Root;

            if (canvas == null || canvas.Name != Ns + "Canvas")
            {
                throw new InvalidOperationException("Die Datei enthält kein Canvas.");
            }

            var paths = canvas.Elements(Ns + "Path").ToList();

            if (paths.Count == 0)
            {
                throw new InvalidOperationException("Das Canvas enthält keine Path-Elemente.");
            }

            StringBuilder sb = new();

            sb.AppendLine("<DrawingImage>");
            sb.AppendLine("    <DrawingImage.Drawing>");

            if (paths.Count > 1)
            {
                sb.AppendLine("        <DrawingGroup>");
            }

            foreach (var path in paths)
            {
                string geometry = path.Attribute("Data")?.Value ?? "";

                if (string.IsNullOrWhiteSpace(geometry))
                    continue;

                string fill = path.Attribute("Fill")?.Value ?? "Transparent";
                string stroke = path.Attribute("Stroke")?.Value ?? "";
                string thickness = path.Attribute("StrokeThickness")?.Value ?? "1";

                if (string.IsNullOrWhiteSpace(stroke))
                {
                    sb.AppendLine(CultureInfo.CurrentCulture,$"            <GeometryDrawing Brush=\"{fill}\" Geometry=\"{Escape(geometry)}\" />");
                }
                else
                {
                    sb.AppendLine(CultureInfo.CurrentCulture, $"            <GeometryDrawing Geometry=\"{Escape(geometry)}\">");
                    sb.AppendLine(CultureInfo.CurrentCulture, $"                <GeometryDrawing.Brush>");
                    sb.AppendLine(CultureInfo.CurrentCulture, $"                    <SolidColorBrush Color=\"{fill}\"/>");
                    sb.AppendLine(CultureInfo.CurrentCulture, $"                </GeometryDrawing.Brush>");
                    sb.AppendLine(CultureInfo.CurrentCulture, $"                <GeometryDrawing.Pen>");
                    sb.AppendLine(CultureInfo.CurrentCulture, $"                    <Pen Brush=\"{stroke}\" Thickness=\"{thickness}\"/>");
                    sb.AppendLine(CultureInfo.CurrentCulture, $"                </GeometryDrawing.Pen>");
                    sb.AppendLine(CultureInfo.CurrentCulture, $"            </GeometryDrawing>");
                }
            }

            if (paths.Count > 1)
            {
                sb.AppendLine("        </DrawingGroup>");
            }

            sb.AppendLine("    </DrawingImage.Drawing>");
            sb.AppendLine("</DrawingImage>");

            return sb.ToString();
        }

        private static string Escape(string value)
        {
            return SecurityElement.Escape(value) ?? value;
        }
    }
}
