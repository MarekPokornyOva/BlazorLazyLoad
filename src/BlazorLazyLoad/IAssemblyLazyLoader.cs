#region using
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.AspNetCore.Components.RenderTree;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
#endregion using

namespace BlazorLazyLoad
{
	public interface IAssemblyLazyLoadResolver
	{
		Task ResolveAsync(string uri,bool isInterceptedLink);
	}

	public abstract class AssemblyLazyLoadResolverBase:IAssemblyLazyLoadResolver
	{
		public abstract Task ResolveAsync(string uri,bool isInterceptedLink);

		protected IRouterEnvelope Router => JSInteropMethods.Router;

		public static void LoadServices(Assembly assembly)
		{
			//Find Program.ConfigureServices(IServiceCollection) method.
			MethodInfo configureServices = FindDefaultConfigureServicesMethod(assembly);
			if (configureServices==default)
				return;

			LoadServices(configureServices);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		static MethodInfo FindDefaultConfigureServicesMethod(Assembly assembly)
			=> assembly.GetTypes().FirstOrDefault(x => string.Equals(x.Name,"Program",StringComparison.Ordinal))?
				.GetMethod("ConfigureServices",BindingFlags.Public|BindingFlags.Static,null,new Type[] { typeof(IServiceCollection) },null);

		public static void LoadServices(MethodInfo configureServices)
		{
			//Call the ConfigureServices(IServiceCollection) method.
			IServiceCollection services = JSInteropMethods.Services;
			configureServices.Invoke(null,new object[] { services });

			//Is it possible to get if a service registration was changed? Maybe with tracking wrapper. But how about specific Factory on transient ServiceDescriptor?
			//We can return from this method if nothing got changed.

			//Get new IServiceProvider and inject it to Renderer and WebAssemblyHost.
			IServiceProvider sp = services.BuildServiceProvider();
			IServiceScope newScope = sp.GetRequiredService<IServiceScopeFactory>().CreateScope();
			WebAssemblyHost host = JSInteropMethods.Host;
			Type hostType = host.GetType();
			hostType.GetField("_scope",BindingFlags.NonPublic|BindingFlags.Instance).SetValue(host,newScope);
			object renderer = hostType.GetField("_renderer",BindingFlags.NonPublic|BindingFlags.Instance).GetValue(host);
			typeof(Renderer).GetField("_serviceProvider",BindingFlags.NonPublic|BindingFlags.Instance).SetValue(renderer,newScope.ServiceProvider);
		}
	}
}
