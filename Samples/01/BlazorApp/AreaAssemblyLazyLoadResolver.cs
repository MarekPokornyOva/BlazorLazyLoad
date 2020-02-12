#region using
using BlazorLazyLoad;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Routing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Reflection;
using System.Threading.Tasks;
#endregion using

namespace BlazorApp
{
	public class AreaAssemblyLazyLoadResolver:AssemblyLazyLoadResolverBase
	{
		readonly HttpClient _httpClient;
		readonly NavigationManager _navigationManager;
		public AreaAssemblyLazyLoadResolver(HttpClient httpClient,NavigationManager navigationManager)
		{
			_httpClient=httpClient;
			_navigationManager=navigationManager;
		}

		public override async Task ResolveAsync(string uri,bool isInterceptedLink)
		{
			//Get requested assembly based on the first path segment. This is highly specific, other applications might use different strategy.
			string[] segments = new Uri(uri,UriKind.Absolute).Segments.Select(x => x.Trim('/')).Where(x => x.Length>0).ToArray();
			if (segments.Length<2)
				return;
			string assemblyName = segments[0];

			//We need to inject new assembly to the routers as those resolves which page to display.
			//Let's find them first. This way is not nice but does anybody better one? Feel free and shout.
			EventHandler<LocationChangedEventArgs> locationChangedEvent = (EventHandler<LocationChangedEventArgs>)typeof(NavigationManager).GetField("_locationChanged",BindingFlags.NonPublic|BindingFlags.Instance)
				.GetValue(_navigationManager);
			Router[] routers = locationChangedEvent.GetInvocationList().Select(x => x.Target).OfType<Router>().ToArray();

			Assembly asm=null;
			foreach (Router router in routers)
			{
				IEnumerable<Assembly> additionalAssemblies = router.AdditionalAssemblies??Enumerable.Empty<Assembly>();
				//Don't inject the assembly multiple times.
				if (additionalAssemblies.Any(x => string.Equals(x.GetName().Name,assemblyName,StringComparison.OrdinalIgnoreCase)))
					continue;

				//Load assembly and its services.
				if (asm==null)
				{
					Task<byte[]> dllBytes = _httpClient.GetByteArrayAsync($"_framework/_bin/{assemblyName}.dll");
					Task<byte[]> pdbBytes;
					try
					{
						pdbBytes=_httpClient.GetByteArrayAsync($"_framework/_bin/{assemblyName}.pdb");
					}
					catch
					{
						pdbBytes=null;
					}

					//It's needed to manage referenced assemblies somehow. How to get the list before assembly load? Can't use AppDomain.AssemblyResolve as it's sync event. Use System.Reflection.Metadata?
					asm=pdbBytes==null ? Assembly.Load(await dllBytes) : Assembly.Load(await dllBytes,await pdbBytes);
					this.LoadServices(asm);
				}

				//Inject the assembly to the router.
				ParameterView pv = ParameterView.FromDictionary(router.GetType().GetProperties()
					.Where(x => x.CustomAttributes.Any(x => x.AttributeType==typeof(ParameterAttribute)))
					.ToDictionary(pi => pi.Name,pi => string.Equals(pi.Name,nameof(Router.AdditionalAssemblies),StringComparison.Ordinal)
						? additionalAssemblies.Concat(new Assembly[] { asm }).ToArray()
						: pi.GetValue(router)));
				await router.SetParametersAsync(pv);
			}
		}
	}
}
