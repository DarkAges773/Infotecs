namespace Infotecs.Utilities
{
	public interface IFileCompressor
	{
		public string FileExtension { get; }
		/// <summary>
		/// Compresses directory source directory into file.
		/// </summary>
		/// <param name="sourceDir">Directory to compress</param>
		/// <param name="destinationArchiveFile">Full name of the resulting file</param>
		public void CompressDirectory(string sourceDir, string destinationArchiveFile);
	}
}