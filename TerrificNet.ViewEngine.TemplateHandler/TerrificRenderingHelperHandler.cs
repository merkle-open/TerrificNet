using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Schema;
using TerrificNet.ViewEngine.Client;
using TerrificNet.ViewEngine.Schema;
using TerrificNet.ViewEngine.ViewEngines;

namespace TerrificNet.ViewEngine.TemplateHandler
{
	internal class TerrificRenderingHelperHandler : IRenderingHelperHandler, IHelperHandlerWithSchema, IHelperHandlerClient
    {
        private readonly ITerrificTemplateHandler _handler;
        private readonly ISchemaProvider _schemaProvider;
        private readonly ITemplateRepository _templateRepository;
		private readonly IClientTemplateGenerator _clientTemplateGenerator;
		private readonly Stack<RenderingContext> _contextStack = new Stack<RenderingContext>();

		public TerrificRenderingHelperHandler(ITerrificTemplateHandler handler, ISchemaProvider schemaProvider, ITemplateRepository templateRepository, IClientTemplateGenerator clientTemplateGenerator)
        {
            _handler = handler;
            _schemaProvider = schemaProvider;
            _templateRepository = templateRepository;
	        _clientTemplateGenerator = clientTemplateGenerator;
        }

        public void PushContext(RenderingContext context)
        {
            _contextStack.Push(context);
        }

        public void PopContext()
        {
            _contextStack.Pop();
        }

        public bool IsSupported(string name)
        {
            return new[] { "module", "placeholder", "label", "partial" }.Any(name.StartsWith);
        }

        private RenderingContext Context { get { return _contextStack.Peek(); } }

        public void Evaluate(object model, string name, IDictionary<string, string> parameters)
        {
            if ("module".Equals(name, StringComparison.OrdinalIgnoreCase))
            {
                var templateName = parameters["template"].Trim('"');

                var skin = string.Empty;
                if (parameters.ContainsKey("skin"))
                    skin = parameters["skin"].Trim('"');

                _handler.RenderModule(templateName, skin, Context);
            }
            else if ("placeholder".Equals(name, StringComparison.OrdinalIgnoreCase))
            {
                var key = parameters["key"].Trim('"');
                _handler.RenderPlaceholder(model, key, Context);
            }
            else if ("label".Equals(name, StringComparison.OrdinalIgnoreCase))
            {
                var key = parameters.Keys.First().Trim('"');
                _handler.RenderLabel(key, Context);
            }
            else if ("partial".Equals(name, StringComparison.OrdinalIgnoreCase))
            {
                var template = parameters["template"].Trim('"');
                _handler.RenderPartial(template, model, Context);
            }
            else
                throw new NotSupportedException(string.Format("Helper with name {0} is not supported", name));
        }

        public JSchema GetSchema(string name, IDictionary<string, string> parameters)
        {
            if ("partial".Equals(name, StringComparison.OrdinalIgnoreCase))
            {
                TemplateInfo templateInfo;
                if (!_templateRepository.TryGetTemplate(parameters["template"].Trim('"'), out templateInfo))
                    return null;

                return _schemaProvider.GetSchemaFromTemplate(templateInfo);
            }
            else if ("placeholder".Equals(name, StringComparison.OrdinalIgnoreCase))
            {
                var placeholderSchema = new JSchema();
                var itemSchema = new JSchema
                {
                    Type = JSchemaType.Array
                };
                itemSchema.Items.Add(GetViewSchema());
                placeholderSchema.Properties.Add(parameters["key"].Trim('"'), itemSchema);

                var schema = new JSchema();
                schema.Properties.Add("_placeholders", placeholderSchema);
                
                return schema;
            }
            return null;
        }

        private static JSchema GetViewSchema()
        {
            //var schema = new JSchema();
            //schema.OneOf.Add(GetPartialViewSchema());
            //schema.OneOf.Add(GetModuleViewSchema());

            //return schema;
            return GetPartialViewSchema();
        }

        private static JSchema GetModuleViewSchema()
        {
            const string schema = @"{
                ""type"":
                ""object"",
                ""properties"":
                {
                    ""module"":
                    {
                        ""type"": [""string"", ""null""],
                        ""format"": ""template""
                    }
                ,
                    ""skin"":
                    {
                        ""type"": [""string"", ""null""]
                    }
                ,
                    ""data_variation"":
                    {
                        ""type"": [""string"", ""null""]
                    }
                }
            }";

            return JsonConvert.DeserializeObject<JSchema>(schema);
        }

        private static JSchema GetPartialViewSchema()
        {
            const string schema = @"{
                ""type"":
                ""object"",
                ""properties"":
                {
                    ""template"":
                    {
                        ""type"": ""string"",
                        ""format"": ""template""
                    }
                }
            }";

            return JsonConvert.DeserializeObject<JSchema>(schema);
        }

		public IClientModel Evaluate(IClientContext context, IClientModel model, string name, IDictionary<string, string> parameters)
		{
			if ("partial".Equals(name, StringComparison.OrdinalIgnoreCase))
			{
				TemplateInfo templateInfo;
				if (!_templateRepository.TryGetTemplate(parameters["template"].Trim('"'), out templateInfo))
					return model;

				_clientTemplateGenerator.Generate(templateInfo, new PartialClientContextAdapter(templateInfo.Id, context), model);
			} 
            else if ("label".Equals(name, StringComparison.OrdinalIgnoreCase))
            {
                var key = parameters.Keys.First().Trim('"');
                var builder = new StringBuilder();
                using (var writer = new StringWriter(builder))
                {
                    _handler.RenderLabel(key, new RenderingContext(writer));
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