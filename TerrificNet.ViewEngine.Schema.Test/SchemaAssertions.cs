using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json.Schema;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TerrificNet.ViewEngine.Schema.Test
{
    public static class SchemaAssertions
    {
        public static void AssertSingleProperty(JSchema schema, string propertyName, JSchemaType schemaType, bool required = true)
        {
            Assert.IsNotNull(schema);
            Assert.IsNotNull(schema.Properties);
            Assert.IsTrue(schema.Properties.ContainsKey(propertyName), string.Format("property with name '{0}' expected.", propertyName));
            Assert.IsNotNull(schema.Properties[propertyName].Type);
            Assert.AreEqual(schemaType, schema.Properties[propertyName].Type.Value);
            if (required)
                Assert.IsTrue(schema.Required.Contains(propertyName), "Property {0} should be required", propertyName);
        }
    }
}
