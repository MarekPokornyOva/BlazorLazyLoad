#region using
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
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

		public static LazyLoadServicesBuilder AddLazyLoad<TAssemblyLazyLoadResolver>(this IServiceCollection services) where TAssemblyLazyLoadResolver : class, IAssemblyLazyLoadResolver
			=> AddLazyLoad<TAssemblyLazyLoadResolver,MetadataAssemblyDependencyResolver,DefaultAssemblyDownloader>(services);

		public static LazyLoadServicesBuilder AddLazyLoad<TAssemblyLazyLoadResolver, TAssemblyDependencyResolver, TAssemblyProvider>(this IServiceCollection services)
					where TAssemblyLazyLoadResolver : class, IAssemblyLazyLoadResolver
					where TAssemblyDependencyResolver : class, IAssemblyDependencyResolver
					where TAssemblyProvider : class, IAssemblyProvider
		{
			services.AddSingleton<IAssemblyLazyLoadResolver,TAssemblyLazyLoadResolver>();
			services.AddSingleton<IAssemblyDependencyResolver,TAssemblyDependencyResolver>();
			services.AddSingleton<IAssemblyProvider,TAssemblyProvider>();
			return services.AddLazyLoadCore();
		}

		public static LazyLoadServicesBuilder AddLazyLoad<TAssemblyLazyLoadResolver, TAssemblyDependencyResolver, TAssemblyProvider>(this IServiceCollection services,
			Func<IServiceProvider,TAssemblyLazyLoadResolver> assemblyLazyLoadResolverImplementationFactory,
			Func<IServiceProvider,TAssemblyDependencyResolver> assemblyDependencyResolverImplementationFactory,
			Func<IServiceProvider,TAssemblyProvider> assemblyProviderImplementationFactory
			) 
					where TAssemblyLazyLoadResolver : class, IAssemblyLazyLoadResolver
					where TAssemblyDependencyResolver : class, IAssemblyDependencyResolver
					where TAssemblyProvider : class, IAssemblyProvider
		{
			services.AddSingleton<IAssemblyLazyLoadResolver>(assemblyLazyLoadResolverImplementationFactory);
			services.AddSingleton<IAssemblyDependencyResolver>(assemblyDependencyResolverImplementationFactory);
			services.AddSingleton<IAssemblyProvider>(assemblyProviderImplementationFactory);
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
