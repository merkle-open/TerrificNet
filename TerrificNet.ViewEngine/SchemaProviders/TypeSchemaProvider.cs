using Newtonsoft.Json.Schema;

namespace TerrificNet.ViewEngine.SchemaProviders
{
    public class TypeSchemaProvider : ISchemaProvider
    {
        private readonly IModelTypeRepository _modelTypeRepository;

        public TypeSchemaProvider(IModelTypeRepository modelTypeRepository)
        {
            _modelTypeRepository = modelTypeRepository;
        }

        public JSchema GetSchemaFromTemplate(TemplateInfo template)
        {
            var type = _modelTypeRepository.GetModelTypeFromTemplate(template);
            var schemaGenerator = new JSchemaGenerator();
            return schemaGenerator.Generate(type);
        }
    }
}
