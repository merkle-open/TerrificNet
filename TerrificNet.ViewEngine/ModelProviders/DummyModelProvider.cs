using System.Collections.Generic;

namespace TerrificNet.ViewEngine.ModelProviders
{
    public class DummyModelProvider : IModelProvider
    {
        public object GetModelFromPath(string path)
        {
            var model = new Dictionary<string, object>();
            model["name"] = "Hans Muster";

            return model;
        }
    }
}