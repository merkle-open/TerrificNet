using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using Newtonsoft.Json.Linq;
using TerrificNet.ViewEngine.Cache;
using TerrificNet.ViewEngine.ViewEngines.TemplateHandler;
using Veil;
using Veil.Compiler;

namespace TerrificNet.ViewEngine.ViewEngines
{
	public class VeilViewEngine : IViewEngine
	{
		private readonly ICacheProvider _cacheProvider;
        private readonly ITerrificHelperHandlerFactory _helperHandlerFactory;
	    private readonly IMemberLocator _memberLocator;

		public VeilViewEngine(ICacheProvider cacheProvider,
            ITerrificHelperHandlerFactory helperHandlerFactory,
            INamingRule namingRule)
		{
			_cacheProvider = cacheProvider;
		    _helperHandlerFactory = helperHandlerFactory;
		    _memberLocator = new MemberLocatorFromNamingRule(namingRule);
		}

		private IView CreateView(string content, Type modelType)
		{
			var hash = string.Concat("template_", GetHash(new MD5CryptoServiceProvider(), content), modelType.FullName);

			IView view;
			if (!_cacheProvider.TryGet(hash, out view))
			{
			    var helperHandlers = _helperHandlerFactory.Create().ToArray();
			    var viewEngine = new VeilEngine(helperHandlers: helperHandlers, memberLocator: _memberLocator);
				if (modelType == typeof(object))
                    view = CreateNonGenericView(content, helperHandlers, viewEngine);
				else
					view = (IView)typeof(VeilViewEngine).GetMethod("CreateView", BindingFlags.NonPublic | BindingFlags.Static).MakeGenericMethod(modelType).Invoke(null, new object[] { content, helperHandlers, viewEngine });

				_cacheProvider.Set(hash, view, DateTimeOffset.Now.AddHours(24));
			}

			return view;
		}

		private static IView CreateNonGenericView(string content, ITerrificHelperHandler[] helperHandlers, IVeilEngine viewEngine)
		{
			var render = viewEngine.CompileNonGeneric("handlebars", new StringReader(content), typeof(object));
			var view = new VeilViewAdapter<object>(new VeilView<object>(render, helperHandlers));
			return view;
		}

		// do not remove, invoked dynamicaly
		// ReSharper disable once UnusedMember.Local
		private static IView CreateView<T>(string content, ITerrificHelperHandler[] helperHandlers, IVeilEngine veilEngine)
		{
			var render = veilEngine.Compile<T>("handlebars", new StringReader(content));
			return new VeilViewAdapter<T>(new VeilView<T>(render, helperHandlers));
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
			private readonly ITerrificHelperHandler[] _terrificHelpers;

			public VeilView(Action<TextWriter, T> render, ITerrificHelperHandler[] terrificHelpers)
			{
				_render = render;
				_terrificHelpers = terrificHelpers;
			}

			public void Render(T model, RenderingContext context)
			{
				foreach (var helper in _terrificHelpers)
					helper.PushContext(context);

				_render(context.Writer, model);

				foreach (var helper in _terrificHelpers)
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

		
	}

    public class MemberLocatorFromNamingRule : MemberLocator
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

    public interface ITerrificHelperHandlerFactory
    {
        IEnumerable<ITerrificHelperHandler> Create();
    }
}
