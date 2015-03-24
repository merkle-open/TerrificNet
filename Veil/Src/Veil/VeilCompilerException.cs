using System;
using Veil.Parser;

namespace Veil
{
    /// <summary>
    /// Represent general errors during compilation of templates
    /// </summary>
    public class VeilCompilerException : Exception
    {
	    public SyntaxTreeNode Node { get; private set; }

	    /// <summary>
        /// Creates an exception with the supplied messages
        /// </summary>
		public VeilCompilerException(string message, SyntaxTreeNode node)
            : base(message)
	    {
		    Node = node;
	    }
    }
}