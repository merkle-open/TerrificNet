using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;

namespace TerrificNet.ViewEngine.Globalization
{
	public class JsonLabelService : ILabelService
	{
		private readonly IFileSystem _fileSystem;
		private readonly string _fileName;

		public JsonLabelService(IFileSystem fileSystem)
		{
			_fileSystem = fileSystem;
			_fileName = "labels.json";
		}

		private Dictionary<string, string> Load()
		{
			Dictionary<string, string> data;
			using (var stream = new StreamReader(_fileSystem.OpenReadOrCreate(_fileName)))
			{
				data = new JsonSerializer().Deserialize<Dictionary<string, string>>(new JsonTextReader(stream));
			}

			return data ?? new Dictionary<string, string>();
		}

		private void Save(Dictionary<string, string> data)
		{
			using (var stream = new StreamWriter(_fileSystem.OpenWrite(_fileName)))
			{
				new JsonSerializer().Serialize(new JsonTextWriter(stream), data);
			}
		}

		public string Get(string key)
		{
			var data = Load();

			if (!data.ContainsKey(key))
			{
				data[key] = key.Replace("/", "_");
				Save(data);
			}

			return data[key];
		}

		public void Remove(string key)
		{
			var data = Load();
			data.Remove(key);
			Save(data);
		}

		public void Set(string key, string value)
		{
			var data = Load();
			data[key] = value;
			Save(data);
		}
	}
}
