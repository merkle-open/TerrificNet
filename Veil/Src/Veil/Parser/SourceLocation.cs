namespace Veil.Parser
{
	public class SourceLocation
	{
		public string TemplateId { get; private set; }
		public int Row { get; private set; }
		public int Column { get; private set; }

		public SourceLocation(string templateId, int row, int column)
		{
			TemplateId = templateId;
			Row = row;
			Column = column;
		}
	}
}