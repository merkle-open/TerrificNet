using System;
using System.Collections.Generic;

namespace TerrificNet.ViewEngine.SchemaProviders
{
    public interface IModelTypeRepository
    {
        Type GetModelTypeFromTemplate(TemplateInfo template);
    }

    public class DefaultModelTypeRepository : IModelTypeRepository
    {
        private readonly Dictionary<string, Type> _repository = new Dictionary<string, Type>(); 

        public void Register(Type type, TemplateInfo template)
        {
            _repository.Add(template.Id, type);
        }

        public Type GetModelTypeFromTemplate(TemplateInfo template)
        {
            Type type;
            if (!_repository.TryGetValue(template.Id, out type))
                throw new KeyNotFoundException(string.Format("No type for template with id '{0}' found", template.Id));

            return type;
        }
    }
}