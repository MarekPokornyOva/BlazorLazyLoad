#region using
using Microsoft.AspNetCore.Components;
using System.Threading.Tasks;
#endregion using

namespace BlazorLazyLoad
{
	public class LazyLoadDirectHandler:IComponent
	{
		[Inject]
		private NavigationManager NavigationManager
		{
			get;
			set;
		}

		[Inject]
		private IAssemblyLazyLoadResolver AssemblyLazyLoadResolver
		{
			get;
			set;
		}

		public void Attach(RenderHandle renderHandle)
		{}

		public async Task SetParametersAsync(ParameterView parameters)
		{
			string url = new System.Uri(NavigationManager.Uri,System.UriKind.RelativeOrAbsolute).AbsoluteUri;
			await AssemblyLazyLoadResolver.ResolveAsync(url,true);
		}
	}
}
