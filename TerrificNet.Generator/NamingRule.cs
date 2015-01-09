using System;
using System.Linq;
using System.Text.RegularExpressions;
using Newtonsoft.Json.Schema;

namespace TerrificNet.Generator
{
    public interface INamingRule
    {
        string GetClassName(JsonSchema schema, string propertyName);
        string GetClassNameFromArrayItem(JsonSchema schema, string propertyName);
        string GetPropertyName(string input);
        string GetNamespaceName(JsonSchema schema);
    }

    public class NamingRule : INamingRule
    {
        public string GetClassName(JsonSchema schema, string propertyName)
        {
            var className = GetPropertyName(schema.Title);
            if (string.IsNullOrEmpty(className))
            {
                className = propertyName;
            }

            return className;
        }

        public string GetClassNameFromArrayItem(JsonSchema schema, string propertyName)
        {
            var className = GetPropertyName(schema.Title);
            if (string.IsNullOrEmpty(className))
            {
                className = Singular(propertyName);
            }

            return className;
        }

        private static string Singular(string propertyName)
        {
            if (propertyName.EndsWith("s"))
                return propertyName.Substring(0, propertyName.Length - 1);
            
            return propertyName;
        }

        public string GetPropertyName(string input)
        {
            return NormalizeName(input);
        }

        public string GetNamespaceName(JsonSchema schema)
        {
            return NormalizeName(schema.Title);
        }

        private static string NormalizeName(string input)
        {
            if (string.IsNullOrEmpty(input))
                return input;

            return ConvertToPascalCase(input).Replace(" ", "").Replace("_", "").Trim();
        }

        private static string ConvertToPascalCase(string input)
        {
            return new Regex(@"\p{Lu}\p{Ll}+|\p{Lu}+(?!\p{Ll})|\p{Ll}+|\d+").Replace(input, (MatchEvaluator) EvaluatePascal);
        }

        private static string EvaluatePascal(Match match)
        {
            var value = match.Value;
            var valueLength = value.Length;

            if (valueLength == 1)
                return value.ToUpper();

            if (valueLength <= 2 && IsWordUpper(value))
                return value;

            return value.Substring(0, 1).ToUpper() + value.Substring(1, valueLength - 1).ToLower();
        }

        private static bool IsWordUpper(string word)
        {
            return word.All(c => !char.IsLower(c));
        }
    }
}