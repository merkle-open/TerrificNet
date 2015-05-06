using System;
using System.Collections.Generic;
using System.Threading.Tasks;
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

			int? index = null;
			var indexLocal = GetPropValue(model, indexProperty) as int?;
			if (indexLocal != null)
				index = indexLocal;
			return index;
		}

		private static object GetPropValue(object src, string propName)
		{
			return src.GetType().GetProperty(propName).GetValue(src, null);
		}
    }
}