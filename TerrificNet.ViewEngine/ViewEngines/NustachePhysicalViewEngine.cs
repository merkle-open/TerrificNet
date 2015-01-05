using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using Nustache.Core;
using TerrificNet.ViewEngine.Cache;
using TerrificNet.ViewEngine.Helper;

namespace TerrificNet.ViewEngine.ViewEngines
{
	public class NustachePhysicalViewEngine : IViewEngine
	{
		private readonly ITemplateRepository _templateRepository;
		private readonly ICacheProvider _cacheProvider;

		public NustachePhysicalViewEngine(IModelProvider modelProvider, ITemplateRepository templateRepository, ICacheProvider cacheProvider)
		{
			_templateRepository = templateRepository;
			_cacheProvider = cacheProvider;

			//TODO refactor
			new TerrificModuleHelper(this, modelProvider, _templateRepository).Register(Helpers.Contains, Helpers.Register);
			new TerrificPlaceholderHelper(_templateRepository, this, modelProvider).Register(Helpers.Contains, Helpers.Register);
		}

		public IView CreateView(string content)
		{
			var hash = string.Concat("template_", GetHash(new MD5CryptoServiceProvider(), content));

			Template template;
			if (!_cacheProvider.TryGet(hash, out template))
			{
				template = new Template();
				template.Load(new StringReader(content));

				_cacheProvider.Set(hash, template, DateTimeOffset.Now.AddHours(24));
			}

			return new NustacheView(template, _templateRepository);
		}

		private class NustacheView : IView
		{
			private readonly Template _template;
			private readonly ITemplateRepository _templateRepository;

			public NustacheView(Template template, ITemplateRepository templateRepository)
			{
				_template = template;
				_templateRepository = templateRepository;
			}

			private Template LoadTemplate(string name)
			{
				TemplateInfo templateInfo;
				if (!_templateRepository.TryGetTemplate(name, string.Empty, out templateInfo))
					return null;

				using (var reader = new StreamReader(templateInfo.Open()))
				{
					var template = new Template(name);
					template.Load(reader);
					return template;
				}
			}

			public string Render(object model)
			{
				var builder = new StringBuilder();
				using (var writer = new StringWriter(builder))
				{
					_template.Render(model, writer, LoadTemplate);
				}

				return builder.ToString();
			}
		}

		public bool TryCreateView(TemplateInfo templateInfo, out IView view)
		{
			view = null;
			if (templateInfo == null)
				return false;

			using (var reader = new StreamReader(templateInfo.Open()))
			{
				var content = reader.ReadToEnd();

				view = CreateView(content);
				return true;
			}
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
