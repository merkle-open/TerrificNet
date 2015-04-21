using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using TerrificNet.ViewEngine.Cache;
using Veil;
using Veil.Compiler;
using Veil.Handlebars;
using Veil.Helper;

namespace TerrificNet.ViewEngine.ViewEngines
{
	public class VeilViewEngine : IViewEngine
	{
		private readonly ICacheProvider _cacheProvider;
		private readonly IHelperHandlerFactory _helperHandlerFactory;
		private readonly IMemberLocator _memberLocator;

		public VeilViewEngine(ICacheProvider cacheProvider,
			IHelperHandlerFactory helperHandlerFactory,
			INamingRule namingRule)
		{
			_cacheProvider = cacheProvider;
			_helperHandlerFactory = helperHandlerFactory;
			_memberLocator = new MemberLocatorFromNamingRule(namingRule);
		}

		private async Task<IView> CreateView(TemplateInfo templateInfo, Type modelType)
		{
			var hash = string.Concat("template_", templateInfo.Id, templateInfo.ETag, modelType.FullName);

			IView view;
			if (!_cacheProvider.TryGet(hash, out view))
			{
				string content;
				using (var reader = new StreamReader(templateInfo.Open()))
				{
					content = await reader.ReadToEndAsync().ConfigureAwait(false);
				}

				var helperHandlers = _helperHandlerFactory.Create().ToArray();
				var viewEngine = new VeilEngine(helperHandlers, _memberLocator);
				if (modelType == typeof(object))
					view = CreateNonGenericView(templateInfo.Id, content, viewEngine);
				else
					view = (IView)typeof(VeilViewEngine).GetMethod("CreateView", BindingFlags.NonPublic | BindingFlags.Static).MakeGenericMethod(modelType).Invoke(null, new object[] { templateInfo.Id, content, viewEngine });

				_cacheProvider.Set(hash, view, DateTimeOffset.Now.AddHours(24));
			}

			return view;
		}

		private static IView CreateNonGenericView(string templateId, string content, IVeilEngine viewEngine)
		{
			var renderAsync = viewEngine.CompileNonGenericAsync(templateId, new HandlebarsParser(), new StringReader(content), typeof(object));
			var render = viewEngine.CompileNonGeneric(templateId, new HandlebarsParser(), new StringReader(content), typeof(object));
			var view = new VeilViewAdapter<object>(templateId, new VeilView<object>(renderAsync, render));
			return view;
		}

		// do not remove, invoked dynamicaly
		// ReSharper disable once UnusedMember.Local
		private static IView CreateView<T>(string templateId, string content, IVeilEngine veilEngine)
		{
			var renderAsync = veilEngine.CompileAsync<T>(templateId, new HandlebarsParser(), new StringReader(content));
			var render = veilEngine.Compile<T>(templateId, new HandlebarsParser(), new StringReader(content));
			return new VeilViewAdapter<T>(templateId, new VeilView<T>(renderAsync, render));
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

			public Task RenderAsync(object model, RenderingContext context)
			{
				context.Data["templateId"] = _templateId;

				var castModel = model is T ? (T)model : default(T);
				if (castModel != null)
					return _adaptee.RenderAsync(castModel, context);

				// TODO: Verify what is to be done with null model values
				if (typeof(T) == typeof(object))
					return _adaptee.RenderAsync((T)(object)new JObject(), context);

				return _adaptee.RenderAsync(Activator.CreateInstance<T>(), context);
			}

			public void Render(object model, RenderingContext context)
			{
				context.Data["templateId"] = _templateId;

				var castModel = model is T ? (T)model : default(T);
				if (castModel != null)
				{
					_adaptee.Render(castModel, context);
					return;
				}

				// TODO: Verify what is to be done with null model values
				if (typeof(T) == typeof(object))
				{
					_adaptee.Render((T)(object)new JObject(), context);
					return;
				}

				_adaptee.Render(Activator.CreateInstance<T>(), context);
			}
		}

		private class VeilView<T> : IView<T>
		{
			private readonly Func<RenderingContext, T, Task> _renderAsync;
			private readonly Action<RenderingContext, T> _render;

			public VeilView(Func<RenderingContext, T, Task> renderAsync, Action<RenderingContext, T> render )
			{
				_renderAsync = renderAsync;
				_render = render;
			}

			public Task RenderAsync(T model, RenderingContext context)
			{
				return _renderAsync(context, model);
			}

			public void Render(T model, RenderingContext context)
			{
				_render(context, model);
			}
		}

		public Task<IView> CreateViewAsync(TemplateInfo templateInfo, Type modelType)
		{
			if (templateInfo == null)
				return null;

			return CreateView(templateInfo, modelType);
		}
	}
}
