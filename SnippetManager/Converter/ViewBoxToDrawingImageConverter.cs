namespace SnippetManager.Converter
{
    using System;
    using System.Globalization;
    using System.Linq;
    using System.Security;
    using System.Text;
    using System.Xml.Linq;

    /// <summary>
    /// Konvertiert ein ViewBox/Canvas-basiertes Icon-XAML
    /// in ein DrawingImage-XAML.
    /// </summary>
    public static class ViewBoxToDrawingImageConverter
    {
        private static readonly XNamespace Ns = "http://schemas.microsoft.com/winfx/2006/xaml/presentation";
        private static readonly XNamespace XNs = "http://schemas.microsoft.com/winfx/2006/xaml";

        public static string Convert(string viewBoxXaml, string keyName = null, bool withNamespace = false)
        {
            if (string.IsNullOrWhiteSpace(viewBoxXaml))
            {
                throw new ArgumentException("XAML darf nicht leer sein.");
            }

            var root = XElement.Parse(viewBoxXaml);

            if (root.Name.LocalName != "Viewbox")
            {
                throw new InvalidOperationException("Root muss ein Viewbox Element sein.");
            }

            string key = $"Icon{keyName}" ?? root.Attribute(XNs + "Key")?.Value ?? "ConvertedDrawingImage";

            double targetWidth = ParseDouble(root.Attribute("Width")?.Value, 64);
            double targetHeight = ParseDouble(root.Attribute("Height")?.Value, 64);

            // Erstes Canvas finden
            var outerCanvas = root.Descendants().FirstOrDefault(e => e.Name.LocalName == "Canvas");

            if (outerCanvas == null)
            {
                throw new InvalidOperationException("Kein Canvas gefunden.");
            }

            double sourceWidth = ParseDouble(outerCanvas.Attribute("Width")?.Value, targetWidth);
            double sourceHeight = ParseDouble(outerCanvas.Attribute("Height")?.Value, targetHeight);

            double scaleX = targetWidth / sourceWidth;
            double scaleY = targetHeight / sourceHeight;

            // Alle Path-Elemente sammeln
            var paths = root.Descendants().Where(e => e.Name.LocalName == "Path").ToList();

            var sb = new StringBuilder();

            if (withNamespace == true)
            {
                sb.AppendLine(CultureInfo.CurrentCulture, $@"<DrawingImage xmlns=""{Ns}""\nxmlns:x=""{XNs}""\nx:Key=""{key.Replace("VB", "DI")}"">");
            }
            else
            {
                sb.AppendLine(CultureInfo.CurrentCulture, $@"<DrawingImage x:Key=""{key.Replace("VB", "DI")}"">");
            }

            sb.AppendLine("    <DrawingImage.Drawing>");
            sb.AppendLine("        <DrawingGroup>");

            // Transform
            sb.AppendLine("            <DrawingGroup.Transform>");
            sb.AppendLine(CultureInfo.CurrentCulture,
                $"                <ScaleTransform ScaleX=\"{scaleX.ToString(CultureInfo.InvariantCulture)}\" " +
                $"ScaleY=\"{scaleY.ToString(CultureInfo.InvariantCulture)}\" />");
            sb.AppendLine("            </DrawingGroup.Transform>");
            sb.AppendLine();

            // GeometryDrawings erzeugen
            foreach (var path in paths)
            {
                string data = path.Attribute("Data")?.Value;
                string fill = path.Attribute("Fill")?.Value;

                if (string.IsNullOrWhiteSpace(data))
                {
                    continue;
                }

                sb.AppendLine(CultureInfo.CurrentCulture, $"<GeometryDrawing Brush=\"{fill}\" Geometry=\"{EscapeXml(data)}\" />");

            }

            sb.AppendLine();

            sb.AppendLine("        </DrawingGroup>");
            sb.AppendLine("    </DrawingImage.Drawing>");
            sb.AppendLine("</DrawingImage>");

            return sb.ToString();
        }

        private static double ParseDouble(string value, double fallback)
        {
            if (double.TryParse(
                    value,
                    NumberStyles.Any,
                    CultureInfo.InvariantCulture,
                    out double result))
            {
                return result;
            }

            return fallback;
        }

        private static string EscapeXml(string value)
        {
            return SecurityElement.Escape(value);
        }
    }
}
