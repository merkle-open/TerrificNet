using System.Threading.Tasks;
using Newtonsoft.Json.Schema;
using Newtonsoft.Json.Schema.Generation;

namespace TerrificNet.ViewEngine.SchemaProviders
{
    public class TypeSchemaProvider : ISchemaProvider
    {
        private readonly IModelTypeRepository _modelTypeRepository;

        public TypeSchemaProvider(IModelTypeRepository modelTypeRepository)
        {
            _modelTypeRepository = modelTypeRepository;
        }

        public Task<JSchema> GetSchemaFromTemplateAsync(TemplateInfo template)
        {
            // TODO: use async
            var type = _modelTypeRepository.GetModelTypeFromTemplate(template);
            var schemaGenerator = new JSchemaGenerator();
            return Task.FromResult(schemaGenerator.Generate(type));
        }
    }
}
