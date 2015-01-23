using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Versioning;
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
            var repo = workspace.GetRemoteRepository();
            var localRepo = workspace.GetLocalRepository();
            var fileSystem = workspace.GetFileSystem();
            var packageManager = new PackageManager(repo, new DefaultPackagePathResolver(fileSystem), fileSystem, localRepo);

            packageManager.Logger = new ConsoleLogger();
            packageManager.PackageInstalled += (s, args) => InstallIntoWorkspace(workspace, args.Package);
            packageManager.InstallPackage(_settings.PackageId, null, false, true);

            return Task.FromResult<object>(null);
        }

        private void InstallIntoWorkspace(ApplicationWorkspace workspace, IPackage package)
        {
            var projectDirectory = workspace.GetProjectDirectory(_settings);
            if (!Directory.Exists(projectDirectory))
                Directory.CreateDirectory(projectDirectory);

            foreach (var libFile in FilterCompatibles(package.AssemblyReferences))
            {
                using (var file = new FileStream(Path.Combine(projectDirectory, libFile.Name), FileMode.Create, FileAccess.ReadWrite))
                {
                    libFile.GetStream().CopyTo(file);
                }
            }

            if (package.Id == _settings.PackageId) // only install content from base package
            {
                foreach (var contentFile in FilterCompatibles(package.GetContentFiles()))
                {
                    var targetPath = Path.Combine(projectDirectory, contentFile.EffectivePath);
                    if (File.Exists(targetPath))
                    {
                        if (_settings.PreservedFiles.Contains(contentFile.EffectivePath))
                            continue;
                    }

                    var targetDirectory = Path.GetDirectoryName(targetPath);
                    if (!Directory.Exists(targetDirectory))
                        Directory.CreateDirectory(targetDirectory);

                    using (var file = new FileStream(targetPath, FileMode.Create, FileAccess.ReadWrite))
                    {
                        contentFile.GetStream().CopyTo(file);
                    }
                }
            }
        }

        private static IEnumerable<T> FilterCompatibles<T>(IEnumerable<T> assemblyReferences)
            where T : IFrameworkTargetable
        {
            if (assemblyReferences != null)
            {
                IEnumerable<T> compatibleItems;
                if (VersionUtility.TryGetCompatibleItems(GetDefaultFramework(), assemblyReferences, out compatibleItems))
                {
                    return compatibleItems;
                }
            }
            return Enumerable.Empty<T>();
        }

        private static FrameworkName GetDefaultFramework()
        {
            return new FrameworkName(".NETFramework", new Version(4, 5));
        }
    }
}