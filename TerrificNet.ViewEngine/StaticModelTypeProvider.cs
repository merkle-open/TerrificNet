using System;

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

        public bool TryGetModelTypeFromTemplate(TemplateInfo templateInfo, out Type type)
        {
            var schema = _schemaProvider.GetSchemaFromTemplate(templateInfo);
            var typeName = string.Format("{0}.{1}.{2},{3}", _rootNamespace, _namingRule.GetNamespaceName(schema), _namingRule.GetClassName(schema, string.Empty), _assemblyName);

            try
            {
                type = Type.GetType(typeName, true);
            }
            catch (TypeLoadException)
            {
                type = null;
                return false;
            }

            return true;
        }
    }
}