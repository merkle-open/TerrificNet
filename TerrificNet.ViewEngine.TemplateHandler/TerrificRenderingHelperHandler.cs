using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Schema;
using TerrificNet.ViewEngine.Client;
using TerrificNet.ViewEngine.Schema;
using Veil;
using Veil.Helper;

namespace TerrificNet.ViewEngine.TemplateHandler
{
	internal class TerrificRenderingHelperHandler : IHelperHandler, IHelperHandlerWithSchema, IHelperHandlerClient
    {
        private readonly ITerrificTemplateHandler _handler;
        private readonly ISchemaProvider _schemaProvider;
        private readonly ITemplateRepository _templateRepository;
		private readonly IClientTemplateGenerator _clientTemplateGenerator;

		public TerrificRenderingHelperHandler(ITerrificTemplateHandler handler, ISchemaProvider schemaProvider, ITemplateRepository templateRepository, IClientTemplateGenerator clientTemplateGenerator)
        {
            _handler = handler;
            _schemaProvider = schemaProvider;
            _templateRepository = templateRepository;
	        _clientTemplateGenerator = clientTemplateGenerator;
        }

        public bool IsSupported(string name)
        {
            return new[] { "module", "placeholder", "label", "partial" }.Any(name.StartsWith);
        }

        public Task EvaluateAsync(object model, RenderingContext context, string name, IDictionary<string, string> parameters)
        {
            if ("module".Equals(name, StringComparison.OrdinalIgnoreCase))
            {
                var templateName = parameters["template"].Trim('"');

                var skin = string.Empty;
                if (parameters.ContainsKey("skin"))
                    skin = parameters["skin"].Trim('"');

                return _handler.RenderModuleAsync(templateName, skin, context);
            }
            if ("placeholder".Equals(name, StringComparison.OrdinalIgnoreCase))
            {
                var key = parameters["key"].Trim('"');
                return _handler.RenderPlaceholderAsync(model, key, context);
            }
            if ("label".Equals(name, StringComparison.OrdinalIgnoreCase))
            {
                var key = parameters.Keys.First().Trim('"');
                return _handler.RenderLabelAsync(key, context);
            }
            if ("partial".Equals(name, StringComparison.OrdinalIgnoreCase))
            {
                var template = parameters["template"].Trim('"');
                return _handler.RenderPartialAsync(template, model, context);
            }

            throw new NotSupportedException(string.Format("Helper with name {0} is not supported", name));
        }

        public JSchema GetSchema(string name, IDictionary<string, string> parameters)
        {
            if ("partial".Equals(name, StringComparison.OrdinalIgnoreCase))
            {
                // TODO: Use async
                var templateInfo = _templateRepository.GetTemplateAsync(parameters["template"].Trim('"')).Result;
                if (templateInfo == null)
                    return null;

                // TODO: Use async
                return _schemaProvider.GetSchemaFromTemplateAsync(templateInfo).Result;
            }
            
            return null;
        }

	    public IClientModel Evaluate(IClientContext context, IClientModel model, string name, IDictionary<string, string> parameters)
		{
			if ("partial".Equals(name, StringComparison.OrdinalIgnoreCase))
			{
                // TODO: Use async
			    var templateInfo = _templateRepository.GetTemplateAsync(parameters["template"].Trim('"')).Result;
				if (templateInfo == null)
					return model;

				_clientTemplateGenerator.Generate(templateInfo, new PartialClientContextAdapter(templateInfo.Id, context), model);
			} 
            else if ("label".Equals(name, StringComparison.OrdinalIgnoreCase))
            {
                var key = parameters.Keys.First().Trim('"');
                var builder = new StringBuilder();
                using (var writer = new StringWriter(builder))
                {
                    _handler.RenderLabelAsync(key, new RenderingContext(writer));
                }
                context.WriteLiteral(builder.ToString());
            }

		    return model;
		}

	    private class PartialClientContextAdapter : IClientContext
	    {
	        private readonly IClientContext _adaptee;

	        public PartialClientContextAdapter(string templateId, IClientContext adaptee)
	        {
	            _adaptee = adaptee;
	            this.TemplateId = templateId;
	        }

	        public string TemplateId { get; private set; }

	        public void WriteLiteral(string content)
	        {
	            _adaptee.WriteLiteral(content);
	        }

	        public void WriteExpression(IClientModel model)
	        {
                _adaptee.WriteExpression(model);
	        }

	        public void WriteEncodeExpression(IClientModel model)
	        {
	            _adaptee.WriteEncodeExpression(model);
	        }

	        public IClientModel BeginIterate(IClientModel model)
	        {
	            return _adaptee.BeginIterate(model);
	        }

	        public void EndIterate()
	        {
	            _adaptee.EndIterate();
	        }

	        public void BeginIf(IClientModel model)
	        {
	            _adaptee.BeginIf(model);
	        }

	        public void EndIf()
	        {
	            _adaptee.EndIf();
	        }

	        public void ElseIf()
	        {
	            _adaptee.ElseIf();
	        }
	    }
    }
}