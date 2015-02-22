using System.Collections.Generic;
using TerrificNet.ViewEngine.Client;
using TerrificNet.ViewEngine.ViewEngines;

namespace TerrificNet.ViewEngine.TemplateHandler
{
    internal class TemplateIdHelperHandler : IRenderingHelperHandler, IHelperHandlerClient
    {
        private readonly Stack<RenderingContext> _contextStack = new Stack<RenderingContext>();

        public bool IsSupported(string name)
        {
            return name.StartsWith("template-id");
        }

        public void Evaluate(object model, string name, IDictionary<string, string> parameters)
        {
            var context = _contextStack.Peek();
            context.Writer.Write(context.Data["templateId"]);
        }

        public void PushContext(RenderingContext context)
        {
            _contextStack.Push(context);
        }

        public void PopContext()
        {
            _contextStack.Pop();
        }

        public IClientModel Evaluate(IClientContext context, IClientModel model, string name, IDictionary<string, string> parameters)
        {
            context.WriteLiteral(context.TemplateId);
            return model;
        }
    }
}
