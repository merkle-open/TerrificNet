using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json.Schema;
using TerrificNet.ViewEngine.Schema;
using TerrificNet.ViewEngine.ViewEngines.TemplateHandler;

namespace TerrificNet.ViewEngine.ViewEngines
{
    internal class TerrificHelperHandler : ITerrificHelperHandler, IHelperHandlerWithSchema
    {
        private readonly ITerrificTemplateHandler _handler;
        private readonly ISchemaProvider _schemaProvider;
        private readonly ITemplateRepository _templateRepository;
        private readonly Stack<RenderingContext> _contextStack = new Stack<RenderingContext>();

        public TerrificHelperHandler(ITerrificTemplateHandler handler, ISchemaProvider schemaProvider, ITemplateRepository templateRepository)
        {
            _handler = handler;
            _schemaProvider = schemaProvider;
            _templateRepository = templateRepository;
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
                placeholderSchema.Items.Add(GetViewSchema());
                placeholderSchema.Properties.Add(parameters["key"].Trim('"'), itemSchema);

                var schema = new JSchema();
                schema.Properties.Add("_placeholders", placeholderSchema);
                
                return schema;
            }
            return null;
        }

        private static JSchema GetViewSchema()
        {
            var schema = new JSchema();
            //schema.OneOf

            return schema;
        }
    }
}