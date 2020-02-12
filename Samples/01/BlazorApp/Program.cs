#region using
using System.Threading.Tasks;
using Microsoft.AspNetCore.Blazor.Hosting;
using BlazorLazyLoad;
#endregion using

namespace BlazorApp
{
	public class Program
	{
		public static async Task Main(string[] args)
		{
			var builder = WebAssemblyHostBuilder.CreateDefault(args);
			builder.RootComponents.Add<App>("app");
			builder.RootComponents.Add<LazyLoadDirectHandler>("body");
			LazyLoadServicesBuilder lazyLoadServicesBuilder = builder.Services.AddLazyLoad<AreaAssemblyLazyLoadResolver>();

			WebAssemblyHost host = builder.Build();
			lazyLoadServicesBuilder.SetHost(host);
			await host.RunAsync();
		}
	}
}
