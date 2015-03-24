using System.IO;
using System.Linq;
using System.Threading.Tasks;
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

        public Task<JSchema> GetSchemaFromTemplateAsync(TemplateInfo template)
        {
            var extractor = new SchemaExtractor(new HandlebarsParser());
            var helperHandlers = _helperHandlerFactory != null ? _helperHandlerFactory.Create().ToArray() : null;

            // TODO: Use async
            var schema = extractor.Run(template.Id, new StreamReader(template.Open()), _memberLocator, helperHandlers);
            if (schema != null && string.IsNullOrEmpty(schema.Title))
                schema.Title = string.Concat(template.Id, "Model");

            return Task.FromResult(schema);
        }
    }
}
