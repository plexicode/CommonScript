using System;
using System.Collections.Generic;
using System.Linq;
using CommonScript.Compiler.Internal;

namespace CommonScript.Compiler
{
    internal class Bundler
    {
        public static CompilationBundle bundleCompilation(StaticContext staticCtx, string rootId, CompiledModule[] modules)
        {
            CompiledModule[] deterministicOrder = modules.OrderBy(m => m.id).ToArray();
            CompilationBundle bundle = FunctionWrapper.CompilationBundle_new();

            List<FunctionEntity> functions = new List<FunctionEntity>();
            List<FunctionEntity> builtInFunctions = new List<FunctionEntity>(); // these get flattened into functions.
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

                AbstractEntity[] orderedEntities = [
                    .. deterministicKeyOrder.Select(k => m.flattenedEntities[k]),
                    .. m.lambdaEntities,
                ];

                foreach (AbstractEntity tle in orderedEntities)
                {
                    switch (tle.type)
                    {
                        case (int)EntityType.CONST: break;

                        case (int)EntityType.FUNCTION:
                            FunctionEntity func = (FunctionEntity)tle.specificData;
                            if (tle.fileContext.isCoreBuiltin)
                            {
                                builtInFunctions.Add(func);
                            }
                            else
                            {
                                functions.Add(func);
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
                            functions.Add((FunctionEntity)tle.specificData);
                            break;

                        case (int)EntityType.NAMESPACE:
                            // skip
                            break;

                        default:
                            throw new NotImplementedException();
                    }
                }
            }

            List<AbstractEntity> finalOrder = new List<AbstractEntity>();

            functions = new List<FunctionEntity>(builtInFunctions.Concat(functions));

            for (int i = 0; i < functions.Count; i++)
            {
                AbstractEntity fn = functions[i].baseData;
                fn.serializationIndex = i + 1;
                finalOrder.Add(fn);
            }

            for (int i = 0; i < enums.Count; i++)
            {
                enums[i].baseData.serializationIndex = i + 1;
                finalOrder.Add(enums[i].baseData);
            }

            ClassEntity[] sortedClasses = SortClasses(classes);

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

            allocateStringAndTokenIds(bundle);

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
            bundle.builtInCount = builtInFunctions.Count;

            return bundle;
        }

        private static ClassEntity[] SortClasses(List<ClassEntity> unorderedClasses)
        {
            ClassEntity[] deterministicOrder = unorderedClasses.OrderBy(c => c.baseData.fqName).ToArray();
            for (int i = 0; i < deterministicOrder.Length; i++)
            {
                ClassEntity cls = deterministicOrder[i];
                cls.classDepth = cls.baseClassEntity == null ? 1 : -1;
            }

            foreach (ClassEntity cls in deterministicOrder)
            {
                SortClassesHelperSetDepth(cls);
            }

            return deterministicOrder.OrderBy(c => c.classDepth).ToArray();
        }

        private static void SortClassesHelperSetDepth(ClassEntity cls)
        {
            if (cls.classDepth != -1) return;
            SortClassesHelperSetDepth(cls.baseClassEntity);
            cls.classDepth = cls.baseClassEntity.classDepth + 1;
        }

        private static void allocateStringAndTokenIds(CompilationBundle bundle)
        {
            List<ByteCodeRow> allByteCode = new List<ByteCodeRow>();
            for (int i = 1; i < bundle.functionById.Count; i++)
            {
                BundleFunctionInfo fn = bundle.functionById[i];
                allByteCode.AddRange(fn.code);
            }
            for (int i = 1; i < bundle.lambdaById.Count; i++)
            {
                BundleFunctionInfo fn = bundle.lambdaById[i];
                allByteCode.AddRange(fn.code);
            }

            Dictionary<string, int> stringUsageCount = new Dictionary<string, int>();
            Dictionary<string, int> tokenCountByFingerprint = new Dictionary<string, int>();

            foreach (ByteCodeRow row in allByteCode)
            {
                string str = row.stringArg;
                Token tok = row.token;
                if (str != null)
                {
                    if (!stringUsageCount.ContainsKey(str)) stringUsageCount[str] = 0;
                    stringUsageCount[str] = stringUsageCount[str] + 1;
                }

                if (tok != null)
                {
                    string fp = FunctionWrapper.Token_getFingerprint(tok);
                    if (!tokenCountByFingerprint.ContainsKey(fp)) tokenCountByFingerprint[fp] = 0;
                    tokenCountByFingerprint[fp] = tokenCountByFingerprint[fp] + 1;
                }
            }

            string[] stringByIndex = stringUsageCount.Keys.OrderBy(k => k).OrderBy(k => -stringUsageCount[k]).ToArray();
            string[] fpByIndex = tokenCountByFingerprint.Keys.OrderBy(k => k).OrderBy(k => -tokenCountByFingerprint[k]).ToArray();
            List<string> stringById = new List<string>() { null };
            stringById.AddRange(stringByIndex);
            Dictionary<string, int> stringToId = new Dictionary<string, int>();
            Dictionary<string, int> tokenFingerprintToId = new Dictionary<string, int>();
            for (int i = 0; i < stringByIndex.Length; i++)
            {
                string s = stringByIndex[i];
                stringToId[s] = i + 1;
            }
            for (int i = 0; i < fpByIndex.Length; i++)
            {
                string fp = fpByIndex[i];
                tokenFingerprintToId[fp] = i;
            }

            Token[] tokensById = new Token[fpByIndex.Length];
            foreach (ByteCodeRow row in allByteCode)
            {
                if (row.stringArg != null)
                {
                    row.stringId = stringToId[row.stringArg];
                }

                if (row.token != null)
                {
                    row.tokenId = tokenFingerprintToId[row.token.Fingerprint];
                    tokensById[row.tokenId] = row.token;
                }
            }

            bundle.tokensById = tokensById;
            bundle.stringById = stringById.ToArray();
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
                    throw new NotImplementedException();

                case (int)EntityType.FUNCTION:
                case (int)EntityType.CONSTRUCTOR:
                case (int)EntityType.LAMBDA_ENTITY:
                    bundleFunction(staticCtx, (FunctionEntity)entity.specificData, bundle);
                    break;

                case (int)EntityType.PROPERTY:
                    throw new NotImplementedException();

                default:
                    throw new InvalidOperationException();
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
            BundleClassInfo bci = FunctionWrapper.BundleClassInfo_new(
                classEntity.baseData.serializationIndex,
                baseClassId,
                classEntity.baseData.simpleName,
                mems["@ctor"].serializationIndex,
                mems.ContainsKey("@cctor") ? mems["@cctor"].serializationIndex : 0,
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
                        throw new NotImplementedException();
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
            List<ByteCodeRow> flatByteCode = new List<ByteCodeRow>(FunctionWrapper.flatten(buffer));

            for (int i = 0; i < flatByteCode.Count; i++)
            {
                ByteCodeRow row = flatByteCode[i];
                if (row.opCode <= 0) throw new InvalidOperationException(); // break or continue was not resolved.
                if (row.tryCatchInfo != null)
                {
                    int[] tryInfoArgs = row.tryCatchInfo;
                    tryInfoArgs[0] = i - flatByteCode.Count;
                    ByteCodeRow tryInfoRow = FunctionWrapper.ByteCodeRow_new(OpCodes.OP_TRY_INFO, null, null, tryInfoArgs);
                    flatByteCode.Add(tryInfoRow);
                }
            }

            BundleFunctionInfo fd = FunctionWrapper.BundleFunctionInfo_new(
                flatByteCode, 
                argcMin, 
                argc, 
                isLambda ? null : entity.baseData.simpleName);
            
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
