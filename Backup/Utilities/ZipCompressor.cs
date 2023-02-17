using Microsoft.Extensions.Logging;
using System.IO.Compression;

namespace Infotecs.Utilities
{
	public class ZipCompressor : IFileCompressor
	{
		public string FileExtension => "zip";
		private readonly ILogger<ZipCompressor>? _log;
		public ZipCompressor(ILogger<ZipCompressor>? log = null)
		{
			_log = log;
		}
		public void CompressDirectory(string sourceDir, string destinationArchiveFile)
		{
			_log?.LogDebug($"Compressing archive: {destinationArchiveFile}");
			ZipFile.CreateFromDirectory(sourceDir, destinationArchiveFile);
		}
	}
}