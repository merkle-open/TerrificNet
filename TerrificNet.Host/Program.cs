using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Reflection;
using System.Threading.Tasks;
using Newtonsoft.Json;
using NuGet;
using HttpClient = NuGet.HttpClient;

namespace TerrificNet.Host
{
    class Program
    {
        class MyHttpClient : HttpClient
        {
            private WebRequest _client;

            public MyHttpClient(Uri uri) : base(uri)
            {
                _client = WebRequest.Create(uri);
            }

            public override System.Net.WebResponse GetResponse()
            {
                return _client.GetResponse();

                //return base.GetResponse();
            }
        }

        static void Main(string[] args)
        {
            PackageRepositoryFactory.Default.HttpClientFactory = uri => new MyHttpClient(uri);

            var rootDirectory = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            var configuration = ReadConfiguration(rootDirectory);

            var installers = configuration.Projects.Values.Select(s => new ApplicationInstaller(s));
            var workspace = new ApplicationWorkspace(configuration.Workspace);
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
