using Cocona;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace ConfluenceExport
{
	internal class Program
	{
		private ILogger<Program> _logger;
		private IConfiguration _configuration;

		public Program(ILogger<Program> logger, IConfiguration configuration)
		{
			_logger = logger;
			_configuration = configuration;
			_logger.LogInformation("Create Program Instance");
		}

		static void Main(string[] args)
		{
			var builder = CoconaApp.CreateHostBuilder()
				.ConfigureLogging(logging =>
				{
					logging.AddDebug();
				})
				.ConfigureAppConfiguration((hostingContext, config) =>
				{
					config.AddUserSecrets("e81a2485-9782-49c8-9c7e-9f15836e25e9");
				})
				.ConfigureServices(services =>
				{
					services.AddScoped<HttpClient>();
					services.AddTransient<ExportPdfService>();
				});
				
			builder.Run<Program>(args);
		}

		public void Run([FromService] ExportPdfService myService)
		{
			string token = _configuration["K15tExport:ServiceApiKey"];
			myService.RunAsync(token).Wait();
		}
	}
}
