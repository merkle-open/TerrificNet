using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace TerrificNet.Host
{
    class Program
    {
        static void Main(string[] args)
        {
            var rootDirectory = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            var configuration = ReadConfiguration(rootDirectory);

            var logger = new ConsoleLogger();
            var installers = configuration.Projects.Values.Select(s => new ApplicationInstaller(s) { Logger = logger });
            var workspace = new ApplicationWorkspace(configuration.Workspace)
            {
                Logger = logger
            };
            InstallAll(workspace, installers.ToArray()).Wait();

            Console.Read();
        }

        private static void ClearProjects(ApplicationWorkspace workspace)
        {
            workspace.ClearProjects();
        }

        private static void ClearRepository(ApplicationWorkspace workspace)
        {
            workspace.ClearLocalRepository();
        }

        private static Configuration ReadConfiguration(string rootDirectory)
        {
            using (var stream = new StreamReader(Path.Combine(rootDirectory, "config.json")))
            {
                var configuration = new JsonSerializer().Deserialize<Configuration>(new JsonTextReader(stream));

                configuration.Workspace.RootDirectory = rootDirectory;
                foreach (var project in configuration.Projects)
                {
                    project.Value.Name = project.Key;
                }

                return configuration;
            }
        }

        private static async Task InstallAll(ApplicationWorkspace workspace, params ApplicationInstaller[] installers)
        {
            await Task.WhenAll(installers.Select(i => i.Install(workspace)));
        }
    }
}
