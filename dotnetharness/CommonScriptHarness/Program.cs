using CommonScript.Compiler;
using CommonScript.Runtime;
using System;
using System.Collections.Generic;

namespace CommonScriptHarness
{
    internal class Program
    {
        static void Main(string[] args)
        {
            bool isTestRun = false;
            string testDir = null;
            string singleTestFile = null;

            foreach (string arg in args)
            {
                if (arg == "runtests")
                {
                    isTestRun = true;
                }
                else if (arg.StartsWith("testdir:"))
                {
                    testDir = arg.Substring("testdir:".Length);
                }
                else if (arg.StartsWith("simpletest:"))
                {
                    singleTestFile = arg.Substring("simpletest:".Length);
                }
                else
                {
                    throw new InvalidOperationException("Unrecognized argument: " + arg);
                }
            }

            if (singleTestFile != null)
            {
                if (!System.IO.File.Exists(singleTestFile))
                {
                    throw new InvalidOperationException("file does not exist: " + singleTestFile);
                }
                string fileName = System.IO.Path.GetFileName(singleTestFile);
                string moduleId = "simple";
                Dictionary<string, Dictionary<string, string>> files = new Dictionary<string, Dictionary<string, string>>();
                files[moduleId] = new Dictionary<string, string>();
                files[moduleId][fileName] = System.IO.File.ReadAllText(singleTestFile);
                CompileAndRunProgram("Vanilla", "0.1.0", moduleId, files, false);
            }

            if (isTestRun)
            {
                if (testDir == null) throw new InvalidOperationException("'testdir:path-to-tests' command line arugment is required");
                if (!System.IO.Directory.Exists(testDir)) throw new InvalidOperationException("testdir path was not found");

                TestFileDatabase fileDb = new TestFileDatabase(testDir);

                System.Console.WriteLine("Running Tests...");

                DoTest("smoke", fileDb.Smoke());
                DoTest("countdown", fileDb.CountDown());
                DoTest("enumconst", fileDb.EnumConst()); // Lots of errors, mostly just a stub with lots of commented out stuff.
                // DoTest("lambdas", fileDb.Lambdas());
                DoTest("mapfilterreduce", fileDb.MapFilterReduce());
                DoTest("oopsimple", fileDb.OopSimple());
                DoTest("primitivemethods", fileDb.PrimitiveMethods());
                DoTest("slicingindexing", fileDb.SlicingIndexing());
                DoTest("staticmembers", fileDb.StaticMembers());
                DoTest("trycatch", fileDb.TryCatch());

                Console.WriteLine("\n*** All tests completed ***");
                return;
            }
        }

        private static string CompileAndRunProgram(
            string langId,
            string langVer,
            string moduleId,
            Dictionary<string, Dictionary<string, string>> files,
            bool returnOutputAsString)
        {
            List<string> outputBuffer = new List<string>();
            Dictionary<string, Func<object, object[], object>> extensions = new Dictionary<string, Func<object, object[], object>>();
            extensions["io_stdout"] = (_, args) =>
            {
                // TODO: this should be constructed from the runtime engine.
                string val = ValueConverter.RTValueToReadableString(args[0]);
                if (returnOutputAsString)
                {
                    outputBuffer.Add(val);
                }
                else
                {
                    System.Console.WriteLine(val);
                }
                return null;
            };
            CompilationEngine compiler = new CompilationEngine(langId, langVer, extensions.Keys);
            CompilationResult result = compiler.DoStaticCompilation(moduleId, files);
            if (result.ErrorMessage != null)
            {
                throw new InvalidOperationException(result.ErrorMessage);
            }

            string[] procArgs = [];
            RuntimeContext rt = new RuntimeContext(langId, langVer, result.ByteCodePayload, extensions, procArgs);
            TaskResult taskResult = rt.StartMainTask();
            if (returnOutputAsString)
            {
                if (outputBuffer.Count > 0) outputBuffer.Add("");
                return string.Join("\n", outputBuffer);
            }
            return null;
        }

        private static void DoTest(string moduleId, Dictionary<string, Dictionary<string, string>> files)
        {
            Console.WriteLine("Test: " + moduleId);
            string result = CompileAndRunProgram("TestScript", "0.1.0", moduleId, files, true);
            if (result == "")
            {
                throw new Exception("Test failed: " + moduleId + "\nPossibly misconfigured. It completed without any STDOUT output");
            }

            if (result != "ALL DONE\n")
            {
                throw new Exception("Test failed: " + moduleId + "\n" + "Result: \n" + result);
            }
        }
    }
}
