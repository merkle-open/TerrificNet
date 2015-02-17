using System;
using Newtonsoft.Json.Schema;

namespace TerrificNet.Generator
{
	public interface IJsonSchemaCodeGenerator
	{
        string Generate(JSchema schema);
        Type Compile(JSchema schema);
	}
}