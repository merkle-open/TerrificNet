using System.Collections.Generic;
using System.Threading.Tasks;
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

        public Task EvaluateAsync(object model, RenderingContext context, string name, IDictionary<string, string> parameters)
        {
            var templateId = context.Data["templateId"];
            return context.Writer.WriteAsync(templateId as string);
        }

        public IClientModel Evaluate(IClientContext context, IClientModel model, string name, IDictionary<string, string> parameters)
        {
            context.WriteLiteral(context.TemplateId);
            return model;
        }
    }
}
