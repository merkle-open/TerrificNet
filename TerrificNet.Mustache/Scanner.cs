using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace TerrificNet.Mustache
{
    public class Scanner
    {
        private static readonly Regex DelimitersRegex = new Regex(@"^=\s*(\S+)\s+(\S+)\s*=$");

        public class Literal
        {
            public LiteralType Type { get; set; }
            public string Content { get; set; }
            public int Line { get; set; }
            public int Column { get; set; }

            public Literal(LiteralType type, string content, int line = 0, int column = 0)
            {
                Type = type;
                Content = content;
                Line = line;
                Column = column;
            }
        }

        public enum LiteralType
        {
            Text,
            BlockStart,
            InvertBlockStart,
            TemplateDefinition,
            EndSection,
            TemplateInclude,
            VariableReference
        }

        public IEnumerable<Literal> Scan(string template)
        {
            var regex = MakeRegex("{{", "}}");
            var i = 0;
            var lineEnded = false;

            while (true)
            {
                Match m;

                if ((m = regex.Match(template, i)).Success)
                {
                    var previousLiteral = template.Substring(i, m.Index - i);

                    var leadingWhiteSpace = m.Groups[1];
                    var leadingLineEnd = m.Groups[2];
                    var leadingWhiteSpaceOnly = m.Groups[3];
                    var marker = m.Groups[4].Value.Trim();
                    var trailingWhiteSpace = m.Groups[5];
                    var trailingLineEnd = m.Groups[6];

                    var isStandalone = (leadingLineEnd.Success || (lineEnded && m.Index == i)) && trailingLineEnd.Success;

                    Literal part = null;

                    if (marker[0] == '=')
                    {
                        var delimiters = DelimitersRegex.Match(marker);

                        if (delimiters.Success)
                        {
                            var start = delimiters.Groups[1].Value;
                            var end = delimiters.Groups[2].Value;
                            regex = MakeRegex(start, end);
                        }
                    }
                    else if (marker[0] == '#')
                    {
                        part = new Literal(LiteralType.BlockStart, marker.Substring(1).Trim(), m.Index);
                    }
                    else if (marker[0] == '^')
                    {
                        part = new Literal(LiteralType.InvertBlockStart, marker.Substring(1).Trim(), m.Index);
                    }
                    else if (marker[0] == '<')
                    {
                        part = new Literal(LiteralType.TemplateDefinition, marker.Substring(1).Trim(), m.Index);
                    }
                    else if (marker[0] == '/')
                    {
                        part = new Literal(LiteralType.EndSection, marker.Substring(1).Trim(), m.Index);
                    }
                    else if (marker[0] == '>')
                    {
                        part = new Literal(LiteralType.TemplateInclude, marker.Substring(1).Trim(), m.Index/*, lineEnded || i == 0 ? leadingWhiteSpaceOnly.Value : null*/);
                    }
                    else if (marker[0] != '!')
                    {
                        if (marker == "else")
                        {
                            part = new Literal(LiteralType.BlockStart, marker, m.Index);
                        }
                        else
                        {
                            part = new Literal(LiteralType.VariableReference, marker, m.Index);
                            isStandalone = false;
                        }
                    }

                    if (!isStandalone)
                    {
                        previousLiteral += leadingWhiteSpace;
                    }
                    else
                    {
                        previousLiteral += leadingLineEnd;

                        if (part.Type == LiteralType.TemplateInclude)
                        {
                            previousLiteral += leadingWhiteSpaceOnly;
                        }
                    }

                    if (previousLiteral != "")
                    {
                        yield return new Literal(LiteralType.Text, previousLiteral, m.Index);
                    }

                    if (part != null)
                    {
                        yield return part;
                    }

                    i = m.Index + m.Length;

                    if (!isStandalone)
                    {
                        i -= trailingWhiteSpace.Length;
                    }

                    lineEnded = trailingLineEnd.Success;
                }
                else
                {
                    break;
                }
            }

            if (i < template.Length)
            {
                var remainingLiteral = template.Substring(i);

                yield return new Literal(LiteralType.Text, remainingLiteral);
            }
        }

        private static Regex MakeRegex(string start, string end)
        {
            return new Regex(
                @"((^|\r?\n)?([\r\t\v ]*))" +
                Regex.Escape(start) +
                @"([\{]?[^" + Regex.Escape(end.Substring(0, 1)) + @"]+?\}?)" +
                Regex.Escape(end) +
                @"([\r\t\v ]*(\r?\n|$)?)"
            );
        }
    }
}
