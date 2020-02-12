#region using
using Microsoft.AspNetCore.Blazor.Hosting;
using Microsoft.Extensions.DependencyInjection;
using System;
#endregion using

namespace BlazorLazyLoad
{
	public static class LazyLoadServicesExtensions
	{
		public static LazyLoadServicesBuilder AddLazyLoadCore(this IServiceCollection services)
		{
			JSInteropMethods.Services=services;
			JSInteropMethods.ServiceProvider=services.BuildServiceProvider();
			return new LazyLoadServicesBuilder(services);
		}

		public static LazyLoadServicesBuilder AddLazyLoad<TResolver>(this IServiceCollection services) where TResolver : class,IAssemblyLazyLoadResolver
		{
			services.AddSingleton<IAssemblyLazyLoadResolver,TResolver>();
			return services.AddLazyLoadCore();
		}

		public static LazyLoadServicesBuilder AddLazyLoad<TResolver>(this IServiceCollection services,Func<IServiceProvider,TResolver> implementationFactory) where TResolver : class, IAssemblyLazyLoadResolver
		{
			services.AddSingleton<IAssemblyLazyLoadResolver>(implementationFactory);
			return services.AddLazyLoadCore();
		}
	}

	public class LazyLoadServicesBuilder
	{
		internal LazyLoadServicesBuilder(IServiceCollection services)
			=> Services=services;

		public void SetHost(WebAssemblyHost host)
		{
			JSInteropMethods.Host=host;
		}

		public IServiceCollection Services { get; }
	}
}
