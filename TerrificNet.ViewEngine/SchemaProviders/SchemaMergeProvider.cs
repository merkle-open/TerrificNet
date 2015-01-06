using Newtonsoft.Json.Schema;
using TerrificNet.ViewEngine.Schema;

namespace TerrificNet.ViewEngine.SchemaProviders
{
    public class SchemaMergeProvider : ISchemaProvider
    {
        private readonly ISchemaProvider _schemaProvider;
        private readonly ISchemaProvider _schemaBaseProvider;

        public SchemaMergeProvider(ISchemaProvider schemaProvider, ISchemaProvider schemaBaseProvider)
        {
            _schemaProvider = schemaProvider;
            _schemaBaseProvider = schemaBaseProvider;
        }

        public JsonSchema GetSchemaFromTemplate(TemplateInfo template)
        {
            var comparer = new SchemaComparer();
            return comparer.Apply(_schemaProvider.GetSchemaFromTemplate(template), _schemaBaseProvider.GetSchemaFromTemplate(template), new SchemaComparisionReport());
        }
    }
}
