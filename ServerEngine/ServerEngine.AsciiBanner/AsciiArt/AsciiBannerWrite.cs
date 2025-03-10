using Figgle;
using System.Reflection;

namespace ServerEngine.AsciiBanner
{
	/// <summary>
	/// AsciiBannerWriter.
	/// </summary>
	public sealed class AsciiBannerWriter
	{
		private const string SOURCE_NAME = "ServerEngine.AsciiBanner.AsciiArt.Fonts.";

		public AsciiBannerWriter(string fontName)
		{
			var fontFile = $"{SOURCE_NAME}{fontName}";

			var assembly = Assembly.GetExecutingAssembly();
			using var stream = assembly.GetManifestResourceStream(fontFile);

			if (stream == null)
			{
				throw new FileNotFoundException($"Font file '{fontFile}' not found.");
			}

			_font = FiggleFontParser.Parse(stream);
		}

		/// <summary>
		/// FiggleFont
		/// </summary>
		private FiggleFont _font { get; }

		/// <summary>
		/// Get Ascii-text
		/// </summary>
		public string GetAsciiText(string input)
		{
			return _font.Render(input);
		}
	}
}
