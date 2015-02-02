using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TerrificNet.ViewEngine.Config;

namespace TerrificNet.ViewEngine
{
    public class DefaultModuleRepository : IModuleRepository
    {
        private readonly ITerrificNetConfig _configuration;
        private readonly ITemplateRepository _templateRepository;

        public DefaultModuleRepository(ITerrificNetConfig configuration, ITemplateRepository templateRepository)
        {
            _configuration = configuration;
            _templateRepository = templateRepository;
        }

        public IEnumerable<ModuleDefinition> GetAll()
        {
            return _templateRepository.GetAll()
                .Where(t => t.Id.StartsWith(_configuration.ModulePath))
                .GroupBy(t => Path.GetDirectoryName(t.Id))
                .Select(t => new ModuleDefinition(t.Key, t.ToDictionary(GetSkinName)));
        }

        private static string GetSkinName(TemplateInfo templateInfo)
        {
            var parts = templateInfo.Id.Split('-');
            if (parts.Length > 1)
                return parts[parts.Length - 1];

            return templateInfo.Id;
        }

        public bool TryGetModuleDefinitionById(string id, out ModuleDefinition moduleDefinition)
        {
            throw new NotImplementedException();
        }
    }
}
