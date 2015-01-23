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

            var installers = configuration.Projects.Values.Select(s => new ApplicationInstaller(s));
            InstallAll(configuration.Workspace, installers.ToArray()).Wait();

            Console.Read();
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

        private static async Task InstallAll(ApplicationWorkspaceSettings wsSettings, params ApplicationInstaller[] installers)
        {
            var workspace = new ApplicationWorkspace(wsSettings);
            await Task.WhenAll(installers.Select(i => i.Install(workspace)));
        }
    }
}
