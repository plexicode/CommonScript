﻿using CommonScript.Compiler;
using CommonScript.Runtime;
using System.Collections.Generic;
using System;

namespace CommonScriptCli
{
    internal class Program
    {
        private static readonly string CLI_LANG_NAME = "BasicCommonScript";
        private static readonly string CLI_LANG_VER = "0.1.0";

        static void Main(string[] args)
        {
#if DEBUG
            MainImpl(args);
#else
            try
            {
                MainImpl(args);
            }
            catch (UserFacingException ufe)
            {
                System.Console.WriteLine(ufe.Message);
            }
#endif
        }

        private static CompilationResult PerformCompilation(ParsedArgs args, ICollection<string> extensionFunctionNames)
        {
            CompilationEngine engine = new CompilationEngine(CLI_LANG_NAME, CLI_LANG_VER, extensionFunctionNames);

            string mainModuleName = "main";
            AdaptiveCompilation compilation = engine.CreateAdaptiveCompilation(mainModuleName);
            while (!compilation.IsComplete)
            {
                string nextModId = compilation.NextRequiredModule;
                Dictionary<string, string> code;
                if (nextModId == mainModuleName)
                {
                    code = DiskUtil.GatherFilesAsDictionary(args.SourceDirectory, ".script")
                        ?? throw new UserFacingException("Source directory does not exist.");
                }
                else
                {
                    if (!args.ModuleDirByName.TryGetValue(nextModId, out string? modDir))
                    {
                        throw new UserFacingException("Module does not exist: " + nextModId);
                    }
                    code = DiskUtil.GatherFilesAsDictionary(modDir, ".script")
                        ?? throw new UserFacingException("The directory for the module " + nextModId + " does not exist.");
                }
                compilation.ProvideFilesForUserModuleCompilation(nextModId, code);
            }

            return compilation.GetCompilation();
        }

        private static void ExportByteCode(string byteCodePathRaw, byte[] byteCode)
        {
            string byteCodePath = System.IO.Path.GetFullPath(byteCodePathRaw);

            // If a directory is provided, use a default file name of out.cxe
            if (System.IO.Directory.Exists(byteCodePath))
            {
                byteCodePath = System.IO.Path.Combine(byteCodePath, "out.cxe");
            }

            string byteCodeParent = System.IO.Path.GetDirectoryName(byteCodePath)!;
            DiskUtil.EnsureDirectoryExists(byteCodeParent);

            System.IO.File.WriteAllBytes(byteCodePath, byteCode);
        }

        private static void MainImpl(string[] cliArgs)
        {
            ParsedArgs args = ParsedArgs.ParseArgs(cliArgs);
            VanillaExtensionSet extensions = new VanillaExtensionSet();

            CompilationResult result = PerformCompilation(args, extensions.ExtensionIds);

            if (result.ErrorMessage != null)
            {
                Console.WriteLine(result.ErrorMessage);
                return;
            }

            byte[] byteCode = result.ByteCodePayload;
            if (args.IsByteCodeOnly)
            {
                ExportByteCode(args.ByteCodeExportPath!, byteCode);
                return;
            }

            RuntimeContext rt = new RuntimeContext(CLI_LANG_NAME, CLI_LANG_VER, byteCode, extensions.GetExtensionsLookup(), args.UserRuntimeArgs);
            CanonicalEventLoop eventLoop = new CanonicalEventLoop(rt);
            extensions.SetEventLoop(eventLoop, rt);

            TaskResult taskResult = eventLoop.StartEventLoop();

            switch (taskResult.Status)
            {
                case TaskResultStatus.ERROR:
                    Console.WriteLine(taskResult.ErrorMessage);
                    break;

                case TaskResultStatus.DONE:
                    break;

                default:
                    // Event Loop will not produce non-process-ending results.
                    throw new InvalidOperationException();
            }
        }
    }
}
