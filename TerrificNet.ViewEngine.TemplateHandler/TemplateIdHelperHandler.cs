using System.Collections.Generic;
using TerrificNet.ViewEngine.Client;
using Veil;
using Veil.Helper;

namespace TerrificNet.ViewEngine.TemplateHandler
{
    internal class TemplateIdHelperHandler : IHelperHandler, IHelperHandlerClient
    {
        public bool IsSupported(string name)
        {
            return name.StartsWith("template-id");
        }

        public void Evaluate(object model, RenderingContext context, string name, IDictionary<string, string> parameters)
        {
            context.Writer.Write(context.Data["templateId"]);
        }

        public IClientModel Evaluate(IClientContext context, IClientModel model, string name, IDictionary<string, string> parameters)
        {
            context.WriteLiteral(context.TemplateId);
            return model;
        }
    }
}
