using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace TerrificNet.Host
{
    class Program
    {
        static void Main(string[] args)
        {
            var settings = new ApplicationSettings
            {
                PackageSource = "https://nuget.org/api/v2/",
                PackageId = "EntityFramework"
            };

            var wsSettings = new ApplicationWorkspaceSettings(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "projects", "packages");
            var applicationInstaller = new ApplicationInstaller(settings);

            InstallAll(wsSettings, applicationInstaller).Wait();

            Console.Read();
        }

        private static async Task InstallAll(ApplicationWorkspaceSettings wsSettings, params ApplicationInstaller[] installers)
        {
            var workspace = new ApplicationWorkspace(wsSettings);
            await Task.WhenAll(installers.Select(i => i.Install(workspace)));
        }
    }
}
