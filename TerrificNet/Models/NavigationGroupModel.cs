using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TerrificNet.ViewEngine.ViewEngines.TemplateHandler;

namespace TerrificNet.Models
{
    public class NavigationGroupModel : ViewDefinition
    {
        public List<ActionModel> Actions { get; set; }
    }

    public class ActionModel
    {
        public string Name { get; set; }
        public string Link { get; set; }
    }
}
