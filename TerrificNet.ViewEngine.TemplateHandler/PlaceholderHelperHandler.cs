using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Schema;
using Veil;
using Veil.Helper;

namespace TerrificNet.ViewEngine.TemplateHandler
{
	internal class PlaceholderHelperHandler : IHelperHandler
	{
		private readonly ITerrificTemplateHandler _handler;

		public PlaceholderHelperHandler(ITerrificTemplateHandler handler)
		{
			_handler = handler;
		}

		public bool IsSupported(string name)
		{
			return name.StartsWith("placeholder", StringComparison.OrdinalIgnoreCase);
		}

		public Task EvaluateAsync(object model, RenderingContext context, IDictionary<string, string> parameters)
		{
			var key = parameters["key"].Trim('"');
			var index = TryGetIndex(parameters, model);
			return _handler.RenderPlaceholderAsync(model, key, index, context);
		}

		public void Evaluate(object model, RenderingContext context, IDictionary<string, string> parameters)
		{
			var key = parameters["key"].Trim('"');
			var index = TryGetIndex(parameters, model);
			_handler.RenderPlaceholder(model, key, index, context);
		}

		private static int? TryGetIndex(IDictionary<string, string> parameters, object model)
		{
			string indexProperty;
			if (!parameters.TryGetValue("index", out indexProperty))
				return null;

			indexProperty = indexProperty.Trim('"');

			int? index = null;
			int indexLocal;
			if (TryGetPropValue(model, indexProperty, out indexLocal))
				index = indexLocal;

			return index;
		}

		private static bool TryGetPropValue<TValue>(object src, string propertyName, out TValue value)
		{
			value = default(TValue);

			//JObject
			var jObject = src as JObject;
			JToken jValue;
			if (jObject != null && jObject.TryGetValue(propertyName, StringComparison.InvariantCultureIgnoreCase, out jValue))
			{
				value = jValue.Value<TValue>();
				return true;
			}

			//Dictionary
			var dictionaryObject = src as IDictionary<string, object>;
			object dictionaryValue;
			if (dictionaryObject != null && dictionaryObject.TryGetValue(propertyName, out dictionaryValue) && dictionaryValue is TValue)
			{
				value = (TValue)dictionaryValue;
				return true;
			}

			var property = src.GetType().GetProperty(propertyName, BindingFlags.IgnoreCase);
			if (property != null)
			{
				value = (TValue)property.GetValue(src, null);

				return true;
			}

			return false;
		}
	}
}