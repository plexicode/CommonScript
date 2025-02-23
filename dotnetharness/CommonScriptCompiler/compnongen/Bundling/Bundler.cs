using System;
using System.Collections.Generic;
using System.Linq;
using CommonScript.Compiler.Internal;

namespace CommonScript.Compiler
{
    internal static class Bundler
    {
        public static CompilationBundle bundleCompilation(StaticContext staticCtx, string rootId, CompiledModule[] modules)
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
                                    if (mainFunc != null) FunctionWrapper.Errors_Throw(tle.firstToken, "There are multiple functions named main in the root module.");
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
                bundleEntity(staticCtx, entity, bundle);
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


        private static void bundleEntity(StaticContext staticCtx, AbstractEntity entity, CompilationBundle bundle)
        {
            switch (entity.type)
            {
                case (int)EntityType.CONST:
                    // these are already flattened into expressions and have no more use.
                    break;

                case (int)EntityType.CLASS:
                    bundleClass((ClassEntity)entity.specificData, bundle);
                    break;

                case (int)EntityType.ENUM:
                    bundleEnum((EnumEntity)entity.specificData, bundle);
                    break;

                case (int)EntityType.FIELD:
                    FunctionWrapper.fail("not implemented");
                    break;

                case (int)EntityType.FUNCTION:
                case (int)EntityType.CONSTRUCTOR:
                case (int)EntityType.LAMBDA_ENTITY:
                    bundleFunction(staticCtx, (FunctionEntity)entity.specificData, bundle);
                    break;

                case (int)EntityType.PROPERTY:
                    FunctionWrapper.fail("not implemented");
                    break;

                default:
                    FunctionWrapper.fail(""); // invalid operation
                    break;
            }
        }

        private static void bundleClass(ClassEntity classEntity, CompilationBundle bundle)
        {
            int baseClassId = 0;
            if (classEntity.baseClassEntity != null)
            {
                baseClassId = classEntity.baseClassEntity.baseData.serializationIndex;
            }
            Dictionary<string, AbstractEntity> mems = classEntity.classMembers;
            int staticCtorId = 0;
            if (mems.ContainsKey("@cctor")) staticCtorId = mems["@cctor"].serializationIndex;
            BundleClassInfo bci = FunctionWrapper.BundleClassInfo_new(
                classEntity.baseData.serializationIndex,
                baseClassId,
                classEntity.baseData.simpleName,
                mems["@ctor"].serializationIndex,
                staticCtorId,
                new Dictionary<string, int>(),
                classEntity.newDirectMemberOffsets,
                new List<string>(),
                new List<string>());

            foreach (string memberName in classEntity.classMembers.Keys)
            {
                AbstractEntity member = classEntity.classMembers[memberName];
                switch (member.type)
                {
                    case (int)EntityType.FUNCTION:
                        bci.methodsToId[memberName] = member.serializationIndex;
                        if (member.isStatic) bci.staticMethods.Add(memberName);
                        break;
                    case (int)EntityType.FIELD:
                        if (member.isStatic) bci.staticFields.Add(memberName);
                        break;
                    case (int)EntityType.CONSTRUCTOR:
                        break; // already handled.
                    default:
                        FunctionWrapper.fail("Not implemented");
                        break;
                }
            }

            bundle.classById.Add(bci);
        }

        private static void bundleEnum(EnumEntity enumEntity, CompilationBundle bundle)
        {
            BundleEnumInfo bei = FunctionWrapper.BundleEnumInfo_createFromEntity(enumEntity);
            bundle.enumById.Add(bei);
        }

        private static void bundleFunction(StaticContext staticCtx, FunctionEntity entity, CompilationBundle bundle)
        {
            bool isLambda = entity.baseData.type == (int)EntityType.LAMBDA_ENTITY;
            ByteCodeBuffer buffer = null;
            int argc = entity.argTokens.Length;
            int argcMin = 0;
            for (int i = 0; i < argc; i++)
            {
                Token argToken = entity.argTokens[i];
                Expression argValue = entity.argDefaultValues[i];

                ByteCodeBuffer argBuffer;
                if (argValue == null)
                {
                    argcMin++;
                    argBuffer = FunctionWrapper.create1(OpCodes.OP_PUSH_ARG, argToken, null, i);
                }
                else
                {
                    ByteCodeBuffer defaultValBuffer = FunctionWrapper.serializeExpression(staticCtx, argValue);
                    argBuffer = FunctionWrapper.create2(OpCodes.OP_PUSH_ARG_IF_PRESENT, argToken, null, i, defaultValBuffer.length);
                    argBuffer = FunctionWrapper.join2(argBuffer, defaultValBuffer);
                }
                buffer = FunctionWrapper.join3(
                    buffer,
                    argBuffer,
                    FunctionWrapper.create0(OpCodes.OP_ASSIGN_VAR, argToken, argToken.Value));
            }

            foreach (Statement stmnt in entity.code)
            {
                buffer = FunctionWrapper.join2(buffer, FunctionWrapper.serializeStatement(staticCtx, stmnt));
            }
            ByteCodeRow[] flatByteCode = FunctionWrapper.flatten(buffer);
            List<ByteCodeRow> byteCodeFinal = new List<ByteCodeRow>();
            for (int i = 0; i < flatByteCode.Length; i++)
            {
                byteCodeFinal.Add(flatByteCode[i]);
            }

            for (int i = 0; i < byteCodeFinal.Count; i++)
            {
                ByteCodeRow row = byteCodeFinal[i];
                if (row.opCode <= 0) FunctionWrapper.fail(""); // break or continue was not resolved.
                if (row.tryCatchInfo != null)
                {
                    int[] tryInfoArgs = row.tryCatchInfo;
                    tryInfoArgs[0] = i - byteCodeFinal.Count;
                    ByteCodeRow tryInfoRow = FunctionWrapper.ByteCodeRow_new(OpCodes.OP_TRY_INFO, null, null, tryInfoArgs);
                    byteCodeFinal.Add(tryInfoRow);
                }
            }

            string fnName = null;
            if (!isLambda) fnName = entity.baseData.simpleName;
            BundleFunctionInfo fd = FunctionWrapper.BundleFunctionInfo_new(
                byteCodeFinal, 
                argcMin, 
                argc, 
                fnName);
            
            if (isLambda)
            {
                bundle.lambdaById.Add(fd);
            }
            else
            {
                bundle.functionById.Add(fd);
            }
        }
    }
}
