using System;

namespace TerrificNet.ViewEngine.IO
{
	public interface IPathHelper
	{
		PathInfo Combine(params PathInfo[] parts);
        PathInfo GetDirectoryName(PathInfo filePath);
        PathInfo ChangeExtension(PathInfo fileName, string extension);
        PathInfo GetFileNameWithoutExtension(PathInfo path);
		string GetExtension(PathInfo path);
	}

    public class PathInfo : IEquatable<PathInfo>
    {
        private readonly string _path;

        private PathInfo(string path)
        {
            _path = path;
        }

        public static PathInfo Create(string path)
        {
            return new PathInfo(path);
        }

        public override string ToString()
        {
            return _path;
        }

        public PathInfo TrimStart(char c)
        {
            return Create(_path.TrimStart(c));
        }

        public bool StartsWith(PathInfo directory)
        {
            return this.ToString().StartsWith(directory.ToString(), StringComparison.OrdinalIgnoreCase);
        }

        public bool EndsWith(string fileExtension)
        {
            return this.ToString().EndsWith(fileExtension, StringComparison.OrdinalIgnoreCase);
        }

        public override bool Equals(object obj)
        {
            if (obj == null)
                return false;

            if (object.ReferenceEquals(this, obj))
                return true;

            var p = obj as PathInfo;
            if (p != null)
                return this.Equals(p);

            return false;
        }

        public bool Equals(PathInfo other)
        {
            return this._path.Equals(other._path);
        }

        public override int GetHashCode()
        {
            return _path.GetHashCode();
        }
    }
}