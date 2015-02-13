using System.IO;
using System.Linq;
using Newtonsoft.Json.Schema;
using TerrificNet.ViewEngine.Schema;
using Veil.Compiler;
using Veil.Handlebars;
using Veil.Helper;

namespace TerrificNet.ViewEngine.SchemaProviders
{
    public class HandlebarsViewSchemaProvider : ISchemaProvider
    {
        private readonly IHelperHandlerFactory _helperHandlerFactory;
        private readonly IMemberLocator _memberLocator;

        public HandlebarsViewSchemaProvider(IHelperHandlerFactory helperHandlerFactory, IMemberLocator memberLocator)
        {
            _helperHandlerFactory = helperHandlerFactory;
            _memberLocator = memberLocator;
        }

        public JsonSchema GetSchemaFromTemplate(TemplateInfo template)
        {
            var extractor = new SchemaExtractor(new HandlebarsParser());
            var helperHandlers = _helperHandlerFactory != null ? _helperHandlerFactory.Create().ToArray() : null;

            var schema = extractor.Run(new StreamReader(template.Open()), _memberLocator, helperHandlers);
            if (schema != null && string.IsNullOrEmpty(schema.Title))
                schema.Title = string.Concat(template.Id, "Model");

            return schema;
        }
    }
}
