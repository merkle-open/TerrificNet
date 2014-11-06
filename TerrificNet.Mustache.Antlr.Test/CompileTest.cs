using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace TerrificNet.Mustache.Antlr.Test
{
	[TestClass]
	public class CompileTest
	{
		private readonly object _model = new
		{
			name = "Hans Muster",
			age = 20,
			address = new { street = "Backer 1" },
			status = false,
			status2 = true
		};

		[TestMethod]
		public void TestString()
		{
			const string template = "Hallo {{#if age}}Das ist {{name}} ein Test{{/if}}";
			var result = new MustacheTemplateEngine().Compile(template)(_model);

			Assert.AreEqual("Hallo Das ist Hans Muster ein Test", result);
		}

		[TestMethod]
		public void TestComplexVariable()
		{
			const string template = "{{address.street}}";
			var result = new MustacheTemplateEngine().Compile(template)(_model);

			Assert.AreEqual("Backer 1", result);
		}

		[TestMethod]
		public void TestBooleanIf()
		{
			const string template = "{{#if status}}True{{/if}}";
			var result = new MustacheTemplateEngine().Compile(template)(_model);

			Assert.AreEqual("", result);
		}

		[TestMethod]
		public void TestBooleanIfTrue()
		{
			const string template = "{{#if status2}}True{{/if}}";
			var result = new MustacheTemplateEngine().Compile(template)(_model);

			Assert.AreEqual("True", result);
		}
	}
}
