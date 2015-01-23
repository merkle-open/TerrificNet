namespace TerrificNet.Host
{
    public class ApplicationWorkspaceSettings
    {
        private readonly string _rootDirectory;
        private readonly string _projectsDirectoryName;
        private readonly string _packagesDirectoryName;

        public ApplicationWorkspaceSettings(string rootDirectory, string projectsDirectoryName, string packagesDirectoryName)
        {
            _rootDirectory = rootDirectory;
            _projectsDirectoryName = projectsDirectoryName;
            _packagesDirectoryName = packagesDirectoryName;
        }

        public string RootDirectory
        {
            get { return _rootDirectory; }
        }

        public string ProjectsDirectoryName
        {
            get { return _projectsDirectoryName; }
        }

        public string PackagesDirectoryName
        {
            get { return _packagesDirectoryName; }
        }
    }
}