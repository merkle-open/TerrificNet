using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq.Expressions;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Veil;

namespace TerrificNet.ViewEngine.ViewEngines.TemplateHandler
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

    internal class Merge : DynamicObject
    {
        private readonly dynamic _obj1;
        private readonly dynamic _obj2;

        public Merge(dynamic obj1, dynamic obj2)
        {
            _obj1 = obj1;
            _obj2 = obj2;
        }

        public override IEnumerable<string> GetDynamicMemberNames()
        {
            return _obj1.GetDynamicMemberNames().Union(_obj2.GetDynamicMemberNames());
        }

        public override DynamicMetaObject GetMetaObject(Expression parameter)
        {
            return new DynamicMetaObject(parameter, BindingRestrictions.Empty);
        }

        public override bool TryBinaryOperation(BinaryOperationBinder binder, object arg, out object result)
        {
            return _obj1.TryBinaryOperation(binder, arg, out result) ||
                   _obj2.TryBinaryOperation(binder, arg, out result);
        }

        public override bool TryConvert(ConvertBinder binder, out object result)
        {
            return _obj1.TryConvert(binder, out result) ||
                   _obj2.TryConvert(binder, out result);
        }

        public override bool TryCreateInstance(CreateInstanceBinder binder, object[] args, out object result)
        {
            return _obj1.TryCreateInstance(binder, args, out result) ||
                   _obj2.TryCreateInstance(binder, args, out result);
        }

        public override bool TryDeleteIndex(DeleteIndexBinder binder, object[] indexes)
        {
            return _obj1.TryDeleteIndex(binder, indexes) ||
                   _obj2.TryDeleteIndex(binder, indexes);
        }

        public override bool TryDeleteMember(DeleteMemberBinder binder)
        {
            return _obj1.TryDeleteMember(binder) ||
                   _obj2.TryDeleteMember(binder);
        }

        public override bool TryGetIndex(GetIndexBinder binder, object[] indexes, out object result)
        {
            return _obj1.TryGetIndex(binder, indexes, out result) ||
                   _obj2.TryGetIndex(binder, indexes, out result);
        }

        public override bool TryGetMember(GetMemberBinder binder, out object result)
        {
            return _obj1.TryGetMember(binder, out result) ||
                   _obj2.TryGetMember(binder, out result);
        }

        public override bool TryInvoke(InvokeBinder binder, object[] args, out object result)
        {
            return _obj1.TryInvoke(binder, args, out result) ||
                   _obj2.TryInvoke(binder, args, out result);
        }

        public override bool TryInvokeMember(InvokeMemberBinder binder, object[] args, out object result)
        {
            return _obj1.TryInvokeMember(binder, args, out result) ||
                   _obj2.TryInvokeMember(binder, args, out result);
        }

        public override bool TrySetIndex(SetIndexBinder binder, object[] indexes, object value)
        {
            return _obj1.TrySetIndex(binder, indexes, value) ||
                   _obj2.TrySetIndex(binder, indexes, value);
        }

        public override bool TrySetMember(SetMemberBinder binder, object value)
        {
            return _obj1.TrySetMember(binder, value) ||
                   _obj2.TrySetMember(binder, value);
        }

        public override bool TryUnaryOperation(UnaryOperationBinder binder, out object result)
        {
            return _obj1.TryUnaryOperation(binder, out result) ||
                   _obj2.TryUnaryOperation(binder, out result);
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