using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json.Schema;

namespace TerrificNet.ViewEngine.Schema
{
    public class SchemaComparer
    {
        public JSchema Apply(JSchema schema, JSchema baseSchema, SchemaComparisionReport report, string propertyName = null)
        {
            if (schema == null)
                return baseSchema;

            if (baseSchema == null)
                return schema;

            if (!string.IsNullOrEmpty(baseSchema.Title))
                schema.Title = baseSchema.Title;

            if (!string.IsNullOrEmpty(baseSchema.Format))
                schema.Format = baseSchema.Format;

            if (baseSchema.Type != null)
            {
                if (!CanConvert(schema.Type, baseSchema.Type))
                    report.Push(new TypeChangeFailure(propertyName, schema, baseSchema));
                else
                    schema.Type = baseSchema.Type;
            }

            if (schema.Properties != null)
            {
                foreach (var property in schema.Properties)
                {
                    if (baseSchema.Properties == null || !baseSchema.Properties.ContainsKey(property.Key))
                    {
                        report.Push(new MissingPropertyInfo(property.Key, schema, baseSchema));
                        continue;
                    }

                    var baseProperty = baseSchema.Properties[property.Key];
                    Apply(property.Value, baseProperty, report, property.Key);
                }
            }

            if (schema.Items != null && baseSchema.Items != null)
            {
                for (int i = 0; i < schema.Items.Count && i < baseSchema.Items.Count; i++)
                {
                    Apply(schema.Items[i], baseSchema.Items[i], report);
                }
            }

            return schema;
        }

        private bool CanConvert(JSchemaType? type, JSchemaType? typeBase)
        {
            if (!type.HasValue)
                return true;

            if ((type.Value != JSchemaType.Object && type.Value != JSchemaType.Array)
                && (typeBase.Value == JSchemaType.Object || typeBase.Value == JSchemaType.Array))
                return false;

            return true;
        }

    }

    public class SchemaComparisionReport
    {
        private readonly List<SchemaComparisonNotification> _notifications = new List<SchemaComparisonNotification>();

        public IEnumerable<SchemaComparisonNotification> GetFailures()
        {
            return _notifications.Where(n => n.Level == SchemaComparisionNotificationLevel.Failure);
        }

        internal void Push(SchemaComparisonNotification notification)
        {
            _notifications.Add(notification);
        }

        public IEnumerable<SchemaComparisonNotification> GetInfos()
        {
            return _notifications.Where(n => n.Level == SchemaComparisionNotificationLevel.Info);
        }
    }

    public enum SchemaComparisionNotificationLevel
    {
        Info,
        Warning,
        Failure
    }

    public abstract class SchemaComparisonNotification
    {
        protected SchemaComparisonNotification(JSchema schemaPart, JSchema schemaPartBase, SchemaComparisionNotificationLevel level)
        {
            SchemaPart = schemaPart;
            SchemaBasePart = schemaPartBase;
            Level = level;
        }

        public JSchema SchemaPart { get; private set; }
        public JSchema SchemaBasePart { get; private set; }
        public SchemaComparisionNotificationLevel Level { get; set; }
    }

    public class MissingPropertyInfo : SchemaComparisonNotification
    {
        public MissingPropertyInfo(string propertyName, JSchema schemaPart, JSchema schemaPartBase) 
            : base(schemaPart, schemaPartBase, SchemaComparisionNotificationLevel.Info)
        {
            PropertyName = propertyName;
        }

        public string PropertyName { get; set; }

    }

    public class TypeChangeFailure : SchemaComparisonNotification
    {
        public TypeChangeFailure(string propertyName, JSchema schemaPart, JSchema schemaPartBase)
            : base(schemaPart, schemaPartBase, SchemaComparisionNotificationLevel.Failure)
        {
            this.PropertyName = propertyName;
        }

        public string PropertyName { get; private set; }
    }

    public class ValueConflict : SchemaComparisonNotification
    {
        public string PropertyName { get; private set; }

        public ValueConflict(string propertyName, JSchema schemaPart, JSchema schemaPartBase, SchemaComparisionNotificationLevel level)
            : base(schemaPart, schemaPartBase, level)
        {
            PropertyName = propertyName;
        }

        public object Value1 { get; internal set; }
        public object Value2 { get; internal set; }
    }
}
