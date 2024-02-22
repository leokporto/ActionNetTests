using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace ConfluenceExport.Models
{
	internal class StartExportRequest
	{
		public StartExportRequest()
		{
		}

		[JsonPropertyName("pageId")]
		public string PageId { get; set; }
		
		[JsonPropertyName("templateId")]
		public string TemplateId { get; set; }
		
		[JsonPropertyName("scope")]
		public string Scope { get; set; }
		
		[JsonPropertyName("locale")]
		public string Locale { get; set; }
		
		[JsonPropertyName("versionId")]
		public string VersionId { get; set; }
		
		[JsonPropertyName("variantId")]
		public string VariantId { get; set; }
		
		[JsonPropertyName("languageKey")]
		public string LanguageKey { get; set; }
		
		[JsonPropertyName("timeZone")]
		public string TimeZone { get; set; }
	}
}
