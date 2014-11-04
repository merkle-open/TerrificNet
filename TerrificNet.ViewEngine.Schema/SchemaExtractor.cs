using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json.Schema;
using Nustache.Core;

namespace TerrificNet.ViewEngine.Schema
{
    public class SchemaExtractor
    {
        public JsonSchema Run(string templatePath)
        {
            using (var reader = new StreamReader(templatePath))
            {
                var template = new Template();
                template.Load(reader);

                var visitor = new VariablePartVisitor();
                var schemaBuilder = new SchemaBuilder();

                foreach (var part in template.Parts)
                {
                    visitor.Variable = null;
                    part.Accept(visitor);
                    if (visitor.Variable != null)
                    {
                        var path = visitor.Variable.Path;
                        var builder = schemaBuilder;
                        foreach (var pathPart in path.Split('.'))
                        {
                            builder = builder.PushProperty(pathPart);
                        }
                    }
                }

                return schemaBuilder.GetSchema();
            }
        }

        private class SchemaBuilder
        {
            private readonly JsonSchema _schema;

            public SchemaBuilder()
            {
                _schema = new JsonSchema();
            }

            private SchemaBuilder(JsonSchema schema)
            {
                _schema = schema;
            }

            public SchemaBuilder PushProperty(string property)
            {
                if (_schema.Properties == null)
                    _schema.Properties = new Dictionary<string, JsonSchema>();

                _schema.Type = JsonSchemaType.Object;

                JsonSchema propertySchema;
                if (_schema.Properties.TryGetValue(property, out propertySchema))
                    return new SchemaBuilder(propertySchema);

                propertySchema = new JsonSchema();
                _schema.Properties.Add(property, propertySchema);

                propertySchema.Type = JsonSchemaType.String;
                propertySchema.Required = true;

                return new SchemaBuilder(propertySchema);
            }

            public SchemaBuilder PushIsRequired(bool isRequired)
            {
                return this;
            }

            public JsonSchema GetSchema()
            {
                return _schema;
            }
        }

        private class VariablePartVisitor : PartVisitor
        {
            public VariableReference Variable { get; set; }

            public void Visit(Section section)
            {
            }

            public void Visit(Block block)
            {
            }

            public void Visit(LiteralText literal)
            {
            }

            public void Visit(EndSection endSections)
            {
            }

            public void Visit(InvertedBlock invertedBlock)
            {
            }

            public void Visit(TemplateInclude include)
            {
            }

            public void Visit(VariableReference variable)
            {
                this.Variable = variable;
            }
        }
    }
}
