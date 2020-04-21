#region using
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
#endregion using

namespace BlazorLazyLoad
{
	public class LazyLoadComponentPlaceHolder:ComponentBase
	{
		Type _innerComponentType;
		KeyValuePair<string,object>[] _parms;

		[Inject] IAssemblyDependencyResolver AssemblyDependencyResolver { get; set; }

		public async override Task SetParametersAsync(ParameterView parameters)
		{
			IReadOnlyDictionary<string,object> parms=parameters.ToDictionary();

			if (!(parms.TryGetValue("_Assembly",out object asmVal)&&(asmVal is string asm)&&parms.TryGetValue("_Type",out object typeVal)&&typeVal is string type))
				throw new ArgumentException("Specify _Assembly and _Type attributes as string values identifying a valid component class.");
			_parms=parms.Where(x => string.Equals(x.Key,"_Assembly",StringComparison.OrdinalIgnoreCase)==string.Equals(x.Key,"_Type",StringComparison.OrdinalIgnoreCase)).ToArray();

			_innerComponentType=await EnsureComponentAsync(asm,type);

			StateHasChanged();
		}

		protected override void BuildRenderTree(RenderTreeBuilder builder)
		{
			builder.OpenComponent(1,_innerComponentType);
			int a = 1;
			foreach (KeyValuePair<string,object> parm in _parms)
				builder.AddAttribute(++a,parm.Key,parm.Value);
			builder.CloseComponent();
		}

		async Task<Type> EnsureComponentAsync(string assemblyName,string type)
		{
			IEnumerable<Assembly> newAssemblies = (await AssemblyDependencyResolver.ResolveAsync(assemblyName));
			LoadServices(newAssemblies);
			return newAssemblies.First().GetType(type,true);
		}

		protected virtual void LoadServices(IEnumerable<Assembly> newAssemblies)
		{
			foreach (Assembly asm in newAssemblies)
				AssemblyLazyLoadResolverBase.LoadServices(asm);
		}
	}
}
