using System;
using System.Collections.Generic;
using System.Linq;
using CommonScript.Compiler.Internal;

namespace CommonScript.Compiler
{
    internal class Resolver
    {
        public Dictionary<string, AbstractEntity> nestedEntities;
        public Dictionary<string, AbstractEntity> enumsByMemberFqName;
        public Dictionary<string, AbstractEntity> flattenedEntities;
        public Dictionary<string, AbstractEntity> flattenedEntitiesAndEnumValues;
        public Dictionary<string, AbstractEntity> flattenedEntitiesNoEnumParents;
        public List<FunctionLikeEntity> lambdas = [];

        public AbstractEntity activeEntity = null;
        public AbstractEntity[] entityList = null;
        public Statement breakContext = null;

        private ExpressionResolver expressionResolver;
        private StatementResolver statementResolver;
        private EntityResolver entityResolver;

        private StringSet extensionNames;

        public Resolver(Dictionary<string, AbstractEntity> rootEntities, List<string> extensionNames)
        {
            this.extensionNames = FunctionWrapper.StringSet_fromList(extensionNames);

            this.expressionResolver = new ExpressionResolver(this);
            this.statementResolver = new StatementResolver(this);
            this.entityResolver = new EntityResolver(this);
            this.expressionResolver.statementResolver = this.statementResolver;
            this.entityResolver.statementResolver = this.statementResolver;
            this.expressionResolver.entityResolver = this.entityResolver;
            this.statementResolver.entityResolver = this.entityResolver;
            this.statementResolver.expressionResolver = this.expressionResolver;
            this.entityResolver.expressionResolver = this.expressionResolver;

            this.nestedEntities = rootEntities;
            this.flattenedEntities = new Dictionary<string, AbstractEntity>();
            this.enumsByMemberFqName = new Dictionary<string, AbstractEntity>();
            this.flattenedEntitiesAndEnumValues = new Dictionary<string, AbstractEntity>();
            this.flattenedEntitiesNoEnumParents = new Dictionary<string, AbstractEntity>();

            this.entityList = Resolver.FlattenEntities(rootEntities);
            foreach (AbstractEntity tle in this.entityList)
            {
                this.flattenedEntities[tle.fqName] = tle;
                this.flattenedEntitiesAndEnumValues[tle.fqName] = tle;
                if (tle.type != (int)EntityType.ENUM)
                {
                    this.flattenedEntitiesNoEnumParents[tle.fqName] = tle;
                }
            }

            foreach (EnumEntity enumDef in this.entityList.Where(e => e.type == (int)EntityType.ENUM).Select(e => (EnumEntity)e.specificData))
            {
                for (int i = 0; i < enumDef.memberNameTokens.Length; i++)
                {
                    string fqName = enumDef.baseData.fqName + "." + enumDef.memberNameTokens[i].Value;
                    this.enumsByMemberFqName[fqName] = enumDef.baseData;
                    this.flattenedEntitiesAndEnumValues[fqName] = enumDef.baseData;
                    this.flattenedEntitiesNoEnumParents[fqName] = enumDef.baseData;
                }
            }
        }

        public bool isValidRegisteredExtension(string extensionName)
        {
            return FunctionWrapper.StringSet_has(this.extensionNames, extensionName);
        }

        internal static AbstractEntity[] FlattenEntities(Dictionary<string, AbstractEntity> rootEntities)
        {
            List<AbstractEntity> output = new List<AbstractEntity>();
            List<AbstractEntity> queue = new List<AbstractEntity>(rootEntities.Values);
            for (int i = 0; i < queue.Count; i++)
            {
                AbstractEntity entity = queue[i];
                output.Add(entity);

                foreach (AbstractEntity mem in AbstractEntityUtil.getMemberLookup(entity).Values)
                {
                    queue.Add(mem);
                }
            }
            return output.ToArray();
        }

        public void Resolve()
        {
            List<FunctionLikeEntity> functions = new List<FunctionLikeEntity>();
            List<ClassEntity> classes = new List<ClassEntity>();
            List<ConstEntity> constants = new List<ConstEntity>();
            List<EnumEntity> enums = new List<EnumEntity>();
            List<FunctionLikeEntity> constructors = new List<FunctionLikeEntity>();
            List<FieldEntity> fields = new List<FieldEntity>();

            AbstractEntity[] entities = this.flattenedEntities.Values.ToArray();

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
                    functions.Add((FunctionLikeEntity)tle.specificData);
                }
                else if (tle.type == (int)EntityType.CLASS)
                {
                    classes.Add((ClassEntity)tle.specificData);
                }
                else if (tle.type == (int)EntityType.CONSTRUCTOR)
                {
                    constructors.Add((FunctionLikeEntity)tle.specificData);
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

            this.AddImplicitIncrementingEnumValueDefinitions(enums);

            string[] constAndEnumResolutionOrder = this.DetermineConstAndEnumResolutionOrder(constants, enums);

            this.PerformFullResolutionPassOnConstAndEnums(constAndEnumResolutionOrder);

            ClassEntity[] orderedClasses = this.ResolveBaseClassesAndEstablishClassOrder(classes, this.flattenedEntities);

            for (int i = 0; i < orderedClasses.Length; i++)
            {
                EntityResolver.DetermineMemberOffsets(orderedClasses[i]);
            }

            for (int i = 0; i < functions.Count; i++)
            {
                this.entityResolver.ResetAutoVarId();
                this.entityResolver.ResolveFunctionFirstPass(functions[i]);
            }

            for (int i = 0; i < constructors.Count; i++)
            {
                this.entityResolver.ResetAutoVarId();
                this.entityResolver.ResolveFunctionFirstPass(constructors[i]);
            }

            // At this point, all lambdas have been repoerted.
            for (int i = 0; i < lambdas.Count; i++) // DO NOT CHANGE TO FOR-EACH. This count can grow as a result of nested lambdas.
            {
                this.entityResolver.ResetAutoVarId();
                this.entityResolver.ResolveFunctionFirstPass(lambdas[i]);
            }

            for (int i = 0; i < functions.Count; i++)
            {
                this.entityResolver.ResolveFunctionSecondPass(functions[i]);
            }

            for (int i = 0; i < constructors.Count; i++)
            {
                this.entityResolver.ResolveFunctionSecondPass(constructors[i]);
            }

            for (int i = 0; i < lambdas.Count; i++)
            {
                this.entityResolver.ResolveFunctionSecondPass(lambdas[i]);
            }
        }

        internal void ReportNewLambda(FunctionLikeEntity lamb)
        {
            this.lambdas.Add(lamb);
        }

        private ClassEntity[] ResolveBaseClassesAndEstablishClassOrder(
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
                this.activeEntity = bc.baseData;
                Token baseClassToken = bc.baseClassTokens[0];
                AbstractEntity bcEntity = this.DoLookup2(baseClassToken, baseClassToken.Value);
                if (bcEntity != null)
                {
                    for (int i = 2; i < bc.baseClassTokens.Length; i += 2)
                    {
                        string next = bc.baseClassTokens[i].Value;
                        if (bcEntity != null)
                        {
                            Dictionary<string, AbstractEntity> lookup = AbstractEntityUtil.getMemberLookup(bcEntity);
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
                this.activeEntity = null;
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

        private void PerformFullResolutionPassOnConstAndEnums(string[] resOrder)
        {
            for (int passNum = 1; passNum <= 2; passNum++)
            {
                for (int i = 0; i < resOrder.Length; i++)
                {
                    AbstractEntity entity = this.flattenedEntitiesAndEnumValues[resOrder[i]];
                    this.activeEntity = entity;
                    if (entity.type == (int)EntityType.ENUM)
                    {
                        EnumEntity enumDef = (EnumEntity)entity.specificData;
                        int memberIndex = this.GetEnumMemberIndex(resOrder[i], enumDef);
                        Expression val = enumDef.memberValues[memberIndex];
                        if (passNum == 1)
                        {
                            val = this.expressionResolver.ResolveExpressionFirstPass(val);
                        }
                        else
                        {
                            val = this.expressionResolver.ResolveExpressionSecondPass(val);
                        }

                        if (passNum == 2 && val.type != ExpressionType.INTEGER_CONST)
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
                            val = this.expressionResolver.ResolveExpressionFirstPass(val);
                        }
                        else
                        {
                            val = this.expressionResolver.ResolveExpressionSecondPass(val);
                            if (!IsExpressionConstant(val))
                            {
                                FunctionWrapper.Errors_Throw(val.firstToken, "A constant expression is required here.");
                            }
                        }

                        constEnt.constValue = val;
                    }
                    this.activeEntity = null;
                }
            }
        }

        internal static bool IsExpressionConstant(Expression expr)
        {
            switch (expr.type)
            {
                case ExpressionType.BOOL_CONST:
                case ExpressionType.NULL_CONST:
                case ExpressionType.INTEGER_CONST:
                case ExpressionType.FLOAT_CONST:
                case ExpressionType.STRING_CONST:
                case ExpressionType.ENUM_CONST:
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
            Expression varRoot = Expression.createVariable(null, root);
            return Expression.createDotField(varRoot, null, field);
        }

        // For all undefined values of an enum, make it equal to the previous value + 1 by
        // injecting EnumName.MemberName + 1
        // If the first value is undefined, inject 1
        private void AddImplicitIncrementingEnumValueDefinitions(List<EnumEntity> enums)
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
                            enumEnt.memberValues[j] = Expression.createIntegerConstant(token, 1);
                        }
                        else
                        {
                            enumEnt.memberValues[j] = Expression.createBinaryOp(
                                BuildFakeDotChain(enumEnt.baseData.simpleName, enumEnt.memberNameTokens[j - 1].Value),
                                BuildFakeToken(token, "+", true),
                                Expression.createIntegerConstant(null, 1)
                            );
                        }
                    }
                }
            }
        }

        // Returns strings because we treat the individual enum values as separate entities.
        private string[] DetermineConstAndEnumResolutionOrder(
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
                c.constValue = this.GetListOfUnresolvedConstReferences((FileContext)c.baseData.OBJ_TEMP_CAST_fileContext, ns, c.constValue, refsOut);
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
                    e.memberValues[i] = this.GetListOfUnresolvedConstReferences((FileContext)e.baseData.OBJ_TEMP_CAST_fileContext, ns, val, refsOut);
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
                    AbstractEntity item = this.flattenedEntitiesAndEnumValues[itemFqName];
                    Token itemToken = item.firstToken;
                    if (item.fqName != itemFqName) // an enum member
                    {
                        EnumEntity parentEnum = (EnumEntity)item.specificData;
                        int enumValIndex = this.GetEnumMemberIndex(itemFqName, parentEnum);
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

        private int GetEnumMemberIndex(string memNameOrFqMemName, EnumEntity enumEntity)
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

        private Expression GetListOfUnresolvedConstReferences(FileContext file, string fqNamespace, Expression expr, List<string> refsOut)
        {
            return this.GetListOfUnresolvedConstReferencesImpl(file, fqNamespace, expr, refsOut);
        }

        private Expression GetListOfUnresolvedConstReferencesImpl(FileContext file, string fqNamespace, Expression expr, List<string> refs)
        {
            switch (expr.type)
            {
                case ExpressionType.INTEGER_CONST:
                case ExpressionType.BOOL_CONST:
                case ExpressionType.FLOAT_CONST:
                case ExpressionType.NULL_CONST:
                case ExpressionType.STRING_CONST:
                    // This is fine
                    return expr;

                case ExpressionType.BINARY_OP:
                    expr.left = this.GetListOfUnresolvedConstReferencesImpl(file, fqNamespace, expr.left, refs);
                    expr.right = this.GetListOfUnresolvedConstReferencesImpl(file, fqNamespace, expr.right, refs);
                    return expr;

                case ExpressionType.VARIABLE:
                    AbstractEntity referenced = this.TryDoExactLookupForConstantEntity(file, fqNamespace, expr.strVal);
                    if (referenced == null)
                    {
                        FunctionWrapper.Errors_Throw(expr.firstToken, "No definition for '" + expr.strVal + "'");
                    }
                    else
                    {
                        if (((FileContext)referenced.OBJ_TEMP_CAST_fileContext).compiledModule != file.compiledModule)
                        {
                            if (referenced.type == (int)EntityType.CONST)
                            {
                                throw new NotImplementedException();
                            }
                            else if (referenced.type == (int)EntityType.ENUM)
                            {
                                throw new NotImplementedException();
                            }
                            else
                            {
                                throw new NotImplementedException();
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

                case ExpressionType.DOT_FIELD:
                    string[] fullRefSegments = Expression.DotField_getVariableRootedDottedChain(expr, "Cannot use this type of entity from a constant expression.");
                    string fullRefDotted = string.Join('.', fullRefSegments);
                    AbstractEntity reffedEntity = this.TryDoExactLookupForConstantEntity(file, fqNamespace, fullRefDotted);
                    if (reffedEntity == null)
                    {
                        FunctionWrapper.Errors_Throw(expr.firstToken, "Invalid expression for constant.");
                    }
                    if (((FileContext)reffedEntity.OBJ_TEMP_CAST_fileContext).compiledModule != file.compiledModule)
                    {
                        if (reffedEntity.type == (int)EntityType.CONST) return ((ConstEntity)reffedEntity.specificData).constValue;

                        if (reffedEntity.type == (int)EntityType.ENUM)
                        {
                            throw new NotImplementedException();
                        }
                        else
                        {
                            throw new NotImplementedException();
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
        private AbstractEntity TryDoExactLookupForConstantEntity(FileContext file, string fqNamespace, string dottedEntityName)
        {
            // If you are using a fully-qualified name, just go with that.
            if (this.flattenedEntitiesNoEnumParents.ContainsKey(dottedEntityName))
            {
                return this.flattenedEntitiesNoEnumParents[dottedEntityName];
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
                if (this.flattenedEntitiesNoEnumParents.ContainsKey(lookupName))
                {
                    return this.flattenedEntitiesNoEnumParents[lookupName];
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

        private AbstractEntity DoDirectLookup(string fqAttempt)
        {
            if (this.flattenedEntities.ContainsKey(fqAttempt))
            {
                return this.flattenedEntities[fqAttempt];
            }
            return null;
        }

        internal AbstractEntity DoLookup2(Token throwToken, string name)
        {
            // root level entities and namespaces always have precedence.
            if (this.flattenedEntities.ContainsKey(name))
            {
                return this.flattenedEntities[name];
            }

            if (((FileContext)this.activeEntity.OBJ_TEMP_CAST_fileContext).importsByVar.ContainsKey(name))
            {
                return new ModuleWrapperEntity(throwToken, ((FileContext)this.activeEntity.OBJ_TEMP_CAST_fileContext).importsByVar[name]).baseData;
            }

            AbstractEntity walker = this.activeEntity;
            while (walker != null)
            {
                Dictionary<string, AbstractEntity> lookup = AbstractEntityUtil.getMemberLookup(walker); 
                if (lookup.ContainsKey(name))
                {
                    return lookup[name];
                }
                walker = walker.nestParent;
            }

            foreach (ImportStatement impStmnt in ((FileContext)this.activeEntity.OBJ_TEMP_CAST_fileContext).imports)
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

        // Note that you may get an enum when passing in the fully qualified member name.
        internal AbstractEntity DoLookup(string ns, string dottedName)
        {
            string name = dottedName.Split('.')[0];

            if (ns == "") return this.DoDirectLookup(name);

            AbstractEntity e = this.DoDirectLookup(ns + "." + name);
            if (e != null) return e;

            string[] parts = ns.Split('.');
            for (int i = 1; i < parts.Length; i++)
            {
                parts[i] = parts[i - 1] + "." + parts[i];
            }

            for (int i = parts.Length - 1; i >= 0; i--)
            {
                e = this.DoDirectLookup(parts[i] + "." + name);
                if (e != null) return e;
            }

            return null;
        }

        internal string GetCurrentNamespace()
        {
            if (this.activeEntity.nestParent == null) return "";
            return this.activeEntity.nestParent.fqName;
        }
    }
}
