using System;
using System.Threading.Tasks;
using Newtonsoft.Json.Schema;
using TerrificNet.ViewEngine.Cache;

namespace TerrificNet.ViewEngine
{
    public class SchemaProviderCacheAdapter : ISchemaProvider
    {
        private readonly ISchemaProvider _adaptee;
        private readonly ICacheProvider _cacheProvider;

        public SchemaProviderCacheAdapter(ISchemaProvider adaptee, ICacheProvider cacheProvider)
        {
            _adaptee = adaptee;
            _cacheProvider = cacheProvider;
        }

        public Task<JSchema> GetSchemaFromTemplateAsync(TemplateInfo template)
        {
            var hash = string.Concat("template_", template.Id, template.ETag);

            Task<JSchema> schema;
            if (_cacheProvider.TryGet(hash, out schema))
                return schema;

            var result = _adaptee.GetSchemaFromTemplateAsync(template);
            _cacheProvider.Set(hash, result, DateTimeOffset.Now.AddHours(24));

            return result;
        }
    }
}