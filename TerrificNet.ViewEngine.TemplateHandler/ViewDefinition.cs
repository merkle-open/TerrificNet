using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Veil;

namespace TerrificNet.ViewEngine.TemplateHandler
{
    public abstract class ViewDefinition
    {
        [JsonProperty("_placeholder")]
        public PlaceholderDefinitionCollection Placeholder { get; set; }

        [JsonExtensionData]
        public IDictionary<string, object> ExtensionData { get; set; }

        protected internal abstract void Render(ITerrificTemplateHandler templateHandler, object model, RenderingContext context);

        public static T FromJObject<T>(JToken obj)
            where T : ViewDefinition
        {
            var serializer = new JsonSerializer();
            serializer.Converters.Add(new ViewDefinitionTypeConverter());

            return obj.ToObject<T>(serializer);
        }
    }

    public class ViewDefinitionTypeConverter : JsonConverter
    {
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            serializer.Serialize(writer, value);
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            var jObject = JObject.Load(reader);

            object target;
            if (jObject.Property("module") != null)
                target = new ModuleViewDefinition();
            else if (jObject.Property("template") != null)
                target = new PartialViewDefinition();
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
            return objectType != typeof(PageViewDefinition) && typeof(ViewDefinition).IsAssignableFrom(objectType);
        }
    }

    public class PageViewDefinition : PartialViewDefinition
    {
        public string Id { get; set; }

        [JsonProperty("title")]
        public string Title { get; set; }

        [JsonProperty("styles")]
        public List<StyleImport> Styles { get; set; }

        [JsonProperty("scripts")]
        public List<ScriptImport> Scripts { get; set; }
    }

    public class PartialViewDefinition : ViewDefinition, IRuntimeModel
    {
        [JsonProperty("template")]
        public string Template { get; set; }

        [JsonProperty("data")]
        public object Data { get; set; }

        protected internal override void Render(ITerrificTemplateHandler templateHandler, object model, RenderingContext context)
        {
            templateHandler.RenderPartial(Template, this, context);
        }
    }

    public class ModuleViewDefinition : ViewDefinition
    {
        [JsonProperty("module")]
        public string Module { get; set; }
 
        [JsonProperty("skin")]
        public string Skin { get; set; }

		[JsonProperty("data_variation")]
		public string DataVariation { get; set; }

        protected internal override void Render(ITerrificTemplateHandler templateHandler, object model, RenderingContext context)
        {
			if (context.Data.ContainsKey("data_variation"))
				context.Data["data_variation"] = this.DataVariation;
			else
				context.Data.Add("data_variation", this.DataVariation);

            templateHandler.RenderModule(Module, Skin, context);
        }
    }
}