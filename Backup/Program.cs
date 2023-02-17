using Infotecs.Backup.Services;
using Infotecs.Utilities;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;

namespace Infotecs.Backup
{
	internal class Program
	{
		static void Main(string[] args)
		{
			var builder = new ConfigurationBuilder();
			BuildConfig(builder);

			Log.Logger = new LoggerConfiguration()
				.ReadFrom.Configuration(builder.Build())
				.Enrich.FromLogContext()
				.WriteTo.Console()
				// Please note, that when running the app more than once per minute will make it to rewrite log file.
				// This would need resolving in a production app, but it will do for a test application purposes.
				.WriteTo.File(@"logs\log.txt", rollingInterval: RollingInterval.Minute)
				.CreateLogger();

			Log.Logger.Information("Application Starting");

			try
			{
				var host = Host.CreateDefaultBuilder()
					.ConfigureServices((context, services) =>
					{
						services.AddTransient<IBackupService, BackupService>()
							.AddScoped<IFileCompressor, ZipCompressor>();
					})
					.UseSerilog()
					.Build();

				var svc = ActivatorUtilities.CreateInstance<BackupService>(host.Services);
				svc.Run();
			}
			catch (Exception e)
			{
				Log.Logger.Error(e, "Unhandled exception occured:");
#if DEBUG
				throw;
#endif
			}
		}

		static void BuildConfig(IConfigurationBuilder builder)
		{
			builder.SetBasePath(Directory.GetCurrentDirectory())
				.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
				.AddJsonFile($"appsettings.{Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Production"}.json", optional: true)
				.AddEnvironmentVariables();
		}
	}
}