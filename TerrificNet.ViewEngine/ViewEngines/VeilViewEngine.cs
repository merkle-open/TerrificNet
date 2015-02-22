using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using Newtonsoft.Json.Linq;
using TerrificNet.ViewEngine.Cache;
using Veil;
using Veil.Compiler;

namespace TerrificNet.ViewEngine.ViewEngines
{
	public class VeilViewEngine : IViewEngine
	{
		private readonly ICacheProvider _cacheProvider;
        private readonly IRenderingHelperHandlerFactory _helperHandlerFactory;
	    private readonly IMemberLocator _memberLocator;

		public VeilViewEngine(ICacheProvider cacheProvider,
            IRenderingHelperHandlerFactory helperHandlerFactory,
            INamingRule namingRule)
		{
			_cacheProvider = cacheProvider;
		    _helperHandlerFactory = helperHandlerFactory;
		    _memberLocator = new MemberLocatorFromNamingRule(namingRule);
		}

		private IView CreateView(string templateId, string content, Type modelType)
		{
			var hash = string.Concat("template_", GetHash(new MD5CryptoServiceProvider(), content), modelType.FullName);

			IView view;
			if (!_cacheProvider.TryGet(hash, out view))
			{
			    var helperHandlers = _helperHandlerFactory.Create().ToArray();
			    var viewEngine = new VeilEngine(helperHandlers: helperHandlers, memberLocator: _memberLocator);
				if (modelType == typeof(object))
                    view = CreateNonGenericView(templateId, content, helperHandlers, viewEngine);
				else
                    view = (IView)typeof(VeilViewEngine).GetMethod("CreateView", BindingFlags.NonPublic | BindingFlags.Static).MakeGenericMethod(modelType).Invoke(null, new object[] { templateId, content, helperHandlers, viewEngine });

				_cacheProvider.Set(hash, view, DateTimeOffset.Now.AddHours(24));
			}

			return view;
		}

		private static IView CreateNonGenericView(string templateId, string content, IRenderingHelperHandler[] helperHandlers, IVeilEngine viewEngine)
		{
			var render = viewEngine.CompileNonGeneric("handlebars", new StringReader(content), typeof(object));
			var view = new VeilViewAdapter<object>(templateId, new VeilView<object>(render, helperHandlers));
			return view;
		}

		// do not remove, invoked dynamicaly
		// ReSharper disable once UnusedMember.Local
		private static IView CreateView<T>(string templateId, string content, IRenderingHelperHandler[] helperHandlers, IVeilEngine veilEngine)
		{
			var render = veilEngine.Compile<T>("handlebars", new StringReader(content));
			return new VeilViewAdapter<T>(templateId, new VeilView<T>(render, helperHandlers));
		}

		private class VeilViewAdapter<T> : IView
		{
		    private readonly string _templateId;
		    private readonly IView<T> _adaptee;

			public VeilViewAdapter(string templateId, IView<T> adaptee)
			{
			    _templateId = templateId;
			    _adaptee = adaptee;
			}

		    public void Render(object model, RenderingContext context)
		    {
		        context.Data["templateId"] = _templateId;

				if (model != null)
					_adaptee.Render((T)model, context);
				else
				{
					// TODO: Verify what is to be done with null model values
					if (typeof(T) == typeof(object))
						_adaptee.Render((T)(object)new JObject(), context);
					else
						_adaptee.Render(Activator.CreateInstance<T>(), context);
				}
			}
		}

		private class VeilView<T> : IView<T>
		{
			private readonly Action<TextWriter, T> _render;
			private readonly IRenderingHelperHandler[] _renderingHelpers;

			public VeilView(Action<TextWriter, T> render, IRenderingHelperHandler[] renderingHelpers)
			{
				_render = render;
				_renderingHelpers = renderingHelpers;
			}

			public void Render(T model, RenderingContext context)
			{
				foreach (var helper in _renderingHelpers)
					helper.PushContext(context);

				_render(context.Writer, model);

				foreach (var helper in _renderingHelpers)
					helper.PopContext();
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

				view = CreateView(templateInfo.Id, content, modelType);
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
