using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using TerrificNet.ViewEngine.Client;
using Veil;
using Veil.Helper;

namespace TerrificNet.ViewEngine.TemplateHandler
{
    internal class LabelHelperHandler : IHelperHandler, IHelperHandlerClient
    {
        private readonly ITerrificTemplateHandler _handler;

        public LabelHelperHandler(ITerrificTemplateHandler handler)
        {
            _handler = handler;
        }

        public bool IsSupported(string name)
        {
            return name.StartsWith("label", StringComparison.OrdinalIgnoreCase);
        }

		public void Evaluate(object model, RenderingContext context, IDictionary<string, string> parameters)
		{
			var key = parameters.Keys.First().Trim('"');
			_handler.RenderLabel(key, context);
		}

        public IClientModel Evaluate(IClientContext context, IClientModel model, string name, IDictionary<string, string> parameters)
        {
            var key = parameters.Keys.First().Trim('"');
            var builder = new StringBuilder();
            using (var writer = new StringWriter(builder))
            {
                _handler.RenderLabel(key, new RenderingContext(writer));
            }

            context.WriteLiteral(builder.ToString());
            return model;
        }
    }
}