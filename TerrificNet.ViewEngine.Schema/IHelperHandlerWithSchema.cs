using System.Collections.Generic;
using Newtonsoft.Json.Schema;

namespace TerrificNet.ViewEngine.Schema
{
    public interface IHelperHandlerWithSchema
    {
        JSchema GetSchema(string name, IDictionary<string, string> parameters);
    }
}