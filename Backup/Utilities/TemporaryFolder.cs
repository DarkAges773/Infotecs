using Microsoft.Extensions.Logging;

namespace Infotecs.Utilities
{
	class TemporaryFolder : IDisposable
	{
		private readonly DirectoryInfo _dir;
		private readonly ILogger? _log;
		/// <summary>
		/// Path to the temporary directory.
		/// </summary>
		public DirectoryInfo Dir { get { return _dir; } }
		/// <summary>
		/// Creates temporary folder in specified directory. Temporary folder is deleted when object gets disposed.
		/// </summary>
		/// <param name="path">Must lead to an existing directory.</param>
		/// <exception cref="ArgumentNullException"></exception>
		/// <exception cref="DirectoryNotFoundException"></exception>
		public TemporaryFolder(string path, ILogger? log = null)
		{
			_log = log;

			if(path == null)
				throw new ArgumentNullException(nameof(path));
				
			var pathDirInfo = new DirectoryInfo(path);
			if(!pathDirInfo.Exists)
				throw new DirectoryNotFoundException($"Temporary folders can only be created in existing directory, but specified directory does not exist: {pathDirInfo.FullName}.");

			string dirPath = Path.Combine(path, Guid.NewGuid().ToString());

			_log?.LogDebug($"Creating temporary folder at {path}");
			_dir = Directory.CreateDirectory(dirPath);
			_log?.LogDebug($"Temporary folder created: {_dir.FullName}");
		}
		public void Dispose()
		{
			_log?.LogDebug($"Removing temporary folder: {_dir.FullName}");
			if(_dir.Exists)
			{
				string dirName = _dir.FullName;
				_dir.Delete(true);
				_log?.LogDebug($"Temporary folder removed: {dirName}");
			}
		}
	}
}
