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
            return _handler.RenderPlaceholderAsync(model, key, context);
        }

		public void Evaluate(object model, RenderingContext context, IDictionary<string, string> parameters)
		{
			var key = parameters["key"].Trim('"');
			_handler.RenderPlaceholder(model, key, context);
		}
    }
}