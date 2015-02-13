using System.Collections.Generic;
using System.Net.Http;
using System.Web.Http.Routing;
using TerrificNet.ViewEngine;
using TerrificNet.ViewEngine.Config;

namespace TerrificNet
{
	internal class ValidTemplateRouteConstraint : IHttpRouteConstraint
	{
		private readonly ITemplateRepository _templateRepository;
		private readonly IFileSystem _fileSystem;
		private readonly ITerrificNetConfig _configuration;

		public ValidTemplateRouteConstraint(ITemplateRepository templateRepository, IFileSystem fileSystem,
			ITerrificNetConfig configuration)
		{
			_templateRepository = templateRepository;
			_fileSystem = fileSystem;
			_configuration = configuration;
		}

		public bool Match(HttpRequestMessage request, IHttpRoute route, string parameterName,
			IDictionary<string, object> values,
			HttpRouteDirection routeDirection)
		{
			object pathObj;
			if (values.TryGetValue(parameterName, out pathObj))
			{
				var path = pathObj as string;
				if (!string.IsNullOrEmpty(path))
				{
					TemplateInfo templateInfo;
					if (!_templateRepository.TryGetTemplate(path, out templateInfo))
					{
						var fileName = _fileSystem.Path.ChangeExtension(_fileSystem.Path.Combine(_configuration.ViewPath, path),
							"html.json");
						if (_fileSystem.FileExists(fileName))
							return true;
					}
				}
			}
			
			return false;
		}
	}
}