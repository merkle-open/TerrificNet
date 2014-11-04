using Newtonsoft.Json.Schema;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TerrificNet.ViewEngine.Schema.Test;

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
            return comparer.Apply(_schemaProvider.GetSchemaFromPath(path), _schemaBaseProvider.GetSchemaFromPath(path), new List<SchemaComparisonFailure>());
        }
    }
}
