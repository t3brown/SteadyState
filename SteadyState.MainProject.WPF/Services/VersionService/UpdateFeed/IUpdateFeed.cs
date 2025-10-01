using System.Threading.Tasks;

namespace SteadyState.MainProject.WPF.Services.VersionInfrastructure.UpdateFeed
{
	internal sealed record UpdateInfo(string Version, string DownloadUrl);

	internal interface IUpdateFeed
	{
		Task<UpdateInfo> GetLatestAsync();
	}
}
