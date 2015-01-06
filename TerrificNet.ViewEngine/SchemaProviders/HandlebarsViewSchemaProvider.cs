using System.IO;
using Newtonsoft.Json.Schema;
using TerrificNet.ViewEngine.Schema;
using Veil.Handlebars;

namespace TerrificNet.ViewEngine.SchemaProviders
{
    public class HandlebarsViewSchemaProvider : ISchemaProvider
    {
        public JsonSchema GetSchemaFromTemplate(TemplateInfo template)
        {
            var extractor = new SchemaExtractor(new HandlebarsTemplateParserRegistration());
            return extractor.Run(new StreamReader(template.Open()));
        }
    }
}
