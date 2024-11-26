using Figgle;
using System.Reflection;

namespace ServerEngine.AsciiBanner
{
    public class AsciiBanner
    {
        private const string RESOURCE_BASE = "ServerEngine.AsciiBanner.Fonts.";

        public AsciiBanner(string fontName)
        {
            var fontFile = $"{RESOURCE_BASE}{fontName}";

            var assembly = Assembly.GetExecutingAssembly();
            var stream = assembly.GetManifestResourceStream(fontFile);

            _font = FiggleFontParser.Parse(stream);
        }

        /// <summary>
        /// Filestream
        /// </summary>
        private FileStream _fontStream { get; set; }

        /// <summary>
        /// FiggleFont
        /// </summary>
        private FiggleFont _font { get; set; }

        /// <summary>
        /// Get Ascii-text
        /// </summary>
        public string GetAsciiText(string input)
        {
            return _font.Render($"{input}");
        }
    }
}
