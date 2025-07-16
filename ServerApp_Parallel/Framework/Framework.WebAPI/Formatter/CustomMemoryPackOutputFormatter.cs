using MemoryPack;
using MemoryPack.Compression;
using Microsoft.AspNetCore.Mvc.Formatters;

namespace Framework.Core.WebAPI.Formatters
{
	/// <summary>
	/// BrotliMemorypackOutputFormatter
	/// </summary>
	public class BrotliMemorypackOutputFormatter : OutputFormatter
	{
		private const string ContentType = "application/x-memorypack";
		private readonly MemoryPackSerializerOptions options;
		private readonly bool checkContentType = false;

		public BrotliMemorypackOutputFormatter(bool checkContentType = false)
			: this(null!)
		{
			this.checkContentType = checkContentType;
		}

		public BrotliMemorypackOutputFormatter(MemoryPackSerializerOptions options)
		{
			this.options = new MemoryPackSerializerOptions();
			SupportedMediaTypes.Add(ContentType);
		}

		public override bool CanWriteResult(OutputFormatterCanWriteContext context)
		{
			if (checkContentType)
			{
				return (context.ContentType == ContentType);
			}

			return true;
		}

		public override Task WriteResponseBodyAsync(OutputFormatterWriteContext context)
		{
			if (context.Object == null)
			{
				var stream = new MemoryStream();
				stream.WriteByte(MemoryPackCode.NullObject);
				stream.CopyTo(context.HttpContext.Response.Body);

				return stream.CopyToAsync(context.HttpContext.Response.Body);
			}

			// HTTP Context.
			var httpContext = context.HttpContext;
			httpContext.Response.ContentType = ContentType;

			var objectType = (context.ObjectType == null || context.ObjectType == typeof(object))
					? context.Object.GetType()
					: context.ObjectType;

			// Compression(require using)
			using (var compressor = new BrotliCompressor())
			{
				MemoryPackSerializer.Serialize(objectType, compressor, context.Object);
				var sd = compressor.CopyToAsync(context.HttpContext.Response.Body).AsTask();
				return sd;
			}
		}
	}
}
