using System;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using TerrificNet.Models;
using TerrificNet.ViewEngine;

namespace TerrificNet.ModelProviders
{
    public class DataVariationModelProvider : IModelProvider
    {
        public object GetDefaultModelForTemplate(TemplateInfo template)
        {
            return new DataVariationCollectionModel
            {
                Variations = new List<DataVariationModel>
                {
                    new DataVariationModel
                    {
                        Link = "",
                        Name = "Variation1"
                    },
                    new DataVariationModel
                    {
                        Link = "",
                        Name = "Variation2"
                    }
                }
            };
        }

        public void UpdateDefaultModelForTemplate(TemplateInfo template, object content)
        {
            throw new NotImplementedException();
        }

        public object GetModelForTemplate(TemplateInfo template, string dataId)
        {
            throw new NotImplementedException();
        }
    }
}
