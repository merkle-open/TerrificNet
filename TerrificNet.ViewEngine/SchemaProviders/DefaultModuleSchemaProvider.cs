using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json.Schema;
using TerrificNet.ViewEngine.Schema;

namespace TerrificNet.ViewEngine.SchemaProviders
{
    public class DefaultModuleSchemaProvider : IModuleSchemaProvider
    {
        private readonly SchemaCombiner _schemaCombiner;
        private readonly ISchemaProvider _schemaProvider;

        public DefaultModuleSchemaProvider(ISchemaProvider schemaProvider) : this(new SchemaCombiner(), schemaProvider)
        {
        }

        internal DefaultModuleSchemaProvider(SchemaCombiner schemaCombiner, ISchemaProvider schemaProvider)
        {
            _schemaCombiner = schemaCombiner;
            _schemaProvider = schemaProvider;
        }

        public JsonSchema GetSchemaFromModule(ModuleDefinition module)
        {
            IEnumerable<TemplateInfo> templates;
            if (module.Skins != null)
                templates = module.Skins.Values;
            else
                templates = Enumerable.Empty<TemplateInfo>();

            if (module.DefaultTemplate != null)
                templates = templates.Union(new[] {module.DefaultTemplate});

            var enumerator = templates.GetEnumerator();
            if (!enumerator.MoveNext())
                return null;

            var result = _schemaProvider.GetSchemaFromTemplate(enumerator.Current);

            var report = new SchemaComparisionReport();
            while (enumerator.MoveNext())
                result = _schemaCombiner.Apply(result, _schemaProvider.GetSchemaFromTemplate(enumerator.Current), report);

            return result;
        }
    }
}
