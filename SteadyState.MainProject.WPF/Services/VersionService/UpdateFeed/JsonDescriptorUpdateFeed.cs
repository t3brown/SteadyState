using Newtonsoft.Json.Linq;
using SteadyState.MainProject.WPF.Services.VersionInfrastructure.UpdateFeed;
using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace SteadyState.MainProject.WPF.Services.VersionService.UpdateFeed
{
	internal class JsonDescriptorUpdateFeed : IUpdateFeed
	{
		private const string DescriptorUrl = "https://xn----8sbad0brpbh5a2e.xn--p1ai/SteadyStateES/descriptor.json";

		public async Task<UpdateInfo> GetLatestAsync()
		{
			using var http = new HttpClient();
			var text = await http.GetStringAsync(DescriptorUrl);
			if (string.IsNullOrWhiteSpace(text)) throw new Exception("Не получет ответ от сервиса.");

			var j = JObject.Parse(text);
			var ver = j["version"]?.ToString();
			var url = j["downloadLink"]?.ToString();
			return (!string.IsNullOrWhiteSpace(ver) && !string.IsNullOrWhiteSpace(url))
				? new UpdateInfo(ver, url)
				: throw new Exception("Не удалось разобрать ответ от сервиса.");
		}
	}
}
