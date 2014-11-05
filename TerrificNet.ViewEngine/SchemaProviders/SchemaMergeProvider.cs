using Newtonsoft.Json.Schema;
using System.Collections.Generic;
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

        public JsonSchema GetSchemaFromPath(string path)
        {
            var comparer = new SchemaComparer();
            return comparer.Apply(_schemaProvider.GetSchemaFromPath(path), _schemaBaseProvider.GetSchemaFromPath(path), new SchemaComparisionReport());
        }
    }
}
