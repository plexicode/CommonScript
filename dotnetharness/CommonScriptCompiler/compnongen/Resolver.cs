using System;
using System.Collections.Generic;
using System.Linq;
using CommonScript.Compiler.Internal;

namespace CommonScript.Compiler
{
    internal static class ResolverUtil
    {
        public static bool isValidRegisteredExtension(Resolver resolver, string extensionName)
        {
            return FunctionWrapper.StringSet_has(resolver.extensionNames, extensionName);
        }

        public static void Resolve(Resolver resolver)
        {
            List<FunctionEntity> functions = new List<FunctionEntity>();
            List<ClassEntity> classes = new List<ClassEntity>();
            List<ConstEntity> constants = new List<ConstEntity>();
            List<EnumEntity> enums = new List<EnumEntity>();
            List<FunctionEntity> constructors = new List<FunctionEntity>();
            List<FieldEntity> fields = new List<FieldEntity>();

            AbstractEntity[] entities = resolver.flattenedEntities.Values.ToArray();

            for (int i = 0; i < entities.Length; i++)
            {
                AbstractEntity tle = entities[i];

                if (tle.type == (int)EntityType.CONST)
                {
                    constants.Add((ConstEntity)tle.specificData);
                }
                else if (tle.type == (int)EntityType.ENUM)
                {
                    enums.Add((EnumEntity)tle.specificData);
                }
                else if (tle.type == (int)EntityType.FUNCTION)
                {
                    functions.Add((FunctionEntity)tle.specificData);
                }
                else if (tle.type == (int)EntityType.CLASS)
                {
                    classes.Add((ClassEntity)tle.specificData);
                }
                else if (tle.type == (int)EntityType.CONSTRUCTOR)
                {
                    constructors.Add((FunctionEntity)tle.specificData);
                }
                else if (tle.type == (int)EntityType.FIELD)
                {
                    fields.Add((FieldEntity)tle.specificData);
                }
                else if (tle.type == (int)EntityType.NAMESPACE)
                {
                    // Nothing to resolve.
                }
                else
                {
                    throw new NotImplementedException();
                }
            }

            ResolverUtil.AddImplicitIncrementingEnumValueDefinitions(enums);

            string[] constAndEnumResolutionOrder = DetermineConstAndEnumResolutionOrder(resolver, constants, enums);

            PerformFullResolutionPassOnConstAndEnums(resolver, constAndEnumResolutionOrder);

            ClassEntity[] orderedClasses = ResolveBaseClassesAndEstablishClassOrder(resolver, classes, resolver.flattenedEntities);

            for (int i = 0; i < orderedClasses.Length; i++)
            {
                EntityResolverUtil.EntityResolver_DetermineMemberOffsets(orderedClasses[i]);
            }

            for (int i = 0; i < functions.Count; i++)
            {
                EntityResolverUtil.EntityResolver_ResetAutoVarId(resolver);
                EntityResolverUtil.EntityResolver_ResolveFunctionFirstPass(resolver, functions[i]);
            }

            for (int i = 0; i < constructors.Count; i++)
            {
                EntityResolverUtil.EntityResolver_ResetAutoVarId(resolver);
                EntityResolverUtil.EntityResolver_ResolveFunctionFirstPass(resolver, constructors[i]);
            }

            // At this point, all lambdas have been reported.
            // DO NOT CHANGE TO FOR-EACH. This count can grow as a result of nested lambdas.
            for (int i = 0; i < resolver.lambdas.Count; i++) 
            {
                EntityResolverUtil.EntityResolver_ResetAutoVarId(resolver);
                EntityResolverUtil.EntityResolver_ResolveFunctionFirstPass(resolver, resolver.lambdas[i]);
            }

            for (int i = 0; i < functions.Count; i++)
            {
                EntityResolverUtil.EntityResolver_ResolveFunctionSecondPass(resolver, functions[i]);
            }

            for (int i = 0; i < constructors.Count; i++)
            {
                EntityResolverUtil.EntityResolver_ResolveFunctionSecondPass(resolver, constructors[i]);
            }

            for (int i = 0; i < resolver.lambdas.Count; i++)
            {
                EntityResolverUtil.EntityResolver_ResolveFunctionSecondPass(resolver, resolver.lambdas[i]);
            }
        }

        internal static void ReportNewLambda(Resolver resolver, FunctionEntity lamb)
        {
            resolver.lambdas.Add(lamb);
        }

        private static ClassEntity[] ResolveBaseClassesAndEstablishClassOrder(
            Resolver resolver, 
            List<ClassEntity> classes,
            Dictionary<string, AbstractEntity> flattenedEntities)
        {
            // TODO: add external module resolving as a separate feature.
            ClassEntity[] deterministicOrder = classes.OrderBy(c => c.baseData.fqName).ToArray();
            List<ClassEntity> finalOrder = new List<ClassEntity>();
            List<ClassEntity> baseClassRequired = new List<ClassEntity>();
            foreach (ClassEntity e in deterministicOrder)
            {
                if (e.baseClassTokens != null)
                {
                    baseClassRequired.Add(e);
                }
                else
                {
                    finalOrder.Add(e);
                }
            }
            foreach (ClassEntity bc in baseClassRequired)
            {
                string ns = string.Join('.', bc.baseData.fqName.Split('.').SkipLast(1));
                resolver.activeEntity = bc.baseData;
                Token baseClassToken = bc.baseClassTokens[0];
                AbstractEntity bcEntity = DoLookup2(resolver, baseClassToken, baseClassToken.Value);
                if (bcEntity != null)
                {
                    for (int i = 2; i < bc.baseClassTokens.Length; i += 2)
                    {
                        string next = bc.baseClassTokens[i].Value;
                        if (bcEntity != null)
                        {
                            Dictionary<string, AbstractEntity> lookup = FunctionWrapper.Entity_getMemberLookup(resolver.staticCtx, bcEntity);
                            if (lookup.ContainsKey(next))
                            {
                                bcEntity = lookup[next];
                            }
                            else
                            {
                                bcEntity = null;
                            }
                        }
                    }
                }
                if (bcEntity == null)
                {
                    FunctionWrapper.Errors_Throw(bc.baseData.firstToken, "Could not resolve base class");
                    throw new NotImplementedException();
                }
                if (bcEntity.type != (int)EntityType.CLASS)
                {
                    FunctionWrapper.Errors_Throw(bc.baseData.firstToken, bcEntity.fqName + " is not a valid class.");
                }

                bc.baseClassEntity = (ClassEntity)bcEntity.specificData;
                resolver.activeEntity = null;
            }

            Dictionary<string, bool> includedInOrder = new Dictionary<string, bool>();
            foreach (ClassEntity bc in baseClassRequired)
            {
                includedInOrder[bc.baseData.fqName] = false;
            }
            foreach (ClassEntity bc in baseClassRequired)
            {
                ClassEntity walker = bc;
                List<ClassEntity> order = new List<ClassEntity>();
                while (walker != null)
                {
                    // If it's external or already included, stop.
                    if (!includedInOrder.ContainsKey(walker.baseData.fqName) ||
                        includedInOrder[walker.baseData.fqName])
                    {
                        walker = null;
                    }
                    else
                    {
                        order.Add(walker);
                        includedInOrder[walker.baseData.fqName] = true;
                        walker = walker.baseClassEntity;
                    }

                    if (order.Count > deterministicOrder.Length)
                    {
                        FunctionWrapper.Errors_Throw(bc.baseData.firstToken, "This class has a cycle in its base class chain.");
                    }
                }

                order.Reverse();
                finalOrder.AddRange(order);
            }

            return finalOrder.ToArray();
        }

        private static void PerformFullResolutionPassOnConstAndEnums(Resolver resolver, string[] resOrder)
        {
            for (int passNum = 1; passNum <= 2; passNum++)
            {
                for (int i = 0; i < resOrder.Length; i++)
                {
                    AbstractEntity entity = resolver.flattenedEntitiesAndEnumValues[resOrder[i]];
                    resolver.activeEntity = entity;
                    if (entity.type == (int)EntityType.ENUM)
                    {
                        EnumEntity enumDef = (EnumEntity)entity.specificData;
                        int memberIndex = ResolverUtil.GetEnumMemberIndex(resolver, resOrder[i], enumDef);
                        Expression val = enumDef.memberValues[memberIndex];
                        if (passNum == 1)
                        {
                            val = ExpressionResolverUtil.ExpressionResolver_ResolveExpressionFirstPass(resolver, val);
                        }
                        else
                        {
                            val = ExpressionResolverUtil.ExpressionResolver_ResolveExpressionSecondPass(resolver, val);
                        }

                        if (passNum == 2 && val.type != (int) ExpressionType.INTEGER_CONST)
                        {
                            FunctionWrapper.Errors_Throw(enumDef.memberNameTokens[memberIndex], "This enum value has a non-integer value.");
                        }

                        enumDef.memberValues[memberIndex] = val;
                    }
                    else
                    {
                        ConstEntity constEnt = (ConstEntity)entity.specificData;
                        Expression val = constEnt.constValue;
                        if (passNum == 1)
                        {
                            val = ExpressionResolverUtil.ExpressionResolver_ResolveExpressionFirstPass(resolver, val);
                        }
                        else
                        {
                            val = ExpressionResolverUtil.ExpressionResolver_ResolveExpressionSecondPass(resolver, val);
                            if (!IsExpressionConstant(val))
                            {
                                FunctionWrapper.Errors_Throw(val.firstToken, "A constant expression is required here.");
                            }
                        }

                        constEnt.constValue = val;
                    }
                    resolver.activeEntity = null;
                }
            }
        }

        internal static bool IsExpressionConstant(Expression expr)
        {
            switch (expr.type)
            {
                case (int) ExpressionType.BOOL_CONST:
                case (int) ExpressionType.NULL_CONST:
                case (int) ExpressionType.INTEGER_CONST:
                case (int) ExpressionType.FLOAT_CONST:
                case (int) ExpressionType.STRING_CONST:
                case (int) ExpressionType.ENUM_CONST:
                    return true;
            }
            return false;
        }


        private static Token BuildFakeToken(Token template, string value, bool isPunc)
        {
            return FunctionWrapper.Token_new(
                value,
                isPunc ? (int) TokenType.PUNCTUATION : (int) TokenType.NAME,
                template.File,
                template.Line,
                template.Col);
        }

        private static Expression BuildFakeDotChain(string root, string field)
        {
            Expression varRoot = FunctionWrapper.Expression_createVariable(null, root);
            return FunctionWrapper.Expression_createDotField(varRoot, null, field);
        }

        // For all undefined values of an enum, make it equal to the previous value + 1 by
        // injecting EnumName.MemberName + 1
        // If the first value is undefined, inject 1
        private static void AddImplicitIncrementingEnumValueDefinitions(List<EnumEntity> enums)
        {
            for (int i = 0; i < enums.Count; i++)
            {
                EnumEntity enumEnt = enums[i];
                for (int j = 0; j < enumEnt.memberNameTokens.Length; j++)
                {
                    Token token = enumEnt.memberNameTokens[j];
                    if (enumEnt.memberValues[j] == null)
                    {
                        if (j == 0)
                        {
                            enumEnt.memberValues[j] = FunctionWrapper.Expression_createIntegerConstant(token, 1);
                        }
                        else
                        {
                            enumEnt.memberValues[j] = FunctionWrapper.Expression_createBinaryOp(
                                BuildFakeDotChain(enumEnt.baseData.simpleName, enumEnt.memberNameTokens[j - 1].Value),
                                BuildFakeToken(token, "+", true),
                                FunctionWrapper.Expression_createIntegerConstant(null, 1)
                            );
                        }
                    }
                }
            }
        }

        // Returns strings because we treat the individual enum values as separate entities.
        private static string[] DetermineConstAndEnumResolutionOrder(
            Resolver resolver,
            List<ConstEntity> constants,
            List<EnumEntity> enums)
        {
            Dictionary<string, List<string>> referencesMadeByFqItem = new Dictionary<string, List<string>>();

            // You left off thinking about how to register references found in the expression resolver
            // out here and then using that to build up the references lookup.
            // Then you were thinking about how this only pertains to the first pass, that really this 
            // function should do both passes and be renamed to ResolveConstantsAndEnums()


            foreach (ConstEntity c in constants)
            {
                string ns = c.baseData.nestParent == null ? "" : c.baseData.nestParent.fqName;

                List<string> refsOut = new List<string>();
                c.constValue = ResolverUtil.GetListOfUnresolvedConstReferences(resolver, c.baseData.fileContext, ns, c.constValue, refsOut);
                referencesMadeByFqItem[c.baseData.fqName] = refsOut;
            }

            foreach (EnumEntity e in enums)
            {
                string ns = e.baseData.nestParent == null ? "" : e.baseData.nestParent.fqName;
                int memCount = e.memberNameTokens.Length;

                for (int i = 0; i < memCount; i++)
                {
                    string memFqName = e.baseData.fqName + "." + e.memberNameTokens[i].Value;
                    Expression val = e.memberValues[i];
                    List<string> refsOut = new List<string>();
                    e.memberValues[i] = ResolverUtil.GetListOfUnresolvedConstReferences(resolver, e.baseData.fileContext, ns, val, refsOut);
                    referencesMadeByFqItem[memFqName] = refsOut;
                }
            }

            // 0 - unresolved, 1 - resolving, 2 - resolved
            Dictionary<string, int> resolutionStatus = new Dictionary<string, int>();
            List<string> resolutionOrder = new List<string>();

            List<string> q = new List<string>(referencesMadeByFqItem.Keys.OrderBy(k => k).Reverse());
            while (q.Count > 0)
            {
                int lastIndex = q.Count - 1;
                string itemFqName = q[lastIndex];
                q.RemoveAt(lastIndex);
                if (itemFqName == "*")
                {
                    // indicator that the next item has all prerequisites added to the list.
                    // it is now safe to add it to the list.
                    string resolvedItem = q[lastIndex - 1];
                    q.RemoveAt(lastIndex - 1);
                    resolutionStatus[resolvedItem] = 2;
                    resolutionOrder.Add(resolvedItem);
                }
                else
                {
                    AbstractEntity item = resolver.flattenedEntitiesAndEnumValues[itemFqName];
                    Token itemToken = item.firstToken;
                    if (item.fqName != itemFqName) // an enum member
                    {
                        EnumEntity parentEnum = (EnumEntity)item.specificData;
                        int enumValIndex = ResolverUtil.GetEnumMemberIndex(resolver, itemFqName, parentEnum);
                        itemToken = parentEnum.memberNameTokens[enumValIndex];
                    }

                    int status = 0;
                    if (resolutionStatus.ContainsKey(itemFqName)) status = resolutionStatus[itemFqName];

                    if (status == 1)
                    {
                        FunctionWrapper.Errors_Throw(item.firstToken, "This definition contains a resolution cycle.");
                    }

                    if (status == 2)
                    {
                        // Already resolved. Skip!
                    }
                    else
                    {
                        q.Add(itemFqName);
                        q.Add("*");
                        resolutionStatus[itemFqName] = 1;
                        List<string> references = referencesMadeByFqItem[itemFqName];
                        for (int i = references.Count - 1; i >= 0; i--)
                        {
                            q.Add(references[i]);
                        }
                    }
                }
            }

            return resolutionOrder.ToArray();
        }

        private static int GetEnumMemberIndex(Resolver resolver, string memNameOrFqMemName, EnumEntity enumEntity)
        {
            int lastDot = memNameOrFqMemName.LastIndexOf('.');
            string memName = memNameOrFqMemName;
            if (lastDot != -1)
            {
                memName = memNameOrFqMemName.Substring(lastDot + 1);
            }

            for (int i = 0; i < enumEntity.memberNameTokens.Length; i++)
            {
                if (enumEntity.memberNameTokens[i].Value == memName)
                {
                    return i;
                }
            }

            return -1;
        }

        private static Expression GetListOfUnresolvedConstReferences(Resolver resolver, FileContext file, string fqNamespace, Expression expr, List<string> refsOut)
        {
            return ResolverUtil.GetListOfUnresolvedConstReferencesImpl(resolver, file, fqNamespace, expr, refsOut);
        }

        private static Expression GetListOfUnresolvedConstReferencesImpl(Resolver resolver, FileContext file, string fqNamespace, Expression expr, List<string> refs)
        {
            switch (expr.type)
            {
                case (int) ExpressionType.INTEGER_CONST:
                case (int) ExpressionType.BOOL_CONST:
                case (int) ExpressionType.FLOAT_CONST:
                case (int) ExpressionType.NULL_CONST:
                case (int) ExpressionType.STRING_CONST:
                    // This is fine
                    return expr;

                case (int) ExpressionType.BINARY_OP:
                    expr.left = ResolverUtil.GetListOfUnresolvedConstReferencesImpl(resolver, file, fqNamespace, expr.left, refs);
                    expr.right = ResolverUtil.GetListOfUnresolvedConstReferencesImpl(resolver, file, fqNamespace, expr.right, refs);
                    return expr;

                case (int) ExpressionType.VARIABLE:
                    AbstractEntity referenced = ResolverUtil.TryDoExactLookupForConstantEntity(resolver, file, fqNamespace, expr.strVal);
                    if (referenced == null)
                    {
                        FunctionWrapper.Errors_Throw(expr.firstToken, "No definition for '" + expr.strVal + "'");
                    }
                    else
                    {
                        if (referenced.fileContext.compiledModule != file.compiledModule)
                        {
                            if (referenced.type == (int)EntityType.CONST)
                            {
                                FunctionWrapper.fail("Not implemented");
                            }
                            else if (referenced.type == (int)EntityType.ENUM)
                            {
                                FunctionWrapper.fail("Not implemented");
                            }
                            else
                            {
                                FunctionWrapper.fail("Not implemented");
                            }
                        }

                        switch (referenced.type)
                        {
                            case (int)EntityType.CONST:
                                refs.Add(referenced.fqName);
                                break;
                            case (int)EntityType.ENUM:
                                throw new NotImplementedException();

                            default:
                                FunctionWrapper.Errors_Throw(expr.firstToken, "Cannot refer to this entity from a constant expression.");
                                break;
                        }
                    }
                    return expr;

                case (int) ExpressionType.DOT_FIELD:
                    string[] fullRefSegments = FunctionWrapper.DotField_getVariableRootedDottedChain(expr, "Cannot use this type of entity from a constant expression.");
                    string fullRefDotted = string.Join('.', fullRefSegments);
                    AbstractEntity reffedEntity = ResolverUtil.TryDoExactLookupForConstantEntity(resolver, file, fqNamespace, fullRefDotted);
                    if (reffedEntity == null)
                    {
                        FunctionWrapper.Errors_Throw(expr.firstToken, "Invalid expression for constant.");
                    }
                    if (reffedEntity.fileContext.compiledModule != file.compiledModule)
                    {
                        if (reffedEntity.type == (int)EntityType.CONST) return ((ConstEntity)reffedEntity.specificData).constValue;

                        if (reffedEntity.type == (int)EntityType.ENUM)
                        {
                            FunctionWrapper.fail("Not implemented");
                        }
                        else
                        {
                            FunctionWrapper.fail("Not implemented");
                        }
                    }

                    if (reffedEntity.type == (int)EntityType.CONST)
                    {
                        refs.Add(reffedEntity.fqName);
                    }
                    else if (reffedEntity.type == (int)EntityType.ENUM)
                    {
                        string enumMemberName = fullRefSegments[fullRefSegments.Length - 1];
                        string enumName = fullRefDotted.Substring(0, fullRefDotted.Length - enumMemberName.Length - 1);
                        refs.Add(reffedEntity.fqName + "." + enumMemberName);
                    }
                    else
                    {
                        FunctionWrapper.Errors_Throw(expr.firstToken, "Cannot reference this entity from here.");
                    }
                    return expr;

                default:
                    FunctionWrapper.Errors_Throw(expr.firstToken, "Invalid expression for constant.");
                    break;
            }

            return null;
        }

        // Finds the constant entity value given the current namespace and name.
        // This will find constsants and enum values. Enum parents will be ignored.
        private static AbstractEntity TryDoExactLookupForConstantEntity(Resolver resolver, FileContext file, string fqNamespace, string dottedEntityName)
        {
            // If you are using a fully-qualified name, just go with that.
            if (resolver.flattenedEntitiesNoEnumParents.ContainsKey(dottedEntityName))
            {
                return resolver.flattenedEntitiesNoEnumParents[dottedEntityName];
            }

            // Check all possible nest levels of the current namespace with the full dotted name suffixed at the end.
            // e.g. namespace = Foo.Bar, reference = Baz
            // -> check Foo.Bar.Baz
            // -> check Foo.Baz
            // no need to check just "Baz" as that was done by the previous check hence the > 0 instead of >= 0 loop condition. 
            List<string> nsParts = [.. fqNamespace.Split('.')];
            while (nsParts.Count > 0)
            {
                string lookupName = string.Join('.', [.. nsParts, .. dottedEntityName]);
                if (resolver.flattenedEntitiesNoEnumParents.ContainsKey(lookupName))
                {
                    return resolver.flattenedEntitiesNoEnumParents[lookupName];
                }
                nsParts.RemoveAt(nsParts.Count - 1);
            }

            // If we didn't find it, it means it doesn't exist in this module. So let's check the imported modules that 
            // are scoped to a variable. This means the first segment in the reference is the import variable name.
            string[] entityNameSegments = dottedEntityName.Split('.');
            if (file.importsByVar.ContainsKey(entityNameSegments[0]))
            {
                // If it was found, then use the rest of the dotted segment chain as a fully qualified
                // reference within that imported module.
                CompiledModule targetModule = file.importsByVar[entityNameSegments[0]].compiledModuleRef;
                string scopedName = string.Join('.', [.. entityNameSegments.Skip(1)]);
                return CompiledModuleEntityLookup(targetModule, scopedName);
            }

            // If it wasn't found locally or in an variable-scoped import, then check the wildcard imports.
            // In these cases, the entity name is fully qualified as-is and does not require truncation.
            foreach (ImportStatement imp in file.imports)
            {
                if (imp.importTargetVariableName == null)
                {
                    return CompiledModuleEntityLookup(imp.compiledModuleRef, dottedEntityName);
                }
            }

            return null;
        }
        private static AbstractEntity CompiledModuleEntityLookup(CompiledModule mod, string fqName)
        {
            if (mod.entitiesNoEnumParents.ContainsKey(fqName))
            {
                return mod.entitiesNoEnumParents[fqName];
            }
            string[] parts = fqName.Split('.');
            string potentialEnumParentName = string.Join('.', parts.SkipLast(1));
            if (mod.entitiesNoEnumParents.ContainsKey(potentialEnumParentName))
            {
                return mod.entitiesNoEnumParents[potentialEnumParentName];
            }

            return null;
        }

        internal static AbstractEntity DoLookup2(Resolver resolver, Token throwToken, string name)
        {
            // root level entities and namespaces always have precedence.
            if (resolver.flattenedEntities.ContainsKey(name))
            {
                return resolver.flattenedEntities[name];
            }

            if (resolver.activeEntity.fileContext.importsByVar.ContainsKey(name))
            {
                return FunctionWrapper.ModuleWrapperEntity_new(throwToken, resolver.activeEntity.fileContext.importsByVar[name]).baseData;
            }

            AbstractEntity walker = resolver.activeEntity;
            while (walker != null)
            {
                Dictionary<string, AbstractEntity> lookup = FunctionWrapper.Entity_getMemberLookup(resolver.staticCtx, walker); 
                if (lookup.ContainsKey(name))
                {
                    return lookup[name];
                }
                walker = walker.nestParent;
            }

            foreach (ImportStatement impStmnt in resolver.activeEntity.fileContext.imports)
            {
                if (impStmnt.isPollutionImport)
                {
                    if (impStmnt.compiledModuleRef.nestedEntities.ContainsKey(name))
                    {
                        return impStmnt.compiledModuleRef.nestedEntities[name];
                    }
                }
            }

            return null;
        }
    }
}
