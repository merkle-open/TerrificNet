using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json.Schema;
using Veil.Compiler;
using Veil.Helper;
using Veil.Parser;
using Veil.Parser.Nodes;

namespace TerrificNet.ViewEngine.Schema
{
    public class SchemaExtractor
    {
        private readonly ITemplateParser _templateParser;

        public SchemaExtractor(ITemplateParser templateParser)
        {
            _templateParser = templateParser;
        }

        public JSchema Run(string templateId, StreamReader reader, IMemberLocator memberLocator, IHelperHandler[] helperHandlers)
        {
            using (reader)
            {
                var node = _templateParser.Parse(templateId, reader, typeof(object), memberLocator, helperHandlers);
                var visitor = new SchemaBuilderVisitor();
                visitor.Visit(node);

                return visitor.Schema;
            }
        }

        private class SchemaBuilderVisitor : NodeVisitorBase<JSchema>
        {
            private readonly Stack<JSchema> _schemas = new Stack<JSchema>();

            public SchemaBuilderVisitor()
            {
                _schemas.Push(new JSchema());
            }

            public JSchema Schema { get { return _schemas.Peek(); } }

            protected override JSchema VisitIterateNode(IterateNode iterateNode)
            {
                var schema = this.VisitExpressionNode(iterateNode.Collection);
                if (schema == null)
                    return null;

                var arrayItemSchema = new JSchema();

                schema.Type = JSchemaType.Array;

                _schemas.Push(arrayItemSchema);
                this.VisitBlockNode(iterateNode.Body);
                this.VisitBlockNode(iterateNode.EmptyBody);
                arrayItemSchema = _schemas.Pop();

                schema.Items.Add(arrayItemSchema);

                return schema;
            }

            protected override JSchema VisitConditionalNode(ConditionalNode node)
            {
                var schema = base.VisitConditionalNode(node);
                var parentSchema = _schemas.Peek();
                //schema.Required = false;
                if (schema.Type == null)
                    schema.Type = JSchemaType.Boolean;

                return schema;
            }

            protected override JSchema VisitBlockNode(BlockNode blockNode)
            {
                foreach (var child in blockNode.Nodes)
                {
                    Visit(child);
                }

                return null;
            }

            protected override JSchema VisitWriteExpressionNode(WriteExpressionNode writeExpressionNode)
            {
                var schema = base.VisitWriteExpressionNode(writeExpressionNode);
                if (schema == null)
                    return null;

                schema.Type = JSchemaType.String;
                return schema;
            }

            protected override JSchema VisitSubModelExpressionNode(SubModelExpressionNode subModuleExpression)
            {
                var schema = this.VisitExpressionNode(subModuleExpression.ModelExpression);
                _schemas.Push(schema);
                schema = VisitExpressionNode(subModuleExpression.SubModelExpression);
                _schemas.Pop();

                return schema;
            }

            protected override JSchema VisitLateBoundExpression(LateBoundExpressionNode lateboundExpression)
            {
                var modelSchema = _schemas.Peek();
                if (lateboundExpression.Scope == ExpressionScope.ModelOfParentScope)
                {
                    var subSchema = _schemas.Pop();
                    modelSchema = _schemas.Peek();
                    _schemas.Push(subSchema);
                }

                string propertyName = lateboundExpression.ItemName;

                modelSchema.Type = JSchemaType.Object;
                JSchema existingSchema;
                if (!modelSchema.Properties.TryGetValue(propertyName, out existingSchema))
                {
                    existingSchema = new JSchema();
                    modelSchema.Properties.Add(propertyName, existingSchema);
                }
                modelSchema.Required.Add(propertyName);

                return existingSchema;
            }

            protected override JSchema VisitHelperNode(HelperExpressionNode helperNode)
            {
                var provider = helperNode.HelperHandler as IHelperHandlerWithSchema;
                if (provider != null)
                {
                    var schema = provider.GetSchema(helperNode.Name, helperNode.Parameters);
                    if (schema != null)
                    {
                        var result = new SchemaCombiner().Apply(_schemas.Pop(), schema, new SchemaComparisionReport());
                        _schemas.Push(result);
                        return result;
                    }
                }
                return base.VisitHelperNode(helperNode);
            }

        }
    }
}
