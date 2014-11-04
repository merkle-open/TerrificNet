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

                var schemaBuilder = new SchemaBuilder();
                var visitor = new VariablePartVisitor(schemaBuilder);

                template.Accept(visitor);

                return schemaBuilder.GetSchema();
            }
        }

        public class PropertySchemaBuilder : SchemaBuilder
        {
            private readonly JsonSchema _schema;

            public PropertySchemaBuilder(JsonSchema schema) : base(schema)
            {
                _schema = schema;
            }

            public PropertySchemaBuilder ChangeIsRequired(bool isRequired)
            {
                _schema.Required = isRequired;
                return this;
            }

            public PropertySchemaBuilder ChangeType(JsonSchemaType type)
            {
                _schema.Type = type;
                return this;
            }

            public SchemaBuilder ChangeToArray()
            {
                var arrayItemSchema = new JsonSchema();

                _schema.Type = JsonSchemaType.Array;
                _schema.Items = new List<JsonSchema>
                {
                    arrayItemSchema
                };

                return new SchemaBuilder(arrayItemSchema);
            }
        }

        public class SchemaBuilder
        {
            private readonly JsonSchema _schema;

            public SchemaBuilder()
            {
                _schema = new JsonSchema();
            }

            public SchemaBuilder(JsonSchema schema)
            {
                _schema = schema;
            }

            public PropertySchemaBuilder PushPath(string path)
            {
                var builder = this;
                foreach (var pathPart in path.Split('.'))
                {
                    builder = builder.PushProperty(pathPart);
                }

                return builder as PropertySchemaBuilder;
            }

            public PropertySchemaBuilder PushProperty(string property)
            {
                if (_schema.Properties == null)
                    _schema.Properties = new Dictionary<string, JsonSchema>();

                _schema.Type = JsonSchemaType.Object;

                JsonSchema propertySchema;
                if (!_schema.Properties.TryGetValue(property, out propertySchema))
                {
                    propertySchema = new JsonSchema();
                    _schema.Properties.Add(property, propertySchema);

                    propertySchema.Type = JsonSchemaType.String;
                    propertySchema.Required = true;                    
                }
                return new PropertySchemaBuilder(propertySchema);
            }

            public JsonSchema GetSchema()
            {
                return _schema;
            }
        }

        private class VariablePartVisitor : PartVisitor
        {
            private SchemaBuilder _schemaBuilder;

            public VariablePartVisitor(SchemaBuilder schemaBuilder)
            {
                _schemaBuilder = schemaBuilder;
            }

            public void Visit(Section section)
            {
                VisitParts(section);
            }

            public void Visit(Block block)
            {
                var parts = block.Name.Split(' ');
                if (parts.Length > 0)
                {
                    if (parts[0] == "if")
                    {
                        var propertyBuilder = _schemaBuilder.PushPath(parts[1]);
                        propertyBuilder.ChangeIsRequired(false);
                        propertyBuilder.ChangeType(JsonSchemaType.Boolean);

                        VisitParts(block);
                    }
                    else if (parts[0] == "each")
                    {
                        var tempBuilder = _schemaBuilder;
                        var propertyBuilder = _schemaBuilder.PushPath(parts[1]);
                        _schemaBuilder = propertyBuilder.ChangeToArray();
                        VisitParts(block);

                        _schemaBuilder = tempBuilder;
                    }
                }
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
                var builder = _schemaBuilder.PushPath(variable.Path);
                builder.ChangeType(JsonSchemaType.String);
            }

            private void VisitParts(Section section)
            {
                foreach (var part in section.Parts)
                {
                    part.Accept(this);
                }
            }

        }
    }
}
