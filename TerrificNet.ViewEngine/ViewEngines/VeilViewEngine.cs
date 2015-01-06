using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using Newtonsoft.Json.Linq;
using TerrificNet.ViewEngine.Cache;
using Veil;
using Veil.Helper;

namespace TerrificNet.ViewEngine.ViewEngines
{
	public class VeilViewEngine : IViewEngine
	{
		private readonly ICacheProvider _cacheProvider;
		private readonly IModelProvider _modelProvider;
		private readonly ITemplateRepository _templateRepository;

		public VeilViewEngine(ICacheProvider cacheProvider, IModelProvider modelProvider, ITemplateRepository templateRepository)
		{
			_cacheProvider = cacheProvider;
			_modelProvider = modelProvider;
			_templateRepository = templateRepository;
		}

		public IView CreateView(string content)
		{
			var hash = string.Concat("template_", GetHash(new MD5CryptoServiceProvider(), content));

			VeilView view;
			if (!_cacheProvider.TryGet(hash, out view))
			{
				var helperHandler = new TerrificHelperHandler(this, _modelProvider, _templateRepository);

				var render = new VeilEngine(helperHandler: helperHandler).CompileNonGeneric("handlebars", new StringReader(content), typeof(object));
				view = new VeilView(render);
				_cacheProvider.Set(hash, view, DateTimeOffset.Now.AddHours(24));
			}

			return view;
		}

		private class VeilView : IView
		{
			private readonly Action<TextWriter, object> _render;

			public VeilView(Action<TextWriter, object> render)
			{
				_render = render;
			}

			public string Render(object model)
			{
				var builder = new StringBuilder();
				using (var writer = new StringWriter(builder))
				{
					_render(writer, model);
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

		private class TerrificHelperHandler : IHelperHandler
		{
			private readonly IViewEngine _viewEngine;
			private readonly IModelProvider _modelProvider;
			private readonly ITemplateRepository _templateRepository;

			public TerrificHelperHandler(IViewEngine viewEngine, IModelProvider modelProvider, ITemplateRepository templateRepository)
			{
				_viewEngine = viewEngine;
				_modelProvider = modelProvider;
				_templateRepository = templateRepository;
			}

			public string Evaluate(object model, string name, IDictionary<string, string> parameters)
			{
				if ("module".Equals(name, StringComparison.OrdinalIgnoreCase))
				{
					var templateName = parameters["template"].Trim('"');

					var skin = string.Empty;
					if (parameters.ContainsKey("skin"))
						skin = parameters["skin"].Trim('"');

					TemplateInfo templateInfo;
					IView view;
					if (_templateRepository.TryGetTemplate(templateName, skin, out templateInfo) &&
						_viewEngine.TryCreateView(templateInfo, out view))
					{
						var moduleModel = _modelProvider.GetModelForTemplate(templateInfo) ?? new object();
						return view.Render(moduleModel);
					}

					return "Problem loading template " + templateName + (!string.IsNullOrEmpty(skin) ? "-" + skin : string.Empty);
				}

				if ("placeholder".Equals(name, StringComparison.OrdinalIgnoreCase))
				{
					var key = parameters["key"].Trim('"');

					var tmp = model as JObject;
					if (tmp == null)
						return string.Empty;

					var placeholder = tmp.GetValue("_placeholder") as JObject;
					if (placeholder == null)
						return string.Empty;

					var placeholderConfigs = placeholder.GetValue(key) as JArray;
					if (placeholderConfigs == null)
						return string.Empty;

					var sb = new StringBuilder();
					foreach (var placeholderConfig in placeholderConfigs)
					{
						var templateName = placeholderConfig["template"].Value<string>();

						var skin = string.Empty;
						var skinRaw = placeholderConfig["skin"];
						if (skinRaw != null)
							skin = placeholderConfig["skin"].Value<string>();

						TemplateInfo templateInfo;
						IView view;
						if (_templateRepository.TryGetTemplate(templateName, skin, out templateInfo) &&
							_viewEngine.TryCreateView(templateInfo, out view))
						{
                            var moduleModel = placeholderConfig["data"] ?? _modelProvider.GetModelForTemplate(templateInfo);
							sb.Append(view.Render(moduleModel));
						}
						else
							sb.Append("Problem loading template " + templateName + (!string.IsNullOrEmpty(skin) ? "-" + skin : string.Empty));
					}

					return sb.ToString();
				}

				throw new NotSupportedException(string.Format("Helper with name {0} is not supported", name));
			}
		}
	}
}
