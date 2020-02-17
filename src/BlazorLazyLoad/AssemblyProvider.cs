#region using
using System.Net.Http;
using System.Threading.Tasks;
#endregion using

namespace BlazorLazyLoad
{
	public interface IAssemblyProvider
	{
		Task<(byte[] DllBytes, byte[] PdbBytes)> GetAssemblyAsync(string assemblyName);
	}

	public class DefaultAssemblyDownloader:IAssemblyProvider
	{
		readonly HttpClient _httpClient;
		public DefaultAssemblyDownloader(HttpClient httpClient)
			=> _httpClient=httpClient;

		public async Task<(byte[] DllBytes, byte[] PdbBytes)> GetAssemblyAsync(string assemblyName)
		{
			Task<byte[]> dllBytes = _httpClient.GetByteArrayAsync($"_framework/_bin/{assemblyName}.dll");
			byte[] pdbBytes;
			try
			{
				pdbBytes=await _httpClient.GetByteArrayAsync($"_framework/_bin/{assemblyName}.pdb");
			}
			catch
			{
				pdbBytes=null;
			}

			return (await dllBytes, pdbBytes);
		}
	}
}
