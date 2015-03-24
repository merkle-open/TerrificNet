using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace Veil.Handlebars
{
    internal static class HandlebarsTokenizer
    {
        private static readonly Regex handlebars = new Regex(@"(?<!{)({{[^{}]+}})|({{{[^{}]+}}})(?!})", RegexOptions.Compiled);

        public static IEnumerable<HandlebarsToken> Tokenize(TextReader templateReader)
        {
            var template = templateReader.ReadToEnd();
            var matches = handlebars.Matches(template);
            var index = 0;
            foreach (Match match in matches)
            {
                if (index < match.Index)
                {
                    yield return new HandlebarsToken(false, template.Substring(index, match.Index - index), false, false, false, index);
                }

                var token = match.Value.Trim();
                var isHtmlEscape = token.Count(c => c == '{') == 2;
                token = token.Trim('{', '}');
                var trimLastLiteral = token.StartsWith("~");
                var trimNextLiteral = token.EndsWith("~");
                token = token.Trim('~').Trim();
                yield return new HandlebarsToken(true, token, isHtmlEscape, trimLastLiteral, trimNextLiteral, index);

                index = match.Index + match.Length;
            }
            if (index < template.Length)
            {
                yield return new HandlebarsToken(false, template.Substring(index), false, false, false, index);
            }
        }
    }

    internal struct HandlebarsToken
    {
	    private bool isSyntaxToken;
        private string content;
        private bool isHtmlEscape;
        private bool trimLastLiteral;
        private bool trimNextLiteral;
	    private int _index;

        public HandlebarsToken(bool isSyntaxToken, string content, bool isHtmlEscape, bool trimLastLiteral, bool trimNextLiteral, int index)
        {
	        this.isSyntaxToken = isSyntaxToken;
            this.content = content;
            this.isHtmlEscape = isHtmlEscape;
            this.trimLastLiteral = trimLastLiteral;
            this.trimNextLiteral = trimNextLiteral;

			_index = index;
        }

		public int Position { get { return _index; } }

        public bool IsSyntaxToken { get { return this.isSyntaxToken; } }

        public string Content { get { return this.content; } }

        public bool IsHtmlEscape { get { return this.isHtmlEscape; } }

        public bool TrimLastLiteral { get { return this.trimLastLiteral; } }

        public bool TrimNextLiteral { get { return this.trimNextLiteral; } }
    }
}