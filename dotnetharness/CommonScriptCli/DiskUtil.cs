using System.Collections.Generic;
using System.Linq;

namespace CommonScriptCli
{
    internal class DiskUtil
    {
        public static void EnsureDirectoryExists(string path)
        {
            string absPath = System.IO.Path.GetFullPath(path);
            EnsureDirectoryExistsImpl(absPath);
        }

        private static void EnsureDirectoryExistsImpl(string path)
        {
            if (System.IO.Directory.Exists(path)) return;
            string? parent = System.IO.Path.GetDirectoryName(path);
            if (parent == null) return;
            EnsureDirectoryExists(parent);
            System.IO.Directory.CreateDirectory(path);
        }

        public static Dictionary<string, string>? GatherFilesAsDictionary(string path, string extension)
        {
            if (!extension.StartsWith(".")) extension = "." + extension;
            extension = extension.ToLowerInvariant();
            string[]? files = GatherFiles(path);
            if (files == null) return null;
            Dictionary<string, string> output = new Dictionary<string, string>();

            foreach (string relPath in files.Where(f => f.ToLowerInvariant().EndsWith(extension)))
            {
                string absPath = System.IO.Path.GetFullPath(System.IO.Path.Combine(path, relPath).Replace('/', System.IO.Path.DirectorySeparatorChar));
                string content = System.IO.File.ReadAllText(absPath);
                output[relPath] = content;
            }
            return output;
        }

        // Returns null if directory not found.
        public static string[]? GatherFiles(string path)
        {
            string absPath = System.IO.Path.GetFullPath(path);
            if (!System.IO.Directory.Exists(absPath))
            {
                return null;
            }
            List<string> output = new List<string>();
            GatherFilesImpl(absPath, "", output);
            return output.ToArray();
        }

        private static HashSet<string> IGNORE_FILES = new HashSet<string>([
            ".git", ".DS_Store", "desktop.ini", "thumbs.db",
        ]);

        private static void GatherFilesImpl(string abs, string rel, List<string> output)
        {
            foreach (string childAbsPath in System.IO.Directory.GetDirectories(abs))
            {
                string name = System.IO.Path.GetFileName(childAbsPath);
                if (IGNORE_FILES.Contains(name)) continue;
                string childRelPath = rel == "" ? name : (rel + '/' + name);
                GatherFilesImpl(childAbsPath, childRelPath, output);
            }

            foreach (string childAbsPath in System.IO.Directory.GetFiles(abs))
            {
                string name = System.IO.Path.GetFileName(childAbsPath);
                if (IGNORE_FILES.Contains(name)) continue;
                string childRelPath = rel == "" ? name : (rel + '/' + name);
                output.Add(childRelPath);
            }
        }
    }
}
