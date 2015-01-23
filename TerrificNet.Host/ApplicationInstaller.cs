using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using NuGet;

namespace TerrificNet.Host
{
    class ApplicationInstaller
    {
        private readonly ApplicationSettings _settings;

        public ApplicationInstaller(ApplicationSettings settings)
        {
            _settings = settings;
        }

        public Task Install(ApplicationWorkspace workspace)
        {
            var repo = PackageRepositoryFactory.Default.CreateRepository(_settings.PackageSource);
            var localRepo = workspace.GetLocalRepository();
            var fileSystem = workspace.GetFileSystem();
            var packageManager = new PackageManager(repo, new DefaultPackagePathResolver(fileSystem), fileSystem, localRepo);

            packageManager.Logger = new ConsoleLogger();
            packageManager.PackageInstalled += (s, args) => InstallIntoWorkspace(workspace, args.Package);
            packageManager.InstallPackage(_settings.PackageId, null, false, false);

            return Task.FromResult<object>(null);
        }

        private void InstallIntoWorkspace(ApplicationWorkspace workspace, IPackage package)
        {
            var projectDirectory = workspace.GetProjectDirectory(_settings);
            if (!Directory.Exists(projectDirectory))
                Directory.CreateDirectory(projectDirectory);

            foreach (var libFile in FilterAssemblyReferences(package.AssemblyReferences))
            {
                using (var file = new FileStream(Path.Combine(projectDirectory, libFile.Name), FileMode.Create, FileAccess.ReadWrite))
                {
                    libFile.GetStream().CopyTo(file);
                }
            }
        }

        private static IEnumerable<IPackageAssemblyReference> FilterAssemblyReferences(IEnumerable<IPackageAssemblyReference> assemblyReferences)
        {
            if (assemblyReferences != null)
            {
                IEnumerable<IPackageAssemblyReference> compatibleItems;
                if (VersionUtility.TryGetCompatibleItems(VersionUtility.DefaultTargetFramework, assemblyReferences, out compatibleItems))
                {
                    return compatibleItems;
                }
            }
            return Enumerable.Empty<IPackageAssemblyReference>();
        }
    }
}