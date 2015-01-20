using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using TerrificNet.ViewEngine.Cache;
using Veil;
using Veil.Compiler;
using Veil.Helper;

namespace TerrificNet.ViewEngine.ViewEngines
{
    public class VeilViewEngine : IViewEngine
	{
		private readonly ICacheProvider _cacheProvider;
        private readonly ITerrificTemplateHandlerFactory _templateHandlerFactory;
        private readonly IMemberLocator _memberLocator;

        public VeilViewEngine(ICacheProvider cacheProvider, ITerrificTemplateHandlerFactory templateHandlerFactory, INamingRule namingRule)
		{
			_cacheProvider = cacheProvider;
	        _templateHandlerFactory = templateHandlerFactory;
            _memberLocator = new MemberLocatorFromNamingRule(namingRule);
		}

        private IView CreateView(string content, Type modelType)
		{
			var hash = string.Concat("template_", GetHash(new MD5CryptoServiceProvider(), content), modelType.FullName);

			IView view;
			if (!_cacheProvider.TryGet(hash, out view))
			{
			    var helperHandler = new TerrificHelperHandler(_templateHandlerFactory.Create());

                var viewEngine = new VeilEngine(helperHandler: helperHandler, memberLocator: _memberLocator);
			    if (modelType == typeof (object))
			        view = CreateNonGenericView(content, helperHandler, viewEngine);
			    else
                    view = (IView)typeof(VeilViewEngine).GetMethod("CreateView", BindingFlags.NonPublic | BindingFlags.Static).MakeGenericMethod(modelType).Invoke(null, new object[] { content, helperHandler, viewEngine });

			    _cacheProvider.Set(hash, view, DateTimeOffset.Now.AddHours(24));
			}

			return view;
		}

        private static IView CreateNonGenericView(string content, TerrificHelperHandler helperHandler, IVeilEngine viewEngine)
        {
            var render = viewEngine.CompileNonGeneric("handlebars", new StringReader(content), typeof (object));
            var view = new VeilViewAdapter<object>(new VeilView<object>(render, helperHandler));
            return view;
        }

        private static IView CreateView<T>(string content, TerrificHelperHandler helperHandler, IVeilEngine veilEngine)
        {
            var render = veilEngine.Compile<T>("handlebars", new StringReader(content));
            return new VeilViewAdapter<T>(new VeilView<T>(render, helperHandler));
        }

        private class VeilViewAdapter<T> : IView
        {
            private readonly IView<T> _adaptee;

            public VeilViewAdapter(IView<T> adaptee)
            {
                _adaptee = adaptee;
            }

            public void Render(object model, RenderingContext context)
            {
                if (model != null)
                    _adaptee.Render((T) model, context);
                else
                    // TODO: Verify what is to be done with null model values
                    _adaptee.Render(Activator.CreateInstance<T>(), context);
            }
        }

        private class VeilView<T> : IView<T>
		{
            private readonly Action<TextWriter, T> _render;
		    private readonly TerrificHelperHandler _terrificHelper;

		    public VeilView(Action<TextWriter, T> render, TerrificHelperHandler terrificHelper)
			{
			    _render = render;
			    _terrificHelper = terrificHelper;
			}

		    public void Render(T model, RenderingContext context)
		    {
		        _terrificHelper.SetContext(context);
                _render(context.Writer, model);
		    }
		}

		public bool TryCreateView(TemplateInfo templateInfo, Type modelType, out IView view)
		{
			view = null;
			if (templateInfo == null)
				return false;

			using (var reader = new StreamReader(templateInfo.Open()))
			{
				var content = reader.ReadToEnd();

				view = CreateView(content, modelType);
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

        private class MemberLocatorFromNamingRule : MemberLocator
        {
            private readonly INamingRule _namingRule;

            public MemberLocatorFromNamingRule(INamingRule namingRule)
            {
                _namingRule = namingRule;
            }

            public override MemberInfo FindMember(Type modelType, string name, MemberTypes types)
            {
                name = _namingRule.GetPropertyName(name);
                return base.FindMember(modelType, name, types);
            }
            
        }
	}
}
