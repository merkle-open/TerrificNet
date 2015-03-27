using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace TerrificNet.ViewEngine.TemplateHandler.UI
{
    public class ViewDefinitionTypeConverter : JsonConverter
    {
        private readonly ITemplateRepository _templateRespository;
        private readonly IModelTypeProvider _typeProvider;

        public ViewDefinitionTypeConverter(ITemplateRepository templateRespository, IModelTypeProvider typeProvider)
        {
            _templateRespository = templateRespository;
            _typeProvider = typeProvider;
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            serializer.Serialize(writer, value);
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            var jObject = JObject.Load(reader);

            object target;
            if (jObject.Property("module") != null)
            {
                target = new ModuleViewDefinition();
            }
            else if (jObject.Property("template") != null)
            {
                var template = _templateRespository.GetTemplateAsync(jObject.Property("template").Value.Value<string>()).Result;
                var type = _typeProvider.GetModelTypeFromTemplateAsync(template).Result;
                target = Activator.CreateInstance(typeof(PageViewDefinition<>).MakeGenericType(type), template);
                
            }
            else
                return serializer.Deserialize(jObject.CreateReader(), objectType);

            var jObjectReader = jObject.CreateReader();
            jObjectReader.Culture = reader.Culture;
            jObjectReader.DateParseHandling = reader.DateParseHandling;
            jObjectReader.DateTimeZoneHandling = reader.DateTimeZoneHandling;
            jObjectReader.FloatParseHandling = reader.FloatParseHandling;

            serializer.Populate(jObjectReader, target);

            return target;
        }

        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(IPageViewDefinition) || typeof(ViewDefinition).IsAssignableFrom(objectType);
        }
    }
}