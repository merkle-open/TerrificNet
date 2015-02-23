using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Web.Http;
using TerrificNet.Configuration;
using TerrificNet.ViewEngine;
using TerrificNet.ViewEngine.IO;

namespace TerrificNet.Controllers
{
    public class StaticFileController : ApiController
	{
        private readonly IFileSystem _fileSystem;
	    private readonly ServerConfiguration _serverConfiguration;

	    public StaticFileController(IFileSystem fileSystem, ServerConfiguration serverConfiguration)
        {
	        _fileSystem = fileSystem;
	        _serverConfiguration = serverConfiguration;
        }

	    public virtual HttpResponseMessage Get(string path)
	    {
		    return GetInternal(path);
	    }

		protected virtual string FilePath { get { return null; } }

	    protected HttpResponseMessage GetInternal(string path)
	    {
            var filePath = _fileSystem.Path.Combine(this.FilePath, path);
            if (!_fileSystem.FileExists(filePath))
                return new HttpResponseMessage(HttpStatusCode.NotFound);

            var stream = _fileSystem.OpenRead(filePath);
            var message = new HttpResponseMessage(HttpStatusCode.OK) { Content = new StreamContent(stream) };
            message.Content.Headers.ContentType = new MediaTypeHeaderValue(GetMimeType(_fileSystem.Path.GetExtension(path).ToLower()));
            return message;
	    }

		private string GetMimeType(string extension)
		{
			if (extension == null)
			{
				throw new ArgumentNullException("extension");
			}

			if (!extension.StartsWith("."))
			{
				extension = "." + extension;
			}

			string mime;
			return _serverConfiguration.MimeTypes.TryGetValue(extension, out mime) ? mime : "application/octet-stream";
		}
	}
}