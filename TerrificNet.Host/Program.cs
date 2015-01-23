using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using NuGet;

namespace TerrificNet.Host
{
    class Program
    {
        static void Main(string[] args)
        {
            string packageSource = "https://nuget.org/api/v2/";
            string packageId = "EntityFramework";

            var repo = PackageRepositoryFactory.Default.CreateRepository(packageSource);
            var localRepo = PackageRepositoryFactory.Default.CreateRepository(InstallationPath);
            var projectDir = Path.Combine(RootDirectory, "run");
            var packageManager = new PackageManager(repo, InstallationPath);

            packageManager.PackageInstalled += packageManager_PackageInstalled;
            packageManager.InstallPackage(packageId, null, false, false);
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

        private static string InstallationPath
        {
            get
            {
                return Path.Combine(RootDirectory, "packages");
            }
        }

        private static string RootDirectory
        {
            get { return Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location); }
        }

        static void packageManager_PackageInstalled(object sender, PackageOperationEventArgs e)
        {
            foreach (var libFile in FilterAssemblyReferences(e.Package.AssemblyReferences))
            {
                using (var file = new FileStream(Path.Combine(InstallationPath, libFile.Name), FileMode.Create, FileAccess.ReadWrite))
                {
                    libFile.GetStream().CopyTo(file);
                }
            }
        }
    }
}
