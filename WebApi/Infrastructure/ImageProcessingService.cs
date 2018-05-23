using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.Extensions.Options;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats.Jpeg;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;
using SixLabors.ImageSharp.Processing.Transforms;
using SixLabors.Primitives;

namespace WebApi.Infrastructure
{
	public class ImageProcessingService
	{
		public class Options
		{
			public IEnumerable<string> SupportedFormats { get; set; }

			public int Quality { get; set; }

			public int UserSize { get; set; }

			public int PostSize { get; set; }
		}

		Options _options;

		public ImageProcessingService(IOptions<Options> options)
		{
			_options = options.Value ?? throw new ArgumentNullException(nameof(options));
		}

		public bool IsSupportedFormat(string contentType, string fileName)
		{
			return contentType.Contains("image") &&
				_options.SupportedFormats.Any(extension =>
					fileName.EndsWith(extension, StringComparison.OrdinalIgnoreCase));
		}

		public void ResizeUserImage(Stream input, Stream output)
		{
			Resize(input, output, new ResizeOptions()
			{
				Mode = ResizeMode.Crop,
				Size = new Size(_options.UserSize)
			});
		}

		public void ResizePostImage(Stream input, Stream output)
		{
			Resize(input, output, new ResizeOptions()
			{
				Mode = ResizeMode.Max,
				Size = new Size(_options.PostSize)
			});
		}

		private void Resize(Stream input, Stream output, ResizeOptions options)
		{
			using (Image<Rgba32> image = Image.Load(input))
			{
				image.Mutate(c => c.Resize(options));
				image.SaveAsJpeg(output, new JpegEncoder() {
					IgnoreMetadata = true,
					Quality = _options.Quality
				});
			}
		}
	}
}
