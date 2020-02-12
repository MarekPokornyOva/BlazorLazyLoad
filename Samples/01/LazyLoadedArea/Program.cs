#region using
using Microsoft.Extensions.DependencyInjection;
#endregion using

namespace LazyLoadedArea
{
	public class Program
	{
		public static void Main(string[] args)
		{
			//this entrypoint is requested only during project build. Otherwise, it's useless.
		}

		public static void ConfigureServices(IServiceCollection services)
		{
			services.AddSingleton<ITimeProvider,TimeProvider>();
			services.AddSingleton<IMessageProvider,CircuitMessagesProvider>();
		}
	}
}
