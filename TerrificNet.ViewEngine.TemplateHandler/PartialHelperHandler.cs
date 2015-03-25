using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Newtonsoft.Json.Schema;
using TerrificNet.ViewEngine.Client;
using TerrificNet.ViewEngine.Schema;
using Veil;
using Veil.Helper;

namespace TerrificNet.ViewEngine.TemplateHandler
{
    internal class PartialHelperHandler : IHelperHandler, IHelperHandlerWithSchema, IHelperHandlerClient
    {
        private readonly ITerrificTemplateHandler _handler;
        private readonly ISchemaProvider _schemaProvider;
        private readonly ITemplateRepository _templateRepository;
        private readonly IClientTemplateGenerator _clientTemplateGenerator;

        public PartialHelperHandler(ITerrificTemplateHandler handler, ISchemaProvider schemaProvider, ITemplateRepository templateRepository, IClientTemplateGenerator clientTemplateGenerator)
        {
            _handler = handler;
            _schemaProvider = schemaProvider;
            _templateRepository = templateRepository;
            _clientTemplateGenerator = clientTemplateGenerator;
        }

        public bool IsSupported(string name)
        {
            return name.StartsWith("partial", StringComparison.OrdinalIgnoreCase);
        }

        public Task EvaluateAsync(object model, RenderingContext context, IDictionary<string, string> parameters)
        {
            var template = parameters["template"].Trim('"');
            return _handler.RenderPartialAsync(template, model, context);
        }

        public JSchema GetSchema(string name, IDictionary<string, string> parameters)
        {
            // TODO: Use async
            var templateInfo = _templateRepository.GetTemplateAsync(parameters["template"].Trim('"')).Result;
            if (templateInfo == null)
                return null;

            // TODO: Use async
            return _schemaProvider.GetSchemaFromTemplateAsync(templateInfo).Result;
        }

        public IClientModel Evaluate(IClientContext context, IClientModel model, string name, IDictionary<string, string> parameters)
        {
            // TODO: Use async
            var templateInfo = _templateRepository.GetTemplateAsync(parameters["template"].Trim('"')).Result;
            if (templateInfo == null)
                return model;

            _clientTemplateGenerator.Generate(templateInfo, new PartialClientContextAdapter(templateInfo.Id, context), model);
            
            return model;
        }

        private class PartialClientContextAdapter : IClientContext
        {
            private readonly IClientContext _adaptee;

            public PartialClientContextAdapter(string templateId, IClientContext adaptee)
            {
                _adaptee = adaptee;
                this.TemplateId = templateId;
            }

            public string TemplateId { get; private set; }

            public void WriteLiteral(string content)
            {
                _adaptee.WriteLiteral(content);
            }

            public void WriteExpression(IClientModel model)
            {
                _adaptee.WriteExpression(model);
            }

            public void WriteEncodeExpression(IClientModel model)
            {
                _adaptee.WriteEncodeExpression(model);
            }

            public IClientModel BeginIterate(IClientModel model)
            {
                return _adaptee.BeginIterate(model);
            }

            public void EndIterate()
            {
                _adaptee.EndIterate();
            }

            public void BeginIf(IClientModel model)
            {
                _adaptee.BeginIf(model);
            }

            public void EndIf()
            {
                _adaptee.EndIf();
            }

            public void ElseIf()
            {
                _adaptee.ElseIf();
            }
        }

    }
}