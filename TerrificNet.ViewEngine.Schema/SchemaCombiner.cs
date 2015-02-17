using System;
using System.Collections.Generic;
using Newtonsoft.Json.Schema;

namespace TerrificNet.ViewEngine.Schema
{
    public class SchemaCombiner
    {
        public virtual JSchema Apply(JSchema schema1, JSchema schema2, SchemaComparisionReport report,
            string propertyName = null)
        {
            if (schema1 == null)
                throw new ArgumentNullException("schema1");

            if (schema2 == null)
                throw new ArgumentNullException("schema2");

            if (report == null)
                report = new SchemaComparisionReport();

            var result = new JSchema
            {
                Title = Use(schema1, schema2, "title", s => s.Title, report),
                Type = Use(schema1, schema2, "type", s => s.Type, report, v => v.HasValue)
            };

            foreach (var prop in UseProperties(schema1, schema2, report))
            {
                result.Properties.Add(prop);
            }

            return result;
        }

        private IDictionary<string, JSchema> UseProperties(JSchema schema1, JSchema schema2, SchemaComparisionReport report)
        {
            if (schema1.Properties == null)
                return schema2.Properties;

            if (schema2.Properties == null)
                return schema2.Properties;

            var result = new Dictionary<string, JSchema>();
            foreach (var propertyEntry in schema1.Properties)
            {
                Proceed(schema2, report, result, propertyEntry);
            }

            foreach (var propertyEntry in schema2.Properties)
            {
                Proceed(schema1, report, result, propertyEntry);
            }

            return result;
        }

        private void Proceed(JSchema otherSchema, SchemaComparisionReport report, IDictionary<string, JSchema> result, KeyValuePair<string, JSchema> propertyEntry)
        {
            if (!result.ContainsKey(propertyEntry.Key))
            {
                JSchema resultSchema;
                JSchema propSchema2;
                if (otherSchema.Properties.TryGetValue(propertyEntry.Key, out propSchema2))
                    resultSchema = Apply(propertyEntry.Value, propSchema2, report, propertyEntry.Key);
                else
                    resultSchema = propertyEntry.Value;

                result.Add(propertyEntry.Key, resultSchema);
            }
        }

        private static T Use<T>(JSchema schema1, JSchema schema2, string propertyName,
            Func<JSchema, T> propertyAccess, SchemaComparisionReport report, Func<T, bool> hasValueCheck)
        {
            var val1 = propertyAccess(schema1);
            var val2 = propertyAccess(schema2);

            var hasValue1 = hasValueCheck(val1);
            var hasValue2 = hasValueCheck(val2);

            if (hasValue1 && hasValue2 && !val1.Equals(val2))
            {
                report.Push(new ValueConflict(propertyName, schema1, schema2, SchemaComparisionNotificationLevel.Failure)
                {
                    Value1 = val1,
                    Value2 = val2
                });
                return default(T);
            }

            if (hasValue1)
                return val1;

            if (hasValue2)
                return val2;

            return default(T);
        }

        private static string Use(JSchema schema1, JSchema schema2, string propertyName, Func<JSchema, string> propertyAccess, SchemaComparisionReport report)
        {
            return Use<string>(schema1, schema2, propertyName, propertyAccess, report, f => !string.IsNullOrEmpty(f));
        }
    }
}
