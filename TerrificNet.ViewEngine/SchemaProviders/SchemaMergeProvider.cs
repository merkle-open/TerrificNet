using System.Threading.Tasks;
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

        public async Task<JSchema> GetSchemaFromTemplateAsync(TemplateInfo template)
        {
            var comparer = new SchemaComparer();
            var schema1Task = _schemaProvider.GetSchemaFromTemplateAsync(template);
            var schema2Task = _schemaBaseProvider.GetSchemaFromTemplateAsync(template);

            await Task.WhenAll(schema1Task, schema2Task).ConfigureAwait(false);

            return comparer.Apply(schema1Task.Result, schema2Task.Result, new SchemaComparisionReport());
        }
    }
}
