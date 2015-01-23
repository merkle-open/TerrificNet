using System.IO;
using System.Linq;
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

        public IPackageRepository GetRemoteRepository()
        {
            return new AggregateRepository(_settings.Repositories.Select(r => PackageRepositoryFactory.Default.CreateRepository(r)));
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

        public void ClearLocalRepository()
        {
            Directory.Delete(InstallationPath);
        }

        public void ClearProjects()
        {
            Directory.Delete(Path.Combine(RootDirectory, _settings.ProjectsDirectoryName));
        }
    }
}