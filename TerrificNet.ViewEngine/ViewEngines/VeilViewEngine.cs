using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using TerrificNet.ViewEngine.Cache;
using Veil;
using Veil.Helper;

namespace TerrificNet.ViewEngine.ViewEngines
{
    public class VeilViewEngine : IViewEngine
	{
		private readonly ICacheProvider _cacheProvider;
        private readonly ITerrificTemplateHandlerFactory _templateHandlerFactory;

        public VeilViewEngine(ICacheProvider cacheProvider, ITerrificTemplateHandlerFactory templateHandlerFactory)
		{
			_cacheProvider = cacheProvider;
	        _templateHandlerFactory = templateHandlerFactory;
		}

		public IView CreateView(string content)
		{
			var hash = string.Concat("template_", GetHash(new MD5CryptoServiceProvider(), content));

			VeilView view;
			if (!_cacheProvider.TryGet(hash, out view))
			{
			    var helperHandler = new TerrificHelperHandler(_templateHandlerFactory.Create());

				var render = new VeilEngine(helperHandler: helperHandler).CompileNonGeneric("handlebars", new StringReader(content), typeof(object));
				view = new VeilView((a, c, d) =>
				{
				    helperHandler.SetContext(d);
				    render(a, c);
				});
				_cacheProvider.Set(hash, view, DateTimeOffset.Now.AddHours(24));
			}

			return view;
		}

		private class VeilView : IView
		{
            private readonly Action<TextWriter, object, RenderingContext> _render;

			public VeilView(Action<TextWriter, object, RenderingContext> render)
			{
				_render = render;
			}

			public void Render(object model, RenderingContext context)
			{
				_render(context.Writer, model, context);
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
		    private readonly ITerrificTemplateHandler _handler;
		    private RenderingContext _context;

		    public TerrificHelperHandler(ITerrificTemplateHandler handler)
            {
                _handler = handler;
            }

		    public void SetContext(RenderingContext context)
		    {
		        _context = context;
		    }

		    public void Evaluate(object model, string name, IDictionary<string, string> parameters)
			{
				if ("module".Equals(name, StringComparison.OrdinalIgnoreCase))
				{
					var templateName = parameters["template"].Trim('"');

					var skin = string.Empty;
					if (parameters.ContainsKey("skin"))
						skin = parameters["skin"].Trim('"');

                    _handler.RenderModule(templateName, skin, _context);
				}
				else if ("placeholder".Equals(name, StringComparison.OrdinalIgnoreCase))
				{
				    var key = parameters["key"].Trim('"');
                    _handler.RenderPlaceholder(model, key, _context);
				}
                else
			        throw new NotSupportedException(string.Format("Helper with name {0} is not supported", name));
			}
		}
	}
}
