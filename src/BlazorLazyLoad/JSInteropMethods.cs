#region using
using Microsoft.AspNetCore.Blazor.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.JSInterop;
using System;
using System.ComponentModel;
using System.Reflection;
using System.Threading.Tasks;
#endregion using

namespace BlazorLazyLoad
{
	[EditorBrowsable(EditorBrowsableState.Never)]
	public static class JSInteropMethods
	{
		internal static IServiceProvider ServiceProvider;
		internal static IServiceCollection Services;
		internal static WebAssemblyHost Host;

		[JSInvokable("NotifyLocationChanged")]
		public static async Task NotifyLocationChanged(string uri,bool isInterceptedLink)
		{
			await ServiceProvider.GetRequiredService<IAssemblyLazyLoadResolver>().ResolveAsync(uri,isInterceptedLink);

			//Call Microsoft.AspNetCore.Blazor.JSInteropMethods.NotifyLocationChanged(uri,isInterceptedLink)
			//Would it be possible to send original parameters and call it via DotNetDispatcher? It'd need to get ServiceProvider.GetRequiredService<IJSRuntime>() instance.
			Type jSInteropMethodsType = Type.GetType("Microsoft.AspNetCore.Blazor.JSInteropMethods, Microsoft.AspNetCore.Blazor");
			jSInteropMethodsType.GetMethod("NotifyLocationChanged",BindingFlags.Public|BindingFlags.Static)
				.Invoke(null,new object[] { uri,isInterceptedLink });
		}
	}
}
