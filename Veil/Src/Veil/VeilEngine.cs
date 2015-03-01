using System;
using System.IO;
using System.Linq.Expressions;
using System.Reflection;
using Veil.Compiler;
using Veil.Helper;
using Veil.Parser;

namespace Veil
{
	/// <summary>
	/// Compiles templates for execution
	/// </summary>
	public class VeilEngine : IVeilEngine
	{
		private static readonly MethodInfo GenericCompileMethod = typeof(VeilEngine).GetMethod("Compile", new [] { typeof(ITemplateParser), typeof(TextReader) });
	    private readonly IHelperHandler[] _helperHandlers;
	    private readonly IMemberLocator _memberLocator;

	    /// <summary>
		/// Creates a VeilEngine with an <see cref="IVeilContext"/> to enable support for Includes/Partials/MasterPages.
		/// </summary>
		public VeilEngine(IHelperHandler[] helperHandlers = null, IMemberLocator memberLocator = null)
		{
	        _helperHandlers = helperHandlers;
	        _memberLocator = memberLocator;
		}

		/// <summary>
		/// Parses and compiles the specified template
		/// </summary>
		/// <typeparam name="T">The type of the model that will be passed to the template</typeparam>
		/// <param name="parserKey">Key of the <see cref="ITemplateParser"/> to use to parse the template.</param>
		/// <param name="templateContents">The contents of the template to compile</param>
		/// <returns>A compiled action ready to be executed as needed to render the template</returns>
		public Action<TextWriter, T> Compile<T>(string parserKey, TextReader templateContents)
		{
		    var parser = GetParser(parserKey);
		    return Compile<T>(parser, templateContents);
		}

	    public Action<TextWriter, T> Compile<T>(ITemplateParser parser, TextReader templateContents)
	    {
            if (templateContents == null)
                throw new ArgumentNullException("templateContents");

            var syntaxTree = parser.Parse(templateContents, typeof(T), _memberLocator, _helperHandlers);
            return new VeilTemplateCompiler<T>(_helperHandlers).Compile(syntaxTree);
        }

	    /// <summary>
		/// Parses and compiles the specified template when the model type is not known
		/// </summary>
		/// <param name="parserKey">Key of the <see cref="ITemplateParser"/> to use to parse the template.</param>
		/// <param name="templateContents">The contents of the template to compile</param>
		/// <param name="modelType">The type of the model that will be passed to the template</param>
		/// <returns>A compiled action that will cast the model before execution</returns>
		public Action<TextWriter, object> CompileNonGeneric(string parserKey, TextReader templateContents, Type modelType)
	    {
	        return CompileNonGeneric(GetParser(parserKey), templateContents, modelType);
	    }

	    public Action<TextWriter, object> CompileNonGeneric(ITemplateParser parser, TextReader templateContents, Type modelType)
	    {
            var typedCompileMethod = GenericCompileMethod.MakeGenericMethod(modelType);
            var compiledTemplate = typedCompileMethod.Invoke(this, new object[] { parser, templateContents });

            var writer = Expression.Parameter(typeof(TextWriter));
            var model = Expression.Parameter(typeof(object));
            var castModel = Expression.Variable(modelType);
            var template = Expression.Constant(compiledTemplate);
            var lambda = Expression.Lambda<Action<TextWriter, object>>(
                Expression.Block(
                    new[] { castModel },
                    Expression.Assign(castModel, Expression.TypeAs(model, modelType)),
                    Expression.Call(template, compiledTemplate.GetType().GetMethod("Invoke"), writer, castModel)
                ),
                writer,
                model
            );
            return lambda.Compile();
	    }

        private static ITemplateParser GetParser(string parserKey)
        {
            if (String.IsNullOrEmpty(parserKey)) throw new ArgumentNullException("parserKey");
            if (!VeilStaticConfiguration.IsParserRegistered(parserKey))
                throw new ArgumentException("A parser with key '{0}' is not registered.".FormatInvariant(parserKey), "parserKey");

            var parser = VeilStaticConfiguration.GetParserInstance(parserKey);
            return parser;
        }

	}
}