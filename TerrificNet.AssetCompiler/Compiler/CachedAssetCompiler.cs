using System;
using System.Runtime.Caching;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace TerrificNet.AssetCompiler.Compiler
{
	public class CachedAssetCompiler : IAssetCompiler
	{
		private readonly IAssetCompiler _compiler;
		private readonly MemoryCache _cache = MemoryCache.Default;

		public CachedAssetCompiler(IAssetCompiler compiler)
		{
			_compiler = compiler;
		}

		public async Task<string> CompileAsync(string content, bool minify)
		{
			var hash = GetHash(new MD5CryptoServiceProvider(), content);

			var cacheContent = _cache.Get(hash) as string;
			if (cacheContent == null)
			{
                cacheContent = await _compiler.CompileAsync(content, minify);
				_cache.Set(hash, cacheContent, new CacheItemPolicy
				{
					Priority = CacheItemPriority.NotRemovable,
					AbsoluteExpiration = DateTimeOffset.Now.AddMinutes(5)
				});
			}

			return cacheContent;
		}

		public bool CanProcess(string filename)
		{
			return _compiler.CanProcess(filename);
		}

		public string MimeType
		{
			get { return _compiler.MimeType; }
		}

		private static string GetHash(HashAlgorithm md5Hash, string input)
		{

			// Convert the input string to a byte array and compute the hash. 
			var data = md5Hash.ComputeHash(Encoding.UTF8.GetBytes(input));

			// Create a new Stringbuilder to collect the bytes 
			// and create a string.
			var sBuilder = new StringBuilder();

			// Loop through each byte of the hashed data  
			// and format each one as a hexadecimal string. 
			for (var i = 0; i < data.Length; i++)
			{
				sBuilder.Append(data[i].ToString("x2"));
			}

			// Return the hexadecimal string. 
			return sBuilder.ToString();
		}
	}
}