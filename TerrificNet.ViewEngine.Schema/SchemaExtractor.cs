using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json.Schema;
using Veil.Parser;
using Veil.Parser.Nodes;

namespace TerrificNet.ViewEngine.Schema
{
    public class SchemaExtractor
    {
        private readonly ITemplateParserRegistration _templateParserRegistration;

        public SchemaExtractor(ITemplateParserRegistration templateParserRegistration)
        {
            _templateParserRegistration = templateParserRegistration;
        }

        public JsonSchema Run(string templatePath)
        {
            if (!File.Exists(templatePath))
                return null;

            using (var reader = new StreamReader(templatePath))
            {
                var node = _templateParserRegistration.ParserFactory().Parse(reader, typeof(object));
                var visitor = new SchemaBuilderVisitor();
                visitor.Visit(node);

                return visitor.Schema;
            }
        }

        private class SchemaBuilderVisitor : NodeVisitorBase<JsonSchema>
        {
            private readonly Stack<JsonSchema> _schemas = new Stack<JsonSchema>();

            public SchemaBuilderVisitor()
            {
                _schemas.Push(new JsonSchema());
            }

            public JsonSchema Schema { get { return _schemas.Peek(); } }

            protected override JsonSchema VisitIterateNode(IterateNode iterateNode)
            {
                var schema = this.VisitExpressionNode(iterateNode.Collection);
                if (schema == null)
                    return null;

                var arrayItemSchema = new JsonSchema();

                schema.Type = JsonSchemaType.Array;
                schema.Items = new List<JsonSchema>
                {
                    arrayItemSchema
                };

                _schemas.Push(arrayItemSchema);
                this.VisitBlockNode(iterateNode.Body);
                this.VisitBlockNode(iterateNode.EmptyBody);
                _schemas.Pop();

                return schema;
            }

            protected override JsonSchema VisitConditionalNode(ConditionalNode node)
            {
                var schema = base.VisitConditionalNode(node);
                schema.Required = false;
                if (schema.Type == null)
                    schema.Type = JsonSchemaType.Boolean;

                return schema;
            }

            protected override JsonSchema VisitBlockNode(BlockNode blockNode)
            {
                foreach (var child in blockNode.Nodes)
                {
                    Visit(child);    
                }

                return null;
            }

            protected override JsonSchema VisitWriteExpressionNode(WriteExpressionNode writeExpressionNode)
            {
                var schema = base.VisitWriteExpressionNode(writeExpressionNode);
                if (schema == null)
                    return null;

                schema.Type = JsonSchemaType.String;
                return schema;
            }

            protected override JsonSchema VisitSubModelExpressionNode(SubModelExpressionNode subModuleExpression)
            {
                var schema = this.VisitExpressionNode(subModuleExpression.ModelExpression);
                _schemas.Push(schema);
                schema = VisitExpressionNode(subModuleExpression.SubModelExpression);
                _schemas.Pop();

                return schema;
            }

            protected override JsonSchema VisitLateBoundExpression(LateBoundExpressionNode lateboundExpression)
            {
                JsonSchema modelSchema = _schemas.Peek();
                string propertyName = lateboundExpression.ItemName;
                if (modelSchema.Properties == null)
                    modelSchema.Properties = new Dictionary<string, JsonSchema>();

                modelSchema.Type = JsonSchemaType.Object;
                JsonSchema existingSchema;
                if (!modelSchema.Properties.TryGetValue(propertyName, out existingSchema))
                {
                    existingSchema = new JsonSchema
                    {
                        Required = true,
                        //Type = JsonSchemaType.String
                    };
                    modelSchema.Properties.Add(propertyName, existingSchema);
                }

                return existingSchema;
            }
        }
    }
}
