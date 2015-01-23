using Newtonsoft.Json;

namespace TerrificNet.Host
{
    public class ApplicationWorkspaceSettings
    {
        public ApplicationWorkspaceSettings(string rootDirectory, string projectsDirectoryName, string packagesDirectoryName)
        {
            this.RootDirectory = rootDirectory;
            this.ProjectsDirectoryName = projectsDirectoryName;
            this.PackagesDirectoryName = packagesDirectoryName;
        }

        [JsonIgnore]
        public string RootDirectory { get; set; }

        [JsonProperty("projectsDirectoryName")]
        public string ProjectsDirectoryName { get; set; }

        [JsonProperty("packagesDirectoryName")]
        public string PackagesDirectoryName { get; set; }
    }
}