using System;
using Antlr4.Runtime;

namespace TerrificNet.Mustache.Antlr
{
	partial class MustacheLexer : Lexer
	{
		String start = "{{";
		String end = "}}";
		bool whiteSpaceControl;

		private bool isWhite(int ch)
		{
			return ch == ' ' || ch == '\t' || ch == '\r' || ch == '\n';
		}

		private bool consumeUntil(string token)
		{
			int offset = 0;
			while (!isEOF(offset) && !ahead(token, offset) &&
			  !isWhite(_input.La(offset + 1)))
			{
				offset += 1;
			}
			if (offset == 0)
			{
				return false;
			}
			// Since we found the text, increase the CharStream's index.
			_input.Seek(_input.Index + offset - 1);
			return true;
		}

		private bool comment(String start, String end)
		{
			String commentClose;
			if (ahead(start + "!--"))
			{
				commentClose = "--" + end;
			}
			else if (ahead(start + "!"))
			{
				commentClose = end;
			}
			else
			{
				return false;
			}

			int offset = 0;
			while (!isEOF(offset))
			{
				if (ahead(commentClose, offset))
				{
					break;
				}
				offset += 1;
			}
			offset += commentClose.Length;
			// Since we found the text, increase the CharStream's index.
			_input.Seek(_input.Index + offset - 1);

			return true;
		}

		private bool varEscape(String start, String end)
		{
			if (ahead("\\" + start))
			{
				int offset = start.Length;
				while (!isEOF(offset))
				{
					if (ahead(end, offset))
					{
						break;
					}
					if (ahead(start, offset))
					{
						return false;
					}
					offset += 1;
				}
				offset += end.Length;
				// Since we found the text, increase the CharStream's index.
				_input.Seek(_input.Index + offset - 1);

				return true;
			}
			return false;
		}

		private bool startToken(String delim)
		{
			var matches = tryToken(delim + "~");
			if (matches)
			{
				whiteSpaceControl = true;
			}
			return matches || tryToken(delim);
		}

		private bool startToken(String delim, String subtype)
		{
			bool matches = tryToken(delim + subtype);
			if (!matches)
			{
				matches = tryToken(delim + "~" + subtype);
				if (matches)
				{
					whiteSpaceControl = true;
				}
			}
			return matches;
		}

		private bool endToken(String delim)
		{
			return endToken(delim, "");
		}

		private bool endToken(String delim, String subtype)
		{
			var matches = tryToken(subtype + delim);
			if (!matches)
			{
				matches = tryToken(subtype + "~" + delim);
				if (matches)
				{
					whiteSpaceControl = true;
				}
			}
			return matches;
		}

		private bool tryToken(String text)
		{
			if (ahead(text))
			{
				// Since we found the text, increase the CharStream's index.
				_input.Seek(_input.Index + text.Length - 1);
				return true;
			}
			return false;
		}

		private bool isEOF(int offset)
		{
			return _input.Size < offset + 1;
		}

		private bool ahead(String text)
		{
			return ahead(text, 0);
		}

		private bool ahead(String text, int offset)
		{
			// See if `text` is ahead in the CharStream.
			for (int i = 0; i < text.Length; i++)
			{
				int ch = _input.La(i + offset + 1);
				if (ch != text[i])
				{
					// Nope, we didn't find `text`.
					return false;
				}
			}

			return true;
		}
	}
}
