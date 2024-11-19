using System;
using System.Collections.Generic;
using System.Linq;

namespace CommonScript.Compiler
{
    internal class Bundler
    {
        public static CompilationBundle bundleCompilation(string rootId, CompiledModule[] modules)
        {
            CompiledModule[] deterministicOrder = modules.OrderBy(m => m.id).ToArray();
            CompilationBundle bundle = new CompilationBundle();

            List<FunctionLikeEntity> functions = new List<FunctionLikeEntity>();
            List<FunctionLikeEntity> builtInFunctions = new List<FunctionLikeEntity>(); // these get flattened into functions.
            List<EnumEntity> enums = new List<EnumEntity>();
            List<ClassEntity> classes = new List<ClassEntity>();
            List<FieldEntity> fields = new List<FieldEntity>();
            List<LambdaEntity> lambdas = new List<LambdaEntity>();
            FunctionEntity mainFunc = null;

            foreach (CompiledModule m in deterministicOrder)
            {
                string[] deterministicKeyOrder = m.flattenedEntities.Keys.OrderBy(k => k).ToArray();

                // If deferring main to an included module, that would go here.
                bool checkForMain = m.id == rootId;

                foreach (string entityFqName in deterministicKeyOrder)
                {
                    AbstractEntity tle = m.flattenedEntities[entityFqName];

                    switch (tle.type)
                    {
                        case EntityType.CONST: break;
                        
                        case EntityType.FUNCTION:
                            FunctionLikeEntity func = (FunctionLikeEntity) tle;
                            if (tle.fileContext.isCoreBuiltin)
                            {
                                builtInFunctions.Add(func);
                            }
                            else
                            {
                                functions.Add(func);
                                if (checkForMain && tle.simpleName == "main")
                                {
                                    if (mainFunc != null) Errors.ThrowError(tle.firstToken, "There are multiple functions named main in the root module.");
                                    mainFunc = (FunctionEntity)func;
                                }
                            }
                            break;

                        case EntityType.FIELD:
                            fields.Add((FieldEntity)tle);
                            break;


                        case EntityType.ENUM:
                            enums.Add((EnumEntity)tle);
                            break;
                        
                        case EntityType.CLASS:
                            classes.Add((ClassEntity)tle);
                            break;

                        case EntityType.CONSTRUCTOR:
                            functions.Add((ConstructorEntity)tle);
                            break;

                        case EntityType.NAMESPACE:
                            // skip
                            break;

                        default:
                            throw new NotImplementedException();
                    }
                }

                lambdas.AddRange(m.lambdaEntities);
            }

            List<AbstractEntity> finalOrder = new List<AbstractEntity>();

            functions = new List<FunctionLikeEntity>(builtInFunctions.Concat(functions));

            for (int i = 0; i < functions.Count; i++)
            {
                AbstractEntity fn = functions[i];
                fn.serializationIndex = i + 1;
                finalOrder.Add(fn);
            }

            for (int i = 0; i < enums.Count; i++)
            {
                enums[i].serializationIndex = i + 1;
                finalOrder.Add(enums[i]);
            }

            ClassEntity[] sortedClasses = SortClasses(classes); 

            for (int i = 0; i < sortedClasses.Length; i++)
            {
                ClassEntity cls = sortedClasses[i];
                cls.serializationIndex = i + 1;
                finalOrder.Add(cls);
            }

            for (int i = 0; i < lambdas.Count; i++)
            {
                lambdas[i].serializationIndex = i + 1;
            }

            foreach (AbstractEntity entity in finalOrder)
            {
                bundleEntity(entity, bundle);
            }

            allocateStringAndTokenIds(bundle);

            if (mainFunc == null)
            {
                Errors.ThrowGeneralError("There is no main() function defined.");
            }

            if (mainFunc.argTokens.Length >= 2)
            {
                Errors.ThrowError(
                    mainFunc.nameToken,
                    "The main function can only take in at most one argument. " +
                    "Note that multiple CLI arguments are passed in to main(args) as a single list of strings.");
            }

            bundle.mainFunctionId = mainFunc.serializationIndex;
            bundle.builtInCount = builtInFunctions.Count;

            return bundle;
        }

        private static ClassEntity[] SortClasses(List<ClassEntity> unorderedClasses)
        {
            ClassEntity[] deterministicOrder = unorderedClasses.OrderBy(c => c.fqName).ToArray();
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
                    string fp = tok.getFingerprint();
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

        private static void bundleEntity(AbstractEntity entity, CompilationBundle bundle)
        {
            switch (entity.type)
            {
                case EntityType.CONST:
                    // these are already flattened into expressions and have no more use.
                    break;

                case EntityType.CLASS:
                    bundleClass((ClassEntity) entity, bundle);
                    break;

                case EntityType.ENUM:
                    bundleEnum((EnumEntity) entity, bundle);
                    break;

                case EntityType.FIELD:
                    throw new NotImplementedException();
                
                case EntityType.FUNCTION:
                case EntityType.CONSTRUCTOR:
                    bundleFunction((FunctionLikeEntity) entity, bundle, entity.nestParent != null && entity.nestParent.type == EntityType.CLASS);
                    break;
                
                case EntityType.PROPERTY:
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
                baseClassId = classEntity.baseClassEntity.serializationIndex;
            }
            Dictionary<string, AbstractEntity> mems = classEntity.classMembers;
            BundleClassInfo bci = new BundleClassInfo()
            {
                id = classEntity.serializationIndex,
                parentId = baseClassId,
                name = classEntity.simpleName,
                ctorId = mems["@ctor"].serializationIndex,
                staticCtorId = mems.ContainsKey("@cctor") ? mems["@cctor"].serializationIndex : 0,
                methodsToId = new Dictionary<string, int>(),
                newDirectMembersByNextOffsets = classEntity.newDirectMemberOffsets,
                staticMethods = new List<string>(),
                staticFields = new List<string>(),
            };

            foreach (string memberName in classEntity.classMembers.Keys)
            {
                AbstractEntity member = classEntity.classMembers[memberName];
                switch (member.type)
                {
                    case EntityType.FUNCTION:
                        bci.methodsToId[memberName] = member.serializationIndex;
                        if (member.isStatic) bci.staticMethods.Add(memberName);
                        break;
                    case EntityType.FIELD:
                        if (member.isStatic) bci.staticFields.Add(memberName);
                        break;
                    case EntityType.CONSTRUCTOR:
                        break; // already handled.
                    default:
                        throw new NotImplementedException();
                }
            }

            bundle.classById.Add(bci);
        }

        private static void bundleEnum(EnumEntity enumEntity, CompilationBundle bundle)
        {
            BundleEnumInfo bei = BundleEnumInfo.createFromEntity(enumEntity);
            bundle.enumById.Add(bei);
        }

        private static void bundleFunction(FunctionLikeEntity entity, CompilationBundle bundle, bool isMethod)
        {
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
                    argBuffer = ByteCode.create1(OpCodes.OP_PUSH_ARG, argToken, null, i);
                }
                else
                {
                    ByteCodeBuffer defaultValBuffer = Serializer.serializeExpression(argValue);
                    argBuffer = ByteCode.create2(OpCodes.OP_PUSH_ARG_IF_PRESENT, argToken, null, i, defaultValBuffer.length);
                    argBuffer = ByteCode.join2(argBuffer, defaultValBuffer);
                }
                buffer = ByteCode.join3(
                    buffer,
                    argBuffer,
                    ByteCode.create0(OpCodes.OP_ASSIGN_VAR, argToken, argToken.Value));
            }

            foreach (Statement stmnt in entity.code)
            {
                buffer = ByteCode.join2(buffer, Serializer.serializeStatement(stmnt));
            }
            List<ByteCodeRow> flatByteCode = new List<ByteCodeRow>(ByteCode.flatten(buffer));

            for (int i = 0; i < flatByteCode.Count; i++)
            {
                ByteCodeRow row = flatByteCode[i];
                if (row.opCode <= 0) throw new InvalidOperationException(); // break or continue was not resolved.
                if (row.tryCatchInfo != null)
                {
                    int[] tryInfoArgs = row.tryCatchInfo;
                    tryInfoArgs[0] = i - flatByteCode.Count;
                    ByteCodeRow tryInfoRow = new ByteCodeRow(OpCodes.OP_TRY_INFO, null, null, tryInfoArgs);
                    flatByteCode.Add(tryInfoRow);
                }
            }

            BundleFunctionInfo fd = new BundleFunctionInfo()
            {
                code = flatByteCode.ToArray(),
                argcMin = argcMin,
                argcMax = argc,
                name = entity.simpleName,
            };
            bundle.functionById.Add(fd);
        }
    }
}
