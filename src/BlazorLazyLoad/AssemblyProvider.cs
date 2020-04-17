#region using
using System.IO;
using System.IO.Compression;
using System.Net.Http;
using System.Threading.Tasks;
#endregion using

namespace BlazorLazyLoad
{
	public interface IAssemblyProvider
	{
		Task<(byte[] DllBytes, byte[] PdbBytes)> GetAssemblyAsync(string assemblyName);
	}

	#region AssemblyProviderBase
	public abstract class AssemblyProviderBase:IAssemblyProvider
	{
		readonly HttpClient _httpClient;
		public AssemblyProviderBase(HttpClient httpClient)
			=> _httpClient=httpClient;

		public abstract Task<(byte[] DllBytes, byte[] PdbBytes)> GetAssemblyAsync(string assemblyName);

		public Task<byte[]> DownloadFileBytes(string filename)
			=> _httpClient.GetByteArrayAsync("_framework/_bin/"+filename);

		public async Task<byte[]> TryDownloadFileBytes(string filename)
		{
			try
			{
				return await DownloadFileBytes(filename);
			}
			catch
			{ }
			return null;
		}
	}
	#endregion AssemblyProviderBase

	public class DefaultAssemblyDownloader:AssemblyProviderBase
	{
		public DefaultAssemblyDownloader(HttpClient httpClient) : base(httpClient)
		{ }

		public override async Task<(byte[] DllBytes, byte[] PdbBytes)> GetAssemblyAsync(string assemblyName)
		{
			//Ungzip works with streams but it's still worth to download data as byte[] because HttpClient do various memory optimizations and content length checks when downloading as byte[].
			Task<byte[]> dllBytes = Ungzip(DownloadFileBytes(assemblyName+".dll.gz"));
			return (await dllBytes, await Ungzip(TryDownloadFileBytes(assemblyName+".pdb.gz")));
		}

		async Task<byte[]> Ungzip(Task<byte[]> source)
		{
			byte[] gzipBytes = await source;
			if (gzipBytes==null)
				return null;
			using (Stream gzipStream = new MemoryStream(gzipBytes))
			using (GZipStream decompressStream = new GZipStream(gzipStream,CompressionMode.Decompress))
			using (MemoryStream rawStream = new MemoryStream())
			{
				decompressStream.CopyTo(rawStream);
				return rawStream.GetBuffer();
			}
		}
	}

	public class NonGZippedAssemblyDownloader:AssemblyProviderBase
	{
		public NonGZippedAssemblyDownloader(HttpClient httpClient) : base(httpClient)
		{ }

		public override async Task<(byte[] DllBytes, byte[] PdbBytes)> GetAssemblyAsync(string assemblyName)
		{
			Task<byte[]> dllBytes = DownloadFileBytes(assemblyName+".dll");
			return (await dllBytes, await TryDownloadFileBytes(assemblyName+".pdb"));
		}
	}
}
