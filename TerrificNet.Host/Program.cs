using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Versioning;
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
            var resolver = new DefaultPackagePathResolver(InstallationPath);
            var project = new ConsoleProjectSystem(projectDir);
            var packageManager = new PackageManager(repo, InstallationPath);
            //var projectManager = new ProjectManager(packageManager, resolver, project, localRepo);

            //projectManager.IsInstalled();

            //var package = repo.FindPackage(packageId);
            //if (projectManager.IsInstalled(package))
            //{
            //    projectManager.RemovePackageReference(package, true, true);
            //}

            //projectManager.PackageReferenceAdded += projectManager_PackageReferenceAdded;

            //var package = repo.FindPackagesById(packageId).FilterByPrerelease(false).First();
            //projectManager.AddPackageReference(package, false, false);

            packageManager.PackageInstalled += packageManager_PackageInstalled;
            packageManager.InstallPackage(packageId, null, false, false);

            //foreach (var s in projectManager.LocalRepository.GetPackages())
            //{
            //    if (!projectManager.IsInstalled(s))
            //        packageManager.InstallPackage(s, false, false);
            //}

            //packageManager.InstallPackage(packageId);
            //packageManager.UpdatePackage(packageId, true, false);

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

        static void projectManager_PackageReferenceAdded(object sender, PackageOperationEventArgs e)
        {
            foreach (var libFile in e.Package.GetLibFiles())
            {
                var targetPath = Path.Combine(e.InstallPath, libFile.Path);
                //Directory.CreateDirectory(targetPath);
                //using (Stream outputStream = File.Create(targetPath))
                {
                    //libFile.GetStream().CopyTo(outputStream);
                }
            }
        }

        private class ConsoleProjectSystem : PhysicalFileSystem, IProjectSystem
        {
            public ConsoleProjectSystem(string root) : base(root)
            {
            }

            public dynamic GetPropertyValue(string propertyName)
            {
                return null;
            }

            public void AddReference(string referencePath, Stream stream)
            {
                //string fileName = Path.GetFileName(referencePath);
                //string fullPath = this.GetFullPath(fileName);
                //this.AddFile(fullPath, stream);

                //using (var file = new FileStream(Path.Combine(InstallationPath, Path.GetFileName(referencePath)), FileMode.CreateNew, FileAccess.ReadWrite))
                {
                    //using (var source = new FileStream(referencePath, FileMode.Open))
                    {
                        //stream.CopyTo(file);
                    }
                }
            }

            public override void AddFile(string path, Stream stream)
            {
                base.AddFile(path, stream);
            }

            public override void AddFiles(System.Collections.Generic.IEnumerable<IPackageFile> files, string rootDir)
            {
                base.AddFiles(files, rootDir);
            }

            public override string GetFullPath(string path)
            {
                var fullPath = base.GetFullPath(path);

                return fullPath;
            }

            public void AddFrameworkReference(string name)
            {
            }

            public bool ReferenceExists(string name)
            {
                return this.FileExists(name);
            }

            public void RemoveReference(string name)
            {
            }

            public bool IsSupportedFile(string path)
            {
                //return true;
                return Path.GetExtension(path) == "dll";
            }

            public string ResolvePath(string path)
            {
                return path;
            }

            public void AddImport(string targetFullPath, ProjectImportLocation location)
            {
                
            }

            public void RemoveImport(string targetFullPath)
            {
            }

            public bool FileExistsInProject(string path)
            {
                return this.FileExists(path);
            }

            public FrameworkName TargetFramework
            {
                get { return VersionUtility.DefaultTargetFramework; }
            }

            public string ProjectName
            {
                get { return "gugus"; }
            }

            public bool IsBindingRedirectSupported
            {
                get { return true; }
            }
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
                
                //if (libFile.TargetFramework == VersionUtility.DefaultTargetFramework)
                {
                    using (var file = new FileStream(Path.Combine(InstallationPath, libFile.Name), FileMode.Create, FileAccess.ReadWrite))
                    {
                        libFile.GetStream().CopyTo(file);
                    }
                }
            }
        }
    }
}
