using ConfluenceExport.Models;
using Microsoft.Extensions.Logging;
using System.Net.Http.Headers;
using System.Net.Http.Json;

namespace ConfluenceExport
{
	public class ExportPdfService
	{
		private ILogger<ExportPdfService> _logger;
		private HttpClient _httpClient;

		public ExportPdfService(HttpClient httpClient, ILogger<ExportPdfService> logger)
		{
			_httpClient = httpClient;
			_httpClient.BaseAddress = new System.Uri("https://scroll-pdf.us.exporter.k15t.app/api");
			_logger = logger;
			_logger.LogInformation("Create ExportPdfService Instance");
		}

		public async Task RunAsync(string token)
		{
			try
			{
				var exportParams = new StartExportRequest()
				{
					PageId = "917241857",
					TemplateId = "com.k15t.scroll.pdf.default-template-article",
					Scope = "descendants",
					Locale = "",
					VersionId = "",
					VariantId = "",
					LanguageKey = "",
					TimeZone = ""
				};

				_logger.LogInformation("Starting export");

				var mediaTypeHeader = new MediaTypeHeaderValue("application/json");
				JsonContent jsonContent = JsonContent.Create(exportParams, mediaType: mediaTypeHeader);
				
				
				_httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {token}");				

				_logger.LogInformation("headers added");

				await Task.Run(async () =>
				{
					return await StartExportJob(exportParams).ConfigureAwait(false);

					
				}).ContinueWith((previous) =>
				{
					_logger.LogInformation("Export started");
					_logger.LogInformation($"Job ID: {previous}");
				});	
				
			}
			catch (Exception ex)
			{
				_logger.LogError($"ERROR: {ex.Message}");
				_logger.LogError(ex.StackTrace);
			}
		}

		private async Task<string> StartExportJob(StartExportRequest exportParams)
		{
			string jobIdValue = "";
			try
			{
				var mediaTypeHeader = new MediaTypeHeaderValue("application/json");
				JsonContent jsonContent = JsonContent.Create(exportParams, mediaType: mediaTypeHeader);

				_logger.LogInformation("Starting export");

				Uri uri = new Uri(@"https://scroll-pdf.us.exporter.k15t.app/api/public/1/exports");

				HttpResponseMessage response = await _httpClient.PostAsync(uri, jsonContent).ConfigureAwait(false);

				_logger.LogInformation($"Returned code {response.StatusCode}");

				if (response.StatusCode == System.Net.HttpStatusCode.OK)
				{
					string responseContent = await response.Content.ReadAsStringAsync();
					if (!string.IsNullOrWhiteSpace(responseContent))
					{
						string jobId = responseContent.Substring(responseContent.IndexOf('{') + 1, responseContent.IndexOf('}') - 1);
						string[] jobIdArray = jobId.Split(':');
						jobIdValue = jobIdArray[1].Trim().Trim('\"');
						_logger.LogInformation($"Job ID: {jobIdValue}");
					}

				}

			}
			catch (Exception ex)
			{
				_logger.LogError($"ERROR: {ex.Message}");
				_logger.LogError(ex.StackTrace);
				jobIdValue = "";
			}

			return jobIdValue;
		}
	}

	
}
