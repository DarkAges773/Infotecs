using Infotecs.RssFeeder.Services;
using System.Net;

namespace Infotecs.RssFeeder
{
	public static class NamedHttpClients
	{
		public const string ProxiedClient = "ProxiedClient";
	}
	public class Program
	{
		public static void Main(string[] args)
		{
			var builder = WebApplication.CreateBuilder(args);
			builder.Configuration.AddXmlFile("appsettings.xml");
			builder.Services.AddControllersWithViews();
			// if proxy address is specified, then use proxied client, if not use default.
			if(builder.Configuration.GetValue<string>("ProxyAddress") != null)
			{
				builder.Services.AddHttpClient(NamedHttpClients.ProxiedClient)
					.ConfigurePrimaryHttpMessageHandler(() => new HttpClientHandler()
					{
						// if proxy credentials are given, use proxy with those, if not, then use proxy without credentials.
						Proxy = (builder.Configuration.GetValue<string>("ProxyUser") == null || builder.Configuration.GetValue<string>("ProxyPassword") == null) ? 
						new WebProxy(builder.Configuration.GetValue<string>("ProxyAddress")) : 
						new WebProxy(builder.Configuration.GetValue<string>("ProxyAddress"), true, null, new NetworkCredential(
							builder.Configuration.GetValue<string>("ProxyUser"),
							builder.Configuration.GetValue<string>("ProxyPassword")
						))
					});
			}
			else
			{
				builder.Services.AddHttpClient();
			}
			builder.Services.AddScoped<RssService>();
			builder.Services.AddSingleton<PeriodicHostedService>();
			builder.Services.AddHostedService(
				provider => provider.GetRequiredService<PeriodicHostedService>());

			var app = builder.Build();

			// Configure the HTTP request pipeline.
			if (!app.Environment.IsDevelopment())
			{
				app.UseExceptionHandler("/Home/Error");
			}
			app.UseStaticFiles();

			app.UseRouting();

			app.UseAuthorization();

			app.MapControllerRoute(
				name: "default",
				pattern: "{controller=Home}/{action=Index}/{id?}");

			app.Run();
		}
	}
}