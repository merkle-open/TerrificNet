using System;
using System.Threading.Tasks;
using Newtonsoft.Json.Schema;
using TerrificNet.ViewEngine;
using TerrificNet.ViewEngine.Cache;

namespace TerrificNet.Generator
{
    public class DynamicModelTypeProvider : IModelTypeProvider
    {
        private readonly IJsonSchemaCodeGenerator _generator;
        private readonly ISchemaProvider _schemaProvider;
        private readonly ICacheProvider _cacheProvider;

        public DynamicModelTypeProvider(IJsonSchemaCodeGenerator generator, ISchemaProvider schemaProvider, ICacheProvider cacheProvider)
        {
            _generator = generator;
            _schemaProvider = schemaProvider;
            _cacheProvider = cacheProvider;
        }

        public async Task<Type> GetModelTypeFromTemplateAsync(TemplateInfo templateInfo)
        {
            var hash = string.Concat("template_", templateInfo.Id);
            CacheEntry entry;

            JSchema schema;
            int hashCode;
            if (_cacheProvider.TryGet(hash, out entry))
            {
                if (entry.ETag.Equals(templateInfo.ETag, StringComparison.Ordinal))
                    return entry.Result;

                schema = await _schemaProvider.GetSchemaFromTemplateAsync(templateInfo).ConfigureAwait(false);
                hashCode = schema.ToString().GetHashCode();

                if (entry.SchemaHash == hashCode)
                    return entry.Result;
            }
            else
            {
                schema = await _schemaProvider.GetSchemaFromTemplateAsync(templateInfo).ConfigureAwait(false);
                hashCode = schema.ToString().GetHashCode();                
            }

            var result = _generator.Compile(schema);
            _cacheProvider.Set(hash, new CacheEntry { Result = result, SchemaHash = hashCode, ETag = templateInfo.ETag }, DateTimeOffset.Now.AddHours(24));

            return result;
        }

        private class CacheEntry
        {
            public string ETag { get; set; }
            public int SchemaHash { get; set; }
            public Type Result { get; set; }
        }
    }
}