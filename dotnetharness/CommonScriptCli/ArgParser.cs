using System.Collections.Generic;

namespace CommonScriptCli
{
    /*
        Args prepended with XCS: are execution arguments for Common Script CLI itself.
        Each argument that requires more information is defined with the next argument in the list.
        All other unaccounted args are passed sequentially to the user application.
    */
    internal class ParsedArgs
    {
        private enum CliArg
        {
            UNKNOWN,

            // The source directory for the main module. Multiple definitions of this flag
            // will overwrite previous definitions. If not specified, the current directory
            // is treated as the source directory.
            // --XCS:source path/to/source/directory
            SOURCE_DIR,

            // A directory containing a module name. The value is in the format of
            // {modulename}:{path to module} (without curly braces)
            // Multiple definitions are allowed.
            // --XCS:module helper:path/to/helper/lib
            MODULE_INFO,

            // A directory to emit the byte code. This prevents execution.
            // --XCS:bytecode path/to/bytecode/folder
            BYTECODE_OUT_DIR,

            // A path to a configuration file that contains values for the above arguments.
            // The contents of the config file are treated as though they were arguments
            // provided via command line.
            // --XCS:config path/to/config/file.config
            CONFIG_FILE,
        }

        private static readonly Dictionary<string, CliArg> FLAG_TO_ARG = new Dictionary<string, CliArg>() {
            { "-XCS:s", CliArg.SOURCE_DIR },
            { "--XCS:source", CliArg.SOURCE_DIR },
            { "-XCS:m", CliArg.MODULE_INFO },
            { "--XCS:module", CliArg.MODULE_INFO },
            { "-XCS:bc", CliArg.BYTECODE_OUT_DIR },
            { "--XCS:bytecode", CliArg.BYTECODE_OUT_DIR },
            { "-XCS:c", CliArg.CONFIG_FILE },
            { "--XCS:config", CliArg.CONFIG_FILE },
        };

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

        private static CliArg GetArgType(string rawValue)
        {
            return FLAG_TO_ARG.TryGetValue(rawValue, out CliArg output)
                ? output
                : CliArg.UNKNOWN;
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
