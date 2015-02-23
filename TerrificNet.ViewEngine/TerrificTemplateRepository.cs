using System.Collections.Generic;
using System.Linq;
using TerrificNet.ViewEngine.IO;

namespace TerrificNet.ViewEngine
{
	public class TerrificTemplateRepository : ITemplateRepository
	{
		private readonly IFileSystem _fileSystem;

	    public TerrificTemplateRepository(IFileSystem fileSystem)
		{
		    _fileSystem = fileSystem;
		}

	    public bool TryGetTemplate(string id, out TemplateInfo templateInfo)
		{
		    var fileName = id;
		    var templates = GetAll().ToDictionary(f => f.Id, f => f);
		    return templates.TryGetValue(fileName, out templateInfo);
		}

	    private IEnumerable<TemplateInfo> Read()
	    {
			return _fileSystem.DirectoryGetFiles(null, "html").Select(f =>
			{
			    var relativePath = GetTemplateId(f).TrimStart('/');
                return new FileTemplateInfo(relativePath, f, _fileSystem); 
            });
	    }

	    private string GetTemplateId(string info)
	    {
	        return _fileSystem.Path.Combine(_fileSystem.Path.GetDirectoryName(info), _fileSystem.Path.GetFileNameWithoutExtension(info));
	    }

	    public IEnumerable<TemplateInfo> GetAll()
	    {
	        return Read();
	    }
	}
}