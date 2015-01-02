using System;
using System.Collections.Generic;

namespace TerrificNet.ViewEngine.ModelProviders
{
    public class DummyModelProvider : IModelProvider
    {
        public object GetModelForTemplate(string template)
        {
            var model = new Dictionary<string, object>();
            model["name"] = "Hans Muster";

            return model;
        }

        public void UpdateModelForTemplate(string template, object content)
        {
            throw new NotSupportedException();
        }
    }
}