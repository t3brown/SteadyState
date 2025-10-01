using Newtonsoft.Json.Linq;
using System;
using System.Linq;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace SteadyState.MainProject.WPF.Services.VersionInfrastructure.UpdateFeed
{
    internal class GitHubReleaseUpdateFeed : IUpdateFeed
	{

		private const string Url = "https://api.github.com/repos/{0}/{1}/releases/latest";

		private string _owner;
		private string _repo;
		private Regex _assetNamePattern;

		public GitHubReleaseUpdateFeed(string owner, string repo, string assetNamePattern = @"\.exe$")
		{
			_owner = owner;
			_repo = repo;
			_assetNamePattern = new Regex(assetNamePattern, RegexOptions.IgnoreCase | RegexOptions.Compiled);
		}

		public async Task<UpdateInfo> GetLatestAsync()
		{
			using var httpClint = new HttpClient();
			httpClint.DefaultRequestHeaders.UserAgent.ParseAdd("SteadyStateES-Updater/1.0");
			var response = await httpClint.GetStringAsync(string.Format(Url, _owner, _repo));

			var data = JObject.Parse(response);
			var info = Extract(data);

			return info ?? throw new Exception("Ошибка разбора ответа от GitHub");
		}

		private UpdateInfo? Extract(JToken data)
		{
			var tag = data["tag_name"]?.ToString();
			var assets = (JArray?)data["assets"];
			var asset = assets?.FirstOrDefault(a =>
			{
				var name = a?["name"]?.ToString() ?? "";
				return _assetNamePattern.IsMatch(name);
			});

			var url = asset?["browser_download_url"]?.ToString();
			if (string.IsNullOrWhiteSpace(tag) || string.IsNullOrWhiteSpace(url))
				return null;

			return new UpdateInfo(tag, url);
		}
	}
}
