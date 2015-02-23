using System;
using System.IO;
using System.Linq;
using System.Text;

namespace TerrificNet.ViewEngine.IO
{
    public class FileSystemProvider
    {
        public IFileSystem GetFileSystem(string hostPath, string path, out string basePath)
        {
            IFileSystem fileSystem;
            if (path.StartsWith("zip://"))
            {
                string baseInFile;
                var filePath = GetFilePath(new Uri(path, UriKind.RelativeOrAbsolute), out baseInFile);
                if (!Path.IsPathRooted(filePath))
                    filePath = Path.GetFullPath(Path.Combine(hostPath, filePath));
                else
                    filePath = Path.GetFullPath(filePath);

                fileSystem = CreateZipFileSystem(filePath, baseInFile);
                basePath = string.Empty;
            }
            else
            {
                //fileSystem = new CachedFileSystem();
                basePath = Path.Combine(hostPath, path);
                fileSystem = new FileSystem(basePath);

                basePath = string.Empty;
            }
            return fileSystem;
        }

        protected internal virtual ZipFileSystem CreateZipFileSystem(string filePath, string baseInFile)
        {
            return new ZipFileSystem(filePath, baseInFile);
        }

        private static string GetFilePath(Uri uri, out string rootPath)
        {
            var pathBuilder = new StringBuilder();
            string result = String.Empty;

            var host = uri.IsAbsoluteUri ? new[] {uri.Host} : new string[0];

            foreach (var segment in host.Union(uri.Segments).Where(s => !string.IsNullOrEmpty(s) && s != "/"))
            {
                var part = segment.TrimEnd('/');
                pathBuilder.Append(part);

                if (!String.IsNullOrEmpty(Path.GetExtension(part)))
                {
                    result = pathBuilder.ToString();
                    pathBuilder.Clear();
                }
                else
                    pathBuilder.Append('/');
            }

            rootPath = pathBuilder.ToString();

            return result;
        }
    }
}