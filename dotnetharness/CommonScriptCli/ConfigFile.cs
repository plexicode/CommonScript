using System.Collections.Generic;

namespace CommonScriptCli
{
    internal class ConfigFile
    {
        public string MainSource { get; private set; } = null;
        public Dictionary<string, string> ModuleDirectoriesById { get; private set; } = new Dictionary<string, string>();

        public static ConfigFile Parse(string path)
        {
            string absPath = System.IO.Path.GetFullPath(path);
            string content = DiskUtil.TryFileReadText(absPath) ?? throw new UserFacingException("Config file does not exist: " + absPath);
            string projDir = System.IO.Path.GetDirectoryName(absPath)!;

            ConfigFile output = new ConfigFile();

            string[] lines = content.Split('\n');
            for (int i = 0; i < lines.Length; i++)
            {
                int lineNum = i + 1;
                string line = lines[i].Trim();
                if (line.StartsWith('#') || line == "") continue;
                string[] parts = line.Split(':', 2);
                string key = parts[0].Trim().ToUpperInvariant();
                string value = parts[1].Trim();
                if (parts.Length != 2) ThisLineIsInvalid(absPath, lineNum, "A key and value is required.");
                switch (key)
                {
                    case "MAIN":
                        output.MainSource = DiskUtil.FlexibleCombine(projDir, value);
                        break;

                    case "MODULE":
                        string[] subparts = value.Split(':', 2);
                        if (subparts.Length != 2) ThisLineIsInvalid(absPath, lineNum, "Modules require an ID and a path.");
                        output.ModuleDirectoriesById[subparts[0].Trim()] = DiskUtil.FlexibleCombine(projDir, subparts[1].Trim());
                        break;

                    default:
                        ThisLineIsInvalid(absPath, lineNum, "Unrecognized config option: '" + key + "'");
                        break;
                }
            }

            return output;
        }

        private static void ThisLineIsInvalid(string path, int line, string? msg)
        {
            string fullMsg = "Line " + line + " in the config file " + path + " is invalid";
            if (msg != null) fullMsg += ": " + msg;
            throw new UserFacingException(fullMsg);
        }
    }
}
