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
                .Select(CreateModuleDefinition);
        }

        private static ModuleDefinition CreateModuleDefinition(IGrouping<string, TemplateInfo> t)
        {
            var moduleId = GetModuleId(t.Key);
            var defaultTemplateCandidates = GetDefaultTemplateCandidates(moduleId);
            var defaultTemplate = t.FirstOrDefault(a => defaultTemplateCandidates.Contains(a.Id));
            return new ModuleDefinition(moduleId, defaultTemplate, t.Where(t1 => t1 != defaultTemplate).ToDictionary(GetSkinName));
        }

        private static IEnumerable<string> GetDefaultTemplateCandidates(string moduleId)
        {
            yield return string.Concat(moduleId, '/', "default");
            yield return string.Concat(moduleId, '/', Path.GetFileName(moduleId));
        }

        private static string GetModuleId(string moduleId)
        {
            return moduleId.Replace('\\', '/');
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
