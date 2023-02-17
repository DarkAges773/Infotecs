namespace Infotecs.RssFeeder.Services
{
	class PeriodicHostedService : BackgroundService
	{
		private readonly TimeSpan _period;
		private readonly IServiceScopeFactory _factory;
		private readonly IConfiguration _config;
		public bool IsEnabled { get; set; } = true;
		public PeriodicHostedService(IServiceScopeFactory factory, IConfiguration config)
		{
			_factory = factory;
			_config = config;
			_period = TimeSpan.FromSeconds(_config.GetValue<double>("RssRefreshFrequency", 5));
		}
		protected override async Task ExecuteAsync(CancellationToken stoppingToken)
		{
			using PeriodicTimer timer = new PeriodicTimer(_period);
			do
			{
				try
				{
					if (IsEnabled)
					{
						await using AsyncServiceScope asyncScope = _factory.CreateAsyncScope();
						RssService sampleService = asyncScope.ServiceProvider.GetRequiredService<RssService>();
						await sampleService.ParseRssAsync();
					}
				}
				catch (Exception)
				{
					continue;
				}
			}
			while (!stoppingToken.IsCancellationRequested && await timer.WaitForNextTickAsync(stoppingToken));
		}
	}
}
