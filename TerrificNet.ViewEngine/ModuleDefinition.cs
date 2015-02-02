using System.Collections.Generic;

namespace TerrificNet.ViewEngine
{
    public class ModuleDefinition
    {
        public string Id { get; private set; }

        public ModuleDefinition(string id, TemplateInfo defaultTemplate, IReadOnlyDictionary<string, TemplateInfo> skins)
        {
            Id = id;
            DefaultTemplate = defaultTemplate;
            Skins = skins;
        }

        public TemplateInfo DefaultTemplate { get; private set; }

        public IReadOnlyDictionary<string, TemplateInfo> Skins { get; private set; }
    }
}
