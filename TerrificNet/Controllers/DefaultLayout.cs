using System.Collections.Generic;
using TerrificNet.Models;
using TerrificNet.ViewEngine.TemplateHandler.UI;

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
                            new PartialViewDefinition<NavigationGroupModel>
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
            var model = (NavigationGroupModel)((IPartialViewDefinition)viewDefinition.Placeholder["content"][0]).Data;
            if (model.Actions == null)
                model.Actions = new List<ActionModel>();

            model.Actions.Add(actionModel);

            return viewDefinition;
        }

        public static PageViewDefinition IncludeScript(this PageViewDefinition viewDefintion, string scriptSource)
        {
            var script = new ScriptImport {Src = scriptSource};

            EnsureData(viewDefintion);

            if (viewDefintion.Data.Scripts == null)
                viewDefintion.Data.Scripts = new List<ScriptImport>();

            viewDefintion.Data.Scripts.Add(script);

            return viewDefintion;
        }

        private static void EnsureData(PageViewDefinition viewDefintion)
        {
            if (viewDefintion.Data == null)
                viewDefintion.Data = new PageViewData();
        }

        public static PageViewDefinition IncludeStyle(this PageViewDefinition viewDefinition, string styleSource)
        {
            var script = new StyleImport { Href = styleSource };

            EnsureData(viewDefinition);

            if (viewDefinition.Data.Styles == null)
                viewDefinition.Data.Styles = new List<StyleImport>();

            viewDefinition.Data.Styles.Add(script);

            return viewDefinition;
        }
    }
}