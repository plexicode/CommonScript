using System;
using System.Collections.Generic;
using System.Linq;

namespace CommonScriptHarness
{
    internal class TestFileDatabase
    {
        private string testDir;
        private static readonly string TEST_LIB = "testlib, testlib.script, testlib.script";
        public TestFileDatabase(string testDir)
        {
            this.testDir = testDir;
        }

        public Dictionary<string, Dictionary<string, string>> HelloTest()
        {
            return this.GetFiles(new string[] {
                "test,    hellotest.script,    hellotest.script",
            });
        }

        public Dictionary<string, Dictionary<string, string>> Smoke()
        {
            return this.GetFiles(new string[] {
                "smoke,  smoke.script,  smoke/smoke.script",
                TEST_LIB
            });
        }

        public Dictionary<string, Dictionary<string, string>> EnumConst()
        {
            return this.GetFiles(new string[] {
                "dep,  dep.script,  enumconst/dep.script",
                "enumconst,  enumconst.script,  enumconst/enumconst.script",
                TEST_LIB
            });
        }

        public Dictionary<string, Dictionary<string, string>> OopSimple()
        {
            return this.GetFiles(new string[] {
                "dep,  dep.script,  oopsimple/dep.script",
                "oopsimple,  oopsimple.script,  oopsimple/oopsimple.script",
                TEST_LIB
            });
        }

        public Dictionary<string, Dictionary<string, string>> CountDown()
        {
            return this.GetFiles(new string[] {
                "collatz,    collatz.script,    countdown/collatz.script",
                "countdown,  main.script,  countdown/main.script",
                TEST_LIB
            });
        }

        public Dictionary<string, Dictionary<string, string>> PrimitiveMethods()
        {
            return this.GetFiles(new string[] {
                "primitivemethods,    primitivemethods.script,    primitivemethods/primitivemethods.script",
                TEST_LIB
            });
        }

        public Dictionary<string, Dictionary<string, string>> SlicingIndexing()
        {
            return this.GetFiles(new string[] {
                "slicingindexing,    slicingindexing.script,    slicingindexing/slicingindexing.script",
                TEST_LIB
            });
        }

        public Dictionary<string, Dictionary<string, string>> StaticMembers()
        {
            return this.GetFiles(new string[] {
                "staticmembers,    staticmembers.script,    staticmembers/staticmembers.script",
                TEST_LIB
            });
        }

        public Dictionary<string, Dictionary<string, string>> TryCatch()
        {
            return this.GetFiles(new string[] {
                "trycatch,    trycatch.script,    trycatch/trycatch.script",
                TEST_LIB
            });
        }

        public Dictionary<string, Dictionary<string, string>> Lambdas()
        {
            return this.GetFiles(new string[] {
                "lambdas,  lambdas.script,  lambdas/lambdas.script",
                TEST_LIB
            });
        }

        public Dictionary<string, Dictionary<string, string>> MapFilterReduce()
        {
            return this.GetFiles(new string[] {
                "mapfilterreduce,  mapfilterreduce.script,  mapfilterreduce/mapfilterreduce.script",
                TEST_LIB
            });
        }

        private Dictionary<string, Dictionary<string, string>> GetFiles(string[] items)
        {
            Dictionary<string, Dictionary<string, string>> moduleFileLookup = new Dictionary<string, Dictionary<string, string>>();
            foreach (string datum in items)
            {
                string[] parts = datum.Split(',').Select(v => v.Trim()).ToArray();
                string moduleId = parts[0];
                string virtualFileName = parts[1];
                string actualFilePath = (this.testDir + "/" + parts[2])
                    .Replace("\\", "/")
                    .Replace("//", "/")
                    .Replace('/', System.IO.Path.DirectorySeparatorChar);
                if (!moduleFileLookup.ContainsKey(moduleId))
                {
                    moduleFileLookup[moduleId] = new Dictionary<string, string>();
                    moduleFileLookup[moduleId][virtualFileName] = System.IO.File.ReadAllText(actualFilePath);
                }
            }

            return moduleFileLookup;
        }
    }
}
