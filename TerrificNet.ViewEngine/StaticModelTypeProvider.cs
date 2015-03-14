using System;
using System.Threading.Tasks;

namespace TerrificNet.ViewEngine
{
    public class StaticModelTypeProvider : IModelTypeProvider
    {
        private readonly string _rootNamespace;
        private readonly string _assemblyName;
        private readonly INamingRule _namingRule;
        private readonly ISchemaProvider _schemaProvider;

        public StaticModelTypeProvider(string rootNamespace, string assemblyName, INamingRule namingRule, ISchemaProvider schemaProvider)
        {
            _rootNamespace = rootNamespace;
            _assemblyName = assemblyName;
            _namingRule = namingRule;
            _schemaProvider = schemaProvider;
        }

        public async Task<Type> GetModelTypeFromTemplateAsync(TemplateInfo templateInfo)
        {
            var schema = await _schemaProvider.GetSchemaFromTemplateAsync(templateInfo).ConfigureAwait(false);
            var typeName = string.Format("{0}.{1}.{2},{3}", _rootNamespace, _namingRule.GetNamespaceName(schema), _namingRule.GetClassName(schema, string.Empty), _assemblyName);

            try
            {
                return Type.GetType(typeName, true);
            }
            catch (TypeLoadException)
            {
                return null;
            }
        }
    }
}