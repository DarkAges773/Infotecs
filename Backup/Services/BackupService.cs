using Infotecs.Utilities;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Infotecs.Backup.Services
{
	public class BackupService : IBackupService
	{
		private readonly ILogger<BackupService>? _log;
		private readonly IConfiguration _config;
		private readonly string _backupDir;
		private readonly string[] _sourceDirs;

		private readonly IFileCompressor _fileCompressor;

		public BackupService(IConfiguration config, IFileCompressor fileCompressor, ILogger<BackupService>? log = null)
		{
			_log = log;
			_config = config;
			_fileCompressor = fileCompressor;
			_backupDir = _config.GetValue<string>("DestinationFolder");
			_sourceDirs = _config.GetSection("SourceFolders")
				.GetChildren()
				.Select(x => x.Value)
				.ToArray();

			var backupDirInfo = new DirectoryInfo(_backupDir);
			if (!backupDirInfo.Exists)
				Directory.CreateDirectory(_backupDir);
		}
		public void Run()
		{
			_log?.LogDebug($"Backup service started.");
			foreach (string sourceDir in _sourceDirs)
			{
					BackupDirectory(sourceDir);
			}
			_log?.LogInformation($"Backup complete.");
		}
		/// <summary>
		/// Backs up one directory
		/// </summary>
		/// <param name="sourceDir">Directory to be backed up</param>
		private void BackupDirectory(string sourceDir)
		{
			_log?.LogInformation($"Backing up: {sourceDir}");
			using var tempDir = new TemporaryFolder(_backupDir, _log);

			try
			{
				CopyDirectory(sourceDir, tempDir.Dir.FullName);
			}
			catch (DirectoryNotFoundException e)
			{
				_log?.LogWarning($"{e.Message} Skipping backup creation for {sourceDir}");
				return;
			}
			catch (UnauthorizedAccessException e)
			{
				_log?.LogWarning($"{e.Message} Skipping backup creation for {sourceDir}");
				return;
			}

			_fileCompressor.CompressDirectory(tempDir.Dir.FullName, GenerateUniqueBackupName(Path.GetFileName(sourceDir), _fileCompressor.FileExtension));
		}
		/// <summary>
		/// Generates backup name with current timestamp, file name and file extension. If a file with generated name already exists, tries generating with added duplicateNumber 
		/// </summary>
		/// <param name="fileName">The name of backup</param>
		/// <param name="fileExtension">File extension of generated name. Example string "zip"</param>
		/// <param name="duplicateNum">Used for determining the amount of existing duplicates</param>
		/// <returns>Unique file name</returns>
		private string GenerateUniqueBackupName(string fileName, string fileExtension, int duplicateNum = 0)
		{
			string destinationArchiveFileName = Path.Combine(
				_backupDir, 
				$"{DateTime.Now.ToString(_config.GetValue<string>("TimestampFormat"))}-{fileName}{(duplicateNum > 0 ? $"-{duplicateNum}" : "")}.{fileExtension}"
			);
			if (File.Exists(destinationArchiveFileName))
			{
				destinationArchiveFileName = GenerateUniqueBackupName(fileName, fileExtension, ++duplicateNum);
			}
			return destinationArchiveFileName;
		}
		/// <summary>
		/// Copies source directory to destination directory
		/// </summary>
		/// <param name="sourceDir">A directory to be copied</param>
		/// <param name="destinationDir">Destination directory</param>
		/// <param name="recursive">Whether the directory should be copied recursively. Default = true</param>
		/// <exception cref="DirectoryNotFoundException"></exception>
		/// <exception cref="UnauthorizedAccessException"></exception>
		private void CopyDirectory(string sourceDir, string destinationDir, bool recursive = true)
		{
			_log?.LogDebug($"Copying: {sourceDir} to {destinationDir} {(recursive ? "recursively" : "")}");
			var dir = new DirectoryInfo(sourceDir);

			if (!dir.Exists)
				throw new DirectoryNotFoundException($"Source directory not found: {dir.FullName}");

			DirectoryInfo[] dirs = dir.GetDirectories();
			Directory.CreateDirectory(destinationDir);

			foreach (FileInfo file in dir.GetFiles())
			{
				_log?.LogDebug($"Copying file: {file.FullName}");
				string targetFilePath = Path.Combine(destinationDir, file.Name);
				file.CopyTo(targetFilePath);
			}

			if (recursive)
			{
				foreach (DirectoryInfo subDir in dirs)
				{
					string newDestinationDir = Path.Combine(destinationDir, subDir.Name);
					CopyDirectory(subDir.FullName, newDestinationDir, true);
				}
			}
		}
	}
}