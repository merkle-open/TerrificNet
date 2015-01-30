using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TerrificNet.ViewEngine
{
    internal class PathUtility
    {
        private class PathToken
        {
            public char Chararcter { get; private set; }
            internal TokenType Type { get; private set; }

            public PathToken(TokenType type)
            {
                Type = type;
            }

            public PathToken(char chararcter)
                : this(TokenType.Part)
            {
                Chararcter = chararcter;
            }
        }

        private enum TokenType
        {
            Part,
            Seperator,
            Root,
            Parent,
            Self,
            Begin
        }

        public static string Combine(params string[] parts)
        {
            return CombineTokens(TokenizePath(parts));
        }

        private static string CombineTokens(IEnumerable<PathToken> pathTokens)
        {
            var builder = new StringBuilder();
            PathToken lastLoken = null;
            foreach (var token in pathTokens)
            {
                if (lastLoken != null && token.Type == TokenType.Root)
                    throw new ArgumentException("Can not combine root paths");

                if (token.Type == TokenType.Seperator && lastLoken != null && lastLoken.Type == TokenType.Seperator)
                    throw new ArgumentException("Duplicated path seperator");

                if (token.Type == TokenType.Root)
                {
                    builder.Append('/');
                    lastLoken = token;
                }
                else if (token.Type == TokenType.Begin && lastLoken != null && lastLoken.Type != TokenType.Seperator)
                {
                    builder.Append('/');
                    lastLoken = token;
                }
                else if (token.Type == TokenType.Seperator)
                {
                    builder.Append('/');
                    lastLoken = token;
                }
                else if (token.Type == TokenType.Part)
                {
                    builder.Append(token.Chararcter);
                    lastLoken = token;
                }
            }

            if (builder.Length > 0)
                builder.Remove(builder.Length - 1, 1);

            return builder.ToString();
        }

        private static IEnumerable<PathToken> TokenizePath(params string[] parts)
        {
            var queue = new Stack<PathToken>();

            foreach (var part in parts)
            {
                if (string.IsNullOrEmpty(part))
                    continue;

                var enumerator = part.GetEnumerator();
                bool moveNext = enumerator.MoveNext();
                queue.Push(new PathToken(TokenType.Begin));

                while (moveNext)
                {
                    PathToken token;
                    moveNext = Move(enumerator, queue.Peek(), out token);

                    if (token.Type == TokenType.Self)
                    {
                        DequeueToSeperator(queue);
                    }
                    else if (token.Type == TokenType.Parent)
                    {
                        MoveOverSeperator(queue);
                        DequeueToSeperator(queue);
                    }
                    else
                        queue.Push(token);
                }

                if (queue.Peek().Type != TokenType.Seperator)
                    queue.Push(new PathToken(TokenType.Seperator));
            }

            return queue.Reverse();
        }

        private static void DequeueToSeperator(Stack<PathToken> queue)
        {
            if (queue.Count == 0)
                throw new ArgumentException("One of the parts leaves the root path");

            while (queue.Count > 1 && queue.Peek().Type != TokenType.Seperator)
            {
                queue.Pop();
            }
        }

        private static void MoveOverSeperator(Stack<PathToken> queue)
        {
            DequeueToSeperator(queue);

            if (queue.Peek().Type == TokenType.Seperator)
                queue.Pop();
        }

        private static bool Move(IEnumerator<char> enumerator, PathToken lastToken, out PathToken token)
        {
            token = null;

            if (enumerator.Current == '\\' || enumerator.Current == '/')
            {
                if (lastToken.Type == TokenType.Begin)
                    token = new PathToken(TokenType.Root);
                else
                    token = new PathToken(TokenType.Seperator);

                return enumerator.MoveNext();
            }

            if (enumerator.Current == '.' && (lastToken.Type == TokenType.Begin || lastToken.Type == TokenType.Seperator))
            {
                if (enumerator.MoveNext())
                {
                    if (enumerator.Current == '\\' || enumerator.Current == '/')
                    {
                        token = new PathToken(TokenType.Self);
                        return enumerator.MoveNext();
                    }

                    if (enumerator.Current == '.')
                    {
                        if (enumerator.MoveNext() && (enumerator.Current == '\\' || enumerator.Current == '/'))
                        {
                            token = new PathToken(TokenType.Parent);
                            return enumerator.MoveNext();
                        }
                        throw new InvalidOperationException("A relative path must be ../");
                    }

                    token = new PathToken(enumerator.Current);
                    return enumerator.MoveNext();
                }
                return false;
            }

            token = new PathToken(enumerator.Current);
            return enumerator.MoveNext();
        }

        public static string GetDirectoryName(string filePath)
        {
            return CombineTokens(TokenizePath(filePath).Reverse().Skip(1).SkipWhile(t => t.Type != TokenType.Seperator).Reverse());
        }
    }
}