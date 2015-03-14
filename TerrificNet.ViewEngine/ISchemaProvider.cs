using System.Threading.Tasks;
using Newtonsoft.Json.Schema;

namespace TerrificNet.ViewEngine
{
    public interface ISchemaProvider
    {
        Task<JSchema> GetSchemaFromTemplateAsync(TemplateInfo template);
    }
}