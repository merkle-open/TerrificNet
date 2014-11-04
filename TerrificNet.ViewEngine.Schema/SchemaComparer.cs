using Newtonsoft.Json.Schema;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TerrificNet.ViewEngine.Schema.Test
{
    public class SchemaComparer
    {
        public JsonSchema Apply(JsonSchema schema, JsonSchema baseSchema, List<SchemaComparisonFailure> failures, string propertyName = null)
        {
            if (schema == null)
                throw new ArgumentNullException("schema");

            if (baseSchema == null)
                return schema;

            if (!string.IsNullOrEmpty(baseSchema.Title))
                schema.Title = baseSchema.Title;

            if (baseSchema.Type != null)
            {
                if (!CanConvert(schema.Type, baseSchema.Type))
                    failures.Add(new TypeChangeFailure(propertyName, schema, baseSchema));
                else
                    schema.Type = baseSchema.Type;
            }

            if (schema.Properties != null)
            {
                foreach (var property in schema.Properties)
                {
                    if (baseSchema.Properties == null || !baseSchema.Properties.ContainsKey(property.Key))
                        continue;

                    var baseProperty = baseSchema.Properties[property.Key];
                    Apply(property.Value, baseProperty, failures, property.Key);
                }
            }

            if (schema.Items != null && baseSchema.Items != null)
            {
                for (int i = 0; i < schema.Items.Count && i < baseSchema.Items.Count; i++)
                {
                    Apply(schema.Items[i], baseSchema.Items[i], failures);
                }
            }

            return schema;
        }

        private bool CanConvert(JsonSchemaType? type, JsonSchemaType? typeBase)
        {
            if (!type.HasValue)
                return true;

            if ((type.Value != JsonSchemaType.Object && type.Value != JsonSchemaType.Array)
                && (typeBase.Value == JsonSchemaType.Object || typeBase.Value == JsonSchemaType.Array))
                return false;

            return true;
        }

    }

    public class SchemaComparisonFailure
    {
        public SchemaComparisonFailure(JsonSchema schemaPart, JsonSchema schemaPartBase)
        {
            this.SchemaPart = schemaPart;
            this.SchemaBasePart = schemaPartBase;
        }

        public JsonSchema SchemaPart { get; private set; }
        public JsonSchema SchemaBasePart { get; private set; }
    }

    public class TypeChangeFailure : SchemaComparisonFailure
    {
        public TypeChangeFailure(string propertyName, JsonSchema schemaPart, JsonSchema schemaPartBase)
            : base(schemaPart, schemaPartBase)
        {
            this.PropertyName = propertyName;
        }

        public string PropertyName { get; private set; }

        public override string ToString()
        {
            return base.ToString();
        }
    }
}
