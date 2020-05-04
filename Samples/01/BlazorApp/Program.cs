#region using
using System;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using BlazorLazyLoad;
using Microsoft.Extensions.DependencyInjection;
#endregion using

namespace BlazorApp
{
	public class Program
	{
		public static async Task Main(string[] args)
		{
			var builder = WebAssemblyHostBuilder.CreateDefault(args);
			builder.RootComponents.Add<App>("app");

			builder.Services.AddTransient(sp => new HttpClient { BaseAddress=new Uri(builder.HostEnvironment.BaseAddress) });
			LazyLoadServicesBuilder lazyLoadServicesBuilder = builder.Services.AddLazyLoad<AreaAssemblyLazyLoadResolver>();

			WebAssemblyHost host = builder.Build();
			lazyLoadServicesBuilder.SetHost(host);
			await host.RunAsync();
		}
	}
}
