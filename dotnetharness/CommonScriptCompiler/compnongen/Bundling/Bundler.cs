using System;
using System.Collections.Generic;
using System.Linq;
using CommonScript.Compiler.Internal;

namespace CommonScript.Compiler
{
    internal static class Bundler
    {
        public static CompilationBundle bundleCompilation(StaticContext staticCtx, string rootId,
            CompiledModule[] modules)
        {
            CompiledModule[] deterministicOrder = FunctionWrapper.getDeterministOrderOfModules(modules);
            CompilationBundle bundle = FunctionWrapper.CompilationBundle_new();

            // Functions are distinguished to ensure builtInFunctions get sorted first in byte code 
            List<FunctionEntity> userFunctions = new List<FunctionEntity>();
            List<FunctionEntity> coreBuiltInFunctions = new List<FunctionEntity>();

            List<EnumEntity> enums = new List<EnumEntity>();
            List<ClassEntity> classes = new List<ClassEntity>();
            List<FieldEntity> fields = new List<FieldEntity>();
            List<FunctionEntity> lambdas = new List<FunctionEntity>();
            FunctionEntity mainFunc = null;

            foreach (CompiledModule m in deterministicOrder)
            {
                string[] deterministicKeyOrder = m.flattenedEntities.Keys.OrderBy(k => k).ToArray();

                // If deferring main to an included module, that would go here.
                bool checkForMain = m.id == rootId;

                List<AbstractEntity> orderedEntities = new List<AbstractEntity>();
                for (int i = 0; i < deterministicKeyOrder.Length; i++)
                {
                    orderedEntities.Add(m.flattenedEntities[deterministicKeyOrder[i]]);
                }

                for (int i = 0; i < m.lambdaEntities.Count; i++)
                {
                    orderedEntities.Add(m.lambdaEntities[i]);
                }

                foreach (AbstractEntity tle in orderedEntities)
                {
                    switch (tle.type)
                    {
                        case (int)EntityType.CONST: break;

                        case (int)EntityType.FUNCTION:
                            FunctionEntity func = (FunctionEntity)tle.specificData;
                            if (tle.fileContext.isCoreBuiltin)
                            {
                                coreBuiltInFunctions.Add(func);
                            }
                            else
                            {
                                userFunctions.Add(func);
                                if (checkForMain && tle.simpleName == "main")
                                {
                                    if (mainFunc != null)
                                        FunctionWrapper.Errors_Throw(tle.firstToken,
                                            "There are multiple functions named main in the root module.");
                                    mainFunc = func;
                                }
                            }

                            break;

                        case (int)EntityType.LAMBDA_ENTITY:
                            lambdas.Add((FunctionEntity)tle.specificData);
                            break;

                        case (int)EntityType.FIELD:
                            fields.Add((FieldEntity)tle.specificData);
                            break;


                        case (int)EntityType.ENUM:
                            enums.Add((EnumEntity)tle.specificData);
                            break;

                        case (int)EntityType.CLASS:
                            classes.Add((ClassEntity)tle.specificData);
                            break;

                        case (int)EntityType.CONSTRUCTOR:
                            userFunctions.Add((FunctionEntity)tle.specificData);
                            break;

                        case (int)EntityType.NAMESPACE:
                            // skip
                            break;

                        default:
                            FunctionWrapper.fail("Not implemented");
                            break;
                    }
                }
            }

            List<AbstractEntity> finalOrder = new List<AbstractEntity>();

            List<FunctionEntity> allFunctions = new List<FunctionEntity>();
            for (int i = 0; i < coreBuiltInFunctions.Count; i++)
            {
                allFunctions.Add(coreBuiltInFunctions[i]);
            }

            for (int i = 0; i < userFunctions.Count; i++)
            {
                allFunctions.Add(userFunctions[i]);
            }

            for (int i = 0; i < allFunctions.Count; i++)
            {
                AbstractEntity fn = allFunctions[i].baseData;
                fn.serializationIndex = i + 1;
                finalOrder.Add(fn);
            }

            for (int i = 0; i < enums.Count; i++)
            {
                enums[i].baseData.serializationIndex = i + 1;
                finalOrder.Add(enums[i].baseData);
            }

            ClassEntity[] sortedClasses = FunctionWrapper.SortClassesInDeterministicDependencyOrder(classes.ToArray());

            for (int i = 0; i < sortedClasses.Length; i++)
            {
                ClassEntity cls = sortedClasses[i];
                cls.baseData.serializationIndex = i + 1;
                finalOrder.Add(cls.baseData);
            }

            for (int i = 0; i < lambdas.Count; i++)
            {
                lambdas[i].baseData.serializationIndex = i + 1;
                finalOrder.Add(lambdas[i].baseData);
            }

            foreach (AbstractEntity entity in finalOrder)
            {
                FunctionWrapper.bundleEntity(staticCtx, entity, bundle);
            }

            FunctionWrapper.allocateStringAndTokenIds(bundle);

            if (mainFunc == null)
            {
                FunctionWrapper.Errors_ThrowGeneralError("There is no main() function defined.");
            }

            if (mainFunc.argTokens.Length >= 2)
            {
                FunctionWrapper.Errors_Throw(
                    mainFunc.baseData.nameToken,
                    "The main function can only take in at most one argument. " +
                    "Note that multiple CLI arguments are passed in to main(args) as a single list of strings.");
            }

            bundle.mainFunctionId = mainFunc.baseData.serializationIndex;
            bundle.builtInCount = coreBuiltInFunctions.Count;

            return bundle;
        }
    }
}
