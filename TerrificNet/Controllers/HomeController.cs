using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Web.Http;
using TerrificNet.Models;
using TerrificNet.ViewEngine.ViewEngines.TemplateHandler;

namespace TerrificNet.Controllers
{
    public class HomeController : TemplateControllerBase
    {
        [HttpGet]
        public HttpResponseMessage Index()
        {
            var viewDefinition = WithDefaultLayout(new ViewDefinition
            {
                Template = "components/modules/ApplicationOverview/ApplicationOverview"
            });

            return View(viewDefinition.Template, viewDefinition);
        }

        private static ViewDefinition WithDefaultLayout(params ViewDefinition[] content)
        {
            var viewDefinition = new ViewDefinition
            {
                Template = "views/_layouts/_layout",
                Placeholder = new PlaceholderDefinitionCollection
                {
                    {
                        "content", new[]
                        {
                            new NavigationGroupModel
                            {
                                Template = "views/_layouts/page",
                                Actions = new List<ActionModel>
                                {
                                    new ActionModel { Name = "Create View", Link = "#" },
                                    new ActionModel { Name = "Create Module", Link = "#" }
                                }
                                ,
                                Placeholder = new PlaceholderDefinitionCollection
                                {
                                    {
                                        "phContent", content
                                    }
                                }
                            }
                        }
                    }
                }
            };
            return viewDefinition;
        }
    }
}
