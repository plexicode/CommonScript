using System.Collections.Generic;

namespace CommonScriptCli
{
    internal class ParsedArgs
    {
        public string[] UserRuntimeArgs { get; set; } = [];
        public Dictionary<string, string> ModuleDirByName { get; set; } = new Dictionary<string, string>();
        public string SourceDirectory { get; set; } = ".";
        public string? ByteCodeExportPath { get; set; } = null;
        public bool IsByteCodeOnly { get { return this.ByteCodeExportPath != null; } }

        private static void ParseModuleInfo(string argName, string rawArgValue, ParsedArgs argsOut)
        {
            string[] parts = rawArgValue.Split(':', 2);
            if (parts.Length != 2)
            {
                throw new CliArgumentException(argName + " requires a subsequent value of the form {name}:{path} e.g. helper:../../helpercode");
            }

            string name = parts[0].Trim();
            string path = parts[1];

            argsOut.ModuleDirByName[name] = path;
        }

        private enum CliArg
        {
            UNKNOWN,
            SOURCE_DIR,
            MODULE_INFO,
            BYTECODE_OUT_DIR,
            CONFIG_FILE,
        }

        private static CliArg GetArgType(string rawValue)
        {
            switch (rawValue)
            {
                case "-XCS:s":
                case "--XCS:source":
                    return CliArg.SOURCE_DIR;

                case "-XCS:m":
                case "--XCS:module":
                    return CliArg.MODULE_INFO;

                case "-XCS:bc":
                case "--XCS:bytecode":
                    return CliArg.BYTECODE_OUT_DIR;

                case "-XCS:c":
                case "--XCS:config":
                    return CliArg.CONFIG_FILE;

                default:
                    return CliArg.UNKNOWN;
            }
        }

        private static bool ArgTypeRequiresSubsequentValue(CliArg arg)
        {
            return arg != CliArg.UNKNOWN;
        }

        public static ParsedArgs ParseArgs(string[] args)
        {
            ParsedArgs parsedArgs = new ParsedArgs();
            List<string> userArgs = new List<string>();

            for (int i = 0; i < args.Length; i++)
            {
                string arg = args[i];
                CliArg argType = GetArgType(arg);
                if (argType == CliArg.UNKNOWN)
                {
                    userArgs.Add(arg);
                }
                else
                {
                    string argValue = "";
                    if (ArgTypeRequiresSubsequentValue(argType))
                    {
                        if (i + 1 >= args.Length) throw new CliArgumentException(arg + " requires an argument after it.");
                        argValue = args[++i];
                    }

                    switch (argType)
                    {
                        case CliArg.SOURCE_DIR:
                            parsedArgs.SourceDirectory = argValue;
                            break;

                        case CliArg.MODULE_INFO:
                            ParseModuleInfo(arg, argValue, parsedArgs);
                            break;

                        case CliArg.BYTECODE_OUT_DIR:
                            parsedArgs.ByteCodeExportPath = argValue;
                            break;

                        case CliArg.CONFIG_FILE:
                            ConfigFile cfgFile = ConfigFile.Parse(argValue);
                            parsedArgs.SourceDirectory = cfgFile.MainSource ?? parsedArgs.SourceDirectory;
                            foreach (string modId in cfgFile.ModuleDirectoriesById.Keys)
                            {
                                parsedArgs.ModuleDirByName[modId] = cfgFile.ModuleDirectoriesById[modId];
                            }
                            break;
                    }
                }
            }

            parsedArgs.UserRuntimeArgs = userArgs.ToArray();

            return parsedArgs;
        }
    }
}
