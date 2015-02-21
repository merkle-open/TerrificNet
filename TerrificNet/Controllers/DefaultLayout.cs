using System.Collections.Generic;
using TerrificNet.Models;
using TerrificNet.ViewEngine.TemplateHandler;

namespace TerrificNet.Controllers
{
    static internal class DefaultLayout
    {
        public static PageViewDefinition WithDefaultLayout(params ViewDefinition[] content)
        {
            var viewDefinition = new PageViewDefinition
            {
                Template = "views/_layouts/_layout",
                Placeholder = new PlaceholderDefinitionCollection
                {
                    {
                        "content", new ViewDefinition[]
                        {
                            new PartialViewDefinition
                            {
                                Template = "views/_layouts/page",
                                Placeholder = new PlaceholderDefinitionCollection
                                {
                                    {
                                        "phContent", content
                                    }
                                },
                                Data = new NavigationGroupModel()
                            }
                        }
                    }
                }
            };
            return viewDefinition;
        }

        public static ViewDefinition AddAction(this ViewDefinition viewDefinition, ActionModel actionModel)
        {
            var model = (NavigationGroupModel)((PartialViewDefinition)viewDefinition.Placeholder["content"][0]).Data;
            if (model.Actions == null)
                model.Actions = new List<ActionModel>();

            model.Actions.Add(actionModel);

            return viewDefinition;
        }

        public static PageViewDefinition IncludeScript(this PageViewDefinition viewDefintion, string scriptSource)
        {
            var script = new ScriptImport {Src = scriptSource};

            if (viewDefintion.Scripts == null)
                viewDefintion.Scripts = new List<ScriptImport>();

            viewDefintion.Scripts.Add(script);

            return viewDefintion;
        }

        public static PageViewDefinition IncludeStyle(this PageViewDefinition viewDefinition, string styleSource)
        {
            var script = new StyleImport { Href = styleSource };

            if (viewDefinition.Styles == null)
                viewDefinition.Styles = new List<StyleImport>();

            viewDefinition.Styles.Add(script);

            return viewDefinition;
        }
    }
}