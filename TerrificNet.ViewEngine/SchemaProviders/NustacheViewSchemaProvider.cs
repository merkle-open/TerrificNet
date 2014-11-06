﻿using System.IO;
using Newtonsoft.Json.Schema;
using TerrificNet.ViewEngine.Config;
using TerrificNet.ViewEngine.Schema;
using Veil.Handlebars;

namespace TerrificNet.ViewEngine.SchemaProviders
{
    public class NustacheViewSchemaProvider : ISchemaProvider
    {
        private readonly string _basePath;

		public NustacheViewSchemaProvider(ITerrificNetConfig config)
        {
            _basePath = config.ViewPath;
        }

        public JsonSchema GetSchemaFromPath(string path)
        {
            var extractor = new SchemaExtractor(new HandlebarsTemplateParserRegistration());
            return extractor.Run(Path.Combine(_basePath, path));
        }
    }
}
