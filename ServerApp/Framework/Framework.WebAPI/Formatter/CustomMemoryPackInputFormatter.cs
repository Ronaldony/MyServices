
using MemoryPack;
using MemoryPack.Compression;
using Microsoft.AspNetCore.Mvc.Formatters;

namespace Framework.Core.WebAPI.Formatters
{
	/// <summary>
	/// BrotliMemorypackInputFormatter
	/// </summary>
	public class BrotliMemorypackInputFormatter : InputFormatter
	{
		private const string ContentType = "application/x-memorypack";

		public BrotliMemorypackInputFormatter()
		{
			SupportedMediaTypes.Add(ContentType);
		}

		public override async Task<InputFormatterResult> ReadRequestBodyAsync(InputFormatterContext context)
		{
			var request = context.HttpContext.Request;

			// Decompression(require using)
			using (var decompressor = new BrotliDecompressor())
			{
				using (var ms = new MemoryStream())
				{
					await request.Body.CopyToAsync(ms);

					var bytes = decompressor.Decompress(ms.ToArray());

					var result = MemoryPackSerializer.Deserialize(context.ModelType, bytes);
					return await InputFormatterResult.SuccessAsync(result).ConfigureAwait(false);
				}
			}
		}
	}
}
