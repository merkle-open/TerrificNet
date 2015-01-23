using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Practices.Unity;
using TerrificNet.Models;
using TerrificNet.UnityModules;
using TerrificNet.ViewEngine;

namespace TerrificNet.ModelProviders
{
    public class ApplicationOverviewModelProvider : IModelProvider
    {
        private readonly TerrificNetApplication[] _applications;

        public ApplicationOverviewModelProvider(TerrificNetApplication[] applications)
        {
            _applications = applications;
        }

        public object GetDefaultModelForTemplate(TemplateInfo template)
        {
            if (template.Id != "components/modules/ApplicationOverview/ApplicationOverview")
                return null;

            var model = new ApplicationOverviewModel
            {
                Applications = _applications.Select(a => new ViewOverviewModel
                {
                    Name = a.Name,
                    Views = GetViews(a.Section, a.Container.Resolve<ITemplateRepository>()).ToList()
                }).ToList()
            };

            return model;
        }

        public void UpdateDefaultModelForTemplate(TemplateInfo template, object content)
        {
            throw new NotSupportedException();
        }

        public object GetModelForTemplate(TemplateInfo template, string dataId)
        {
            throw new NotSupportedException();
        }

        private static IEnumerable<ViewItemModel> GetViews(string section, ITemplateRepository templateRepository)
        {
            foreach (var file in templateRepository.GetAll())
            {
                yield return new ViewItemModel
                {
                    Text = file.Id,
                    Url = string.Format("/{0}", file.Id),
                    EditUrl = string.Format("/web/edit.html?template={0}", file.Id),
                    AdvancedUrl = string.Format("/web/edit_advanced.html?template={0}", file.Id),
                    SchemaUrl = string.Format("{0}schema/{1}", section, file.Id)
                };
            }
        }
    }
}