using System.IO;
using NuGet;

namespace TerrificNet.Host
{
    public class ApplicationWorkspace
    {
        private readonly ApplicationWorkspaceSettings _settings;

        public ApplicationWorkspace(ApplicationWorkspaceSettings settings)
        {
            _settings = settings;
        }

        public IFileSystem GetFileSystem()
        {
            return new PhysicalFileSystem(InstallationPath);
        }

        public IPackageRepository GetLocalRepository()
        {
            return PackageRepositoryFactory.Default.CreateRepository(InstallationPath);
        }

        private string InstallationPath
        {
            get { return Path.Combine(RootDirectory, _settings.PackagesDirectoryName); }
        }

        private string RootDirectory
        {
            get { return _settings.RootDirectory; }
        }

        public string GetProjectDirectory(ApplicationSettings settings)
        {
            return Path.Combine(RootDirectory, _settings.ProjectsDirectoryName, settings.Name);
        }
    }
}