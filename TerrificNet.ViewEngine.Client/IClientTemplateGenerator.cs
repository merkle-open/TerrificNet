namespace TerrificNet.ViewEngine.Client
{
	public interface IClientTemplateGenerator
	{
		void Generate(TemplateInfo templateInfo, IClientContext clientContext, IClientModel clientModel);
	}

	public interface IClientTemplateGeneratorFactory
	{
		IClientTemplateGenerator Create();
	}
}